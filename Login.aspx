<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SiTConnect.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login Form</title>
    <script src="https://www.google.com/recaptcha/api.js?render=6LcPDEEdAAAAAMPySV3zO2ULM9lSUrNHcQhpgez3"></script>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            SITConnect Login<br />
            <br />
            <br />
            <table class="auto-style1">
                <tr>
                    <td>
                        <asp:Label ID="lb_email" runat="server" Text="Email:"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox ID="tb_email" runat="server" onkeyup="javascript:validateEmail()"></asp:TextBox>
                        <asp:Label ID="lb_email_error" runat="server" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lb_pwd" runat="server" Text="Password:"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox ID="tb_pwd" runat="server" onkeyup="javascript:validatePwd()" TextMode="Password"></asp:TextBox>
                        <asp:Label ID="lb_pwd_error" runat="server" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>

                <tr>
                    <td>
                        <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
                        <button type="button" onclick="javascript:validateAll()">Login</button>
                        <asp:Button ID="btn_login" runat="server" OnClick="btn_login_Click" Text="Login" Width="128px" style="visibility:hidden;" />
                        <br />
                        <br />
                        <asp:Label ID="lb_error" runat="server" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
    </form>
    <script>
        var emptyError = "This cannot be empty";
        var specialError = "This cannot contains special characters";

        function validateEmail() {
            var email = document.getElementById("<%=tb_email.ClientID%>").value;
            var pattern = /\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*/;
            if (email == "") {
                document.getElementById("lb_email_error").innerHTML = emptyError;
                document.getElementById("lb_email_error").style.color = "Red";
            } else if (!(pattern.test(email))) {
                document.getElementById("lb_email_error").innerHTML = "Invalid Email format";
                document.getElementById("lb_email_error").style.display = "Red";
            } else {
                document.getElementById("lb_email_error").style.display = "none";
                return true;
            }
            document.getElementById("lb_email_error").style.display = "block";
            return false;
        }

        function validatePwd() {
            var pwd = document.getElementById("<%=tb_pwd.ClientID%>").value;
            if (pwd == "") {
                document.getElementById("lb_pwd_error").innerHTML = emptyError;
                document.getElementById("lb_pwd_error").style.color = "Red";
            } else {
                document.getElementById("lb_pwd_error").style.display = "none";
                return true;
            }
            document.getElementById("lb_pwd_error").style.display = "block";
            return false;
        }

        function validateAll() {
            console.log("Validating All input..")

            var valid = true;
            valid = validateEmail();
            valid = validatePwd();
                
            if (validateEmail() && validatePwd()) {
                document.getElementById("<%=btn_login.ClientID%>").click();
            }

        }
    </script>
    <script>
         grecaptcha.ready(function () {
             grecaptcha.execute('6LcPDEEdAAAAAMPySV3zO2ULM9lSUrNHcQhpgez3', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
             });
         });
    </script>
</body>
</html>
