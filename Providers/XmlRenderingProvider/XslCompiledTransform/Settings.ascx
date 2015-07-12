<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="DotNetNuke.Modules.Xml.Providers.XmlRenderingProvider.XslCompiledTransform.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="ParameterEditor" Src="~/DesktopModules/xml/Parameters/ParameterEditor.ascx" %>
<div>
    <asp:Label ID="lblXslDefinition" resourcekey="lblXslDefinition" runat="server" CssClass="normal">In this section, you can define the source of your XSl Stylesheet. You can define additional arguments to provide your script with context information.</asp:Label>
    <dnn:url id="UrlXsl" runat="server" required="True" showtrack="False" shownewwindow="False"
        showlog="False" urltype="F" showfiles="True" showurls="True" showtabs="False"
        width="300">
    </dnn:url>
</div>
<div id="divXslSettings" runat="server" style="margin-top: -20px; margin-bottom: 10px" class="dnnClear">
    <dnn:sectionhead id="dshXslParams" runat="server" isexpanded="true" resourcekey="dshXslParams"
        section="sectXslParams" cssclass="SubHead">
    </dnn:sectionhead>
    <div id="sectXslParams" runat="server">
        <dnn:parametereditor id="PedXsl" runat="server" purpose="XSL"></dnn:parametereditor>
    </div>
</div>
