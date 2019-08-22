using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Roster.Models;
using Roster.RosterDLL;
using RosterSystem.Repository;

namespace Roster.Controllers
{ 
    public class AccountController : Controller
    {
        RosterEntities db = new RosterEntities();
        public GenericUnitOfWork UnitOfWork = new GenericUnitOfWork();
        private readonly RosterEntities _entities = new RosterEntities();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        RegisterViewModel ObjRejister = new RegisterViewModel();

        // GET: Account
        //public ActionResult Index()
        //{
        //    return View();
        //}
        //public ActionResult Login()
        //{
        //    return View();
        //}
        //public ActionResult EmployeeDetails()
        //{
        //    return View();
        //}
        public AccountController()
        {
        }

        public ActionResult RegisterUser()
        {
            ObjRejister.rolelist = _entities.SP_RoleList().ToList();
            ObjRejister.ObjUserList = _entities.SP_UserList().ToList();
            // return View(ObjRejister);
            return View("RegisterUser", ObjRejister);
        }
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterUser(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, Lastname = model.LastName, User_Role = model.user_role };
                if (model.Id == null)
                {
                    var password1 = model.FirstName + '@' + 1234;
                    var result = await UserManager.CreateAsync(user, password1);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        TempData["msg3"] = "<script language='javascript' type='text/javascript'>alert('User Registered Successfully!');</script>";
                    }
                }
                else
                {
                    var user1 = UserManager.FindById(model.Id);
                    user1.FirstName = model.FirstName;
                    user1.Lastname = model.LastName;
                    user1.User_Role = model.user_role;
                    user1.Email = model.Email;
                    user1.UserName = model.Email;
                    var result = await UserManager.UpdateAsync(user1);
                    if (result.Succeeded)
                    {
                        TempData["msg3"] = "<script language='javascript' type='text/javascript'>alert('Changes Saved Successfully.');</script>";
                    }
                    AddErrors(result);
                }


                //AddErrors(result);
            }
            model.rolelist = _entities.SP_RoleList().ToList();
            model.ObjUserList = _entities.SP_UserList().ToList();
            return View(model);
        }
        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            TempData["msg"] = "";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            Session["AspLoginEmailID"] = model.UserName;
            var AspLoginDetails = db.SP_LoginUserDetails(Convert.ToString(Session["AspLoginEmailID"])).ToList();
            if (AspLoginDetails.Count>0)
            {
                var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        Session["UserName"] = Convert.ToString(model.UserName);
                        Session["MemberRole"] = AspLoginDetails[0].User_Role.ToString();
                        return RedirectToLocal(returnUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(model);
                }
            }
            else
            {
                //TempData["msg"] = "<script language='javascript' type='text/javascript'>alert('User Not Valid!');</script>";
                TempData["msg"] = "User is Not Valid!";
                return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    
                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

   

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
     
        public ActionResult LogOff()
        {
            if (!string.IsNullOrEmpty(Convert.ToString(Session["UserName"])))
            {
                Session["UserName"] = "";
                Response.Cookies["MemberRole"].Value = "";
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetNoStore();

            HttpCookie Cookies = new HttpCookie("WebTime");
            Cookies.Value = "";
            Cookies.Expires = DateTime.Now.AddHours(-1);
            Response.Cookies.Add(Cookies);
            HttpContext.Session.Clear();
            Session.Abandon();
            //
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        public ActionResult GetUserId(string UserId)
        {
            try
            {
                RegisterViewModel objRegisterDataModel = new RegisterViewModel();
                var usersActivities = UnitOfWork.GetRepositoryInstance<SP_GetLoginDtsByRegisterUser_Result>().GetResultBySqlProcedure("SP_GetLoginDtsByRegisterUser @ID ", new System.Data.SqlClient.SqlParameter("ID", SqlDbType.VarChar) { Value = UserId }).FirstOrDefault();
                if (usersActivities != null && usersActivities.id != null)
                {
                    objRegisterDataModel.FirstName = usersActivities.FirstName;
                    objRegisterDataModel.LastName = usersActivities.LastName;
                    objRegisterDataModel.Email = usersActivities.Email;
                    objRegisterDataModel.user_role = usersActivities.user_role;
                    //objRegisterDataModel.rolelist = new SelectList(UnitOfWork.GetRepositoryInstance<user_roles>().GetAllRecordsIQueryable().Where(i => i.is_active == true ), "user_role", "user_role_description");
                    objRegisterDataModel.rolelist = _entities.SP_RoleList().ToList();
                    objRegisterDataModel.Id = usersActivities.id;
                }
                objRegisterDataModel.ObjUserList = _entities.SP_UserList().ToList();
                return View("RegisterUser", objRegisterDataModel);
            }
            catch (Exception ex)
            {
                return RedirectToAction("RegisterUser", "Account");
            }
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            return Redirect("/Roster/index");
            //if (Url.IsLocalUrl(returnUrl))
            //{
            //    return Redirect(returnUrl);
            //}
            //return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

  
       [AllowAnonymous]
       public ActionResult ForgotPassword()
        {
            return View();
        }
        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
                var user = await userManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Email", "Please enter valid Email ID!");
                    return View(model);
                }
                var Passwordexists = await UserManager.HasPasswordAsync(user.Id);
                if (!Passwordexists)
                {
                    return RedirectToAction("ForgotPassword", "Account");
                }
                await userManager.RemovePasswordAsync(user.Id);
                await userManager.AddPasswordAsync(user.Id, model.Password);
                ModelState.AddModelError("Email", "Your Password is Successfull !");
                return View(model);
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }
    }
}