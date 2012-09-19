<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MVCNestedModels.Models.ToDoItem>" %>
<%@ Import Namespace=" MVCControlsToolkit.Core" %>
<%@ Import Namespace=" MVCControlsToolkit.Controls" %>
<%@ Import Namespace=" MVCNestedModels.Models" %>
<body>
        <% string[] ImportanceUrls = { "", Url.Content("~/Content/ExclamationMark.png") };
           string[] ImportanceAlts = { "", "important" }; 
   
                 %>
        <%:Html.IsValid() %>
        <% if (Model != null)
           {%>
        <%: Html.ValidationSummary(true)%>
        
        <fieldset>
            <legend>Edit ToDo</legend>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Code)%>
            </div>
            <div class="editor-field">
                <%:Html.FormattedDisplay(model => model.Code)%>
                <%: Html.HiddenFor(model => model.Code)%>

            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Name)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Name)%>
                <%: Html.ValidationMessageFor(model => model.Name)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Description)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Description)%>
                <%: Html.ValidationMessageFor(model => model.Description)%>
            </div>
           <div class="editor-field">
                <%: Html.CheckBoxFor(model => model.Important)%><%: Html.LabelFor(model => model.Important)%>
           </div>
            <%
              if (Model.Important)
              {
              %>
                <%:Html.DetailFormSyncInfos(m => m.Important, ImportanceAlts[1], false, urlValue: ImportanceUrls[1]) %>
              <%} %>
              <%else
              {%>
                <%:Html.DetailFormSyncInfos(m => m.Important, ImportanceAlts[0], false, urlValue: ImportanceUrls[0]) %>
              <%} %>
             <div>
             <p>dropdown synchronized with the grid</p>  
             </div>
              <div>
              <%:Html.LabelFor(m => m.ToDoRole) %>
             <%:Html.DropDownListFor(m => m.ToDoRole,
                                 ChoiceListHelper.CreateGrouped(
                                 RegisterModel.AllRoles,  
                                            m => m.Code,
                                            m => m.Name, m => m.GroupCode, m => m.GroupName))%>
                                            <%:Html.ValidationMessageFor(m => m.ToDoRole, "*") %>
             </div>   
             <div>
             <p>Listbox synchronized with the grid, select multiple roles</p>  
             </div>
             <div>
             <%:Html.LabelFor(m => m.ToDoRoles) %><br />
                <%:Html.DropDownListFor(m => m.ToDoRoles,
                                 ChoiceListHelper.CreateGrouped(
                                 RegisterModel.AllRoles,  
                                            m => m.Code,
                                            m => m.Name, m => m.GroupCode, m => m.GroupName))%>
                                            
             </div>
            <%: Html.DetailFormSyncInfos(m => m.ToDoRoles, 
                Html.Partial("RoleList").ToString(), false) %>
            <p>
                <input type="submit" value="Save" /> 
            </p>
             
        </fieldset>
    <% }

           else
           { %>

           <div> Record has been deleted </div>

           <% } %>
 
 </body>
