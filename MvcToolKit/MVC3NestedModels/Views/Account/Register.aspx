<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MVCNestedModels.Models.RegisterModel>" %>
<%@ Import Namespace="MVCNestedModels.Controls" %>
<%@ Import Namespace=" MVCControlsToolkit.Core" %>
<%@ Import Namespace=" MVCControlsToolkit.Controls" %>
<%@ Import Namespace=" MVCNestedModels.Models" %>
<%@ Import Namespace=" System.Linq.Expressions" %>
<%@ Import Namespace=" MVCControlsToolkit.Controls.Bindings" %>

<asp:Content ID="registerTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Register
</asp:Content>

<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">

    
    <h2>Create a New Account</h2>
    
    <p>
        Use the form below to create a new account. 
    </p>
    
    <p>
        Passwords are required to be a minimum of <%: ViewData["PasswordLength"] %> characters in length.
    </p>
   <% Html.EnableClientValidation(); %>
   <%: Html.GetTimer(new TimeSpan(0, 0, 1), "ServerTime", "clock") %><div id="clock"></div>
    <% using (Html.BeginForm()) { %>
        <%: Html.ValidationSummary(false, "Account creation was unsuccessful. Please correct the errors and try again.") %>
        <%:Html.OpenTContext() %>
        <div>
            <fieldset>
                
                <legend>Account Information</legend>
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.UserName) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.UserName)%>
                    <%: Html.ValidationMessageFor(m => m.UserName, "*") %>
                </div>
                
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.Email) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.Email) %>
                    <%: Html.ValidationMessageFor(m => m.Email) %>
                    
                </div>
                
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.Password) %>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.Password) %>
                    <%: Html.ValidationMessageFor(m => m.Password) %>
                </div>
                
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.ConfirmPassword) %>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.ConfirmPassword) %>
                    <%: Html.ValidationMessageFor(m => m.ConfirmPassword) %>
                </div>
                <%var hp = Html.DescendatntCast(m => m.PersonalData).To<PersonalInfosExt>(); %>
                <div class="editor-label">
                    <%: hp.LabelFor(m => m.Name) %>
                </div>
                <div class="editor-field">
                    <%: hp.TextBoxFor(m => m.Name) %>
                    <%: hp.ValidationMessageFor(m => m.Name) %>
                </div>

                <div class="editor-label">
                    <%: hp.LabelFor(m => m.SurName) %>
                </div>
                <div class="editor-field">
                    <%: hp.TextBoxFor(m => m.SurName) %>
                    <%: hp.ValidationMessageFor(m => m.SurName) %>
                </div>
                <div class="editor-label">
                <strong>
                With the help of the DescendantCast helper we are able to work with a subclass of the original class PersonalInfos defined in the ViewModel that has
                one more field: Adress. 
                </strong>
                </div>
                <div class="editor-label">
                    <%: hp.LabelFor(m => m.Adress) %>
                </div>
                <div class="editor-field">
                    <%: hp.TextBoxFor(m => m.Adress)%>
                    <%: hp.ValidationMessageFor(m => m.Adress)%>
                </div>
                <div class="editor-label">
                    <%: hp.CheckBoxFor(m => m.Female) %>
                    <%: hp.LabelFor(m => m.Female)%>
                </div>
                <div class="editor-label">
                <strong>
                See below both the use of TypedTextBox and DynamicRangeAttribute. 
                The age has been defined as a float to show number formatting.
                </strong>
                </div>
                <div class="editor-label">
                    <%: hp.LabelFor(m => m.MinAge) %>
                    <%: hp.TextBoxFor(m => m.MinAge) %>
                    <%: hp.ValidationMessageFor(m => m.MinAge) %>
                </div>
                <div class="editor-label">
                    <%: hp.LabelFor(m => m.Age) %>
                    <%: hp.TypedTextBoxFor(m => m.Age, contentAlign: ContentAlign.Right, watermarkCss:"watermark" ) %>
                    <%: hp.ValidationMessageFor(m => m.Age) %>
                </div> 
                <div class="editor-label">
                    <%: hp.LabelFor(m => m.MaxAge) %>
                    <%: hp.TextBoxFor(m => m.MaxAge) %>
                    <%: hp.ValidationMessageFor(m => m.MaxAge) %>
                </div> 
                <div>
                <p></p>
                <div><strong>Now TypedTextBox and TypedEditDisplay supports also Date and Time Fields. Try it!</strong></div>
                <div>
                <%:hp.TypedTextBoxFor(m => m.BirthDate, "watermark", calendarOptions: new CalendarOptions { GoToCurrent=true})%>
                <%: hp.ValidationMessageFor(m => m.BirthDate) %>
                </div>
                <p></p>
                <div><strong>Now you can enable the client side check of the date/time format on all DateTime fields. Try a date/time/date+time/ in the same format as the one shown by windows in your computer</strong></div>
                <div>
                <%:hp.TextBoxFor(m => m.BirthDate1)%>
                <%: hp.ValidationMessageFor(m => m.BirthDate1) %>
                </div>
                <p></p>
                <strong>
                See below three alternatives ways to build the list of selected roles for a user:
                Dual Selct Box,  CheckBox List, and Grouped Multiselect. 
                After a normal single-select to select a single role, with prompt, and required field validation
                </strong>
                </div>
                <div class="editor-label">
                   
                    <%: Html.LabelFor(m => m.Roles)%>
                </div>
                <div>
                
                  <%: Html.ThemedChoiceListFor(m => m.Roles,
                                ChoiceListHelper.Create(
                                RegisterModel.AllRoles,
                                (t => t.Code),
                                (t => t.Name),
                                m => new
                                {
                                    style = m.Code % 2 == 0 ?
                                        "color:Blue; background-color:White" :
                                        "color:Red; background-color:White"
                                }),
                                "Dualselect") %>
                  
                </div>
                <script type="text/javascript">
                    $('.dualselect').bind('itemChange', function (e, data) { alert(data.ChangeType); });
                </script>
               <div>
               <div>
               <%: Html.LabelFor(m => m.Roles1) %>
               <table>
                <%: Html.CheckBoxListFor(
                    m => m.Roles1,
                    ChoiceListHelper.Create(RegisterModel.AllRoles, 
                        m => m.Code, 
                        m => m.Name,
                        m => new
                        {
                            style = m.Code % 2 == 0 ?
                                "color:Blue; background-color:White" :
                                "color:Red; background-color:White"
                        }),
                    true,
                    itemTemplate: _S.L<CheckBoxListItem<int>>(
                        (x) => string.Format(
                            "<tr><td>{0}</td><td {2}>{1}</td></tr>",
                            x.CheckBoxFor(m => m.Selected).ToString(),
                            x.DisplayFor(m => m.Label).ToString(),
                            BasicHtmlHelper.GetAttributesString(x.ViewData.Model.LabelAttributes))
                    ))
                    %>
               </table>
               </div>
               <div>
                <%: Html.DropDownListFor(m => m.Roles2,
                    new {style="height:100px"},
                    ChoiceListHelper.CreateGrouped(RegisterModel.AllRoles,
                                m => m.Code,
                                m => m.Name,
                                m => m.GroupCode,
                                m => m.GroupName,
                                m => new {style="color:White; background-color:Black"},
                                m =>  new { style = m.Code%2 == 0 ? 
                                    "color:Blue; background-color:White": 
                                    "color:Red; background-color:White"} ))
                 %>
               </div>
               <p></p>
               <div><strong>Now TypedEditDisplay supports also DropDowns. Choose a Role and click the finish editing button.
               The DropDown will become normal text. Click on the edit button to change it: a Dropdown will appear again.</strong></div>
               <div class='editDisplayContainer'>
                <%: Html.TypedEditDisplayFor(m => m.SingleRole,
                    ChoiceListHelper.CreateGrouped(RegisterModel.AllRoles,
                                m => m.Code,
                                m => m.Name,
                                m => m.GroupCode,
                                m => m.GroupName,
                                m => new {style="color:White; background-color:Black"},
                                m =>  {
                                    if (m == null)
                                        return new 
                                        {
                                            @class = "watermark"
                                        };
                                    else
                                    return new
                                    {
                                        style = m.Code % 2 == 0 ?
                                            "color:Blue; background-color:White" :
                                            "color:Red; background-color:White"
                                    };
                                }), editEnabled: false)
                 %>
                 <%: Html.ValidationMessageFor(m => m.SingleRole, "*") %>
               <%:Html.EditDisplayToggle(".editDisplayContainer", "finish editing", "edit value", "EditDisplayButton") %>        
               </div>

               

               <p></p>
               
               <div><strong>Click on the populate button to populate the dropdown via Ajax</strong></div>
                   <select id="dynamicSelect"> </select><input id="btnDynamicSelect" type="button" value="Populate dropdown" />
                   <script type="text/javascript">
                       $('#btnDynamicSelect').click(function () {
                           MvcControlsToolkit_UpdateDropDownOptions('<%:Url.Action("AvailableRoles", "Account") %>', $('#dynamicSelect'), 'Choose Role', function (x) { return x == '' ? 'watermark' : 'normal' }, "inverted", function (y) { alert("populated dropdown: "+y.attr("id"))});
                       });
                </script>
               <div></div>
               <p></p>
               <div><strong> Use of an implemntation of the IUpdateModel interface to modify the way data are represented:
               Absolute values are transformed into Total+Percentages, and the inverse transformation is applied when the View is posted
               </strong></div>
                <% var transformHelper =
                  hp.RenderWith(
                    hp.InvokeUpdateTransform(m => m,
                        new PercentageUpdater(),
                        new string[]
                        {
                            "ForexInvestment",
                            "FuturesInvestment",
                            "SharesInvestment"
                        }));
                 %>
                 <div>Investments</div>
                <div>
                    <table>
                        <tr>
                            <td>Total Amount to Invest</td>
                            <td><%: transformHelper.TextBoxFor(m => m.Total)%></td>
                            <td><%: transformHelper.ValidationMessageFor(m => m.Total, "*")%></td>
                        </tr>
       
                    <% for (int index = 0; index < transformHelper.ViewData.Model.Percentages.Length; index++)
                        {  
                            %>
                        <tr>
                            <td><%:MvcHtmlString.Create(transformHelper.ViewData.Model.Labels[index])%></td>
                            <td><%:transformHelper.RangeFor(m => m.Percentages[index], new { style = "text-align:right", min = 0d, max = 100d })%></td>
                            <td></td>
                        </tr>
                    <%} %>  
           
                    </table>

                </div>
               </div>
                <div class="editor-label">
                   <strong>
                    Reorder elements by dragging them with the mouse
                    </strong>
                </div>
                <div class="editor-label">
                 
                 <% RenderInfo<IEnumerable<string>> convertedKeywords =
                        Html.ConvertToIEnumerable<RegisterModel, string[], string>(m => m.Keywords); %>
                 <%
                     Func<HtmlHelper<string>, string> keywordsTemplate = 
                         (x) => x.TypedTextBoxFor(m => m, watermarkCss:"watermark", overrideWatermark: "insert keyword").ToString()+
                                x.SortableListDeleteButton("Delete",  ManipulationButtonStyle.Link).ToString();
                      %>
                 <%:  
                 
                 Html.SortableListFor(convertedKeywords,
                    _S.L<string>(
                                   (x) => x.TypedTextBoxFor(m => m, watermarkCss: "watermark", overrideWatermark: "insert keyword").ToString() +
                                x.SortableListDeleteButton("Delete",  ManipulationButtonStyle.Link).ToString()
                                ),
                               "KeywordInsert", 0.8f, 
                    htmlAttributesContainer: new Dictionary<string, object> { { "class", "SortableList" } },
                               itemCss: "normalRow", altItemCss: "alternateRow")
                  %>
                  
                  <%: Html.SortableListAddButtonFor(convertedKeywords, "Add New Item")%>
                </div>
                
                <div class="editor-label">
                   <strong>
                    The Same Keywords list as before but with sorting disabled, with customs Items Container,
                    Custom Item type, header and footer, and with a TypedEditDisplay to edit the names. Click on the names to edit them.
                    </strong>
                </div>
                <div class="editor-label">
                <table>
                    <thead>
                    <tr>
                        <td ><strong><%:Html.SortButtonForCollection(m => m.Keywords1, m => m.Keyword, null, SortButtonStyle.Button) %></strong></td><td ><strong><%:Html.SortButtonForCollection(m => m.Keywords1, m => m.Title, null, SortButtonStyle.Button)%></strong></td><td ></td> 
                    </tr>
                    </thead>
                <%:
                    Html.SortableListFor(m => m.Keywords1, 
                    _S.L<KeywordItem>(
                    (x) =>"<td style='width: 120px'>" + x.TypedEditDisplayFor(m => m.Keyword, new {style= "width: 100px"}, new {style= "width: 100px"}, simpleClick: true).ToString()+
                                                        x.ValidationMessageFor(m => m.Keyword, "*")+
                          "<td style='width: 120px'>" + x.TypedEditDisplayFor(m => m.Title, new { style = "width: 100px" }, new { style = "width: 100px" }, simpleClick: true).ToString() +
                                                                    x.ValidationMessageFor(m => m.Title, "*") +      
                        "</td><td>"+x.SortableListDeleteButton("Delete",  ManipulationButtonStyle.Link).ToString()
                                + "</td>"
                    ),
                    canSort: false,
                    allItemsContainer: ExternalContainerType.tbody,
                    itemContainer: ExternalContainerType.tr,
                    itemCss: "normalRow", altItemCss: "alternateRow",
                    
                    footerTemplate: _S.L<KeywordItem>((x) => 
                        "<td colspan='3'><strong>Keywords Item Footer</strong></td>"))
                %>
                </table>
                <%: Html.SortableListAddButtonFor(m => m.Keywords1, "Add New Item") %>
                <%: Html.EnableSortingNoTrackFor(m => m.Keywords1, m => m.KeywordsOrdering, "NormalHeaderToDo", "AscendingHeaderToDo", "DescendingHeaderToDo") %>
                
                </div>
                <div style="margin: 10px 10px 10px 10px">
                   <strong> 
                    RangeAction.Propagate: A minimum distance of four hours is forced. <br />
                    Changing one date the other is changed accordingly to enforce the minimum distance
                   </strong>
                </div>

                <div>
                <%var DTS = Html.DateTimeFor(m => m.Start, DateTime.Now);  %>
                <%: DTS.Date() %>&nbsp;&nbsp;<%: DTS.Time() %><input id="btnDate" type="button" value="Now" />
                </div>
                <script type="text/javascript">
                    $('#btnDate').click(function () {
                        MvcControlsToolkit_DateTimeInput_SetById('<%: Html.PrefixedId(m => m.Start) %>', new Date(), null, null);
                    });
                </script>
                
                <div><br /><br /></div>
                <div>
                <%var DT = Html.DateTime("Stop", Model.Stop, dateInCalendar : true);  %>
                <%: DT.DateCalendar(
                    inLine: false,
                    calendarOptions: new CalendarOptions
                    {
                         ChangeYear=true,
                         ChangeMonth=true,
                    })
                    %>&nbsp;&nbsp;<%: DT.Time() %>
                </div>
                <div style="margin: 10px 10px 10px 10px">   
                    Mutual exclusive checkboxes
                </div>
                 <div>
                    <input id="Checkbox1" type="checkbox" class="display-field, couple" checked="checked" />
                    <input id="Checkbox2" type="checkbox" class="display-field, couple" />
                    <input id="Checkbox3" type="checkbox" class="display-field, couple" />
                    <input id="Checkbox4" type="checkbox" class="display-field, couple" />
                    <%:Html.MutualExclusiveChoice("couple") %>
                </div>
                <div id='query'>
                <div id="attempt">
                <% Html.DeclareStringArray(new string[] { "", Url.Content("~/Content/ExclamationMark.png") }, "Importance");
                   Html.DeclareStringArray(new string[] { "", "important" }, "ImportanceAlt"); 
   
                 %>
                <table>
                    <thead>
                        <tr>
                        <td colspan="5"> TO DO LIST</td>
                        </tr>
                        <tr>
                        <td class="HeaderContainer"><strong><%:Html.SortButtonForTrackedCollection(m => m.ToDoList, m => m.Name, sortButtonStyle: SortButtonStyle.Button) %></strong></td>
                        <td class="HeaderContainer"><strong><%:Html.SortButtonForTrackedCollection(m => m.ToDoList, m => m.Description, sortButtonStyle: SortButtonStyle.Button)%></strong></td>
                        <td class="HeaderContainer"><strong><%:Html.ColumnNameForTrackedCollection(m => m.ToDoList, m => m.Important) %></strong></td>
                        <td class="HeaderContainer"><strong><%:Html.ColumnNameForTrackedCollection(m => m.ToDoList, m => m.ToDoRole) %></strong></td>
                        <td class="HeaderContainer"><strong><%:Html.ColumnNameForTrackedCollection(m => m.ToDoList, m => m.ToDoRoles) %></strong></td>
                        <td class="HeaderContainer"><strong></strong></td>
                        <td class="HeaderContainer"><strong></strong></td>
                        </tr>
                    </thead>
                    <tbody class='gridRoot'>
                    <%: Html.DataGridFor(m => m.ToDoList, ItemContainerType.tr,
                                "ToDoEditItem", "ToDoDisplayItem", null, "ToDoInsertItem", "ToDoUndeleteItem",
                     itemCss: "normalRow", altItemCss: "alternateRow",
                     toTrack: new FieldsToTrack<ToDoItem>().
                                   Add(f => f.Code).Add(f => f.Name).Add(f => f.Description).Add(m => m.Important)
                                   .Add(f => f.ToDoRoles).Add(f => f.ToDoRole))
                                   %>
                    </tbody>
                </table>
                </div>
                
                <script type="text/javascript">
                    $('.gridRoot').bind('itemChange', function (e, data) { alert(data.ChangeType); });
                    
                </script>
                
                <div class="HideAtStart paging">
                    <% var pager = Html.PagerFor(m => m.Page, m => m.PrevPage, m => m.TotalPages); %>
                    
                    <%:pager.PageButton("<<", PageButtonType.First, PageButtonStyle.Link) %>
                    <%:pager.PageButton("<", PageButtonType.Previous, PageButtonStyle.Link) %>
                    <%:pager.PageChoice(4, pageNames: p => string.Format("({0})", p)) %>
                    <%:pager.PageButton(">", PageButtonType.Next, PageButtonStyle.Link) %>
                    <%:pager.PageButton(">>", PageButtonType.Last, PageButtonStyle.Link) %>
                    
                    <%:pager.PageButton("Go To", PageButtonType.GoTo, PageButtonStyle.Button) %><%:pager.GoToText(new {style="width:50px;" })%>
                </div>
                
                <%: Html.EnableSortingFor(m => m.ToDoList, m => m.ToDoOrder, "NormalHeaderToDo", "AscendingHeaderToDo", "DescendingHeaderToDo", causePostback: false, page: m => m.Page, oneColumnSorting: false) %>

               
                <p>
                    <input type="submit" id="mybutton" value="Register" />
                </p>
                </div>
               
                <div class="editor-label">
                <strong>
                Select different content either by pressing Selection Buttons or by using the checkboxes.
                See how the content shown changes, and how all selection tools are synchronized
                </strong>
                </div>
                <div>
                <%:Html.ViewListFor(m => m.Selection, Html.PrefixedId("choiceTest"), "isChangedToDo")%>
                <%:Html.SelectionButton("choice 1", "choice1", Html.PrefixedId("choiceTest"), Html.PrefixedId("choice1_button"), ManipulationButtonStyle.Link) %>
                <%:Html.SelectionButton("choice 2", "choice2", Html.PrefixedId("choiceTest"), Html.PrefixedId("choice2_button"), ManipulationButtonStyle.Link) %>
                <%:Html.SelectionButton("choice 3", "choice3", Html.PrefixedId("choiceTest"), Html.PrefixedId("choice3_button"), ManipulationButtonStyle.Link) %>
                    <input id="Checkbox6" type="checkbox" class='<%:Html.PrefixedId("choiceTest_checkbox")+" "+Html.PrefixedId("choice1_checkbox") %>' />
                    <input id="Checkbox7" type="checkbox"  class='<%:Html.PrefixedId("choiceTest_checkbox")+" "+Html.PrefixedId("choice2_checkbox") %>'/>
                    <input id="Checkbox8" type="checkbox"  class='<%:Html.PrefixedId("choiceTest_checkbox")+" "+Html.PrefixedId("choice3_checkbox") %>'/>
                </div>
                <div id="fatherChoices">
                A
                <div id='<%:Html.PrefixedId("choice1")%>'  class="choiceTest"><%: Html.DataFilter(m=>m.ItemFilter, new ToDoItemByNameFilter(), "ToDoItemByNameFilterView") %></div>
                B
                <div id='<%:Html.PrefixedId("choice2")%>'  class="choiceTest">choice 2</div>
                C
                <div id='<%:Html.PrefixedId("choice3")%>'  class="choiceTest">choice 3</div>
                D
                </div>
            <div>
            <strong>TreeView: when in edit mode, move the nodes dragging them with the mouse after having selected the new father node.
            click on the label of a node to edit the label</strong>
            </div>
            <div></div>
            <div>
   
            <%:
                Html.TreeViewFor(
                    m => m.EmailFolders,
                    i => i == 0 ? "Children" : null,
                    ExternalContainerType.span,
                               "filetree treeview-red treeToedit",
                    new object[] 
                    {
                        _S.L<EmailFolder>(h => 
                            "<span class='folder'>" +h.DisplayFor(m => m.Name).ToString()+"</span>"),
                            
                        _S.L<EmailDocument>(h => 
                            "<span class='file'>" +h.DisplayFor(m => m.Name).ToString()+"</span>") 
                    },
                    (x, y) => x is EmailFolder ? 0 : 1,
                    "filetree treeview-red treeToedit",
                    new object[] 
                    {
                        _S.L<EmailFolder>(h => string.Format(
                            "<div class='folder' >{0} {1} {2} {3} {4}</div>", 
                            h.TypedEditDisplayFor(m => m.Name, simpleClick: true).ToString(),
                            h.TreeViewDeleteButton(Url.Content("~/Content/folder_delete_16.png"), 
                                ManipulationButtonStyle.Image).ToString(), 
                            h.TreeViewAddButton(1, Url.Content("~/Content/document_add_16.png"), 
                                ManipulationButtonStyle.Image).ToString(),
                            h.TreeViewAddButton(0, Url.Content("~/Content/folder_add_16.png"), 
                                ManipulationButtonStyle.Image).ToString(),
                            h.Hidden("IsFolder", true).ToString())),
                            
                        _S.L<EmailDocument>(h => string.Format(
                            "<div class='file' >{0} {1} {2}</div>",
                            h.TypedEditDisplayFor(m => m.Name, simpleClick: true, editEnabled: true).ToString(),
                            h.TreeViewDeleteButton(Url.Content("~/Content/document_delete_16.png"), 
                                ManipulationButtonStyle.Image).ToString(),
                            h.Hidden("IsFolder", false).ToString())) 
                    },
                    (x, y) => x is EmailFolder ? 0 : 1,
                    TreeViewMode.InitializeDisplay,
                    (x) => "allnodes",
                    (x, y) => TreeViewItemStatus.initializeShow)
                    
       
            %>
            </div><input id="Button10" type="button" value="button" />
            <script type="text/javascript">
                $('.treeToedit').bind('itemChange', function (e, data) { alert(data.ChangeType); });
                
                </script>
             
            <div><%:Html.TreeViewToggleEditButtonFor(
                 m => m.EmailFolders,
                 "Edit Folders", "TreeViewEdit",
                 "Undo Changes", "TreeViewUndo",
                 "Redo Changes", "TreeViewRedo")
                 %></div>
            
            </fieldset>
            <div>
            <strong>Client ViewModel & Client-side Bindings</strong>
            </div>
            <div>Below a Client Side ViewModel is defined and used to collect keywords and selected keywords with a ListBox. <br />
            Bindings are used to define the interactions between DOM elements and the ViewModel, and between Dom elements.
            </div>
            <div><br /></div>
            <div id="ClientViewModelContainer">
            <%
           var clientKeywordsHelper =
                  Html.TransformedHelper(
                      m => m.ClientKeywords,
                      new ClientViewModel());
            %>
           <%: clientKeywordsHelper.TemplateFor(m => m, "ClientViewModelExample", null, true, "keywordsHandling", "ClientViewModelContainer", true) %>
           
            </div>
        </div>
        <%: Html.CloseTContext() %>
    <% } %>

    <% Html.DetailFormFor(Ajax, m => m.ToDoList, ExternalContainerType.div,
           "EditDetailToDo", "Account", null, "isChangedToDo", "isDeletedToDo", detailDialog:
           new MVCControlsToolkit.Controls.DataGrid.Dialog
           {
               Title="prova dialogo",
               Show="slide",
               Hide="slide"

           });%>

           <%: Html.AntiFlicker("HideAtStart")%>
</asp:Content>
