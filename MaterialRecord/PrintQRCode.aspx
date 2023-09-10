<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintQRCode.aspx.cs" Inherits="MaterialRecord.PrintQRCode" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script type="text/javascript" src="./Scripts/jquery-3.4.1.js"></script>
    <script type="text/javascript" src="./Scripts/bootstrap.min.js"></script>
    <link rel="stylesheet" href="./Content/bootstrap.min.css" />
    <script>
        function Printiframe() {
            window.print();
        }

        // 顯示讀取遮罩
        function ShowProgressBar() {
            displayProgress();
            displayMaskFrame();
        }
        // 隱藏讀取遮罩
        function HideProgressBar() {
            var progress = $('#divProgress');
            var maskFrame = $("#divMaskFrame");
            progress.hide();
            maskFrame.hide();
        }
        // 顯示讀取畫面
        function displayProgress() {
            var w = $(document).width();
            var h = $(window).height();
            var progress = $('#divProgress');
            progress.css({ "z-index": 999999, "top": (h / 2) - (progress.height() / 2), "left": (w / 2) - (progress.width() / 2) });
            progress.show();
        }
        // 顯示遮罩畫面
        function displayMaskFrame() {
            var w = $(window).width();
            var h = $(document).height();
            var maskFrame = $("#divMaskFrame");
            maskFrame.css({ "z-index": 999998, "opacity": 0.7, "width": w, "height": h });
            maskFrame.show();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ListView ID="ListView1" runat="server" OnItemDataBound="ListView1_ItemDataBound">
            <EmptyDataTemplate>
                    <table runat="server" style="">
                        <tr>
                            <td>無符合資料</td>
                        </tr>
                    </table>
                </EmptyDataTemplate>
            <ItemTemplate>
                <div style="page-break-after: always">
                    <asp:TextBox ID="code1" runat="server" Text='<%#Eval("code") %>' hidden=""></asp:TextBox>
                    <table>
                        <tr>
                            <td>
                                <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>
            </ItemTemplate>
        </asp:ListView>

        <div id="divProgress" style="text-align: center; display: none; position: fixed; top: 50%; left: 50%;">
            <asp:Image ID="imgLoading" runat="server" ImageUrl="~/gif/Loading_Bar.gif" />
            <br />
            <asp:Label ID="Label19" runat="server" Text="資料讀取中" Style="color: red; font-size: 20px;"></asp:Label>
        </div>
        <div id="divMaskFrame" style="background-color: #F2F4F7; display: none; left: 0px; position: absolute; top: 0px;">
        </div>
    </form>
</body>
</html>
