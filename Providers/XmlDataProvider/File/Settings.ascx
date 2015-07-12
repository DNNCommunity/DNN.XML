<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="DotNetNuke.Modules.Xml.Providers.XmlDataProvider.File.Settings" %>
<%@ Register Src="~/controls/urlcontrol.ascx" TagName="urlcontrol" TagPrefix="dnn" %>
<dnn:urlcontrol ID="FileControl" runat="server" Required="False" ShowTrack="False" ShowNewWindow="False"
    ShowLog="False" ShowFiles="true" ShowUrls="false" UrlType="F" ShowTabs="False" FileFilter ="xml"
    Width="300" />
