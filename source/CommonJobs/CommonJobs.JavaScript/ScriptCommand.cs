using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommonJobs.JavaScript
{
    public abstract class ScriptCommand<TResult>
    {
        private string[] dependencies;
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
            var resultWrapper = Context.RunScript<ScriptResultWrapper<TResult>>(scriptName, GetParameters());
            if (resultWrapper.Successful)
            {
                return resultWrapper.Result;
            }
            else
            {
                throw new ScriptCommandException(resultWrapper);
            }
        }

        protected ScriptCommand(string packageName, string scriptName, params string[] dependencies)
        {
            this.dependencies = dependencies;
            this.scriptName = scriptName;
        }
        
        protected abstract object[] GetParameters();
    }
}