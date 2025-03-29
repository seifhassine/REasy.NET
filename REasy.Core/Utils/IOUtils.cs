using System.Text;
using System.Text.Json;

namespace REasy.Core.Utils
{
    public static class IOUtils
    {
        public static int Align(int offset, int alignment) => 
            (offset + alignment - 1) & ~(alignment - 1);

        public static string ReadWideString(byte[] data, int offset)
        {
            if (offset < 0 || offset >= data.Length)
                return string.Empty;
            
            int maxBytes = data.Length - offset;
            
            string str = Encoding.Unicode.GetString(data, offset, maxBytes);
            
            int nullPos = str.IndexOf('\0');
            
            return nullPos >= 0 ? str[..nullPos] : str;
        }
        public static int GetWideStringLength(byte[] data, int offset)
        {
            if (offset < 0 || offset >= data.Length)
                return 0;
                
            int length = 0;
            int maxLength = (data.Length - offset) / 2;
            
            while (length < maxLength)
            {
                if (data[offset + (length * 2)] == 0 && data[offset + (length * 2) + 1] == 0)
                    break;
                
                length++;
            }
            
            return length;
        }
        public static int GetIntValue(Dictionary<string, object> dict, string key, int defaultValue)
        {
            if (!dict.TryGetValue(key, out var val))
                return defaultValue;
                
            if (val is JsonElement jsonElement)
            {
                switch (jsonElement.ValueKind)
                {
                    case JsonValueKind.Number:
                        return jsonElement.GetInt32();
                    case JsonValueKind.String:
                        if (int.TryParse(jsonElement.GetString(), out int result))
                            return result;
                        break;
                }
                return defaultValue;
            }
            
            return Convert.ToInt32(val);
        }

        public static bool GetBoolValue(Dictionary<string, object> dict, string key)
        {
            if (!dict.TryGetValue(key, out var val))
                return false;
                
            if (val is JsonElement jsonElement)
            {
                switch (jsonElement.ValueKind)
                {
                    case JsonValueKind.True:
                        return true;
                    case JsonValueKind.False:
                        return false;
                    case JsonValueKind.Number:
                        return jsonElement.GetInt32() != 0;
                    case JsonValueKind.String:
                        if (bool.TryParse(jsonElement.GetString(), out bool result))
                            return result;
                        break;
                }
                return false;
            }
            
            return Convert.ToBoolean(val);
        }
    }
}