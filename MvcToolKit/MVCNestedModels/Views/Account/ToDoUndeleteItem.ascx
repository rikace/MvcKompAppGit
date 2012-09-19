<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MVCNestedModels.Models.ToDoItem>" %>
<%@ Import Namespace=" MVCControlsToolkit.Core" %>
<%@ Import Namespace=" MVCControlsToolkit.Controls" %>

            <td class="editor-field" colspan="3">
               Deleted Item: <%:Model.Name %> 
            </td>
            
            <td class="editor-label" >
                
                <%: Html.DataButton(DataButtonType.Undelete, "Undelete Item", null) %>
            </td>
