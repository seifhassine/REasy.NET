using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using REasy.Core.Formats.Rsz.Interfaces;

namespace REasy.Core.Formats.Rsz
{
    public class TypeRegistry : ITypeRegistry
    {
        private readonly string _jsonPath;
        private readonly Dictionary<string, Dictionary<string, object>> _registry;

        public TypeRegistry(string jsonPath)
        {
            _jsonPath = jsonPath;
            var patcher = new TypeRegistryPatcher(jsonPath);
            
            string jsonContent = File.ReadAllText(jsonPath);
            var rawRegistry = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(
                jsonContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new Dictionary<string, Dictionary<string, object>>();

            _registry = patcher.PatchRegistry(rawRegistry);
        }

        public Dictionary<string, object> GetTypeInfo(uint typeId)
        {
            string hexKey = typeId.ToString("x");
            
            if (_registry.TryGetValue(hexKey, out var typeInfo))
            {
                return typeInfo;
            }

            return new Dictionary<string, object>();
        }

        public List<Dictionary<string, object>> GetTypeFields(uint typeId)
        {
            var typeInfo = GetTypeInfo(typeId);
            if (typeInfo != null && typeInfo.TryGetValue("fields", out var fieldsObj))
            {
                return fieldsObj switch
                {
                    List<Dictionary<string, object>> list => list,
                    JsonElement jsonElement when jsonElement.ValueKind == JsonValueKind.Array =>
                        jsonElement.EnumerateArray()
                                .Select(element => JsonSerializer.Deserialize<Dictionary<string, object>>(element.GetRawText()))
                                .Where(dict => dict != null)
                                .ToList()!,
                    _ => new List<Dictionary<string, object>>()
                };
            }
            return new List<Dictionary<string, object>>();
        }

        public bool TryGetTypeFields(uint typeId, out List<Dictionary<string, object>> fields)
        {
            fields = GetTypeFields(typeId);
            return fields != null;
        }
        public (Dictionary<string, object> TypeInfo, uint TypeId) FindTypeByName(string typeName)
        {
            foreach (var kv in _registry)
            {
                var info = kv.Value;
                if (info.TryGetValue("name", out var nameObj) &&
                    nameObj is JsonElement nameElement &&
                    nameElement.ValueKind == JsonValueKind.String &&
                    nameElement.GetString() == typeName)
                {
                    if (info.TryGetValue("fields", out var fieldsObj) &&
                        fieldsObj is JsonElement fieldsElement &&
                        fieldsElement.ValueKind == JsonValueKind.Array)
                    {
                        info["fields"] = fieldsElement
                            .EnumerateArray()
                            .Select(e => JsonSerializer.Deserialize<Dictionary<string, object>>(e.GetRawText()))
                            .Where(dict => dict != null)
                            .ToList();
                    }
                    uint typeId = Convert.ToUInt32(kv.Key, 16);
                    return (info, typeId);
                }
            }
            return (new Dictionary<string, object>(), 0);
        }

        public (Dictionary<string, object> TypeInfo, uint TypeId) FindTypeByTypeId(uint typeId)
        {
            var typeInfo = GetTypeInfo(typeId);
            if (typeInfo != null)
            {
                return (typeInfo, typeId);
            }
            return  (new Dictionary<string, object>(), 0);
        }
        public bool TryGetTypeName(uint typeId, out string name)
        {
            var typeInfo = GetTypeInfo(typeId);
            if (typeInfo != null && typeInfo.TryGetValue("name", out var nameObj))
            {
                if (nameObj is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.String)
                {
                    name = jsonElement.GetString() ?? string.Empty;
                    return true;
                }
                else if (nameObj is string nameStr)
                {
                    name = nameStr;
                    return true;
                }
            }
            name = string.Empty;
            return false;
        }
    }
}
