using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Career.Data;
using Career.Models;

namespace Career.Controllers
{
    /// <summary>
    /// The controller responsiple for all the user operations,, login, register, logout, changing passward, etc ...
    /// </summary>
    [Authorize]
    public class AccountController : Controller
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
        public AccountController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            mContext = context;
            mUserManager = userManager;
            mSignInManager = signInManager;

        }


        /// <summary>
        /// The call to go to the login page
        /// </summary>
        /// <returns> Return the login page </returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            // If the user is signed in
            if (mSignInManager.IsSignedIn(User))

                // Let him back in according to his type
                return RedirectToAction("SelectType");

            // Else, gives the login view
            return View();
        }
        
        /// <summary>
        /// The action to log the user in 
        /// </summary>
        /// <param name="model"> Login details </param>
        /// <returns> Redirect to the appropriate view according to the sign in result </returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // sign out from any previous account
            if(mSignInManager.IsSignedIn(User))
            {
                // Let him back in according to his type
                return RedirectToAction("SelectType");
            }
            // Validating the view model
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Getting the user name from the email, to be used in signing in
            var user = await mUserManager.FindByEmailAsync(model.Email);

            // If the email is not in the database, 
            if(user == null)
            {
                // Display error message
                ModelState.AddModelError("", "Email and Passward doesn't match");

                // Let the user try again
                return View(model);
            }

            // Try to sign in 
            var result = await mSignInManager.PasswordSignInAsync (user.UserName, model.Password, model.RememberMe, false);

            // If the sign in succeed
            if(result.Succeeded)
            {
                // Check if this user is the admin,
                var Ad = await mUserManager.IsInRoleAsync(user, "Admin");

                // If admin
                if(Ad)
                    // Redirect to the admin controller
                    return RedirectToAction("Index","Admin");
                
                // If not admin redirect to the select type view
                return RedirectToAction("SelectType");
            }

            // If the user is locked
            if(result.IsLockedOut)
            {
                // TODO: lockedout
                return View("Lockout");
            }

            // If the sign in failed
            ModelState.AddModelError("", "Email and Passward doesn't match");

            // Let the user try again 
            return View(model);
        }
        
        /// <summary>
        /// The call to go to the Register page
        /// </summary>
        /// <returns> Return the register page </returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            // If the user is signed in
            if (mSignInManager.IsSignedIn(User))

                // Let him back in according to his type
                return RedirectToAction("SelectType");

            // Else, give the register view
            return View();
        }

        /// <summary>
        /// The action to register the user
        /// </summary>
        /// <param name="model"> Register details </param>
        /// <returns> Redirect to the appropriate view according to the register result </returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Validating the view model
            if (ModelState.IsValid)
            {
                // Making a user
                var appUser = new ApplicationUser { UserName = model.UserName, Email = model.Email, IsVerified = false };

                // Try to register
                var registerResult = await mUserManager.CreateAsync(appUser, model.Password);

                // If succeeded 
                if (registerResult.Succeeded)
                {
                    // Add the general role (User)
                    await mUserManager.AddToRoleAsync(appUser, "User");

                    // And sign in this user
                    await mSignInManager.SignInAsync(appUser, isPersistent: false);

                    // TODO : 
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    // Redirect to the select type view
                    return RedirectToAction("SelectType");
                }
                // If the register failed
                foreach (var error in registerResult.Errors)
                {
                    // Add each error to display them on the view
                    ModelState.AddModelError("",error.Description);
                }
                
            }

            // Let the user know his faults and try again
            return View(model);
        }

        /// <summary>
        /// Log out from the logged in account
        /// </summary>
        /// <returns> Return the login page </returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task <IActionResult> Logout()
        {
            // Making sure that there is user signed in 
            if(mSignInManager.IsSignedIn(User))
            {
                // Sign out
                await mSignInManager.SignOutAsync();
            }

            // TODO: if the home page was made available for outsider, redirect to there... 
            return Redirect("Login");
        }

        /// <summary>
        /// To select if the user is a student or an employer
        /// </summary>
        /// <returns> Return the view to make the user select which type </returns>
        [HttpGet]
        [Authorize(Roles ="User")]
        public async Task<IActionResult> SelectType()
        {
            // Get the current user
            var user = await mUserManager.GetUserAsync(User);

            // Check if already have (Student) role
            var St = await mUserManager.IsInRoleAsync(user, "Student");

            // If yes
            if (St)
            {
                // Redirect to the student controller
                return RedirectToAction("Index", "Student");
            }

            // Also check if already have (Employer) role
            var Em = await mUserManager.IsInRoleAsync(user, "Employer");

            // If yes
            if (Em)
            {
                // Redirect to the employer controller
                return RedirectToAction("Index", "Employer");
            }

            // If doesn't have (Student) or (Employer) role, give the view to the user to decide
            return View();
        }

        /// <summary>
        /// To select if the user is a student or an employer
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> SelectType(UserType userType)
        {
            
            // Get the current user
            var user = await mUserManager.GetUserAsync(User);

            // Check if already have (Student) role
            var St = await mUserManager.IsInRoleAsync(user, "Student");

            // If yes
            if (St)
            {
                // Redirect to the student controller
                return RedirectToAction("Index", "Student");
            }

            // Also check if already have (Employer) role
            var Em = await mUserManager.IsInRoleAsync(user, "Employer");

            // If yes
            if (Em)
            {
                // Redirect to the employer controller
                return RedirectToAction("Index", "Employer");
            }

            // See what is the user choice
            switch (userType)
            {
                // In case the user selected student
                case UserType.Student:
                    // Give the user the (Student) role
                    await mUserManager.AddToRoleAsync(user,"Student");

                    // Re-sign him in, as giving the role may sign him out
                    await mSignInManager.SignInAsync(user,false);

                    // Redireect to the student controller
                    return RedirectToAction("Index", "Student");

                // In case the user selected employer
                case UserType.Employer:
                    // Give the user the (Employer) role
                    await mUserManager.AddToRoleAsync(user, "Employer");

                    // Re-sign him in, as giving the role may sign him out
                    await mSignInManager.SignInAsync(user, false);

                    // Redireect to the employer controller
                    return RedirectToAction("Index", "Employer");
            }

            // If nothing from before worked, let the user try again
            return View();
        }
               

    }
}