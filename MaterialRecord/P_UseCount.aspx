<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="P_UseCount.aspx.cs" Inherits="MaterialRecord.P_UseCount" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        $(init); function init() {
            loadMode();
            dropdownShow();
            tableRowColor();
        }
        function OpenHintModal() {
            $('#HintModal').modal({ keyboard: false, backdrop: 'static' });
            $('#HintModal').on('shown.bs.modal', function () {
            });
        };
        function HideHintModal() {
            $('#HintModal').hide();
            $(".modal-backdrop").attr('style', 'display:none');
        };
        function checkAll() {
            var check = document.getElementById('ContentPlaceHolder1_ListView1_chk_allselected').checked;
            $('input:checkbox').prop("checked", check);
        };
        function pageCheck(allcheck) {
            allcheck = allcheck.toLowerCase() === 'true';
            document.getElementById('ContentPlaceHolder1_ListView1_chk_allselected').checked = allcheck;
        };
        function summaryMode() {
            $('#ContentPlaceHolder1_hf_mode').attr('value', 'summary');
            $('.summary').attr('style', 'display:inline');
            $('.batch').attr('style', 'display:none');
        };
        function batchMode() {
            $('#ContentPlaceHolder1_hf_mode').attr('value', 'batch');
            $('.summary').attr('style', 'display:none');
            $('.batch').attr('style', 'display:inline');
        };
        function loadMode() {
            var mode = $('#ContentPlaceHolder1_hf_mode').val();
            if (mode == "batch") {
                batchMode();
            } else {
                summaryMode();
            }
        };
        function hideHintDiv() {
            $('#div_recycling').attr('style', 'display:none');
            $('#div_remark').attr('style', 'display:none');
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="hf_mode" runat="server" />

    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <script type="text/javascript">
                Sys.Application.add_load(init);
            </script>
            <asp:TextBox ID="txt_check" runat="server" Visible="false"></asp:TextBox>

            <div class="form-row">
                <div class="col-4 row">
                    <div>
                        <asp:Label ID="lbl_state" runat="server" Text="狀態" CssClass="querytext"></asp:Label>
                        <asp:DropDownList ID="ddl_state" runat="server" CssClass="queryDropdownlist"></asp:DropDownList>&nbsp;&nbsp;
                    </div>
                    <div>
                        <asp:Label ID="lbl_factory" runat="server" Text="廠區" CssClass="querytext"></asp:Label>
                        <asp:DropDownList ID="ddl_queryFactory" runat="server" CssClass="queryDropdownlist"></asp:DropDownList>&nbsp;&nbsp;
                    </div>
                    <div>
                        <asp:Label ID="lbl_type" runat="server" Text="類別" CssClass="querytext"></asp:Label>
                        <asp:DropDownList ID="ddl_type" runat="server" CssClass="queryDropdownlist"></asp:DropDownList>&nbsp;&nbsp;
                    </div>
                </div>
                <%--<div class="col-1">
                    <asp:Label ID="lbl_factory" runat="server" Text="廠區" CssClass="querytext"></asp:Label>
                    <asp:DropDownList ID="ddl_queryFactory" runat="server" CssClass="dropdownlist" Width="70%"></asp:DropDownList>
                </div>--%>
                <%--<div class="col-1">
                    <asp:Label ID="lbl_type" runat="server" Text="類別" CssClass="querytext"></asp:Label>
                    <asp:DropDownList ID="ddl_type" runat="server" CssClass="dropdownlist" Width="70%"></asp:DropDownList>
                </div>--%>
                <div class="col-3">
                    <asp:TextBox ID="txt_condition" runat="server" class="form-control" placeholder="編號、名稱"></asp:TextBox>
                </div>
                <div class="col-2">
                    <asp:Button ID="btn_clear" runat="server" Text="清除" class="btn btn-dark" OnClientClick="queryClear();return false;" />
                    <asp:Button ID="btn_query" runat="server" Text="查詢" class="btn btn-secondary" OnClick="btn_query_Click" />
                </div>
                <div class="col-3" style="text-align: right;">
                    <div class="summary">
                        <asp:Button ID="btn_batchMode" runat="server" Text="批次修改" class="btn btn-primary" OnClick="btn_batchMode_Click" />&nbsp;&nbsp;
                    </div>
                    <div class="batch">
                        <asp:Button ID="btn_summary" runat="server" Text="放棄" class="btn btn-primary" OnClick="btn_summary_Click" />&nbsp;&nbsp;
                <asp:Button ID="btn_batchEdit" runat="server" Text="修改" class="btn btn-success" OnClick="btn_batchEdit_Click" />&nbsp;&nbsp;
                    </div>
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
                        <td style="text-align: center">
                            <div class="summary">
                                <div class="dropdown">
                                    <div class=""></div>
                                    <asp:Button ID="btnGroupDrop1" runat="server" Text="功能" type="button" class="btn btn-primary WriteProtect" CausesValidation="False" OnClientClick="return false;" />
                                    <div class="dropdown-menu" aria-labelledby="btnGroupDrop1">
                                        <asp:LinkButton ID="lbtn_recycling" runat="server" Text="可使用次數修改" CssClass="dropdown-item edit" CausesValidation="False" CommandArgument='<%# Eval("allData") %>' OnCommand="lbtn_recycling_Command" />
                                    </div>
                                </div>
                            </div>
                            <div class="batch">
                                <asp:CheckBox ID="chk_row" runat="server" AutoPostBack="true" OnCheckedChanged="chk_row_CheckedChanged" />
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
                            <th runat="server" style="width: 5%; text-align: center">
                                <%--<asp:CheckBox ID="chk_allselected" runat="server" OnCheckedChanged="chk_allselected_CheckedChanged" AutoPostBack="true" />全選</th>--%>
                                <div class="summary">
                                    功能
                                </div>
                                <div class="batch">
                                    <asp:CheckBox ID="chk_allselected" runat="server" OnClick="checkAll();" AutoPostBack="true"/>全選
                                </div>
                            </th>
                            <th runat="server" style="width: 10%">類別</th>
                            <th runat="server" style="width: 10%">編號</th>
                            <th runat="server" style="width: 15%">名稱</th>
                            <th runat="server" style="width: 8%">可使用次數</th>
                            <th runat="server" style="width: 5%">廠區</th>
                            <th runat="server" style="width: 6%">狀態</th>
                            <th runat="server" style="width: 5%">建立日</th>
                            <th runat="server" style="width: 8%">開始使用日</th>
                            <th runat="server" style="width: 8%">報廢日</th>
                            <th runat="server" style="width: 20%">備註</th>
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
                                <div class="form-group row" id="div_recycling">
                                    <label for="lbl_UseCount" class="col-sm-4 col-form-label">可使用次數</label>
                                    <div class="col-sm-8">
                                        <asp:TextBox ID="txt_UseCount" runat="server" class="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group row" id="div_remark">
                                    <label for="lbl_Remark" class="col-sm-4 col-form-label">備註</label>
                                    <div class="col-sm-8">
                                        <asp:TextBox ID="txt_Remark" runat="server" class="form-control" Rows="3" TextMode="MultiLine"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <asp:Button CssClass="btn btn-success" ID="btn_hintConfirm" runat="server" Text="確定" OnClick="btn_hintConfirm_Click" OnClientClick="ShowProgressBar();" />
                                <asp:Button CssClass="btn btn-secondary" ID="btn_hintClose" runat="server" Text="取消" OnClientClick="HideHintModal();return false;" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <%--HintModal--%>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btn_hintConfirm" />
            <asp:PostBackTrigger ControlID="btn_query" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
