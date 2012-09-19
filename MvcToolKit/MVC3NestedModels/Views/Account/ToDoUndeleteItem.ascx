<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MVCNestedModels.Models.ToDoItem>" %>
<%@ Import Namespace=" MVCControlsToolkit.Core" %>
<%@ Import Namespace=" MVCControlsToolkit.Controls" %>

            <td class="editor-field" colspan="5">
               Deleted Item: <%:Html.DisplayField(m => m.Name) %> 
            </td>
            
            <td class="editor-label" colspan="2">
                
                <%: Html.DataButton(DataButtonType.Undelete, "Undelete Item", null) %>
            </td>
