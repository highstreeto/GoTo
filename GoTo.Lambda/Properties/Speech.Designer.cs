﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GoTo.Lambda.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Speech {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Speech() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("GoTo.Lambda.Properties.Speech", typeof(Speech).Assembly);
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
        ///   Looks up a localized string similar to Hier die Verbindungen von {0} nach {1}..
        /// </summary>
        internal static string FoundTrips {
            get {
                return ResourceManager.GetString("FoundTrips", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Verbindungen von {0} nach {1}.
        /// </summary>
        internal static string FoundTripsTitle {
            get {
                return ResourceManager.GetString("FoundTripsTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Das habe ich nicht verstanden..
        /// </summary>
        internal static string InvalidRequest {
            get {
                return ResourceManager.GetString("InvalidRequest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ich konnte keine Verbindungen von {0} nach {1} finden..
        /// </summary>
        internal static string NoTripsFound {
            get {
                return ResourceManager.GetString("NoTripsFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ich suche nach Verbindungen von {0} nach {1}.
        /// </summary>
        internal static string SearchingForTrips {
            get {
                return ResourceManager.GetString("SearchingForTrips", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Mit GoTo kannst du fragen, wie du an dein Ziel gelangst mit Öffis oder Mitfahrgelegenheiten. Zum Beispiel kannst du fragen: Wie komme ich nach Hagenberg?.
        /// </summary>
        internal static string Starter {
            get {
                return ResourceManager.GetString("Starter", resourceCulture);
            }
        }
    }
}