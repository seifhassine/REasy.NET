namespace REasy.Core.Formats.Rsz
{
    public interface IRszHeader
    {
        int SIZE { get; }
        void Parse(byte[] data);
    }
    public class ScnHeader : IRszHeader
    {
        public const int SIZE = 64;
        int IRszHeader.SIZE => SIZE;

        public byte[] Signature { get; set; } = new byte[4];
        public uint InfoCount { get; set; }
        public uint ResourceCount { get; set; }
        public uint FolderCount { get; set; }
        public uint PrefabCount { get; set; }
        public uint UserdataCount { get; set; }
        public ulong FolderTable { get; set; }
        public ulong ResourceInfoTable { get; set; }
        public ulong PrefabInfoTable { get; set; }
        public ulong UserdataInfoTable { get; set; }
        public ulong DataOffset { get; set; }

        public void Parse(byte[] data)
        {
            if (data.Length < SIZE)
                throw new ArgumentException($"Data too small for SCN header. Expected at least {SIZE} bytes, got {data.Length}");

            using (var reader = new BinaryReader(new MemoryStream(data)))
            {
                Signature = reader.ReadBytes(4);
                InfoCount = reader.ReadUInt32();
                ResourceCount = reader.ReadUInt32();
                FolderCount = reader.ReadUInt32();
                PrefabCount = reader.ReadUInt32();
                UserdataCount = reader.ReadUInt32();
                FolderTable = reader.ReadUInt64();
                ResourceInfoTable = reader.ReadUInt64();
                PrefabInfoTable = reader.ReadUInt64();
                UserdataInfoTable = reader.ReadUInt64();
                DataOffset = reader.ReadUInt64();
            }
        }
    }
    public class PfbHeader : IRszHeader
    {
        public const int SIZE = 56;
        int IRszHeader.SIZE => SIZE;

        public byte[] Signature { get; set; } = new byte[4];
        public uint InfoCount { get; set; }
        public uint ResourceCount { get; set; }
        public uint GameobjectRefInfoCount { get; set; }
        public uint UserdataCount { get; set; }
        public uint Reserved { get; set; }
        public ulong GameobjectRefInfoTable { get; set; }
        public ulong ResourceInfoTable { get; set; }
        public ulong UserdataInfoTable { get; set; }
        public ulong DataOffset { get; set; }

        public void Parse(byte[] data)
        {
            if (data.Length < SIZE)
                throw new ArgumentException($"Data too small for PFB header. Expected at least {SIZE} bytes, got {data.Length}");

            using (var reader = new BinaryReader(new MemoryStream(data)))
            {
                Signature = reader.ReadBytes(4);
                InfoCount = reader.ReadUInt32();
                ResourceCount = reader.ReadUInt32();
                GameobjectRefInfoCount = reader.ReadUInt32();
                UserdataCount = reader.ReadUInt32();
                Reserved = reader.ReadUInt32();
                GameobjectRefInfoTable = reader.ReadUInt64();
                ResourceInfoTable = reader.ReadUInt64();
                UserdataInfoTable = reader.ReadUInt64();
                DataOffset = reader.ReadUInt64();
            }
        }
    }
    public class UsrHeader : IRszHeader
    {
        public const int SIZE = 48;
        int IRszHeader.SIZE => SIZE;

        public byte[] Signature { get; set; } = new byte[4];
        public uint ResourceCount { get; set; }
        public uint UserdataCount { get; set; }
        public uint InfoCount { get; set; }
        public ulong ResourceInfoTable { get; set; }
        public ulong UserdataInfoTable { get; set; }
        public ulong DataOffset { get; set; }
        public ulong Reserved { get; set; }

        public void Parse(byte[] data)
        {
            if (data.Length < SIZE)
                throw new ArgumentException($"Data too small for USR header. Expected at least {SIZE} bytes, got {data.Length}");

            using (var reader = new BinaryReader(new MemoryStream(data)))
            {
                Signature = reader.ReadBytes(4);
                ResourceCount = reader.ReadUInt32();
                UserdataCount = reader.ReadUInt32();
                InfoCount = reader.ReadUInt32();
                ResourceInfoTable = reader.ReadUInt64();
                UserdataInfoTable = reader.ReadUInt64();
                DataOffset = reader.ReadUInt64();
                Reserved = reader.ReadUInt64();
            }
        }
    }
}