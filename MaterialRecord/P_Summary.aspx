<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="P_Summary.aspx.cs" Inherits="MaterialRecord.P_Summary" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        function OpenAddModal() {
            $('#AddModal').modal({ keyboard: false, backdrop: 'static' });
            $('#AddModal').on('shown.bs.modal', function () {
            });
        };
        function OpenHintModal() {
            $('#HintModal').modal({ keyboard: false, backdrop: 'static' });
            $('#HintModal').on('shown.bs.modal', function () {
            });
        };
        function OpenHistoryModal() {
            $('#HistoryModal').modal({ keyboard: false, backdrop: 'static' });
            $('#HistoryModal').on('shown.bs.modal', function () {
            });
        };
        function OpenPrintModal() {
            $('#PrintModal').modal({ keyboard: false, backdrop: 'static' });
            $('#PrintModal').on('shown.bs.modal', function () {
            });
        };
        function HideAddModal() {
            $('#AddModal').hide();
            $(".modal-backdrop").attr('style', 'display:none');
        };
        function HideHintModal() {
            $('#HintModal').hide();
            $(".modal-backdrop").attr('style', 'display:none');
        };
        function HideHistoryModal() {
            $('#HistoryModal').hide();
            $(".modal-backdrop").attr('style', 'display:none');
        };
        function HidePrintModal() {
            $('#PrintModal').hide();
            $(".modal-backdrop").attr('style', 'display:none');
        };
        function HideAddModalID() {
            $('#addmodal_id').attr('style', 'display:none');
        };
        function HideAddModalNumber() {
            $('#addmodal_number').attr('style', 'display:none');
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" >
        <ContentTemplate>
            <script type="text/javascript">
                Sys.Application.add_load(init);
            </script>

            <div class="form-row">
                <div class="col-4 row">
                    <div>
                        <asp:Label ID="lbl_state" runat="server" Text="狀態" CssClass="querytext"></asp:Label>
                        <asp:DropDownList ID="ddl_state" runat="server" CssClass="queryDropdownlist"></asp:DropDownList>&nbsp&nbsp
                    </div>
                    <div>
                        <asp:Label ID="lbl_factory" runat="server" Text="廠區" CssClass="querytext"></asp:Label>
                        <asp:DropDownList ID="ddl_queryFactory" runat="server" CssClass="queryDropdownlist"></asp:DropDownList>&nbsp&nbsp
                    </div>
                    <div>
                        <asp:Label ID="lbl_type" runat="server" Text="類別" CssClass="querytext"></asp:Label>
                        <asp:DropDownList ID="ddl_type" runat="server" CssClass="queryDropdownlist"></asp:DropDownList>&nbsp&nbsp
                    </div>
                </div>
                <%--<div class="col-2">
                    <asp:Label ID="lbl_factory" runat="server" Text="廠區" CssClass="querytext"></asp:Label>
                    <asp:DropDownList ID="ddl_queryFactory" runat="server" CssClass="queryDropdownlist"></asp:DropDownList>
                </div>--%>
                <%--<div class="col-2">
                    <asp:Label ID="lbl_type" runat="server" Text="類別" CssClass="querytext"></asp:Label>
                    <asp:DropDownList ID="ddl_type" runat="server" CssClass="queryDropdownlist"></asp:DropDownList>
                </div>--%>
                <div class="col-3">
                    <asp:TextBox ID="txt_condition" runat="server" class="form-control" placeholder="編號、名稱" OnTextChanged="btn_query_Click" AutoPostBack="true"></asp:TextBox>
                </div>
                <div class="col-2">
                    <asp:Button ID="btn_clear" runat="server" Text="清除" class="btn btn-dark" OnClientClick="queryClear();return false;" />
                    <asp:Button ID="btn_query" runat="server" Text="查詢" class="btn btn-secondary" OnClick="btn_query_Click" />
                </div>
                <div class="col-3" style="text-align: right;">
                    <asp:Button ID="btn_insert" runat="server" Text="新增" class="btn btn-primary" OnClick="btn_insert_Click" />&nbsp;&nbsp;
                    <asp:Button ID="btn_batchInsert" runat="server" Text="批次新增" class="btn btn-primary" OnClick="btn_batchInsert_Click" />&nbsp;&nbsp;
                    <asp:Button ID="btn_print" runat="server" Text="列印標籤" class="btn btn-primary" OnClick="btn_print_Click" />
                </div>
            </div>
            <br />
            <asp:ListView ID="ListView1" runat="server" OnPagePropertiesChanged="ListView1_PagePropertiesChanged">
                <EmptyDataTemplate>
                    <table runat="server" style="">
                        <tr>
                            <td>未傳回資料。</td>
                        </tr>
                    </table>
                </EmptyDataTemplate>
                <ItemTemplate>
                    <tr style="" class="trOnHoverItemTemplate" id='<%#Container.DataItemIndex + 1 %>'>
                        <td>
                            <div class="dropdown">
                                <asp:Button ID="btnGroupDrop1" runat="server" Text="功能" type="button" class="btn btn-primary WriteProtect" CausesValidation="False" OnClientClick="return false;" />
                                <div class="dropdown-menu" aria-labelledby="btnGroupDrop1">
                                    <asp:LinkButton ID="lbtn_edit" runat="server" Text="修改" CssClass="dropdown-item edit" CausesValidation="False" CommandArgument='<%# Eval("allData") %>' OnCommand="lbtn_edit_Command" />
                                    <asp:LinkButton ID="lbtn_print" runat="server" Text="列印標籤" CssClass="dropdown-item edit" CausesValidation="False" CommandArgument='<%# Eval("allData") %>' OnCommand="lbtn_print_Command" />
                                    <asp:LinkButton ID="lbtn_history" runat="server" Text="歷史記錄" CssClass="dropdown-item edit" CausesValidation="False" CommandArgument='<%# Eval("ID") %>' OnCommand="lbtn_history_Command" />
                                    <%--<asp:LinkButton ID="lbtn_scrap" runat="server" Text="報廢" CssClass="dropdown-item edit WriteProtect" CausesValidation="False" CommandArgument='<%# Eval("allData") %>' OnCommand="lbtn_scrap_Command" />
                                    <asp:LinkButton ID="lbtn_inFactory" runat="server" Text="返廠" CssClass="dropdown-item edit WriteProtect" CausesValidation="False" CommandArgument='<%# Eval("allData") %>' OnCommand="lbtn_inFactory_Command" />
                                    <asp:LinkButton ID="lbtn_outFactory" runat="server" Text="出廠" CssClass="dropdown-item edit WriteProtect" CausesValidation="False" CommandArgument='<%# Eval("allData") %>' OnCommand="lbtn_outFactory_Command" />
                                    <asp:LinkButton ID="lbtn_invalid" runat="server" Text="失效" CssClass="dropdown-item edit WriteProtect" CausesValidation="False" CommandArgument='<%# Eval("allData") %>' OnCommand="lbtn_invalid_Command" />--%>
                                    <asp:LinkButton ID="lbtn_delete" runat="server" Text="刪除" CssClass="dropdown-item edit" CausesValidation="False" CommandArgument='<%# Eval("allData") %>' OnCommand="lbtn_delete_Command" />
                                </div>
                            </div>
                        </td>
                        <td>
                            <asp:Label Text='<%# Eval("MaterialType") %>' runat="server" ID="MaterialTypeLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("ID") %>' runat="server" ID="IDLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("MaterialName") %>' runat="server" ID="MaterialNameLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("UseCount") %>' runat="server" ID="UseCountLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("Factory") %>' runat="server" ID="FactoryLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("MaterialState") %>' runat="server" ID="MaterialStateLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("CreateDate") %>' runat="server" ID="CreateDateLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("StartUseDate") %>' runat="server" ID="StartUseDateLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("ScrapDate") %>' runat="server" ID="ScrapDateLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("Remark") %>' runat="server" ID="RemarkLabel" /></td>
                    </tr>
                </ItemTemplate>
                <LayoutTemplate>
                    <table runat="server" id="itemPlaceholderContainer" style="" border="0" class="table table-striped">
                        <tr runat="server">
                            <th runat="server" style="width: 6%;">功能</th>
                            <th runat="server" style="width: 10%">類別</th>
                            <th runat="server" style="width: 10%">編號</th>
                            <th runat="server" style="width: 15%">名稱</th>
                            <th runat="server" style="width: 8%">可使用次數</th>
                            <th runat="server" style="width: 5%">廠區</th>
                            <th runat="server" style="width: 6%">狀態</th>
                            <th runat="server" style="width: 5%">建立日</th>
                            <th runat="server" style="width: 8%">開始使用日</th>
                            <th runat="server" style="width: 8%">報廢日</th>
                            <th runat="server" style="width: 19%">備註</th>
                        </tr>
                        <tr runat="server" id="itemPlaceholder"></tr>
                    </table>
                </LayoutTemplate>
            </asp:ListView>
            <div style="text-align: center">
                <asp:DataPager ID="DataPager1" runat="server" PagedControlID="ListView1" PageSize="10">
                    <Fields>
                        <asp:NextPreviousPagerField ButtonType="Button" ShowFirstPageButton="True" ShowNextPageButton="False" ButtonCssClass="btn btn-outline-secondary btn-medium" FirstPageText="第一頁" PreviousPageText="上一頁" />
                        <asp:NumericPagerField ButtonType="Button" CurrentPageLabelCssClass="btn" NextPreviousButtonCssClass="btn" NumericButtonCssClass="btn btn-outline-secondary btn-medium" />
                        <asp:NextPreviousPagerField ButtonType="Button" ShowLastPageButton="True" ShowPreviousPageButton="False" ButtonCssClass="btn btn-outline-secondary btn-medium" LastPageText="最後一頁" NextPageText="下一頁" />
                    </Fields>
                </asp:DataPager>
            </div>

            <%--AddModal--%>
            <div class="container">
                <div class="modal" role="dialog" tabindex="-1" id="AddModal">
                    <div class="modal-dialog" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <asp:Label ID="lbl_modalTitle" CssClass="modal-title" runat="server" Text="" ForeColor="Black" Width="100%"></asp:Label>
                                <button type="button" class="close" data-dismiss="modal">&times;</button>
                                <asp:TextBox ID="txt_state" runat="server" Visible="false"></asp:TextBox>
                                <%--<button type="button" class="close" data-dismiss="modal" aria-label="Close" hidden="">
                                    <span aria-hidden="true">&times;</span>
                                </button>--%>
                            </div>
                            <div class="modal-body">
                                <div>
                                    <asp:Label ID="lbl_msg" runat="server" Text="" CssClass="message"></asp:Label>
                                </div>
                                <div class="form-group row" id="addmodal_number">
                                    <label for="lbl_number" class="col-sm-4 col-form-label addRedStar">批次新增數量</label>
                                    <div class="col-sm-8">
                                        <asp:TextBox ID="txt_number" runat="server" class="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label for="lbl_MaterialType" class="col-sm-4 col-form-label addRedStar">耗材類型</label>
                                    <div class="col-sm-8">
                                        <asp:DropDownList ID="ddl_MaterialType" runat="server" CssClass="dropdownlist"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group row" id="addmodal_id">
                                    <label for="lbl_ID" class="col-sm-4 col-form-label addRedStar">編號</label>
                                    <div class="col-sm-8">
                                        <asp:TextBox ID="txt_ID" runat="server" class="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label for="lbl_MaterialName" class="col-sm-4 col-form-label addRedStar">名稱</label>
                                    <div class="col-sm-8">
                                        <asp:TextBox ID="txt_MaterialName" runat="server" class="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label for="lbl_UseCount" class="col-sm-4 col-form-label addRedStar">可使用次數</label>
                                    <div class="col-sm-8">
                                        <asp:TextBox ID="txt_UseCount" runat="server" class="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label for="lbl_Factory" class="col-sm-4 col-form-label addRedStar">廠區</label>
                                    <div class="col-sm-8">
                                        <asp:DropDownList ID="ddl_Factory" runat="server" CssClass="dropdownlist"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label for="lbl_MaterialState" class="col-sm-4 col-form-label addRedStar">狀態</label>
                                    <div class="col-sm-8">
                                        <asp:DropDownList ID="ddl_MaterialState" runat="server" CssClass="dropdownlist"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label for="lbl_Remark" class="col-sm-4 col-form-label">備註</label>
                                    <div class="col-sm-8">
                                        <asp:TextBox ID="txt_Remark" runat="server" class="form-control" Rows="3" TextMode="MultiLine"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <asp:Button CssClass="btn btn-success" ID="btn_addSave" runat="server" Text="儲存" OnClick="btn_addSave_Click" OnClientClick="ShowProgressBar();" />
                                <asp:Button CssClass="btn btn-secondary" ID="btn_addClose" runat="server" Text="關閉" OnClientClick="HideAddModal();return false;" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <%--AddModal--%>

            <%--HintModal--%>
            <div class="container">
                <div class="modal" role="dialog" tabindex="-1" id="HintModal">
                    <div class="modal-dialog" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <asp:Label ID="lbl_hintModalTitle" CssClass="modal-title" runat="server" Text="確認" ForeColor="Black" Width="100%"></asp:Label>
                                <button type="button" class="close" data-dismiss="modal">&times;</button>
                                <asp:TextBox ID="txt_hintState" runat="server" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="txt_hintData" runat="server" Visible="false"></asp:TextBox>
                                <%--<button type="button" class="close" data-dismiss="modal" aria-label="Close" hidden="">
                                    <span aria-hidden="true">&times;</span>
                                </button>--%>
                            </div>
                            <div class="modal-body">
                                <div class="form-group row">
                                    <asp:Label ID="lbl_hint" runat="server" Text="" class="col-form-label"></asp:Label>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <asp:Button CssClass="btn btn-success" ID="btn_hintConfirm" runat="server" Text="確定" OnClick="btn_hintConfirm_Click" OnClientClick="ShowProgressBar();" />
                                <asp:Button CssClass="btn btn-secondary" ID="btn_hintClose" runat="server" Text="關閉" OnClientClick="HideHintModal();return false;" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <%--HintModal--%>

            <%--HistoryModal--%>
            <div class="container">
                <div class="modal" role="dialog" tabindex="-1" id="HistoryModal">
                    <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable" role="document" style="max-width: 80%;">
                        <div class="modal-content">
                            <div class="modal-header">
                                <asp:Label ID="lbl_historyModalTitle" CssClass="modal-title" runat="server" Text="歷史紀錄" ForeColor="Black" Width="100%"></asp:Label>
                                <button type="button" class="close" data-dismiss="modal">&times;</button>
                            </div>
                            <div class="modal-body">
                                <asp:ListView ID="lv_detail" runat="server">
                                    <ItemTemplate>
                                        <tr style="">
                                            <td>
                                                <asp:Label Text='<%# Eval("DoAction") %>' runat="server" ID="DoActioneLabel" /></td>
                                            <td>
                                                <asp:Label Text='<%# Eval("MaterialType") %>' runat="server" ID="TypeNameLabel" /></td>
                                            <td>
                                                <asp:Label Text='<%# Eval("ID") %>' runat="server" ID="IDLabel" /></td>
                                            <td>
                                                <asp:Label Text='<%# Eval("MaterialName") %>' runat="server" ID="MaterialNameLabel" /></td>
                                            <td>
                                                <asp:Label Text='<%# Eval("UseCount") %>' runat="server" ID="UseCountLabel" /></td>
                                            <td>
                                                <asp:Label Text='<%# Eval("EditCount") %>' runat="server" ID="EditCountLabel" /></td>
                                            <td>
                                                <asp:Label Text='<%# Eval("Factory") %>' runat="server" ID="FactoryLabel" /></td>
                                            <td>
                                                <asp:Label Text='<%# Eval("MaterialState") %>' runat="server" ID="MaterialStateLabel" /></td>
                                            <td>
                                                <asp:Label Text='<%# Eval("Remark") %>' runat="server" ID="RemarkLabel" /></td>
                                        </tr>
                                    </ItemTemplate>
                                    <LayoutTemplate>
                                        <table runat="server" id="itemPlaceholderContainer" style="" border="0" class="table table-striped">
                                            <tr runat="server">
                                                <th runat="server" style="width: 15%;">動作</th>
                                                <th runat="server" style="width: 10%">類別</th>
                                                <th runat="server" style="width: 15%">編號</th>
                                                <th runat="server" style="width: 15%">名稱</th>
                                                <th runat="server" style="width: 10%">修改前<br>可使用次數</th>
                                                <th runat="server" style="width: 10%">修改後<br>可使用次數</th>
                                                <th runat="server" style="width: 5%">廠區</th>
                                                <th runat="server" style="width: 7%">狀態</th>
                                                <th runat="server" style="width: 13%">備註</th>
                                            </tr>
                                            <tr runat="server" id="itemPlaceholder"></tr>
                                        </table>
                                    </LayoutTemplate>

                                </asp:ListView>
                            </div>
                            <div class="modal-footer">
                                <asp:Button CssClass="btn btn-secondary" ID="btn_historyClose" runat="server" Text="關閉" OnClientClick="HideHistoryModal();return false;" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <%--HistoryModal--%>

            <%--PrintModal--%>
            <div class="container">
                <div class="modal" role="dialog" tabindex="-1" id="PrintModal">
                    <div class="modal-dialog" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <asp:Label ID="lbl_printModalTitle" CssClass="modal-title" runat="server" Text="列印標籤" ForeColor="Black" Width="100%"></asp:Label>
                                <button type="button" class="close" data-dismiss="modal">&times;</button>
                            </div>
                            <div class="modal-body">
                                <div>
                                    <asp:Label ID="lbl_printMsg" runat="server" Text="編號無需加入類別碼，僅輸入數字，如：1-99999" CssClass="message"></asp:Label>
                                </div>
                                <div class="form-group row">
                                    <label for="lbl_MaterialState" class="col-sm-4 col-form-label addRedStar">狀態</label>
                                    <div class="col-sm-8">
                                        <asp:DropDownList ID="ddl_printMaterialState" runat="server" CssClass="dropdownlist"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="form-group row">
                                    <label for="lbl_MaterialType" class="col-sm-4 col-form-label addRedStar">耗材類型</label>
                                    <div class="col-sm-8">
                                        <asp:DropDownList ID="ddl_printMaterialType" runat="server" CssClass="dropdownlist"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label for="lbl_ID" class="col-sm-4 col-form-label addRedStar">編號</label>
                                    <div class="col-sm-8 row">
                                        <div style="padding-right:15px"></div>
                                        <asp:TextBox ID="txt_startID" runat="server" class="form-control" Width="44%"></asp:TextBox>&nbsp;~&nbsp;
                                        <asp:TextBox ID="txt_endID" runat="server" class="form-control" Width="44%"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <asp:Button CssClass="btn btn-success" ID="btn_printConfirm" runat="server" Text="確定" OnClick="btn_printConfirm_Click" OnClientClick="ShowProgressBar();" />
                                <asp:Button CssClass="btn btn-secondary" ID="btn_printColse" runat="server" Text="關閉" OnClientClick="HidePrintModal();return false;" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <%--PrintModal--%>

            <%--<asp:GridView ID="GridView1" runat="server"></asp:GridView>--%>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btn_addSave" />
            <asp:PostBackTrigger ControlID="btn_hintConfirm" />
            <asp:PostBackTrigger ControlID="btn_printConfirm" />
            <asp:PostBackTrigger ControlID="btn_query" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
