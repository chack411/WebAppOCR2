using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAppOCR.Models;

namespace WebAppOCR.Controllers
{
    public class HomeController : Controller
    {
        public string eventName = "VS Code Meetup #2 - Live Share";

        public IActionResult Index()
        {
            var names = new List<string>();

            foreach (var item in names)
            {
                
            }

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Sample Application for .NET Standard, .NET Core and Cognitive Services.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // GET: Home/Ocr
        public ActionResult Ocr()
        {
            return View();
        }

        // POST: Home/Ocr
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ocr(Image imageData)
        {
            try
            {
                Uri image = new Uri(imageData.ImageUrl);

                // Setup for OcrLib
                // 1) Go to https://www.microsoft.com/cognitive-services/en-us/computer-vision-api 
                //    Sign up for computer vision api
                // 2) Add environment variable "Vision_API_Subscription_Key" and set Computer vision key as value
                //    e.g. Vision_API_Subscription_Key=123456789abcdefghijklmnopqrstuvw

                Task<string> task = Task.Run(() => ChackLib.OcrLib.DoOcr(image));
                task.Wait();

                imageData.Result = task.Result;

                return View("OcrView", imageData);
            }
            catch
            {
                return View();
            }
        }

        // GET: Home/Noodle
        public ActionResult Noodle()
        {
            return View();
        }

        // POST: Home/Noodle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Noodle(Image imageData)
        {
            try
            {
                Uri imageUrl = new Uri(imageData.ImageUrl);

                Task<ChackLib.Prediction> task = Task.Run(() => ChackLib.NoodleFinder.Noodle(imageUrl));
                task.Wait();

                imageData.Result = ChackLib.NoodleFinder.ConvertToText(task.Result);

                return View("NoodleView", imageData);
            }
            catch
            {
                return View();
            }
        }
    }
}
