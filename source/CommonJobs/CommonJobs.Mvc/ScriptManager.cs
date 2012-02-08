using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommonJobs.Mvc
{
    public class ScriptManager
    {
        private const string CURRENT_VIEWDATA_SCRIPT_MANAGER = "_COMMOMJOBS_CURRENT_VIEWDATA_SCRIPT_MANAGER_";

        List<ScriptManagerEntry> entries = new List<ScriptManagerEntry>();

        internal static ScriptManager GetFromViewData(ViewDataDictionary viewData)
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

        public void RegisterCss(string path, int priority = 1000, object htmlAttributes = null, bool omitAppVersion = false)
        {
            Register(new CssReferenceEntry()
            {
                Priority = priority,
                Path = path,
                HtmlAttributes = htmlAttributes,
                OmitAppVersion = omitAppVersion
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

        internal IEnumerable<ScriptManagerEntry> GetEntries()
        {
            return entries.OrderBy(x => x.Priority).ThenBy(x => x.SetOrder);
        }
    }
}