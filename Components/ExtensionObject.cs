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
using System.Web.UI;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;

namespace DotNetNuke.Modules.Xml.Components
{
    public class ExtensionObject
    {
        private readonly ModuleInfo _moduleConfiguration;
        private readonly Page _page;

        public ExtensionObject(Page page, ModuleInfo configuration)
        {
            _moduleConfiguration = configuration;
            _page = page;
        }

// ReSharper disable InconsistentNaming
        public string tokenReplace(string pattern)

        {
            var tr = new TokenReplace {ModuleInfo = _moduleConfiguration};
            return (tr.ReplaceEnvironmentTokens(pattern));
        }


        public void setPageTitle(string title)

        {
            if ((_page != null))
            {
                if ((_page) is CDefault)
                {
                    ((CDefault) _page).Title = title;
                }
                else
                {
                    _page.Title = title;
                }
            }
        }

        public void setModuleTitle(string title)
        {
            if ((_moduleConfiguration != null))
            {
                _moduleConfiguration.ModuleTitle = title;
            }
        }

        public void setPageDescription(string description)
        {
            if ((_page != null) && (_page) is CDefault)
            {
                ((CDefault) _page).Description = description;
            }
        }

        public void registerStyleSheet(string href)
        {
            if ((_page != null) && (_page) is CDefault)
            {
                ((CDefault) _page).AddStyleSheet(Globals.CreateValidID(href), href);
            }
        }

        public void registerClientScriptBlock(string id, string script)
        {
            if ((_page != null))
            {
                _page.ClientScript.RegisterClientScriptBlock(GetType(), id, script);
            }
        }

        public void registerClientScriptInclude(string id, string href)
        {
            if ((_page != null))
            {
                _page.ClientScript.RegisterClientScriptInclude(id, href);
            }
        }

        // ReSharper restore InconsistentNaming
    }
}