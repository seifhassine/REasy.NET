using System.Buffers.Binary;
using System.Text;
using REasy.Core.Formats.Rsz.Common;
using REasy.Core.Formats.Rsz.Interfaces;
using REasy.Core.Utils;

namespace REasy.Core.Formats.Rsz.Parsers
{
    public class RszInstanceParser
    {
        private readonly RszFile _rszFile;
        private readonly ITypeRegistry _typeRegistry;
        private readonly Dictionary<int, string> _typeNameCache = new();
        private readonly Dictionary<int, RszRSZUserDataInfo> _userDataDict;
        private readonly Dictionary<string, StructDefinition> _structCache = new();
        private readonly HashSet<int> _gameObjects;
        private readonly HashSet<int> _folders;

        public RszInstanceParser(RszFile rszFile)
        {
            _rszFile = rszFile ?? throw new ArgumentNullException(nameof(rszFile));
            _typeRegistry = rszFile.TypeRegistry ?? throw new ArgumentException("Invalid TypeRegistry");
            _userDataDict = rszFile.RszUserDataInfos.ToDictionary(x => (int)x.InstanceId);
            _gameObjects = new HashSet<int>(rszFile.GameObjectInstanceIds);
            _folders = new HashSet<int>(rszFile.FolderInstanceIds);
        }

        public void ParseInstances()
        {
            var instances = _rszFile.InstanceInfos;
            var parsed = _rszFile.ParsedElements;
            var hierarchy = _rszFile.InstanceHierarchy;

            parsed.Clear();
            parsed[0] = new Dictionary<string, object>();
            
            foreach (var idx in Enumerable.Range(0, instances.Count))
                hierarchy[idx] = new InstanceHierarchyInfo();

            int offset = 0;
            var dataSpan = _rszFile.Data.AsSpan();

            for (int idx = 1; idx < instances.Count; idx++)
            {
                if (_userDataDict.ContainsKey(idx))
                {
                    parsed[idx] = new Dictionary<string, object>();
                    continue;
                }

                var instance = instances[idx];
                if (!_typeNameCache.TryGetValue((int)instance.TypeId, out var typeName))
                {
                    typeName = _typeRegistry.GetTypeInfo(instance.TypeId)
                        ?.GetValueOrDefault("name")?.ToString() ?? "Unknown";
                    _typeNameCache[(int)instance.TypeId] = typeName;
                }

                var fields = _typeRegistry.GetTypeFields(instance.TypeId)
                    .ConvertAll(ParseFieldDefinition);

                parsed[idx] = new Dictionary<string, object>(fields.Count);
                var result = ParseFields(dataSpan, offset, fields, idx);
                offset = result.offset;
                
                foreach (var (key, value) in result.fields)
                    parsed[idx][key] = value;
            }
        }

        private (int offset, Dictionary<string, object> fields) ParseFields(
            ReadOnlySpan<byte> data, int offset, List<FieldDefinition> fields, int instanceId)
        {
            var results = new Dictionary<string, object>(fields.Count);
            foreach (var field in fields)
            {
                var result = ParseField(data, offset, field, instanceId);
                offset = result.offset;
                results[field.Name] = result.value;
            }

            return (offset, results);
        }

        private (int offset, object value) ParseField(
        ReadOnlySpan<byte> data, int offset, FieldDefinition field, int instanceId)
        {
            var typeClass = RszTypeResolver.GetTypeClass(field);
            if (field.IsArray)
            {
                offset = IOUtils.Align(offset, 4);
                uint count = BitConverter.ToUInt32(data.Slice(offset, 4));
                offset += 4;
                return ParseArray(data, offset, field, count, instanceId, typeClass);
            }
            offset = IOUtils.Align(offset, field.Alignment);
            return ParseValue(data, offset, field, instanceId, typeClass);
        }

        
        private (int offset, object value) ParseArray(
            ReadOnlySpan<byte> data, int offset, FieldDefinition field, uint count, int instanceId, Type typeClass)
        {
            object value;
            switch (typeClass.Name)
            {
                case nameof(StructData):
                    var structDef = GetStructDefinition(field.OriginalType);
                    var structs = new List<Dictionary<string, object>>((int)count);
                    for (int i = 0; i < count; i++)
                    {
                        var result = ParseFields(data, offset, structDef.Fields, instanceId);
                        offset = result.offset;
                        structs.Add(result.fields);
                    }
                    value = new StructData(structs, field.OriginalType);
                    break;

                case nameof(MaybeObject):
                    bool isObjectArray = count > 0 && IsValidReference(
                        BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4)),
                        instanceId);
                    if (isObjectArray)
                    {
                        var objects = new ObjectData[count];
                        for (int i = 0; i < count; i++)
                        {
                            offset = IOUtils.Align(offset, field.Alignment);
                            int refId = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4));
                            offset += 4;
                            if (IsValidReference(refId, instanceId))
                                AddHierarchy(instanceId, refId);
                            objects[i] = new ObjectData(refId);
                        }
                        value = new ArrayData(objects.Cast<object>().ToList(), typeClass, field.OriginalType);
                    }
                    else
                    {
                        var bytes = new RawBytesData[count];
                        for (int i = 0; i < count; i++)
                        {
                            offset = IOUtils.Align(offset, field.Alignment);
                            bytes[i] = new RawBytesData(data.Slice(offset, field.Size).ToArray(), field.Size);
                            offset += field.Size;
                        }
                        value = new ArrayData(bytes.Cast<object>().ToList(), typeof(RawBytesData), field.OriginalType);
                    }
                    break;

                case nameof(UserDataData):
                    var userData = new UserDataData[count];
                    for (int i = 0; i < count; i++)
                    {
                        offset = IOUtils.Align(offset, field.Alignment);
                        int dataId = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4));
                        offset += 4;
                        userData[i] = new UserDataData(
                            _userDataDict.TryGetValue(dataId, out var info)
                                ? _rszFile.GetRszUserDataString(info)
                                : string.Empty,
                            dataId);
                    }
                    value = new ArrayData(userData.Cast<object>().ToList(), typeClass, field.OriginalType);
                    break;

                default:
                    var items = new object[count];
                    for (int i = 0; i < count; i++)
                    {
                        offset = IOUtils.Align(offset, field.Alignment);
                        var result = ParseValue(data, offset, field, instanceId, typeClass);
                        offset = result.offset;
                        items[i] = result.value;
                    }
                    value = new ArrayData(items.ToList(), typeClass, field.OriginalType);
                    break;
            }
            return (offset, value);
        }

        
        private (int offset, object value) ParseValue(
            ReadOnlySpan<byte> data, int offset, FieldDefinition field, int instanceId, Type typeClass)
        {
            object value;
            if (typeClass == typeof(BoolData))
            {
                value = new BoolData(data[offset] != 0);
                offset += 1;
            }
            else if (typeClass == typeof(S8Data))
            {
                value = new S8Data((sbyte)data[offset]);
                offset += 1;
            }
            else if (typeClass == typeof(U8Data))
            {
                value = new U8Data(data[offset]);
                offset += 1;
            }
            else if (typeClass == typeof(S16Data))
            {
                value = new S16Data(BinaryPrimitives.ReadInt16LittleEndian(data.Slice(offset, 2)));
                offset += 2;
            }
            else if (typeClass == typeof(U16Data))
            {
                value = new U16Data(BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2)));
                offset += 2;
            }
            else if (typeClass == typeof(S32Data))
            {
                value = new S32Data(BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4)));
                offset += 4;
            }
            else if (typeClass == typeof(U32Data))
            {
                value = new U32Data(BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)));
                offset += 4;
            }
            else if (typeClass == typeof(S64Data))
            {
                value = new S64Data(BinaryPrimitives.ReadInt64LittleEndian(data.Slice(offset, 8)));
                offset += 8;
            }
            else if (typeClass == typeof(U64Data))
            {
                value = new U64Data(BinaryPrimitives.ReadUInt64LittleEndian(data.Slice(offset, 8)));
                offset += 8;
            }
            else if (typeClass == typeof(F32Data))
            {
                value = new F32Data(BinaryPrimitives.ReadSingleLittleEndian(data.Slice(offset, 4)));
                offset += 4;
            }
            else if (typeClass == typeof(F64Data))
            {
                value = new F64Data(BinaryPrimitives.ReadDoubleLittleEndian(data.Slice(offset, 8)));
                offset += 8;
            }
            else if (typeClass == typeof(StringData))
            {
                offset = IOUtils.Align(offset, 4);
                int length = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4)) * 2;
                offset += 4;
                value = new StringData(Encoding.Unicode.GetString(data.Slice(offset, length)).TrimEnd('\0'));
                offset += length;
            }
            else if (typeClass == typeof(RuntimeTypeData))
            {
                offset = IOUtils.Align(offset, 4);
                int utf8Length = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4));
                offset += 4;
                value = new RuntimeTypeData(Encoding.UTF8.GetString(data.Slice(offset, utf8Length)).TrimEnd('\0'));
                offset += utf8Length;
            }
            else if (typeClass == typeof(Vec2Data) || typeClass == typeof(Float2Data))
            {
                float x = BinaryPrimitives.ReadSingleLittleEndian(data.Slice(offset, 4));
                float y = BinaryPrimitives.ReadSingleLittleEndian(data.Slice(offset + 4, 4));
                value = typeClass == typeof(Vec2Data)
                    ? new Vec2Data(x, y)
                    : new Float2Data(x, y);
                offset += field.Size;
            }
            else if (typeClass == typeof(Vec3Data) || typeClass == typeof(Float3Data) || typeClass == typeof(Vec3ColorData))
            {
                float x = BinaryPrimitives.ReadSingleLittleEndian(data.Slice(offset, 4));
                float y = BinaryPrimitives.ReadSingleLittleEndian(data.Slice(offset + 4, 4));
                float z = BinaryPrimitives.ReadSingleLittleEndian(data.Slice(offset + 8, 4));
                value = typeClass switch
                {
                    _ when typeClass == typeof(Vec3Data) => new Vec3Data(x, y, z),
                    _ when typeClass == typeof(Vec3ColorData) => new Vec3ColorData(x, y, z),
                    _ => new Float3Data(x, y, z)
                };
                offset += field.Size;
            }
            else if (typeClass == typeof(Vec4Data) || typeClass == typeof(Float4Data) || typeClass == typeof(QuaternionData))
            {
                float x = BinaryPrimitives.ReadSingleLittleEndian(data.Slice(offset, 4));
                float y = BinaryPrimitives.ReadSingleLittleEndian(data.Slice(offset + 4, 4));
                float z = BinaryPrimitives.ReadSingleLittleEndian(data.Slice(offset + 8, 4));
                float w = BinaryPrimitives.ReadSingleLittleEndian(data.Slice(offset + 12, 4));
                value = typeClass switch
                {
                    _ when typeClass == typeof(Vec4Data) => new Vec4Data(x, y, z, w),
                    _ when typeClass == typeof(QuaternionData) => new QuaternionData(x, y, z, w),
                    _ => new Float4Data(x, y, z, w)
                };
                offset += field.Size;
            }
            else if (typeClass == typeof(ColorData))
            {
                value = new ColorData(data[offset], data[offset + 1], data[offset + 2], data[offset + 3]);
                offset += 4;
            }
            else if (typeClass == typeof(RangeData))
            {
                float min = BinaryPrimitives.ReadSingleLittleEndian(data.Slice(offset, 4));
                float max = BinaryPrimitives.ReadSingleLittleEndian(data.Slice(offset + 4, 4));
                value = new RangeData(min, max);
                offset += field.Size;
            }
            else if (typeClass == typeof(RangeIData))
            {
                int min = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4));
                int max = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset + 4, 4));
                value = new RangeIData(min, max);
                offset += field.Size;
            }
            else if (typeClass == typeof(Mat4Data))
            {
                var matrix = new float[16];
                for (int i = 0; i < 16; i++)
                    matrix[i] = BinaryPrimitives.ReadSingleLittleEndian(data.Slice(offset + i * 4, 4));
                value = new Mat4Data(matrix.ToList());
                offset += field.Size;
            }
            else if (typeClass == typeof(OBBData))
            {
                var values = new float[20];
                for (int i = 0; i < 20; i++)
                    values[i] = BinaryPrimitives.ReadSingleLittleEndian(data.Slice(offset + i * 4, 4));
                value = new OBBData(values.ToList());
                offset += field.Size;
            }
            else if (typeClass == typeof(CapsuleData))
            {
                var start = new Vec3Data(
                    BinaryPrimitives.ReadSingleLittleEndian(data.Slice(offset, 4)),
                    BinaryPrimitives.ReadSingleLittleEndian(data.Slice(offset + 4, 4)),
                    BinaryPrimitives.ReadSingleLittleEndian(data.Slice(offset + 8, 4))
                );
                var end = new Vec3Data(
                    BinaryPrimitives.ReadSingleLittleEndian(data.Slice(offset + 16, 4)),
                    BinaryPrimitives.ReadSingleLittleEndian(data.Slice(offset + 20, 4)),
                    BinaryPrimitives.ReadSingleLittleEndian(data.Slice(offset + 24, 4))
                );
                float radius = BinaryPrimitives.ReadSingleLittleEndian(data.Slice(offset + 32, 4));
                value = new CapsuleData(start, end, radius);
                offset += field.Size;
            }
            else if (typeClass == typeof(GuidData) || typeClass == typeof(GameObjectRefData))
            {
                var guidBytes = data.Slice(offset, 16).ToArray();
                string guidString = new Guid(guidBytes).ToString();
                value = typeClass == typeof(GuidData)
                    ? new GuidData(guidString, guidBytes)
                    : new GameObjectRefData(guidString, guidBytes);
                offset += field.Size;
            }
            else if (typeClass == typeof(ObjectData))
            {
                int refId = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4));
                if (IsValidReference(refId, instanceId))
                    AddHierarchy(instanceId, refId);
                value = new ObjectData(refId);
                offset += field.Size;
            }
            else if (typeClass == typeof(UserDataData))
            {
                int dataId = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4));
                _userDataDict.TryGetValue(dataId, out var info);
                value = new UserDataData(info != null 
                    ? _rszFile.GetRszUserDataString(info) 
                    : string.Empty, dataId);
                offset += field.Size;
            }
            else
            {
                var bytes = data.Slice(offset, field.Size).ToArray();
                value = new RawBytesData(bytes, field.Size);
                offset += field.Size;
            }
            return (offset, value);
        }
        private StructDefinition GetStructDefinition(string typeName)
        {
            if (!_structCache.TryGetValue(typeName, out var def))
            {
                var (typeInfo, _) = _typeRegistry.FindTypeByName(typeName);
                var fields = typeInfo?.GetValueOrDefault("fields") as List<Dictionary<string, object>> 
                    ?? new List<Dictionary<string, object>>();
                def = new StructDefinition(
                    fields.ConvertAll(ParseFieldDefinition),
                    typeInfo != null ? IOUtils.GetIntValue(typeInfo, "size", 0) : 4
                );
                _structCache[typeName] = def;
            }
            return def;
        }

        private FieldDefinition ParseFieldDefinition(Dictionary<string, object> field)
        {
            return new FieldDefinition(
                name: field.GetValueOrDefault("name")?.ToString() ?? "<unnamed>",
                type: field.GetValueOrDefault("type")?.ToString()?.ToLowerInvariant() ?? "unknown",
                size: IOUtils.GetIntValue(field, "size", 4),
                isNative: IOUtils.GetBoolValue(field, "native"),
                isArray: IOUtils.GetBoolValue(field, "array"),
                originalType: field.GetValueOrDefault("original_type")?.ToString() ?? "",
                alignment: IOUtils.GetIntValue(field, "align", 1)
            );
        }

        private bool IsValidReference(int refId, int currentId) => 
            refId > 0 && refId < currentId && 
            !_gameObjects.Contains(refId) && 
            !_folders.Contains(refId);

        private void AddHierarchy(int parentId, int childId)
        {
            if (_rszFile.InstanceHierarchy.TryGetValue(parentId, out var parent) &&
                _rszFile.InstanceHierarchy.TryGetValue(childId, out var child))
            {
                parent.Children.Add(childId);
                child.Parent = parentId;
            }
        }

    }
}