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
    public partial class Registration : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration
            .ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        string ImageName;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_register_Click(object sender, EventArgs e)
        {
            HashPwd();

            RijndaelManaged cipher = new RijndaelManaged();
            cipher.GenerateKey();
            Key = cipher.Key;
            IV = cipher.IV;

            if (validation() && photoUpload())
            {
                if (ValidateCaptcha())
                {
                    if (!(checkExists(tb_email.Text.Trim())))
                    {
                        createAccount();
                        Response.Redirect("Login.aspx", false);
                    }
                    else
                    {
                        error_summary.Text = "Email is already used";
                    }
                }
            }
        }

        protected Boolean photoUpload()
        {
            if (fu_photo.HasFile)
            {
                string extensions = System.IO.Path.GetExtension(fu_photo.FileName);

                if (extensions == ".jpg" || extensions == ".png")
                {

                    int fileSize = fu_photo.PostedFile.ContentLength / 1024;

                    if (fileSize > 3072)
                    {
                        lb_photo_error.Text = "Photo Uploaded size should be less than 3 mb only;";
                        return false;
                    }
                    else
                    {
                        string path = Server.MapPath("~/Images/");

                        ImageName = string.Concat(Path.GetFileName(DateTime.Now.ToString().Replace(':', '-') + fu_photo.FileName).Where(c => !Char.IsWhiteSpace(c)));
                        fu_photo.SaveAs(path + ImageName);

                        return true;
                    }



                }
                else
                {
                    lb_photo_error.Text = "Photo Uploaded is invalid. Only accept .jpg & .png";
                    return false;
                }
            }
            else
            {
                lb_photo_error.Text = "Photo is required to upload";
                return false;
            }
        }

        protected void HashPwd()
        {
            string pwd = tb_pwd.Text.ToString().Trim();

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] saltByte = new byte[8];

            rng.GetBytes(saltByte);
            salt = Convert.ToBase64String(saltByte);
            SHA512Managed hashing = new SHA512Managed();
            string pwdWithSalt = pwd + salt;
            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
            finalHash = Convert.ToBase64String(hashWithSalt);
        }

        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0,
               plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }

        protected Boolean checkExists(string email)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select FName FROM Account WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["FName"] != null)
                        {
                            if (reader["FName"] != DBNull.Value)
                            {
                                return true;
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
            return false;
        }

        protected void createAccount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@Id, @FName, @LName, @Email, @PasswordHash, @PasswordSalt, @DateTimeRegistered, @DateOfBirth, @Photo, @CardNumber, @CardName, @CardExpiryDate, @CardSecurityCode, @IV, @Key, @LoginAttempt, @IsLocked, @LockedAt, @FirstOldPassword, @SecondOldPassword, @PasswordCreatedAt)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                            cmd.Parameters.AddWithValue("@FName", HttpUtility.HtmlEncode(tb_fname.Text.Trim()));
                            cmd.Parameters.AddWithValue("@LName", HttpUtility.HtmlEncode(tb_lname.Text.Trim()));
                            //cmd.Parameters.AddWithValue("@Nric", encryptData(tb_nric.Text.Trim()));
                            cmd.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(tb_email.Text.Trim()));
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                            cmd.Parameters.AddWithValue("@DateTimeRegistered", DateTime.Now);
                            cmd.Parameters.AddWithValue("@DateOfBirth", HttpUtility.HtmlEncode(tb_dob.Text.Trim()));
                            if (ImageName == null)
                            {
                                cmd.Parameters.AddWithValue("@Photo", DBNull.Value);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@Photo", ImageName);
                            }
                            cmd.Parameters.AddWithValue("@CardNumber", Convert.ToBase64String(encryptData(tb_card_no.Text.Trim())));
                            cmd.Parameters.AddWithValue("@CardName", HttpUtility.HtmlEncode(tb_card_name.Text.Trim()));
                            cmd.Parameters.AddWithValue("@CardExpiryDate", Convert.ToBase64String(encryptData(tb_expiry_date.Text.Trim())));
                            cmd.Parameters.AddWithValue("@CardSecurityCode", Convert.ToBase64String(encryptData(tb_security_code.Text.Trim())));
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                            cmd.Parameters.AddWithValue("@LoginAttempt", 0);
                            cmd.Parameters.AddWithValue("@IsLocked", 0);
                            cmd.Parameters.AddWithValue("@LockedAt", DBNull.Value);
                            cmd.Parameters.AddWithValue("@FirstOldPassword", DBNull.Value);
                            cmd.Parameters.AddWithValue("@SecondOldPassword", DBNull.Value);
                            cmd.Parameters.AddWithValue("@PasswordCreatedAt", DateTime.Now);
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

        public Boolean ContainsOnlyAlphaNumericCharacters(string inputString)
        {
            var regexItem = new Regex(@"^[a-zA-Z0-9\s]*$");
            return regexItem.IsMatch(inputString);
        }

        private Boolean validation()
        {
            var fname = tb_fname.Text.Trim();
            var lname = tb_lname.Text.Trim();
            var email = tb_email.Text.Trim();
            var dob = tb_dob.Text.Trim();
            var pwd = tb_pwd.Text.Trim();
            var confirm = tb_confirm_pwd.Text.Trim();
            var cardNo = tb_card_no.Text.Trim();
            var cardName = tb_card_name.Text.Trim();
            var expiryDate = tb_expiry_date.Text.Trim();
            var securityCode = tb_security_code.Text.Trim();

            Regex emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            var dateRegex = new Regex(@"^[a-zA-Z0-9\s/-]*$");

            var error = "";

            if (fname == "")
            {
                error += "First name cannot be empty<br/>";
            }
            else if (!(ContainsOnlyAlphaNumericCharacters(fname)))
            {
                error += "First name cannot be contain special characters<br/>";
            }

            if (lname == "")
            {
                error += "Last name cannot be empty<br/>";
            }
            else if (!(ContainsOnlyAlphaNumericCharacters(lname)))
            {
                error += "Last name cannot be contain special characters<br/>";
            }

            if (email == "")
            {
                error += "Email cannot be empty<br/>";
            }
            else if (!(emailRegex.Match(email).Success))
            {
                error += "Email format is incorrect<br/>";
            }

            if (dob == "")
            {
                error += "Birth Date cannot be empty<br/>";
            }

            if (pwd == "")
            {
                error += "Password cannot be empty<br/>";
            }
            else if (checkPassword(pwd) < 5)
            {
                error += "Password is not strong enough<br/>";
            }

            if (confirm == "")
            {
                error += "Confirm password cannot be empty<br/>";
            }
            else if (pwd != confirm)
            {
                error += "Confirm password and password should be equal<br/>";
            }

            if (cardNo == "")
            {
                error += "Card Number cannot be empty<br/>";
            }
            else if (cardNo.Length != 16)
            {
                error += "Card Number needs to be 16 numbers<br/>";
            }
            else if (!(ContainsOnlyAlphaNumericCharacters(cardNo)))
            {
                error += "Card Number cannot be contain special characters<br/>";
            }

            if (cardName == "")
            {
                error += "Card name cannot be empty<br/>";
            }
            else if (!(ContainsOnlyAlphaNumericCharacters(cardName)))
            {
                error += "Card name cannot be contain special characters<br/>";
            }

            if (expiryDate == "")
            {
                error += "Expiry Date cannot be empty<br/>";
            }
            else if (!(dateRegex.Match(expiryDate).Success))
            {
                error += "Expiry Date cannot be contain special characters<br/>";
            }

            if (securityCode == "")
            {
                error += "Security Code cannot be empty<br/>";
            }
            else if (securityCode.Length != 3)
            {
                error += "Security Code needs to be 3 numbers<br/>";
            }
            else if (!(ContainsOnlyAlphaNumericCharacters(securityCode)))
            {
                error += "Security Code cannot be contain special characters<br/>";
            }

            if (error != "")
            {
                error_summary.Text = error;
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

        protected void btn_check_pwd_Click(object sender, EventArgs e)
        {
            var pwd = tb_pwd.Text;
            int scores = checkPassword(tb_pwd.Text);
            string status = "";
            switch (scores)
            {
                case 1:
                    status = "Very Weak";
                    break;
                case 2:
                    status = "Weak";
                    break;
                case 3:
                    status = "Medium";
                    break;
                case 4:
                    status = "Strong";
                    break;
                case 5:
                    status = "Very Strong";
                    break;
                default:
                    break;
            }
            lb_pwd_error.Text = "Status : " + status;
            if (scores < 5)
            {
                lb_pwd_error.ForeColor = Color.Red;
                return;
            }
            lb_pwd_error.ForeColor = Color.Green;
            tb_pwd.Text = pwd;
        }

        public bool ValidateCaptcha()
        {
            bool result = true;

            //When user submits the recaptcha form, the user gets a response POST parameter. 
            //captchaResponse consist of the user click pattern. Behaviour analytics! AI :) 
            string captchaResponse = Request.Form["g-recaptcha-response"];

            //To send a GET request to Google along with the response and Secret key.
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
           (" https://www.google.com/recaptcha/api/siteverify?secret=6LcPDEEdAAAAAMU_dIEY7zHI129zjR8-R4twCuhX &response=" + captchaResponse);


            try
            {

                //Codes to receive the Response in JSON format from Google Server
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        //The response in JSON format
                        string jsonResponse = readStream.ReadToEnd();

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        //Create jsonObject to handle the response e.g success or Error
                        //Deserialize Json
                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);

                        //Convert the string "False" to bool false or "True" to bool true
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


    }
}