<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<%@ Import Namespace=" MVCControlsToolkit.Core" %>
<%@ Import Namespace=" MVCControlsToolkit.Controls" %>
<div >
<%: Html.SortableListAddButton("Insert New", ManipulationButtonStyle.Link)%>
<span id='<%: Html.SortableListNewName() %>' >
<%: Html.TextBoxFor( m => m) %><%: Html.SortableListCancelButton("Cancel", ManipulationButtonStyle.Link) %>
</span>

</div>
