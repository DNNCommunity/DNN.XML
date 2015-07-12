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
using System.IO;
using System.Web.UI;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Modules.Xml.Components;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;

namespace DotNetNuke.Modules.Xml
{
    public partial class XmlModule : PortalModuleBase, IActionable
    {
        #region Private Fields

        private XmlBaseController _baseController;
        private ShowMode _currentMode = ShowMode.Inline;

        #endregion

        #region Event Handlers

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Page_Load runs when the control is loaded
        /// </summary>
        /// -----------------------------------------------------------------------------
        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                _baseController = new XmlBaseController(this);
                _currentMode = _baseController.CheckShowMode(Request.QueryString["ShowMode"].DefaultIfNullOrEmpty());
                var downloadLink = ResolveUrl("~" + Definition.PathOfModule + "download.ashx") + "?tabid=" + TabId + "&mid=" + ModuleId;

                switch (_currentMode)
                {
                    case ShowMode.Response:
                        Response.Redirect(downloadLink);
                        break;
                    case ShowMode.Link:
                        lnkShowContent.NavigateUrl = downloadLink;
                        break;
                    default:
                        using (var writer = new StringWriter())
                        {
                            _baseController.Render(writer);
                            Controls.Clear();
                            Controls.Add(new LiteralControl(writer.ToString()));
                        }
                        break;
                }
            }
            catch (SecurityException exc)
            {
                UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("CAS.ErrorMessage", LocalResourceFile), ModuleMessage.ModuleMessageType.YellowWarning);
                Exceptions.LogException(exc);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

        public XmlModule()
        {
            Load += Page_Load;
        }

        #region IActionable Members

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var actions = new ModuleActionCollection
                                  {
                                      {
                                          GetNextActionID(), Localization.GetString(ModuleActionType.AddContent, LocalResourceFile), ModuleActionType.AddContent, "", "", EditUrl(), false, SecurityAccessLevel.Edit, true, false
                                          },
                                      {
                                          GetNextActionID(), Localization.GetString("cmdShowSource", LocalResourceFile), "", "", "",
                                          ResolveUrl("~" + Definition.PathOfModule + "download.ashx") + "?showsource=true&tabid=" + TabId + "&mid=" + ModuleId, false, SecurityAccessLevel.Edit, true, true
                                          }
                                  };
                return actions;
            }
        }

        #endregion
    }
}