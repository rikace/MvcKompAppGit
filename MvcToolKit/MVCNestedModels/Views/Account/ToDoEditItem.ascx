<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MVCNestedModels.Models.ToDoItem>" %>
<%@ Import Namespace=" MVCControlsToolkit.Core" %>
<%@ Import Namespace=" MVCNestedModels.Models" %>
<%@ Import Namespace=" MVCControlsToolkit.Controls" %>
            
            <td class="editor-field">
                <%: Html.ValidationMessageFor(m => m.Name, "*")%><%: Html.TextBoxFor(m => m.Name) %>
            </td>
            <td class="editor-field">
                <%: Html.ValidationMessageFor(m => m.Description, "*")%><%: Html.TextBoxFor(m => m.Description)%>
            </td>
            <td>
             <%: Html.CheckBoxFor(m => m.Important) %>
            </td>
             <td>
              
             </td>
              <td>
            <%:Html.DropDownListFor(m => m.ToDoRole,
                                 ChoiceListHelper.CreateGrouped(
                                 RegisterModel.AllRoles,  
                                            m => m.Code,
                                            m => m.Name, m => m.GroupCode, m => m.GroupName))%>
                                            <%:Html.ValidationMessageFor(m => m.ToDoRole, "*") %>
             <%:Html.ThemedChoiceListFor(m => m.ToDoRoles,
                                 ChoiceListHelper.Create(
                                 RegisterModel.AllRoles,  
                                            m => m.Code,
                                            m => m.Name), "Dualselect")%>
            </td>
            <td class="editor-label" >
                <%: Html.DataButton(DataButtonType.ResetRow, "Reset Row", null) %>
            </td>
            <td class="editor-label" >
                <%: Html.HiddenFor(m => m.Code) %>
                <%: Html.DataButton(DataButtonType.Cancel, "Cancel Changes", null) %>
            </td>
            
            





