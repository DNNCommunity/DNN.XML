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
using System.IO;
using System.Text;
using System.Web.UI;
using System.Xml;
using System.Xml.Xsl;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Modules.Xml.Components;
using DotNetNuke.Modules.Xml.Parameters;

namespace DotNetNuke.Modules.Xml.Providers.XmlRenderingProvider.XslCompiledTransform
{
    public class Provider : XmlRenderingProvider
    {
        private readonly string _providerName;

        public Provider()
        {
            _providerName = "XSL";
        }

        private bool ProhibitDtd
        {
            get
            {
                bool prohibitDtd;
                var tryParse = bool.TryParse(GetComponentSettings()["prohibitDtd"].DefaultIfNullOrEmpty(bool.TrueString), out prohibitDtd);
                if (!tryParse) prohibitDtd = true;
                return prohibitDtd;
            }
        }
        
        private bool EnableDocument
        {
            get
            {
                bool enableDocument;
                var tryParse = bool.TryParse(GetComponentSettings()["enableDocument"].DefaultIfNullOrEmpty(bool.FalseString ), out enableDocument);
                if (!tryParse) enableDocument = false ;
                return enableDocument;
            }
        }

        private bool EnableScript
        {
            get
            {
                bool enableScript;
                var tryParse = bool.TryParse(GetComponentSettings()["enableScript"].DefaultIfNullOrEmpty(bool.FalseString), out enableScript);
                if (!tryParse) enableScript = false;
                return enableScript;
            }
        }

        public override void Render(XmlReader reader, TextWriter output, Page page, ModuleInfo moduleConfiguration)
        {
            var settings = moduleConfiguration.ModuleSettings;
            var xslTransform = GetXslTransform(settings[Setting.TransUrl].DefaultIfNullOrEmpty(), moduleConfiguration.PortalID);

            if ((xslTransform == null)) return;
            
            var sb = new StringBuilder();
               using (var xmlOutput = new StringWriter(sb))
            {
                var args = TransformArgumentList(moduleConfiguration.ModuleID);
                args.AddExtensionObject("http://www.dotnetnuke.com/xml", new ExtensionObject(page, moduleConfiguration));
                xslTransform.Transform(reader, args, xmlOutput);
            }
            var s = sb.ToString();
            output.Write(s);
        }

        private XsltArgumentList TransformArgumentList(int moduleId)
        {
            return new ParameterController(_providerName).GetParameters(moduleId).ToXsltArgumentList();
        }

        public override void ExportProviderSettings(int moduleId, XmlWriter writer)
        {
            ParameterController.ExportProviderSettings(moduleId, writer, _providerName);
        }

        public override void ImportProviderSettings(int moduleId, XmlNode settingsNode)
        {
            ParameterController.ImportProviderSettings(moduleId, settingsNode, _providerName);
        }

        /// <summary>
        ///   GetXSLContent loads the xsl content into an Xsl.XslCompiledTransform
        /// </summary>
        /// <param name = "contentUrl">The url to the xsl text</param>
        /// <param name = "prohibitDtd"></param>
        /// <returns>A XslCompiledTransform</returns>
        private static System.Xml.Xsl.XslCompiledTransform GetXslContentByWebRequest(string contentUrl, bool prohibitDtd, bool enableDocument, bool enableScript)
        {
            var xslCompiledTransform = new System.Xml.Xsl.XslCompiledTransform();
            var req = Globals.GetExternalRequest(contentUrl);
            using (var result = req.GetResponse())
            {
                using (var receiveStream = result.GetResponseStream())
                {
                    if (receiveStream != null)
                    {
                        using (var objXslTransform = XmlReader.Create(receiveStream, new XmlReaderSettings {ProhibitDtd = prohibitDtd}))
                        {
                            var settings = new XsltSettings(enableDocument, enableScript);
                            xslCompiledTransform.Load(objXslTransform, settings, new XmlUrlResolver());
                        }
                    }
                }
            }
            return xslCompiledTransform;
        }


        /// <summary>
        ///   Loads the stylesheet into the transform engine
        /// </summary>
        private System.Xml.Xsl.XslCompiledTransform GetXslTransform(string xslsrc, int portalId)
        {
            if (!string.IsNullOrEmpty(xslsrc))
            {
                switch (Globals.GetURLType(xslsrc))
                {
                    case TabType.Url:
                        return GetXslContentByWebRequest(xslsrc, ProhibitDtd, EnableDocument, EnableScript);
                    default:
                        
                        var trans = new System.Xml.Xsl.XslCompiledTransform();
                        using (var compiledStylesheet = Utils.CreateXmlReader(xslsrc, portalId, ProhibitDtd))
                        {
                            var settings = new XsltSettings(EnableDocument, EnableScript);
                            trans.Load(compiledStylesheet, settings, new XmlUrlResolver() );
                        }
                        
                        return trans;
                }
            }
            return null;
        }
    }
}