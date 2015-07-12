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
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Modules.Xml.BaseControls;
using DotNetNuke.Modules.Xml.Components;
using DotNetNuke.Modules.Xml.Providers.XmlDataProvider;
using DotNetNuke.Modules.Xml.Providers.XmlRenderingProvider;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.Xml
{
    public partial class EditXml : PortalModuleBase
    {
        #region Fields

        private BaseSourceSettings _renderingSettings;
        private BaseSourceSettings _sourceSettings;
        private string _localizedString;

        #endregion

        #region "Event Handlers"

        private void Page_Init(object sender, EventArgs e)
        {
            cmdUpdate.Click += cmdUpdate_Click;
            cmdCancel.Click += cmdCancel_Click;
            cmdClearSearchIndex.Click += cmdClearSearchIndex_Click;
            cmdClearEnableByParam.Click += cmdClearEnableByParam_Click;


            //datasource
            foreach (var provider in XmlDataProvider.List())
            {
                _localizedString = Localization.GetString("Provider", XmlDataProvider.Instance(provider).LocalResourceFile);
                rblXmlDataProvider.Items.Add(new ListItem(_localizedString, provider));
            }
            var xmlDataProviderName = Settings[Setting.SourceProvider].DefaultIfNullOrEmpty(XmlDataProvider.NameOfDefaultProvider());
            rblXmlDataProvider.SelectedValue = xmlDataProviderName;

            //rendering
            foreach (var provider in XmlRenderingProvider.List())
            {
                _localizedString = Localization.GetString("Provider", XmlRenderingProvider.Instance(provider).LocalResourceFile);
                rblXmlRendering.Items.Add(new ListItem(_localizedString, provider));
            }
            var xmlRenderingProviderName = Settings[Setting.RenderingProvider].DefaultIfNullOrEmpty(XmlRenderingProvider.NameOfDefaultProvider());
            rblXmlRendering.SelectedValue = xmlRenderingProviderName;
        }

        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                rblIndexRun.Enabled = true;

                LoadXmlDataProviderSettingsControl();
                LoadXmlRenderingSettingsControl();

                BindAdvancedSettings();

                //Module failed to load
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void Page_PreRender(object sender, EventArgs e)
        {
            CheckWhetherParametersAreDynamic();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect(Globals.NavigateURL(), true);
                //Module failed to load
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (Page.IsValid & UrlAccessIsPermitted())
                {
                    // Update settings in the database
                    var objModules = new ModuleController();
                    if ((_sourceSettings != null))
                        _sourceSettings.SaveSettings();
                    if ((_renderingSettings != null))
                        _renderingSettings.SaveSettings();

                    objModules.UpdateModuleSetting(ModuleId, Setting.ContentType, rblContentType.SelectedValue);
                    objModules.UpdateModuleSetting(ModuleId, Setting.IndexRun, rblIndexRun.SelectedValue);
                    objModules.UpdateModuleSetting(ModuleId, Setting.RenderTo, rblOutput.SelectedValue);
                    objModules.UpdateModuleSetting(ModuleId, Setting.EnableParam, txtEnableParam.Text);
                    objModules.UpdateModuleSetting(ModuleId, Setting.EnableValue, txtEnableValue.Text);
                    objModules.UpdateModuleSetting(ModuleId, Setting.SourceProvider, rblXmlDataProvider.SelectedValue);
                    objModules.UpdateModuleSetting(ModuleId, Setting.RenderingProvider, rblXmlRendering.SelectedValue);
                    // Redirect back to the portal home page
                    Response.Redirect(Globals.NavigateURL(), true);
                }
                //Module failed to load
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void cmdClearSearchIndex_Click(object sender, EventArgs e)
        {
            var objController = new XmlBaseController(ModuleConfiguration, Page);
            objController.ClearSearchIndex();
        }

        private void cmdClearEnableByParam_Click(object sender, EventArgs e)
        {
            txtEnableParam.Text = "";
            txtEnableValue.Text = "";
        }

        #endregion

        #region "Private Methods"

        /// <summary>
        ///   Check whether parameters are dynamic
        /// </summary>
        private void CheckWhetherParametersAreDynamic()
        {
            var indexAllowed = (_sourceSettings != null) && _sourceSettings.IsStatic && _renderingSettings.IsStatic;
            rblIndexRun.Enabled = indexAllowed;
            lblDynamicParameter.Visible = !indexAllowed;
            if (!indexAllowed)
            {
                rblIndexRun.SelectedValue = Definition.ConvertIndexRunToString(IndexRun.Never);
            }
        }

        /// <summary>
        ///   Check needed permissions  (Medium trust)
        /// </summary>
        private bool UrlAccessIsPermitted()
        {
            return _renderingSettings.UrlAccessIsPermitted & _sourceSettings.UrlAccessIsPermitted;
        }

        /// <summary>
        ///   Bind XML Source settings
        /// </summary>
        private void LoadXmlDataProviderSettingsControl()
        {
            var providerName = rblXmlDataProvider.SelectedValue;
            if (string.IsNullOrEmpty(providerName)) return;
            var provider = XmlDataProvider.Instance(providerName);
            if (provider != null)
            {
                _sourceSettings = (BaseSourceSettings) LoadControl(provider.SettingsControlPath());
                SourceSettingsPlaceHolder.Controls.Add(_sourceSettings);
                _sourceSettings.LoadSettings();
            }
            else
            {
                SourceSettingsPlaceHolder.Controls.Add(new LiteralControl(string.Format("System was not able to load Settings Control for {0}", providerName)));
            }
        }

        private void BindAdvancedSettings()
        {
            if (Page.IsPostBack) return;
            rblContentType.SelectedValue = Settings[Setting.ContentType].DefaultIfNullOrEmpty("xml|text/xml");
            rblIndexRun.SelectedValue = Settings[Setting.IndexRun].DefaultIfNullOrEmpty("Never");
            rblOutput.SelectedValue = Settings[Setting.RenderTo].DefaultIfNullOrEmpty("Inline").Replace("Default", "Inline");
            txtEnableParam.Text = Settings[Setting.EnableParam].DefaultIfNullOrEmpty();
            txtEnableValue.Text = Settings[Setting.EnableValue].DefaultIfNullOrEmpty();
        }

        private void LoadXmlRenderingSettingsControl()
        {
            var providername = rblXmlRendering.SelectedValue;
            if (string.IsNullOrEmpty(providername)) return;
            var provider = XmlRenderingProvider.Instance(providername);
            if (provider == null)
            {
                XmlRenderingPlaceHolder.Controls.Add(new LiteralControl(string.Format("System was not able to load Settings Control for {0}", providername)));
            }
            else
            {
                var loadControl1 = LoadControl(provider.SettingsControlPath());
                _renderingSettings = (BaseSourceSettings) loadControl1;
                _renderingSettings.ModuleContext = ModuleContext;
                XmlRenderingPlaceHolder.Controls.Add(_renderingSettings);
                _renderingSettings.LoadSettings();
            }
        }

        #endregion

        public EditXml()
        {
            PreRender += Page_PreRender;
            Load += Page_Load;
            Init += Page_Init;
        }
    }
}