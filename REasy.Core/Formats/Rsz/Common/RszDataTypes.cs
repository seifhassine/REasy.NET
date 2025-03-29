namespace REasy.Core.Formats.Rsz
{
    public class ArrayData
    {
        public List<object> Values { get; set; }
        public Type ElementType { get; set; }
        public string OrigType { get; set; }

        public ArrayData(List<object> values, Type elementType, string origType = "")
        {
            Values = values ?? new List<object>();
            ElementType = elementType;
            OrigType = origType;
        }

        public int AddElement(object element)
        {
            if (ElementType != null && !ElementType.IsInstanceOfType(element))
            {
                throw new ArgumentException($"Expected {ElementType.Name}, got {element.GetType().Name}");
            }

            Values.Add(element);
            return Values.Count - 1;
        }

        public override string ToString()
        {
            return $"Array[{ElementType?.Name ?? "unknown"}]({Values.Count} items)";
        }
    }
    public class StructData
    {
        public List<Dictionary<string, object>> Values { get; set; }
        public string OrigType { get; set; }

        public StructData(List<Dictionary<string, object>> values, string origType = "")
        {
            Values = values ?? new List<Dictionary<string, object>>();
            OrigType = origType;
        }

        public int AddElement(Dictionary<string, object> element)
        {
            Values.Add(element);
            return Values.Count - 1;
        }

        public override string ToString()
        {
            return $"Struct({Values.Count} items){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class ObjectData
    {
        public int Value { get; set; }
        public string OrigType { get; set; }

        public ObjectData(int value = 0, string origType = "")
        {
            Value = value;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"Object({Value}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class ResourceData
    {
        public int Value { get; set; }
        public string OrigType { get; set; }

        public ResourceData(int value = 0, string origType = "")
        {
            Value = value;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"Resource({Value}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class UserDataData
    {
        public string Value { get; set; }
        public int Index { get; set; }
        public string OrigType { get; set; }

        public UserDataData(string value = "", int index = 0, string origType = "")
        {
            Value = value;
            Index = index;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"UserData({Value}, {Index}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class BoolData
    {
        public bool Value { get; set; }
        public string OrigType { get; set; }

        public BoolData(bool value = false, string origType = "")
        {
            Value = value;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"{Value}{(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class S8Data
    {
        public sbyte Value { get; set; }
        public string OrigType { get; set; }

        public S8Data(sbyte value = 0, string origType = "")
        {
            Value = value;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"{Value}{(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class U8Data
    {
        public byte Value { get; set; }
        public string OrigType { get; set; }

        public U8Data(byte value = 0, string origType = "")
        {
            Value = value;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"{Value}{(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class S16Data
    {
        public short Value { get; set; }
        public string OrigType { get; set; }

        public S16Data(short value = 0, string origType = "")
        {
            Value = value;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"{Value}{(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class U16Data
    {
        public ushort Value { get; set; }
        public string OrigType { get; set; }

        public U16Data(ushort value = 0, string origType = "")
        {
            Value = value;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"{Value}{(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class S32Data
    {
        public int Value { get; set; }
        public string OrigType { get; set; }

        public S32Data(int value = 0, string origType = "")
        {
            Value = value;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"{Value}{(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class U32Data
    {
        public uint Value { get; set; }
        public string OrigType { get; set; }

        public U32Data(uint value = 0, string origType = "")
        {
            Value = value;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"{Value}{(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class S64Data
    {
        public long Value { get; set; }
        public string OrigType { get; set; }

        public S64Data(long value = 0, string origType = "")
        {
            Value = value;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"{Value}{(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class U64Data
    {
        public ulong Value { get; set; }
        public string OrigType { get; set; }

        public U64Data(ulong value = 0, string origType = "")
        {
            Value = value;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"{Value}{(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class F32Data
    {
        public float Value { get; set; }
        public string OrigType { get; set; }

        public F32Data(float value = 0, string origType = "")
        {
            Value = value;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"{Value.ToString("F6")}{(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class F64Data
    {
        public double Value { get; set; }
        public string OrigType { get; set; }

        public F64Data(double value = 0, string origType = "")
        {
            Value = value;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"{Value.ToString("F6")}{(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class StringData
    {
        public string Value { get; set; }
        public string OrigType { get; set; }

        public StringData(string value = "", string origType = "")
        {
            Value = value;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"\"{Value}\"{(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class Uint2Data
    {
        public uint X { get; set; }
        public uint Y { get; set; }
        public string OrigType { get; set; }

        public Uint2Data(uint x = 0, uint y = 0, string origType = "")
        {
            X = x;
            Y = y;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"UINT2({X}, {Y}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class Uint3Data
    {
        public uint X { get; set; }
        public uint Y { get; set; }
        public uint Z { get; set; }
        public string OrigType { get; set; }

        public Uint3Data(uint x = 0, uint y = 0, uint z = 0, string origType = "")
        {
            X = x;
            Y = y;
            Z = z;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"UINT3({X}, {Y}, {Z}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class Int2Data
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string OrigType { get; set; }

        public Int2Data(int x = 0, int y = 0, string origType = "")
        {
            X = x;
            Y = y;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"INT2({X}, {Y}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class Int3Data
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public string OrigType { get; set; }

        public Int3Data(int x = 0, int y = 0, int z = 0, string origType = "")
        {
            X = x;
            Y = y;
            Z = z;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"INT3({X}, {Y}, {Z}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class Int4Data
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int W { get; set; }
        public string OrigType { get; set; }

        public Int4Data(int x = 0, int y = 0, int z = 0, int w = 0, string origType = "")
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"INT4({X}, {Y}, {Z}, {W}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class Float2Data
    {
        public float X { get; set; }
        public float Y { get; set; }
        public string OrigType { get; set; }

        public Float2Data(float x = 0, float y = 0, string origType = "")
        {
            X = x;
            Y = y;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"FLOAT2({X.ToString("F6")}, {Y.ToString("F6")}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class Float3Data
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public string OrigType { get; set; }

        public Float3Data(float x = 0, float y = 0, float z = 0, string origType = "")
        {
            X = x;
            Y = y;
            Z = z;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"FLOAT3({X.ToString("F6")}, {Y.ToString("F6")}, {Z.ToString("F6")}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class Float4Data
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }
        public string OrigType { get; set; }

        public Float4Data(float x = 0, float y = 0, float z = 0, float w = 0, string origType = "")
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"FLOAT4({X.ToString("F6")}, {Y.ToString("F6")}, {Z.ToString("F6")}, {W.ToString("F6")}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class Mat4Data
    {
        public List<float> Values { get; set; }
        public string OrigType { get; set; }

        public Mat4Data(IEnumerable<float> values, string origType = "")
        {
            Values = new List<float>();
            if (values != null)
            {
                Values.AddRange(values.Take(16));
            }
            
            while (Values.Count < 16)
            {
                Values.Add(0.0f);
            }
            
            OrigType = origType;
        }

        public float this[int index]
        {
            get => Values[index];
            set => Values[index] = value;
        }

        public override string ToString()
        {
            return $"MAT4({string.Join(", ", Values.Select(v => v.ToString("F6")))})";
        }
    }

    public class Vec2Data
    {
        public float X { get; set; }
        public float Y { get; set; }
        public string OrigType { get; set; }

        public Vec2Data(float x = 0, float y = 0, string origType = "")
        {
            X = x;
            Y = y;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"VEC2({X.ToString("F6")}, {Y.ToString("F6")}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class Vec3Data
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public string OrigType { get; set; }

        public Vec3Data(float x = 0, float y = 0, float z = 0, string origType = "")
        {
            X = x;
            Y = y;
            Z = z;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"VEC3({X.ToString("F6")}, {Y.ToString("F6")}, {Z.ToString("F6")}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class Vec3ColorData
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public string OrigType { get; set; }

        public Vec3ColorData(float x = 0, float y = 0, float z = 0, string origType = "")
        {
            X = x;
            Y = y;
            Z = z;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"VEC3COLOR({X.ToString("F6")}, {Y.ToString("F6")}, {Z.ToString("F6")}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class Vec4Data
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }
        public string OrigType { get; set; }

        public Vec4Data(float x = 0, float y = 0, float z = 0, float w = 0, string origType = "")
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"VEC4({X.ToString("F6")}, {Y.ToString("F6")}, {Z.ToString("F6")}, {W.ToString("F6")}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class QuaternionData
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }
        public string OrigType { get; set; }

        public QuaternionData(float x = 0, float y = 0, float z = 0, float w = 0, string origType = "")
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"QUAT({X.ToString("F6")}, {Y.ToString("F6")}, {Z.ToString("F6")}, {W.ToString("F6")}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class GuidData
    {
        public string GuidStr { get; set; }
        public byte[] RawBytes { get; set; }
        public string OrigType { get; set; }

        public GuidData(string guidStr, byte[] rawBytes, string origType = "")
        {
            GuidStr = guidStr ?? "00000000-0000-0000-0000-000000000000";
            RawBytes = rawBytes ?? new byte[16];
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"GUID({GuidStr}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }

        public string ReturnClass()
        {
            return GetType().Name;
        }
    }

    public class ColorData
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public int A { get; set; }
        public string OrigType { get; set; }

        public ColorData(int r = 0, int g = 0, int b = 0, int a = 0, string origType = "")
        {
            R = r;
            G = g;
            B = b;
            A = a;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"COLOR({R}, {G}, {B}, {A}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class AABBData
    {
        public Vec3Data Min { get; set; }
        public Vec3Data Max { get; set; }
        public string OrigType { get; set; }

        public AABBData(float minX = 0, float minY = 0, float minZ = 0,
                       float maxX = 0, float maxY = 0, float maxZ = 0, string origType = "")
        {
            Min = new Vec3Data(minX, minY, minZ);
            Max = new Vec3Data(maxX, maxY, maxZ);
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"AABB(Min:{Min}, Max:{Max}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class CapsuleData
    {
        public Vec3Data Start { get; set; }
        public Vec3Data End { get; set; }
        public float Radius { get; set; }
        public string OrigType { get; set; }

        public CapsuleData(Vec3Data start, Vec3Data end, float radius = 0, string origType = "")
        {
            Start = start ?? new Vec3Data();
            End = end ?? new Vec3Data();
            Radius = radius;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"CAPSULE(Start:{Start}, End:{End}, Radius:{Radius.ToString("F6")}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class ConeData
    {
        public Vec3Data Position { get; set; }
        public Vec3Data Direction { get; set; }
        public float Angle { get; set; }
        public float Distance { get; set; }
        public string OrigType { get; set; }

        public ConeData(Vec3Data position, Vec3Data direction, float angle = 0, float distance = 0, string origType = "")
        {
            Position = position ?? new Vec3Data();
            Direction = direction ?? new Vec3Data();
            Angle = angle;
            Distance = distance;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"CONE(Pos:{Position}, Dir:{Direction}, Angle:{Angle.ToString("F6")}, Dist:{Distance.ToString("F6")}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class LineSegmentData
    {
        public Vec3Data Start { get; set; }
        public Vec3Data End { get; set; }
        public string OrigType { get; set; }

        public LineSegmentData(Vec3Data start, Vec3Data end, string origType = "")
        {
            Start = start ?? new Vec3Data();
            End = end ?? new Vec3Data();
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"LINESEGMENT(Start:{Start}, End:{End}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class OBBData
    {
        public List<float> Values { get; set; }
        public string OrigType { get; set; }

        public OBBData(IEnumerable<float> values, string origType = "")
        {
            Values = new List<float>();
            if (values != null)
            {
                Values.AddRange(values.Take(20));
            }
            
            while (Values.Count < 20)
            {
                Values.Add(0.0f);
            }
            
            OrigType = origType;
        }

        public float this[int index]
        {
            get => Values[index];
            set => Values[index] = value;
        }

        public override string ToString()
        {
            return $"OBB({string.Join(", ", Values.Select(v => v.ToString("F6")))})";
        }
    }

    public class PointData
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public string OrigType { get; set; }

        public PointData(float x = 0, float y = 0, float z = 0, string origType = "")
        {
            X = x;
            Y = y;
            Z = z;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"POINT({X.ToString("F6")}, {Y.ToString("F6")}, {Z.ToString("F6")}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class RangeData
    {
        public float Min { get; set; }
        public float Max { get; set; }
        public string OrigType { get; set; }

        public RangeData(float min = 0, float max = 0, string origType = "")
        {
            Min = min;
            Max = max;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"RANGE({Min.ToString("F6")}, {Max.ToString("F6")}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class RangeIData
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public string OrigType { get; set; }

        public RangeIData(int min = 0, int max = 0, string origType = "")
        {
            Min = min;
            Max = max;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"RANGEI({Min}, {Max}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class SizeData
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public string OrigType { get; set; }

        public SizeData(float width = 0, float height = 0, string origType = "")
        {
            Width = width;
            Height = height;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"SIZE({Width.ToString("F6")}, {Height.ToString("F6")}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class SphereData
    {
        public Vec3Data Center { get; set; }
        public float Radius { get; set; }
        public string OrigType { get; set; }

        public SphereData(Vec3Data center, float radius = 0, string origType = "")
        {
            Center = center ?? new Vec3Data();
            Radius = radius;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"SPHERE(Center:{Center}, Radius:{Radius.ToString("F6")}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class CylinderData
    {
        public Vec3Data Center { get; set; }
        public float Radius { get; set; }
        public float Height { get; set; }
        public string OrigType { get; set; }

        public CylinderData(Vec3Data center, float radius = 0, float height = 0, string origType = "")
        {
            Center = center ?? new Vec3Data();
            Radius = radius;
            Height = height;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"CYLINDER(Center:{Center}, Radius:{Radius.ToString("F6")}, Height:{Height.ToString("F6")}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class AreaData
    {
        public Vec2Data Min { get; set; }
        public Vec2Data Max { get; set; }
        public string OrigType { get; set; }

        public AreaData(Vec2Data min, Vec2Data max, string origType = "")
        {
            Min = min ?? new Vec2Data();
            Max = max ?? new Vec2Data();
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"AREA(Min:{Min}, Max:{Max}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class RectData
    {
        public float MinX { get; set; }
        public float MinY { get; set; }
        public float MaxX { get; set; }
        public float MaxY { get; set; }
        public string OrigType { get; set; }

        public RectData(float minX = 0, float minY = 0, float maxX = 0, float maxY = 0, string origType = "")
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"RECT({MinX.ToString("F6")}, {MinY.ToString("F6")}, {MaxX.ToString("F6")}, {MaxY.ToString("F6")}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class GameObjectRefData
    {
        public string GuidStr { get; set; }
        public byte[] RawBytes { get; set; }
        public string OrigType { get; set; }

        public GameObjectRefData(string guidStr = "", byte[]? rawBytes = null, string origType = "")
        {
            GuidStr = string.IsNullOrEmpty(guidStr) ? "00000000-0000-0000-0000-000000000000" : guidStr;
            RawBytes = rawBytes ?? new byte[16];
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"GAMEOBJECTREF({GuidStr}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class RuntimeTypeData
    {
        public string Value { get; set; }
        public string OrigType { get; set; }

        public RuntimeTypeData(string value = "", string origType = "")
        {
            Value = value;
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"RUNTIMETYPE({Value}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class MaybeObject
    {
        public string OrigType { get; set; }

        public MaybeObject(string origType = "")
        {
            OrigType = origType;
        }

        public override string ToString()
        {
            return $"MAYBEOBJECT{(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }

    public class RawBytesData
    {
        public byte[] RawBytes { get; set; }
        public int FieldSize { get; set; }
        public string OrigType { get; set; }

        public RawBytesData(byte[] rawBytes, int fieldSize = 1, string origType = "")
        {
            RawBytes = rawBytes ?? Array.Empty<byte>();
            FieldSize = fieldSize;
            OrigType = origType;
        }

        public override string ToString()
        {
            string bytesStr = RawBytes.Length <= 8 
                ? string.Join(" ", RawBytes.Select(b => b.ToString("X2")))
                : string.Join(" ", RawBytes.Take(8).Select(b => b.ToString("X2"))) + "...";
            return $"RAWBYTES[{RawBytes.Length}]({bytesStr}){(string.IsNullOrEmpty(OrigType) ? "" : $" [{OrigType}]")}";
        }
    }
}