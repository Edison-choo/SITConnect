<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePwd.aspx.cs" Inherits="SiTConnect.ChangePwd" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            width: 298px;
        }
        .auto-style3 {
            width: 298px;
            height: 30px;
        }
        .auto-style4 {
            height: 30px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            SITConnect Change Password<br />
            <br />
            <asp:Label ID="lb_change_pwd" runat="server"></asp:Label>
            <br />
            <table class="auto-style1">
                <tr>
                    <td class="auto-style2">
                        <asp:Label ID="Label1" runat="server" Text="Current Password"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="tb_current_pwd" runat="server" onkeyup="javascript:validatePwd()" TextMode="Password"></asp:TextBox>
                        <asp:Label ID="lb_current_pwd_error" runat="server" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">
                        <asp:Label ID="Label2" runat="server" Text="New Password"></asp:Label>
                    </td>
                    <td class="auto-style4">
                        <asp:TextBox ID="tb_new_pwd" runat="server" onkeyup="javascript:validateNewPwd()" TextMode="Password"></asp:TextBox>
                        <asp:Label ID="lb_new_pwd_error" runat="server" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">
                        <asp:Label ID="Label3" runat="server" Text="Confirm New Password"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="tb_confirm_new_pwd" runat="server" TextMode="Password"></asp:TextBox>
                        <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="tb_new_pwd" ControlToValidate="tb_confirm_new_pwd" ErrorMessage="Password and confirm password must be same" ForeColor="Red"></asp:CompareValidator>
                        <asp:Label ID="lb_confirm_new_pwd_error" runat="server" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">
                        <asp:Label ID="tb_error" runat="server" ForeColor="Red"></asp:Label>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style2">&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style2">
                        <button type="button" onclick="javascript:validateAll()">Register</button>
                        <asp:Button ID="btn_submit" runat="server" OnClick="btn_submit_Click" Text="Submit" style="visibility:hidden;"/>
                    </td>
                    <td>&nbsp;</td>
                </tr>
            </table>
        </div>
    </form>
    <script>
        var emptyError = "This cannot be empty";
        var specialError = "This cannot contains special characters";

        function validateNewPwd() {
            var str = document.getElementById("<%=tb_new_pwd.ClientID%>").value;
            document.getElementById("lb_new_pwd_error").style.display = "Block";
            if (str == "") {
                document.getElementById("lb_new_pwd_error").innerHTML = "Please enter a value!";
                document.getElementById("lb_new_pwd_error").style.color = "Red";
                return false;
            } else if (str.length < 12) {
                document.getElementById("lb_new_pwd_error").innerHTML = "Password Length must be at least 12 characters";
                document.getElementById("lb_new_pwd_error").style.color = "Red";
                return false;
            } else if (str.search(/[0-9]/) == -1) {
                document.getElementById("lb_new_pwd_error").innerHTML = "Password require at least 1 number";
                document.getElementById("lb_new_pwd_error").style.color = "Red";
                return false;
            } else if (str.search(/[a-z]/) == -1) {
                document.getElementById("lb_new_pwd_error").innerHTML = "Password require at least a lowercase character";
                document.getElementById("lb_new_pwd_error").style.color = "Red";
                return false;
            } else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("lb_new_pwd_error").innerHTML = "Password require at least a uppercase character";
                document.getElementById("lb_new_pwd_error").style.color = "Red";
                return false;
            } else if (str.search(/[^0-9a-zA-Z]/) == -1) {
                document.getElementById("lb_new_pwd_error").innerHTML = "Password require at least a special character";
                document.getElementById("lb_new_pwd_error").style.color = "Red";
                return false;
            }
            document.getElementById("lb_new_pwd_error").innerHTML = "Strong password";
            document.getElementById("lb_new_pwd_error").style.color = "Blue";
            return true;
        }

        function validatePwd() {
            var pwd = document.getElementById("<%=tb_current_pwd.ClientID%>").value;
            if (pwd == "") {
                document.getElementById("lb_current_pwd_error").innerHTML = emptyError;
                document.getElementById("lb_current_pwd_error").style.color = "Red";
            } else {
                document.getElementById("lb_current_pwd_error").style.display = "none";
                return true;
            }
            document.getElementById("lb_current_pwd_error").style.display = "block";
            return false;
        }

        function validateConfirmPwd() {
            var confirmPwd = document.getElementById("<%=tb_confirm_new_pwd.ClientID%>").value;
            var pwd = document.getElementById("<%=tb_new_pwd.ClientID%>").value;
            if (confirmPwd == "") {
                document.getElementById("lb_confirm_new_pwd_error").innerHTML = emptyError;
                document.getElementById("lb_confirm_new_pwd_error").style.color = "Red";
            } else if (confirmPwd != pwd) {
                document.getElementById("lb_confirm_new_pwd_error").innerHTML = "Passwords do not match!";
                document.getElementById("lb_confirm_new_pwd_error").style.color = "Red";
            } else {
                document.getElementById("lb_confirm_new_pwd_error").style.display = "none";
                return true;
            }
            document.getElementById("lb_confirm_new_pwd_error").style.display = "block";
            return false;
        }

        function validateAll() {
            console.log("Validating All input..")

            var valid = true;
            valid = validateNewPwd();
            valid = validatePwd();
                
            if (validateNewPwd() && validatePwd() && validateConfirmPwd()) {
                document.getElementById("<%=btn_submit.ClientID%>").click();
            }

        }
    </script>
</body>
</html>
