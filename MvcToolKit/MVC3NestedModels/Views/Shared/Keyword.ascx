<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<%@ Import Namespace=" MVCControlsToolkit.Core" %>
<%@ Import Namespace=" MVCControlsToolkit.Controls" %>
<div id='<%: Html.PrefixedId("InnerContainer") %>'>
<%: Html.TypedTextBoxFor(m => m, watermarkCss:"watermark", overrideWatermark: "insert keyword") %><%: Html.SortableListDeleteButton("Delete",  ManipulationButtonStyle.Link) %>
</div>