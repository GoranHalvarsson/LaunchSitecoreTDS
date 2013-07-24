﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Full Left Sidebar with Title and Content.ascx.cs" Inherits="LaunchSitecore.layouts.LaunchSitecore.Default.Sublayouts.Full_Left_Sidebar_with_Title_and_Content" %>
<%@ Register TagPrefix="sc" Namespace="Sitecore.Web.UI.WebControls" Assembly="Sitecore.Kernel" %>
<div id="content">
 <sc:Placeholder ID="phContentTop" Key="content-top" runat="server" />
 <sc:Placeholder ID="phContent" Key="content" runat="server" />
</div>
<div id="sidebar">
 <sc:Placeholder ID="phSidebar" Key="sidebar" runat="server" />
</div>