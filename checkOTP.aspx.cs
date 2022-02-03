using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace SiTConnect
{
    public partial class checkOTP : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration
            .ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        private string OTPValue;
        private DateTime CreatedAt;

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_submit_Click(object sender, EventArgs e)
        {
            if (Validation())
            {
                var otpVal = tb_otp.Text.ToString().Trim();
                var email = Session["TwoFAEmail"].ToString();
                getOTPValue();

                if (otpVal == OTPValue)
                {
                    TimeSpan ts = DateTime.Now - CreatedAt;
                    if (ts.TotalMinutes > 1)
                    {
                        lb_error.Text = "OTP expired. Resending a new OTP...";
                        logger.Info("OTP expired. Resending a new OTP to {Id}...", Session["TwoFA"].ToString());
                        deleteOTP(Session["TwoFA"].ToString());
                        createOTP(email);
                        getOTPValue();
                        sendEmail(email);
                    }
                    else
                    {
                        deleteOTP(Session["TwoFA"].ToString());

                        logger.Info("Login success. User Account {Email} is logged in.", Session["TwoFA"].ToString());

                        Session.Remove("TwoFA");
                        Session.Remove("TwoFAEmail");

                        Session["LoggedIn"] = email;

                        Session["OverMaxAge"] = !checkMaxPwdAge(email);

                        string guid = Guid.NewGuid().ToString();
                        Session["AuthToken"] = guid;

                        Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                        

                        if (!checkMaxPwdAge(email))
                        {
                            Response.Redirect("ChangePwd.aspx", false);
                        }
                        else
                        {
                            Response.Redirect("Home.aspx", false);
                        }
                    }
                }
                else
                {
                    lb_error.Text = "OTP value is incorrect!";
                    logger.Info("Login failed. OTP value is incorrect for {Id}", Session["TwoFA"].ToString());
                }
            }
        }

        private bool Validation()
        {
            var otp = tb_otp.Text.Trim();

            var error = "";

            if (otp == "")
            {
                error += "OTP cannot be empty<br/>";
            }
            else if (otp.Length != 6)
            {
                error += "OTP value is 6 digit long<br/>";
            }
            else if (!(ContainsOnlyNumericCharacters(otp)))
            {
                error += "OTP can only contain numeric characters<br/>";
            }

            if (error != "")
            {
                lb_error.Text = error;
                return false;
            }
            return true;

        }

        public Boolean ContainsOnlyNumericCharacters(string inputString)
        {
            var regexItem = new Regex(@"^[0-9]*$");
            return regexItem.IsMatch(inputString);
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

        protected void deleteOTP(string id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM OTP WHERE UserId = @UserId"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@UserId", id);
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

        public void sendEmail(string email)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                MailAddress from = new MailAddress("hotteokki234@gmail.com");
                MailAddress to = new MailAddress(email);

                MailMessage message = new MailMessage(from, to);
                message.Body = $"OTP values are {OTPValue}";
                message.Body += Environment.NewLine;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.Subject = "OTP Values";
                message.SubjectEncoding = System.Text.Encoding.UTF8;

                client.Host = "smtp.gmail.com";
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
    }
}