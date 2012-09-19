<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace=" MVCControlsToolkit.Core" %>
<%@ Import Namespace=" MVCControlsToolkit.Controls" %>

<% var options = ViewData["ThemeParams"] as ChoiceListDescription;
   var RoleSelect = DualSelectHelper.DualSelectFor(options.HtmlHelper, options.Expression, options.ChoiceList, new Dictionary<string, object> { {"class", "dualselect"}});
   
    %>

<table>
               
    <tr>
    <td  valign="middle" align="center">
    <div><%:RoleSelect.AvailableItemsFilter(new Dictionary<string, object>
        {
            {"style", "width: 100px;"}
        })%></div>
    <%:RoleSelect.AvailableItems(
        new Dictionary<string, object>
        {
            {"style", "height:150px; width: 100px;"}
        }
        ) %>
    </td>
    <td valign="middle" align="center"> 
    <table>
        <tr><td  valign="middle" align="center">
        <%:RoleSelect.ClearSelectionButton(ThemedControlsStrings.Get("DualSelect_MoveAllLeft", "DualSelect"))%>
        </td>
        </tr>
        <tr><td  valign="middle" align="center">
        <%:RoleSelect.UnSelectButton(ThemedControlsStrings.Get("DualSelect_MoveLeft", "DualSelect"))%>
        </td>
        </tr>
        <tr><td  valign="middle" align="center">
        <%:RoleSelect.SelectButton(ThemedControlsStrings.Get("DualSelect_MoveRight", "DualSelect"))%>
        </td>
        </tr>
        <tr><td  valign="middle" align="center">
        <%:RoleSelect.SelectAllButton(ThemedControlsStrings.Get("DualSelect_MoveAllRight", "DualSelect"))%>
        </td>
        </tr>
    </table>
    </td>
    <td  valign="middle" align="center">
    <div><%:RoleSelect.SelectedItemsFilter(new Dictionary<string, object>
        {
            {"style", "width: 100px;"}
        })%></div>
    <%:RoleSelect.SelectedItems(
        new Dictionary<string, object>
        {
            {"style", "height:150px; width: 100px;"}
        }
        ) %>
    </td>
    <td  valign="middle" align="center">
    <div><%:RoleSelect.GoUpButton(ThemedControlsStrings.Get("DualSelect_MoveUp", "DualSelect"))%></div>
    <div><%:RoleSelect.GoDownButton(ThemedControlsStrings.Get("DualSelect_MoveDown", "DualSelect"))%></div>
    </td>
</tr>
</table>