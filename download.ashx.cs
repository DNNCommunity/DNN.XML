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
using System.Collections;
using System.Web;
using System.Xml;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Modules.Xml.Components;
using DotNetNuke.Modules.Xml.Providers.XmlDataProvider;
using DotNetNuke.Security.Permissions;

namespace DotNetNuke.Modules.Xml
{
    public class Download : IHttpHandler
    {
        private static void RenderToResponseStream(HttpResponse response, XmlBaseController baseController)
        {
            // save script timeout
            var scriptTimeOut = HttpContext.Current.Server.ScriptTimeout;
            // temporarily set script timeout to large value ( this value is only applicable when application is not running in Debug mode )
            HttpContext.Current.Server.ScriptTimeout = int.MaxValue;
            response.ContentType = baseController.ContentType;
            response.AppendHeader("content-disposition", "inline; filename=" + baseController.FileName);
            baseController.Render(response.OutputStream);
            response.Flush();
            // reset script timeout
            HttpContext.Current.Server.ScriptTimeout = scriptTimeOut;
        }

        private static void ShowSource(HttpResponse response, int moduleId, int portalId, Hashtable settings)
        {
            if (response == null) throw new ArgumentNullException("response");
            var scriptTimeOut = HttpContext.Current.Server.ScriptTimeout;
            // temporarily set script timeout to large value ( this value is only applicable when application is not running in Debug mode )
            HttpContext.Current.Server.ScriptTimeout = int.MaxValue;
            response.ContentType = "xml|text/xml";
            response.AppendHeader("content-disposition", "inline; filename=" + "datasource.xml");
            using (var writer = XmlWriter.Create(response.OutputStream))
            {
                var providerName = settings[Setting.SourceProvider].DefaultIfNullOrEmpty();
                if (providerName != string.Empty)
                    writer.WriteNode(XmlDataProvider.Instance(providerName).Load(moduleId, portalId, settings), false);
                writer.Flush();
            }
            response.Flush();
            // reset script timeout
            HttpContext.Current.Server.ScriptTimeout = scriptTimeOut;
        }

        #region " WebHandler "

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                var portalSettings = PortalController.Instance.GetCurrentPortalSettings();

                if (context.Request.QueryString["tabid"] == null || context.Request.QueryString["mid"] == null)
                    return;
                // get TabId
                var tabId = -1;
                if ((context.Request.QueryString["tabid"] != null))
                {
                    tabId = Int32.Parse(context.Request.QueryString["tabid"]);
                }

                // get ModuleId
                var moduleId = -1;
                if ((context.Request.QueryString["mid"] != null))
                {
                    moduleId = Int32.Parse(context.Request.QueryString["mid"]);
                }

                UserController.Instance.GetCurrentUserInfo();
                
                var moduleInfo = ModuleController.Instance.GetModule(moduleId, tabId, false);
                var settings = moduleInfo.ModuleSettings;

                if (context.Request.QueryString["showsource"] == null)
                {
                    if (ModulePermissionController.CanViewModule(moduleInfo))
                    {
                        RenderToResponseStream(context.Response, new XmlBaseController(moduleInfo));
                    }
                }
                else
                {
                    if (ModulePermissionController.CanManageModule(moduleInfo))
                    {
                        ShowSource(context.Response, moduleId, portalSettings.PortalId, settings);
                    }
                }
            }
            catch (Exception)
            {
                context.Response.Write("Not defined");
            }
        }

        #endregion
    }
}