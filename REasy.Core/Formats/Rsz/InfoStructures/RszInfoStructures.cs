namespace REasy.Core.Formats.Rsz
{
    public interface IRszInfoStructure
    {
        int SIZE { get; }
        
        int Parse(byte[] data, int offset);
    }
    public class GameObjectRefInfo : IRszInfoStructure
    {
        public const int SIZE = 16;
        int IRszInfoStructure.SIZE => SIZE;

        public int ObjectId { get; set; }
        public int PropertyId { get; set; }
        public int ArrayIndex { get; set; }
        public int TargetId { get; set; }

        public int Parse(byte[] data, int offset)
        {
            if (offset + SIZE > data.Length)
                throw new ArgumentException($"Truncated GameObjectRefInfo at 0x{offset:X}");

            using (var ms = new MemoryStream(data, offset, SIZE))
            using (var reader = new BinaryReader(ms))
            {
                ObjectId = reader.ReadInt32();
                PropertyId = reader.ReadInt32();
                ArrayIndex = reader.ReadInt32();
                TargetId = reader.ReadInt32();
            }

            return offset + SIZE;
        }
    }
    public class RszGameObject : IRszInfoStructure
    {
        public const int SIZE = 32;
        int IRszInfoStructure.SIZE => SIZE;

        public byte[] Guid { get; set; } = new byte[16];
        public int Id { get; set; }
        public int ParentId { get; set; }
        public ushort ComponentCount { get; set; }
        public short Unknown { get; set; }
        public int PrefabId { get; set; }

        public int Parse(byte[] data, int offset)
        {
            if (offset + SIZE > data.Length)
                throw new ArgumentException($"Truncated gameobject at 0x{offset:X}");

            using (var ms = new MemoryStream(data, offset, SIZE))
            using (var reader = new BinaryReader(ms))
            {
                Guid = reader.ReadBytes(16);
                Id = reader.ReadInt32();
                ParentId = reader.ReadInt32();
                ComponentCount = reader.ReadUInt16();
                Unknown = reader.ReadInt16();
                PrefabId = reader.ReadInt32();
            }

            return offset + SIZE;
        }
    }
    public class PfbGameObject : IRszInfoStructure
    {
        public const int SIZE = 12;
        int IRszInfoStructure.SIZE => SIZE;

        public int Id { get; set; }
        public int ParentId { get; set; }
        public int ComponentCount { get; set; }

        public int Parse(byte[] data, int offset)
        {
            if (offset + SIZE > data.Length)
                throw new ArgumentException($"Truncated PfbGameObject at 0x{offset:X}");

            using (var ms = new MemoryStream(data, offset, SIZE))
            using (var reader = new BinaryReader(ms))
            {
                Id = reader.ReadInt32();
                ParentId = reader.ReadInt32();
                ComponentCount = reader.ReadInt32();
            }

            return offset + SIZE;
        }
    }
    public class RszFolderInfo : IRszInfoStructure
    {
        public const int SIZE = 8;
        int IRszInfoStructure.SIZE => SIZE;

        public int Id { get; set; }
        public int ParentId { get; set; }

        public int Parse(byte[] data, int offset)
        {
            if (offset + SIZE > data.Length)
                throw new ArgumentException($"Truncated folder info at 0x{offset:X}");

            using (var ms = new MemoryStream(data, offset, SIZE))
            using (var reader = new BinaryReader(ms))
            {
                Id = reader.ReadInt32();
                ParentId = reader.ReadInt32();
            }

            return offset + SIZE;
        }
    }
    public class RszResourceInfo : IRszInfoStructure
    {
        public const int SIZE = 8;
        int IRszInfoStructure.SIZE => SIZE;

        public uint StringOffset { get; set; }
        public uint Reserved { get; set; }

        public int Parse(byte[] data, int offset)
        {
            if (offset + SIZE > data.Length)
                throw new ArgumentException($"Truncated resource info at 0x{offset:X}");

            using (var ms = new MemoryStream(data, offset, SIZE))
            using (var reader = new BinaryReader(ms))
            {
                StringOffset = reader.ReadUInt32();
                Reserved = reader.ReadUInt32();
            }

            return offset + SIZE;
        }
    }
    public class RszPrefabInfo : IRszInfoStructure
    {
        public const int SIZE = 8;
        int IRszInfoStructure.SIZE => SIZE;

        public uint StringOffset { get; set; }
        public uint ParentId { get; set; }

        public int Parse(byte[] data, int offset)
        {
            if (offset + SIZE > data.Length)
                throw new ArgumentException($"Truncated prefab info at 0x{offset:X}");

            using (var ms = new MemoryStream(data, offset, SIZE))
            using (var reader = new BinaryReader(ms))
            {
                StringOffset = reader.ReadUInt32();
                ParentId = reader.ReadUInt32();
            }

            return offset + SIZE;
        }
    }
    public class RszUserDataInfo : IRszInfoStructure
    {
        public const int SIZE = 16;
        int IRszInfoStructure.SIZE => SIZE;

        public uint Hash { get; set; }
        public uint CRC { get; set; }
        public ulong StringOffset { get; set; }

        public int Parse(byte[] data, int offset)
        {
            if (offset + SIZE > data.Length)
                throw new ArgumentException($"Truncated userdata info at 0x{offset:X}");

            using (var ms = new MemoryStream(data, offset, SIZE))
            using (var reader = new BinaryReader(ms))
            {
                Hash = reader.ReadUInt32();
                CRC = reader.ReadUInt32();
                StringOffset = reader.ReadUInt64();
            }

            return offset + SIZE;
        }
    }
    public class RszRSZUserDataInfo : IRszInfoStructure
    {
        public const int SIZE = 16;
        int IRszInfoStructure.SIZE => SIZE;

        public uint InstanceId { get; set; }
        public uint Hash { get; set; }
        public ulong StringOffset { get; set; }

        public int Parse(byte[] data, int offset)
        {
            if (offset + SIZE > data.Length)
                throw new ArgumentException($"Truncated RSZUserData info at 0x{offset:X}");

            using (var ms = new MemoryStream(data, offset, SIZE))
            using (var reader = new BinaryReader(ms))
            {
                InstanceId = reader.ReadUInt32();
                Hash = reader.ReadUInt32();
                StringOffset = reader.ReadUInt64();
            }

            return offset + SIZE;
        }
    }
    public class RszRSZHeader : IRszInfoStructure
    {
        public const int SIZE = 48;
        int IRszInfoStructure.SIZE => SIZE;

        public uint Magic { get; set; }
        public uint Version { get; set; }
        public uint ObjectCount { get; set; }
        public uint InstanceCount { get; set; }
        public uint UserdataCount { get; set; }
        public uint Reserved { get; set; }
        public ulong InstanceOffset { get; set; }
        public ulong DataOffset { get; set; }
        public ulong UserdataOffset { get; set; }

        public int Parse(byte[] data, int offset)
        {
            if (offset + SIZE > data.Length)
                throw new ArgumentException($"Truncated RSZ header at 0x{offset:X}");

            using (var ms = new MemoryStream(data, offset, SIZE))
            using (var reader = new BinaryReader(ms))
            {
                Magic = reader.ReadUInt32();
                Version = reader.ReadUInt32();
                ObjectCount = reader.ReadUInt32();
                InstanceCount = reader.ReadUInt32();
                UserdataCount = reader.ReadUInt32();
                Reserved = reader.ReadUInt32();
                InstanceOffset = reader.ReadUInt64();
                DataOffset = reader.ReadUInt64();
                UserdataOffset = reader.ReadUInt64();
            }

            return offset + SIZE;
        }
    }
    public class RszInstanceInfo : IRszInfoStructure
    {
        public const int SIZE = 8;
        int IRszInfoStructure.SIZE => SIZE;

        public uint TypeId { get; set; }
        public uint CRC { get; set; }

        public int Parse(byte[] data, int offset)
        {
            if (offset + SIZE > data.Length)
                throw new ArgumentException($"Truncated instance info at 0x{offset:X}");

            using (var ms = new MemoryStream(data, offset, SIZE))
            using (var reader = new BinaryReader(ms))
            {
                TypeId = reader.ReadUInt32();
                CRC = reader.ReadUInt32();
            }

            return offset + SIZE;
        }
    }
}