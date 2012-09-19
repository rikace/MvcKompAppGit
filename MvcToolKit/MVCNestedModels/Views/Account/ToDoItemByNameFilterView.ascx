<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MVCNestedModels.Controls.ToDoItemByNameFilter>" %>
<%: Html.TextBoxFor(m => m.Name) %><%: Html.ValidationMessageFor(m =>m.Name) %>

