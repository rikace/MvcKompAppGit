<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MVCNestedModels.Models.ClientViewModel>" %>
<%@ Import Namespace="MVCNestedModels.Controls" %>
<%@ Import Namespace=" MVCControlsToolkit.Core" %>
<%@ Import Namespace=" MVCControlsToolkit.Controls" %>
<%@ Import Namespace=" MVCControlsToolkit.Controls.Bindings" %>
<% var bindings = Html.ClientBindings();
            bindings
                .AddMethod("addNewItem",
                @"
                    function () {"+
                        bindings.VerifyFieldsValid (m => m.ItemToAdd) + @"
                        if (this.ItemToAdd() == null || this.ItemToAdd().length == 0) return;
                        this.Keywords.push(this.ItemToAdd());
                        this.SelectedKeywords.push(this.ItemToAdd());
                        this.ItemToAdd('');  
                    }
                    ")
                .AddMethod("removeSelected",
                @"
                    function () {
                        this.Keywords.removeAll(this.SelectedKeywords());
                        this.SelectedKeywords([]); // Reset selection to an empty set
                    }

                    ");
            %>
            <div>
            
            <%: Html.DropDownListFor(m => m.SelectedKeywords, new {style="min-width:100px"}, Html.CreateChoiceList(
                m => m.Keywords,
                m => m,
                m => m, overridePrompt: "prova"))%>
            </div>
            <div>
                <%:
                
               Html.TypedTextBoxFor(
                    m => m.ItemToAdd, 
                    "watermark")
                %>
                <%:
                    Html.ValidationMessageFor(m => m.ItemToAdd, "*")
                %>
                <%
                var addButtonBindings = bindings
                    .Click(m => m, "addNewItem")
                    .Get();
                var removeButtonBinding = bindings
                    .Click(m => m, "removeSelected")
                    .Enable(m => m.SelectedKeywords, "{0}.length > 0").Get();
                var sumbitButtonBindings = bindings.Click(m => m, "saveAndSubmit").Get();
                 %>
                <input type="button" value="Add" data-bind='<%: addButtonBindings %>'/>
                <input type="button" value="Remove Selected" data-bind='<%: removeButtonBinding %>'/>
                <input type="button" value="Submit" data-bind='<%: sumbitButtonBindings %>'/>
            </div>