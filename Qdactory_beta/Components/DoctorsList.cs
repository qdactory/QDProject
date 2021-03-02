using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qdactory_beta.Components
{
    public class DoctorsList: ViewComponent
    {
        public DoctorsList(){}
       public IViewComponentResult Invoke(List<DoctorCard> list)
        {
            
            return View("default",list);
        }
       

    }
}
