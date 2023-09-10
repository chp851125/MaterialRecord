<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="P_InFactory.aspx.cs" Inherits="MaterialRecord.P_InFactory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
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
        function hideHintDiv() {
            $('#div_remark').attr('style', 'display:none');
        };
        function autoQuery() {
            document.getElementById('btn_query').click();
        };
         function autoFactory() {
            var mode = document.getElementById('ContentPlaceHolder1_hf_mode').value;
            if (mode === 'true') {
                document.getElementById('ContentPlaceHolder1_btn_inFactory').click();
            }
        };

        $(function () {
            $('#customSwitches').click(function () {
                var mode = document.getElementById('customSwitches').checked;
                $('#ContentPlaceHolder1_hf_mode').attr('value', mode);
            });
        });

        $(init); function init() {
            var mode = document.getElementById('ContentPlaceHolder1_hf_mode').value;
            if (mode == 'true' || mode == 'false') {
                document.getElementById('customSwitches').checked = (mode === 'true');
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="hf_id" runat="server" />
    <asp:HiddenField ID="hf_mode" runat="server" />
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <script type="text/javascript">
                Sys.Application.add_load(init);
            </script>

            <div class="form-row">
                <div class="col-3">
                    <asp:TextBox ID="txt_condition" runat="server" class="form-control" placeholder="編號" AutoPostBack="true" OnTextChanged="btn_query_Click"></asp:TextBox>
                </div>
                <div class="col-2">
                    <asp:Button ID="btn_clear" runat="server" Text="清除" class="btn btn-dark" OnClientClick="queryClear();return false;" />
                    <asp:Button ID="btn_query" runat="server" Text="查詢" class="btn btn-secondary" OnClick="btn_query_Click" />
                </div>
                <div class="col-7" style="text-align: right;">
                    <div class="custom-control custom-switch" style="display:inline-block;text-align:right">
                        <input type="checkbox" class="custom-control-input" id="customSwitches">
                        <label class="custom-control-label" for="customSwitches">自動返廠模式</label>
                    </div>
                    &nbsp&nbsp
                    <asp:Button ID="btn_code" runat="server" Text="掃描條碼" class="btn btn-primary" OnClick="btn_code_Click" />&nbsp&nbsp
                    <%--<asp:Button ID="btn_inFactory" runat="server" Text="返廠" class="btn btn-primary" OnClick="btn_inFactory_Click" />&nbsp&nbsp--%>
                    <asp:Button ID="btn_scrap" runat="server" Text="報廢" class="btn btn-primary" OnClick="btn_scrap_Click" />&nbsp&nbsp
                </div>
            </div>
            <br />
            <asp:Button ID="btn_inFactory" runat="server" Text="返廠" class="btn btn-primary" OnClick="btn_inFactory_Click" Width="33%" />&nbsp&nbsp
            <br /><br />

            <asp:ListView ID="ListView1" runat="server" OnPagePropertiesChanged="ListView1_PagePropertiesChanged">
                <EmptyDataTemplate>
                    <asp:Label ID="lbl_msg" runat="server" Text="" CssClass="message"></asp:Label>
                </EmptyDataTemplate>
                <ItemTemplate>
                    <tr style="" class="trOnHoverItemTemplate" id='<%#Container.DataItemIndex + 1 %>'>
                        <td>
                            <asp:Label Text='<%# Eval("CreateTime") %>' runat="server" ID="CreateTimeLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("DoAction") %>' runat="server" ID="DoActionLabel" /></td>
                        <%--<td>
                            <asp:Label Text='<%# Eval("MaterialType") %>' runat="server" ID="MaterialTypeLabel" /></td>--%>
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
                        <%--<td>
                            <asp:Label Text='<%# Eval("StartUseDate") %>' runat="server" ID="StartUseDateLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("ScrapDate") %>' runat="server" ID="ScrapDateLabel" /></td>--%>
                        <td>
                            <asp:Label Text='<%# Eval("Remark") %>' runat="server" ID="RemarkLabel" /></td>
                    </tr>
                </ItemTemplate>
                <LayoutTemplate>
                    <table runat="server" id="itemPlaceholderContainer" style="" border="0" class="table table-striped">
                        <tr runat="server">
                            <th runat="server" style="width: 15%">建立時間</th>
                            <th runat="server" style="width: 12%">動作</th>
                            <th runat="server" style="width: 10%">編號</th>
                            <th runat="server" style="width: 18%">名稱</th>
                            <th runat="server" style="width: 10%">修改前<br>可使用次數</th>
                            <th runat="server" style="width: 10%">修改後<br>可使用次數</th>
                            <th runat="server" style="width: 5%">廠區</th>
                            <th runat="server" style="width: 7%">狀態</th>
                            <%--<th runat="server" style="width: 6%">開始使用日</th>
                            <th runat="server" style="width: 5%">報廢日</th>--%>
                            <th runat="server" style="width: 13%">備註</th>
                        </tr>
                        <tr runat="server" id="itemPlaceholder"></tr>
                    </table>
                </LayoutTemplate>
            </asp:ListView>
            <div style="text-align: center">
                <asp:DataPager ID="DataPager1" runat="server" PagedControlID="ListView1" PageSize="5">
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
            <asp:PostBackTrigger ControlID="btn_code" />
            <asp:PostBackTrigger ControlID="txt_condition" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
