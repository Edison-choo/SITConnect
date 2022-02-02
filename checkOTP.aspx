<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="checkOTP.aspx.cs" Inherits="SiTConnect.checkOTP" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            width: 129px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            Enter OTP received in email<br />
            <br />
            <table class="auto-style1">
                <tr>
                    <td class="auto-style2">
            <asp:TextBox ID="tb_otp" runat="server" onkeyup="javascript:validateOTP()"></asp:TextBox>
                    </td>
                    <td>
            <asp:Label ID="lb_otp_error" runat="server" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
            </table>
            <br />
            <asp:Label ID="lb_error" runat="server" ForeColor="Red"></asp:Label>
            <br />
            <button type="button" onclick="javascript:validateAll()">Submit</button>
            <asp:Button ID="btn_submit" runat="server" OnClick="btn_submit_Click" Text="Submit" style="visibility:hidden;" />
        </div>
    </form>
    <script>
        var emptyError = "This cannot be empty";
        var patternError = "This can only contains numeric characters";

        function validateOTP() {
            var otp = document.getElementById("<%=tb_otp.ClientID%>").value;
            if (otp == "") {
                 document.getElementById("lb_otp_error").innerHTML = emptyError;
                 document.getElementById("lb_otp_error").style.color = "Red";
            } else if (!(otp.search(/[^0-9]/) == -1)) {
                 document.getElementById("lb_otp_error").innerHTML = patternError;
                 document.getElementById("lb_otp_error").style.color = "Red";
            } else if (otp.length != 6) {
                document.getElementById("lb_otp_error").innerHTML = "OTP value is 6 digit long";
                document.getElementById("lb_otp_error").style.color = "Red";
            } else {
                 document.getElementById("lb_otp_error").style.display = "none";
                 return true;
             }
             document.getElementById("lb_otp_error").style.display = "block";
             return false;
        }

        function validateAll() {
            console.log("Validating All input..")

            if (validateOTP()) {
                document.getElementById("<%=btn_submit.ClientID%>").click();
            }
        }
    </script>
</body>
</html>
