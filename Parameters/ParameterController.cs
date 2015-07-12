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
using System.Data;
using System.Xml;
using DotNetNuke.Data;
using DotNetNuke.Modules.Xml.Components;

namespace DotNetNuke.Modules.Xml.Parameters
{
    public class ParameterController
    {
        #region Fields

        private readonly string _purpose;

        #endregion

        #region Initialisation

        public ParameterController()
        {
            _purpose = "";
        }

        public ParameterController(string purpose)
        {
            _purpose = purpose;
        }

        #endregion

        #region Data Access

        /// <summary>
        ///   Creates a new  object in the data store.
        /// </summary>
        /// <param name = "parameter">Parameter object.</param>
        public void AddParameter(ParameterInfo parameter)
        {
            DataProvider.Instance().ExecuteNonQuery("Xml_Parameter_Add", parameter.ModuleId, _purpose, parameter.Name,
                                                    parameter.Type, parameter.TypeArgument, parameter.IsValueRequired);
        }


        /// <summary>
        ///   Retrieves an existing  object from the data store.
        /// </summary>
        /// <param name = "key">Parameter identifier.</param>
        public ParameterInfo GetParameter(ParameterInfo.UniqueKey key)
        {
            ParameterInfo objParam;
            using (var objReader = DataProvider.Instance().ExecuteReader("Xml_Parameter_Get", key.Id))
            {
                objParam = objReader.Read() ? FillParameterInfo(objReader) : new ParameterInfo();
            }
            return objParam;
        }


        /// <summary>
        ///   Retrieves a collection of  objects from the data store.
        /// </summary>
        /// <param name = "moduleId">Module identifier.</param>
        public ParameterList GetParameters(int moduleId)
        {
            var colParams = new ParameterList();

            using (
                var objReader = DataProvider.Instance().ExecuteReader("Xml_Parameter_GetList", moduleId, _purpose))
            {
                // loop thru data
                while (objReader.Read())
                {
                    colParams.Add(FillParameterInfo(objReader));
                }
            }

            // return
            return colParams;
        }

        /// <summary>
        ///   Updates an existing  object in the data store.
        /// </summary>
        /// <param name = "parameter">Parameter object.</param>
        public void UpdateParameter(ParameterInfo parameter)
        {
            DataProvider.Instance().ExecuteNonQuery("Xml_Parameter_Update", parameter.Id, parameter.Name,
                                                    parameter.Type, parameter.TypeArgument,
                                                    parameter.IsValueRequired);
        }


        /// <summary>
        ///   Removes an existing  object from the data store.
        /// </summary>
        /// <param name = "key">Parameter identifier.</param>
        public void DeleteParameter(ParameterInfo.UniqueKey key)
        {
            DataProvider.Instance().ExecuteNonQuery("Xml_Parameter_Delete", key.Id);
        }

        public static void ExportProviderSettings(int moduleId, XmlWriter writer, string providerName)
        {
            foreach (var param in new ParameterController(providerName).GetParameters(moduleId))
            {
                writer.WriteStartElement(Portable.ParamElement);
                writer.WriteAttributeString(Portable.NameAttribute, param.Name);
                writer.WriteAttributeString(Portable.TypeAttribute, param.Type);
                if (param.IsArgumentRequired())
                    writer.WriteAttributeString(Portable.ArgAttribute, param.TypeArgument);
                if (param.IsValueRequired)
                    writer.WriteAttributeString(Portable.ValueRequiredAttribute, "true");
                writer.WriteEndElement();
            }
        }

        public static void ImportProviderSettings(int moduleId, XmlNode settingsNode, string providerName)
        {
            if ((settingsNode != null))
            {
                var pc = new ParameterController(providerName);
                foreach (var p in pc.GetParameters(moduleId))
                {
                    pc.DeleteParameter(p.Key);
                }
                foreach (XmlNode node in settingsNode.SelectNodes(Portable.ParamElement))
                {
                    if (node.Attributes == null) continue;
                    var p = new ParameterInfo
                                {
                                    ModuleId = moduleId,
                                    Name = node.Attributes["name"].Value,
                                    Type = node.Attributes[Portable.TypeAttribute].Value
                                };
                    if (p.IsArgumentRequired())
                        p.TypeArgument = node.Attributes[Portable.ArgAttribute].Value;
                    p.IsValueRequired = (node.Attributes[Portable.ValueRequiredAttribute] != null);
                    pc.AddParameter(p);
                }
            }
        }

        #endregion

        private static ParameterInfo FillParameterInfo(IDataReader objreader)
        {
            var objParam = new ParameterInfo
                               {
                                   Id = objreader.GetInt32(0),
                                   ModuleId = objreader.GetInt32(1),
                                   Name = objreader.GetString(2),
                                   Type = objreader.GetString(3)
                               };
            if (!objreader.IsDBNull(4))
                objParam.TypeArgument = objreader.GetString(4);
            objParam.IsValueRequired = objreader.GetBoolean(5);
            return objParam;
        }
    }
}