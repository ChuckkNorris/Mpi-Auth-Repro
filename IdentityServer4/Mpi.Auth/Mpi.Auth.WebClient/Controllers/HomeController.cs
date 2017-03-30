using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IdentityServer4.Test;
using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Authorization;
using IdentityServer4;
using IdentityServer4.Stores;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Mpi.Auth.WebClient.Controllers
{
    public class HomeController : Controller
    {
       
        public HomeController() { 
          
        }
        public IActionResult Index() {
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Login() {
        //    Console.WriteLine($"{vm.Username} : {vm.Password}");
        //    if (_users.ValidateCredentials(vm.Username, vm.Password)) {
        //        var user = _users.FindByUsername(vm.Username);
        //        AuthenticationProperties props = null;
        //        await HttpContext.Authentication.SignInAsync(user.SubjectId, user.Username, props);
        //        return Redirect("~/Home/contact");
        //    }
            
        //    return Redirect("~/Home/about");
        //}

        public async Task Logout() {
            await HttpContext.Authentication.SignOutAsync("Cookies");
            await HttpContext.Authentication.SignOutAsync("oidc");
        }

        public async Task<IActionResult> CallApi() {
            var accessToken = await HttpContext.Authentication.GetTokenAsync("access_token");
            Console.WriteLine($"Access Token: {accessToken}");
            var client = new HttpClient();
            client.SetBearerToken(accessToken);
            var content = await client.GetStringAsync("http://localhost:5001/identity");

            ViewBag.Json = JArray.Parse(content).ToString();
            return View("json");
        }

        [Authorize(Policy = "EmployeeOnly")]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [Authorize]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }

    
}
