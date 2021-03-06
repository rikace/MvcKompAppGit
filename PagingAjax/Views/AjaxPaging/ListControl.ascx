<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MvcAjaxPaging.IPagedList<Product>>" %>

    <table class="grid">
		<thead>
			<tr>
				<th>Product name</th>
				<th>Category</th>
			</tr>
		</thead>
		<tbody>
			<% foreach (var product in ViewData.Model) { %>
				<tr>
					<td><%= product.Name %></td>
					<td><%= product.Category %></td>
				</tr>
			<% } %>
		</tbody>
	</table>
	
	<div class="pager">
		<%= Ajax.Pager(new AjaxOptions { UpdateTargetId = "divGrid", OnBegin = "beginPaging", OnSuccess = "successPaging", OnFailure = "failurePaging" },
            ViewData.Model.PageSize, ViewData.Model.PageNumber, ViewData.Model.TotalItemCount, new { controller = "AjaxPaging", action = "Index" })%>
	</div>


