<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="DotNetNuke.Modules.Xml.Providers.XmlDataProvider.HttpRequest.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="ParameterEditor" Src="~/DesktopModules/xml/Parameters/ParameterEditor.ascx" %>

<asp:Label ID="Label1" runat="server" ResourceKey="lblUrlLocation" class="NormalBold">Location: ( Enter The Address Of The Link ): </asp:Label><br />
<asp:TextBox ID="ctlUrlxml" runat="server" Width="300" />
<div id="divUrlSettings" runat="server" style="margin-top: 10px; margin-bottom: 10px">
    <table>
        <tr>
            <td class="SubHead">
                <dnn:Label ID="plCaching" Suffix=":" ControlName="cbxCaching" runat="server" Text="Enable Caching of DataSource" />
            </td>
            <td>
                <asp:CheckBox ID="cbxCaching" runat="server" />
            </td>
        </tr>
    </table>
    <dnn:SectionHead ID="dshUrlParams" runat="server" IsExpanded="true" IncludeRule="false"
        ResourceKey="dshUrlParams" Section="sectXmlParam" CssClass="SubHead"></dnn:SectionHead>
    <div id="sectXmlParam" runat="server">
        <dnn:ParameterEditor ID="pedSource" runat="server" Purpose="HttpRequestProvider" RequiredValuesNeeded="true"
            SupportsFallbackValues="true" />
    </div>
    <dnn:SectionHead ID="dshQueryStringEncoding" runat="server" IsExpanded="false" IncludeRule="false"
        ResourceKey="dshQueryStringEncoding" Section="sectQueryStringEncoding" CssClass="SubHead">
    </dnn:SectionHead>
    <div id="sectQueryStringEncoding" runat="server" class="normalBold">
        <dnn:Label ID="plQueryStringEncoding" runat="server" ControlName="rblQueryStringEncoding"
            Suffix=":"></dnn:Label>
        <asp:RadioButtonList ID="rblQueryStringEncoding" runat="server" CssClass="normalBold">
            <asp:ListItem Value="ASCII">ASCII</asp:ListItem>
            <asp:ListItem Value="Default">Windows Default</asp:ListItem>
            <asp:ListItem Value="UTF8" Selected="True">UTF 8</asp:ListItem>
        </asp:RadioButtonList>
    </div>
    <dnn:SectionHead ID="dshSecurity" CssClass="SubHead" runat="server" Text="Security Options (optional)"
        Section="tblSecurity" ResourceKey="lblSecurityTitle" IncludeRule="False" IsExpanded="false">
    </dnn:SectionHead>
    <table cellspacing="0" cellpadding="2" border="0" summary="Security Options (optional)"
        id="tblSecurity" runat="server">
        <tr>
            <td class="NormalBold">
                <dnn:Label ID="lblDomain" runat="server" ControlName="txtAccount" Suffix=":"></dnn:Label>
            </td>
            <td>
                <asp:TextBox ID="txtAccount" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="NormalBold">
                <dnn:Label ID="lblPassword" runat="server" ControlName="txtPassword" Suffix=":">
                </dnn:Label>
            </td>
            <td>
                <asp:TextBox ID="txtPassword" CssClass="NormalTextBox" runat="server" MaxLength="256"
                    Width="300" TextMode="Password"></asp:TextBox>
            </td>
        </tr>
    </table>
</div>
