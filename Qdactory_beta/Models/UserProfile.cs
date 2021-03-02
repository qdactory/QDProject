using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Qdactory_beta.Models
{
    public class UserProfile
    {
        public List<Comment> comments;
        public List<Offer> offers;
        public List<DoctorCard> doctors;
        public Engagment engagment;
        public UserInfo info;
        public List<City> cities = new List<City>();
        public List<Department> departments;
        public ChatTalk chatTalk;
      
        public UserProfile(Options opt)
        {
            var app = new MainApp();
            var doc = new DoctorEndPoint();
            opt.Country = "sa";
            this.comments = GetComments(opt,doc);
            this.engagment = GetEngagments(opt,doc);
            this.info = GetUserInfo(opt,app);
            this.doctors = GetOtherDoctors(opt, doc);
            this.cities=app.Set_cities(opt.Lang, opt.Cities);
           
        }

        public UserInfo GetUserInfo(Options opt, MainApp ep)
        {
            opt.Listing_type = "using_user_id";

            var others = ep.GetUsers(opt,0,1);
            return others[0];
        }
        public List<DoctorCard> GetOtherDoctors(Options opt,DoctorEndPoint ep)
        {
            /* get doctors ased on doctor's department */
            
            opt.Listing_type = "doctors_rated_by_user_id";
            var others = ep.GetDoctors(opt,0,6);
            return others;
        }
        public List<Offer> GetDoctorOffers(Options opt, DoctorEndPoint ep)
        {
            return null;
        }
        public List<Comment> GetComments(Options opt, DoctorEndPoint ep)
        {
            opt.Listing_type = "comments_by_user_id";
            var comments = ep.GetComments(opt,0,6);
            return comments;
        }
        public Engagment GetEngagments(Options opt, DoctorEndPoint ep)
        {
            return null;
        }
    }
}