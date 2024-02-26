using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Career.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Career.Controllers
{
    /// <summary>
    /// The student controller 
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
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
        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            mContext = context;
            mUserManager = userManager;
            mSignInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}