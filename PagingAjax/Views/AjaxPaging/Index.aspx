<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IPagedList<Product>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Index</h2>

    <div id="divGrid">
    <% Html.RenderPartial("ListControl", this.Model);  %>
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
</asp:Content>
