<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="SiTConnect.Home" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            height: 26px;
        }
        .auto-style3 {
            height: 24px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            SITConnect Home Page<br />
            <br />
            Profile Detials<br />
            <br />
            <asp:Image ImageUrl="../Images/2022 4-04-35 PM183e1190-e589-4d0c-97f5-5cff6dbfae23" ID="image" runat="server" Height="76px" Width="85px" />
            <br />
            <table class="auto-style1">
                <tr>
                    <td>First Name:</td>
                    <td>
                        <asp:Label ID="lb_fname" runat="server" Text="Label"></asp:Label>
                    </td>
                    <td>Last Name:</td>
                    <td>
                        <asp:Label ID="lb_lname" runat="server" Text="Label"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">Email</td>
                    <td class="auto-style3">
                        <asp:Label ID="lb_email" runat="server" Text="Label"></asp:Label>
                    </td>
                    <td class="auto-style3">Date Of Birth:</td>
                    <td class="auto-style3">
                        <asp:Label ID="lb_dob" runat="server" Text="Label"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2"></td>
                    <td class="auto-style2"></td>
                    <td class="auto-style2"></td>
                    <td class="auto-style2"></td>
                </tr>
                <tr>
                    <td class="auto-style2" colspan="2">Credit Card Number:</td>
                    <td class="auto-style2" colspan="2">
                        <asp:Label ID="lb_card_no" runat="server" Text="lb_card_no"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">Expiry Date:</td>
                    <td class="auto-style2">
                        <asp:Label ID="lb_expiry_date" runat="server" Text="Label"></asp:Label>
                    </td>
                    <td class="auto-style2">Security Code</td>
                    <td class="auto-style2">
                        <asp:Label ID="lb_security_code" runat="server" Text="Label"></asp:Label>
                    </td>
                </tr>
            </table>
            <br />
            <br />

            <asp:Button ID="btn_change_pwd" runat="server" OnClick="Button1_Click" Text="Change Password" Width="128px" />
        </div>
        <p>
            <asp:Button ID="btn_logout" runat="server" OnClick="btn_logout_Click" Text="Logout" />
        </p>
    </form>
    
</body>
</html>
