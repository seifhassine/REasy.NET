using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace REasy.Core.Formats.Rsz
{
    public class TypeRegistryPatcher
    {
        private readonly string _registryPath;
        private readonly string _cachePath;
        private Dictionary<string, Dictionary<string, Dictionary<string, object>>> _cache;

        public TypeRegistryPatcher(string registryPath)
        {
            _registryPath = registryPath;
            _cachePath = GetCachePath(registryPath);
            _cache = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
            LoadCache();
        }

        private string GetCachePath(string registryPath)
        {
            string dirName = Path.GetDirectoryName(registryPath) ?? string.Empty;
            string baseName = Path.GetFileName(registryPath);
            string cacheDir = Path.Combine(dirName, ".cache");

            if (!Directory.Exists(cacheDir))
            {
                Directory.CreateDirectory(cacheDir);
            }

            return Path.Combine(cacheDir, $"{baseName}.patch_cache");
        }

        private void LoadCache()
        {
            if (File.Exists(_cachePath))
            {
                try
                {
                    string jsonContent = File.ReadAllText(_cachePath);
                    var deserializedCache = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, object>>>>(
                        jsonContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                    _cache = deserializedCache ?? new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
                }
                catch (Exception ex) when (ex is JsonException || ex is IOException)
                {
                    Console.WriteLine($"Error loading patch cache: {ex.Message}");
                    _cache = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
                }
            }
        }

        private void SaveCache()
        {
            try
            {
                string cacheDir = Path.GetDirectoryName(_cachePath) ?? string.Empty;
                if (!string.IsNullOrEmpty(cacheDir) && !Directory.Exists(cacheDir))
                {
                    Directory.CreateDirectory(cacheDir);
                }

                string jsonContent = JsonSerializer.Serialize(_cache, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(_cachePath, jsonContent);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error saving patch cache: {ex.Message}");
            }
        }

        private string GetFileTimestamp(string filePath)
        {
            return File.GetLastWriteTime(filePath).ToString("o");
        }

        public Dictionary<string, Dictionary<string, object>> PatchRegistry(Dictionary<string, Dictionary<string, object>> registry)
        {
            string currentTimestamp = GetFileTimestamp(_registryPath);
            string cacheKey = currentTimestamp;

            if (_cache.TryGetValue(cacheKey, out var cachedRegistry))
            {
                Console.WriteLine($"Using cached patches for {_registryPath}");
                return cachedRegistry;
            }

            Console.WriteLine($"Creating new patches for {_registryPath}");
            var patchedRegistry = DeepCopyRegistry(registry);

            foreach (var kvp in patchedRegistry)
            {
                string typeKey = kvp.Key;
                var typeInfo = kvp.Value;

                if (typeInfo.TryGetValue("fields", out var fieldsObj) && fieldsObj is JsonElement jsonElement)
                {
                    if (jsonElement.ValueKind == JsonValueKind.Array)
                    {
                        var fields = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(
                            jsonElement.GetRawText(),
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        );

                        if (fields != null)
                        {
                            typeInfo["fields"] = fields;
                            PatchFields(fields);
                        }
                    }
                }
            }

            _cache.Clear();
            _cache[cacheKey] = patchedRegistry;
            SaveCache();

            return patchedRegistry;
        }

        private void PatchFields(List<Dictionary<string, object>> fields)
        {
            var seenNames = new Dictionary<string, int>();

            for (int i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                if (field.TryGetValue("name", out var nameObj) && nameObj is string name && !string.IsNullOrEmpty(name))
                {
                    if (seenNames.TryGetValue(name, out int count))
                    {
                        count++;
                        seenNames[name] = count;

                        string newName = $"{name}_{count}";
                        Console.WriteLine($"Renamed duplicate field '{name}' to '{newName}'");

                        field["name"] = newName;
                    }
                    else
                    {
                        seenNames[name] = 1;
                    }
                }
            }
        }

        private Dictionary<string, Dictionary<string, object>> DeepCopyRegistry(Dictionary<string, Dictionary<string, object>> registry)
        {
            string serialized = JsonSerializer.Serialize(registry);
            return JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(serialized) 
                ?? new Dictionary<string, Dictionary<string, object>>();
        }
    }
}