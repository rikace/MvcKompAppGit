<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MVCNestedModels.Models.ToDoItem>" %>
<%@ Import Namespace=" MVCControlsToolkit.Core" %>
<%@ Import Namespace=" MVCControlsToolkit.Controls" %>
<%@ Import Namespace="MVCNestedModels.Models" %>
            <td class="editor-field">
                <%: Html.DisplayField(model => model.Name)%>
            </td>
            <td class="editor-field">
                <%: Html.DisplayField(model => model.Description)%>
            </td>
            <td>
            <%: Html._D(m => m.Important, valuesArrayName: "ImportanceAlt", urlsArrayName:  "Importance") %>
            </td>
            <td>
            <%: Html.DisplayField(m => m.ToDoRole,
                formattedValue: ChoiceListHelper.DisplayValue(
                    RegisterModel.AllRoles,
                    m => m.Code, m => m.Name,
                    Model.ToDoRole,
                    "No role selected")
            ) %>
            </td>
            <td id='<%:Html.PrefixedId(m => m.ToDoRoles)%>'>
            <%: Html.Partial("RoleList") %>
            </td>
            <td class="editor-label">
                <%: Html.ImgDataButton(DataButtonType.Edit, "../../Content/small_pencil.gif", null)%>
            </td>
            <td class="editor-label">
                <%:Html.DetailLink(Ajax, "Edit Details", DetailType.Display, "EditDetailToDo", "Account",
                    new { 
                        id = Model.Code,
                        ViewHint = "DisplayDetailToDo",
                                    }, null)%>
                <%: Html.LinkDataButton(DataButtonType.Delete, "Delete Item", null) %>
            </td>