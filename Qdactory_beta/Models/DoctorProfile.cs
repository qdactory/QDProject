using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Qdactory_beta.Models
{
    public class DoctorProfile
    {
        public string UserId;
        public Comment LoggedInUserComment;
        public List<Comment> comments;
        public List<Offer> offers;
        public List<DoctorCard> otherdoctors;
        public Engagment engagment;
        public DoctorCard info;
        public List<City> cities = new List<City>();
        public List<Department> departments;
        public ChatTalk chatTalk;
        public string city;
        public string country;
        public bool ActivePackage;
        public bool ShowMobile;
        public string Call;
        public bool AcceptApnt = false;
        public Comment DocRatingDetails;




        public DoctorProfile(Options opt)
        {
            int lang_id = 1;
            if (opt.Lang == "ar")
                lang_id = 0;

            var app = new MainApp();
            var doc = new DoctorEndPoint();
            /* since the url does not indicate which city or dept */
            opt.Department = doc.GetDoctorDept(opt.DocId)[1]; 
            opt.City = doc.GetDocCity(opt.DocId, "en");
            opt.Country = "sa";
            this.UserId = doc.GetDoctorUserId(opt.DocId);
            this.comments = GetComments(opt,doc);
            this.offers = GetDoctorOffers(opt,doc);
            this.engagment = GetEngagments(opt,doc);
            this.info = GetDoctorCard(opt,doc);
            this.ActivePackage = doc.HasActivePackage(opt.DocId);
            if (!this.ActivePackage)
            {
                this.otherdoctors = GetOtherDoctors(opt, doc);
            }
            else
                this.otherdoctors = new List<DoctorCard>();
            
            this.cities=app.Set_cities(opt.Lang, opt.Cities);
            this.DocRatingDetails = doc.GetDoctorGradeDetails(opt.DocId);
            this.info.City_name = opt.City;
            this.info.Dept_name = opt.Department;
            this.info.City_name= doc.GetDocCity(opt.DocId, opt.Lang);
            this.info.Dept_name = doc.GetDoctorDept(opt.DocId)[lang_id];
            this.city = doc.GetDocCity(opt.DocId, "en").ToLower();
            this.country = "sa";
            opt.City = this.city.ToLower();
            opt.Country = this.country.ToLower();
            this.departments = app.GetDepartments(opt);
            if (this.info.ShowMob != "N")
            {
                this.ShowMobile = true;
                this.Call = this.info.Call;
            }
            else
            {
                this.ShowMobile = false;
            }
            this.info.Inst_names = doc.AcceptFormAppointment(this.info.Inst_names);
            foreach (var i in this.info.Inst_names)
                if (i.AccetFormApnt)
                    this.AcceptApnt = true;


        }
        public DoctorCard GetDoctorCard(Options opt, DoctorEndPoint ep)
        {
            opt.Listing_type = "doctor_with_id";
            var others = ep.GetDoctors(opt,0,1);
            return others[0];
        }
        public List<DoctorCard> GetOtherDoctors(Options opt,DoctorEndPoint ep)
        {
            /* get doctors ased on doctor's department */
            
            opt.Listing_type = "doctors_in_given_department_and_city_of_doctor";
            var others = ep.GetDoctors(opt,0,6);
            return others;
        }
        public List<Offer> GetDoctorOffers(Options opt, DoctorEndPoint ep)
        {
            return null;
        }
        public List<Comment> GetComments(Options opt, DoctorEndPoint ep)
        {
            opt.Listing_type = "comments_by_doc_id";
            var comments = ep.GetComments(opt,0,6);
            return comments;
        }
        public Engagment GetEngagments(Options opt, DoctorEndPoint ep)
        {
            return null;
        }
    }


    public class ChatTalk
    {
        public bool UserAllowed { get; set; }
        public bool DoctorActivated { get; set; }

        public UserChatTalk Me { get; set; }
        public UserChatTalk Other { get; set; }
    }

    public class UserChatTalk
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhotoUrl { get; set; }

    }
}


