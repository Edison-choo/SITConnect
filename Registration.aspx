<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="SiTConnect.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Registration form</title>
    <script src="https://www.google.com/recaptcha/api.js?render=6LcPDEEdAAAAAMPySV3zO2ULM9lSUrNHcQhpgez3"></script>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            width: 263px;
        }
        .auto-style4 {
            width: 219px;
            height: 30px;
        }
        .auto-style5 {
            height: 23px;
        }
        .auto-style6 {
            width: 219px;
        }
        .auto-style7 {
            width: 303px;
        }
        .auto-style8 {
            height: 30px;
            width: 14px;
        }
        .auto-style9 {
            width: 14px;
        }
        .auto-style10 {
            width: 219px;
            height: 4px;
        }
        .auto-style11 {
            height: 4px;
            width: 14px;
        }
        .auto-style12 {
            height: 4px;
        }
        .auto-style13 {
            width: 263px;
            height: 27px;
        }
        .auto-style14 {
            width: 303px;
            height: 27px;
        }
        .auto-style15 {
            height: 27px;
        }
        .auto-style16 {
            height: 27px;
            width: 66px;
        }
        .auto-style17 {
            width: 66px;
        }
        .auto-style18 {
            height: 30px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            SITConnect Registration<br />
            <br />
            <br />
            <br />
            <table class="auto-style1">
                <tr>
                    <td class="auto-style13">
                        <asp:Label ID="lb_fname" runat="server" Text="First Name"></asp:Label>
                    </td>
                    <td class="auto-style14">
                        <asp:TextBox ID="tb_fname" runat="server" Width="293px" onkeyup="javascript:validateFname()"></asp:TextBox>
                    </td>
                    <td class="auto-style16">
                    </td>
                    <td class="auto-style15">
                        <asp:Label ID="lb_fname_error" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">
                        <asp:Label ID="lb_lname" runat="server" Text="Last Name"></asp:Label>
                    </td>
                    <td class="auto-style7">
                        <asp:TextBox ID="tb_lname" runat="server" Width="292px" onkeyup="javascript:validateLname()"></asp:TextBox>
                    </td>
                    <td class="auto-style17">
                        &nbsp;</td>
                    <td>
                        <asp:Label ID="lb_lname_error" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">
                        <asp:Label ID="lb_email" runat="server" Text="Email"></asp:Label>
                    </td>
                    <td class="auto-style7">
                        <asp:TextBox ID="tb_email" runat="server" Width="292px" onkeyup="javascript:validateEmail()"></asp:TextBox>
                    </td>
                    <td class="auto-style17">
                        &nbsp;</td>
                    <td>
                        <asp:Label ID="lb_email_error" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">
                        <asp:Label ID="lbl_pwd" runat="server" Text="Password"></asp:Label>
                    </td>
                    <td class="auto-style7">
                        <asp:TextBox ID="tb_pwd" runat="server" Width="292px" TextMode="Password" onkeyup="javascript:validate()"></asp:TextBox>
                    </td>
                    <td class="auto-style17">
                        <asp:Button ID="btn_check_pwd" runat="server" OnClick="btn_check_pwd_Click" Text="Check" />
                    </td>
                    <td>
                        <asp:Label ID="lb_pwd_error" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">
                        <asp:Label ID="lb_confirm_pwd" runat="server" Text="Confirm Password"></asp:Label>
                    </td>
                    <td class="auto-style7">
                        <asp:TextBox ID="tb_confirm_pwd" runat="server" Width="291px" TextMode="Password" onkeyup="javascript:validateConfirmPwd()"></asp:TextBox>
                    </td>
                    <td class="auto-style17">
                        &nbsp;</td>
                    <td>
                        <asp:Label ID="lb_confirm_pwd_error" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">
                        <asp:Label ID="lb_dob" runat="server" Text="Date of Birth"></asp:Label>
                    </td>
                    <td class="auto-style7">
                        <asp:TextBox ID="tb_dob" runat="server" Width="289px" TextMode="Date" onkeyup="javascript:validateDob()"></asp:TextBox>
                    </td>
                    <td class="auto-style17">
                        &nbsp;</td>
                    <td>
                        <asp:Label ID="lb_dob_error" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">
                        <asp:Label ID="lb_photo" runat="server" Text="Photo"></asp:Label>
                    </td>
                    <td class="auto-style7">
                        <asp:FileUpload ID="fu_photo" runat="server" Width="305px" />
                    </td>
                    <td class="auto-style17">
                        &nbsp;</td>
                    <td>
                        <asp:Label ID="lb_photo_error" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
            <br />
            Credit Card Info<br />
             <table class="auto-style1">
                 <tr>
                     <td class="auto-style4">
                         <asp:Label ID="lb_card_no" runat="server" Text="Card Number"></asp:Label>
                     </td>
                     <td class="auto-style8"></td>
                     <td class="auto-style18"></td>
                 </tr>
                 <tr>
                     <td class="auto-style5" colspan="2">
                         <asp:TextBox ID="tb_card_no" runat="server" Width="564px" MaxLength="16" onkeyup="javascript:validateCardNo()"></asp:TextBox>
                     </td>
                     <td class="auto-style5">
                         <asp:Label ID="lb_card_no_error" runat="server"></asp:Label>
                     </td>
                 </tr>
                 <tr>
                     <td class="auto-style6">
                         <asp:Label ID="lb_card_name" runat="server" Text="Name on card"></asp:Label>
                     </td>
                     <td class="auto-style9">&nbsp;</td>
                     <td>&nbsp;</td>
                 </tr>
                 <tr>
                     <td colspan="2">
                         <asp:TextBox ID="tb_card_name" runat="server" Width="559px" onkeyup="javascript:validateCardName()"></asp:TextBox>
                     </td>
                     <td>
                         <asp:Label ID="lb_card_name_error" runat="server"></asp:Label>
                     </td>
                 </tr>
                 <tr>
                     <td class="auto-style10">
                         <asp:Label ID="lb_expiry_date" runat="server" Text="Expiry Date"></asp:Label>
                     </td>
                     <td class="auto-style11">
                         <asp:Label ID="lb_security_code" runat="server" Text="Security Code"></asp:Label>
                     </td>
                     <td class="auto-style12"></td>
                 </tr>
                 <tr>
                     <td class="auto-style6">
                         <asp:TextBox ID="tb_expiry_date" runat="server" Width="256px" onkeyup="javascript:validateExpiryDate()"></asp:TextBox>
                     </td>
                     <td class="auto-style9">
                         <asp:TextBox ID="tb_security_code" runat="server" Width="256px" MaxLength="3" onkeyup="javascript:validateSecurityCode()"></asp:TextBox>
                     </td>
                     <td>&nbsp;</td>
                 </tr>
                 <tr>
                     <td class="auto-style6">
                         <asp:Label ID="lb_expiry_date_error" runat="server"></asp:Label>
                     </td>
                     <td class="auto-style9">
                         <asp:Label ID="lb_security_code_error" runat="server"></asp:Label>
                     </td>
                     <td>&nbsp;</td>
                 </tr>
                 <tr>
                     <td class="auto-style6">
                         <asp:Label ID="error_summary" runat="server" ForeColor="Red"></asp:Label>
                     </td>
                     <td class="auto-style9">&nbsp;</td>
                     <td>&nbsp;</td>
                 </tr>
                 <tr>
                     <td colspan="2">
                         <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
                         <button type="button" onclick="javascript:validateAll()">Register</button>
                         <asp:Button ID="btn_register" runat="server" Text="Register" Width="567px" OnClick="btn_register_Click" style="visibility:hidden;" />
                     </td>
                     <td>&nbsp;</td>
                 </tr>
             </table>
        </div>
     <br />
    <script>
        var emptyError = "This cannot be empty";
        var specialError = "This cannot contains special characters";

        function validate() {
            var str = document.getElementById("<%=tb_pwd.ClientID%>").value;
            console.log(str);
            document.getElementById("lb_pwd_error").style.display = "Block";
            if (str == "") {
                document.getElementById("lb_pwd_error").innerHTML = "Please enter a value!";
                document.getElementById("lb_pwd_error").style.color = "Red";
                return false;
            } else if (str.length < 12) {
                document.getElementById("lb_pwd_error").innerHTML = "Password Length must be at least 12 characters";
                document.getElementById("lb_pwd_error").style.color = "Red";
                return false;
            } else if (str.search(/[0-9]/) == -1) {
                document.getElementById("lb_pwd_error").innerHTML = "Password require at least 1 number";
                document.getElementById("lb_pwd_error").style.color = "Red";
                return false;
            } else if (str.search(/[a-z]/) == -1) {
                document.getElementById("lb_pwd_error").innerHTML = "Password require at least a lowercase character";
                document.getElementById("lb_pwd_error").style.color = "Red";
                return false;
            } else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("lb_pwd_error").innerHTML = "Password require at least a uppercase character";
                document.getElementById("lb_pwd_error").style.color = "Red";
                return false;
            } else if (str.search(/[^0-9a-zA-Z]/) == -1) {
                document.getElementById("lb_pwd_error").innerHTML = "Password require at least a special character";
                document.getElementById("lb_pwd_error").style.color = "Red";
                return false;
            }
            document.getElementById("lb_pwd_error").innerHTML = "Strong password";
            document.getElementById("lb_pwd_error").style.color = "Blue";
            return true;
        }

        function validateFname() {
            var fname = document.getElementById("<%=tb_fname.ClientID%>").value;
            if (fname == "") {
                document.getElementById("lb_fname_error").innerHTML = emptyError;
                document.getElementById("lb_fname_error").style.color = "Red";
            } else if (!(fname.search(/[^0-9a-zA-Z\s]/) == -1)) {
                document.getElementById("lb_fname_error").innerHTML = specialError;
                document.getElementById("lb_fname_error").style.color = "Red";
            } else {
                document.getElementById("lb_fname_error").style.display = "none";
                return true;
            }
            document.getElementById("lb_fname_error").style.display = "block";
            return false;
        }

        function validateLname() {
            var lname = document.getElementById("<%=tb_lname.ClientID%>").value;
            if (lname == "") {
                document.getElementById("lb_lname_error").innerHTML = emptyError;
                document.getElementById("lb_lname_error").style.color = "Red";
            } else if (!(lname.search(/[^0-9a-zA-Z\s]/) == -1)) {
                document.getElementById("lb_lname_error").innerHTML = specialError;
                document.getElementById("lb_lname_error").style.color = "Red";
            } else {
                document.getElementById("lb_lname_error").style.display = "none";
                return true;
            }
            document.getElementById("lb_lname_error").style.display = "block";
            return false;
        }

        function validateEmail() {
            var email = document.getElementById("<%=tb_email.ClientID%>").value;
            var pattern = /\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*/;
            if (email == "") {
                document.getElementById("lb_email_error").innerHTML = emptyError;
                document.getElementById("lb_email_error").style.color = "Red";
            } else if (!(pattern.test(email))) {
                document.getElementById("lb_email_error").innerHTML = "Invalid Email format";
                document.getElementById("lb_email_error").style.color = "Red";
            } else {
                document.getElementById("lb_email_error").style.display = "none";
                return true;
            }
            document.getElementById("lb_email_error").style.display = "block";
            return false;
        }

        function validateDob() {
            var dob = document.getElementById("<%=tb_dob.ClientID%>").value;
            if (dob == "") {
                document.getElementById("lb_dob_error").innerHTML = emptyError;
                document.getElementById("lb_dob_error").style.color = "Red";
            } else {
                document.getElementById("lb_dob_error").style.display = "none";
                return true;
            }
            document.getElementById("lb_dob_error").style.display = "block";
            return false;
        }

        function validateConfirmPwd() {
            var confirmPwd = document.getElementById("<%=tb_confirm_pwd.ClientID%>").value;
            var pwd = document.getElementById("<%=tb_pwd.ClientID%>").value;
            if (confirmPwd == "") {
                document.getElementById("lb_confirm_pwd_error").innerHTML = emptyError;
                document.getElementById("lb_confirm_pwd_error").style.color = "Red";
            } else if (confirmPwd != pwd) {
                document.getElementById("lb_confirm_pwd_error").innerHTML = "Passwords do not match!";
                document.getElementById("lb_confirm_pwd_error").style.color = "Red";
            } else {
                document.getElementById("lb_confirm_pwd_error").style.display = "none";
                return true;
            }
            document.getElementById("lb_confirm_pwd_error").style.display = "block";
            return false;
        }

        function validateCardName() {
            var cardName = document.getElementById("<%=tb_card_name.ClientID%>").value;
            if (cardName == "") {
                document.getElementById("lb_card_name_error").innerHTML = emptyError;
                document.getElementById("lb_card_name_error").style.color = "Red";
            } else if (!(cardName.search(/[^0-9a-zA-Z\s]/) == -1)) {
                document.getElementById("lb_card_name_error").innerHTML = specialError;
                document.getElementById("lb_card_name_error").style.color = "Red";
            } else {
                document.getElementById("lb_card_name_error").style.display = "none";
                return true;
            }
            document.getElementById("lb_card_name_error").style.display = "block";
            return false;
        }

        function validateCardNo() {
            var cardNo = document.getElementById("<%=tb_card_no.ClientID%>").value;
            if (cardNo == "") {
                document.getElementById("lb_card_no_error").innerHTML = emptyError;
                document.getElementById("lb_card_no_error").style.color = "Red";
            } else if (!(cardNo.search(/[^0-9a-zA-Z]/) == -1)) {
                document.getElementById("lb_card_no_error").innerHTML = specialError;
                document.getElementById("lb_card_no_error").style.color = "Red";
            } else {
                document.getElementById("lb_card_no_error").style.display = "none";
                return true;
            }
            document.getElementById("lb_card_no_error").style.display = "block";
            return false;
        }

        function validateExpiryDate() {
            var expiryDate = document.getElementById("<%=tb_expiry_date.ClientID%>").value;
            if (expiryDate == "") {
                document.getElementById("lb_expiry_date_error").innerHTML = emptyError;
                document.getElementById("lb_expiry_date_error").style.color = "Red";
            } else if (expiryDate.search(/[0-9]/) == -1) {
                document.getElementById("lb_expiry_date_error").style.display = "none";
            } else if (!(expiryDate.search(/[^0-9a-zA-Z\/-]/) == -1)) {
                document.getElementById("lb_expiry_date_error").innerHTML = specialError;
                document.getElementById("lb_expiry_date_error").style.color = "Red";
            } else {
                document.getElementById("lb_expiry_date_error").style.display = "none";
                return true;
            }
            document.getElementById("lb_expiry_date_error").style.display = "block";
            return false;
        }

        function validateSecurityCode() {
            var securityCode = document.getElementById("<%=tb_security_code.ClientID%>").value;
            if (securityCode == "") {
                document.getElementById("lb_security_code_error").innerHTML = emptyError;
                document.getElementById("lb_security_code_error").style.color = "Red";
            } else if (!(securityCode.search(/[^0-9a-zA-Z]/) == -1)) {
                document.getElementById("lb_security_code_error").innerHTML = specialError;
                document.getElementById("lb_security_code_error").style.color = "Red";
            } else if (securityCode.length != 3) {
                document.getElementById("lb_security_code_error").innerHTML = "Security Code needs to be 3 numbers";
                document.getElementById("lb_security_code_error").style.color = "Red";
            } else {
                document.getElementById("lb_security_code_error").style.display = "none";
                return true;
            }
            document.getElementById("lb_security_code_error").style.display = "block";
            return false;
        }

        function validateAll() {
            console.log("Validating All input..")

            var valid = true;
            valid = validate();
            valid = validateFname();
            valid = validateLname();
            valid = validateEmail();
            valid = validateDob();
            valid = validateConfirmPwd();
            valid = validateCardName();
            valid = validateCardNo();
            valid = validateExpiryDate();
            valid = validateSecurityCode();

            if (validate() && validateFname() && validateLname() && validateEmail() && validateDob() && validateConfirmPwd() && validateCardName() && validateCardNo() && validateExpiryDate() && validateSecurityCode()) {
                document.getElementById("<%=btn_register.ClientID%>").click();
            }
        }
    </script>
    </form>
    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6LcPDEEdAAAAAMPySV3zO2ULM9lSUrNHcQhpgez3', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>
     </body>
</html>
