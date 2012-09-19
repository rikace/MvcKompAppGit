<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MVCNestedModels.Controls.PercentageUpdater>" %>
<%@ Import Namespace=" MVCControlsToolkit.Core" %>
<%@ Import Namespace=" MVCControlsToolkit.Controls" %>
   
   <div>Investments</div>
   <div>
       <table>
            <tr>
                <td>Total Amount to Invest</td>
                <td><%: Html.TextBoxFor(m => m.Total)%></td>
                <td><%:Html.ValidationMessageFor(m => m.Total, "*") %></td>
           </tr>
       
       <% for(int index=0; index<Model.Percentages.Length; index++)
          {  
               %>
           <tr>
                <td><%:MvcHtmlString.Create(Model.Labels[index])%></td>
                <td><%:Html.RangeFor(m => m.Percentages[index], new { style = "text-align:right", min=0d, max=100d })%></td>
                <td></td>
           </tr>
        <%} %>  
           
      </table>

  </div>

