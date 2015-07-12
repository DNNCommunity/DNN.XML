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
using System.IO;
using System.Xml;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Modules.Xml.Providers.XmlDataProvider;
using DotNetNuke.Modules.Xml.Providers.XmlRenderingProvider;
using DotNetNuke.Services.Search;

namespace DotNetNuke.Modules.Xml.Components
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///   The Controller class for Xml Module
    /// </summary>
    /// -----------------------------------------------------------------------------
    public class FeatureController : IPortable, ISearchable
    {
        #region IPortable Members

        /// <summary>
        ///   IPortable: Export
        /// </summary>
        public string ExportModule(int moduleId)
        {
            var settings = new ModuleController().GetModuleSettings(moduleId);
            //start export
            var strXml = new StringWriter();
            XmlWriter writer = new XmlTextWriter(strXml);
            writer.WriteStartElement(Portable.ModuleElement);
            writer.WriteStartElement(Portable.SettingsElement);

            foreach (DictionaryEntry item in settings)
            {
                writer.WriteStartElement(Portable.SettingElement);
                writer.WriteAttributeString(Portable.NameAttribute, Convert.ToString(item.Key));
                writer.WriteAttributeString(Portable.ValueAttribute, Convert.ToString(item.Value));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteStartElement("XmlDataProvider");
            var nameOfSourceProvider = settings[Setting.SourceProvider].ToString();
            writer.WriteAttributeString(Portable.NameAttribute, nameOfSourceProvider);
            XmlDataProvider.Instance(nameOfSourceProvider).ExportProviderSettings(moduleId, writer);
            writer.WriteEndElement();
            writer.WriteStartElement("XmlRenderingProvider");
            var nameOfRenderingProvider = settings[Setting.RenderingProvider].ToString();
            writer.WriteAttributeString(Portable.NameAttribute, nameOfRenderingProvider);
            XmlRenderingProvider.Instance(nameOfRenderingProvider).ExportProviderSettings(moduleId, writer);
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Close();

            return strXml.ToString();
        }

        /// <summary>
        ///   IPortable:Import
        /// </summary>
        public void ImportModule(int moduleId, string content, string version, int userId)
        {
            XmlNode node;
            var nodes = Globals.GetContent(content, Portable.ModuleElement);
            var objModules = new ModuleController();
            var nameOfXmlDataProvider = string.Empty;
            var nameOfXmlRenderingProvider = string.Empty;
            foreach (XmlNode nodeLoopVariable in nodes.SelectSingleNode(Portable.SettingsElement).ChildNodes)
            {
                node = nodeLoopVariable;
                var settingName = node.Attributes[Portable.NameAttribute].Value;
                var settingValue = node.Attributes[Portable.ValueAttribute].Value;
                objModules.UpdateModuleSetting(moduleId, settingName, settingValue);
                if (settingName == Setting.SourceProvider)
                    nameOfXmlDataProvider = settingValue;
                if (settingName == Setting.RenderingProvider)
                    nameOfXmlRenderingProvider = settingValue;
            }

            XmlDataProvider.Instance(nameOfXmlDataProvider).ImportProviderSettings(moduleId, nodes.SelectSingleNode("XmlDataProvider"));
            XmlRenderingProvider.Instance(nameOfXmlRenderingProvider).ImportProviderSettings(moduleId, nodes.SelectSingleNode("XmlRenderingProvider"));

            //LEGACY SUPPORT
            XmlDataProvider.Instance("HttpRequestProvider").ImportProviderSettings(moduleId, nodes.SelectSingleNode("URL"));
            XmlRenderingProvider.Instance("XslCompiledTransformProvider").ImportProviderSettings(moduleId, nodes.SelectSingleNode("XSL"));

        }

        #endregion

        #region ISearchable Members

        ///<summary>
        ///  DotNetNuke Search support
        ///</summary>
        public SearchItemInfoCollection GetSearchItems(ModuleInfo modInfo)
        {
            var controller = new XmlBaseController(modInfo);

            var portalId = modInfo.PortalID;
            var administratorId = new PortalController().GetPortal(portalId).AdministratorId;

            var searchItemCollection = new SearchItemInfoCollection();
            if (MustAddContentToSearch(modInfo))
            {
                var sw = new StringWriter();
                controller.Render(sw);
                sw.Flush();
                var content = sw.ToString();
                var now = DateTime.Now;
                searchItemCollection.Add(new SearchItemInfo(modInfo.ModuleTitle, content, administratorId, now, modInfo.ModuleID, "", content));
                var mc = new ModuleController();
                mc.UpdateModuleSetting(modInfo.ModuleID, Setting.LastIndexRun, DateTime.Now.ToString("s"));
            }
            return searchItemCollection;
        }

        #endregion

        /// <summary>
        ///   Determines whether the module should be indexed or not.
        /// </summary>
        private static bool MustAddContentToSearch(ModuleInfo modInfo)
        {
            var settings = modInfo.ModuleSettings;
            var lastRun = DateTime.Parse(settings[Setting.LastIndexRun].DefaultIfNullOrEmpty(DateTime.MinValue.ToString("s")));
            switch (Definition.ParseIndexRun(settings[Setting.IndexRun].DefaultIfNullOrEmpty("Never")))
            {
                case IndexRun.Always:
                    return true;
                case IndexRun.Never:
                    return false;
                case IndexRun.NextRun:
                    var mc = new ModuleController();
                    mc.UpdateModuleSetting(modInfo.ModuleID, Setting.IndexRun, Definition.ConvertIndexRunToString(IndexRun.Never));
                    return true;
                case IndexRun.OncePerDay:
                    return lastRun.AddDays(1) < DateTime.Now;
                case IndexRun.OncePerHour:
                    return lastRun.AddHours(1) < DateTime.Now;
                default:
                    return false;
            }
        }
    }
}