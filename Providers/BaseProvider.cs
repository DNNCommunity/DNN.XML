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

#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using DotNetNuke.ComponentModel;
using DotNetNuke.Modules.Xml.Components;

#endregion

namespace DotNetNuke.Modules.Xml.Providers
{
    public abstract class BaseProvider<T>
    {
        private static Dictionary<string, T> _xmlProviders;

        #region Shared/Static Methods

        static BaseProvider()
        {
            ComponentFactory.InstallComponents(new ProviderInstaller(ConfigSection, typeof (T), ComponentLifeStyleType.Transient));
        }

        private static string ConfigSection
        {
            get
            {
                var camelCaseSection = typeof (T).Name.Replace("Provider", "");
                return char.ToLowerInvariant(camelCaseSection.First()) + camelCaseSection.Substring(1);
            }
        }

        // return the defaultprovider
        public static T Instance()
        {
            return ComponentFactory.GetComponent<T>();
        }

        // return provider by name
        public static T Instance(string name)
        {
            return ComponentFactory.GetComponent<T>(name);
        }

        public static string[] List()
        {
            return ComponentFactory.GetComponentList<T>();
        }

        public static string NameOfDefaultProvider()
        {
            _xmlProviders = ComponentFactory.GetComponents<T>();
            object defaultProvider = Instance();
            return _xmlProviders.Keys.Where(key => defaultProvider.IsOfSameTypeAs(_xmlProviders[key])).FirstOrDefault();
        }

        #endregion

        #region Public Functions

        public virtual string LocalResourceFile
        {
            get { return string.Format("{0}App_LocalResources\\Provider.resx", ProviderPath()).Replace("\\", "/").TrimStart('~'); }
        }

        public virtual string ProviderPath()
        {
            return Convert.ToString(GetComponentSettings()["providerPath"]);
        }

        public virtual string SettingsControlPath()
        {
            return string.Format("{0}settings.ascx", ProviderPath());
        }

        public IDictionary GetComponentSettings()
        {
            return ComponentFactory.GetComponentSettings(GetType());
        }

        #endregion

        #region Abstract Methods

        public virtual void ExportProviderSettings(int moduleId, XmlWriter writer)
        {
        }

        public virtual void ImportProviderSettings(int moduleId, XmlNode xmlNode)
        {
        }

        #endregion
    }
}