﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UniversalSearch.ascx.cs" Inherits="RockWeb.Blocks.Cms.UniversalSearch" %>

<style>
    .model-cannavigate {
        cursor: pointer;
    }
</style>

<script>
    Sys.Application.add_load( function () {
        $("div.photo-round").lazyload({
            effect: "fadeIn"
        });
    });
</script>

<asp:UpdatePanel ID="upnlContent" runat="server" UpdateMode="Conditional">
    <ContentTemplate>

        <asp:Panel ID="pnlView" runat="server" CssClass="panel panel-block" DefaultButton="btnSearch">

            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-search"></i> Universal Search</h1>
            </div>
            <div class="panel-body">
                <Rock:NotificationBox ID="nbWarnings" runat="server" NotificationBoxType="Warning" />

                <asp:Literal ID="lPreHtml" runat="server" />
                <div class="input-group searchbox">
                    <div class="input-group-addon"><i class="fa fa-search"></i></div>
                    <asp:TextBox id="tbSearch" runat="server" CssClass="form-control" Placeholder="Search" />

                    <span id="spanButtonGroup" runat="server" class="input-group-btn">
                        <asp:LinkButton ID="btnSearch" CssClass="btn btn-primary" runat="server" OnClick="btnSearch_Click">Go</asp:LinkButton>
                    </span>
                </div>


                <div class="clearfix margin-t-sm">
                    <asp:LinkButton ID="lbRefineSearch" runat="server" Text="Refine Search" OnClick="lbRefineSearch_Click" CssClass="pull-right" />
                </div>
                <asp:Literal ID="lPostHtml" runat="server" />


                <asp:Panel ID="pnlRefineSearch" runat="server" Visible="false">
                    <div class="well margin-t-md">
                        <h4>Filters</h4>
                        <Rock:RockCheckBoxList ID="cblModelFilter" runat="server" CssClass="js-model-filter" RepeatDirection="Horizontal" Label="Information Types" />

                        <hr id="hrSeparator" runat="server" />

                        <div class="row">
                            <asp:PlaceHolder ID="phFilters" runat="server" />
                        </div>
                    </div>
                </asp:Panel>

                <div class="margin-t-lg">
                    <asp:Literal ID="lResults" runat="server" />
                </div>

                <asp:Panel ID="pnlPagination" runat="server" CssClass="text-center">
                     <ul class="pagination">
                        <asp:Literal ID="lPagination" runat="server" />
                    </ul>
                </asp:Panel>
            </div>

        </asp:Panel>

        <asp:Panel ID="pnlEditModal" runat="server" Visible="false">
            <Rock:ModalDialog ID="mdEdit" runat="server" OnSaveClick="lbSave_Click" Title="Universal Search Configuration" OnCancelScript="clearDialog();">
                <Content>

                    <asp:UpdatePanel ID="upnlEdit" runat="server">
                        <ContentTemplate>
                            <div class="row">
                                <div class="col-md-6">
                                    <Rock:RockDropDownList ID="RockDropDownList1" runat="server" Label="Search Type" />
                                </div>
                            </div>"

                            <div class="row">
                                <div class="col-md-6">
                                    <Rock:RockDropDownList ID="ddlSearchType" runat="server" Label="Search Type" />
                                    <Rock:RockTextBox ID="tbResultsPerPage" runat="server" Label="Results Per Page" CssClass="input-width-sm" />
                                    <Rock:RockCheckBox ID="cbShowRefinedSearch" runat="server" Label="Show Refinded Search Options" />
                                    <Rock:RockCheckBox ID="cbShowScores" runat="server" Label="Show Scores" Help="Enables the display of scores for help with debugging." />
                                    <Rock:RockCheckBox ID="cbUseCustomResults" runat="server" Label="Use Custom Results Template" Help="Determines if the custom Lava results template should be used." />
                                    <Rock:RockCheckBox ID="cbShowFilter" runat="server" Label="Show Model Filter" />
                                </div>
                                <div class="col-md-6">
                                    <Rock:RockTextBox ID="tbBaseFieldFilters" runat="server" Label="Base Field Filters" Help="These field filters will always be enabled and will not be changeable by the individual. Uses the same syntax as the lava command." />
                                    <Rock:RockCheckBoxList ID="cblEnabledModels" runat="server" Label="Enabled Models" />
                                    <Rock:RockCheckBoxList ID="cblDisabledSecurityModels" runat="server" Label="Disable Security Models" Help="Determines which models security checking should be disabled on to improve load times. This only applies to results shown using the default templates. Security is never processed when using the 'Custom Results Template'." />
                                    
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <Rock:CodeEditor ID="ceCustomResultsTemplate" runat="server" Label="Custom Results Template" EditorMode="Lava" EditorTheme="Rock" />
                                    <Rock:RockCheckBoxList ID="cblLavaCommands" runat="server" RepeatDirection="Horizontal" Label="Custom Results Lava Commands" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6">
                                    <Rock:CodeEditor ID="cePreHtml" runat="server" Label="Search Input Pre-HTML" Help="Custom Lava to place before the search input (for styling)." EditorMode="Lava" EditorTheme="Rock" />
                                </div>
                                <div class="col-md-6">
                                    <Rock:CodeEditor ID="cePostHtml" runat="server" Label="Search Input Post-HTML" Help="Custom Lava to place after the search input (for styling)." EditorMode="Lava" EditorTheme="Rock" />
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </Content>
            </Rock:ModalDialog>

        </asp:Panel>

        <script>
            Sys.Application.add_load( function () {
                $(".model-cannavigate").on("click", function () {
                    window.document.location = $(this).data("href");
                });

                $(".js-model-filter input").on('change', function () {

                    var entityId = $(this).val();
                    var selector = ".js-entity-id-" + entityId + " input";

                    $(selector).prop("checked", $(this).is(':checked'));
                });

                $(".js-entity-filter-field input").on('click', function () {

                    var input = $(this);
                    
                    if (input.val()) {
                        var parentNode = input.parents(".js-entity-filter-field");
                        var entityClass = [...parentNode[0].classList].find((c) => c.indexOf("js-entity-id-") >= 0);
                        var entityId = entityClass.replace("js-entity-id-", "");

                        $(".js-model-filter input[value='" + entityId + "'").prop("checked", true);
                    }
                });
            });
        </script>

    </ContentTemplate>
</asp:UpdatePanel>
