using REasy.Core.Formats.Rsz;
namespace REasy.Tests
{
    class RszParsingTest
    {
        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: RszParsingTest.exe <path_to_rsz_file> <path_to_type_registry_json>");
                return;
            }

            string rszFilePath = args[0];
            string typeRegistryPath = args[1];

            Console.WriteLine($"Loading RSZ file: {rszFilePath}");
            Console.WriteLine($"Using type registry: {typeRegistryPath}");

            try
            {
                var typeRegistry = new TypeRegistry(typeRegistryPath);
                Console.WriteLine("Type registry loaded successfully.");

                Console.WriteLine("\nTesting type registry lookups:");
                
                var (gameObjectTypeInfo, gameObjectTypeId) = typeRegistry.FindTypeByName("via.GameObject");
                if (gameObjectTypeInfo != null)
                {
                    Console.WriteLine($"Found 'via.GameObject' with type ID: 0x{gameObjectTypeId:X}");
                }
                else
                {
                    Console.WriteLine("Could not find 'via.GameObject' type");
                }

                var (transformTypeInfo, transformTypeId) = typeRegistry.FindTypeByName("via.Transform");
                if (transformTypeInfo != null)
                {
                    Console.WriteLine($"Found 'via.Transform' with type ID: 0x{transformTypeId:X}");
                }
                else
                {
                    Console.WriteLine("Could not find 'via.Transform' type");
                }

                Console.WriteLine("\nLoading RSZ file...");
                byte[] fileData = File.ReadAllBytes(rszFilePath);
                
                var rszFile = new RszFile();
                rszFile.TypeRegistry = typeRegistry;
                rszFile.Read(fileData);

                Console.WriteLine($"\nFile type: {(rszFile.IsUsr ? "USR" : (rszFile.IsPfb ? "PFB" : "SCN"))}");
                Console.WriteLine($"Object table entries: {rszFile.ObjectTable.Count}");
                Console.WriteLine($"GameObjects: {(rszFile.IsPfb ? rszFile.PfbGameObjects.Count : rszFile.GameObjects.Count)}");
                Console.WriteLine($"Resource infos: {rszFile.ResourceInfos.Count}");
                Console.WriteLine($"User data infos: {rszFile.UserDataInfos.Count}");
                Console.WriteLine($"RSZ user data infos: {rszFile.RszUserDataInfos.Count}");
                Console.WriteLine($"Instance infos: {rszFile.InstanceInfos.Count}");
                
                Console.WriteLine("\nSample resources:");
                int resourceCount = Math.Min(10, rszFile.ResourceInfos.Count);
                for (int i = 0; i < resourceCount; i++)
                {
                    var resourceInfo = rszFile.ResourceInfos[i];
                    string resourceString = rszFile.GetResourceString(resourceInfo);
                    Console.WriteLine($"  {i}: {resourceString}");
                }

                Console.WriteLine("\nRSZ file parsed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}