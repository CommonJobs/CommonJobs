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
        private HashSet<string> loadedDependencies = new HashSet<string>();

        public JsonSerializerSettings JsonSettings { get; set; }
        public Func<IJavascriptScriptEngine> CreateEngineFunc { get; set; }
        private IJavascriptScriptEngine engine = null;
        public IJavascriptScriptEngine Engine
        {
            get { return engine ?? (engine = CreateEngineFunc()); }
        }

        public string BaseFolder { get; private set; }

        public ScriptContext(string baseFolder)
        {
            BaseFolder = baseFolder;
            CreateEngineFunc = () => new IronJavascriptScriptEngine();
            JsonSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Converters = new[] 
                { 
                    //http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx http://msdn.microsoft.com/en-us/library/az4se3k1.aspx
                    new IsoDateTimeConverter() { DateTimeFormat = "s" } 
                }
            };
        }

        void Import(string fileName)
        {
            var path = Path.GetFullPath(Path.Combine(BaseFolder, fileName));
            Import(() =>
            {
                using (var stream = File.OpenText(path))
                    return stream.ReadToEnd();
            }, path);
        }

        public void Import(params string[] dependencies)
        {
            foreach (var dependency in dependencies)
                Import(dependency);
        }

        public void Import(Func<string> readScriptFunc, string key = null)
        {
            if (key == null)
            {
                var script = readScriptFunc();
                key = script.GetHashCode().ToString();
                readScriptFunc = () => script;
            }
            
            if (!loadedDependencies.Contains(key))
            {
                loadedDependencies.Add(key);
                var script = readScriptFunc();
                //TODO: Ugly patch, fix and remove it
                script = RemoveDuplicatedDeclarationsOfCJLogicModule(script);
                Engine.Run(script);
            }
        }

        #region Ugly patch, fix and remove it
        //TODO: find a more elegant way to fix it IronJS issue
        //Ya que serializo y deserializo todo, lo mejor va a ser usar V8 
        bool moduleCJLogicAlreadyDeclared = false;
        const string ModuleCJLogicDecaration = "var CJLogic;";
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
        #endregion

        string[] GenerateScriptParameters(params object[] parameters)
        {
            return parameters.Select(x => JsonConvert.SerializeObject(x, Formatting.None, JsonSettings)).ToArray();
        }

        public TResult RunScript<TResult>(string functionName, params object[] parameters)
        {
            var script = string.Format(
                "JSON.stringify({0}({1}));",
                functionName,
                string.Join(",", GenerateScriptParameters(parameters)));
            var result = Engine.Run(script);
            return JsonConvert.DeserializeObject<TResult>(result.ToString(), JsonSettings);
        }
    }
}
