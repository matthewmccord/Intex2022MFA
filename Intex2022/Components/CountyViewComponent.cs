using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intex2022.Models;

namespace Intex2022.Components
{
    public class CountyViewComponent : ViewComponent
    {
        private ICrashRepository _context { get; set; }

        public CountyViewComponent(ICrashRepository temp)
        {
            _context = temp;
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedCounty = RouteData?.Values["countySelect"];

            var county = _context.Crashes.Select(x => x.County_Name).Distinct().OrderBy(x => x);


            return View(county);
        }
    }
}
