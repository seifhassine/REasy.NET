using System.Collections.Generic;

namespace REasy.Core.Formats.Rsz.Interfaces
{
    public interface ITypeRegistry
    {
        Dictionary<string, object> GetTypeInfo(uint typeId);
        (Dictionary<string, object> TypeInfo, uint TypeId) FindTypeByName(string typeName);
        (Dictionary<string, object> TypeInfo, uint TypeId) FindTypeByTypeId(uint typeId);
        bool TryGetTypeName(uint typeId, out string name);
        List<Dictionary<string, object>> GetTypeFields(uint typeId);
    }
}
