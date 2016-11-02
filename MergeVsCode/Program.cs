using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MergeVsCode
{
    class Program
    {
        static void Main(string[] args)
        {
            var analysis = new Analysis();

            foreach (var result in Machines.MachineNames()
                .Select(x => Tuple.Create(FileService.GetCodeBackupFolderPath(x),x))
                .Where(x => FileService.IsValid(x.Item1)))
                
            {
                foreach (var file in Directory.EnumerateFiles(result.Item1))
                {
                    string fileName = Path.GetFileName(file);

                    using (StreamReader reader = File.OpenText(file))
                    {
                        JObject jobj = (JObject)JObject.Load(new JsonTextReader(reader));
                        analysis.AddFile(fileName,jobj, result.Item2);
                    }
                }

                
            }

            var folder = FileService.CodeFolderPath;


            foreach (var keyValuePair in analysis._sources)
            {
                File.WriteAllText(Path.Combine(folder,keyValuePair.Key),keyValuePair.Value.ToString(Formatting.Indented));
                Console.WriteLine($"{keyValuePair.Key}\r\n{keyValuePair.Value.ToString(Formatting.Indented)}");
            }

            int i = 0;
        }
    }

    

    public class Analysis
    {
        public Dictionary<string, JObject> _sources = new Dictionary<string, JObject>(); 
        public List<string> Conflicts = new List<string>(); 

        public void AddFile(string fileName, JObject obj, string source)
        {
            
            if (!_sources.ContainsKey(fileName))
            {
                _sources.Add(fileName,obj);
                return;
            }
            else
            {
                var jobj = _sources[fileName];
                foreach (var property in obj.Properties())
                {

                    try
                    {
                        if (jobj[property.Name] == null)
                        {
                            jobj.Add(property);
                        }
                        else
                        {
                            this.Conflicts.Add(property.ToString(Formatting.Indented));
                        }
                       
                    }
                    catch (Exception e)
                    {
                        throw new InvalidOperationException($"Could not add property {property?.Name} from file {source}");
                    }
                }
            }
        }
    }

    public static class Machines
    {
        public static string JeppeLaptop => "Jeppe-Laptop";
        public static string JvkLap01 => "JVKLAP01";

        public static string JvkWks01 => "JvkWks01";

        public static IEnumerable<string> MachineNames()
        {
            yield return JeppeLaptop;
            yield return JvkLap01;
            yield return JvkWks01;
        }
      
        
    }

    public static class FileService
    {
        public static string GetCodeBackupFolderPath(string machine)
        {
            return Path.Combine(GetOnedriveRoot(), $@"Backup\{machine}\Code\Snippets");
        }

        public static string CodeFolderPath
            => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Code\User\Snippets");



        public static string GetOnedriveRoot()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "onedrive");
        }

        public static bool IsValid(this string path)
        {
            return Directory.Exists(path);
        }
    }
}
