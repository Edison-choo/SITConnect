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
using System.Net;
using System.Web.Script.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.ComponentModel;
using System.Configuration;

namespace SiTConnect
{
    public class MyObject
    {
        public string success { get; set; }
        public List<string> ErrorMessage { get; set; }
    }
    public partial class Login : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        private string errorMsg;
        private string OTPValue;
        private DateTime CreatedAt;

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_login_Click(object sender, EventArgs e)
        {
            string pwd = tb_pwd.Text.ToString().Trim();
            string email = tb_email.Text.ToString().Trim();
            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(email);
            string dbSalt = getDBSalt(email);
            try
            {
                if (ValidateCaptcha())
                {
                    if (Validation())
                    {
                        if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                        {

                            string pwdWithSalt = pwd + dbSalt;
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                            string userHash = Convert.ToBase64String(hashWithSalt);


                            if (Convert.ToInt32(getDBValue(email, "IsLocked")) == 1)
                            {
                                DateTime lockedAt = Convert.ToDateTime(getDBValue(email, "LOCKEDAT"));
                                TimeSpan ts = DateTime.Now - lockedAt;
                                if (ts.TotalMinutes > 5)
                                {
                                    lockAccount(email, 0);
                                    EditLoginAttempt(email, 1);
                                }
                                else
                                {
                                    lb_error.Text = "Your account is locked!";
                                    logger.Info("Login failed. User Account {Email} is locked", email);
                                    return;
                                }
                            }

                            if (userHash.Equals(dbHash))
                            {
                                EditLoginAttempt(email, 1);
                                Session["TwoFA"] = getDBValue(email, "Id");

                                Session["TwoFAEmail"] = email;

                                createOTP(email);
                                getOTPValue();
                                sendEmail(email);

                                logger.Info("Login details are authenticated. OTP values are sent to {Email}.", email);

                                Response.Redirect("checkOTP.aspx", false);

                                //Session["LoggedIn"] = email;

                                //Session["OverMaxAge"] = !(checkMaxPwdAge(email));

                                //string guid = Guid.NewGuid().ToString();
                                //Session["AuthToken"] = guid;

                                //Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                                //if (!(checkMaxPwdAge(email)))
                                //{
                                //    Response.Redirect("ChangePwd.aspx", false);
                                //}
                                //else
                                //{
                                //    Response.Redirect("Home.aspx", false);
                                //}
                            }
                            else
                            {
                                if (Convert.ToInt32(getDBValue(email, "LoginAttempt")) > 1)
                                {
                                    lockAccount(email, 1);
                                }
                                else
                                {
                                    EditLoginAttempt(email, 0);
                                    lb_error.Text = "Userid or password is not valid. Please try again.";
                                    logger.Info("Login failed. Incorrect login details for {email}", email);

                                }
                            }
                        }
                        else
                        {
                            lb_error.Text = "Userid or password is not valid. Please try again.";
                            logger.Info("Login failed. Incorrect login details for {email}", email);
                        }
                    }

                }
                else
                {
                    lb_error.Text = "Validate captcha to prove that you are a human.";
                    logger.Info("Login failed. Invalid captcha");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }

        }

        protected Boolean checkMaxPwdAge(string email)
        {
            DateTime createdAt = Convert.ToDateTime(getDBValue(email, "PasswordCreatedAt"));
            TimeSpan ts = DateTime.Now - createdAt;
            if (ts.TotalMinutes > 5)
            {
                return false;
            }

            return true;
        }

        private void lockAccount(string email, int isLocked)
        {

            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("update Account SET IsLocked = @isLocked, LockedAt = @lockedDate WHERE Email=@Email"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", email);
                            cmd.Parameters.AddWithValue("@IsLocked", isLocked);
                            cmd.Parameters.AddWithValue("@lockedDate", DateTime.Now);
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

        protected void EditLoginAttempt(string email, int choice)
        {
            string action = "";
            if (choice == 0)
            {
                action = "LoginAttempt + 1";
            }
            else if (choice == 1)
            {
                action = "0";
            }

            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("update Account SET LoginAttempt = " + action + " WHERE Email=@Email"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", email);
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

        protected string getDBHash(string email)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordHash FROM Account WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
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
            return h;
        }

        protected string getDBSalt(string email)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PASSWORDSALT FROM ACCOUNT WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PASSWORDSALT"] != null)
                        {
                            if (reader["PASSWORDSALT"] != DBNull.Value)
                            {
                                s = reader["PASSWORDSALT"].ToString();
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

        public bool ValidateCaptcha()
        {
            bool result = true;

            string captchaResponse = Request.Form["g-recaptcha-response"];

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
           (" https://www.google.com/recaptcha/api/siteverify?secret=6LcPDEEdAAAAAMU_dIEY7zHI129zjR8-R4twCuhX &response=" + captchaResponse);


            try
            {

                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);

                        result = Convert.ToBoolean(jsonObject.success);//

                    }
                }

                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        private bool Validation()
        {
            var email = tb_email.Text.Trim();
            var pwd = tb_pwd.Text.Trim();

            var error = "";

            if (pwd == "")
            {
                error += "Password cannot be empty";
                lb_pwd_error.Text = "Password cannot be empty";
            }

            if (email == "")
            {
                error += "Email cannot be empty";
                lb_email_error.Text = "Password cannot be empty";
            }
            else if (!Regex.IsMatch(email, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
            {
                error += "Email formet is invalid";
                lb_email_error.Text = "Email formet is invalid";
            }


            if (error != "")
            {
                //tb_error.Text = error;
                return false;
            }
            return true;

        }

        protected void createOTP(string email)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO OTP VALUES(@Id, @OTPValue, @CreatedAt, @UserId)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            Random rnd = new Random();
                            var randomNumber = (rnd.Next(100000, 999999)).ToString();
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                            cmd.Parameters.AddWithValue("@OTPValue", randomNumber);
                            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                            cmd.Parameters.AddWithValue("@UserId", getDBValue(email, "Id"));
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

        static bool mailSent = false;
        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            String token = (string)e.UserState;

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Send canceled.", token);
            }
            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            }
            else
            {
                Console.WriteLine("Message sent.");
            }
            mailSent = true;
        }
        public void sendEmail(string email)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                MailAddress from = new MailAddress("eldersonhelpdesk@gmail.com");
                MailAddress to = new MailAddress(email);

                MailMessage message = new MailMessage(from, to);
                message.Body = $"OTP values are {OTPValue}";
                message.Body += Environment.NewLine;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.Subject = "OTP Values";
                message.SubjectEncoding = System.Text.Encoding.UTF8;

                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["email"], ConfigurationManager.AppSettings["password"]);
                client.EnableSsl = true;
                client.Send(message);
                client.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }

        protected void getOTPValue()
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select * FROM OTP WHERE UserId=@UserId";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserId", Session["TwoFA"].ToString());
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["OTPValue"] != null)
                        {
                            if (reader["OTPValue"] != DBNull.Value)
                            {
                                OTPValue = reader["OTPValue"].ToString();
                            }
                        }

                        if (reader["CreatedAt"] != null)
                        {
                            if (reader["CreatedAt"] != DBNull.Value)
                            {
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"].ToString());
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
            return;
        }


    }
}