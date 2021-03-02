using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qdactory_beta.Models;


namespace Qdactory_beta.Controllers
{
    public class HomeController : Controller
    {
        private readonly ConnectionStrings connectionStrings;
        private readonly ILogger<HomeController> _logger;

        public string default_domain = "http://qdactorymvc-001-site1.itempurl.com/";
        public string default_lang = "ar";
        public string default_country_city = "sa";
        public string default_country_city_ar = "المملكة العربية السعودية";
        public int langId = 1;
        public string SessionName = "user";


        public HomeController(IOptions<ConnectionStrings> connectionStrings)
        {
            this.connectionStrings = connectionStrings.Value;
            //_logger = logger;
        }

        [Route("")]
        public IActionResult Index()
        {
            ViewBag.SideBar = true;
            UserSettings();
            ViewBag.Url = default_domain;
            Page_settings(default_lang, "sa", "all", "home", null);
            var home = new Home(new Options
            {
                Domain = default_domain,
                Lang = default_lang,
                Country = "sa",
                City = "all",
                Listing_type = "promoted_doctors_all_departments_in_all_cities",
                Cities = Get_Cities("sa"),
                Departments = GetAllDepartments()
            });
            return View(home);
        }

        /* GENERAL PAGES */
        [Route("privacy")]
        public IActionResult Privacy()
        {
            ViewBag.LoggedIn = false;
            ViewBag.IsPatient = false;
            ViewBag.DidNotSubmitAlready = false;
            ViewBag.SideBar = false;

            UserSettings();
            Page_settings(default_lang, "sa","all", "privacy", null);
            var home = new Home(
                new Options
                {
                    Domain = default_domain,
                    Lang = default_lang,
                    Country = "all",
                    City = "all",
                    Department = "all",
                    Cities = Get_Cities("sa"),
                    Departments = GetAllDepartments(),
                    Mode = "on_click"
                });
            return View(home);

        }

        [Route("terms")]
        public IActionResult Terms()
        {
            ViewBag.LoggedIn = false;
            ViewBag.IsPatient = false;
            ViewBag.DidNotSubmitAlready = false;
            ViewBag.SideBar = false;

            UserSettings();
            Page_settings(default_lang, "sa", "all", "privacy", null);
            var home = new Home(
                new Options
                {
                    Domain = default_domain,
                    Lang = default_lang,
                    Country = "all",
                    City = "all",
                    Department = "all",
                    Cities = Get_Cities("sa"),
                    Departments = GetAllDepartments(),
                    Mode = "on_click"
                });
            return View(home);

        }

        [Route("about")]
        public IActionResult About()
        {
            ViewBag.LoggedIn = false;
            ViewBag.IsPatient = false;
            ViewBag.DidNotSubmitAlready = false;
            ViewBag.SideBar = false;

            UserSettings();
            Page_settings(default_lang, "sa", "all", "privacy", null);
            var home = new Home(
                new Options
                {
                    Domain = default_domain,
                    Lang = default_lang,
                    Country = "all",
                    City = "all",
                    Department = "all",
                    Cities = Get_Cities("sa"),
                    Departments = GetAllDepartments(),
                    Mode = "on_click"
                });
            return View(home);

        }

        [Route("contactus")]
        public IActionResult Contactus()
        {
            ViewBag.LoggedIn = false;
            ViewBag.IsPatient = false;
            ViewBag.DidNotSubmitAlready = false;
            ViewBag.SideBar = false;

            UserSettings();
            Page_settings(default_lang, "sa", "all", "privacy", null);
            var home = new Home(
                new Options
                {
                    Domain = default_domain,
                    Lang = default_lang,
                    Country = "all",
                    City = "all",
                    Department = "all",
                    Cities = Get_Cities("sa"),
                    Departments = GetAllDepartments(),
                    Mode = "on_click"
                });
            return View(home);

        }
        /* END GENERAL PAGES */


        [Route("{country}-{city}")]
        [Route("{country}")]
        [Route("{lang}/{country}-{city}")]
        public IActionResult HomePage(string lang, string country, string city)
        {
            ViewBag.SideBar = true;
            if (city == null)
                city = "all";
            UserSettings();
            Page_settings(lang, country, city, "home", null);
            var home = new Home(
                new Options
                {
                    Domain = default_domain,
                    Lang = default_lang,
                    Country = country,
                    City = city,
                    Department = "all",
                    Listing_type = "promoted_doctors_all_departments_in_given_city",
                    Cities = Get_Cities(country),
                    Departments = GetAllDepartments()
                });
          
            return View(home);
        }

        [Route("{country}-{city}/{department}")]
        [Route("{lang}/{country}-{city}/{department}")]
        public IActionResult Department(string lang, string country, string city, string department)
        {
            ViewBag.SideBar = true;
            UserSettings();
            Page_settings(lang, country, city, "department", null);
            var dep = new DepartmentProfile(
            new Options
            {
                Domain = default_domain,
                Lang = default_lang,
                Cities = Get_Cities(country),
                Country = country.ToLower(),
                City = city.ToLower(),
                Department = department,
                Departments = GetAllDepartments()
            }
            );
            if (!department.Equals("all_departments"))
            {
                if (CheckDepartment(department))
                {
                    ViewBag.Deparment = department.ToLower();
                }
                else
                {
                    return Redirect("/sa-jeddah/all_departments");
                }
            }
            else
            {
                /* Show all departments */
            }
            return View(dep);
        }


        [Route("doctor/{id}/{name}")]
        [Route("{lang}/doctor/{id}/{name}")]
        public IActionResult Doctor(string lang, int id, string name)
        {
            if (lang != null && (lang == "ar" || lang == "en"))
            {
                default_lang = lang;
               
            }

            ViewBag.SideBar = true;
            UserSettings();
            UserInfo info = new UserInfo();
            if (Request.Cookies["user"] != null)
            {

                var key = Request.Cookies["user"];
                var usr_data = new MainApp().CheckToken(key);
                if (usr_data.Code == "000.000.000")
                {
                    info = (UserInfo)usr_data.Data;

                }
            }
            else
            {
                info.Id = -1; //User not logged in
            }

            
                var doctor = new DoctorProfile(
                new Options
                {
                    Domain = default_domain,
                    Lang = default_lang,
                    Cities = Get_Cities("sa"),
                    UserId = info.Id,
                    DocId = id,
                    Departments = GetAllDepartments()
                });
            string canonical = default_domain + "/doctor/" + doctor.info.Id + "/" + doctor.info.Names[1].Trim();
            if (default_lang != "ar")
            {
                /* for non arabic languages */
                canonical = default_domain + "/" + default_lang + "/doctor/" + doctor.info.Id + "/" + doctor.info.Names[1].Trim();
            }
            MetaTags tag;
            if (default_lang == "ar")
            {
                tag =
                   new MetaTags
                   {
                       Title = " ملف الدكتور " + doctor.info.Names[langId].Trim(),
                       Desc = "شاهد صفحة الطبيب الرائع",
                       Image = doctor.info.Icon,
                       Canonical = canonical
                   };
            }
            else
            {
                tag =
                   new MetaTags
                   {
                       Title = " ملف الدكتور " + doctor.info.Names[langId].Trim(),
                       Desc = "شاهد صفحة الطبيب الرائع",
                       Image = doctor.info.Icon,
                       Canonical = canonical
                   };
            }
            
           Page_settings(lang, "sa", doctor.city, "doc_profile", tag);


            bool is_chat_activated_for_doctor = true;
            if (doctor.info.Doc_verified != "Y")
                is_chat_activated_for_doctor = false;

            UserInfo UserId = null;
            if (ViewBag.LoggedIn)
            {
                /* USER IS LOGGED IN */

                UserId = new MainApp().GetUserId(Request.Cookies["user"]);
                var user_comment = new MainApp().GetUserComment(id, UserId.Id);
                user_comment.DocId = id;
                doctor.LoggedInUserComment = user_comment;



            }
            else
            {
                doctor.LoggedInUserComment = new Comment
                {
                    Punc = 0,
                    Know = 0,
                    Rrec = 0,
                    Stuf = 0,
                    Text = ""
                };
            }
            if (ViewBag.LoggedIn && doctor.info.Doc_verified=="Y")
            {

               
                if (int.Parse(doctor.UserId)!=UserId.Id)
                {

                    /* user is not the same doctor */
                    doctor.chatTalk = new ChatTalk
                    {
                        DoctorActivated = is_chat_activated_for_doctor,
                        UserAllowed = true,
                        Me = new UserChatTalk
                        {
                            Id= UserId.Id.ToString(),
                            Email ="nmey1999@gmail.com",
                            Name ="User"+UserId.Id.ToString()
                            
                        },
                        Other = new UserChatTalk
                        {
                            Id= doctor.UserId,
                            Email="numay.alghalib@gmail.com",
                            PhotoUrl = doctor.info.Icon,
                            Name = doctor.info.Names[1]
                        }

                    };
                }
                else
                {
                    doctor.chatTalk = new ChatTalk
                    {
                        DoctorActivated = is_chat_activated_for_doctor,
                        UserAllowed = false,

                    };
                }

            }
            else
            {
                doctor.chatTalk = new ChatTalk
                {
                    DoctorActivated = is_chat_activated_for_doctor,
                    UserAllowed = false,

                };
            }
  
            return View(doctor);
        }

        [Route("verify-code/{code}")]
        public IActionResult Verify(string code)
        {

            code = new DataEncrypt().Decrypt(code.Replace('!','+'));
            var resp=new MainApp().VerifyEmail(code);
            if (resp.Code == "000.000.000")
            {
                ViewData["message"] = "http://qdactorymvc-001-site1.itempurl.com/lib/emailverified.PNG";
            }
            else
                ViewData["message"] = "لم يتم توثيق البريد الإلكتروني";
            return View();
        }

        [Route("profile/{lang}")]
        public IActionResult PatProfile(string lang)
        {
            DateTime now = DateTime.Now;
            ViewBag.Today = now.ToString("F", new CultureInfo("ar-AE"));
            ViewBag.SideBar = true;
            UserInfo info = new UserInfo();
            if (Request.Cookies["user"] != null)
            {
                var key = Request.Cookies["user"];
                var usr_data = new MainApp().CheckToken(key);
                if (usr_data.Code == "000.000.000")
                {
                    info = (UserInfo)usr_data.Data;
                    ViewBag.Email =new DataEncrypt().Decrypt(info.Email);
                    ViewBag.LoggedIn = true;
                }
                else
                    ViewBag.LoggedIn = false;
            }
            else
            {
                ViewBag.LoggedIn = false;
            }

            Page_settings(lang, null, null, "user_profile", null);

            if(ViewBag.LoggedIn)
            {
                var prf = new UserProfile
                (
                    new Options
                    {
                        UserId = info.Id,
                        Country = "sa",
                        City = "jeddah",
                        Domain = default_domain,
                        Lang = default_lang,

                    }
                );

                prf.chatTalk = new ChatTalk
                {
                    UserAllowed = true,
                    Me = new UserChatTalk
                    {
                        Id = info.Id.ToString(),
                        Email = "nmey1999@gmail.com",
                        Name = "User" + info.Id.ToString()

                    },
                };
                return View(prf);
            }
            else
            {
                return (Redirect(default_lang + "/" + default_country_city));
            }

            
           
        }

        [Route("{lang}/signup/doctor/{country}-{city}")]
        public IActionResult SignUp(string lang, string country, string city)
        {
            ViewBag.LoggedIn = false;
            ViewBag.IsPatient = false;
            ViewBag.DidNotSubmitAlready = false;
            ViewBag.SideBar = false;

            UserSettings_signup();
            Page_settings(default_lang, country, city, "signup_doctor", null);
            var home = new Home(
                new Options
                {
                    Domain = default_domain,
                    Lang = default_lang,
                    Country = country,
                    City = city,
                    Department = "all",
                    Cities = Get_Cities(country),
                    Departments = GetAllDepartments(),
                    Mode = "on_click"
                });
            if (new MainApp().CheckIfAllowedToSignUpAsDoctor(ViewData["UserId"].ToString()))
            {
                ViewBag.DidNotSubmitAlready = true;
            }
            else
                ViewBag.DidNotSubmitAlready = false;
            home.Ranks = new DoctorEndPoint().GetDoctorRanks(2);

            return View(home);
        }

        [Route("{lang}/signup/institute/{country}-{city}")]
        public IActionResult SignUp_Inst(string lang, string country, string city)
        {
            ViewBag.LoggedIn = false;
            ViewBag.IsPatient = false;
            ViewBag.DidNotSubmitAlready = false;
            ViewBag.SideBar = false;

            UserSettings_signup();
            Page_settings(default_lang, country, city, "signup_doctor", null);
            var home = new Home(
                new Options
                {
                    Domain = default_domain,
                    Lang = default_lang,
                    Country = country,
                    City = city,
                    Department = "all",
                    Cities = Get_Cities(country),
                    Departments = GetAllDepartments(),
                    Mode = "on_click"
                });
            return View(home);
        }

        [Route("{lang}/new/doctor/{country}-{city}")]
        public IActionResult AddingDoctor(string lang, string country, string city)
        {
            ViewBag.LoggedIn = false;
            ViewBag.IsPatient = false;
            ViewBag.SideBar = false;

            UserSettings();
            Page_settings(lang, country, city, "adding_doctor", null);
            var home = new Home(
                new Options
                {
                    Domain = default_domain,
                    Lang = default_lang,
                    Country = country,
                    City = city,
                    Department = "all",
                    Cities = Get_Cities(country),
                    Departments = GetAllDepartments(),
                    Mode = "on_click"
                });
            home.Ranks = new DoctorEndPoint().GetDoctorRanks(2);
            return View(home);
        }

        [Route("{lang}/claim/doctor/{id}")]
        public IActionResult Claim(string lang, int id)
        {
            ViewBag.LoggedIn = false;
            ViewBag.IsPatient = false;
            ViewBag.DidNotSubmitAlready = false;
            ViewBag.SideBar = false;
            if (Request.Cookies["user"] != null)
            {
                var key = Request.Cookies["user"];
                var usr_data = new MainApp().CheckToken(key);
                if (usr_data.Code == "000.000.000")
                {
                    ViewBag.LoggedIn = true;
                    var usr = (UserInfo)usr_data.Data;
                    if (usr.Type == "1" || true)
                    {
                        ViewBag.IsPatient = true;
                        ViewBag.DidNotSubmitAlready = true;
                    }
                    else
                    {
                        ViewBag.IsPatient = false;
                    }
                }
                else
                {
                    ViewBag.LoggedIn = false;
                }


            }
            else
            {
                ViewBag.LoggedIn = false;
                ViewBag.IsPatient = false;
                ViewBag.DidNotSubmitAlready = false;

            }
            ViewBag.DidNotSubmitAlready = true;
            var list_type = "doctor_with_id";

            string country = "sa";
            var ep = new DoctorEndPoint();
            var city = "jeddah";

            UserSettings();
            Page_settings(default_lang, country, city, "signup_doctor", null);
            var home = new Home(
                new Options
                {
                    Domain = default_domain,
                    Lang = default_lang,
                    Country = country,
                    City = city,
                    Department = "all",
                    Listing_type = list_type,
                    DocId = id,
                    Cities = Get_Cities(country),
                    Departments = GetAllDepartments(),
                    Mode = "on_click"
                });
            ViewBag.DocId = id;
            home.Ranks = new DoctorEndPoint().GetDoctorRanks(2);

            return View(home);
        }

        [Route("clinic/{id}/{name}")]
        [Route("{lang}/clinic/{id}/{name}")]
        public IActionResult Institute(string lang, int id, string name)
        {
            if (lang != null && (lang == "ar" || lang == "en"))
            {
                default_lang = lang;

            }
            ViewBag.SideBar = true;
            if (HttpContext.Request.Cookies["user"] != null)
            {
                var key = Request.Cookies["user"];
                var usr_data = new MainApp().CheckToken(key);
                if (usr_data.Code == "000.000.000")
                {
                    ViewBag.LoggedIn = true;
                }
                else
                    ViewBag.LoggedIn = false;
            }
            else
            {
                ViewBag.LoggedIn = false;
            }
            UserSettings();
            var inst = new InstituteProfile(
                new Options
                {
                    Domain = default_domain,
                    Lang = default_lang,
                    Cities = Get_Cities(default_country_city.Split('-')[0]),
                    InstId = id,
                    Departments = GetAllDepartments()
                }
                );

            string canonical = default_domain + "/clinic/" + inst.info.Id + "/" + inst.info.Names[1].Trim();
            if (lang!=null && lang != "ar")
            {
                /* for non arabic languages */
                canonical = default_domain + "/" + default_lang + "/doctor/" + inst.info.Id + "/" + inst.info.Names[1].Trim();
            }
            MetaTags tag;
            if (lang==null || lang=="ar")
            {
                tag =
                   new MetaTags
                   {
                       Title = " ملف عيادة " + inst.info.Names[langId].Trim(),
                       Desc = "شاهد صفحة العيادة الخاصة ب",
                       Image = inst.info.Icon,
                       Canonical = canonical
                   };
            }
            else
            {
                tag =
                   new MetaTags
                   {
                       Title = " ملف عيادة " + inst.info.Names[langId].Trim(),
                       Desc = "شاهد صفحة العيادة الخاصة ب",
                       Image = inst.info.Icon,
                       Canonical = canonical
                   };
            }
            ViewBag.InstId = id;
            Page_settings(lang, "sa", inst.city, "inst_profile", tag);
            return View(inst);
        }

        [Route("clinics/{lang}/{country}-{city}")]
        public IActionResult Institutions(string lang, string country, string city)
        {
            ViewBag.SideBar = true;
            if (HttpContext.Request.Cookies["user"] != null)
            {
                var key = Request.Cookies["user"];
                var usr_data = new MainApp().CheckToken(key);
                if (usr_data.Code == "000.000.000")
                    if (usr_data.Code == "000.000.000")
                {
                    ViewBag.LoggedIn = true;
                }
                else
                    ViewBag.LoggedIn = false;
            }
            else
            {
                ViewBag.LoggedIn = false;
            }
            UserSettings();
            var insts = new InstitutesHomePage(
                new Options
                {
                    Domain = default_domain,
                    Lang = default_lang,
                    Cities = Get_Cities(default_country_city.Split('-')[0]),
                    Country = country,
                    City = city,
                    Departments = GetAllDepartments()
                }
                );

            MetaTags tag;
            if (default_lang == "ar")
            {
                tag =
                   new MetaTags
                   {
                       Title = "جميع العيادات المسجلة في قيم دكتوري",
                       Desc = "يمكنكم حجز موعد مع احدى العيادات, أو تصفح اطباء العيادة ومشاهدة التقييمات",
                       Image = "",
                       Canonical = default_domain + "/clinics/" + lang + "/" + country + "-" + city
                   };
            }
            else
            {
                tag =
                   new MetaTags
                   {
                       Title = "View all institutions",
                       Desc = "bla bla bla",
                       Image = "",
                       Canonical = default_domain + "/clinics/" + lang + "/" + country + "-" + city
                   };
            }

            if (default_country_city.Split('-').Length == 2)
                Page_settings(default_lang, default_country_city.Split('-')[0], default_country_city.Split('-')[1], "inst_profile", tag);
            else
                Page_settings(default_lang, default_country_city.Split('-')[0], "jeddah", "inst_profile", tag);

            return View(insts);
        }

        public IActionResult Contact()
        {
            ViewBag.SideBar = true;
            if (Request.Cookies["user"] != null)
            {
                var key = Request.Cookies["user"];
                var usr_data = new MainApp().CheckToken(key);
                if (usr_data.Code == "000.000.000")
                {
                    ViewBag.LoggedIn = true;
                }
                else
                    ViewBag.LoggedIn = false;
            }
            else
            {
                ViewBag.LoggedIn = false;
            }
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public void Page_settings(string lang, string country, string city, string page, MetaTags tags)
        {
            Set_lang(lang, page);
            Set_page(country, city, tags);
        }
        public bool CheckToken(string token)
        {
            bool is_valid_not_expired = false;
            return is_valid_not_expired;
        }
        public void Set_lang(string lang, string page)
        {

            if (lang != null && (lang == "ar" || lang == "en"))
            {
                default_lang = lang;
                if (lang == "ar")
                    ViewBag.LangId = 1;
                else
                    ViewBag.LangId = 0;
            }
            else
            {
                ViewBag.LangId = 1;
                default_lang = "ar";
            }

            if (page == "home" || true)
            {
                var translation = GetTranslation(null, null, default_lang, "home");
                var json = JObject.Parse(translation);
                ViewBag.Label_institutes = json["Label_institutes"][default_lang];
                ViewBag.Link_more = json["Link_more"][default_lang];
                ViewBag.Label_spec_doctors = json["Label_spec_doctors"][default_lang];
                ViewBag.Label_medical_departments = json["Label_medical_departments"][default_lang];
                ViewBag.Placeholder_search_by_doctor_name = json["Placeholder_search_by_doctor_name"][default_lang];
                ViewBag.Link_are_you_doctor = json["Link_are_you_doctor"][default_lang];
                ViewBag.Label_set_location = json["Label_set_location"][default_lang];

                ViewBag.Label_best_doctors_in = json["Label_best_doctors_in"][default_lang];
                ViewBag.Feedback_not_doctors_in = json["Feedback_not_doctors_in"][default_lang];
                ViewBag.Label_doctors_in = json["Label_doctors_in"][default_lang];
                ViewBag.Label_information_about_dept = json["Label_information_about_dept"][default_lang];
                ViewBag.Button_login = json["Button_login"][default_lang];
                ViewBag.Placeholder_search_by_dep_name = json["Placeholder_search_by_dep_name"][default_lang];
                ViewBag.Button_search_inst = json["Button_search_inst"][default_lang];

                ViewBag.Button_save = json["Button_save"][default_lang];
                ViewBag.Button_close = json["Button_close"][default_lang];
                ViewBag.Placeholder_search_by_inst_name = json["Placeholder_search_by_inst_name"][default_lang];
            }

        }
        public void Set_page(string country, string city, MetaTags tags)
        {
            if(country=="sa" && city=="all")
            {
                ViewBag.Country = country;
                ViewBag.Lang = default_lang;
                ViewBag.City = "all";
                Set_Metatags(Url.Content(Request.QueryString.ToString()), tags);
                ViewBag.CountryAndCity = GetFullCountryName(country, default_lang);
            }
            else
            if (CheckCountryCity(country, city))
            {
                ViewBag.Country = country;
                ViewBag.City = city;
                ViewBag.Lang = default_lang;
                Set_Metatags(Url.Content(Request.QueryString.ToString()), tags);
                ViewBag.CountryAndCity = GetFullCountryName(country, default_lang) + " . " + GetCityName(city, default_lang);
              
            }
            else
            {

                Redirect(default_lang + "/" + default_country_city);
            }

        }
        public string GetTranslation(string country, string city, string lang, string page)
        {
            string path = @"translations.json";
            if (System.IO.File.Exists(path))
            {
                return System.IO.File.ReadAllText(path);

            }
            else
                return null;
        }
        public bool CheckCountryCity(string country, string city)
        {

            bool is_correct = false;
            string path = @"cities_" + country + ".txt";
            if (System.IO.File.Exists(path))
            {

                string[] cities = System.IO.File.ReadAllLines(path);


                if (city != null)
                {
                    foreach (var c in cities)
                    {
                        if (city.ToLower().Equals(c.Replace(' ', '_').ToLower()))
                        {
                            is_correct = true;
                            break;
                        }
                    }
                }
                else
                {
                    is_correct = true;

                }
            }


            return is_correct;
        }
        public string GetAllDepartments()
        {
            string path = @"departmentsJson.json";
            if (System.IO.File.Exists(path))
            {
                return System.IO.File.ReadAllText(path);
            }
            else
                return null;
        }
        public bool CheckDepartment(string department)
        {

            bool is_correct = false;
            string path = @"departments.txt";
            if (System.IO.File.Exists(path))
            {
                string[] cities = System.IO.File.ReadAllLines(path);
                foreach (var c in cities)
                {
                    if (c==department)
                    {
                        is_correct = true;
                        break;
                    }
                }
            }

            return is_correct;
        }
        public string GetCityName(string code, string lang)
        {
            code = code.ToLower();
            if (code == "all")
            {
                if (lang == "ar")
                    return "الكل";
                else
                    return "all";
            }

            string json="";
            string path = @"ar_en_cities_sa.json";
            if (System.IO.File.Exists(path))
            {
               json= System.IO.File.ReadAllText(path);
            }
            dynamic obj_cities = JsonConvert.DeserializeObject(json);
             
            foreach (var j in obj_cities)
            {
                string str = j.en;
                if (code == str.ToLower().Replace(" ", "_").Replace(" ", "_"))
                {
                    return j.ar;
                }
            }
            return "جدة";

                
            //if (code == "all")
            //{
            //    if (lang == "ar")
            //        return "الكل";
            //    else
            //        return "all";
            //}
            //else
            //if (code == "jeddah")
            //{
            //    if (lang == "ar")
            //        return "جدة";
            //    else
            //        return code;
            //}
            //else
            //if (code == "makkah")
            //{
            //    if (lang == "ar")
            //        return "مكة";
            //    else
            //        return code;
            //}
            //else
            //if (code == "riyadh")
            //{
                
            //    if (lang == "ar")
            //        return "الرياض";
            //    else
            //        return code;
            //}
            //else
            //    return code;
        }
        public string GetFullCountryName(string code, string lang)
        {
            if (code == "sa")
            {
                if (lang == "ar")
                    return "المملكة العربية السعودية";
                else
                    return "Kingdom of Saudi Arabia";
            }
            else
            {
                if (lang == "ar")
                    return default_country_city_ar.Split('-')[0];
                else
                    return default_country_city.Split('-')[0];
            }
        }
        public void Set_Metatags(string url, MetaTags default_tag)
        {

            var tags = new MetaTags(url, default_domain, default_tag);
            ViewBag.Title = tags.Title;
            ViewBag.Desc = tags.Desc;
            ViewBag.Keys = tags.Keys;
            ViewBag.Image = tags.Image;
            ViewBag.Robots = tags.Robots;
            ViewBag.Visits = tags.Visits;
            ViewBag.Type = tags.Type;
            ViewBag.Canonical = default_domain + url;
            ViewBag.Url = default_domain + url;

        }
        public string Get_Cities(string country)
        {
            string path = @"ar_en_cities_" + country + ".json";
            if (System.IO.File.Exists(path))
            {
                return System.IO.File.ReadAllText(path);
            }
            else
                return null;
        }
        public void UserSettings()
        {          
            /* USER TOKEN & SEC KEY */
            if (Request.Cookies["user"] != null)
            {
                var key = Request.Cookies["user"];
                var usr_data = new MainApp().CheckToken(key);
                if (usr_data.Code == "000.000.000")
                {
                    ViewBag.LoggedIn = true;
                }
                else
                    ViewBag.LoggedIn = false;
            }
            else
            {
                ViewBag.LoggedIn = false;
            }
        }
        public void UserSettings_signup()
        {
  

            /* LANG AND GEO ENDS  */

            if (Request.Cookies["user"] != null)
            {
                var key = Request.Cookies["user"];
                var usr_data = new MainApp().CheckToken(key);
                if (usr_data.Code == "000.000.000")
                {
                    ViewBag.LoggedIn = true;
                   
                    var usr = (UserInfo)usr_data.Data;
                    ViewData["UserId"] = usr.Id;
                    if (usr.Type == "1")
                    {
                        ViewBag.IsPatient = true;
                    }
                    else
                    {
                        ViewBag.IsPatient = false;
                       
                    }
                }
                else
                    ViewBag.LoggedIn = false;
            }
            else
            {
                ViewBag.LoggedIn = false;
                ViewBag.IsPatient = false;

            }
        }

        /* API CALLS */


        /* REPORT & LIKES to COMMENTS */
        [HttpPost]
        [Route("api/comments/like")]
        public JsonResult LikeComment(int REV_ID,string Token)
        {
            //ReportComment
            int USER_ID = -1;
            MainApp app = new MainApp();

            if (Token != null && REV_ID>0)
            {
                var user_data = app.CheckToken(Token);
                if(user_data.Code=="000.000.000")
                {
                    USER_ID = ((UserInfo)user_data.Data).Id;
                }
                if (USER_ID != -1)
                {
                    app.LikeComment(REV_ID);

                    var resp = new ApiResponse
                    {

                        CODE = "000.000.000",
                        MESSAGE = "success",
                        Data = null

                    };
                    return Json(resp);
                }
                else
                {
                    var resp = new ApiResponse
                    {

                        CODE = "111.110.100",
                        MESSAGE = "error",
                        Data = null

                    };
                    return Json(resp);
                }

            }
            else
            {
                var resp = new ApiResponse
                {

                    CODE = "100.000.000",
                    MESSAGE = "error_token_comment_id",
                    Data = null

                };
                return Json(resp);
            }
        }

        [HttpPost]
        [Route("api/comments/report")]
        public JsonResult ReportComment([FromForm] FormReport Report)
        {
            //ReportComment
            int USER_ID = 342;
            MainApp app = new MainApp();
            if (Report.Subject != null && Report.Content != null)
            {
                app.ReportComment(Report.Id, USER_ID, Report.Subject, Report.Content);
                var resp = new ApiResponse
                {

                    CODE = "000.000.000",
                    MESSAGE = "success",
                    Data = null

                };
                return Json(resp);
            }
            else
            {
                var resp = new ApiResponse
                {

                    CODE = "100.000.000",
                    MESSAGE = "error_subject_content",
                    Data = null

                };
                return Json(resp);
            }
        }

        [HttpGet]
        [Route("api/v1/test")]
        public JsonResult Test()
        {
            var x=new NotificationEndPoint().SendSms("yourCode505 please update", "966567894760");

            var resp = new ApiResponse
            {

                CODE = "000.000.000",
                MESSAGE = x,
                Data = null

            };
            return Json(resp);
        }

        /* GENERAL USER ACCOUNTS */
        [HttpPost]
        [Route("api/members/newaccount")]
        public JsonResult NewAccount([FromForm]FormLoginORSignUp form)
        {
            string code;
            string msg;
            if (form.Mobile == null)
            {
                code = "000.000.100";
                msg = "MOBILE_NOT_PROVIDED";
                form = null;
            }
            else
            if (form.Password == null)
            {
                code = "000.000.100";
                msg = "PASS_NOT_PROVIDED";
                form = null;
            }
            else
            if (form.Email == null)
            {
                code = "000.000.100";
                msg = "EMAIL_NOT_PROVIDED";
                form = null;
            }
            else
            if (false && form.DOB == null)
            {
                code = "000.000.100";
                msg = "DOB_NOT_PROVIDED";
                form = null;
            }
            else
            {
                /* Check if mobile is valid and new and not already exists */
                /* Check if email is valid and new and not already exists */
                /* Create Pre Request with information above  after removing prev pre by that user */
                /* send random code to user (4 digits) */

                var ep = new MainApp();
                return Json(ep.SignUp(form.Email, form.Mobile, form.Password, null, null));

            }



            var resp = new ApiResponse
            {

                CODE = code,
                MESSAGE = msg,
                Data = form

            };
            return Json(resp);
        }

        [HttpPost]
        [Route("api/members/verifyphone")]
        public JsonResult UpdatePatPhone([FromForm]PaTProfile form)
        {
            var ep = new MainApp();
            return Json(ep.UpdatePatPhone(form.PHONE_NUMBER,form.SmsCode,form.Token));
        }

        [HttpPost]
        [Route("api/members/verify/{sms_code}")]
        public JsonResult CompleteSignup([FromForm]FormLoginORSignUp form,string sms_code)
        {
            string code;
            string msg;
            if (form.Mobile == null)
            {
                code = "000.000.100";
                msg = "MOBILE_NOT_PROVIDED";
                form = null;
            }
            else
            if (form.Password == null)
            {
                code = "000.000.100";
                msg = "PASS_NOT_PROVIDED";
                form = null;
            }
            else
            if (form.Email == null)
            {
                code = "000.000.100";
                msg = "EMAIL_NOT_PROVIDED";
                form = null;
            }
            else
            if (false && form.DOB == null)
            {
                code = "000.000.100";
                msg = "DOB_NOT_PROVIDED";
                form = null;
            }
            else
            {
                /* Check if mobile is valid and new and not already exists */
                /* Check if email is valid and new and not already exists */
                /* Create Pre Request with information above  after removing prev pre by that user */
                /* send random code to user (4 digits) */
                var ep = new MainApp();
                return Json(ep.SignUp(form.Email, form.Mobile, form.Password, null, null,sms_code));
            }



            var resp = new ApiResponse
            {

                CODE = code,
                MESSAGE = msg,
                Data = form

            };
            return Json(resp);
        }

        [HttpPost]
        [Route("api/members/login")]
        public JsonResult UserLogin([FromForm]FormLoginORSignUp form)
        {
            string code;
            string msg;
            if (form.Mobile == null && form.Email == null)
            {
                code = "000.000.100";
                msg = "MOBILE_OR_EMAIL_NOT_PROVIDED";
                form = null;
            }
            else
            if (form.Email == null && form.Mobile == null)
            {
                code = "000.000.100";
                msg = "EMAIL_OR_MOBILE_NOT_PROVIDED";
                form = null;
            }
            else
            if (form.Password == null)
            {
                code = "000.000.100";
                msg = "PASS_NOT_PROVIDED";
                form = null;
            }
            else
            {
                if (form.Email != null && form.Password != null)
                {
                    /* USE EMAIL AND PASSWORD TO LOGIN */
                    var ep = new MainApp();
                    return Json(ep.Login(form.Email, null, form.Password));
                }
                else
                if (form.Mobile != null && form.Password != null)
                {
                    /* USE MOBILE AND PASSWORD */
                    var ep = new MainApp();
                    return Json(ep.Login(null, form.Mobile, form.Password));
                }
                else
                {
                    code = "000.000.100";
                    msg = "USERLOGIN_NOT_PROVIDED";
                    form = null;
                }



            }



            var resp = new ApiResponse
            {

                CODE = code,
                MESSAGE = msg,
                Data = form

            };
            return Json(resp);
        }

        [HttpPost]
        [Route("api/members/updateprofile")]
        public JsonResult UpdatePatProfile([FromForm] PaTProfile pat)
        {
            var ep = new MainApp();
            return Json(ep.UpdatePatProfile(pat,pat.Token));
        }

        [HttpPost]
        [Route("api/comments/{doc_rev_id}/update")]
        public JsonResult UpdateDoctorComment([FromForm] Comment comment,int doc_rev_id)
        {
            var ep = new MainApp();
            var resp = ep.CheckToken(comment.Token);
            if (resp.Code == "000.000.000")
            {
                comment.Id = doc_rev_id;
                var doc_ep = new DoctorEndPoint();
                return Json(doc_ep.UpdateDoctorComment(comment));
            }
            else
                return Json(resp);
        }


        [HttpPost]
        [Route("api/comments/add")]
        public JsonResult AddDoctorComment([FromForm] Comment comment)
        {
            var ep = new MainApp();
            var resp = ep.CheckToken(comment.Token);
            if (resp.Code == "000.000.000")
            {
                comment.UserId = ((UserInfo)resp.Data).Id.ToString();
                var doc_ep = new DoctorEndPoint();
                
                return Json(doc_ep.AddComment(comment));
            }
            else
                return Json(resp);
        }


        [HttpPost]
        [Route("api/doctors/add")]
        public JsonResult NewAccount([FromForm]FormNewDoctor form)
        {
            var ep = new MainApp();
            var resp = ep.CheckToken(form.Token);
            if (resp.Code == "000.000.000")
            {
                var doc_ep = new DoctorEndPoint();
                form.UserId = ((UserInfo)resp.Data).Id.ToString();
               
                form.ToR = "NEW";
                return Json(doc_ep.NewDoctor(form));
            }
            else
                return Json(resp);
        }

        [HttpPost]
        [Route("api/doctors/apply")]
        public JsonResult ApplyAs([FromForm]FormNewDoctor form)
        {
            var ep = new MainApp();
            var resp = ep.CheckToken(form.Token);
            if (resp.Code == "000.000.000")
            {
                var doc_ep = new DoctorEndPoint();
                form.UserId = ((UserInfo)resp.Data).Id.ToString();
                if (ep.CheckIfAllowedToSignUpAsDoctor(form.UserId))
                {
                    form.ToR = "NEW";
                    return Json(doc_ep.NewDoctor(form, "NEW"));
                }
                else
                {
                    return Json(new ApiResponse
                    {

                        CODE = "111.111.111",
                        MESSAGE = "NOT_ALLOWED",
                        Data = null

                    });
                }
            }
            else
                return Json(resp);
        }

        [HttpPost]
        [Route("api/doctors/claim")]
        public JsonResult CalaimA([FromForm]FormNewDoctor form)
        {
            var ep = new MainApp();
            var resp = ep.CheckToken(form.Token);
            if (resp.Code == "000.000.000")
            {
                var doc_ep = new DoctorEndPoint();
                form.UserId = ((UserInfo)resp.Data).Id.ToString();
                if (ep.CheckIfAllowedToSignUpAsDoctor(form.UserId))
                {
                    form.ToR = "CLAIM";
                    return Json(doc_ep.NewDoctor(form, "CLAIM"));
                }
                else
                {
                    return Json(new ApiResponse
                    {

                        CODE = "111.111.111",
                        MESSAGE = "NOT_ALLOWED",
                        Data = null

                    });
                }
                
            }
            else
                return Json(resp);
        }

        [HttpPost]
        [Route("api/members/update/phone")]
        public JsonResult UpdatePhone([FromForm]FormLoginORSignUp form)
        {
            string msg;
            string code;

            if (form.Mobile == null && form.Email == null)
            {
                code = "000.000.100";
                msg = "MOBILE_NOT_PROVIDED";
                form = null;
            }
            else
            if (form.NewMobile == null)
            {
                code = "000.000.100";
                msg = "NEW_MOBILE_NOT_PROVIDED";
                form = null;
            }
            else
            if (form.Password == null)
            {
                code = "000.000.100";
                msg = "PASS_NOT_PROVIDED";
                form = null;
            }
            else
            if (form.Email == null && form.Mobile == null)
            {
                code = "000.000.100";
                msg = "EMAIL_NOT_PROVIDED";
                form = null;
            }
            else
            {
                /* Check if email/mobile + and password */
                /* Check if new_mobile is valid and new and not already exists */
                /* update code in user table with new random four digits */
                /* send random code to user (4 digits) to user new phone */

                if (form.Email != null && form.Password != null)
                {
                    var ep = new MainApp();
                    return Json(ep.UpdatePhone(form.Email, null, form.Password, form.NewMobile));
                }
                else
                if (form.Mobile != null && form.Password != null)
                {
                    var ep = new MainApp();
                    return Json(ep.UpdatePhone(null, form.Mobile, form.Password, form.NewMobile));
                }
                else
                {
                    code = "000.000.100";
                    msg = "EMAIL_NOT_PROVIDED";
                    form = null;
                }


            }
            var resp = new ApiResponse
            {

                CODE = code,
                MESSAGE = msg,
                Data = form

            };
            return Json(resp);
        }

        [HttpPost]
        [Route("api/members/reset/password")]
        public JsonResult ResetPassword([FromForm]FormLoginORSignUp form)
        {
            string msg;
            string code;

            if (form.Mobile == null && form.Email == null)
            {
                code = "000.000.100";
                msg = "MOBILE_NOT_PROVIDED";
                form = null;
            }

            else
            if (form.Email == null && form.Mobile == null)
            {
                code = "000.000.100";
                msg = "EMAIL_NOT_PROVIDED";
                form = null;
            }
            else
            if (form.DOB == null)
            {
                code = "000.000.100";
                msg = "DOB_NOT_PROVIDED";
                form = null;
            }
            else
            {
                /* IF MOBILE OR EMAIL IS PROVIDED */
                if (form.Mobile != null)
                {
                    /* USE MOBILE AND DOB SEND SMS TO PHONE WITH NEW PASSWORD */
                    /* CHECK IF MOBILE IS VALID */
                    /* CHECK OF MOBILE AND DOB EXISTS */
                    var ep = new MainApp();
                    return Json(ep.ResetPassword(null, form.Mobile, null, form.DOB));
                }
                else
                if (form.Email != null)
                {
                    /* USE EMAIL AND DOB SEND LINK TO EMAIL WTIH CODE FOR RESETING PASSWORD (EXPIRES 24 hours) */
                    /* CHECK IF EMAIL IS VALID */
                    /* CHECK OF EMAIL AND DOB EXISTS */
                    var ep = new MainApp();
                    return Json(ep.ResetPassword(form.Email, null, null, form.DOB));

                }

                code = "000.000.000";
                msg = "SMS_SENT_TO_USER";

            }



            var resp = new ApiResponse
            {

                CODE = code,
                MESSAGE = msg,
                Data = form

            };
            return Json(resp);
        }

        [HttpPost]
        [Route("api/member/token/validate")]
        public JsonResult ValidateToken([FromForm]FormLoginORSignUp form)
        {
            string msg;
            string code;


            if (form.Token == null)
            {
                code = "000.000.100";
                msg = "TOKEN_NOT_PROVIDED";
                form = null;
            }
            else
            {


                var ep = new MainApp();
                return Json(ep.CheckToken(form.Token));

            }



            var resp = new ApiResponse
            {

                CODE = code,
                MESSAGE = msg,
                Data = form

            };
            return Json(resp);
        }
        /* END GENERAL USER ACCOUNT */

        [HttpGet]
        [Route("api/doctors/loadmore/{lang}/{department}/{country}/{city}/{offset}")]
        public JsonResult LoadMore(string lang, string department, string country, string city, int offset)
        {
            var doc = new DoctorEndPoint();
            var resp = new ApiResponse
            {
                CODE = "000.000.000",
                MESSAGE = "Hello World!",
                Data = doc.GetDoctors(new Options { Listing_type = "doctors_in_given_department_and_city_of_doctor", Department = department, Country = country, City = city, Lang = lang }, offset, 12)

            };
            return Json(resp);
        }
        [HttpGet]
        [Route("api/pat/doctors/loadmore/{lang}/{id}/{offset}")]
        public JsonResult LoadMore_pat_profile(string lang, int id, int offset)
        {
            var doc = new DoctorEndPoint();
            var resp = new ApiResponse
            {
                CODE = "000.000.000",
                MESSAGE = "Hello World!",
                Data = doc.GetDoctors(new Options { Listing_type = "doctors_rated_by_user_id", UserId = id, Lang = lang }, offset, 12)

            };
            return Json(resp);
        }

        [HttpGet]
        [Route("api/doctors/search/{country}/{city}/{lang}/{name}/")]
        public JsonResult SearchDoctor(string country, string city, string name,string lang)
        {
            var doc = new DoctorEndPoint();
            var resp = new ApiResponse
            {
                CODE = "000.000.000",
                MESSAGE = "Hello World!",
                Data = doc.SerchDoctors(new Options { Country = country, City = city, Lang = lang.ToLower() }, name)

            };
            return Json(resp);
        }

        [HttpGet]
        [Route("api/Institute/search/{country}/{city}/{name}")]
        public JsonResult SearchInstitute(string country, string city, string name)
        {
            var inst = new InstituteEndPoint();
            var resp = new ApiResponse
            {
                CODE = "000.000.000",
                MESSAGE = "Hello World!",
                Data = inst.SerchInstitutes(new Options { Country = country, City = city, Lang = "ar" }, name)

            };
            return Json(resp);
        }

        [HttpGet]
        [Route("api/Institute/{id}/doctors/appointment/{lang}")]
        public JsonResult GetInstDoctorsApnt(int id, string lang)
        {
            var doc = new DoctorEndPoint();
            var resp = new ApiResponse
            {
                CODE = "000.000.000",
                MESSAGE = "Hello World!",
                Data = doc.GetDoctors(new Options { Listing_type = "doctors_within_inst", InstId = id, Lang = lang }, 0, 120)

            };
            return Json(resp);
        }

        [HttpGet]
        [Route("api/Institute/{id}/doctors/loadmore/{lang}/{offset}")]
        public JsonResult LoadMore_Inst(string lang, int id, int offset)
        {
            var doc = new DoctorEndPoint();
            var resp = new ApiResponse
            {
                CODE = "000.000.000",
                MESSAGE = "Hello World!",
                Data = doc.GetDoctors(new Options { Listing_type = "doctors_within_inst", InstId = id, Lang = lang }, offset, 12)

            };
            return Json(resp);
        }

        [HttpGet]
        [Route("api/doctors/{docid}/comments/{offset}/{lang}")]
        public JsonResult LoadMore_DocComments(int docid, int offset,string lang)
        {
            var doc = new DoctorEndPoint();
            var resp = new ApiResponse
            {
                CODE = "000.000.000",
                MESSAGE = "Hello World!",
                Data = doc.GetComments(new Options { Listing_type = "comments_by_doc_id", DocId = docid, Lang=lang }, offset, 6)

            };
            return Json(resp);
        }

        /* ENDS API CALLS */


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

   
}


