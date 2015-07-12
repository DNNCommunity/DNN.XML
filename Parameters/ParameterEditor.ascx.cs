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
using System.Web.UI.WebControls;
using DotNetNuke.Modules.Xml.BaseControls;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;

namespace DotNetNuke.Modules.Xml.Parameters
{
    public partial class ParameterEditor : BaseXmlControl
    {
        #region Sub-Classes

        #region Nested type: Constants

        /// <summary>
        ///   Represents misc constants for the <see cref = "ParameterEditor" /> control.
        /// </summary>
        private static class Constants
        {
            public const string TableHeadScope = "scope";
            public const string TableHeadRowScope = "row";
            public const string TableHeadColScope = "col";
        }

        #endregion

        #region Nested type: ControlNames

        /// <summary>
        ///   Represents child control names for the <see cref = "ParameterEditor" /> control.
        /// </summary>
        private static class ControlNames
        {
            public const string ParameterDeleteButton = "cmdDeleteParam";
            public const string ParameterNameLabel = "lblParamName";
            public const string ParameterName = "txtParamName";
            public const string ParameterTypeLabel = "lblParamType";
            public const string ParameterType = "cboParamType";
            public const string ParameterArgumentLabel = "lblParamArgument";
            public const string ParameterArgument = "txtParamArgument";
            public const string ParameterScript = "lblParamScript";
            public const string ParameterIsValueRequired = "chkRequired";
            public const string ParameterIsValueRequiredLabel = "lblRequired";
        }

        #endregion

        #region Nested type: LocaleKeys

        /// <summary>
        ///   Represents localization keys for the <see cref = "ParameterEditor" /> control.
        /// </summary>
        protected static class LocaleKeys
        {
            public const string ParameterNameHeader = "Name.Header";
            public const string ParameterTypeHeader = "Type.Header";
            public const string ParameterArgumentHeader = "Argument.Header";
            public const string ParameterDeleteConfirmation = "DeleteParamConfirmation";
            public const string ParameterIsValueRequiredHeader = "IsValueRequired.Header";
            public const string ParameterInvalidHeader = "ParameterInvalid.Header";
            public const string ParameterInvalid = "ParameterInvalid";
            public const string ParameterFallback = "ParameterFallback";
        }

        #endregion

        #endregion

        private bool _isStatic = true;

        public ParameterEditor()
        {
            SupportsFallbackValues = false;
            RequiredValuesNeeded = false;
            Purpose = string.Empty;
            Load += Page_Load;
            Init += Page_Init;
        }

        public string Purpose { get; set; }

        public bool IsStatic
        {
            get { return _isStatic; }
        }

        public bool RequiredValuesNeeded { get; set; }
        public bool SupportsFallbackValues { get; set; }


        protected void GrdParams_CancelCommand(object source, DataGridCommandEventArgs e)
        {
            try
            {
                grdParams.EditItemIndex = -1;
                BindParameters();
                //Module failed to load
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void GrdParams_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            try
            {
                // init vars
                var objController = new ParameterController(Purpose);
                var objParamKey = new ParameterInfo.UniqueKey
                                      {
                                          Id = Convert.ToInt32(grdParams.DataKeys[e.Item.ItemIndex])
                                      };

                // delete parameter
                objController.DeleteParameter(objParamKey);

                // reset edit row
                grdParams.EditItemIndex = -1;
                BindParameters();
                //Module failed to load
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void GrdParams_EditCommand(object source, DataGridCommandEventArgs e)
        {
            try
            {
                SaveParameterEditRow(grdParams);
                grdParams.EditItemIndex = e.Item.ItemIndex;
                grdParams.SelectedIndex = -1;
                BindParameters();
                //Module failed to load
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void GrdParams_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            try
            {
                // add delete confirmation
                var cmdDeleteParam = e.Item.FindControl(ControlNames.ParameterDeleteButton);
                if ((cmdDeleteParam != null))
                {
                    ClientAPI.AddButtonConfirm((WebControl) cmdDeleteParam,
                                               Localization.GetString(LocaleKeys.ParameterDeleteConfirmation,
                                                                      LocalResourceFile));
                }

                // add accessible column headers
                if (e.Item.ItemType == ListItemType.Header)
                {
                    e.Item.Cells[1].Attributes.Add(Constants.TableHeadScope, Constants.TableHeadColScope);
                    e.Item.Cells[2].Attributes.Add(Constants.TableHeadScope, Constants.TableHeadColScope);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void GrdParams_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            try
            {
                const string labelFormat = "<label style=\"display:none;\" for=\"{0}\">{1}</label>";
                var objItemType = e.Item.ItemType;
                if (objItemType == ListItemType.EditItem)
                {
                    // name
                    var ctlLabel = (Label) e.Item.FindControl(ControlNames.ParameterNameLabel);
                    var ctlSetting = e.Item.FindControl(ControlNames.ParameterName);
                    ctlLabel.Text = string.Format(labelFormat, ctlSetting.ClientID,
                                                  Localization.GetString(LocaleKeys.ParameterNameHeader,
                                                                         LocalResourceFile));

                    //required
                    ctlLabel = (Label) e.Item.FindControl(ControlNames.ParameterIsValueRequiredLabel);
                    ctlSetting = e.Item.FindControl(ControlNames.ParameterIsValueRequired);
                    ctlLabel.Text = string.Format(labelFormat, ctlSetting.ClientID,
                                                  Localization.GetString(LocaleKeys.ParameterIsValueRequiredHeader,
                                                                         LocalResourceFile));

                    // type - also add javascript to show/hide arg textbox
                    ctlLabel = (Label) e.Item.FindControl(ControlNames.ParameterTypeLabel);
                    ctlSetting = e.Item.FindControl(ControlNames.ParameterType);
                    ctlLabel.Text = string.Format(labelFormat, ctlSetting.ClientID,
                                                  Localization.GetString(LocaleKeys.ParameterTypeHeader,
                                                                         LocalResourceFile));
                    var typeId = ctlSetting.ClientID;

                    ((WebControl) ctlSetting).Attributes.Add("onblur", "ParameterEditor_showArgument();");
                    ((WebControl) ctlSetting).Attributes.Add("onchange", "ParameterEditor_showArgument();");

                    // argument - also add javascript to set default visiblity based on type
                    ctlSetting = e.Item.FindControl(ControlNames.ParameterArgument);
                    var argId = ctlSetting.ClientID;

                    // add javascript
                    ctlLabel = (Label) e.Item.FindControl(ControlNames.ParameterScript);
                    if (SupportsFallbackValues)
                    {
                        ctlLabel.Text =
                            @"<script type=""text/javascript"">ParameterEditor_showArgument();function ParameterEditor_showArgument(){document.getElementById('" +
                            argId + @"').style.display=((document.getElementById('" + typeId +
                            @"').selectedIndex<4)||(document.getElementById('" + typeId +
                            @"').selectedIndex>9)?'inline':'none');}</script>";
                    }
                    else
                    {
                        ctlLabel.Text =
                            @"<script type=""text/javascript"">ParameterEditor_showArgument();function ParameterEditor_showArgument(){document.getElementById('" +
                            argId + @"').style.display=((document.getElementById('" + typeId +
                            @"').selectedIndex<4)?'inline':'none');}</script>";
                    }
                }
                //Module failed to load
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void GrdParams_UpdateCommand(object source, DataGridCommandEventArgs e)
        {
            try
            {
                // init vars
                var objParam = new ParameterInfo();
                // set values
                if (e.Item.ItemIndex > -1)
                    objParam.Id = Convert.ToInt32(grdParams.DataKeys[e.Item.ItemIndex]);
                objParam.ModuleId = ModuleId;
                objParam.Name = ((TextBox) e.Item.FindControl(ControlNames.ParameterName)).Text;
                objParam.Type = ((DropDownList) e.Item.FindControl(ControlNames.ParameterType)).SelectedValue;
                if (objParam.IsArgumentRequired() || (SupportsFallbackValues && objParam.SupportsFallbackValue()))
                    objParam.TypeArgument = ((TextBox) e.Item.FindControl(ControlNames.ParameterArgument)).Text;
                objParam.IsValueRequired =
                    ((CheckBox) e.Item.FindControl(ControlNames.ParameterIsValueRequired)).Checked;
                // add/update param
                if (objParam.IsValidDefinition)
                {
                    var objController = new ParameterController(Purpose);
                    if (objParam.IsNew)
                    {
                        objController.AddParameter(objParam);
                    }
                    else
                    {
                        objController.UpdateParameter(objParam);
                    }

                    // clear edit row
                    grdParams.EditItemIndex = -1;
                    // bind data
                    BindParameters();
                }
                else
                {
                    ErrorMessagePlaceHolder.Controls.Add(
                        UI.Skins.Skin.GetModuleMessageControl(
                            Localization.GetString(LocaleKeys.ParameterInvalidHeader, LocalResourceFile),
                            Localization.GetString(LocaleKeys.ParameterInvalid, LocalResourceFile),
                            ModuleMessage.ModuleMessageType.RedError));
                }

                //Module failed to load
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void CmdAdd_ParamClick(object sender, EventArgs e)
        {
            try
            {
                // save edit row
                SaveParameterEditRow(cmdAddParam);

                // add item
                grdParams.EditItemIndex = -1;
                BindParameters(true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            Localization.LocalizeDataGrid(ref grdParams, LocalResourceFile);
            BindParameters();
        }

        private void Page_Init(object sender, EventArgs e)
        {
            grdParams.CancelCommand += GrdParams_CancelCommand;
            grdParams.DeleteCommand += GrdParams_DeleteCommand;
            grdParams.EditCommand += GrdParams_EditCommand;
            grdParams.ItemCreated += GrdParams_ItemCreated;
            grdParams.ItemDataBound += GrdParams_ItemDataBound;
            grdParams.UpdateCommand += GrdParams_UpdateCommand;
            cmdAddParam.Click += CmdAdd_ParamClick;
        }

        /// <summary>
        ///   Binds the <see cref = "ParameterInfo" /> settings.
        /// </summary>
        /// <param name = "showAddRow">Specifies whether an additional edit row should be displayed.</param>
        private void BindParameters(bool showAddRow = false)
        {
            var objController = new ParameterController(Purpose);
            var colParams = objController.GetParameters(ModuleId);
            _isStatic = colParams.IsStatic();

            // add new row
            if (showAddRow)
            {
                colParams.Add(new ParameterInfo());
                grdParams.EditItemIndex = colParams.Count - 1;
            }

            // apply data source
            grdParams.DataSource = colParams;
            grdParams.DataBind();
            grdParams.Visible = (colParams.Count > 0 || showAddRow);
            grdParams.Columns[2].Visible = grdParams.Visible && RequiredValuesNeeded;

            // bind settings
        }

        /// <summary>
        ///   Saves the <see cref = "grdParams" /> edit row.
        /// </summary>
        /// <param name = "trigger">Object initiating the save.</param>
        private void SaveParameterEditRow(object trigger)
        {
            SavePendingEdits(trigger);
        }

        public void SavePendingEdits(object source)
        {
            if (grdParams.EditItemIndex <= -1) return;
            var ie = new DataGridCommandEventArgs(grdParams.Items[grdParams.EditItemIndex], source,
                                                  new CommandEventArgs("Update", null));
            GrdParams_UpdateCommand(source, ie);
        }
    }
}