using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using coreWeb31.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace coreWeb31.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");//到登入後的第一頁，自行決定
            }

            return View();
        }

        [HttpPost]
        #region 一般登入
        public async Task<IActionResult> Login(LoginViewModel loginViewModel, string ReturnUrl)
        {
            if(!ModelState.IsValid)
            {
                return View(loginViewModel);
            }

            if(loginViewModel.Account != "a123456" && loginViewModel.Password != "a123456")
            {
                ModelState.AddModelError("", "帳號或密碼錯誤!!!");
                return View(loginViewModel);
            }

            List<Claim> claims = new List<Claim>();
            //使用者ID
            claims.Add(new Claim(ClaimTypes.NameIdentifier, loginViewModel.Account));
            //使用者名稱
            claims.Add(new Claim(ClaimTypes.Name, "測試用戶"));
            //使用者mail
            claims.Add(new Claim(ClaimTypes.Email, "test@gmail.com"));
            //建立證件，類似你的駕照或護照
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            //將 ClaimsIdentity 設定給 ClaimsPrincipal (持有者) 
            ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);//導到原始要求網址
            }
            else
            {
                return RedirectToAction("Index", "Home");//到登入後的第一頁，自行決定
            }
        }
        #endregion

        #region google登入寫法
        /// <summary>
        /// 不做處理直接導到登入前的頁面
        /// </summary>
        /// <param name="ReturnUrl"></param>
        /// <returns></returns>
        //public async Task GoogleLogin(string ReturnUrl)
        //{
        //    // 要設定要導出去的路徑
        //    await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
        //    {
        //        RedirectUri = new PathString(ReturnUrl)
        //    });
        //}


        // 這種寫法會去看 Url.Action去判斷要轉到哪個頁面
        public IActionResult GoogleLogin(string ReturnUrl)
        {
            // 這裡的做法是登入google之後還要做而外處理使用到的

            //1. Challenge
            //var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleLoginBack") };
            //return Challenge(properties, GoogleDefaults.AuthenticationScheme);

            //2. ChallengeResult 加上有登入前的網址
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleLoginBack", new { ReturnUrl }) };
            return new ChallengeResult(GoogleDefaults.AuthenticationScheme, properties);

            //3. ChallengeResult的properties 直接寫在裡面
            //return new ChallengeResult(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties
            //{
            //    RedirectUri = Url.Action(nameof(GoogleLoginBack), new { ReturnUrl })
            //});
        }

        public async Task<IActionResult> GoogleLoginBack(string ReturnUrl)
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
                return BadRequest(); // TODO: Handle this better.

            //List<Claim> claims = new List<Claim>();

            ////使用者ID
            //claims.Add(authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier));
            ////使用者名稱
            //claims.Add(authenticateResult.Principal.FindFirst(ClaimTypes.Name));
            ////使用者mail
            //claims.Add(authenticateResult.Principal.FindFirst(ClaimTypes.Email));

            //var claimsIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            //claimsIdentity.AddClaims(claims);


            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);//導到原始要求網址
            }
            else
            {
                return RedirectToAction("Index", "Home");//到登入後的第一頁，自行決定
            }
        }
        #endregion

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
