<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="R_StatisticsCount.aspx.cs" Inherits="MaterialRecord.R_StatisticsCount" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        function OpenHistoryModal() {
            $('#HistoryModal').modal({ keyboard: false, backdrop: 'static' });
            $('#HistoryModal').on('shown.bs.modal', function () {
            });
        };
        function HideHistoryModal() {
            $('#HistoryModal').hide();
            $(".modal-backdrop").attr('style', 'display:none');
        };
        function queryClear() {
            document.getElementById('ContentPlaceHolder1_txt_startDate').value = '';
            document.getElementById('ContentPlaceHolder1_txt_endDate').value = '';
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

            <div class="form-row">
                <%-- <div class="col-1">
                    <asp:Label ID="lbl_state" runat="server" Text="狀態" CssClass="querytext"></asp:Label>
                    <asp:DropDownList ID="ddl_state" runat="server" CssClass="queryDropdownlist"></asp:DropDownList>
                </div>--%>
                <div class="col-3 row">
                    <div>
                        <asp:Label ID="lbl_factory" runat="server" Text="廠區" CssClass="querytext"></asp:Label>
                        <asp:DropDownList ID="ddl_queryFactory" runat="server" CssClass="queryDropdownlist"></asp:DropDownList>&nbsp&nbsp
                    </div>
                    <div>
                        <asp:Label ID="lbl_type" runat="server" Text="類別" CssClass="querytext"></asp:Label>
                        <asp:DropDownList ID="ddl_type" runat="server" CssClass="queryDropdownlist"></asp:DropDownList>&nbsp&nbsp
                    </div>
                </div>
                <%--<div class="col-1">
                    <asp:Label ID="lbl_type" runat="server" Text="類別" CssClass="querytext"></asp:Label>
                    <asp:DropDownList ID="ddl_type" runat="server" CssClass="dropdownlist" Width="70%"></asp:DropDownList>
                </div>--%>
                <div class="col-8 row">
                    <div>
                        <asp:Label ID="lbl_dateRange" runat="server" Text="日期區間" CssClass="querytext"></asp:Label>
                        <asp:DropDownList ID="ddl_dateType" runat="server" CssClass="queryDropdownlist" Width="1px" style="visibility:hidden;"></asp:DropDownList>&nbsp
                    </div>
                    <div>
                        <asp:TextBox ID="txt_startDate" runat="server" class="form-control" placeholder="輸入格式：YYYYMMDD" CssClass="datepicker" Width="45%"></asp:TextBox> ~ 
                        <asp:TextBox ID="txt_endDate" runat="server" class="form-control" placeholder="輸入格式：YYYYMMDD" CssClass="datepicker" Width="45%"></asp:TextBox>&nbsp
                    </div>
                    <div>
                        <asp:Button ID="btn_clear" runat="server" Text="清除" class="btn btn-dark" OnClientClick="queryClear();return false;" />&nbsp
                        <asp:Button ID="btn_query" runat="server" Text="查詢" class="btn btn-secondary" OnClick="btn_query_Click" />
                    </div>
                </div>
                <%--<div class="col-2">
                    <asp:Label ID="lbl_sum" runat="server" Text="新增量：0 / 報廢量：0" CssClass="message" Font-Size="20px"></asp:Label>
                </div>--%>
                <div class="col-1" style="text-align:right">
                    <asp:Button ID="btn_excel" runat="server" Text="匯出Excel" class="btn btn-primary" OnClick="btn_excel_Click" />
                </div>
            </div>
            <br />
            <asp:Label ID="lbl_sum" runat="server" Text="新增量：0 / 報廢量：0" CssClass="message" Font-Size="20px" ForeColor="Black"></asp:Label>
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
                        <%--<td style="text-align: center">
                            <div class="dropdown">
                                <div class=""></div>
                                <asp:Button ID="btnGroupDrop1" runat="server" Text="查看" type="button" class="btn btn-primary WriteProtect" CausesValidation="False" OnClientClick="return false;" />
                                <div class="dropdown-menu" aria-labelledby="btnGroupDrop1">
                                    <asp:LinkButton ID="lbtn_history" runat="server" Text="歷史記錄" CssClass="dropdown-item edit WriteProtect" CausesValidation="False" CommandArgument='<%# Eval("ID") %>' OnCommand="lbtn_history_Command" />
                                </div>
                            </div>
                        </td>--%>
                        <td>
                            <asp:Label Text='<%# Eval("state") %>' runat="server" ID="stateLabel" /></td>
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
                            <%--<th runat="server" style="width: 5%;">功能</th>--%>
                            <th runat="server" style="width: 8%;">新增/報廢</th>
                            <th runat="server" style="width: 10%">類別</th>
                            <th runat="server" style="width: 10%">編號</th>
                            <th runat="server" style="width: 15%">名稱</th>
                            <th runat="server" style="width: 8%">可使用次數</th>
                            <th runat="server" style="width: 5%">廠區</th>
                            <th runat="server" style="width: 6%">狀態</th>
                            <th runat="server" style="width: 5%">建立日</th>
                            <th runat="server" style="width: 8%">開始使用日</th>
                            <th runat="server" style="width: 6%">報廢日</th>
                            <th runat="server" style="width: 16%">備註</th>
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

            <%--HistoryModal--%>
            <div class="container">
                <div class="modal" role="dialog" tabindex="-1" id="HistoryModal">
                    <div class="modal-dialog" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <asp:Label ID="lbl_historyModalTitle" CssClass="modal-title" runat="server" Text="歷史記錄" ForeColor="Black" Width="100%"></asp:Label>
                                <button type="button" class="close" data-dismiss="modal">&times;</button>
                            </div>
                            <div class="modal-body">
                                <asp:ListView ID="lv_detail" runat="server">
                                    <ItemTemplate>
                                        <tr style="">
                                            <td>
                                                <asp:Label Text='<%# Eval("DoAction") %>' runat="server" ID="DoActioneLabel" /></td>
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
                                                <asp:Label Text='<%# Eval("Remark") %>' runat="server" ID="RemarkLabel" /></td>
                                        </tr>
                                    </ItemTemplate>
                                    <LayoutTemplate>
                                        <table runat="server" id="itemPlaceholderContainer" style="" border="0" class="table table-bordered">
                                            <tr runat="server">
                                                <th runat="server" style="width: 5%;">動作</th>
                                                <th runat="server" style="width: 10%">類別</th>
                                                <th runat="server" style="width: 15%">編號</th>
                                                <th runat="server" style="width: 30%">名稱</th>
                                                <th runat="server" style="width: 10%">可使用次數</th>
                                                <th runat="server" style="width: 5%">廠區</th>
                                                <th runat="server" style="width: 5%">狀態</th>
                                                <th runat="server" style="width: 20%">備註</th>
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

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btn_query" />
            <asp:PostBackTrigger ControlID="btn_excel" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
        