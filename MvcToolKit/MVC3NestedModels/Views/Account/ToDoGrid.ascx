<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MVCNestedModels.Models.ToDoItem>" %>
<%@ Import Namespace=" MVCControlsToolkit.Core" %>
<%@ Import Namespace=" MVCControlsToolkit.Controls" %>
<table>
<tr>
<td colspan="4"> TO DO LIST</td>
</tr>
<tr>
<td class="HeaderContainer"><strong><%:Html.SortButtonFor(m => m.Name, sortButtonStyle: SortButtonStyle.Button) %></strong></td>
<td class="HeaderContainer"><strong><%:Html.SortButtonFor(m => m.Description, sortButtonStyle: SortButtonStyle.Button)%></strong></td>
<td >Edit</td>
<td >Delete</td>
</tr>
<%:ViewData["Content"] as MvcHtmlString %>
</table>
