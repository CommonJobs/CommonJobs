using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommonJobs.JavaScript
{
    public abstract class ScriptCommand<TResult>
    {
        private bool prepared = false;

        private ScriptContext context;
        public ScriptContext Context 
        { 
            get { return context; }
            set 
            {
                prepared = false;
                context = value; 
            }
        }

        public TResult Execute()
        {
            if (!prepared)
            {
                Context.Import(GetDependencies());
                prepared = true;
            }
            var resultWrapper = Context.RunScript<ScriptResultWrapper<TResult>>(GetFunctionName(), GetParameters());
            if (resultWrapper.Successful)
            {
                return resultWrapper.Result;
            }
            else
            {
                throw new ScriptCommandException(resultWrapper);
            }
        }

        protected abstract object[] GetParameters();

        protected abstract string[] GetDependencies();

        protected abstract string GetFunctionName();
    }
}