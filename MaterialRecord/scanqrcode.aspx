<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="scanqrcode.aspx.cs" Inherits="MaterialRecord.scanqrcode" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>QR Scanner</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <script type="text/javascript" src="./Scripts/jquery.mobile-1.4.5.min.js"></script>
    <script type="text/javascript" src="./Scripts/jquery-3.4.1.js"></script>
    <script type="text/javascript" src="./Scripts/bootstrap.min.js"></script>
    <script type="text/javascript" src="./Scripts/title.js"></script>
    <link rel="stylesheet" href="./Content/jquery.mobile-1.4.5.min.css" />
    <link rel="stylesheet" href="./Content/bootstrap.min.css" />
    <link rel="stylesheet" href="./Content/custom.css" />
    <script type="module">

        import QrScanner from "./qr/qr-scanner.min.js";
        QrScanner.WORKER_PATH = './qr/qr-scanner-worker.min.js';

        const video = document.getElementById('qr-video');
        const scan = document.getElementById('scan-video');

        function setResult(result) {
            var mode = document.getElementById('customSwitches').checked;
            scanner.stop();
            if (mode) {
                //自動出入廠
                var factory = location.pathname.substring(3, 4);
                $.ajax({
                    type: "POST",
                    url: "scanqrcode.aspx/checkID",
                    data: '{id:"' + result + '",factory:"' + factory + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.d.toString() == "") {
                            document.getElementById('success').play();
                            alert("執行成功");
                            scanner.stop();
                            setTimeout(scanner.start(), 1000);
                        } else {
                            document.getElementById('fail').play();
                            alert(response.d.toString());
                            setTimeout(scanner.start(), 1000);
                        }
                        scanner.start();
                    },
                    error: function (response) {
                        document.getElementById('fail').play();
                        alert("ERROR: " + response.d);
                        setTimeout(scanner.start(), 1000);
                    }
                });
            } else {
                scanner.stop();
                scan.style.display = "none";
                //轉址
                document.body.innerHTML += '<form id="dynForm" action="' +location.pathname.replace('/','') + '' + location.search + '" method="post"><input type="hidden" name="fanum" ' +
                            'id="fanum" value="' + result + '"></form>';
                document.getElementById("dynForm").submit();
            }
        };
        const scanner = new QrScanner(video, result => setResult(result), error => {
            //camQrResult.textContent = error;
            //camQrResult.style.color = 'inherit';
        });
        window.onload = function () {
            scanner.start();
            $('#backToHomePage').attr('href', location.pathname.replace('/','') + location.search);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <audio id="success" src="sound/success.mp3" preload="auto" controls="controls" hidden="hidden"></audio>
        <audio id="fail" src="sound/fail.wav" preload="auto" controls="controls" hidden="hidden"></audio>
        
        <div align="center" data-role="header" data-position="fixed" id="header" style="position: relative;">
            <h1 class="title">掃描QRcode</h1>
            
            <div class="custom-control custom-switch" style="display:inline-block;text-align:right">
                <input type="checkbox" class="custom-control-input" id="customSwitches">
                <label class="custom-control-label" for="customSwitches">自動出入廠模式</label>
            </div>
        </div>
        <a href="main.aspx" class="ui-btn-left ui-btn ui-icon-back ui-btn-icon-right ui-shadow ui-corner-all" data-transition="slide" id="backToHomePage"
            data-role="button" role="button">回上一頁</a>
        <%--<a href="tyg.app.menu://" class="ui-btn ui-corner-all ui-shadow ui-icon-home ui-btn-icon-right ui-btn-right" data-transition="slide" id="backToTygApp">TYG APP</a>--%>

        <div data-role="main" class="ui-content">
            <div id="scan-video">
                <video id="qr-video" playsinline="" disablepictureinpicture="" style="transform: scaleX(-1); width: 80% !important; height: 80% !important;"></video>
            </div>
        </div>
    </form>
</body>
</html>
