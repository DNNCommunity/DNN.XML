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
using System.Collections.Specialized;
using System.Globalization;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.Tokens;

namespace DotNetNuke.Modules.Xml.Components
{
    public class FilteredNameValueCollectionPropertyAccess : IPropertyAccess
    {
        private readonly NameValueCollection _nameValueCollection;

        public FilteredNameValueCollectionPropertyAccess(NameValueCollection list)
        {
            _nameValueCollection = list;
        }

        #region IPropertyAccess Members

        public CacheLevel Cacheability
        {
            get { return CacheLevel.notCacheable; }
        }

        public string GetProperty(string strPropertyName, string strFormat, CultureInfo formatProvider,
                                  UserInfo accessingUser, Scope accessLevel, ref bool propertyNotFound)
        {
            if (_nameValueCollection == null)
                return string.Empty;
            var value = _nameValueCollection[strPropertyName];

            if (string.IsNullOrEmpty(strFormat)) strFormat = string.Empty;
            if (value != null)
            {
                var security = new PortalSecurity();
                value = security.InputFilter(value, PortalSecurity.FilterFlag.NoScripting);
                return security.InputFilter(PropertyAccess.FormatString(value, strFormat),
                                            PortalSecurity.FilterFlag.NoScripting);
            }
            else
            {
                propertyNotFound = true;
                return string.Empty;
            }
        }

        #endregion
    }
}