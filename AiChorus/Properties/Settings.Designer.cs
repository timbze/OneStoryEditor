﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AiChorus.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.8.1.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string LastProjectFolder {
            get {
                return ((string)(this["LastProjectFolder"]));
            }
            set {
                this["LastProjectFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool UpgradeSettings {
            get {
                return ((bool)(this["UpgradeSettings"]));
            }
            set {
                this["UpgradeSettings"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>LanguageDepot.org [resumable sync]</string>
  <string>http://resumable.languagedepot.org</string>
  <string>LanguageDepot.org [private]</string>
  <string>http://hg-private.languagedepot.org</string>
  <string>LanguageDepot.org [legacy sync]</string>
  <string>http://hg-public.languagedepot.org</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection ServerLabelsToUrls {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["ServerLabelsToUrls"]));
            }
            set {
                this["ServerLabelsToUrls"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("hqMOEEijf4CWzwvir5l/DALL9DspvG52f5JApiquXyLtpPgKGTNNtJmoFSyeKcK+VaJm88/tCaHGwW30j" +
            "W7T4jqYsNdCt5kE0uajYg3VQLE=")]
        public string GoogleSheetsClientIdEncrypted {
            get {
                return ((string)(this["GoogleSheetsClientIdEncrypted"]));
            }
            set {
                this["GoogleSheetsClientIdEncrypted"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("zwyP9UhVu5PidI+xrb0to7lou92s7O/cbS9nCD1mClU=")]
        public string GoogleSheetsClientSecretEncrypted {
            get {
                return ((string)(this["GoogleSheetsClientSecretEncrypted"]));
            }
            set {
                this["GoogleSheetsClientSecretEncrypted"] = value;
            }
        }
    }
}
