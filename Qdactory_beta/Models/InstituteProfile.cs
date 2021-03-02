using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Qdactory_beta.Models
{
    public class InstituteProfile
    {
        public List<Comment> comments;
        public List<Offer> offers;
        public List<DoctorCard> doctors;
        public Engagment engagment;
        public Institute info;
        public List<City> cities = new List<City>();
        public List<Department> departments;
        public string city;
        public string country;
        public InstituteProfile(Options opt)
        {
            var app = new MainApp();
            var doc = new DoctorEndPoint();
            var inst = new InstituteEndPoint();
            this.comments = GetComments(opt, doc);
            this.offers = GetDoctorOffers(opt, inst);
            this.engagment = GetEngagments(opt, doc);
            this.doctors = GetInstDoctors(opt, doc);
            this.info = GetInstituteCard(opt, inst);
            this.cities = app.Set_cities(opt.Lang, opt.Cities);
            this.city = inst.GetInstCity(this.info.Id, "en").ToLower();
            this.country = "sa";
            opt.City = this.city.ToLower();
            opt.Country = this.country.ToLower();
            this.departments = app.GetDepartments(opt);
            this.info.City_name = inst.GetInstCity(this.info.Id, opt.Lang);
        }

        public Institute GetInstituteCard(Options opt, InstituteEndPoint ep)
        {
            opt.Listing_type = "inst_by_id";
            var others = ep.GetInstitutes(opt,0,1);
            return others[0];
        }
        public List<DoctorCard> GetInstDoctors(Options opt, DoctorEndPoint ep)
        {
            opt.Listing_type = "doctors_within_inst";
            var others = ep.GetDoctors(opt,0,12);
            return others;
        }
        public List<Offer> GetDoctorOffers(Options opt, InstituteEndPoint ep)
        {
            var offers = ep.GetOffers(opt, 0, 12);
            return offers;
        }
        public List<Comment> GetComments(Options opt, DoctorEndPoint ep)
        {
            return null;
        }
        public Engagment GetEngagments(Options opt, DoctorEndPoint ep)
        {
            return null;
        }
    }
}