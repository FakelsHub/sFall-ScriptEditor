﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.233
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ScriptEditor.Properties {
    using System;
    
    
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
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
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ScriptEditor.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
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
        
        internal static System.Drawing.Icon CodeText {
            get {
                object obj = ResourceManager.GetObject("CodeText", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
        
        internal static System.Drawing.Bitmap compiled {
            get {
                object obj = ResourceManager.GetObject("compiled", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        internal static System.Drawing.Bitmap compiled_dark {
            get {
                object obj = ResourceManager.GetObject("compiled_dark", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        internal static System.IO.UnmanagedMemoryStream CompileError {
            get {
                return ResourceManager.GetStream("CompileError", resourceCulture);
            }
        }
        
        internal static System.IO.UnmanagedMemoryStream DontFind {
            get {
                return ResourceManager.GetStream("DontFind", resourceCulture);
            }
        }
        
        internal static System.Drawing.Bitmap KeepWindowOff {
            get {
                object obj = ResourceManager.GetObject("KeepWindowOff", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        internal static System.Drawing.Bitmap KeepWindowOn {
            get {
                object obj = ResourceManager.GetObject("KeepWindowOn", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        internal static System.Drawing.Bitmap macros {
            get {
                object obj = ResourceManager.GetObject("macros", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на /*******************************************************************************
        ///        Name: 
        ///        Location: 
        ///        Description: 
        ///
        ///            Created: sFall Script Editor
        ///            Updated: 
        ///*******************************************************************************/
        ///
        ////* Include Files */
        /////#include &quot;define.h&quot;
        /////#include &quot;command.h&quot;
        ///
        ////* Defines */
        ///
        ///
        ////* Script Procedures */
        ///procedure start;
        ///
        ///
        ////* Local Variables which are saved. All Local Variables need to be prepended by LVAR [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string newScript {
            get {
                return ResourceManager.GetString("newScript", resourceCulture);
            }
        }
        
        internal static System.Drawing.Bitmap opcode {
            get {
                object obj = ResourceManager.GetObject("opcode", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        internal static System.Drawing.Bitmap procedure {
            get {
                object obj = ResourceManager.GetObject("procedure", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot; ?&gt;
        ///&lt;SyntaxDefinition name = &quot;UserWords&quot;&gt;
        ///
        ///  &lt;KeyWords name = &quot;SpecMacros&quot; bold=&quot;true&quot; italic=&quot;false&quot; bgcolor=&quot;DimGray&quot; color=&quot;White&quot;&gt;
        ///    &lt;Key word = &quot;DEBUGMSG&quot; /&gt;
        ///  &lt;/KeyWords&gt;
        ///  
        ///  &lt;KeyWords name = &quot;UserMacros&quot; bold=&quot;true&quot; italic=&quot;false&quot; color=&quot;Green&quot;&gt;
        ///    &lt;Key word = &quot;&quot; /&gt;
        ///  &lt;/KeyWords&gt;
        ///  
        ///&lt;/SyntaxDefinition&gt;
        ///.
        /// </summary>
        internal static string User_SyntaxRules {
            get {
                return ResourceManager.GetString("User_SyntaxRules", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на // The file contains a description of the user functions.
        /////
        ///// Structure:
        ///// &quot;function name&quot;&lt;d&gt;&quot;short description of the function&quot;&lt;s&gt;&quot;code that will be inserted into the script&quot;&lt;cr&gt;
        ///// &lt;m&gt; &quot;node name menu&quot;
        ///// &lt;m+&gt; &quot;name for sub node&quot;
        ///// &lt;m-&gt; close menu node.
        ///// &lt;-&gt; delimiter (only for the top level)
        ///// &lt;cr&gt; carriage return to a new line (optional)
        ///// 
        ///// In the &quot;Menu Node&quot;, &quot;function name&quot;, &quot;short description of the function&quot; and
        ///// &quot;code that will be inserted into the script&quot; can contain any ch [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string UserFunctions {
            get {
                return ResourceManager.GetString("UserFunctions", resourceCulture);
            }
        }
        
        internal static System.Drawing.Bitmap variable {
            get {
                object obj = ResourceManager.GetObject("variable", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
    }
}
