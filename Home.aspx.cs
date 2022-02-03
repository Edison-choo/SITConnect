using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SiTConnect
{
    public partial class Home : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        byte[] Key;
        byte[] IV;
        string fname;
        string lname;
        string photo;
        string dob;
        string cardName;
        byte[] cardNo;
        byte[] expiryDate;
        byte[] securityCode;

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

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
                    //if (Convert.ToBoolean(Session["OverMaxAge"].ToString()))
                    //{
                    //    lb_max_age.Text = "You need to change your password";
                    //}
                    getDBAllValue(Session["LoggedIn"].ToString());
                }
            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }
        }

        protected void btn_logout_Click(object sender, EventArgs e)
        {
            logger.Info("Logout success. User Account {Id} is logged out.", getDBValue(Session["LoggedIn"].ToString(), "Id"));

            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }

            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = string.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }

            Response.Redirect("Login.aspx", false);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("ChangePwd.aspx", false);
        }

        protected void getDBAllValue(string email)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select * FROM ACCOUNT WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["FName"] != DBNull.Value)
                        {
                            lb_fname.Text = reader["FName"].ToString();
                        }

                        if (reader["LName"] != DBNull.Value)
                        {
                            lb_lname.Text = reader["LName"].ToString();
                        }

                        if (reader["DateOfBirth"] != DBNull.Value)
                        {
                            lb_dob.Text = reader["DateOfBirth"].ToString();
                        }

                        if (reader["Photo"] != DBNull.Value)
                        {
                            photo = reader["Photo"].ToString();
                            image.ImageUrl = $@"Images/{photo}";
                        }

                        if (reader["CardNumber"] != DBNull.Value)
                        {
                            cardNo = Convert.FromBase64String(reader["CardNumber"].ToString());

                        }

                        if (reader["CardName"] != DBNull.Value)
                        {
                            cardName = reader["CardName"].ToString();
                        }

                        if (reader["CardExpiryDate"] != DBNull.Value)
                        {
                            expiryDate = Convert.FromBase64String(reader["CardExpiryDate"].ToString());

                        }


                        if (reader["CardSecurityCode"] != DBNull.Value)
                        {
                            securityCode = Convert.FromBase64String(reader["CardSecurityCode"].ToString());

                        }

                        if (reader["IV"] != DBNull.Value)
                        {
                            IV = Convert.FromBase64String(reader["IV"].ToString());
                        }


                        if (reader["Key"] != DBNull.Value)
                        {
                            Key = Convert.FromBase64String(reader["Key"].ToString());
                        }

                    }

                    lb_card_no.Text = decryptData(cardNo);
                    lb_expiry_date.Text = decryptData(expiryDate);
                    lb_security_code.Text = decryptData(securityCode);
                    lb_email.Text = email;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }

        }

        protected string decryptData(byte[] cipherText)
        {
            string plainText = null;

            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform decryptTransform = cipher.CreateDecryptor();

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }
            catch (Exception ex) { throw new Exception(ex.ToString()); }
            finally { }
            return plainText;
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
    }
}