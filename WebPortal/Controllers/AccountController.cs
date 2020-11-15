using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.Services;
using WebPortal.Models.Account;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace WebPortal.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<IdentityUser> _userManager;

        private SignInManager<IdentityUser> _signInManager;

        private IEmailServer _emailServer;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailServer emailServer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailServer = emailServer;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View("Register", new Registration());
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(Registration registrationParams)
        {
            if (registrationParams.Password != registrationParams.RetypedPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords don't match");
                return View("Register", registrationParams);
            }

            IdentityUser newUser = new IdentityUser
            {
                UserName = registrationParams.Email,
                Email = registrationParams.Email
            };

            var userCreationResult = await _userManager.CreateAsync(newUser, registrationParams.Password);
            if (userCreationResult.Succeeded)
            {
                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                var webEncodedConfirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationToken));
                var callbackUrl = Url.ActionLink(action: "VerifyEmail",
                    controller: "Account",
                    values: new { id = newUser.Id, token = confirmationToken });
                await _emailServer.SendEmailAsync(newUser.Email, "Email confirmation", callbackUrl);
                //Send email with confirmation link and token
                //Redirect to useful page
            }
            else
            {
                foreach (var error in userCreationResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("Register", registrationParams);
            }

            //await _userManager.AddClaimAsync(newUser, new Claim(ClaimTypes.Role, "IdentityUser"));

            //string emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

            //string tokenVerificationUrl = Url.Action("VerifyEmail", "Registrations", new { id = newUser.Id, token = emailConfirmationToken }, Request.Scheme);

            ////
            //LocalEmailClient.SendEmail("email confirmation", "Account", "VerifyEmail", tokenVerificationUrl);

            //await _messageService.Send(email, "Verify your email", $"Click <a href=\"{tokenVerificationUrl}\">here</a> to verify your email");

            //return Content("Check your email for a verification link");

            //return RedirectToAction(actionName: "Index", controllerName: "Users");

            return RedirectToAction(actionName: "Index", controllerName: "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string id, string token)
        {
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new InvalidOperationException();

            var emailConfirmationResult = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (!emailConfirmationResult.Succeeded)
                return Content(emailConfirmationResult.Errors.Select(error => error.Description).Aggregate((allErrors, error) => allErrors += ", " + error));

            //return Content("Email confirmed, you can now log in");
            return RedirectToAction(actionName: "Index", controllerName: "Users");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string redirectUrl = null)
        {
            return View("Login", new Session() { RedirectUrl = redirectUrl });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(Session session)
        {
            var user = await _userManager.FindByEmailAsync(session.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login");
                return View("Login", session);
            }
            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Confirm your email first");
                return View("Login");
            }

            SignInResult passwordSignInResult = await _signInManager.PasswordSignInAsync(user,
                session.Password, isPersistent: session.RememberMe, lockoutOnFailure: false);

            if (!passwordSignInResult.Succeeded)
            {
                await _userManager.AccessFailedAsync(user);
                ModelState.AddModelError(string.Empty, "Invalid login");
                return View("Login", session);
            }

            if (session.RedirectUrl != null)
            {
                return Redirect(session.RedirectUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        //Forgot password (navigate to form)

        //Forgot password (handle form submission, redirect)

        //Reset password (navigate to form)

        //Reset password (handle form submission, redirect)

    }
}
