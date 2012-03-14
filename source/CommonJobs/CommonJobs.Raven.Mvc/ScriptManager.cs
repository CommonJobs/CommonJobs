using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;

namespace CommonJobs.Raven.Mvc
{
    public sealed class ScriptManager
    {
        private const string CURRENT_VIEWDATA_SCRIPT_MANAGER = "_COMMOMJOBS_CURRENT_VIEWDATA_SCRIPT_MANAGER_";

        List<ScriptManagerEntry> entries = new List<ScriptManagerEntry>();

        private ScriptManager()
        {
        }

        public static ScriptManager GetFromViewData(ViewDataDictionary viewData)
        {
            var scriptManager = viewData[CURRENT_VIEWDATA_SCRIPT_MANAGER] as ScriptManager;
            if (scriptManager == null)
            {
                scriptManager = new ScriptManager();
                viewData.Add(CURRENT_VIEWDATA_SCRIPT_MANAGER, scriptManager);
            }
            return scriptManager;
        }

        private void Register(ScriptManagerEntry entry)
        {
            entry.SetOrder = entries.Count;
            entries.Add(entry);
        }

        public void RegisterCss(string path, int priority = 1000, object htmlAttributes = null, bool omitAppVersion = false, string patchCondition = null)
        {
            Register(new CssReferenceEntry()
            {
                Priority = priority,
                Path = path,
                HtmlAttributes = htmlAttributes,
                OmitAppVersion = omitAppVersion,
                PatchCondition = patchCondition
            });
        }

        public void RegisterJs(string path, int priority = 1000, object htmlAttributes = null, bool omitAppVersion = false)
        {
            Register(new JsReferenceEntry()
            { 
                Priority = priority,
                Path = path,
                HtmlAttributes = htmlAttributes,
                OmitAppVersion = omitAppVersion
            });
        }

        public void RegisterGlobalJavascript(string name, object value, int priority = 1000, object htmlAttributes = null)
        {
            Register(new GlobalJavascriptEntry()
            {
                Priority = priority,
                Name = name,
                Value = value,
                HtmlAttributes = htmlAttributes
            });
        }

        //TODO: refactor this:
        public void RegisterGlobalizationEntries(string globalizeScriptFolder, CultureInfo culture = null, int priority = 1000)
        {
            if (culture == null)
                culture = System.Threading.Thread.CurrentThread.CurrentUICulture;

            Register(new GlobalizationEntries() 
            { 
                Culture = culture, 
                Priority = priority,
                GlobalizeScriptFolder = globalizeScriptFolder
            });
        }
       
        internal IEnumerable<ScriptManagerEntry> GetEntries()
        {
            return entries.OrderBy(x => x.Priority).ThenBy(x => x.SetOrder);
        }
    }
}