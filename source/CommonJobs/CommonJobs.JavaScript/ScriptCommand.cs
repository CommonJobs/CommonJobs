using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommonJobs.JavaScript
{
    public abstract class ScriptCommand<TResult>
    {
        private string[] dependencies;
        string packageName;
        string scriptName;
        private bool prepared = false;
        public Lazy<ScriptContext> LazyContext { get; set; }
        public ScriptContext Context 
        {
            get { return LazyContext.Value; }
            set { LazyContext = new Lazy<ScriptContext>(() => value); } 
        }

        public TResult Execute()
        {
            if (!prepared)
            {
                Context.ImportDependencies(dependencies);
            }
            return Context.RunScript<TResult>(packageName, scriptName, GetParameters());
        }

        protected ScriptCommand(string packageName, string scriptName, params string[] dependencies)
        {
            this.dependencies = dependencies;
            this.packageName = packageName;
            this.scriptName = scriptName;
        }
        
        protected abstract object[] GetParameters();
    }
}