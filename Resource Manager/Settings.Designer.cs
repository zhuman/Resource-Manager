﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Resource_Manager {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.1.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\n<ArrayOfString xmlns:xsi=\"http://www.w3.o" +
            "rg/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" />")]
        public global::System.Collections.Specialized.StringCollection RecentFiles {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["RecentFiles"]));
            }
            set {
                this["RecentFiles"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string lastOpenedPath {
            get {
                return ((string)(this["lastOpenedPath"]));
            }
            set {
                this["lastOpenedPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string lastConvertedPath {
            get {
                return ((string)(this["lastConvertedPath"]));
            }
            set {
                this["lastConvertedPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string lastExportedPath {
            get {
                return ((string)(this["lastExportedPath"]));
            }
            set {
                this["lastExportedPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ExtractionAutoDecompress {
            get {
                return ((bool)(this["ExtractionAutoDecompress"]));
            }
            set {
                this["ExtractionAutoDecompress"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ExtractionOneFolder {
            get {
                return ((bool)(this["ExtractionOneFolder"]));
            }
            set {
                this["ExtractionOneFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ExtractionSavePNGasBMP {
            get {
                return ((bool)(this["ExtractionSavePNGasBMP"]));
            }
            set {
                this["ExtractionSavePNGasBMP"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ExtractionAutoXMBConversion {
            get {
                return ((bool)(this["ExtractionAutoXMBConversion"]));
            }
            set {
                this["ExtractionAutoXMBConversion"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ExtractionAutoDDTToPNGConversion {
            get {
                return ((bool)(this["ExtractionAutoDDTToPNGConversion"]));
            }
            set {
                this["ExtractionAutoDDTToPNGConversion"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ExtractionAutoDDTToTGAConversion {
            get {
                return ((bool)(this["ExtractionAutoDDTToTGAConversion"]));
            }
            set {
                this["ExtractionAutoDDTToTGAConversion"] = value;
            }
        }
    }
}
