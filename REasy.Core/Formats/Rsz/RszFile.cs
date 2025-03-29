using REasy.Core.Formats.Rsz.Interfaces;
using REasy.Core.Utils;

namespace REasy.Core.Formats.Rsz
{
    public class RszFile
    {
        public bool IsUsr { get; private set; }
        public bool IsPfb { get; private set; }
        public string? FilePath { get; set; }
        public string GameVersion { get; set; } = "RE4";

        private byte[] _fullData = Array.Empty<byte>();
        private int _currentOffset = 0;
        
        public IRszHeader? Header { get; private set; }
        public RszRSZHeader RszHeader { get; set; } = new RszRSZHeader();
        
        private List<int> _objectTable = new List<int>();
        public List<int> ObjectTable 
        { 
            get => _objectTable;
            set
            {
                _objectTable = value;
                // Cache is now invalid since object table changed
                _gameObjectInstanceIdsCache = null;
                _folderInstanceIdsCache = null;
            }
        }

        public List<RszGameObject> GameObjects { get; private set; } = new List<RszGameObject>();
        public List<PfbGameObject> PfbGameObjects { get; private set; } = new List<PfbGameObject>();
        public List<RszFolderInfo> FolderInfos { get; private set; } = new List<RszFolderInfo>();
        public List<RszResourceInfo> ResourceInfos { get; private set; } = new List<RszResourceInfo>();
        public List<RszPrefabInfo> PrefabInfos { get; private set; } = new List<RszPrefabInfo>();
        public List<RszUserDataInfo> UserDataInfos { get; private set; } = new List<RszUserDataInfo>();
        public List<GameObjectRefInfo> GameObjectRefInfos { get; private set; } = new List<GameObjectRefInfo>();
        public List<RszRSZUserDataInfo> RszUserDataInfos { get; private set; } = new List<RszRSZUserDataInfo>();
        public List<RszInstanceInfo> InstanceInfos { get; private set; } = new List<RszInstanceInfo>();
        
        public byte[]? ResourceBlock { get; private set; }
        public byte[]? PrefabBlock { get; private set; }
        public byte[]? Data { get; private set; }
        
        public Dictionary<int, Dictionary<string, object>> ParsedElements { get; private set; } = new Dictionary<int, Dictionary<string, object>>();
        public Dictionary<int, InstanceHierarchyInfo> InstanceHierarchy { get; private set; } = new Dictionary<int, InstanceHierarchyInfo>();

        public ITypeRegistry? TypeRegistry { get; set; }

        private Dictionary<RszResourceInfo, string> _resourceStrMap = new Dictionary<RszResourceInfo, string>();
        private Dictionary<RszPrefabInfo, string> _prefabStrMap = new Dictionary<RszPrefabInfo, string>();
        private Dictionary<RszUserDataInfo, string> _userdataStrMap = new Dictionary<RszUserDataInfo, string>();
        private Dictionary<RszRSZUserDataInfo, string> _rszUserdataStrMap = new Dictionary<RszRSZUserDataInfo, string>();
        
        private HashSet<int>? _gameObjectInstanceIdsCache;
        
        public HashSet<int> GameObjectInstanceIds
        {
            get
            {
                if (_gameObjectInstanceIdsCache == null)
                {
                    _gameObjectInstanceIdsCache = new HashSet<int>();
                    
                    if (IsPfb)
                    {
                        foreach (var go in PfbGameObjects)
                        {
                            if (go.Id < ObjectTable.Count)
                            {
                                _gameObjectInstanceIdsCache.Add(ObjectTable[go.Id]);
                            }
                        }
                    }
                    else
                    {
                        foreach (var go in GameObjects)
                        {
                            if (go.Id < ObjectTable.Count)
                            {
                                _gameObjectInstanceIdsCache.Add(ObjectTable[go.Id]);
                            }
                        }
                    }
                }
                
                return _gameObjectInstanceIdsCache;
            }
        }
        
        private HashSet<int>? _folderInstanceIdsCache;
        
        public HashSet<int> FolderInstanceIds
        {
            get
            {
                if (_folderInstanceIdsCache == null)
                {
                    _folderInstanceIdsCache = new HashSet<int>();
                    
                    foreach (var fi in FolderInfos)
                    {
                        if (fi.Id < ObjectTable.Count)
                        {
                            _folderInstanceIdsCache.Add(ObjectTable[fi.Id]);
                        }
                    }
                }
                
                return _folderInstanceIdsCache;
            }
        }
        
        private Dictionary<uint, RszRSZUserDataInfo> _rszUserdataDict = new Dictionary<uint, RszRSZUserDataInfo>();
        private HashSet<uint> _rszUserdataSet = new HashSet<uint>();

        public void Read(byte[] data)
        {
            _fullData = data;
            _currentOffset = 0;
            
            DetermineFileType(data);
            
            ParseHeader(data);
            
            if (IsUsr)      ParseUsrFile(data);
            else if (IsPfb) ParsePfbFile(data);
            else            ParseScnFile(data);
        }
        
        private void DetermineFileType(byte[] data)
        {
            if (data.Length < 4)
                throw new ArgumentException("Data too small to determine file type");

            if (data[0] == 'U' && data[1] == 'S' && data[2] == 'R' && data[3] == 0)
            {
                IsUsr = true;
                IsPfb = false;
            }
            else if (data[0] == 'P' && data[1] == 'F' && data[2] == 'B' && data[3] == 0)
            {
                IsUsr = false;
                IsPfb = true;
            }
            else
            {
                IsUsr = false;
                IsPfb = false;
            }
        }
        
        private void ParseHeader(byte[] data)
        {
            Header = IsUsr ? new UsrHeader() :
                     IsPfb ? new PfbHeader() :
                             new ScnHeader();

            Header.Parse(data);
            _currentOffset = Header switch
            {
                UsrHeader => UsrHeader.SIZE,
                PfbHeader => PfbHeader.SIZE,
                ScnHeader => ScnHeader.SIZE,
                _ => throw new InvalidOperationException("Unknown header type")
            };
        }

        private void ParseUsrFile(byte[] data)
        {
            ParseResourceInfos(data);
            ParseUserDataInfos(data);
            ParseBlocks(data);
            ParseRszSection(data);
        }

        private void ParsePfbFile(byte[] data)
        {
            ParseGameObjects(data);
            ParseGameObjectRefInfos(data);
            ParseResourceInfos(data);
            ParseUserDataInfos(data);
            ParseBlocks(data);
            ParseRszSection(data);
        }
        
        private void ParseScnFile(byte[] data)
        {
            ParseGameObjects(data);
            ParseFolderInfos(data);
            ParseResourceInfos(data);
            ParsePrefabInfos(data);
            ParseUserDataInfos(data);
            ParseBlocks(data);
            ParseRszSection(data);
        }
        
        private void ParseGameObjects(byte[] data)
        {
            uint count = Header switch
            {
                ScnHeader scnHeader => scnHeader.InfoCount,
                PfbHeader pfbHeader => pfbHeader.InfoCount,
                _ => 0
            };

            if (IsPfb)
            {
                for (uint i = 0; i < count; i++)
                {
                    var go = new PfbGameObject();
                    _currentOffset = go.Parse(data, _currentOffset);
                    PfbGameObjects.Add(go);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    var go = new RszGameObject();
                    _currentOffset = go.Parse(data, _currentOffset);
                    GameObjects.Add(go);
                }
            }
        }
        
        private void ParseGameObjectRefInfos(byte[] data)
        {
            if (Header is not PfbHeader pfbHeader)
                return;
                
            for (int i = 0; i < pfbHeader.GameobjectRefInfoCount; i++)
            {
                var refInfo = new GameObjectRefInfo();
                _currentOffset = refInfo.Parse(data, _currentOffset);
                GameObjectRefInfos.Add(refInfo);
            }
            _currentOffset = IOUtils.Align(_currentOffset, 16);
        }
        
        private void ParseFolderInfos(byte[] data)
        {
            if (Header is not ScnHeader scnHeader)
                return;
                
            for (int i = 0; i < scnHeader.FolderCount; i++)
            {
                var folderInfo = new RszFolderInfo();
                _currentOffset = folderInfo.Parse(data, _currentOffset);
                FolderInfos.Add(folderInfo);
            }
            _currentOffset = IOUtils.Align(_currentOffset, 16);
        }
        
        private void ParseResourceInfos(byte[] data)
        {
            uint count = 0;
            if (Header is ScnHeader scnHeader)
                count = scnHeader.ResourceCount;
            else if (Header is PfbHeader pfbHeader)
                count = pfbHeader.ResourceCount;
            else if (Header is UsrHeader usrHeader)
                count = usrHeader.ResourceCount;
                
            for (int i = 0; i < count; i++)
            {
                var resourceInfo = new RszResourceInfo();
                _currentOffset = resourceInfo.Parse(data, _currentOffset);
                ResourceInfos.Add(resourceInfo);
            }
            _currentOffset = IOUtils.Align(_currentOffset, 16);
        }
        
        private void ParsePrefabInfos(byte[] data)
        {
            if (Header is not ScnHeader scnHeader)
                return;
                
            for (int i = 0; i < scnHeader.PrefabCount; i++)
            {
                var prefabInfo = new RszPrefabInfo();
                _currentOffset = prefabInfo.Parse(data, _currentOffset);
                PrefabInfos.Add(prefabInfo);
            }
            _currentOffset = IOUtils.Align(_currentOffset, 16);
        }
        
        private void ParseUserDataInfos(byte[] data)
        {
            uint count = 0;
            if (Header is ScnHeader scnHeader)
                count = scnHeader.UserdataCount;
            else if (Header is PfbHeader pfbHeader)
                count = pfbHeader.UserdataCount;
            else if (Header is UsrHeader usrHeader)
                count = usrHeader.UserdataCount;
                
            for (int i = 0; i < count; i++)
            {
                var userDataInfo = new RszUserDataInfo();
                _currentOffset = userDataInfo.Parse(data, _currentOffset);
                UserDataInfos.Add(userDataInfo);
            }
            _currentOffset = IOUtils.Align(_currentOffset, 16);
        }
        
        private void ParseBlocks(byte[] data)
        {
            foreach (var ri in ResourceInfos)
            {
                string s = IOUtils.ReadWideString(data, (int)ri.StringOffset);
                _resourceStrMap[ri] = s;
            }
            
            foreach (var pi in PrefabInfos)
            {
                string s = IOUtils.ReadWideString(data, (int)pi.StringOffset);
                _prefabStrMap[pi] = s;
            }
            
            foreach (var ui in UserDataInfos)
            {
                string s = IOUtils.ReadWideString(data, (int)ui.StringOffset);
                _userdataStrMap[ui] = s;
            }
        }
        
        private void ParseRszSection(byte[] data)
        {
            _currentOffset = (int)GetDataOffset();
            
            RszHeader = new RszRSZHeader();
            _currentOffset = RszHeader.Parse(data, _currentOffset);
            
            if (RszHeader.ObjectCount > 0)
            {
                using (var ms = new MemoryStream(data, _currentOffset, (int)(RszHeader.ObjectCount * 4)))
                using (var reader = new BinaryReader(ms))
                {
                    List<int> objectTable = new List<int>();
                    for (int i = 0; i < RszHeader.ObjectCount; i++)
                    {
                        objectTable.Add(reader.ReadInt32());
                    }
                    ObjectTable = objectTable; 
                }
                _currentOffset += (int)(RszHeader.ObjectCount * 4);
            }
            
            _currentOffset = (int)GetDataOffset() + (int)RszHeader.InstanceOffset;
            
            for (int i = 0; i < RszHeader.InstanceCount; i++)
            {
                var instanceInfo = new RszInstanceInfo();
                _currentOffset = instanceInfo.Parse(data, _currentOffset);
                InstanceInfos.Add(instanceInfo);
            }
            
            _currentOffset = (int)GetDataOffset() + (int)RszHeader.UserdataOffset;
            
            ParseStandardRszUserdata(data);
            
            Data = new byte[data.Length - _currentOffset];
            Array.Copy(data, _currentOffset, Data, 0, Data.Length);
            
            BuildUserdataLookups();
        }
        
        private void ParseStandardRszUserdata(byte[] data)
        {
            for (int i = 0; i < RszHeader.UserdataCount; i++)
            {
                var userDataInfo = new RszRSZUserDataInfo();
                _currentOffset = userDataInfo.Parse(data, _currentOffset);
                
                if (userDataInfo.StringOffset != 0)
                {
                    long absOffset = (long)(GetDataOffset() + userDataInfo.StringOffset);
                    string s = IOUtils.ReadWideString(_fullData, (int)absOffset);
                    _rszUserdataStrMap[userDataInfo] = s;
                }
                
                RszUserDataInfos.Add(userDataInfo);
            }
            
            if (RszUserDataInfos.Count > 0)
            {
                var lastInfo = RszUserDataInfos.Last();
                if (lastInfo.StringOffset != 0)
                {
                    long absOffset = (long)(GetDataOffset() + lastInfo.StringOffset);
                    int stringLength = IOUtils.GetWideStringLength(_fullData, (int)absOffset);
                    long newOffset = absOffset + (stringLength * 2) + 2;
                    _currentOffset = IOUtils.Align((int)newOffset, 16);
                }
                else
                {
                    _currentOffset = IOUtils.Align(_currentOffset, 16);
                }
            }
            else
            {
                _currentOffset = IOUtils.Align(_currentOffset, 16);
            }
        }
        
        private void BuildUserdataLookups()
        {
            _rszUserdataDict.Clear();
            _rszUserdataSet.Clear();
            
            foreach (var rui in RszUserDataInfos)
            {
                _rszUserdataDict[rui.InstanceId] = rui;
                _rszUserdataSet.Add(rui.InstanceId);
            }
        }
        
        
        private ulong GetDataOffset()
        {
            if (Header is ScnHeader scnHeader)
                return scnHeader.DataOffset;
            else if (Header is PfbHeader pfbHeader)
                return pfbHeader.DataOffset;
            else if (Header is UsrHeader usrHeader)
                return usrHeader.DataOffset;
            
            return 0;
        }

        public string GetResourceString(RszResourceInfo ri)
        {
            return _resourceStrMap.TryGetValue(ri, out var value) ? value : string.Empty;
        }
        
        public string GetPrefabString(RszPrefabInfo pi)
        {
            return _prefabStrMap.TryGetValue(pi, out var value) ? value : string.Empty;
        }
        
        public string GetUserDataString(RszUserDataInfo ui)
        {
            return _userdataStrMap.TryGetValue(ui, out var value) ? value : string.Empty;
        }
        
        public string GetRszUserDataString(RszRSZUserDataInfo rui)
        {
            return _rszUserdataStrMap.TryGetValue(rui, out var value) ? value : string.Empty;
        }

        public void SetResourceString(RszResourceInfo ri, string value)
        {
            _resourceStrMap[ri] = value;
        }
        
        public void SetPrefabString(RszPrefabInfo pi, string value)
        {
            _prefabStrMap[pi] = value;
        }
        
        public void SetUserDataString(RszUserDataInfo ui, string value)
        {
            _userdataStrMap[ui] = value;
        }

        public void SetRszUserDataString(RszRSZUserDataInfo rui, string value)
        {
            _rszUserdataStrMap[rui] = value;
        }
    }
    public class InstanceHierarchyInfo
    {
        public List<int> Children { get; } = new List<int>();
        public int? Parent { get; set; }
    }
}