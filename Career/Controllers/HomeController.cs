using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Career.Data;
using Career.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Career.Controllers
{
    /// <summary>
    /// The home controller 
    /// </summary>
    [Authorize(Roles = "User")]
    public class HomeController : Controller
    {
        #region Protected Members

        /// <summary>
        /// The scoped Application context
        /// </summary>
        protected ApplicationDbContext mContext;

        /// <summary>
        /// The manager for handling user creation, deletion, searching, roles etc...
        /// </summary>
        protected UserManager<ApplicationUser> mUserManager;

        /// <summary>
        /// The manager for handling signing in and out for our users
        /// </summary>
        protected SignInManager<ApplicationUser> mSignInManager;

        #endregion


        /// <summary>
        /// The default constructor
        /// </summary>
        /// <param name="context"> The injected context </param>
        /// <param name="userManager"> The injected User manager </param>
        /// <param name="signInManager"> The injected Sign in manager </param>
        public HomeController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            mContext = context;
            mUserManager = userManager;
            mSignInManager = signInManager;
        }

        /// <summary>
        /// The Home page default view
        /// </summary>
        /// <returns></returns>
        //[AllowAnonymous]
        public IActionResult Index()
        {
            //return Content($"Welcome {HttpContext.User.Identity.Name} {HttpContext.User.IsInRole("administrator")}", "text/html");
            return View();
        }
        
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

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
    }
}
