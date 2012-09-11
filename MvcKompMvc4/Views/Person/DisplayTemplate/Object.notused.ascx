<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% if (Model == null) { %>    
<%= ViewData.ModelMetadata.NullDisplayText %>
<% } else if (ViewData.TemplateInfo.TemplateDepth > 1) { %>    
<%= ViewData.ModelMetadata.SimpleDisplayText %>
<% }  %>    
<table cellpadding="0" cellspacing="0" border="0">    
<% foreach (var prop in ViewData
                        .ModelMetadata
                        .Properties
                        .Where(pm => pm.ShowForDisplay && !ViewData.TemplateInfo.Visited(pm))) { %>        
<tr>                
<td>                    
<div class="display-label">                        
<%= prop.GetDisplayName() %>                    
</div>                
</td>                
<td>                    
<div class="display-field">                        
<%= Html.Display(prop.PropertyName) %>                    
</div>                
</td>            
</tr>       
 
<% } %>    
</table>
<% } %> 