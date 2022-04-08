using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Intex2022.Models;
using Intex2022.Models.ViewModels;

namespace Intex2022.Controllers
{
    public class HomeController : Controller
    {
        private CrashDbContext _context { get; set; }
        private InferenceSession _session;
        private SignInManager<IdentityUser> signInManager;

        public HomeController(CrashDbContext context, InferenceSession session, SignInManager<IdentityUser> sim)
        {
            _context = context;
            _session = session;
            signInManager = sim;
        }


        //------------------ Landing Page ------------------//
        public IActionResult Index()
        {
            return View();
        }



        //------------------ PUBLIC READ LIST ------------------//
        public IActionResult AllList(string countySelect, int pageNum = 1)
        //public IActionResult CrashDetailsList(DateTime crashDate, int pageNum = 1)

        {
            var x = new CrashesViewModel
            {
                Crashes = _context.Crashes
                .OrderBy(p => p.County_Name),
                //.Skip((pageNum - 1) * pageSize)
                //.Take(pageSize),

            };

            return View(x);
        }




        //~~~~~~~~~~~~~~~ ADMIN FUNCTIONS ~~~~~~~~~~~~~~~//

        //------------------ Read List ------------------//

        [Authorize]
        public IActionResult CrashDetailsList(string countySelect)

        {
            //figure out filtering?
            var x = new CrashesViewModel
            {
                Crashes = _context.Crashes
                .Where(p => p.County_Name == countySelect || countySelect == null)
                .OrderBy(p => p.County_Name),
                //.Skip((pageNum - 1) * pageSize)
                //.Take(pageSize),

                PageInfo = new PageInfo
                {
                    TotalNumCrashes =
                    (countySelect == null ? _context.Crashes.Count()
                    : _context.Crashes.Where(p => p.County_Name == countySelect).Count()),
                    //TotalNumCrashes = _context.Crashes.Count(),
                    //CrashesPerPage = pageSize,
                    //CurrentPage = pageNum //idk
                }
            };

            return View(x);
        }


        //------------------ Add ------------------//
        [Authorize(Roles = "Administrator, test55")]
        [HttpGet]
        public IActionResult CreateCrashForm()
        {
            if (!User.IsInRole("Administrator"))
            {
                return View("Error");
            }
            else
            {
                ViewBag.Crashes = _context.Crashes
                    .Select(c => c.City)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();

                return View();
            }
        }

        [Authorize(Roles = "Administrator, test55")]
        [HttpPost]
        public IActionResult CreateCrash([FromForm] Crash crash)
        {
            if (!User.IsInRole("Administrator"))
            {
                return View("Error");
            }
            else
            {


                int newCrashID = _context.Crashes.OrderBy(x => x.CRASH_ID).Last().CRASH_ID;
                crash.CRASH_ID = newCrashID + 1;
                crash.Crash_Date = crash.Crash_Date.Date;
                _context.Add(crash);
                _context.SaveChanges();
                return RedirectToAction("CrashDetailsList");
            }
        }

        //------------------ Edit/Update ------------------//
        [Authorize(Roles = "Administrator, test55")]
        [HttpGet]
        [Route("/Home/UpdateCrashForm/{id}")]
        public IActionResult UpdateCrashForm(int id)
        {
            ViewBag.Crashes = _context.Crashes
                .Select(c => c.City)
                .Distinct()
                .OrderBy(c => c)
                .ToList();
            Crash c = _context.Crashes.FirstOrDefault(c => c.CRASH_ID == id);
            return View(c);
        }

        [Authorize(Roles = "Administrator, test55")]
        [HttpPost]
        public IActionResult UpdateCrash([FromForm] Crash crash)
        {
            crash.Crash_Date = crash.Crash_Date.Date;
            _context.Update(crash);
            _context.SaveChanges();
            return RedirectToAction("CrashDetailsList");
        }

        IQueryable<Crash> Crashes => _context.Crashes;


        //---------------- Delete Confirmation / Deletion -----------------//
        [Authorize]
        [Route("/Home/DeleteConfirmation/{id}")]
        public IActionResult DeleteConfirmation(int id)
        {
            Crash crash = _context.Crashes.FirstOrDefault(c => c.CRASH_ID == id);
            return View("DeleteConfirmation", crash);
        }
        [Authorize(Roles= "Administrator, test55")]
        [Route("/Home/DeleteCrash/{id}")]
        public IActionResult DeleteCrash(int id)
        {
            Crash c = _context.Crashes.FirstOrDefault(c => c.CRASH_ID == id);
            _context.Crashes.Remove(c);
            _context.SaveChanges();
            return RedirectToAction("CrashDetailsList");
        }

        [HttpGet]
        public IActionResult Calculator()
        {
            return View();
        }

        //------------- Privacy Policy -----------//
        public IActionResult PrivacyPolicy()
        {
            return View();
        }

        //---------------- Predictor -----------------//
        [HttpPost]
        public IActionResult Calculator(crashData data)
        {
            var result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", data.AsTensor())
            });
            Tensor<float> score = result.First().AsTensor<float>();
            var prediction = new Prediction { PredictedValue = score.First() };
            result.Dispose();
            // Rounding to nearest integer for predictor and setting a limit on how high the predictor can calculate
            if (prediction.PredictedValue >= 4.5)
            {
                prediction.PredictedValue = 5;
            }
            else if (prediction.PredictedValue >= 3.5)
            {
                prediction.PredictedValue = 4;
            }
            else if (prediction.PredictedValue >= 2.5)
            {
                prediction.PredictedValue = 3;
            }
            else if (prediction.PredictedValue >= 1.5)
            {
                prediction.PredictedValue = 2;
            }
            else if (prediction.PredictedValue >= .5)
            {
                prediction.PredictedValue = 1;
            }
            return RedirectToAction("Calculation", prediction);
        }

        //---------------- Display Predictor Results -----------------//
        public IActionResult Calculation(Prediction prediction)
        {
            return View(prediction);
        }



    }
}