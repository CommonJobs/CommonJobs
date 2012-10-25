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
        readonly string packageName;
        readonly string dependenciesFolderName;
        readonly string baseFolder;
        readonly HashSet<string> loadedDependencies = new HashSet<string>();

        public ScriptContext(IJavascriptScriptEngine engine = null, string baseFolder = null, string packageName = "CJLogic", string dependenciesFolderName = "Scripts")
        {
            this.engine = engine ?? new IronJavascriptScriptEngine();
            //this.engine = engine ?? new JurassicJavascriptScriptEngine();
            this.packageName = packageName;
            this.dependenciesFolderName = dependenciesFolderName;
            this.baseFolder = baseFolder ?? GetDefaultBaseFolder(packageName);
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
                script = RemoveDuplicatedDeclarationsOfCJLogicModule(script);
                engine.Run(script);
            }
        }

        bool moduleCJLogicAlreadyDeclared = false;
        const string ModuleCJLogicDecaration = "var CJLogic;";
        //TODO: find a more elegant way to fix it IronJS issue
        //Ya que serializo y deserializo todo, lo mejor va a ser usar V8 
        private string RemoveDuplicatedDeclarationsOfCJLogicModule(string script)
        {
            //script = script.Replace("var CJLogic;", "if (!this.CJLogic) { var CJLogic; }");
            script = script.Replace(
@"var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
}", 
@"if (!this.__extends) {
var __extends = function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
}}");
            
            if (script.Contains(ModuleCJLogicDecaration))
            {
                
                if (!moduleCJLogicAlreadyDeclared)
                    moduleCJLogicAlreadyDeclared = true;
                else
                    script = script.Replace(ModuleCJLogicDecaration, string.Empty);
            }
             
            return script;
        }

        void Import(string scriptName)
        {
            var fileName = string.Format("{0}.js", scriptName);
            ImportBase(packageName, fileName);
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

        public TResult RunScript<TResult>(string scriptName, params object[] parameters)
        {
            Import(packageName);
            Import(scriptName);
            var script = string.Format(
                "JSON.stringify({0}.{1}({2}));",
                packageName,
                scriptName,
                string.Join(",", GenerateScriptParameters(parameters)));
            var result = engine.Run(script);
            return JsonConvert.DeserializeObject<TResult>(result.ToString(), jsonSettings);
        }

        public TResult Eval<TResult>(string script)
        {
            Import(packageName);
            var result = engine.Run("JSON.stringify(" + script + ");");
            return JsonConvert.DeserializeObject<TResult>(result.ToString(), jsonSettings);
        }

        public void Run(string script)
        {
            Import(packageName);
            engine.Run(script);
        }
    }
}
