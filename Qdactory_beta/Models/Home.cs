using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Qdactory_beta.Models
{
    public class Home
    {
        public List<Department> departments;
        public List<Institute> institutes;
        public List<DoctorCard> doctors;
        public List<City> cities = new List<City>();
        public List<Post> Ranks;
        public Home(Options opt)
        {
            var app = new MainApp();
            var inst = new InstituteEndPoint();
            var doc = new DoctorEndPoint();
            this.institutes = inst.GetInstitutes(opt, 0, 6);
            this.doctors = doc.GetDoctors(opt, 0, 6);
            this.cities = app.Set_cities(opt.Lang, opt.Cities);
            this.departments = app.GetDepartments(opt);
        }
    }
}