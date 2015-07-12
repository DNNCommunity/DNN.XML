// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011 by DotNetNuke Corp. 
//  
//  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//  documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//  the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//  to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in all copies or substantial portions 
//  of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//  TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//  THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//  CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
//  DEALINGS IN THE SOFTWARE.
// 
using DotNetNuke.Entities.Modules;
using DotNetNuke.Modules.Xml.BaseControls;
using DotNetNuke.Modules.Xml.Components;
using DotNetNuke.Modules.Xml.Parameters;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.UserControls;

namespace DotNetNuke.Modules.Xml.Providers.XmlRenderingProvider.XslCompiledTransform
{
    partial class Settings : BaseSourceSettings
    {
        protected UrlControl UrlXsl;
        protected ParameterEditor PedXsl;

        public override bool IsStatic
        {
            get { return PedXsl.IsStatic; }
        }

        public override bool UrlAccessIsPermitted
        {
            get
            {
                var xslUrlIsOk = UrlXsl.UrlType != "U" || UrlXsl.Url == string.Empty || Utils.CheckWebPermission(UrlXsl.Url);
                if (!xslUrlIsOk)
                    UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("XslWebPermisssion.ErrorMessage", LocalResourceFile), ModuleMessage.ModuleMessageType.RedError);
                return xslUrlIsOk;
            }
        }

        public override void LoadSettings()
        {
            BindTransformSettings();
        }

        public override void SaveSettings()
        {
            if (Page.IsValid & UrlAccessIsPermitted)
            {
                PedXsl.SavePendingEdits(this);
                // Update settings in the database
                var objModules = new ModuleController();
                objModules.UpdateModuleSetting(ModuleId, Setting.TransUrl, UrlXsl.Url);
            }
        }

        private void BindTransformSettings()
        {
            PedXsl.ModuleContext = ModuleContext;
            if (!Page.IsPostBack)
            {
                var xslsrc = Settings[Setting.TransUrl].DefaultIfNullOrEmpty();
                UrlXsl.Url = xslsrc;
                UrlXsl.FileFilter = "xsl,xslt";
                if (string.IsNullOrEmpty(xslsrc))
                    UrlXsl.UrlType = "F";
            }
        }
    }
}