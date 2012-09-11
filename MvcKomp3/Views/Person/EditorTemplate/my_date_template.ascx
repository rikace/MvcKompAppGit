<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DateTime>" %>

<div>
<table>
<tr>
    <td><%= Html.Label("Day") %></td>
    <td><%= Html.TextBox("Day", Model.Day)%></td>
</tr>
<tr>
    <td><%= Html.Label("Month")%></td>
    <td><%= Html.TextBox("Month", Model.Month) %></td>
</tr>
<tr>
    <td><%= Html.Label("Year")%></td>
    <td><%= Html.TextBox("Year", Model.Year)%></td>
</tr>
</table>
</div>
