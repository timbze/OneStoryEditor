﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AiChorus.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("AiChorus.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to AdaptIt Chorus.
        /// </summary>
        internal static string AiChorusCaption {
            get {
                return ResourceManager.GetString("AiChorusCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to LanguageDepot.org [resumable sync].
        /// </summary>
        internal static string IDS_DefaultRepoServer {
            get {
                return ResourceManager.GetString("IDS_DefaultRepoServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This only works if you choose an Adapt It Lookup Converter! Do you want to try again?.
        /// </summary>
        internal static string MustUseAiLookupConverterToEdit {
            get {
                return ResourceManager.GetString("MustUseAiLookupConverterToEdit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Usage: &apos;AiChorus /f&apos; to open the project editor, &apos;AiChorus /c&apos; to download (clone) a project, &apos;AiChorus (/s)&apos; to synchronize a project, and &apos;AiChorus /e&apos; to edit the knowledge base.
        /// </summary>
        internal static string UsageString {
            get {
                return ResourceManager.GetString("UsageString", resourceCulture);
            }
        }
    }
}
