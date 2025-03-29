using REasy.Core.Formats.Rsz.Common;

namespace REasy.Core.Formats.Rsz
{
    public static class RszTypeResolver
    {
        public static readonly Dictionary<string, Type> TYPE_MAPPING = new Dictionary<string, Type>
        {
            { "bool", typeof(BoolData) },
            { "s32", typeof(S32Data) },
            { "int", typeof(S32Data) },
            { "uint", typeof(U32Data) },
            { "f32", typeof(F32Data) },
            { "f64", typeof(F64Data) },
            { "float", typeof(F32Data) },
            { "string", typeof(StringData) },
            { "resource", typeof(StringData) },
            { "gameobjectref", typeof(GameObjectRefData) },
            { "object", typeof(ObjectData) },
            { "vec3", typeof(Vec3Data) },
            { "vec4", typeof(Vec4Data) },
            { "obb", typeof(OBBData) },
            { "userdata", typeof(UserDataData) },
            { "vec2", typeof(Vec2Data) },
            { "uint2", typeof(Uint2Data) },
            { "uint3", typeof(Uint3Data) },
            { "u8", typeof(U8Data) },
            { "u16", typeof(U16Data) },
            { "u32", typeof(U32Data) },
            { "u64", typeof(U64Data) },
            { "sphere", typeof(SphereData) },
            { "size", typeof(SizeData) },
            { "s8", typeof(S8Data) },
            { "s16", typeof(S16Data) },
            { "s64", typeof(S64Data) },
            { "runtimetype", typeof(RuntimeTypeData) },
            { "rect", typeof(RectData) },
            { "range", typeof(RangeData) },
            { "rangei", typeof(RangeIData) },
            { "quaternion", typeof(QuaternionData) },
            { "point", typeof(PointData) },
            { "mat4", typeof(Mat4Data) },
            { "linesegment", typeof(LineSegmentData) },
            { "int2", typeof(Int2Data) },
            { "int3", typeof(Int3Data) },
            { "int4", typeof(Int4Data) },
            { "guid", typeof(GuidData) },
            { "float2", typeof(Float2Data) },
            { "float3", typeof(Float3Data) },
            { "float4", typeof(Float4Data) },
            { "cylinder", typeof(CylinderData) },
            { "cone", typeof(ConeData) },
            { "color", typeof(ColorData) },
            { "capsule", typeof(CapsuleData) },
            { "area", typeof(AreaData) },
            { "aabb", typeof(AABBData) },
            { "data", typeof(RawBytesData) },
            { "struct", typeof(StructData) },
        };

        public static Type GetTypeClass(
            FieldDefinition field)
        {
            
            string fieldType = field.Type.ToLowerInvariant(); 
            int fieldSize = field.Size;
            bool isNative = field.IsNative;
            bool isArray = field.IsArray;
            int align = field.Alignment; 
            string originalType = field.OriginalType.ToLowerInvariant();
            string fieldName = field.Name.ToLowerInvariant();

            if (fieldType == "data")
            {
                if (fieldSize == 16)
                {
                    if (align == 8 && isNative)
                    {
                        return typeof(GuidData);
                    }
                    else
                    {
                        return typeof(Vec4Data);
                    }
                }
                else if (fieldSize == 80)
                {
                    return typeof(OBBData);
                }
                else if (fieldSize == 64 && align == 16)
                {
                    return typeof(Mat4Data);
                }
                else if (fieldSize == 48 && align == 16)
                {
                    return typeof(CapsuleData);
                }
                else if (fieldSize == 4 && isNative)
                {
                    return typeof(MaybeObject);
                }
                else if (fieldSize == 1)
                {
                    return typeof(U8Data);
                }
            }

            if (fieldType == "obb" && fieldSize == 16)
            {
                return typeof(Vec4Data);
            }
            
            if (fieldType == "uri" && originalType.Contains("gameobjectref"))
            {
                return typeof(GameObjectRefData);
            }
            
            if (fieldType == "point" && originalType.Contains("range"))
            {
                return typeof(RangeData);
            }
            
            if (fieldType == "vec3" && fieldName.ToLower().Contains("color"))
            {
                return typeof(Vec3ColorData);
            }
                
            if (isArray && isNative && fieldSize == 4 && (fieldType == "s32" || fieldType == "u32"))
            {
                return typeof(MaybeObject);
            }
            
            return TYPE_MAPPING.GetValueOrDefault(fieldType, typeof(RawBytesData));
        }
    }
}