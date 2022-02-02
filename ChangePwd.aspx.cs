using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Net;
using System.Web.Script.Serialization;
using System.IO;

namespace SiTConnect
{
    public partial class ChangePwd : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration
            .ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string newFinalHash;
        static string salt;
        static string currentPwd;
        static string firstOldPwd;
        static string secondOldPwd;
        static string email = "edisonchoo234@gmail.com";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedIn"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
                else
                {
                    email = Session["LoggedIn"].ToString();

                    if (Convert.ToBoolean(Session["OverMaxAge"].ToString()))
                    {
                        lb_change_pwd.Text = "You need to change your password";
                    }
                }
            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }
        }

        protected void btn_submit_Click(object sender, EventArgs e)
        {
            HashPwd();
            currentPwd = getDBValue(email, "PasswordHash");
            firstOldPwd = getDBValue(email, "FirstOldPassword");
            secondOldPwd = getDBValue(email, "SecondOldPassword");
            if (!(validation()))
            {
                return;
            }
            if (!(checkMinPwdAge()))
            {
                return;
            }
            if (newFinalHash == firstOldPwd || newFinalHash == secondOldPwd || newFinalHash == currentPwd)
            {
                tb_error.Text = "Password cannot be the same as your previous two passwords and your current password";
                return;
            }
            changePassword();
            Response.Redirect("Home.aspx", false);

        }

        protected void HashPwd()
        {
            string pwd = tb_current_pwd.Text.ToString().Trim();

            string newPwd = tb_new_pwd.Text.ToString().Trim();


            salt = getDBValue(email, "PasswordSalt");
            SHA512Managed hashing = new SHA512Managed();
            string pwdWithSalt = pwd + salt;
            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
            finalHash = Convert.ToBase64String(hashWithSalt);
            string newPwdWithSalt = newPwd + salt;
            byte[] newHashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(newPwdWithSalt));
            newFinalHash = Convert.ToBase64String(newHashWithSalt);
        }

        protected Boolean checkMinPwdAge()
        {
            DateTime createdAt = Convert.ToDateTime(getDBValue(email, "PasswordCreatedAt"));
            TimeSpan ts = DateTime.Now - createdAt;
            if (ts.TotalMinutes < 1)
            {
                tb_error.Text = "Need to wait until 1 minute after password creation";
                return false;
            }

            return true;
        }

        protected void changePassword()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("update account set passwordHash=@password, firstOldPassword=@firstpwd, secondOldPassword=@secondpwd, passwordCreatedAt=@createdAt where email = @email"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@password", newFinalHash);
                            cmd.Parameters.AddWithValue("@firstpwd", currentPwd);
                            if (firstOldPwd == null)
                            {
                                cmd.Parameters.AddWithValue("@secondpwd", DBNull.Value);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@secondpwd", firstOldPwd);
                            }
                            cmd.Parameters.AddWithValue("@createdAt", DateTime.Now);
                            cmd.Parameters.AddWithValue("@email", email);

                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected string getDBValue(string email, string col)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select " + col + " FROM ACCOUNT WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader[col] != null)
                        {
                            if (reader[col] != DBNull.Value)
                            {
                                s = reader[col].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }

        private Boolean validation()
        {
            var Pwd = tb_current_pwd.Text.Trim();
            var newPwd = tb_new_pwd.Text.Trim();
            var confirmNewPwd = tb_confirm_new_pwd.Text.Trim();

            var error = "";

            if (Pwd == "")
            {
                error += "Password cannot be empty";
                lb_current_pwd_error.Text = "Password cannot be empty";
            }
            else if (finalHash != currentPwd)
            {
                error += "Password is not correct<br/>";
                lb_current_pwd_error.Text = "Password is not correct";
            }

            if (newPwd == "")
            {
                error += "Password cannot be empty";
                lb_new_pwd_error.Text = "Password cannot be empty";
            }
            else if (checkPassword(newPwd) < 5)
            {
                error += "Password is not strong enough<br/>";
                lb_new_pwd_error.Text = "Password is not strong enough";
            }

            if (confirmNewPwd == "")
            {
                error += "Password cannot be empty<br/>";
                lb_confirm_new_pwd_error.Text = "Password cannot be empty";
            }
            else if (confirmNewPwd != newPwd)
            {
                error += "New password and confirm password is not equal<br/>";
                lb_confirm_new_pwd_error.Text = "New password and confirm password is not equal";
            }


            if (error != "")
            {
                //tb_error.Text = error;
                return false;
            }
            return true;

        }

        private int checkPassword(string password)
        {
            int score = 0;

            if (password.Length > 12)
            {
                score += 1;
            }

            if (Regex.IsMatch(password, "[a-z]"))
            {
                score += 1;
            }

            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score += 1;
            }

            if (Regex.IsMatch(password, "[0-9]"))
            {
                score += 1;
            }

            if (Regex.IsMatch(password, "[^A-Z0-9a-z]"))
            {
                score += 1;
            }

            return score;
        }
    }
}