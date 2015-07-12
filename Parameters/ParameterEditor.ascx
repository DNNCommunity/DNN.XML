<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ParameterEditor.ascx.cs" Inherits="DotNetNuke.Modules.Xml.Parameters.ParameterEditor" %>
<%@ Import Namespace="DotNetNuke.Modules.Xml.Parameters" %>
<asp:DataGrid ID="grdParams" summary="List of Parameters" DataKeyField="ID" GridLines="None"
    BorderWidth="0px" AutoGenerateColumns="False" CellPadding="2" CssClass="Normal"
    runat="server">
    <Columns>
        <asp:TemplateColumn>
            <ItemStyle Wrap="False" VerticalAlign="Top"></ItemStyle>
            <ItemTemplate>
                <asp:ImageButton runat="server" CausesValidation="false" CommandName="Edit" ImageUrl="~/images/edit.gif"
                    AlternateText="Edit" resourcekey="Edit" ID="Imagebutton1" />
                <asp:ImageButton ID="cmdDeleteParam" runat="server" CausesValidation="false" CommandName="Delete"
                    ImageUrl="~/images/delete.gif" AlternateText="Delete" resourcekey="Delete" />
            </ItemTemplate>
            <EditItemTemplate>
                <asp:ImageButton runat="server" CausesValidation="false" CommandName="Update" ImageUrl="~/images/save.gif"
                    AlternateText="Save" resourcekey="Save" ID="Imagebutton2" />
                <asp:ImageButton runat="server" CausesValidation="false" CommandName="Cancel" ImageUrl="~/images/cancel.gif"
                    AlternateText="Cancel" resourcekey="Cancel" ID="Imagebutton3" />
            </EditItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Name">
            <HeaderStyle CssClass="NormalBold"></HeaderStyle>
            <ItemStyle CssClass="Normal" VerticalAlign="Top"></ItemStyle>
            <ItemTemplate>
                <%#((ParameterInfo) Container.DataItem).Name%>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:Label ID="lblParamName" runat="server" />
                <asp:TextBox ID="txtParamName" runat="server" MaxLength="50" Text='<%#((ParameterInfo) Container.DataItem).Name%>' />
            </EditItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Required">
            <HeaderStyle HorizontalAlign="Center" CssClass="NormalBold"></HeaderStyle>
            <ItemStyle HorizontalAlign="Center" CssClass="Normal"></ItemStyle>
            <ItemTemplate>
                <asp:Image runat="server" ImageUrl='<%#(bool) DataBinder.Eval(Container.DataItem, "IsValueRequired") ? "~/images/checked.gif" : "~/images/unchecked.gif"%>'
                    Visible='<%#RequiredValuesNeeded%>' ID="Image2" />
            </ItemTemplate>
            <EditItemTemplate>
                <asp:Label ID="lblRequired" runat="server" Visible='<%#RequiredValuesNeeded%>' />
                <asp:CheckBox runat="server" ID="chkRequired" Checked='<%#(bool) DataBinder.Eval(Container.DataItem, "IsValueRequired")%>'
                    Visible='<%#RequiredValuesNeeded%>' />
            </EditItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Value">
            <HeaderStyle CssClass="NormalBold"></HeaderStyle>
            <ItemStyle Wrap="False" CssClass="Normal" VerticalAlign="Top"></ItemStyle>
            <ItemTemplate>
                <asp:Label ID="Label1" runat="server" resourcekey='<%#((ParameterInfo) Container.DataItem).Type%>'>
					<%#((ParameterInfo) Container.DataItem).Type%>
                </asp:Label>
                <%#((ParameterInfo) Container.DataItem).IsArgumentRequired() ? "(" + Convert.ToString(((ParameterInfo) Container.DataItem).TypeArgument) + ")" : ""%>
                <%#SupportsFallbackValues && ((ParameterInfo) Container.DataItem).SupportsFallbackValue() && !String.IsNullOrEmpty(((ParameterInfo) Container.DataItem).TypeArgument)
                                        ? "(" + Localization.GetString(LocaleKeys.ParameterFallback, LocalResourceFile) + ": " + Convert.ToString(((ParameterInfo) Container.DataItem).TypeArgument) + ")"
                                        : ""%>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:Label ID="lblParamType" runat="server" />
                <asp:DropDownList ID="cboParamType" runat="server" SelectedValue='<%#((ParameterInfo) Container.DataItem).Type%>'>
                    <asp:ListItem Value="StaticValue" resourcekey="StaticValue">Static Value</asp:ListItem>
                    <asp:ListItem Value="PassThrough" resourcekey="PassThrough">QueryString Pass-Through Parameter</asp:ListItem>
                    <asp:ListItem Value="FormPassThrough" resourcekey="FormPassThrough">Form Pass-Through Parameter</asp:ListItem>
                    <asp:ListItem Value="UserCustomProperty" resourcekey="UserCustomProperty">Custom User Property</asp:ListItem>
                    <asp:ListItem Value="PortalID" resourcekey="PortalID">Portal ID</asp:ListItem>
                    <asp:ListItem Value="PortalName" resourcekey="PortalName">Portal Name</asp:ListItem>
                    <asp:ListItem Value="HomeDirectory" resourcekey="HomeDirectory">HomeDirectory</asp:ListItem>
                    <asp:ListItem Value="CurrentCulture" resourcekey="CurrentCulture">CurrentCulture</asp:ListItem>
                    <asp:ListItem Value="TabID" resourcekey="TabID">Tab ID</asp:ListItem>
                    <asp:ListItem Value="UserID" resourcekey="UserID">User ID</asp:ListItem>
                    <asp:ListItem Value="ModuleID" resourcekey="ModuleID">Module ID</asp:ListItem>
                    <asp:ListItem Value="UserUsername" resourcekey="UserUsername">User's Username</asp:ListItem>
                    <asp:ListItem Value="UserFirstName" resourcekey="UserFirstName">User's First Name</asp:ListItem>
                    <asp:ListItem Value="UserLastName" resourcekey="UserLastName">User's Last Name</asp:ListItem>
                    <asp:ListItem Value="UserFullName" resourcekey="UserFullName">User's Full Name</asp:ListItem>
                    <asp:ListItem Value="UserEmail" resourcekey="UserEmail">User's Email</asp:ListItem>
                    <asp:ListItem Value="UserWebsite" resourcekey="UserWebsite">User's Website</asp:ListItem>
                    <asp:ListItem Value="UserIM" resourcekey="UserIM">User's IM</asp:ListItem>
                    <asp:ListItem Value="UserStreet" resourcekey="UserStreet">User's Street</asp:ListItem>
                    <asp:ListItem Value="UserUnit" resourcekey="UserUnit">User's Unit</asp:ListItem>
                    <asp:ListItem Value="UserCity" resourcekey="UserCity">User's City</asp:ListItem>
                    <asp:ListItem Value="UserCountry" resourcekey="UserCountry">User's Country</asp:ListItem>
                    <asp:ListItem Value="UserRegion" resourcekey="UserRegion">User's Region</asp:ListItem>
                    <asp:ListItem Value="UserPostalCode" resourcekey="UserPostalCode">User's Postal Code</asp:ListItem>
                    <asp:ListItem Value="UserPhone" resourcekey="UserPhone">User's Phone</asp:ListItem>
                    <asp:ListItem Value="UserCell" resourcekey="UserCell">User's Cell</asp:ListItem>
                    <asp:ListItem Value="UserFax" resourcekey="UserFax">User's Fax</asp:ListItem>
                    <asp:ListItem Value="UserLocale" resourcekey="UserLocale">User's Locale</asp:ListItem>
                    <asp:ListItem Value="UserTimeZone" resourcekey="UserTimeZone">User's TimeZone</asp:ListItem>
                    <asp:ListItem Value="UserIsAuthorized" resourcekey="UserIsAuthorized">User's Authorized Flag</asp:ListItem>
                    <asp:ListItem Value="UserIsLockedOut" resourcekey="UserIsLockedOut">User's Lock Out Flag</asp:ListItem>
                    <asp:ListItem Value="UserIsSuperUser" resourcekey="UserIsSuperUser">User's SuperUser Flag</asp:ListItem>
                </asp:DropDownList>
                <asp:TextBox ID="txtParamArgument" runat="server" MaxLength="2000" Text='<%#((ParameterInfo) Container.DataItem).TypeArgument%>' />
                <asp:Label ID="lblParamScript" runat="server" />
            </EditItemTemplate>
        </asp:TemplateColumn>
    </Columns>
</asp:DataGrid>
<asp:PlaceHolder ID="ErrorMessagePlaceHolder" runat="server" />
<p>
    <asp:LinkButton ID="cmdAddParam" CssClass="CommandButton" runat="server" resourcekey="cmdAddParam"
        Text="Add New Column" CausesValidation="False"></asp:LinkButton>&nbsp;
</p>
