<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="P_EstablishType.aspx.cs" Inherits="MaterialRecord.P_EstablishType" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        function OpenAddModal() {
            $('#AddModal').modal({ keyboard: false, backdrop: 'static' });
            $('#AddModal').on('shown.bs.modal', function () {
            });
        };
        function OpenDeleteModal() {
            $('#DeleteModal').modal({ keyboard: false, backdrop: 'static' });
            $('#DeleteModal').on('shown.bs.modal', function () {
            });
        };
        function HideAddModal() {
            $('#AddModal').hide();
            $(".modal-backdrop").attr('style', 'display:none');
        };
        function HideDeleteModal() {
            $('#DeleteModal').hide();
            $(".modal-backdrop").attr('style', 'display:none');
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
                <div class="col-6">
                    <asp:TextBox ID="txt_condition" runat="server" class="form-control" placeholder="編號、名稱" OnTextChanged="btn_query_Click" AutoPostBack="true"></asp:TextBox>
                </div>
                <div class="col-2">
                    <asp:Button ID="btn_clear" runat="server" Text="清除" class="btn btn-dark" OnClientClick="queryClear();return false;" />
                    <asp:Button ID="btn_query" runat="server" Text="查詢" class="btn btn-secondary" OnClick="btn_query_Click" />
                </div>
                <div class="col-4" style="text-align: right;">
                    <asp:Button ID="btn_insert" runat="server" Text="新增" class="btn btn-primary" OnClick="btn_insert_Click" />&nbsp;&nbsp;
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
                                    <asp:LinkButton ID="lbtn_delete" runat="server" Text="刪除" CssClass="dropdown-item edit" CausesValidation="False" CommandArgument='<%# Eval("allData") %>' OnCommand="lbtn_delete_Command" />
                                </div>
                            </div>
                        </td>
                        <td>
                            <asp:Label Text='<%# Eval("ID") %>' runat="server" ID="IDLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("TypeName") %>' runat="server" ID="TypeNameLabel" /></td>
                        <td>
                            <asp:Label Text='<%# Eval("Remark") %>' runat="server" ID="RemarkLabel" /></td>
                    </tr>
                </ItemTemplate>
                <LayoutTemplate>
                    <table runat="server" id="itemPlaceholderContainer" style="" border="0" class="table table-striped">
                        <tr runat="server">
                            <th runat="server" style="width: 10%;">功能</th>
                            <th runat="server" style="width: 10%">編號</th>
                            <th runat="server" style="width: 40%">名稱</th>
                            <th runat="server" style="width: 40%">備註</th>
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
                                <div class="form-group row">
                                    <label for="lbl_ID" class="col-sm-4 col-form-label addRedStar">編號</label>
                                    <div class="col-sm-8">
                                        <asp:TextBox ID="txt_ID" runat="server" class="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label for="lbl_TypeName" class="col-sm-4 col-form-label addRedStar">名稱</label>
                                    <div class="col-sm-8">
                                        <asp:TextBox ID="txt_TypeName" runat="server" class="form-control"></asp:TextBox>
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
                                <asp:Button CssClass="btn btn-success" ID="btn_addSave" runat="server" Text="存檔" OnClick="btn_addSave_Click" OnClientClick="ShowProgressBar();" />
                                <asp:Button CssClass="btn btn-secondary" ID="btn_addClose" runat="server" Text="關閉" OnClientClick="HideAddModal();return false;" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <%--AddModal--%>

            <%--DeleteModal--%>
            <div class="container">
                <div class="modal" role="dialog" tabindex="-1" id="DeleteModal">
                    <div class="modal-dialog" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <asp:Label ID="lbl_deleteModalTitle" CssClass="modal-title" runat="server" Text="刪除確認" ForeColor="Black" Width="100%"></asp:Label>
                                <button type="button" class="close" data-dismiss="modal">&times;</button>
                                <asp:TextBox ID="txt_deleteID" runat="server" Visible="false"></asp:TextBox>
                                <%--<button type="button" class="close" data-dismiss="modal" aria-label="Close" hidden="">
                                    <span aria-hidden="true">&times;</span>
                                </button>--%>
                            </div>
                            <div class="modal-body">
                                <div class="form-group row">
                                    <asp:Label ID="lbl_delete" runat="server" Text="" class="col-form-label"></asp:Label>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <asp:Button CssClass="btn btn-danger" ID="btn_deleteSave" runat="server" Text="刪除" OnClick="btn_deleteSave_Click" OnClientClick="ShowProgressBar();" />
                                <asp:Button CssClass="btn btn-secondary" ID="btn_deleteClose" runat="server" Text="關閉" OnClientClick="HideDeleteModal();return false;" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <%--DeleteModal--%>
            </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btn_addSave" />
            <asp:PostBackTrigger ControlID="btn_deleteSave" />
            <asp:PostBackTrigger ControlID="btn_query" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
