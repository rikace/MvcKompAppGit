<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<%@ Import Namespace=" MVCControlsToolkit.Core" %>
<%@ Import Namespace=" MVCControlsToolkit.Controls" %>
<div id='<%: Html.PrefixedId("InnerContainer") %>'>
<%: Html.TextBoxFor(m => m) %><%: Html.SortableListDeleteButton("Delete",  ManipulationButtonStyle.Link) %>
</div>