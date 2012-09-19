<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MVCNestedModels.Models.ToDoItem>" %>
<%@ Import Namespace=" MVCControlsToolkit.Core" %>
<%@ Import Namespace=" MVCControlsToolkit.Controls" %>

<td colspan="4"><%: Html.DataButton(DataButtonType.Insert, "Insert New To Do Item", null)%></td>
<td colspan="3"><%:Html.DetailLink(Ajax, "Insert Details", DetailType.Edit, "EditDetailToDo", "Account",
                    new { 
                        id = 0
                                    }, null)%></td>