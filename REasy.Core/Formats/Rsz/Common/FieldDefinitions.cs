
namespace REasy.Core.Formats.Rsz.Common
{
    public readonly struct FieldDefinition
    {
        public string Name { get; }
        public string Type { get; }
        public int Size { get; }
        public bool IsNative { get; }
        public bool IsArray { get; }
        public string OriginalType { get; }
        public int Alignment { get; }

        public FieldDefinition(string name, string type, int size, bool isNative, 
                                bool isArray, string originalType, int alignment)
        {
            Name = name;
            Type = type;
            Size = size;
            IsNative = isNative;
            IsArray = isArray;
            OriginalType = originalType;
            Alignment = alignment;
        }
    }

    public readonly struct StructDefinition
    {
        public List<FieldDefinition> Fields { get; }
        public int Size { get; }

        public StructDefinition(List<FieldDefinition> fields, int size)
        {
            Fields = fields;
            Size = size;
        }
    }
}