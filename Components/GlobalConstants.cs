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

namespace DotNetNuke.Modules.Xml.Components
{
    public static class Definition
    {
        public const string PathOfModule = "/DesktopModules/XML/";


        /// <summary>
        ///   Converts a string representation of a <see cref = "IndexRun" /> to its object value.
        /// </summary>
        /// <param name = "value">Value to convert.</param>
        public static IndexRun ParseIndexRun(string value)
        {
            var objType = default(IndexRun);
            try
            {
                objType = (IndexRun) Enum.Parse(objType.GetType(), value);
            }
            catch
            {
                objType = IndexRun.Never;
            }
            return objType;
        }


        /// <summary>
        ///   Converts the <see cref = "IndexRun" /> to a string value.
        /// </summary>
        public static string ConvertIndexRunToString(IndexRun value)
        {
            return Enum.GetName(value.GetType(), value);
        }
    }

    public class Setting
    {
        public const string SourceProvider = "XML_SourceProvider";
        public const string SourceUrl = "XML_XmlSourceUrl";
        public const string SourceAccount = "XML_XmlSourceAccount";
        public const string SourcePassWord = "XML_XmlSourcePassword";
        public const string TransUrl = "XML_XslTrans";
        public const string ContentType = "XML_ContentType";
        public const string RenderTo = "XML_RenderTo";
        public const string IndexRun = "XML_IndexRun";
        public const string LastIndexRun = "XML_LastIndexRun";
        public const string EnableParam = "XML_EnableParam";
        public const string EnableValue = "XML_EnableValue";
        public const string UrlEncoding = "XML_UrlEncoding";
        public const string SqlConnectionString = "XML_SQLConnectionString";
        public const string SqlQueryString = "XML_SQLQueryString";
        public const string SqlDataProvider = "XML_SQLDataProvider";
        public const string EnableCache = "Xml_EnableCache";

        public const string RenderingProvider = "Xml_RenderingProvider";
    }

    public class Portable
    {
        public const string ModuleElement = "xmlModule";
        public const string SettingsElement = "settings";
        public const string SettingElement = "setting";
        public const string ParamElement = "param";
        public const string NameAttribute = "name";
        public const string TypeAttribute = "type";
        public const string ArgAttribute = "arg";
        public const string ValueAttribute = "value";
        public const string ValueRequiredAttribute = "required";
        public const string SqlElement = "query";
        public const string SqlQueryAttribute = "statement";
        public const string SqlTableAttribute = "tableName";
    }

    public enum IndexRun
    {
        Never,
        NextRun,
        Always,
        OncePerHour,
        OncePerDay
    }

    public enum ShowMode
    {
        Inline,
        Link,
        Response,
        Disabled
    }
}