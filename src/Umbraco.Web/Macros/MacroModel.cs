﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using umbraco.cms.businesslogic.macro;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Macro = Umbraco.Core.Models.Macro;

namespace Umbraco.Web.Macros
{
    public class MacroModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public string MacroControlIdentifier { get; set; }

        public MacroTypes MacroType { get; set; }

        // that one was for CustomControls which are gone in v8
        //public string TypeAssembly { get; set; }

        public string TypeName { get; set; }

        public string Xslt { get; set; }

        public string ScriptName { get; set; }

        public string ScriptCode { get; set; }

        public string ScriptLanguage { get; set; }

        public int CacheDuration { get; set; }

        public bool CacheByPage { get; set; }

        public bool CacheByMember { get; set; }

        public bool RenderInEditor { get; set; }

        public string CacheIdentifier { get; set; }

        public List<MacroPropertyModel> Properties { get; } = new List<MacroPropertyModel>();

        public MacroModel()
        { }

        public MacroModel(IMacro macro)
        {
            if (macro == null) return;

            Id = macro.Id;
            Name = macro.Name;
            Alias = macro.Alias;
            //TypeAssembly = macro.ControlAssembly;
            TypeName = macro.ControlType;
            Xslt = macro.XsltPath;
            ScriptName = macro.ScriptPath;
            CacheDuration = macro.CacheDuration;
            CacheByPage = macro.CacheByPage;
            CacheByMember = macro.CacheByMember;
            RenderInEditor = macro.UseInEditor;

            foreach (var prop in macro.Properties)
                Properties.Add(new MacroPropertyModel(prop.Alias, string.Empty, prop.EditorAlias));

            // can convert enums
            MacroType = Core.Services.MacroService.GetMacroType(macro);
        }

        // fixme what's the point? used only in tests!
        internal static MacroTypes FindMacroType(string xslt, string scriptFile, string scriptType /*, string scriptAssembly*/)
        {
            if (string.IsNullOrEmpty(xslt) == false)
                return MacroTypes.Xslt;

            if (string.IsNullOrEmpty(scriptFile) == false)
            {
                //we need to check if the file path saved is a virtual path starting with ~/Views/MacroPartials, if so then this is 
                //a partial view macro, not a script macro
                //we also check if the file exists in ~/App_Plugins/[Packagename]/Views/MacroPartials, if so then it is also a partial view.
                return (scriptFile.InvariantStartsWith(SystemDirectories.MvcViews + "/MacroPartials/")
                        || (Regex.IsMatch(scriptFile, "~/App_Plugins/.+?/Views/MacroPartials", RegexOptions.Compiled | RegexOptions.IgnoreCase)))
                           ? MacroTypes.PartialView
                           : MacroTypes.Script;
            }

            if (string.IsNullOrEmpty(scriptType) == false && scriptType.InvariantContains(".ascx"))
                return MacroTypes.UserControl;

            //if (string.IsNullOrEmpty(scriptType) == false && !string.IsNullOrEmpty(scriptAssembly))
            //    return MacroTypes.CustomControl;

            return MacroTypes.Unknown;
        }

    }
}
