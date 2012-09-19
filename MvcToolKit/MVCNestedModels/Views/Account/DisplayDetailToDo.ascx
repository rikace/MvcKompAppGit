<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MVCNestedModels.Models.ToDoItem>" %>
<%@ Import Namespace=" MVCControlsToolkit.Core" %>
<%@ Import Namespace=" MVCControlsToolkit.Controls" %>

    <fieldset>
        <legend>ToDo Details</legend>
        
        <div class="display-label">Code</div>
        <div class="display-field"><%:Html.DisplayField(model => model.Code)%></div>
        
        <div class="display-label">Name</div>
        <div class="display-field"><%: Html.DisplayField(model => model.Name)%></div>
        
        <div class="display-label">Description</div>
        <div class="display-field"><%: Html.DisplayField(model => model.Description)%></div>
        
    </fieldset>
    


