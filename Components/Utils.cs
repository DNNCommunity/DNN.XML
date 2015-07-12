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
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.FileSystem;

namespace DotNetNuke.Modules.Xml.Components
{
    public static class Utils
    {
        /// <summary>
        ///   Returns the value as string if the object isn't null or empty. Otherwise it is replaced by the default string
        /// </summary>
        /// <param name = "value"></param>
        /// <param name = "default"></param>
        /// <returns></returns>
        public static string DefaultIfNullOrEmpty(this object value, string @default = "")
        {
            if (value == null)
                return @default;
            if (ReferenceEquals(value, DBNull.Value))
                return @default;
            if (String.IsNullOrEmpty(Convert.ToString(value)))
                return @default;
            return Convert.ToString(value);
        }

        /// <summary>
        ///   determines whether the current Security grants the need permissions to connect to the url
        /// </summary>
        /// <param name = "url"></param>
        /// <returns></returns>
        /// <remarks>
        ///   Helpful in Medium Trust environment.
        /// </remarks>
        public static bool CheckWebPermission(string url)
        {
            return SecurityManager.IsGranted(new WebPermission(NetworkAccess.Connect, url));
        }

        public static bool IsOfSameTypeAs(this object a, object b)
        {
            var typeOfA = a.GetType();
            var typeOfB = b.GetType();
            return typeOfA.IsAssignableFrom(typeOfB) && typeOfB.IsAssignableFrom(typeOfA);
        }

        public static XmlReader CreateXmlReader(string xmlsrc, int portalId, bool prohibitDtd)
        {
            if (xmlsrc == String.Empty) return null;
            var filecontroller = new FileController();
            var xmlFileInfo = filecontroller.GetFileById(filecontroller.ConvertFilePathToFileId(xmlsrc, portalId), portalId);
            return XmlReader.Create(FileSystemUtils.GetFileStream(xmlFileInfo), new XmlReaderSettings {ProhibitDtd = prohibitDtd});
        }

        public static XmlReader CreateXmlReader(string xmlsrc, int portalId)
        {
            return CreateXmlReader(xmlsrc, portalId, true);
        }

        public static string GetSha256HashCode(Credential credential)
        {
            var input = credential.UserName+ credential.Password;
            return GetSha256HashCode( ref input);
        }

        public static string GetSha256HashCode(NetworkCredential credential)
        {
            var input = credential.UserName + credential.Password;
            return GetSha256HashCode(ref input);
        }

        public static string GetSha256HashCode(ref string input)
        {
            var data =new SHA256Managed().ComputeHash(Encoding.Default.GetBytes(input));
            var sBuilder = new StringBuilder();
            for (var i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

    }
}