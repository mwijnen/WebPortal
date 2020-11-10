using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebPortal.Controllers
{
    public class AccountsController : Controller
    {
        private UserManager<IdentityUser> userManager;

        private readonly SignInManager<IdentityUser> signInManager;

        public AccountsController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        //Register (navigate to form and initialize empty registration)

        //Register (handle form submission, create new user, redirect)

        //Verify email action

        //Login (navigate to form)

        //Login (handle form submission, create new session, redirect)

        //Forgot password (navigate to form)

        //Forgot password (handle form submission, redirect)

        //Reset password (navigate to form)

        //Reset password (handle form submission, redirect)

        //Logout
    }
}
