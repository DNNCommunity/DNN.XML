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
using System.Collections;
using System.IO;
using System.Web.UI;
using DotNetNuke.Framework;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Modules;

namespace DotNetNuke.Modules.Xml.BaseControls
{
    public abstract class BaseXmlControl : UserControlBase
    {
        private ModuleInstanceContext _context;
        private string _localResourceFile;

        private IModuleControl _parentModule;

        public ModuleInstanceContext ModuleContext
        {
            get
            {
                if (_context == null)
                {
                    if (ParentModule != null)
                        _context = ParentModule.ModuleContext;
                }
                return _context;
            }
            set { _context = value; }
        }

        public int ModuleId
        {
            get { return ModuleContext.ModuleId; }
        }

        public int TabModuleId
        {
            get { return ModuleContext.TabModuleId; }
        }

        public string LocalResourceFile
        {
            get
            {
                if (string.IsNullOrEmpty(_localResourceFile))
                {
                    _localResourceFile = string.Format("{0}/{1}/{2}", TemplateSourceDirectory,
                                                       Localization.LocalResourceDirectory,
                                                       Path.GetFileName(AppRelativeVirtualPath));
                }
                return _localResourceFile;
            }
            set { _localResourceFile = value; }
        }

        public Hashtable Settings
        {
            get { return ModuleContext.Settings; }
        }

        protected IModuleControl ParentModule
        {
            get
            {
                if (_parentModule == null)
                {
                    Control ctrlCurrent = this;
                    while (ctrlCurrent != null && !(ctrlCurrent is IModuleControl))
                    {
                        ctrlCurrent = ctrlCurrent.Parent;
                    }
                    if (ctrlCurrent != null && (ctrlCurrent is IModuleControl))
                    {
                        _parentModule = (IModuleControl) ctrlCurrent;
                    }
                }
                return _parentModule;
            }
        }
    }
}