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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace DotNetNuke.Modules.Xml.Parameters
{
    public sealed class ParameterList : List<ParameterInfo>
    {
        private Encoding _encoding = Encoding.UTF8;

        public Encoding Encoding
        {
            get { return _encoding; }
            set { _encoding = value; }
        }

        public override string ToString()
        {
            var sbValue = new StringBuilder();
            var bParameterAdded = false;
            for (var i = 0; i <= Count - 1; i++)
            {
                var param = this[i].ToString(Encoding);
                if (param.Length > 0)
                {
                    if (bParameterAdded)
                        sbValue.Append("&");
                    sbValue.Append(param);
                    bParameterAdded = true;
                }
            }
            return sbValue.ToString();
        }

        public XsltArgumentList ToXsltArgumentList()
        {
            var xslArg = new XsltArgumentList();
            foreach (var param in this)
            {
                if (param.IsValidDefinition)
                {
                    var value = param.GetValue();
                    if (value.Length > 0)
                    {
                        var name = param.Name;
                        // ReSharper disable AssignNullToNotNullAttribute
                        if (name != null) xslArg.AddParam(XmlConvert.EncodeName(name), string.Empty, param.GetValue());
                        // ReSharper restore AssignNullToNotNullAttribute
                    }
                }
            }
            return xslArg;
        }

        public bool IsStatic()
        {
            return Count == 0 || this.All(param => param.IsStatic);
        }

        public bool IsValid()
        {
            return Count == 0 || this.All(param => param.IsValidValue);
        }
    }
}