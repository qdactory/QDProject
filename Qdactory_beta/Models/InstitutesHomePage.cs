using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Qdactory_beta.Models
{
    public class InstitutesHomePage
    {
        public List<Institute> slides;
        public List<Institute> institutes;
        public List<Offer> offers;
        public List<City> cities = new List<City>();
        public List<Department> departments;
        public InstitutesHomePage(Options opt)
        {
            var app = new MainApp();
            var inst = new InstituteEndPoint();
            this.offers = GetInstsOffers(opt, inst);
            this.slides = GetSlides(opt, inst);
            this.institutes = GetInstituteCards(opt, inst);
            this.cities = app.Set_cities(opt.Lang, opt.Cities);
            this.departments = app.GetDepartments(opt);
        }

        public List<Institute> GetInstituteCards(Options opt, InstituteEndPoint ep)
        {

            var others = ep.GetInstitutes(opt,0,12);
            return others;
        }
        public List<Institute> GetSlides(Options opt, InstituteEndPoint ep)
        {

            var others = ep.GetInstitutes(opt,0,6);
            return others;
        }
        public List<Offer> GetInstsOffers(Options opt, InstituteEndPoint ep)
        {
            return null;
        }
  
    }
}