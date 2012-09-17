using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CommonJobs.JavaScript
{
    public class ScriptContext
    {
        readonly IJavascriptScriptEngine engine;
        readonly string scriptsFolderName;
        readonly string dependenciesFolderName;
        readonly string baseFolder;
        readonly HashSet<string> loadedDependencies = new HashSet<string>();

        public ScriptContext(IJavascriptScriptEngine engine = null, string baseFolder = null, string scriptsFolderName = "CJLogic", string dependenciesFolderName = "Scripts")
        {
            this.engine = engine ?? new IronJavascriptScriptEngine();
            //this.engine = engine ?? new JurassicJavascriptScriptEngine();
            this.scriptsFolderName = scriptsFolderName;
            this.dependenciesFolderName = dependenciesFolderName;
            this.baseFolder = baseFolder ?? GetDefaultBaseFolder(scriptsFolderName);
            ImportDependencies("json2.js");
        }

        public void ImportDependencies(params string[] dependencies)
        {
            foreach (var dependency in dependencies)
            {
                ImportBase(dependenciesFolderName, dependency);
            }
        }

        string GetDefaultBaseFolder(string folderName)
        {
            //TODO: enhance this code
            var dllUri = Path.GetDirectoryName(this.GetType().Assembly.CodeBase);

            var test1 = new Uri(Path.Combine(dllUri, folderName)).LocalPath;
            if (Directory.Exists(test1))
                return Path.GetDirectoryName(test1);

            var test2 = new Uri(Path.Combine(dllUri, "..", folderName)).LocalPath;
            if (Directory.Exists(test2))
                return Path.GetDirectoryName(test2);

            throw new ApplicationException("Scripts folder not found!");
        }

        void ImportBase(string folderName, string fileName)
        {
            var path = Path.Combine(baseFolder, folderName, fileName);
            if (!loadedDependencies.Contains(path))
            {
                loadedDependencies.Add(path);
                var script = ReadScript(path);
                engine.Run(script);
            }
        }

        void Import(string packageName, string scriptName)
        {
            var fileName = string.Format("{0}.{1}.js", packageName, scriptName);
            ImportBase(scriptsFolderName, fileName);
        }

        string ReadScript(string scriptPath)
        {
            using (var stream = File.OpenText(scriptPath))
            {
                return stream.ReadToEnd();
            }
        }

        JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Converters = new[] 
                { 
                    //http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx http://msdn.microsoft.com/en-us/library/az4se3k1.aspx
                    new IsoDateTimeConverter() { DateTimeFormat = "s" } 
                }
            };

        string[] GenerateScriptParameters(params object[] parameters)
        {
            return parameters.Select(x => JsonConvert.SerializeObject(x, Formatting.None, jsonSettings)).ToArray();
        }

        public TResult RunScript<TResult>(string packageName, string scriptName, params object[] parameters)
        {
            Import(packageName, scriptName);
            var script = string.Format(
                "JSON.stringify({0}.{1}({2}));",
                packageName,
                scriptName,
                string.Join(",", GenerateScriptParameters(parameters)));
            var result = engine.Run(script);
            return JsonConvert.DeserializeObject<TResult>(result.ToString(), jsonSettings);
        }
    }
}
