using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Qdactory_beta
{
    public class InstituteEndPoint
    {
        //const string connetionString = "Data Source=209.124.89.43;Initial Catalog=qayimdac_qd;User ID=qayimdac_qdactory; Password=N7hyp^X11H";
        const string connetionString = "Data Source=209.124.89.43;Initial Catalog=fanarpsk_sandbox;User ID=devTeam; Password=qG9mr#82";

        public List<Offer> GetOffers(Options opt, int offset, int rows_num)
        {
            List<Offer> offers = new List<Offer>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connetionString))
                {
                    string query = @"select * from OFFERS_INSTS where INST_ID="+opt.InstId+ " AND getdate()>END_DATE order by END_DATE DESC offset {{offset}} rows fetch next {{rows_num}} rows only";
                    query = query.Replace("{{offset}}", offset.ToString());
                    query = query.Replace("{{rows_num}}", rows_num.ToString());

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            offers.Add(
                                new Offer
                                {
                                    ImageUrl = dr["OFFER_URL"].ToString(),
                                    Title = (opt.Lang == "en" ? dr["DISPLAY_TITLE_EN"].ToString() : dr["DISPLAY_TITLE_AR"].ToString()),
                                    Id = int.Parse(dr["OFFER_ID"].ToString())
                                }

                                ); 
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return offers;
        }
        public List<Institute> GetInstitutes(Options opt, int offset, int rows_num)
        {
            
            offset *= rows_num;

            List<Institute> list = new List<Institute>();
            string INST_IDz = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(connetionString))
                {
                    string query = @"select INST_ID,ar_NAME, en_NAME,PHONE_NUM,IMG,EMAIL,GPS_LAT,GPS_LON,INST_EMAIL,APP_STATUS,isnull(Inst_status,0) Inst_status from (
                    select ar.INST_ID_M, ar.NAME ar_NAME,en.NAME en_NAME from 
                    (select INST_ID_M,NAME FROM INST_D where LANG_ID=2) ar,
                    (select INST_ID_M,NAME FROM INST_D where LANG_ID=1) en
                    where ar.INST_ID_M=en.INST_ID_M) A LEFT JOIN INST_M on A.INST_ID_M = INST_ID where Inst_status=1
                    AND INST_M.CITY_ID_INST in  (SELECT CITY_ID_M FROM CITY_D WHERE text='@city') 
                    order by INST_ID ASC offset {{offset}} rows fetch next {{rows_num}} rows only";

                    if(opt.Listing_type!=null && opt.Listing_type=="inst_by_id")
                    {
                        query = @"select INST_ID,ar_NAME, en_NAME,PHONE_NUM,IMG,EMAIL,GPS_LAT,GPS_LON,INST_EMAIL,APP_STATUS,isnull(Inst_status,0) Inst_status from (
                        select ar.INST_ID_M, ar.NAME ar_NAME,en.NAME en_NAME from 
                        (select INST_ID_M,NAME FROM INST_D where LANG_ID=2) ar,
                        (select INST_ID_M,NAME FROM INST_D where LANG_ID=1) en
                        where ar.INST_ID_M=en.INST_ID_M) A LEFT JOIN INST_M on A.INST_ID_M = INST_ID where INST_ID={{instid}} 
                        order by INST_ID ASC offset {{offset}} rows fetch next {{rows_num}} rows only";
                        query = query.Replace("{{instid}}", opt.InstId.ToString());



                    }
                    else
                    if (opt.City==null || opt.City == "all")
                    {
                        query = @"select INST_ID,ar_NAME, en_NAME,PHONE_NUM,IMG,EMAIL,GPS_LAT,GPS_LON,INST_EMAIL,APP_STATUS,isnull(Inst_status,0) Inst_status from (
                        select ar.INST_ID_M, ar.NAME ar_NAME,en.NAME en_NAME from 
                        (select INST_ID_M,NAME FROM INST_D where LANG_ID=2) ar,
                        (select INST_ID_M,NAME FROM INST_D where LANG_ID=1) en
                        where ar.INST_ID_M=en.INST_ID_M) A LEFT JOIN INST_M on A.INST_ID_M = INST_ID where Inst_status=1
                        AND INST_M.CITY_ID_INST in  (SELECT CITY_ID_M FROM CITY_D WHERE text!='@city') 
                        order by INST_ID ASC offset {{offset}} rows fetch next {{rows_num}} rows only";
                        opt.City = "all";

                    }

                    if (opt.City!=null && opt.City!="")
                        query = query.Replace("@city", opt.City);
                    else
                        query = query.Replace("@city", "Riyadh");

                    
                    query = query.Replace("{{offset}}", offset.ToString());
                    query = query.Replace("{{rows_num}}", rows_num.ToString());

                   


                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            INST_IDz += "," + dr["INST_ID"].ToString();
                            var status = "Y";
                            if(dr["Inst_status"].ToString()!="1")
                            {
                                status = "N";
                            }
                          
                            if(opt.Lang!="ar")
                            {
                                list.Add(new Institute
                                {
                                    Id = int.Parse(dr["INST_ID"].ToString()),
                                    Names = new string[] { dr["en_NAME"].ToString(), dr["ar_NAME"].ToString() },
                                    Icon = "https://qayimdactory.com/profile_pics/insts_pics/" + dr["IMG"].ToString(),
                                    Rate = 0,
                                    Profile_url = "/" + opt.Lang + "/clinic/" + dr["INST_ID"].ToString() + "/" + dr["ar_NAME"].ToString().Replace(' ', '-'),
                                    Inst_verified = status

                                });
                            }
                            else
                            {
                                list.Add(new Institute
                                {
                                    Id = int.Parse(dr["INST_ID"].ToString()),
                                    Names = new string[] { dr["en_NAME"].ToString(), dr["ar_NAME"].ToString() },
                                    Icon = "https://qayimdactory.com/profile_pics/insts_pics/" + dr["IMG"].ToString(),
                                    Rate = 0,
                                    Profile_url = "/clinic/" + dr["INST_ID"].ToString() + "/" + dr["ar_NAME"].ToString().Replace(' ', '-'),
                                    Inst_verified = status

                                });
                            }
                            

                        }
                    }

                    dr.Close();
                }
            }
            catch(Exception ex)
            {

            }


            if (INST_IDz != "")
            {

                var final_list = PushRateAndTotal(list, INST_IDz.Substring(1));
                return final_list;
            }
            else
                return list;
           
        }
        public List<Institute> SerchInstitutes(Options opt, string name)
        {

            List<Institute> list = new List<Institute>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connetionString))
                {
                    string query = @"
                    SELECT INST_ID_M,NAME FROM INST_D WHERE INST_ID_M in (select INST_ID from INST_M where CITY_ID_INST in (select CITY_ID_M FROM CITY_D WHERE TEXT='{{city}}')
                    ) AND NAME like N'%{{name}}%'
                    ";

                    query = query.Replace("{{name}}", name);
                    query = query.Replace("{{city}}", opt.City);

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            list.Add(new Institute
                            {
                                Id = int.Parse(dr["INST_ID_M"].ToString()),
                                Names = new string[] { dr["NAME"].ToString(), dr["NAME"].ToString() },
                                Icon = "",
                                Rate = 0,
                                Profile_url = "/" + opt.Lang + "/clinic/" + dr["INST_ID_M"].ToString() + "/" + dr["NAME"].ToString().Replace(' ', '-'),

                            });
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
            return list;
        }
        public List<Institute> PushRateAndTotal(List<Institute> list, string inst_idz)
        {
            try
            {
                string query = "select inst_id,FORMAT((AVG(DOC_W_INST)+AVG(INST_W_INT)),'N1') Rate,count(DOC_REV_ID) as Total from doc_Rev_weight where INST_ID in(" + inst_idz + ") group by inst_id";

                using (SqlConnection conn = new SqlConnection(connetionString))
                {

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {

                        while (dr.Read())
                        {
                            list.Find(item => item.Id == int.Parse(dr["inst_id"].ToString())).Rate = float.Parse(dr["Rate"].ToString());
                            list.Find(item => item.Id == int.Parse(dr["inst_id"].ToString())).Total = int.Parse(dr["Total"].ToString());
                        }
                    }
                    else
                    {
                    }
                    dr.Close();
                }

            }
            catch (Exception ex)
            {

            }
            return list;
        }

        public string GetInstCity(int id, string lang)
        {
            int lang_id = 1;
            if (lang == "ar")
                lang_id = 2;

            string query = @"select text from city_d where city_Id_m in (select CITY_ID_INST  from INST_M where INST_ID=" + id + ") and LANG_ID=" + lang_id;

            string city = "any";
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
                            city = dr["text"].ToString();
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
            return city;
        }

    }
}