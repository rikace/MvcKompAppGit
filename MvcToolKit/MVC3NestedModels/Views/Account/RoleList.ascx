<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ToDoItem>" %>
<%@ Import Namespace=" MVCControlsToolkit.Core" %>
<%@ Import Namespace=" MVCControlsToolkit.Controls" %>
<%@ Import Namespace=" MVCNestedModels.Models" %>

<% if(Model != null){ %>
<ul>
<%foreach (string s in ChoiceListHelper.DisplayValues(
      RegisterModel.AllRoles, m => m.Code, m => m.Name, Model.ToDoRoles))
  {    
       %>
       <li><%: s%></li>
       <% } %>
</ul>
<%} %>