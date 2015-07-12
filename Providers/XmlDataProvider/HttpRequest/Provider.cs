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
using System.Net;
using System.Text;
using System.Web.Caching;
using System.Xml;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Modules.Xml.Components;
using DotNetNuke.Modules.Xml.Parameters;
using DotNetNuke.Services.Exceptions;
using SecurityException = System.Security.SecurityException;

namespace DotNetNuke.Modules.Xml.Providers.XmlDataProvider.HttpRequest
{
    public class Provider : XmlDataProvider
    {
        private readonly string _providerName;

        public Provider()
        {
            _providerName = "HttpRequestProvider";
        }

        public override XmlReader Load(int moduleId, int portalId, Hashtable settings)
        {
            var xmlsrc = settings[Setting.SourceUrl].DefaultIfNullOrEmpty();
            XmlReader responseReader = null;
            if (!string.IsNullOrEmpty(xmlsrc))
            {
                try
                {
                    switch (Globals.GetURLType(xmlsrc))
                    {
                        case TabType.Url:
                            {
                                var paramList = new ParameterController(_providerName).GetParameters(moduleId);
                                paramList.Encoding = GetQueryStringEncoding(settings[Setting.UrlEncoding].DefaultIfNullOrEmpty());
                                xmlsrc = new TokenReplace().ReplaceEnvironmentTokens(xmlsrc);
                                if (paramList.IsValid())
                                {
                                    xmlsrc += (xmlsrc.IndexOf("?") == -1 ? "?" : "&") + paramList;
                                    var useCache = bool.Parse(settings[Setting.EnableCache].DefaultIfNullOrEmpty(bool.FalseString));
                                    var credential = new Credential(settings[Setting.SourceAccount].DefaultIfNullOrEmpty(), settings[Setting.SourcePassWord].DefaultIfNullOrEmpty());
                                    responseReader = GetXmlContent(xmlsrc,credential , useCache);
                                }
                                else
                                {
                                    responseReader = NoDataReader("Parameter");
                                }
                            }
                            break;
                        default:
                            responseReader = NoDataReader("Unsupported Url");
                            break;
                    }
                }
                catch (SecurityException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Exceptions.LogException(ex);
                    responseReader = NoDataReader("Exception");
                }
            }
            return responseReader;
        }

        private static XmlReader NoDataReader(string message)
        {
            var x = new XmlDocument();
            x.LoadXml(string.Format("<noData cause=\"{0}\"/>", message));
            return new XmlNodeReader(x);
        }

        /// <summary>
        ///   Returns the needed <see cref = "Encoding" /> for the query string parameter out of settings.
        /// </summary>
        private static Encoding GetQueryStringEncoding(string urlEncoding)
        {
            switch (urlEncoding)
            {
                case "ASCII":
                    return Encoding.ASCII;
                case "Default":
                    return Encoding.Default;
                default:
                    return Encoding.UTF8;
            }
        }

        /// <summary>
        ///   GetXMLContent loads the xml content from a web resource into an XmlReader.
        /// </summary>
        /// <param name = "contentUrl"></param>
        /// <param name = "credential"></param>
        /// <param name="useCache"></param>
        /// <returns>A Xml Reader</returns>
        private static XmlReader GetXmlContent(string contentUrl, Credential credential , bool useCache)
        {
            var cacheKey = GetCacheKey(contentUrl, credential);
            var args = new CacheItemArgs(cacheKey, 0,CacheItemPriority.Normal, contentUrl, credential );
            var doc = useCache ? CBO.GetCachedObject<XmlDocument>(args, GetDocument) : GetDocument(args);
            return new XmlNodeReader(doc);
        }

        private static string GetCacheKey(string contentUrl,Credential credential)
        {
            return string.Format("{0}_{1}", contentUrl, Utils.GetSha256HashCode( credential  ));
        }

        private static WebResponse GetDocumentResponse(string url, Credential credential)
        {
            var req =credential==null ? Globals.GetExternalRequest(url) : Globals.GetExternalRequest(url,credential.NetworkCredential );
            return req.GetResponse();
        }

        private static XmlDocument GetDocument(CacheItemArgs args)
        {
            var url = args.ParamList[0].ToString();
            var credential = (Credential) args.ParamList[1];
            var doc = new XmlDocument();
            try
            {
                using (var rep = GetDocumentResponse(url,credential ))
                {
                    using (var receiveStream = rep.GetResponseStream())
                    {
                        if (receiveStream != null) doc.Load(receiveStream);
                    }
                }
            }
            catch (Exception ex)
            {
                //prevents caching of empty XmlDocuments
                doc = null;
                Exceptions.LogException(ex);
            }
            return doc;
        }

        public override void ExportProviderSettings(int moduleId, XmlWriter writer)
        {
            ParameterController.ExportProviderSettings(moduleId, writer, _providerName);
        }

        public override void ImportProviderSettings(int moduleId, XmlNode settingsNode)
        {
            ParameterController.ImportProviderSettings(moduleId, settingsNode, _providerName);
        }
    }
}