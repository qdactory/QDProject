using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Qdactory_beta.Models
{
    public class DepartmentProfile
    {
        public string department_name;
        public string icon;
        public List<Comment> comments;
        public List<Offer> offers;
        public List<DoctorCard> doctors;
        public List<DoctorCard> otherdoctors;
        public Engagment engagment;
        public Department info;
        public List<City> cities = new List<City>();
        public List<Department> departments;
        public DepartmentProfile(Options opt)
        {
            var app = new MainApp();
            var doc = new DoctorEndPoint();
            this.department_name = doc.GetDeptArabicName(opt.Department);
            this.icon = opt.Department.Replace(' ', '_') + ".png";
            //this.comments = GetComments(opt, doc);
            this.offers = GetDoctorOffers(opt, doc);
            this.engagment = GetEngagments(opt, doc);
           
            this.otherdoctors = GetOtherDeptDoctors(opt, doc);
            this.doctors = GetDeptDoctors(opt, doc, this.otherdoctors);
            this.info = GetDepartmentInfo(opt, app);
            this.cities = app.Set_cities(opt.Lang, opt.Cities);
            this.departments = app.GetDepartments(opt);
        }

        public Department GetDepartmentInfo(Options opt, MainApp ep)
        {
            var others = ep.GetDepartments(opt);
            return others[0];
        }

         public List<DoctorCard> GetOtherDeptDoctors(Options opt, DoctorEndPoint ep)
        {
            opt.Listing_type = "promoted_doctors_in_department_in_a_city";
            var others = ep.GetDoctors(opt, 0, 6);
            return others;
        }
        public List<DoctorCard> GetDeptDoctors(Options opt, DoctorEndPoint ep, List<DoctorCard> promoted)
        {
            opt.Listing_type = "all_doctors_in_department_in_a_city";
            var others = ep.GetDoctorsEclude(opt,0,10,promoted);
            return others;
        }


        public List<Offer> GetDoctorOffers(Options opt, DoctorEndPoint ep)
        {
            return null;
        }
        public List<Comment> GetComments(Options opt, DoctorEndPoint ep)
        {
            opt.Listing_type = "comments_by_city_and_dept";
            var comments=ep.GetComments(opt,0,4);
            return comments;
        }
        public Engagment GetEngagments(Options opt, DoctorEndPoint ep)
        {
            return null;
        }
    }
}