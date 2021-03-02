using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

using System.Data;
using System.Configuration;


namespace Qdactory_beta
{
    public class DoctorEndPoint
    {
        //const string connetionString = "Data Source=209.124.89.43;Initial Catalog=qayimdac_qd;User ID=qayimdac_qdactory; Password=N7hyp^X11H";
        const string connetionString = "Data Source=209.124.89.43;Initial Catalog=fanarpsk_sandbox;User ID=devTeam; Password=qG9mr#82";
        public List<DoctorCard> GetDoctorsEclude(Options opt, int offset, int rows_num, List<DoctorCard> others)
        {
            /* Listing_type = "promoted_doctors_in_department_in_a_city", */
            /* Listing_type ="promoted_doctors_all_departments_in_all_cities",*/
            /* Listing_type = "promoted_doctors_all_departments_in_given_city", */
            /* Listing_type = "doctors_in_given_department_and_city_of_doctor", */
            /* Listing_type = "doctor_with_id", */
            string other_DOC_IDz = "";
            foreach (var dc in others)
                other_DOC_IDz += "," + dc.Id;

            if (other_DOC_IDz == "")
                return new List<DoctorCard>();

            other_DOC_IDz = other_DOC_IDz.Substring(1);

            if (opt.Department != null)
            {
                opt.Department = opt.Department.Replace("_and_", " & ");
                opt.Department = opt.Department.Replace("_", " ");
            }

            offset *= rows_num;
            string query = "";
            if (opt.Listing_type == "all_doctors_in_department_in_a_city")
            {
                if (opt.City != null && opt.Department != null && opt.City != "all" && opt.Department != "all")
                {
                    /* LIST DOCTORS BASED ON CITY & DEPARTMENT */
                    query = @"select DOC_ID,A.ar_FNAME,A.ar_LNAME,A.en_FNAME, A.en_LNAME, DOC_IMG, GENDER, isnull(DOC_VERIFIED,'N') DOC_VERIFIED, isnull(A.ar_SUB_SPEC,' ') ar_SUB_SPEC, isnull(A.en_SUB_SPEC,' ') en_SUB_SPEC from (
                        SELECT ar.DOC_M_ID, ar.FNAME ar_FNAME, en.FNAME en_FNAME,ar.LNAME ar_LNAME, en.LNAME en_LNAME, ar.SUB_SPEC ar_SUB_SPEC, en.SUB_SPEC en_SUB_SPEC  FROM (
                        SELECT DOC_M_ID,FNAME,isnull(MNAME,'') MNAME,LNAME,SUB_SPEC FROM DOC_D WHERE LANG_ID=2) ar,
                        (SELECT DOC_M_ID, FNAME,isnull(MNAME,'') MNAME,LNAME,SUB_SPEC FROM DOC_D WHERE LANG_ID=1) en
                        WHERE ar.DOC_M_ID = en.DOC_M_ID AND ar.DOC_M_ID in (SELECT DOC_ID FROM DOC_M WHERE DOC_ID  not in (" + other_DOC_IDz + ") AND DEPT_ID_DOC in (SELECT DEPT_M_ID FROM DEPT_D WHERE TEXT='" + opt.Department + "' and LANG_ID='1') )) A LEFT JOIN DOC_M on A.DOC_M_ID=DOC_M.DOC_ID WHERE DOC_VERIFIED='Y' AND DOC_M.CITY_ID_DOC in (SELECT CITY_ID_M FROM CITY_D WHERE text='" + opt.City + "') order by DOC_ID ASC offset " + offset + " rows fetch next " + rows_num + " rows only";
                }
                else
                {
                    query = @"select DOC_ID,A.ar_FNAME,A.ar_LNAME,A.en_FNAME, A.en_LNAME, DOC_IMG, GENDER, isnull(DOC_VERIFIED,'N') DOC_VERIFIED, isnull(A.ar_SUB_SPEC,' ') ar_SUB_SPEC, isnull(A.en_SUB_SPEC,' ') en_SUB_SPEC from (
                        SELECT ar.DOC_M_ID, ar.FNAME ar_FNAME, en.FNAME en_FNAME,ar.LNAME ar_LNAME, en.LNAME en_LNAME, ar.SUB_SPEC ar_SUB_SPEC, en.SUB_SPEC en_SUB_SPEC  FROM (
                        SELECT DOC_M_ID,FNAME,isnull(MNAME,'') MNAME,LNAME,SUB_SPEC FROM DOC_D WHERE LANG_ID=2) ar,
                        (SELECT DOC_M_ID, FNAME,isnull(MNAME,'') MNAME,LNAME,SUB_SPEC FROM DOC_D WHERE LANG_ID=1) en
                        WHERE ar.DOC_M_ID = en.DOC_M_ID AND ar.DOC_M_ID in (SELECT DOC_ID FROM DOC_M WHERE DOC_ID  not in (" + other_DOC_IDz + ") AND DEPT_ID_DOC in  (SELECT DEPT_M_ID FROM DEPT_D WHERE TEXT='" + opt.Department + "' and LANG_ID='1') )) A LEFT JOIN DOC_M on A.DOC_M_ID=DOC_M.DOC_ID WHERE DOC_VERIFIED='Y' AND DOC_M.CITY_ID_DOC in (SELECT CITY_ID_M FROM CITY_D WHERE text!='" + opt.City + "') order by DOC_ID ASC offset " + offset + " rows fetch next " + rows_num + " rows only";
                }
            }

            List<DoctorCard> list = new List<DoctorCard>();
            string DOC_IDz = "";
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
                            int count_ = 0;
                            string dept = opt.Department;
                            while (dr.Read())
                            {
                                DOC_IDz += "," + dr["DOC_ID"].ToString();
                                if (opt.Listing_type != "promoted_doctors_in_department_in_a_city" && count_ == 0 && opt.Listing_type != "all_doctors_in_department_in_a_city")
                                {
                                    if (opt.Lang == "ar")
                                    {
                                        dept = GetDoctorDept(int.Parse(dr["DOC_ID"].ToString()))[0];
                                    }
                                    else
                                    {
                                        dept = GetDoctorDept(int.Parse(dr["DOC_ID"].ToString()))[1];
                                    }
                                }
                                else
                                {
                                    if (count_ == 0)
                                    {
                                        if (opt.Lang == "ar")
                                        {
                                            dept = GetDoctorDept(int.Parse(dr["DOC_ID"].ToString()))[0];
                                        }
                                        else
                                        {
                                            dept = GetDoctorDept(int.Parse(dr["DOC_ID"].ToString()))[1];
                                        }
                                    }
                                    count_ = 1;
                                }


                                if (opt.Lang != "ar")
                                {
                                    list.Add(new DoctorCard
                                    {
                                        Id = int.Parse(dr["DOC_ID"].ToString()),
                                        Names = new string[] { dr["en_FNAME"].ToString() + " " + dr["en_LNAME"].ToString(), dr["ar_FNAME"].ToString() + " " + dr["ar_LNAME"].ToString() },
                                        Icon = "https://qayimdactory.com" + "/profile_pics/" + dr["DOC_IMG"].ToString(), /*  opt.Domain  */
                                        Rate = 0,
                                        Total = 0,
                                        Profile_url = "/" + opt.Lang + "/doctor/" + dr["DOC_ID"].ToString() + "/" + (dr["ar_FNAME"].ToString() + " " + dr["ar_LNAME"].ToString()).Replace(' ', '-'),
                                        Dept_name = dept,
                                        Doc_verified = dr["DOC_VERIFIED"].ToString(),
                                        Inst_names = GetInstName(0, opt.Lang, dr["DOC_ID"].ToString()),
                                        SUB_SPEC = new string[] { dr["en_SUB_SPEC"].ToString(), dr["ar_SUB_SPEC"].ToString()

                                    }

                                    });
                                }
                                else
                                {
                                    list.Add(new DoctorCard
                                    {
                                        Id = int.Parse(dr["DOC_ID"].ToString()),
                                        Names = new string[] { dr["en_FNAME"].ToString() + " " + dr["en_LNAME"].ToString(), dr["ar_FNAME"].ToString() + " " + dr["ar_LNAME"].ToString() },
                                        Icon = "https://qayimdactory.com" + "/profile_pics/" + dr["DOC_IMG"].ToString(), /*  opt.Domain  */
                                        Rate = 0,
                                        Total = 0,
                                        Profile_url = "/doctor/" + dr["DOC_ID"].ToString() + "/" + (dr["ar_FNAME"].ToString() + " " + dr["ar_LNAME"].ToString()).Replace(' ', '-'),
                                        Dept_name = dept,
                                        Doc_verified = dr["DOC_VERIFIED"].ToString(),
                                        Inst_names = GetInstName(0, opt.Lang, dr["DOC_ID"].ToString()),

                                        SUB_SPEC = new string[] { dr["en_SUB_SPEC"].ToString(), dr["ar_SUB_SPEC"].ToString()



                                        }


                                    });
                                }
                            }
                        }
                        else
                        {
                        }
                        dr.Close();

                    }
                }
            }
            catch (Exception ex)
            {

            }

            if (DOC_IDz != "")
            {

                var final_list = PushRateAndTotal(list, DOC_IDz.Substring(1));
                return final_list;
            }
            else
                return list;
        }
        public List<DoctorCard> GetDoctors(Options opt, int offset, int rows_num)
        {

            /* Listing_type = "promoted_doctors_in_department_in_a_city", */
            /* Listing_type ="promoted_doctors_all_departments_in_all_cities",*/
            /* Listing_type = "promoted_doctors_all_departments_in_given_city", */
            /* Listing_type = "doctors_in_given_department_and_city_of_doctor", */
            /* Listing_type = "doctor_with_id", */

            if (opt.Department != null)
            {
                opt.Department = opt.Department.Replace("_and_", " & ");
                opt.Department = opt.Department.Replace("_", " ");
            }

            offset *= rows_num;
            string query = "";
            if (opt.Listing_type == "doctors_rated_by_user_id")
            {
                query = @"select DOC_ID,A.ar_FNAME,A.ar_LNAME,A.en_FNAME, A.en_LNAME, DOC_IMG, GENDER, isnull(DOC_VERIFIED,'N') DOC_VERIFIED, isnull(A.ar_SUB_SPEC,' ') ar_SUB_SPEC, isnull(A.en_SUB_SPEC,' ') en_SUB_SPEC from (
                SELECT ar.DOC_M_ID, ar.FNAME ar_FNAME, en.FNAME en_FNAME,ar.LNAME ar_LNAME, en.LNAME en_LNAME,ar.SUB_SPEC ar_SUB_SPEC, en.SUB_SPEC en_SUB_SPEC  FROM (
                SELECT DOC_M_ID,FNAME,isnull(MNAME,'') MNAME,LNAME,SUB_SPEC FROM DOC_D WHERE LANG_ID=2) ar,
                (SELECT DOC_M_ID, FNAME,isnull(MNAME,'') MNAME,LNAME,SUB_SPEC FROM DOC_D WHERE LANG_ID=1) en
                WHERE ar.DOC_M_ID = en.DOC_M_ID) A LEFT JOIN DOC_M on A.DOC_M_ID=DOC_M.DOC_ID WHERE DOC_ID in (SELECT DOC_ID FROM DOC_REV_M WHERE USER_ID='" + opt.UserId + "') order by DOC_ID ASC offset " + offset + " rows fetch next " + rows_num + " rows only";

            }
            else
            if (opt.Listing_type == "doctor_with_id")
            {
                query = @"select DOC_ID,isnull(MOB_NUM,'N') MOB_NUM,isnull(SHOW_MOB,'N') SHOW_MOB, A.ar_FNAME,A.ar_LNAME,A.en_FNAME, A.en_LNAME, DOC_IMG, GENDER, isnull(DOC_VERIFIED,'N') DOC_VERIFIED, isnull(A.ar_SUB_SPEC,' ') ar_SUB_SPEC, isnull(A.en_SUB_SPEC,' ') en_SUB_SPEC from (
                SELECT ar.DOC_M_ID, ar.FNAME ar_FNAME, en.FNAME en_FNAME,ar.LNAME ar_LNAME, en.LNAME en_LNAME,ar.SUB_SPEC ar_SUB_SPEC, en.SUB_SPEC en_SUB_SPEC  FROM (
                SELECT DOC_M_ID,FNAME,isnull(MNAME,'') MNAME,LNAME,SUB_SPEC FROM DOC_D WHERE LANG_ID=2) ar,
                (SELECT DOC_M_ID, FNAME,isnull(MNAME,'') MNAME,LNAME,SUB_SPEC FROM DOC_D WHERE LANG_ID=1) en
                WHERE ar.DOC_M_ID = en.DOC_M_ID) A LEFT JOIN DOC_M on A.DOC_M_ID=DOC_M.DOC_ID WHERE DOC_ID='" + opt.DocId + "'  order by DOC_ID ASC offset " + offset + " rows fetch next " + rows_num + " rows only";
            }
            else
                if (opt.Listing_type == "promoted_doctors_in_department_in_a_city")
            {
                if (opt.City != null && opt.Department != null && opt.City != "all" && opt.Department != "all")
                {
                    /* LIST DOCTORS BASED ON CITY & DEPARTMENT */
                    query = @"select DOC_ID,A.ar_FNAME,A.ar_LNAME,A.en_FNAME, A.en_LNAME, DOC_IMG, GENDER, isnull(DOC_VERIFIED,'N') DOC_VERIFIED, isnull(A.ar_SUB_SPEC,' ') ar_SUB_SPEC, isnull(A.en_SUB_SPEC,' ') en_SUB_SPEC from (
                        SELECT ar.DOC_M_ID, ar.FNAME ar_FNAME, en.FNAME en_FNAME,ar.LNAME ar_LNAME, en.LNAME en_LNAME, ar.SUB_SPEC ar_SUB_SPEC, en.SUB_SPEC en_SUB_SPEC  FROM (
                        SELECT DOC_M_ID,FNAME,isnull(MNAME,'') MNAME,LNAME,SUB_SPEC FROM DOC_D WHERE LANG_ID=2) ar,
                        (SELECT DOC_M_ID, FNAME,isnull(MNAME,'') MNAME,LNAME,SUB_SPEC FROM DOC_D WHERE LANG_ID=1) en
                        WHERE ar.DOC_M_ID = en.DOC_M_ID AND ar.DOC_M_ID in (SELECT DOC_ID FROM DOC_M WHERE DEPT_ID_DOC in (SELECT DEPT_M_ID FROM DEPT_D WHERE TEXT='" + opt.Department + "' and LANG_ID='1') )) A LEFT JOIN DOC_M on A.DOC_M_ID=DOC_M.DOC_ID WHERE DOC_VERIFIED='Y' AND DOC_M.CITY_ID_DOC in (SELECT CITY_ID_M FROM CITY_D WHERE text='" + opt.City + "') order by DOC_ID ASC offset " + offset + " rows fetch next " + rows_num + " rows only";
                }
                else
                {
                    query = @"select DOC_ID,A.ar_FNAME,A.ar_LNAME,A.en_FNAME, A.en_LNAME, DOC_IMG, GENDER, isnull(DOC_VERIFIED,'N') DOC_VERIFIED, isnull(A.ar_SUB_SPEC,' ') ar_SUB_SPEC, isnull(A.en_SUB_SPEC,' ') en_SUB_SPEC from (
                        SELECT ar.DOC_M_ID, ar.FNAME ar_FNAME, en.FNAME en_FNAME,ar.LNAME ar_LNAME, en.LNAME en_LNAME, ar.SUB_SPEC ar_SUB_SPEC, en.SUB_SPEC en_SUB_SPEC  FROM (
                        SELECT DOC_M_ID,FNAME,isnull(MNAME,'') MNAME,LNAME,SUB_SPEC FROM DOC_D WHERE LANG_ID=2) ar,
                        (SELECT DOC_M_ID, FNAME,isnull(MNAME,'') MNAME,LNAME,SUB_SPEC FROM DOC_D WHERE LANG_ID=1) en
                        WHERE ar.DOC_M_ID = en.DOC_M_ID AND ar.DOC_M_ID in (SELECT DOC_ID FROM DOC_M WHERE DEPT_ID_DOC in (SELECT DEPT_M_ID FROM DEPT_D WHERE TEXT='" + opt.Department + "' and LANG_ID='1') )) A LEFT JOIN DOC_M on A.DOC_M_ID=DOC_M.DOC_ID WHERE DOC_VERIFIED='Y' AND DOC_M.CITY_ID_DOC in (SELECT CITY_ID_M FROM CITY_D WHERE text!='" + opt.City + "') order by DOC_ID ASC offset " + offset + " rows fetch next " + rows_num + " rows only";
                }
            }
            else
            if (opt.Listing_type == "promoted_doctors_all_departments_in_all_cities")
            {
                query = @"select DOC_ID,A.ar_FNAME,A.ar_LNAME,A.en_FNAME, A.en_LNAME, DOC_IMG, GENDER, isnull(DOC_VERIFIED,'N') DOC_VERIFIED,isnull(A.ar_SUB_SPEC,' ') ar_SUB_SPEC, isnull(A.en_SUB_SPEC,' ') en_SUB_SPEC from (
                SELECT ar.DOC_M_ID, ar.FNAME ar_FNAME, en.FNAME en_FNAME,ar.LNAME ar_LNAME, en.LNAME en_LNAME,ar.SUB_SPEC ar_SUB_SPEC, en.SUB_SPEC en_SUB_SPEC  FROM (
                SELECT DOC_M_ID,FNAME,isnull(MNAME,'') MNAME,LNAME,SUB_SPEC FROM DOC_D WHERE LANG_ID=2) ar,
                (SELECT DOC_M_ID, FNAME,isnull(MNAME,'') MNAME,LNAME,SUB_SPEC FROM DOC_D WHERE LANG_ID=1) en
                WHERE ar.DOC_M_ID = en.DOC_M_ID) A LEFT JOIN DOC_M on A.DOC_M_ID=DOC_M.DOC_ID WHERE DOC_VERIFIED='Y' order by DOC_ID ASC offset " + offset + " rows fetch next " + rows_num + " rows only";
            }
            else
            if (opt.Listing_type == "promoted_doctors_all_departments_in_given_city")
            {
                /* LIST DOCTORS BASED ON CITY AND ANY DEPARTMENT */

                if (opt.City != null && (opt.Department == null || opt.Department == "all"))
                {
                    query = @"select DOC_ID,A.ar_FNAME,A.ar_LNAME,A.en_FNAME, A.en_LNAME, DOC_IMG, GENDER, isnull(DOC_VERIFIED,'N') DOC_VERIFIED, isnull(A.ar_SUB_SPEC,' ') ar_SUB_SPEC, isnull(A.en_SUB_SPEC,' ') en_SUB_SPEC from (
                        SELECT ar.DOC_M_ID, ar.FNAME ar_FNAME, en.FNAME en_FNAME,ar.LNAME ar_LNAME, en.LNAME en_LNAME, ar.SUB_SPEC ar_SUB_SPEC, en.SUB_SPEC en_SUB_SPEC  FROM (
                        SELECT DOC_M_ID,FNAME,isnull(MNAME,'') MNAME,LNAME,SUB_SPEC FROM DOC_D WHERE LANG_ID=2) ar,
                        (SELECT DOC_M_ID, FNAME,isnull(MNAME,'') MNAME,LNAME,SUB_SPEC FROM DOC_D WHERE LANG_ID=1) en
                        WHERE ar.DOC_M_ID = en.DOC_M_ID ) A LEFT JOIN DOC_M on A.DOC_M_ID=DOC_M.DOC_ID WHERE DOC_VERIFIED='Y' AND DOC_M.CITY_ID_DOC in (SELECT CITY_ID_M FROM CITY_D WHERE text='" + opt.City + "') order by DOC_ID ASC offset " + offset + " rows fetch next " + rows_num + " rows only";
                }
            }
            if (opt.Listing_type == "all_doctors_in_department_in_a_city")
            {

                if (opt.City != null && opt.Department != null && opt.City != "all" && opt.Department != "all")
                {
                    /* LIST DOCTORS BASED ON CITY & DEPARTMENT */
                    query = @"select DOC_ID,A.ar_FNAME,A.ar_LNAME,A.en_FNAME, A.en_LNAME, DOC_IMG, GENDER, isnull(DOC_VERIFIED,'N') DOC_VERIFIED, isnull(A.ar_SUB_SPEC,' ') ar_SUB_SPEC, isnull(A.en_SUB_SPEC,' ') en_SUB_SPEC from (
                        SELECT ar.DOC_M_ID, ar.FNAME ar_FNAME, en.FNAME en_FNAME,ar.LNAME ar_LNAME, en.LNAME en_LNAME , ar.SUB_SPEC ar_SUB_SPEC, en.SUB_SPEC en_SUB_SPEC FROM (
                        SELECT DOC_M_ID,FNAME,isnull(MNAME,'') MNAME,LNAME,SUB_SPEC FROM DOC_D WHERE LANG_ID=2) ar,
                        (SELECT DOC_M_ID, FNAME,isnull(MNAME,'') MNAME,LNAME,SUB_SPEC FROM DOC_D WHERE LANG_ID=1) en
                        WHERE ar.DOC_M_ID = en.DOC_M_ID AND ar.DOC_M_ID in (SELECT DOC_ID FROM DOC_M WHERE DEPT_ID_DOC in (SELECT DEPT_M_ID FROM DEPT_D WHERE TEXT='" + opt.Department + "' and LANG_ID='1') )) A LEFT JOIN DOC_M on A.DOC_M_ID=DOC_M.DOC_ID WHERE  DOC_M.CITY_ID_DOC in (SELECT CITY_ID_M FROM CITY_D WHERE text='" + opt.City + "') order by DOC_VERIFIED DESC offset " + offset + " rows fetch next " + rows_num + " rows only";

                }
                else
                {
                    query = @"select DOC_ID,A.ar_FNAME,A.ar_LNAME,A.en_FNAME, A.en_LNAME, DOC_IMG, GENDER, isnull(DOC_VERIFIED,'N') DOC_VERIFIED, isnull(A.ar_SUB_SPEC,' ') ar_SUB_SPEC, isnull(A.en_SUB_SPEC,' ') en_SUB_SPEC from (
                        SELECT ar.DOC_M_ID, ar.FNAME ar_FNAME, en.FNAME en_FNAME,ar.LNAME ar_LNAME, en.LNAME en_LNAME , ar.SUB_SPEC ar_SUB_SPEC, en.SUB_SPEC en_SUB_SPEC FROM (
                        SELECT DOC_M_ID,FNAME,isnull(MNAME,'') MNAME,LNAME,SUB_SPEC FROM DOC_D WHERE LANG_ID=2) ar,
                        (SELECT DOC_M_ID, FNAME,isnull(MNAME,'') MNAME,LNAME,SUB_SPEC FROM DOC_D WHERE LANG_ID=1) en
                        WHERE ar.DOC_M_ID = en.DOC_M_ID AND ar.DOC_M_ID in (SELECT DOC_ID FROM DOC_M WHERE DEPT_ID_DOC in (SELECT DEPT_M_ID FROM DEPT_D WHERE TEXT='" + opt.Department + "' and LANG_ID='1') )) A LEFT JOIN DOC_M on A.DOC_M_ID=DOC_M.DOC_ID WHERE  DOC_M.CITY_ID_DOC in (SELECT CITY_ID_M FROM CITY_D WHERE text!='" + opt.City + "') order by DOC_VERIFIED DESC offset " + offset + " rows fetch next " + rows_num + " rows only";

                }
            }
            else
            if (opt.Listing_type == "doctors_in_given_department_and_city_of_doctor")
            {
                if (opt.City != null && opt.Department != null && opt.City != "all" && opt.Department != "all")
                {
                    /* LIST DOCTORS BASED ON CITY & DEPARTMENT */
                    query = @"select DOC_ID,A.ar_FNAME,A.ar_LNAME,A.en_FNAME, A.en_LNAME, DOC_IMG, GENDER, isnull(DOC_VERIFIED,'N') DOC_VERIFIED, isnull(A.ar_SUB_SPEC,' ') ar_SUB_SPEC, isnull(A.en_SUB_SPEC,' ') en_SUB_SPEC from (
                        SELECT ar.DOC_M_ID, ar.FNAME ar_FNAME, en.FNAME en_FNAME,ar.LNAME ar_LNAME, en.LNAME en_LNAME, ar.SUB_SPEC ar_SUB_SPEC, en.SUB_SPEC en_SUB_SPEC  FROM (
                        SELECT DOC_M_ID,FNAME,isnull(MNAME,'') MNAME,LNAME,SUB_SPEC FROM DOC_D WHERE LANG_ID=2) ar,
                        (SELECT DOC_M_ID, FNAME,isnull(MNAME,'') MNAME,LNAME,SUB_SPEC FROM DOC_D WHERE LANG_ID=1) en
                        WHERE ar.DOC_M_ID = en.DOC_M_ID AND ar.DOC_M_ID in (SELECT DOC_ID FROM DOC_M WHERE DEPT_ID_DOC in (SELECT DEPT_M_ID FROM DEPT_D WHERE TEXT='" + opt.Department + "' and LANG_ID='1') )) A LEFT JOIN DOC_M on A.DOC_M_ID=DOC_M.DOC_ID WHERE DOC_VERIFIED='Y' AND DOC_M.CITY_ID_DOC in (SELECT CITY_ID_M FROM CITY_D WHERE text='" + opt.City + "') order by DOC_ID ASC offset " + offset + " rows fetch next " + rows_num + " rows only";
                }
            }
            else
            if (opt.Listing_type == "doctors_within_inst")
            {
                query = @"select DOC_ID,A.ar_FNAME,A.ar_LNAME,A.en_FNAME, A.en_LNAME, DOC_IMG, GENDER, isnull(DOC_VERIFIED,'N') DOC_VERIFIED,isnull(A.ar_SUB_SPEC,' ') ar_SUB_SPEC, isnull(A.en_SUB_SPEC,' ') en_SUB_SPEC from (
                SELECT ar.DOC_M_ID, ar.FNAME ar_FNAME, en.FNAME en_FNAME,ar.LNAME ar_LNAME, en.LNAME en_LNAME,ar.SUB_SPEC  ar_SUB_SPEC , en.SUB_SPEC en_SUB_SPEC FROM (
                SELECT DOC_M_ID,FNAME,isnull(MNAME,'') MNAME,LNAME,SUB_SPEC  FROM DOC_D WHERE LANG_ID=2) ar,
                (SELECT DOC_M_ID, FNAME,isnull(MNAME,'') MNAME,LNAME,SUB_SPEC FROM DOC_D WHERE LANG_ID=1) en
                WHERE ar.DOC_M_ID = en.DOC_M_ID) A LEFT JOIN DOC_M on A.DOC_M_ID=DOC_M.DOC_ID WHERE DOC_ID in (
                select DOC_ID from INST_DOC where INST_ID_M='" + opt.InstId + "') order by DOC_VERIFIED DESC offset " + offset + " rows fetch next " + rows_num + " rows only";
            }
            List<DoctorCard> list = new List<DoctorCard>();
            string DOC_IDz = "";
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
                            int count_ = 0;
                            string dept = opt.Department;
                            while (dr.Read())
                            {
                                DOC_IDz += "," + dr["DOC_ID"].ToString();
                                if (opt.Listing_type != "promoted_doctors_in_department_in_a_city" && count_ == 0 && opt.Listing_type != "all_doctors_in_department_in_a_city")
                                {
                                    if (opt.Lang == "ar")
                                    {
                                        dept = GetDoctorDept(int.Parse(dr["DOC_ID"].ToString()))[0];
                                    }
                                    else
                                    {
                                        dept = GetDoctorDept(int.Parse(dr["DOC_ID"].ToString()))[1];
                                    }
                                }
                                else
                                {
                                    if (count_ == 0)
                                    {
                                        if (opt.Lang == "ar")
                                        {
                                            dept = GetDoctorDept(int.Parse(dr["DOC_ID"].ToString()))[0];
                                        }
                                        else
                                        {
                                            dept = GetDoctorDept(int.Parse(dr["DOC_ID"].ToString()))[1];
                                        }
                                    }
                                    count_ = 1;
                                }


                                if (opt.Lang != "ar")
                                {
                                    list.Add(new DoctorCard
                                    {
                                        Id = int.Parse(dr["DOC_ID"].ToString()),
                                        Names = new string[] { dr["en_FNAME"].ToString() + " " + dr["en_LNAME"].ToString(), dr["ar_FNAME"].ToString() + " " + dr["ar_LNAME"].ToString() },
                                        Icon = "https://qayimdactory.com" + "/profile_pics/" + dr["DOC_IMG"].ToString(), /*  opt.Domain  */
                                        Rate = 0,
                                        Total = 0,
                                        Profile_url = "/" + opt.Lang + "/doctor/" + dr["DOC_ID"].ToString() + "/" + (dr["ar_FNAME"].ToString() + " " + dr["ar_LNAME"].ToString()).Replace(' ', '-'),
                                        Dept_name = dept,
                                        Doc_verified = dr["DOC_VERIFIED"].ToString(),
                                        ShowMob = (opt.Listing_type == "doctor_with_id" ? dr["SHOW_MOB"].ToString() : "N"),
                                        Call = (opt.Listing_type == "doctor_with_id" ? dr["MOB_NUM"].ToString() : "N"),
                                        Inst_names = GetInstName(0, opt.Lang, dr["DOC_ID"].ToString()),
                                        SUB_SPEC = new string[] { dr["en_SUB_SPEC"].ToString(), dr["ar_SUB_SPEC"].ToString()

                                    }

                                    });
                                }
                                else
                                {
                                    list.Add(new DoctorCard
                                    {
                                        Id = int.Parse(dr["DOC_ID"].ToString()),
                                        Names = new string[] { dr["en_FNAME"].ToString() + " " + dr["en_LNAME"].ToString(), dr["ar_FNAME"].ToString() + " " + dr["ar_LNAME"].ToString() },
                                        Icon = "https://qayimdactory.com" + "/profile_pics/" + dr["DOC_IMG"].ToString(), /*  opt.Domain  */
                                        Rate = 0,
                                        Total = 0,
                                        Profile_url = "/doctor/" + dr["DOC_ID"].ToString() + "/" + (dr["ar_FNAME"].ToString() + " " + dr["ar_LNAME"].ToString()).Replace(' ', '-'),
                                        Dept_name = dept,
                                        Doc_verified = dr["DOC_VERIFIED"].ToString(),
                                        Inst_names = GetInstName(0, opt.Lang, dr["DOC_ID"].ToString()),
                                        ShowMob = (opt.Listing_type == "doctor_with_id" ? dr["SHOW_MOB"].ToString() : "N"),
                                        Call = (opt.Listing_type == "doctor_with_id" ? dr["MOB_NUM"].ToString() : "N"),
                                        SUB_SPEC = new string[] { dr["en_SUB_SPEC"].ToString(), dr["ar_SUB_SPEC"].ToString()



                                        }


                                    });
                                }
                            }
                        }
                        else
                        {
                        }
                        dr.Close();

                    }
                }
            }
            catch (Exception ex)
            {

            }

            if (DOC_IDz != "")
            {

                var final_list = PushRateAndTotal(list, DOC_IDz.Substring(1));
                return final_list;
            }
            else
                return list;

        }
        public List<DoctorCard> PushRateAndTotal(List<DoctorCard> list, string doc_idz)
        {
            try
            {
                string query = "select doc_id,isnull(FORMAT(AVG(DOC_W_DOC)+AVG(INST_W_DOC),'N1'),0) as Rate,count(DOC_REV_ID) as Total from doc_Rev_weight where doc_id in(" + doc_idz + ") group by doc_id";

                using (SqlConnection conn = new SqlConnection(connetionString))
                {

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {

                        while (dr.Read())
                        {
                            list.Find(item => item.Id == int.Parse(dr["doc_id"].ToString())).Rate = (float)(Math.Round(float.Parse(dr["Rate"].ToString())));
                            list.Find(item => item.Id == int.Parse(dr["doc_id"].ToString())).Total = int.Parse(dr["Total"].ToString());
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
        public List<Post> GetDoctorRanks(int langId)
        {
            List<Post> ranks = new List<Post>();
            string query = "select RANK_ID_M as ID, TEXT as RANK from RANK_D WHERE LANG_ID=" + langId;
            using (SqlConnection conn = new SqlConnection(connetionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while(dr.Read())
                    {
                        ranks.Add(new Post
                        {
                            Id = int.Parse(dr["ID"].ToString()),
                            Post_name = dr["RANK"].ToString()
                        }); ;
                    }
                }
            }
            return ranks;
        }
        public List<Comment> GetComments(Options opt, int offset, int rows_num)
        {

            offset *= rows_num;
            var comments = new List<Comment>();
            string query = "";
            if (opt.Listing_type == "comments_by_city_and_dept")
            {
                query = @"SELECT  ACTIVE,TEXT,INST_ID,DOC_REV_ID,DOC_ID,USER_ID,PUNC,KNOWLEDGE,RECEPTION,STAFF,POLITE,TIMESTAMP FROM DOC_REV_M WHERE TEXT is not null and DOC_ID in " +
                 "(SELECT DOC_ID FROM DOC_M WHERE DEPT_ID_DOC in (SELECT DEPT_M_ID FROM DEPT_D WHERE TEXT='" + opt.Department + "' and LANG_ID='1')" +
                 "AND DOC_M.CITY_ID_DOC in (SELECT CITY_ID_M FROM CITY_D WHERE text='" + opt.City + "')" +
                 ") order by TIMESTAMP DESC  offset " + offset + " rows fetch next " + rows_num + " rows only";
            }
            else
            if (opt.Listing_type == "comments_by_doc_id")
            {
                query = @"SELECT  ACTIVE,TEXT,INST_ID,DOC_REV_ID,DOC_ID,USER_ID,PUNC,KNOWLEDGE,RECEPTION,STAFF,POLITE,TIMESTAMP FROM DOC_REV_M WHERE TEXT is not null AND DOC_ID='" + opt.DocId + "' order by TIMESTAMP DESC  offset " + offset + " rows fetch next " + rows_num + " rows only";
            }
            else
            if (opt.Listing_type == "comments_by_user_id")
            {
                query = @"SELECT  ACTIVE,TEXT,INST_ID,DOC_REV_ID,DOC_ID,USER_ID,PUNC,KNOWLEDGE,RECEPTION,STAFF,POLITE,TIMESTAMP FROM DOC_REV_M WHERE TEXT is not null AND USER_ID='" + opt.UserId + "' order by TIMESTAMP DESC  offset " + offset + " rows fetch next " + rows_num + " rows only";

            }

            if (query != "")
            {
                using (SqlConnection conn = new SqlConnection(connetionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        List<int> inst_idz = new List<int>();
                        string _inst_name = "";
                        int _inst_id = 0;
                        while (dr.Read())
                        {
                            if (inst_idz.Count == 0)
                            {
                                try
                                {
                                    var data = GetInstName(int.Parse(dr["INST_ID"].ToString()), opt.Lang, null)[0];
                                    _inst_name = data.Name;
                                    _inst_id = data.Id;
                                    inst_idz.Add(int.Parse(dr["INST_ID"].ToString()));
                                }
                                catch (Exception ex)
                                {
                                    if (opt.Lang == "ar")
                                        _inst_name = "استشارة عن بعد";
                                    else
                                        _inst_name = "remote consultation";

                                    _inst_id = int.Parse(dr["INST_ID"].ToString());
                                }
                            }
                            else
                            {
                                try
                                {
                                    if (!inst_idz.Contains(int.Parse(dr["INST_ID"].ToString())))
                                    {
                                        var data = GetInstName(int.Parse(dr["INST_ID"].ToString()), opt.Lang, null)[0];
                                        _inst_name = data.Name;
                                        _inst_id = data.Id;
                                        inst_idz.Add(int.Parse(dr["INST_ID"].ToString()));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (opt.Lang == "ar")
                                        _inst_name = "استشارة عن بعد";
                                    else
                                        _inst_name = "remote consultation";

                                    _inst_id = int.Parse(dr["INST_ID"].ToString());
                                }
                            }

                            if (opt.Listing_type == "comments_by_user_id" && opt.UserId > 0)
                            {
                                comments.Add(new Comment
                                {
                                    Instname = _inst_name,
                                    Instid = _inst_id,
                                    Punc = float.Parse(dr["PUNC"].ToString()),
                                    Know = float.Parse(dr["KNOWLEDGE"].ToString()),
                                    Rrec = float.Parse(dr["RECEPTION"].ToString()),
                                    Stuf = float.Parse(dr["STAFF"].ToString()),
                                    Pol = float.Parse(dr["POLITE"].ToString()),
                                    Text = dr["TEXT"].ToString(),
                                    UserId = dr["USER_ID"].ToString(),
                                    Date = dr["TIMESTAMP"].ToString(),
                                    Liked= IsLiked(int.Parse(dr["DOC_REV_ID"].ToString()), opt.UserId),
                                    Id = int.Parse(dr["DOC_REV_ID"].ToString()),
                                    Likes = GetNumLikes(int.Parse(dr["DOC_REV_ID"].ToString())),
                                    Rate = (float.Parse(dr["PUNC"].ToString()) + float.Parse(dr["KNOWLEDGE"].ToString()) + float.Parse(dr["RECEPTION"].ToString()) + float.Parse(dr["STAFF"].ToString()) + float.Parse(dr["POLITE"].ToString())) / 5
                                });
                            }
                            else
                            {

                                comments.Add(new Comment
                                {
                                    Instname = _inst_name,
                                    Instid = _inst_id,
                                    Punc = float.Parse(dr["PUNC"].ToString()),
                                    Know = float.Parse(dr["KNOWLEDGE"].ToString()),
                                    Rrec = float.Parse(dr["RECEPTION"].ToString()),
                                    Stuf = float.Parse(dr["STAFF"].ToString()),
                                    Pol = float.Parse(dr["POLITE"].ToString()),
                                    Text = dr["TEXT"].ToString(),
                                    UserId = dr["USER_ID"].ToString(),
                                    Date = dr["TIMESTAMP"].ToString(),
                                    Id = int.Parse(dr["DOC_REV_ID"].ToString()),
                                    Likes = GetNumLikes(int.Parse(dr["DOC_REV_ID"].ToString())),
                                    Liked=false,
                                    Rate = (float.Parse(dr["PUNC"].ToString()) + float.Parse(dr["KNOWLEDGE"].ToString()) + float.Parse(dr["RECEPTION"].ToString()) + float.Parse(dr["STAFF"].ToString()) + float.Parse(dr["POLITE"].ToString())) / 5
                                });
                            }
                         }
                    }
                }
            }
            return comments;
        }
        public bool IsLiked(int rev_id, int id)
        {
            bool liked = false;
            string query = @"select TOP 1 USER_ID from comments_like WHERE USER_ID='"+id+"' AND REV_ID=" + rev_id;
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
                            liked = true;
                            
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
            return liked;
        }
        
        public int GetNumLikes(int rev_id)
        {
            int likes = 0;
            string query = @"select count(REV_ID) as likes from comments_like WHERE REV_ID=" + rev_id;
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
                            likes = int.Parse(dr["likes"].ToString());
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
            return likes;
        }
        
        public string GetDocDepartment(int id, string lang)
        {
            int lang_id = 1;
            if (lang == "ar")
                lang_id = 2;

            string query = @"SELECT TEXT FROM DEPT_D WHERE DEPT_M_ID=(select DEPT_ID_DOC from DOC_M WHERE DOC_ID="+id+") AND LANG_ID=" + lang_id;

            string dept = "any";
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
                            dept = dr["text"].ToString();
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
            return dept;
        }

        public string GetDocCity(int id, string lang)
        {
            int lang_id = 1;
            if (lang == "ar")
                lang_id = 2;

            string query = @"select text from city_d where city_Id_m in (select city_id_doc from DOC_M where DOC_ID=" + id + ") and LANG_ID=" + lang_id;

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
        public string GetDocEmail(int id)
        {
            string email="";
            DataEncrypt encrypt = new DataEncrypt(); 
            string query = "SELECT EMAIL FROM USERS WHERE TYPE='2' AND M_ID='{{id}}'";
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
        public List<InstMin> AcceptFormAppointment(List<InstMin> list)
        {
            string idz = "";
            foreach (var l in list)
                idz += "," + l.Id;

            string query = @"select INST_ID, isnull(APP_STATUS,0) APP_STATUS,isnull(INST_EMAIL,'no_reaply@qayimdactory.com') INST_EMAIL,Inst_status,PHONE_NUM from INST_M where INST_STATUS=1 AND INST_ID in (" + idz.Substring(1) + ")";
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
                            list.Find(item => item.Id == int.Parse(dr["INST_ID"].ToString())).AccetFormApnt = (int.Parse(dr["APP_STATUS"].ToString()) == 1 ? true : false);
                            list.Find(item => item.Id == int.Parse(dr["INST_ID"].ToString())).Email = dr["INST_EMAIL"].ToString();
                            list.Find(item => item.Id == int.Parse(dr["INST_ID"].ToString())).Phone = dr["PHONE_NUM"].ToString();


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
        public List<InstMin> GetInstName(int id, string lang, string doc_id)
        {
            int lang_id = 1;
            if (lang == "ar")
                lang_id = 2;



            string query = @"select inst_id_m,name from INST_D where INST_Id_m='" + id + "' and LANG_ID=" + lang_id;

            if (doc_id != null)
            {
                query = @"select inst_id_m,name from INST_D where INST_Id_m in(select INST_ID_M FROM INST_DOC WHERE DOC_ID=" + doc_id + ") and LANG_ID=" + lang_id;
            }

            List<InstMin> inst = new List<InstMin>();
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
                            inst.Add(new InstMin
                            {
                                Id = int.Parse(dr["inst_id_m"].ToString()),
                                Name = dr["name"].ToString()
                            });
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
            return inst;
        }
        public string GetDeptArabicName(string dept_name)
        {

            string query = @"select text from DEPT_D where DEPT_M_ID in (SELECT DEPT_M_ID FROM DEPT_D WHERE text='{{dept_name}}' AND LANG_ID=1) AND LANG_ID=2";
            query = query.Replace("{{dept_name}}", dept_name.Replace('_', ' ').Replace("_AND_", "&"));

            string name = dept_name;
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
                            name = dr["text"].ToString();
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
            return name.Replace('_', ' ').Replace("_AND_", "&");
        }
        public List<string> GetDoctorDept(int id)
        {

            string query = @"select ar.TEXT ar_text ,en.TEXT en_text from 
            (select text from DEPT_D where DEPT_M_ID in (select DEPT_ID_DOC from DOC_M where DOC_ID={{id}}) and LANG_ID=2) ar,
            (select text from DEPT_D where DEPT_M_ID in (select DEPT_ID_DOC from DOC_M where DOC_ID={{id}}) and LANG_ID=1) en";
            query = query.Replace("{{id}}", id.ToString());

            List<string> names = new List<string>();
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
                            names.Add(dr["ar_text"].ToString());
                            names.Add(dr["en_text"].ToString());
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
            return names;
        }
        public List<DoctorCard> SerchDoctors(Options opt, string name)
        {

            List<DoctorCard> list = new List<DoctorCard>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connetionString))
                {
                    string query = @"select A.DOC_ID,FNAME,LNAME,DOC_IMG from (
                    select top 20 DOC_M_ID DOC_ID,FNAME,LNAME from DOC_D where (DOC_D.DOC_M_ID in
                    (SELECT DOC_ID FROM DOC_M WHERE DOC_M.CITY_ID_DOC in (select CITY_ID_M FROM CITY_D WHERE TEXT ='{{city}}')) )
                    AND CONCAT(FNAME,' ',LNAME) like N'%{{name}}%') A
                    left join DOC_M on A.DOC_ID=DOC_M.DOC_ID";

                    if (opt.City == null || opt.City == "all")
                    {
                        query = @"select A.DOC_ID,FNAME,LNAME,DOC_IMG from (
                    select top 20 DOC_M_ID DOC_ID,FNAME,LNAME from DOC_D where (DOC_D.DOC_M_ID in
                    (SELECT DOC_ID FROM DOC_M WHERE DOC_M.CITY_ID_DOC in (select CITY_ID_M FROM CITY_D WHERE TEXT !='all')) )
                    AND CONCAT(FNAME,' ',LNAME) like N'%{{name}}%') A
                    left join DOC_M on A.DOC_ID=DOC_M.DOC_ID";

                        query = query.Replace("{{name}}", name);
                    }
                    else
                    {
                        query = query.Replace("{{name}}", name);
                        query = query.Replace("{{city}}", opt.City);
                    }



                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            if (opt.Lang != "ar")
                            {
                                list.Add(new DoctorCard
                                {
                                    Id = int.Parse(dr["DOC_ID"].ToString()),
                                    Names = new string[] { dr["FNAME"].ToString() + " " + dr["LNAME"].ToString(), dr["FNAME"].ToString() + " " + dr["LNAME"].ToString() },
                                    Icon = dr["DOC_IMG"].ToString(), /*  opt.Domain  */
                                    Rate = 0,
                                    Profile_url = "/" + opt.Lang + "/doctor/" + dr["DOC_ID"].ToString() + "/" + (dr["FNAME"].ToString() + " " + dr["LNAME"].ToString()).Replace(' ', '-'),
                                    Dept_name = GetDocDepartment(int.Parse(dr["DOC_ID"].ToString()),opt.Lang)

                                });
                            }
                            else
                            {
                                list.Add(new DoctorCard
                                {
                                    Id = int.Parse(dr["DOC_ID"].ToString()),
                                    Names = new string[] { dr["FNAME"].ToString() + " " + dr["LNAME"].ToString(), dr["FNAME"].ToString() + " " + dr["LNAME"].ToString() },
                                    Icon = dr["DOC_IMG"].ToString(), /*  opt.Domain  */
                                    Rate = 0,
                                    Profile_url = "/doctor/" + dr["DOC_ID"].ToString() + "/" + (dr["FNAME"].ToString() + " " + dr["LNAME"].ToString()).Replace(' ', '-'),
                                    Dept_name = GetDocDepartment(int.Parse(dr["DOC_ID"].ToString()), opt.Lang)


                                });
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

            if (list.Count == 0)
            {
                list.Add(
                    new DoctorCard
                    {
                        Id = 0,
                        Names = new string[] {  "couldn't find your doctor? add now", "لم تجد طبيبك؟ قم بإضافته الآن" },
                        Icon = "default_pics/default_female.jpg", /*  opt.Domain  */
                        Rate = 0,
                        Profile_url = "/ar/new/doctor/sa-jeddah",
                        Dept_name = ""
                    }
                    );
            }
            return list;
        }
        public string GetDoctorUserId(int docid)
        {
            string query = @"SELECT USER_ID FROM USERS WHERE TYPE=2 AND M_ID=" + docid;

            string id = "any";
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
                            id = dr["USER_ID"].ToString();
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
            return id;
        }
        public bool HasActivePackage(int docid)
        {
            bool active = false;
            var query = @"select * from Package_Subscribtions where REQ_DOC_ID=" + docid + " and getdate()<END_DATE AND PKG_STATUS='A'";

            try
            {
                using (SqlConnection conn = new SqlConnection(connetionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        return true;
                    }
                    else
                    {
                        dr.Close();
                        return false;
                    }

                }

            }
            catch (Exception ex)
            {

            }
            return active;
        }
        public Response UpdateDoctorComment(Comment comment)
        {
            string query = "AmendComment";
            try
            {
               
                if (query != "")
                {
                    using (SqlConnection conn = new SqlConnection(connetionString))
                    {


                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Punc", comment.Punc);
                        cmd.Parameters.AddWithValue("@Knowledge", comment.Know);
                        cmd.Parameters.AddWithValue("@Polite", comment.Pol);
                        cmd.Parameters.AddWithValue("@Reception", comment.Rrec);
                        cmd.Parameters.AddWithValue("@Staff", comment.Stuf);
                        cmd.Parameters.AddWithValue("@LangId", 2);
                        cmd.Parameters.AddWithValue("@Comment", comment.Text);
                        cmd.Parameters.AddWithValue("@instid", comment.Instid);
                        cmd.Parameters.AddWithValue("@DocId", comment.DocId);
                        cmd.Parameters.Add("@SendEmail", SqlDbType.VarChar, 1).Value = "N";
                        cmd.Parameters["@SendEmail"].Direction=ParameterDirection.Output;
                        cmd.Parameters.AddWithValue("@Active", 1);
                        cmd.Parameters.AddWithValue("@DOC_REV_ID", comment.Id);
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        var output= Convert.ToString(cmd.Parameters["@SendEmail"].Value);

                        if(output!=null && output=="Y")
                        {
                            /* notify doctor */
                            return new Response
                            {
                                Code = "000.000.000",
                                Message = "ok",
                                Data = null
                            };
                        }

                        return new Response
                        {
                            Code = "000.000.000",
                            Message = "invalid",
                            Data = null
                        };


                    }
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Code = "111.111.111",
                    Message = ex.ToString(),
                    Data = null
                };

            }
            return new Response
            {
                Code = "100.000.000",
                Message = "invalid",
                Data = null
            };
        }

        public Comment GetDoctorGradeDetails(int DOC_ID)
        {
            var d = new Comment
            {
                Punc = 0,
                Know = 0,
                Stuf = 0,
                Pol = 0,
                Rrec = 0
            };

            string query = "SELECT Format(avg(PUNC),'N1') p,Format(avg(knowledge),'N1') k,Format(avg(reception),'N1') r,Format(avg(staff),'N1') s,Format(avg(polite),'N1') po FROM DOC_REV_M WHERE DOC_ID=" + DOC_ID;
            using (SqlConnection conn = new SqlConnection(connetionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        d=new Comment
                        {
                            Punc = float.Parse(dr["p"].ToString()),
                            Know = float.Parse(dr["k"].ToString()),
                            Rrec = float.Parse(dr["r"].ToString()),
                            Pol = float.Parse(dr["po"].ToString()),
                            Stuf = float.Parse(dr["s"].ToString()),

                        };
                        
                        break;
                    }
                }
                dr.Close();
                conn.Close();
            }
            return d;
        }
        
        public Response AddComment(Comment comment)
        {
            string query = "ADD_AMEND_CMNT";
            try
            {

                if (query != "")
                {
                    using (SqlConnection conn = new SqlConnection(connetionString))
                    {


                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Punc", comment.Punc);
                        cmd.Parameters.AddWithValue("@Knowledge", comment.Know);
                        cmd.Parameters.AddWithValue("@Polite", comment.Pol);
                        cmd.Parameters.AddWithValue("@Reception", comment.Rrec);
                        cmd.Parameters.AddWithValue("@Staff", comment.Stuf);
                        cmd.Parameters.AddWithValue("@LangId", 2);
                        cmd.Parameters.AddWithValue("@Comment", comment.Text);
                        cmd.Parameters.AddWithValue("@UpdId", DBNull.Value);
                        cmd.Parameters.AddWithValue("@UserId", comment.UserId);
                        cmd.Parameters.AddWithValue("@instid", comment.Instid);
                        cmd.Parameters.AddWithValue("@DocId", comment.DocId);
                        cmd.Parameters.Add("@SendEmail", SqlDbType.VarChar, 1).Value = "N";
                        cmd.Parameters["@SendEmail"].Direction = ParameterDirection.Output;
                        cmd.Parameters.AddWithValue("@Active", 1);
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        var output = Convert.ToString(cmd.Parameters["@SendEmail"].Value);

                        if (output != null && output == "Y")
                        {

                            /* notify doctor */
                            new NotificationEndPoint().NotifyDoctor("user_add_comment", null, comment.DocId);

                            return new Response
                            {
                                Code = "000.000.000",
                                Message = "ok",
                                Data = null
                            };

                        }

                        return new Response
                        {
                            Code = "000.000.000",
                            Message = "invalid",
                            Data = null
                        };


                    }
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Code = "111.111.111",
                    Message = ex.ToString(),
                    Data = null
                };

            }
            return new Response
            {
                Code = "100.000.000",
                Message = "invalid",
                Data = null
            };
        }
        public Response NewDoctor(FormNewDoctor form)
        {
            string query = "ADD_DOC_PAT";
            try
            {
                if (query != "")
                {
                    using (SqlConnection conn = new SqlConnection(connetionString))
                    {


                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@DeptId", form.DeptId);
                        cmd.Parameters.AddWithValue("@Phone", DBNull.Value);
                        cmd.Parameters.AddWithValue("@Email", DBNull.Value);
                        cmd.Parameters.AddWithValue("@Ext", DBNull.Value);
                        cmd.Parameters.AddWithValue("@UserId", form.UserId);
                        cmd.Parameters.AddWithValue("@CityId", form.CityId);
                        cmd.Parameters.AddWithValue("@Gender", form.Gender);
                        cmd.Parameters.AddWithValue("@Fname", form.Fname);
                        cmd.Parameters.AddWithValue("@Lname", form.Lname);
                        cmd.Parameters.AddWithValue("@Rank", form.Rank);
                        cmd.Parameters.AddWithValue("@InstId", form.InstId);
                        cmd.Parameters.AddWithValue("@LangId", "2");
                        cmd.Parameters.AddWithValue("@SesId", "2867");
                        cmd.Parameters.AddWithValue("@ToR", "ADD");
                        cmd.Parameters.AddWithValue("@DocId", DBNull.Value);
                        cmd.Parameters.Add("@Doc", SqlDbType.Int, 1).Value = 0;
                        cmd.Parameters["@Doc"].Direction = ParameterDirection.Output;
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        var output = int.Parse(Convert.ToString(cmd.Parameters["@Doc"].Value));

                        if (output > 0)
                        {
                            return new Response
                            {
                                Code = "000.000.000",
                                Message = "ok",
                                Data = null
                            };
                        }
                        else
                        {
                            return new Response
                            {
                                Code = "111.000.000",
                                Message = "invalid",
                                Data = null
                            };
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Code = "111.111.111",
                    Message = ex.ToString(),
                    Data = null
                };

            }
            return new Response
            {
                Code = "100.000.000",
                Message = "invalid",
                Data = null
            };
        }
        public Response NewDoctor(FormNewDoctor form,string add_as)
        {
            bool IsClaim = false;
            string query = "ADD_DOC_DOC";
            if (add_as == "CLAIM")
                IsClaim = true;
            
            try
            {
                if (query != "")
                {
                    using (SqlConnection conn = new SqlConnection(connetionString))
                    {


                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@DeptId", form.DeptId);
                        cmd.Parameters.AddWithValue("@Phone", DBNull.Value);
                        cmd.Parameters.AddWithValue("@Email", DBNull.Value);

                        cmd.Parameters.AddWithValue("@Pw", DBNull.Value);
                        cmd.Parameters.AddWithValue("@Ext", DBNull.Value);
                        cmd.Parameters.AddWithValue("@UserId", form.UserId);

                        cmd.Parameters.AddWithValue("@CityId", form.CityId);
                        cmd.Parameters.AddWithValue("@Gender", form.Gender);

                        cmd.Parameters.AddWithValue("@FnameAr", form.Fname);
                        cmd.Parameters.AddWithValue("@LnameAr", form.Lname);
                        cmd.Parameters.AddWithValue("@MnameAr", form.Mname);
                        cmd.Parameters.AddWithValue("@FnameEn", form.FnameSL);
                        cmd.Parameters.AddWithValue("@LnameEn", form.LnameSL);
                        cmd.Parameters.AddWithValue("@MnameEn", form.MnameSL);

                        cmd.Parameters.AddWithValue("@Rank", form.Rank);
                        cmd.Parameters.AddWithValue("@InstId", form.InstId);

                        cmd.Parameters.AddWithValue("@SesId", "2867");

                        cmd.Parameters.AddWithValue("@MedId", form.SCHS);
                        cmd.Parameters.AddWithValue("@DocImg", DBNull.Value);
                        cmd.Parameters.AddWithValue("@DocMob", DBNull.Value);
                        cmd.Parameters.AddWithValue("@ShowMob", 'N');

                        if (IsClaim)
                        {
                            cmd.Parameters.AddWithValue("@DocId", form.DocId);
                            cmd.Parameters.AddWithValue("@Type", "CLAIM");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@Type", "NEW");
                            cmd.Parameters.AddWithValue("@DocId", DBNull.Value);
                        }
                        


                        cmd.Parameters.AddWithValue("@DocIdImg", form.ID_IMG);

                        cmd.Parameters.Add("@Doc", SqlDbType.Int, 1).Value = 0;
                        cmd.Parameters["@Doc"].Direction = ParameterDirection.Output;
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        var output = int.Parse(Convert.ToString(cmd.Parameters["@Doc"].Value));

                        if (output > 0)
                        {
                            return new Response
                            {
                                Code = "000.000.000",
                                Message = "ok",
                                Data = null
                            };
                        }
                        else
                        {
                            return new Response
                            {
                                Code = "111.000.000",
                                Message = "invalid",
                                Data = null
                            };
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Code = "111.111.111",
                    Message = ex.ToString(),
                    Data = null
                };

            }
            return new Response
            {
                Code = "100.000.000",
                Message = "invalid",
                Data = null
            };
        }

        
    }
    public class InstMin
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool AccetFormApnt { get; set; }
        public string Phone { get; set; }
    }
    public class FormNewDoctor
    {
        public int DeptId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Ext { get; set; }
        public string UserId { get; set; }
        public int CityId { get; set; }
        public string Gender { get; set; }
        public string Fname { get; set; }
        public string Mname { get; set; }
        public string Lname { get; set; }

        public string FnameSL { get; set; }
        public string MnameSL { get; set; }
        public string LnameSL { get; set; }
        public int Rank { get; set; }
        public int InstId { get; set; }
        public int LangId { get; set; }
        public int SesId { get; set; }
        public string ToR { get; set; }
        public int DocId { get; set; }
        public int Doc { get; set; }
        public string ID_IMG {get; set;}
        public string SCHS { get; set; }
        public string MOB_NUM { get; set; }
        public string Token { get; set; }
    }

}