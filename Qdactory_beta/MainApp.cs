using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;


namespace Qdactory_beta
{
    public class MainApp
    {
        //const string connetionString = "Data Source=209.124.89.43;Initial Catalog=qayimdac_qd;User ID=qayimdac_qdactory; Password=N7hyp^X11H";
        const string connetionString = "Data Source=209.124.89.43;Initial Catalog=fanarpsk_sandbox;User ID=devTeam; Password=qG9mr#82";
        const string baseUrl = "http://qdactorymvc-001-site1.itempurl.com";


        public bool CheckIfAllowedToSignUpAsDoctor(string UserId)
        {
            var allowed = false;
            using (SqlConnection conn = new SqlConnection(connetionString))
            {
                string query = "select * from DOC_UPD_M WHERE USER_ID_UPD='{{userid}}' AND TYPE_OF_REQUEST in ('CLAIM','NEW')";
                query = query.Replace("{{userid}}", UserId);

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {

                    allowed = false;
                }
                else
                    allowed = true;
            }
            return allowed;
        }
        public bool IsNewPhone(string phone)
        {
            var isnew = false;
            using (SqlConnection conn = new SqlConnection(connetionString))
            {
                string query = "SELECT USER_ID FROM USERS WHERE phone_number='{{phone}}'";
                query = query.Replace("{{phone}}", phone);

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    
                        isnew = false;
                }
                else
                    isnew = true;
            }
            if (isnew)
            {
                using (SqlConnection conn = new SqlConnection(connetionString))
                {
                    string query = "INSERT INTO PRE_USERS(MOBILE,SMS_CODE) VALUES('{{mobile}}','0000');";
                    query = query.Replace("{{mobile}}", phone);
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return isnew;
        }
        public bool IsEmailNew(string email, string token)
        {
            var isnew = false;
            using (SqlConnection conn = new SqlConnection(connetionString))
            {
                string query = "SELECT EMAIL FROM USERS WHERE EMAIL='{{email}}'";
                query = query.Replace("{{email}}", new DataEncrypt().Encrypt(email.ToLower().Trim()));

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        if (dr["EMAIL"].ToString() == token)
                            isnew = true;
                        else
                            isnew = false;
                        break;
                    }

                }
                else
                    isnew = true;
            }
            if (isnew)
            {

                using (SqlConnection conn = new SqlConnection(connetionString))
                {
                    string query = "UPDATE USERS SET EMAIL='{{email}}' WHERE EMAIL='{{token}}'";
                    query = query.Replace("{{email}}", new DataEncrypt().Encrypt(email.ToLower().Trim()));
                    query = query.Replace("{{token}}", token);
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return isnew;
        }
        public Response UpdatePatPhone(string PHONE_NUMBER, string SmsCode, string Token)
        {
            if(IsNewPhone(PHONE_NUMBER))
            {
                var isnew = false;
                using (SqlConnection conn = new SqlConnection(connetionString))
                {
                    string query = "SELECT * FROM PRE_USERS WHERE MOBILE='{{phone}}' AND Sms_code='{{smscode}}'";
                    query = query.Replace("{{phone}}", PHONE_NUMBER);
                    query = query.Replace("{{smscode}}", SmsCode);

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {

                        isnew = true;
                    }
                    else
                        isnew = false;
                }
                if (isnew)
                {
                    using (SqlConnection conn = new SqlConnection(connetionString))
                    {
                        string query = "UPDATE USERS SET PHONE_NUMBER='{{mobile}}' WHERE EMAIL='{{token}}'";
                        query = query.Replace("{{mobile}}", PHONE_NUMBER);
                        query = query.Replace("{{token}}", Token);
                        SqlCommand cmd = new SqlCommand(query, conn);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                if (isnew)
                {
                    return new Response
                    {
                        Code = "000.000.000",
                        Message = "UPDATED",
                        Data = null
                    };
                }
                else
                {
                    return new Response
                    {
                        Code = "100.000.000",
                        Message = "UPDATED",
                        Data = null
                    };
                 }
            }
            else
                return new Response
                {
                    Code = "100.000.000",
                    Message = "ALREADY_EXIST",
                    Data = null
                };
        }
        public Response UpdatePatProfile(PaTProfile info, string token)
        {
           
            using (SqlConnection conn = new SqlConnection(connetionString))
            {

                string query = "";
                if(info.PASSWORD!=null && info.F_NAME!=null && info.L_NAME!=null)
                {
                    if(info.PASSWORD != "" && info.F_NAME != "" && info.L_NAME != "")
                    {
                        query = "UPDATE USERS SET PASSWORD='{{password}}' WHERE EMAIL='{{token}}'";
                        query = query.Replace("{{password}}", new DataEncrypt().HardEncrypt(info.PASSWORD));
                        query = query.Replace("{{token}}", token);
                        query += "; UPDATE PAT_M SET F_NAME=N'{{F_NAME}}', L_NAME=N'{{L_NAME}}', GENDER='{{GENDER}}', DOB='{{DOB}}' WHERE PAT_ID in (SELECT TOP 1 M_ID FROM USERS WHERE EMAIL='{{token}}' AND TYPE=1) ";
                        query = query.Replace("{{token}}", token);
                        query = query.Replace("{{F_NAME}}", info.F_NAME);
                        query = query.Replace("{{L_NAME}}",info.L_NAME);
                        query = query.Replace("{{GENDER}}", info.GENDER);
                        query = query.Replace("{{DOB}}", info.DOB);
                    }
                }
                if (query != "")
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    bool isNew = false;
                    bool updatePhone = false;
                    bool IsUpdatingEmail = false;
                    bool IsUpdatingPhone = false;
                    if (info.EMAIL!=null && info.EMAIL!="")
                    {
                        IsUpdatingEmail = true;
                        isNew =IsEmailNew(info.EMAIL, info.Token);
                    }
                    if(info.PHONE_NUMBER!=null && info.PHONE_NUMBER!="")
                    {
                        updatePhone=IsNewPhone(info.PHONE_NUMBER);
                        IsUpdatingPhone = true;
                    }
                    return new Response
                    {
                        Code = "000.000.000",
                        Message = "UPDATED",
                        Data = new PatUpdateFeedBack
                        {
                            Updating_email = IsUpdatingEmail,
                            Updateing_phone = IsUpdatingPhone,
                            UpdateEmailSuccess = isNew,
                            UpdatePhoneSuccess = updatePhone
                        }
                    };
                }
                else
                {
                    return new Response
                    {
                        Code = "111.111.111",
                        Message = "UPDATED",
                        Data = null
                    };
                }

            }
        }
        public UserInfo GetPatProfile(UserInfo pat)
        {
            using (SqlConnection conn = new SqlConnection(connetionString))
            {


                SqlCommand cmd = new SqlCommand("select F_NAME,L_NAME,GENDER,isnull(DOB,null) DOB from PAT_M  where PAT_ID="+pat.M_ID, conn);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    
                    while (dr.Read())
                    {
                        pat.Names = new string[]
                        {
                            dr["F_NAME"].ToString(),dr["L_NAME"].ToString()
                        };

                        pat.Gender = dr["GENDER"].ToString();
                        pat.DOB = dr["DOB"].ToString();
                    }
                }
            }

            return pat;
        }  
        public List<UserInfo> GetUsers(Options opt, int offset, int rows_num)
        {

            string query = "";
            if (opt.Listing_type == "using_user_id")
            {
                query = @"SELECT USER_ID, EMAIL, PHONE_NUMBER,M_ID, TYPE FROM USERS WHERE USER_ID='"+opt.UserId+"'";
            }
            List<UserInfo> list = new List<UserInfo>();
            
            try
            {
                if (query != "")
                {
                    using (SqlConnection conn = new SqlConnection(connetionString))
                    {


                        SqlCommand cmd = new SqlCommand(query, conn);
                        conn.Open();
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            string dept = opt.Department;
                            while (dr.Read())
                            {
                                string dec_email;
                                try
                                {
                                    dec_email = new DataEncrypt().Decrypt(dr["EMAIL"].ToString());
                                }
                                catch(Exception ex)
                                {
                                    dec_email = "N/A";
                                }
                                if (int.Parse(dr["TYPE"].ToString()) == 1)
                                {
                                    list.Add(
                                        new UserInfo
                                        {
                                            Email = dec_email,
                                            Id = int.Parse(dr["USER_ID"].ToString()),
                                            Mobil = dr["PHONE_NUMBER"].ToString(),
                                            M_ID = int.Parse(dr["M_ID"].ToString()),
                                            Type = dr["TYPE"].ToString()

                                        });
                                    list[0] = GetPatProfile(list[0]);
                                }
                                
                            }
                        }
                        else
                        {
                        }
                        dr.Close();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {

            }

           
             return list;

        }
        public string GetUserEmail(int id)
        {
            string email = "numay.alghalib@gmail.com";
            DataEncrypt encrypt = new DataEncrypt();
            string query = "SELECT EMAIL FROM USERS WHERE USER_ID='{{id}}'";
            query = query.Replace("{{id}}", id.ToString());

            try
            {
                using (SqlConnection conn = new SqlConnection(connetionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            email = encrypt.Decrypt(dr["EMAIL"].ToString());
                        }
                    }
                    else
                    {
                    }
                    dr.Close();
                    conn.Close();
                }

            }
            catch (Exception ex)
            {

            }
            return email;
        }
        public UserInfo GetUserId(string  token)
        {
            
            try
            {
                string query = "SELECT USER_ID, TYPE FROM USERS WHERE EMAIL='"+token+"'";
                if (query != "")
                {
                    using (SqlConnection conn = new SqlConnection(connetionString))
                    {


                        SqlCommand cmd = new SqlCommand(query, conn);
                        conn.Open();
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                           
                            if (dr.Read())
                            {
                                return new UserInfo
                                {
                                    Id = int.Parse(dr["USER_ID"].ToString()),
                                    Type = dr["TYPE"].ToString()
                                };
                            }
                        }
                        else
                        {
                        }
                        dr.Close();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public void ReportComment(int rev_id,int user_id,string subject, string details)
        {
            var comment_author = GetUserIDFromComment(rev_id);
      
            new NotificationEndPoint().NotifyUser("report_comment", null, comment_author,null);

        }
        public void LikeComment(int rev_id)
        {
            var comment_author = GetUserIDFromComment(rev_id);
            new NotificationEndPoint().NotifyUser("user_like_comment", null, comment_author,null);
        }
        
        public int GetUserIDFromComment(int rev_id)
        {
            string query = @"select TOP 1 USER_ID from DOC_REV_M where DOC_REV_ID='{{REV_ID}}'";
            query = query.Replace("{{REV_ID}}", rev_id.ToString());

            try
            {
                using (SqlConnection conn = new SqlConnection(connetionString))
                {


                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {

                        if (dr.Read())
                        {
                            return int.Parse(dr["USER_ID"].ToString());
                        }
                    }
                    else
                    {
                    }
                    dr.Close();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {

            }
            return -1;

        }
        public Comment GetUserComment(int DOC_ID, int USER_ID)
        {
            string query = @"select TOP 1 DOC_REV_ID,DOC_ID,PUNC,KNOWLEDGE, RECEPTION,STAFF,POLITE,RATE_AVG,TIMESTAMP,TEXT,INST_ID from DOC_REV_M where user_id='{{USER_ID}}' AND DOC_ID='{{DOC_ID}}' ORDER BY TIMESTAMP DESC";
            query = query.Replace("{{DOC_ID}}", DOC_ID.ToString());
            query = query.Replace("{{USER_ID}}", USER_ID.ToString());

            try
            {
                using (SqlConnection conn = new SqlConnection(connetionString))
                {


                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {

                        if (dr.Read())
                        {
                            return new Comment
                            {
                                Id = int.Parse(dr["DOC_REV_ID"].ToString()),
                                Punc = int.Parse(dr["PUNC"].ToString()),
                                Pol=int.Parse(dr["POLITE"].ToString()),
                                Know = int.Parse(dr["KNOWLEDGE"].ToString()),
                                Rrec = int.Parse(dr["RECEPTION"].ToString()),
                                Stuf = int.Parse(dr["STAFF"].ToString()),
                                Text = dr["TEXT"].ToString(),
                                DocId=int.Parse(dr["DOC_ID"].ToString()),
                                Instid = int.Parse(dr["INST_ID"].ToString())
                            };
                        }
                    }
                    else
                    {
                    }
                    dr.Close();
                    conn.Close();
                }
            }
            catch(Exception ex)
            {

            }

            return new Comment
            {
                Punc = 0,
                Know = 0,
                Rrec = 0,
                Stuf = 0,
                Text = ""
            };

        }
        public Response Login(string email,string mobile, string password)
        {
            var enc = new DataEncrypt();
            string _code="111.111.111";
            string _msg="Invalid_LOGIN_INFO";
            string _email="";
            string _token="";
            string _type="";
            int _id = 0;

            if(email!=null && email!="")
            {
                email = enc.Encrypt(email.ToLower().Trim());
                /* use email with password */
                var query = "SELECT USER_ID,Email,Type,PASSWORD FROM USERS WHERE Email='{{email}}'";
                query = query.Replace("{{email}}", email);
                try
                {
                    using (SqlConnection conn = new SqlConnection(connetionString))
                    {


                        SqlCommand cmd = new SqlCommand(query, conn);
                        conn.Open();
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {

                            if (dr.Read())
                            {
                                
                                var pass_user = password;
                                var data_pass = dr["PASSWORD"].ToString();
                                if (enc.CheckHardEncrypt(data_pass, pass_user))
                                {
                                    _email = dr["Email"].ToString();
                                    _id = int.Parse(dr["USER_ID"].ToString());
                                    _type = dr["Type"].ToString();
                                    _msg = "ACCOUNT_EXISTS";
                                    _code = "000.000.000";


                                }
                                else
                                {
                                    return new Response
                                    {
                                        Code = _code,
                                        Message = _msg,
                                        Data = null
                                    };
                                }


                            }
                        }
                        else
                        {
                        }
                        dr.Close();
                        conn.Close();
                    }
                }
                catch (Exception ex)
                {

                }
            }
            else
            if(mobile!=null && mobile!="")
            {
                /* user mobile and password */
                var query = "SELECT USER_ID,Email,Type,PASSWORD FROM USERS WHERE PHONE_NUMBER='{{mobile}}'";
                query = query.Replace("{{mobile}}", mobile);
                try
                {
                    using (SqlConnection conn = new SqlConnection(connetionString))
                    {


                        SqlCommand cmd = new SqlCommand(query, conn);
                        conn.Open();
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                           
                            if (dr.Read())
                            {
                                
                                var pass_user = password;
                                var data_pass = dr["PASSWORD"].ToString();
                                if (enc.CheckHardEncrypt(data_pass, pass_user))
                                {
                                    _email = dr["Email"].ToString();
                                    _id = int.Parse(dr["USER_ID"].ToString());
                                    _type = dr["Type"].ToString();
                                    _msg = "ACCOUNT_EXISTS";
                                    _code = "000.000.000";


                                }
                                else
                                {
                                    return new Response
                                    {
                                        Code = _code,
                                        Message = _msg,
                                        Data = null
                                    };
                                }
                                   

                            }
                        }
                        else
                        {
                        }
                        dr.Close();
                        conn.Close();
                    }
                }
                catch (Exception ex)
                {

                }
            }

          
            var usr = new UserInfo
            {
                Id = _id,
                Token = _token,
                Email = _email,
                Type=_type
               
            };

            return new Response
            {
                Code = _code,
                Message = _msg,
                Data = usr
            };
        }
        public void Add_user(string email, string password, string phone, string gender, string dob)
        {
            using (SqlConnection conn = new SqlConnection(connetionString))
            {
                string query = "NEW_PAT_ACT_MVC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Email", email);
                cmd.Parameters.AddWithValue("Password", password);
                cmd.Parameters.AddWithValue("Phone", phone);
                //cmd.Parameters.AddWithValue("Gender", gender);
                //cmd.Parameters.AddWithValue("DOB", dob);
                cmd.Parameters.AddWithValue("UserId", DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            var code = new DataEncrypt().Encrypt(phone);
            var template = new UrlTemplate
            {
                Url = baseUrl + "/verify-code/" + code.Replace('+', '!')
            };
            
             new NotificationEndPoint().NotifyUser("new_user_verification", template, -1,new DataEncrypt().Decrypt(email));

        }

        public int GetUserIdFromPhone(string phone)
        {
            using (SqlConnection conn = new SqlConnection(connetionString))
            {
                string query = "SELECT USER_ID FROM USERS WHERE phone_number='{{code}}'";
                query = query.Replace("{{code}}", phone);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    return int.Parse(dr["USER_ID"].ToString());
                    
                }
            }

            return -1;
        }
        public Response VerifyEmail(string code)
        {
            using (SqlConnection conn = new SqlConnection(connetionString))
            {
                string query = "SELECT * FROM USERS WHERE phone_number='{{code}}'";
                query = query.Replace("{{code}}", code);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    UserVerifiedEmail(code);
                    return new Response
                    {
                        Code = "000.000.000",
                        Message = "EMAIL_VERIFIED",
                        Data = null
                    };
                }
                else
                {
                    return new Response
                    {
                        Code = "111.000.000",
                        Message = "EMAIL_NOT_VERIFIED",
                        Data = null
                    };
                }
            }
        }

        public void UserVerifiedEmail(string code)
        {

            using (SqlConnection conn = new SqlConnection(connetionString))
            {
                string query = "UPDATE USERS SET IS_VERIFIED='Y' WHERE phone_number='{{code}}'";
                query = query.Replace("{{code}}", code);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public Response SignUp(string email, string mobile, string password, string dob, string gender,string sms_code)
        {
            /* Check email is new */
                /* Check mobile is new */
                /* Check pre-user list wher sms_code and mobile exist if so approve  */
                var enc = new DataEncrypt();
            using (SqlConnection conn = new SqlConnection(connetionString))
            {
                string query = "SELECT * FROM USERS WHERE EMAIL='{{email}}' or PHONE_NUMBER='{{mobile}}'";
                var pure_email = email;
                email = enc.Encrypt(email.ToLower().Trim());
                query = query.Replace("{{email}}", email);
                query = query.Replace("{{mobile}}", "0" + mobile);

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    return new Response
                    {
                        Code = "111.111.111",
                        Message = "EMAIL_OR_PHONE_ALREADY_EXISTS",
                        Data = null
                    };
                }
            }

            using (SqlConnection conn = new SqlConnection(connetionString))
            {
                string query = "SELECT * FROM PRE_USERS WHERE MOBILE='{{mobile}}' AND Sms_code='{{sms_code}}'";
                query = query.Replace("{{mobile}}", "966"+mobile);
                query = query.Replace("{{sms_code}}", sms_code);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    Add_user(email, enc.HardEncrypt(password), "0"+mobile, gender, dob);
                    
                    return new Response
                    {
                        Code = "000.000.000",
                        Message = "ACCOUNT_CREATION_COMPLETED",
                        Data = null
                    };
                }
            }
            return new Response
            {
                Code = "100.000.000",
                Message = "INVALID_SMS_CODE",
                Data = null
            };
        }
        public Response SignUp(string email,string mobile, string password, string dob, string gender)
        {
            /* Check email is new */
            /* Check mobile is new */
            /* If all good, create pre-user with random code and send the code to the user's phone
             * and also send token number of pre-user account */
            var enc = new DataEncrypt();
            using (SqlConnection conn = new SqlConnection(connetionString))
            {
                string query = "SELECT * FROM USERS WHERE EMAIL='{{email}}' or PHONE_NUMBER='{{mobile}}'";

                email = enc.Encrypt(email.ToLower().Trim());
                query = query.Replace("{{email}}", email);
                query = query.Replace("{{mobile}}", "0" + mobile);

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    return new Response
                    {
                        Code = "111.111.111",
                        Message = "EMAIL_OR_PHONE_ALREADY_EXISTS",
                        Data = null
                    };
                }
            }
            using (SqlConnection conn = new SqlConnection(connetionString))
            {
                var to = "966567894760";
                if (mobile[0] == '0')
                    to = mobile.Substring(1);
                else
                    to = mobile;

                var code = GenerateRandomNo().ToString();
                string query = "INSERT INTO PRE_USERS (Mobile,Sms_code) values ('{{mobile}}','{{sms_code}}');";
                email = enc.Encrypt(email.ToLower().Trim());
                query = query.Replace("{{sms_code}}", code.ToString());
                query = query.Replace("{{mobile}}", "966" + mobile);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                new NotificationEndPoint().SendSms(code, "966"+mobile);
            }
           
            return new Response
            {
                Code = "000.000.000",
                Message = "pre_generated",
                Data = null
            };
        }
        public Response UpdatePhone(string email, string mobile, string password, string newphone)
        {

            /* Check email is new */

            /* Check mobile is new */

            /* If all good, create pre-user with random code and send the code to the user's phone
             * and also send token number of pre-user account */


            var usr = new UserInfo
            {
                Id = 10,
                Token = "erwrwerw",
                Email = "nmey1999@gmail.com",
                Names = new string[] { "numey alghalib", "نمي آل غالب" },

            };

            return new Response
            {
                Code = "000.000.000",
                Message = "logged in",
                Data = usr
            };
        }
        public Response ResetPassword(string email, string mobile, string password, string dob)
        {

            /* Check email is new */

            /* Check mobile is new */

            /* If all good, create pre-user with random code and send the code to the user's phone
             * and also send token number of pre-user account */


            var usr = new UserInfo
            {
                Id = 10,
                Token = "erwrwerw",
                Email = "nmey1999@gmail.com",
                Names = new string[] { "numey alghalib", "نمي آل غالب" },

            };

            return new Response
            {
                Code = "000.000.000",
                Message = "logged in",
                Data = usr
            };
        }
        public Response CheckToken(string token)
        {

            string _code = "111.111.111";
            string _msg = "Invalid_TOKEN_OR_EXPIRED";
            string _type = "0";
            string _email = "";
            int _id = 0;

            var query = "SELECT USER_ID, TYPE,Email FROM USERS WHERE Email='{{token}}'";
            query = query.Replace("{{token}}", token);
            try
            {
                using (SqlConnection conn = new SqlConnection(connetionString))
                {


                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {

                        while (dr.Read())
                        {
                            _code = "000.000.000";
                            _msg = "VALID_TOKEN";
                            _type = dr["Type"].ToString();
                            _id = int.Parse(dr["USER_ID"].ToString());
                            _email = dr["EMAIL"].ToString();
                            
                            break;

                        }
                    }
                    else
                    {
                    }
                    dr.Close();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {

            }

            var usr = new UserInfo
            {
                Id = _id,
                Email = _email,
                Type = _type

            };

            return new Response
            {
                Code = _code,
                Message = _msg,
                Data = usr
            };
        }
        public List<Department> GetDepartments(Options opt)
        {
            dynamic obj_departments = JsonConvert.DeserializeObject(opt.Departments);
            List<Department> list = new List<Department>();
            foreach(var json in obj_departments)
            {
                
                if (opt.Lang != "ar")
                {
                    
                    list.Add(new Department
                    {
                        Id = json.Id,
                        Names = new string[] { json.Names[0], json.Names[0] },
                        Icon = "https://qayimdactory.com/department/images/"+ ((string)json.Names[1]).Replace(' ','_')+".png",
                        Profile_url = opt.Lang + "/" + opt.Country + "-" + opt.City + "/"+json.Profile_url,
                        Tags = json.Tags
                    });
                }
                else
                {
                    list.Add(new Department
                    {
                        Id = json.Id,
                        Names = new string[] { json.Names[0], json.Names[0] },
                        Icon = "https://qayimdactory.com/department/images/" + ((string)json.Names[1]).Replace(' ', '_') + ".png",
                        Profile_url =  opt.Country + "-" + opt.City + "/"+json.Profile_url,
                        Tags = json.Tags
                    });
                }
            }
            return list;
        }
        public List<City> Set_cities(string lang, string obj)
        {
            List<City> cities = new List<City>();
            if (obj != null)
            {
                dynamic obj_cities = JsonConvert.DeserializeObject(obj);
                if (lang == "en")
                    foreach (var json in obj_cities)
                    {

                        cities.Add(new City { Names = new string[] { json.en, json.en } });
                    }

                if (lang == "ar")
                    foreach (var json in obj_cities)
                    {

                        cities.Add(new City { Names = new string[] { json.en, json.ar } });
                    }

            }
            return cities;
        }

        public int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }
    }
    public class PatUpdateFeedBack
    {
        public bool Updating_email { get; set; }
        public bool Updateing_phone { get; set; }
        public bool UpdateEmailSuccess { get; set; }
        public bool UpdatePhoneSuccess { get; set; }
    }

    
   
}