using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Qdactory_beta
{
    public class MetaTags
    {
        public string Title { get; set; }
        public string Desc { get; set; }
        public string Image { get; set; }
        public string Robots { get; set; }
        public string Visits { get; set; }
        public string Type { get; set; }
        public string Canonical { get; set; }
        public string Keys { get; set; }

        public MetaTags()
        {

        }
        public MetaTags(string url, string domain, MetaTags default_tag)
        {

            /* GET MetaTags From DB based on URL */
            if (url == "/ar/sa-jeddah" || url == "/sa-jeddah")
            {
                this.Title = "ابحث عن طبيب او طبيبة في السعودية مدينة جده";
                this.Desc = "موقع قيم دكتوري يسهل لك الوصول إلى افضل الأطباء حسب تقييم المرضى للأطباء من جميع التخصصات لمدينة جدة السعودية";
                this.Image = domain + "/" + "default.png";
                this.Robots = "Index, follow";
                this.Visits = "1 days";
                this.Type = "website";
                this.Keys = "افضل طبيب في جدة, افضل دكتور اطفال في جدة";
                this.Canonical = domain + url;
            }
            else
            if (url == "/ar/sa-riyadh")
            {
                this.Title = "ابحث عن طبيب او طبيبة في السعودية مدينة الرياض";
                this.Desc = "موقع قيم دكتوري يسهل لك الوصول إلى افضل الأطباء حسب تقييم المرضى للأطباء من جميع التخصصات لمدينة الرياض السعودية";
                this.Image = domain + "/" + "default.png";
                this.Robots = "Index, follow";
                this.Visits = "1 days";
                this.Type = "website";
                this.Keys = "افضل طبيب في جدة, افضل دكتور اطفال في الرياض";
                this.Canonical = domain + url;
            }
            else
            if (url == "/ar/sa")
            {
                this.Title = "أفضل الأطباء في المملكة العربية السعودية";
                this.Desc = "موقع قيم دكتوري يسهل لك الوصول إلى افضل الأطباء حسب تقييم المرضى للأطباء من جميع التخصصات على جميع مدن السعودية";
                this.Image = domain + "/" + "default.png";
                this.Robots = "Index, follow";
                this.Visits = "1 days";
                this.Type = "website";
                this.Keys = "افضل طبيب في السعودية, افضل دكتور اسنان في السعودية";
                this.Canonical = domain + url;
            }
            else
            {
                /* IF URL not exists then use the default tag passed */

                if (default_tag != null)
                {
                    /* passed tag is set then use it */

                    this.Title = default_tag.Title;
                    this.Desc = default_tag.Desc;
                    this.Image = default_tag.Image;
                    this.Robots = default_tag.Robots;
                    this.Visits = default_tag.Visits;
                    this.Type = default_tag.Type;
                    this.Canonical = default_tag.Canonical;
                }
                else
                {
                    /* passed tag is not set then use this default one */

                    this.Title = "ابحث عن طبيب او طبيبة في السعودية مدينة جده";
                    this.Desc = "موقع قيم دكتوري يسهل لك الوصول إلى افضل الأطباء حسب تقييم المرضى للأطباء من جميع التخصصات لمدينة جدة السعودية";
                    this.Image = domain + "/" + "default.png";
                    this.Robots = "Index, follow";
                    this.Visits = "1 days";
                    this.Type = "website";
                    this.Keys = "افضل طبيب في جدة, افضل دكتور اطفال في جدة";
                    this.Canonical = domain + url;
                }

            }
        }
    }
    public class ApiResponse
    {
        public string CODE { get; set; }
        public string MESSAGE { get; set; }
        public object Data { get; set; }
    }
    public class Response
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public Object Data { get; set; }
    }
    public class PaTProfile
    {
        public string F_NAME { get; set; }
        public string L_NAME { get; set; }
        public string EMAIL { get; set; }
        public string PASSWORD { get; set; }
        public string PHONE_NUMBER { get; set; }
        public string GENDER { get; set; }
        public string DOB { get; set; }
        public string Token { get; set; }
        public string SmsCode { get; set; }

    }
    public class UserInfo
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
        public string Mobil { get; set; }
        public string[] Names { get; set; }

        public string Type { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public string SmsCode { get; set; }
        public int M_ID { get; set; }
    }
    public class DoctorCard
    {
        public int Id { get; set; }
        public string[] Names { get; set; }
        public string Icon { get; set; }

        public float Rate { get; set; }
        public int Total { get; set; }

        public List<InstMin> Inst_names { get; set; }
        public string Dept_name { get; set; }
        public string City_name { get; set; }
        public string Country_name { get; set; }
        public string Profile_url { get; set; }
        public string[] SUB_SPEC { get; set; }
        public int Rank { get; set; }
        public string Doc_verified { get; set; }
        public string ShowMob { get; set; }
        public string Call { get; set; }

    }

    public class FormReport
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string Subject {get; set;}
        public string Content { get; set; }

    }
    
    public class FormLoginORSignUp
    {

        public string Email { get; set; }
        public string Mobile { get; set; }
        public string NewMobile { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
       
    }
    public class FormSignUpDoctor
    {
        public string Fname { get; set; }
        public string Mname { get; set; }
        public string Lname { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Gender { get; set; }
        public string Rank { get; set; }
        public string DeptId { get; set; }
        public string CityId { get; set; }
        public string InstId { get; set; }
        public string MedID { get; set; }
        public string MedImage { get; set; }
        public string DocID { get; set; }
    }
    public class Department
    {
        public int Id { get; set; }
        public string[] Names { get; set; }
        public string Icon { get; set; }

        public string Tags { get; set; }
        public string Profile_url { get; set; }
    }
    public class City
    {
        public int Id { get; set; }
        public string[] Names { get; set; }
        public string Icon { get; set; }
    }
    public class Offer
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public string ImageUrl { get; set; }
    }
    public class Engagment
    {
        public List<Post> Posts { get; set; }
        public List<Post> Answers { get; set; }
        public List<Post> Questions { get; set; }
    }
    public class Post
    {
        public int Id { get; set; }
        public string Post_name { get; set; }
        public string Post_title { get; set; }
        public string Post_content { get; set; }
        public string Post_date { get; set; }
    }
    public class Comment
    {
       

        public int Id { get; set; }
        public int DocId { get; set; }
        public string Text { get; set; }
        public float Pol { get; set; }
        public float Know { get; set; }
        public float Rrec { get; set; }
        public float Stuf { get; set; }
        public float Punc { get; set; }
        public float Rate { get; set; }
        public string Date { get; set; }
        public string UserId { get; set; }
        public int Instid { get; set; }
        public string Instname { get; set; }
        public bool Isdoctor { get; set; }
        public int Likes { get; set; }
        public bool Liked { get; set; }
        public string Token { get; set; }
    }
    public class Institute
    {
        public int Id { get; set; }
        public string[] Names { get; set; }
        public string Icon { get; set; }
        public float Rate { get; set; }
        public int Total { get; set; }
        public string City_name { get; set; }
        public string Profile_url { get; set; }

        public string Inst_verified { get; set; }
    }
    public class Options
    {
        public string Domain { get; set; }
        public string Lang { get; set; }
        public string Type { get; set; }
        public string Sort { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Gender { get; set; }
        public int InstId { get; set; }
        public int DocId { get; set; }

        public int UserId { get; set; }
        public string Cities { get; set; }
        public string Departments { get; set; }
        public string Department { get; set; }
        public string Mode { get; set; }
        public string Listing_type { get; set; }

    }
}