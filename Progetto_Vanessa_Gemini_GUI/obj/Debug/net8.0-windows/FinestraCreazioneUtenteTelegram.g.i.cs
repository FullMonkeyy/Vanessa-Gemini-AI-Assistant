﻿#pragma checksum "..\..\..\FinestraCreazioneUtenteTelegram.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "0DA6D08726CAD9A0645FFA4D103CE5F3693DB535"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Progetto_Vanessa_Gemini_GUI;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Progetto_Vanessa_Gemini_GUI {
    
    
    /// <summary>
    /// FinestraCreazioneUtenteTelegram
    /// </summary>
    public partial class FinestraCreazioneUtenteTelegram : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 23 "..\..\..\FinestraCreazioneUtenteTelegram.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox LastnameTB;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\FinestraCreazioneUtenteTelegram.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox FirstnameTB;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\FinestraCreazioneUtenteTelegram.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox PhoneNumber;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\FinestraCreazioneUtenteTelegram.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox CB_Livello;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\FinestraCreazioneUtenteTelegram.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock CodiceShow;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\FinestraCreazioneUtenteTelegram.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label LB_COUNT;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.4.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Progetto_Vanessa_Gemini_GUI;component/finestracreazioneutentetelegram.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\FinestraCreazioneUtenteTelegram.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.4.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\..\FinestraCreazioneUtenteTelegram.xaml"
            ((Progetto_Vanessa_Gemini_GUI.FinestraCreazioneUtenteTelegram)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Caricato);
            
            #line default
            #line hidden
            return;
            case 2:
            this.LastnameTB = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.FirstnameTB = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.PhoneNumber = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.CB_Livello = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 6:
            
            #line 29 "..\..\..\FinestraCreazioneUtenteTelegram.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.GeneraCodice);
            
            #line default
            #line hidden
            return;
            case 7:
            this.CodiceShow = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.LB_COUNT = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

