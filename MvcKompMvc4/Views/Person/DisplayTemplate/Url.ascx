<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<a href="<%: Model %>">
   <%: ViewData.TemplateInfo.FormattedModelValue %>
</a> 
