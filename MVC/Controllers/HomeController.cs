using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MVC.Models.Captcha;
using MVC.Models.Examples;
using System;
using System.Diagnostics;
using System.IO;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        IHttpContextAccessor _accesor;
        public HomeController(IHttpContextAccessor accesor)
        {
            _accesor = accesor;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult Login()
        {
            ViewModelForLogin wm = new ViewModelForLogin();
            return View(wm);
        }

        [HttpPost]
        public IActionResult LoginPost(ViewModelForLogin viewModel)
        {
            ViewModelForLogin wm = new ViewModelForLogin();
            string IP = _accesor.HttpContext.Connection.RemoteIpAddress.ToString();
            ICache ip = new RedisCache();
            ICache user = new RedisCache();
            TimeSpan span = new TimeSpan(24, 0, 0);
            TimeSpan span2 = new TimeSpan(0, 5, 0);

            //Captcha control
            if(viewModel.CaptchaCode != null)
            {
                if(!Captcha.ValidateCaptchaCode(viewModel.CaptchaCode, HttpContext))
                {
                    wm.ErrorMessage = "Captcha Hatalı";
                    wm.ipCount = 10;
                    wm.userCount = 4;
                    return View("Login", wm);
                }
                else
                {
                    wm.ipCount = 1;
                    wm.userCount = 1;
                    return View("Login", wm);
                }
            }

            if (ip.Exists(IP))
            {
                var model = ip.Get<IP>(IP);
                ip.Delete(IP);
                model.request += 1;
                ip.Set<IP>(IP, model, DateTime.Now);

                wm.ipCount = model.request;

                if(DateTime.Now - model.time < span && model.request > 9)
                {
                    wm.ipCount = model.request;
                    wm.ipMessage = "IP 24 saatte 10 küsür kez geldi";
                }
                else
                {
                    wm.ipCount = 1;
                }
            }
            else
            {
                IP _ip = new IP()
                {
                    ip = IP,
                    time = DateTime.Now,
                    request = 1
                };
                ip.Set<IP>(IP, _ip, DateTime.Now);

                wm.ipCount = 1;
            }


            if (user.Exists(viewModel.userName))
            {
                var model = user.Get<Minute>(viewModel.userName);
                user.Delete(viewModel.userName);
                model.request += 1;
                user.Set<Minute>(viewModel.userName, model, DateTime.Now);

                

                if (DateTime.Now - model.time < span2 && model.request > 2)
                {
                    wm.userCount = model.request;
                    wm.userMessage = "UserID 5 dakika içinde 3 küsür kez geldi";
                }
                else
                {
                    wm.userCount = 1;
                }
            }
            else
            {
                Minute _m = new Minute()
                {
                    userId = viewModel.userName,
                    time = DateTime.Now,
                    request = 1
                };
                user.Set<Minute>(viewModel.userName, _m, DateTime.Now);

                wm.userCount = 1;
            }
            
            return View("Login", wm);
        }

        [Route("get-captcha-image")]
        public IActionResult GetCaptchaImage()
        {
            int width = 100;
            int height = 36;
            var captchaCode = Captcha.GenerateCaptchaCode();
            var result = Captcha.GenerateCaptchaImage(width, height, captchaCode);
            HttpContext.Session.SetString("CaptchaCode", result.CaptchaCode);
            Stream s = new MemoryStream(result.CaptchaByteData);
            return new FileStreamResult(s, "image/png");
        }
    }
}
