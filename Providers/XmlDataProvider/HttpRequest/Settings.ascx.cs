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
using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Modules.Xml.BaseControls;
using DotNetNuke.Modules.Xml.Components;
using DotNetNuke.Modules.Xml.Parameters;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;

namespace DotNetNuke.Modules.Xml.Providers.XmlDataProvider.HttpRequest
{
    public partial class Settings : BaseSourceSettings
    {
        const string FakePasswordTemplate = "@@@@@@@@";

        public Settings()
        {
            Init += Page_Init;
        }

        private ParameterEditor QueryStringParameterEditor
        {
            get { return pedSource; }
        }

        public override bool UrlAccessIsPermitted
        {
            get
            {
                var xmlUrlIsOk = ctlUrlxml.Text == string.Empty || Utils.CheckWebPermission(ctlUrlxml.Text);
                if (!xmlUrlIsOk)
                    UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("XmlWebPermisssion.ErrorMessage", LocalResourceFile), ModuleMessage.ModuleMessageType.RedError);
                return xmlUrlIsOk;
            }
        }

        public override bool IsStatic
        {
            get { return QueryStringParameterEditor.IsStatic; }
        }

        public override void LoadSettings()
        {
            var xmlsrc = Settings[Setting.SourceUrl].DefaultIfNullOrEmpty();
            ctlUrlxml.Text = xmlsrc;
            txtAccount.Text = Settings[Setting.SourceAccount].DefaultIfNullOrEmpty();
            var fakePassword = Settings[Setting.SourcePassWord].DefaultIfNullOrEmpty();
            if (fakePassword.Length > 0) fakePassword =FakePasswordTemplate;
            txtPassword.Attributes.Add("value", fakePassword);
            rblQueryStringEncoding.SelectedValue = Settings[Setting.UrlEncoding].DefaultIfNullOrEmpty("UTF8");
            cbxCaching.Checked = bool.Parse(Settings[Setting.EnableCache].DefaultIfNullOrEmpty(bool.FalseString));
        }

        public override void SaveSettings()
        {
            QueryStringParameterEditor.SavePendingEdits(this);
            var objModules = new ModuleController();
            objModules.UpdateModuleSetting(ModuleId, Setting.SourceUrl, ctlUrlxml.Text);
            objModules.UpdateModuleSetting(ModuleId, Setting.UrlEncoding, rblQueryStringEncoding.SelectedValue);
            objModules.UpdateModuleSetting(ModuleId, Setting.SourceAccount, txtAccount.Text);
            if (txtPassword.Text != FakePasswordTemplate ) objModules.UpdateModuleSetting(ModuleId, Setting.SourcePassWord, txtPassword.Text);
            objModules.UpdateModuleSetting(ModuleId, Setting.EnableCache, cbxCaching.Checked.ToString());
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            QueryStringParameterEditor.ModuleContext = ModuleContext;
        }
    }
}