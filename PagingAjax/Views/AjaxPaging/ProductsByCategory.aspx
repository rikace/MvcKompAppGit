<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MvcAjaxPaging.IPagedList<Product>>" %>
<%@ Import Namespace="MvcPaging.Demo.Models"%>
<%@ Import Namespace="MvcAjaxPaging"%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<% using (Html.BeginForm("ViewByCategory", "AjaxPaging", FormMethod.Get)) { %>
	<p>
		Select a category:
		<%= Html.DropDownList("categoryName") %>
		<input type="submit" value="Browse" />
	</p>
	<% } %>
	<% if (ViewData.Model.Count > 0) { %>
	<p>
		Found <%= ViewData.Model.TotalItemCount %> items.
	</p>
	<div id="divGrid">
    <% Html.RenderPartial("ListControlByCategory", this.Model);  %>
    </div>
	
	<script type="text/javascript">

        function beginPaging(args) {
            // Animate
            $('#divGrid').fadeOut('normal');
        }

        function successPaging() {
            // Animate
            $('#divGrid').fadeIn('normal');
        }

        function failurePaging() {
            alert("Could not retrieve contacts.");
        }

    </script>
	<% } %>
</asp:Content>
