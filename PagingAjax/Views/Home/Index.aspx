<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="MvcPaging.Demo.Controllers"%>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%= Html.Encode(ViewData["Message"]) %></h2>
    <dl>
		<dt><%= Html.ActionLink("Simple paging", "Index", "Paging") %></dt>
		<dd>An example of paging a list of products.</dd>
    </dl>
    <dl>
		<dt><%= Html.ActionLink("Paging with filtering", "ViewByCategory", "Paging") %></dt>
		<dd>A list of products that are filtered on the category</dd>
    </dl>
    
    <br /><br />
    
    <dl>
		<dt><%= Html.ActionLink("Ajax simple paging", "Index", "AjaxPaging") %></dt>
		<dd>An example of paging a list of products.</dd>
    </dl>
    <dl>
		<dt><%= Html.ActionLink("Ajax paging with filtering", "ViewByCategory", new { controller = "AjaxPaging", categoryName = "" })%></dt>
		<dd>A list of products that are filtered on the category</dd>
    </dl>
</asp:Content>
