using E_Ticaret.DB;
using E_Ticaret.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Ticaret.Controllers
{
    public class AccountController : BaseController
    {
        // GET: Account

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Models.Account.RegisterModels user)
        {
            try
            {
                if (user.rePassword != user.Member.Password)
                {
                    throw new Exception("Şifreler aynı değildir!");
                }
                if (context.Members.Any(x => x.Email == user.Member.Email))
                {
                    throw new Exception("Bu e-mail adresi zaten kayıtlıdır!");
                }

                user.Member.MemberType = DB.Membertypes.Customer;
                user.Member.AddedDate = DateTime.Now;
                context.Members.Add(user.Member);
                context.SaveChanges();
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                ViewBag.ReError = ex.Message;
                return View();
            }

        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Models.Account.LoginModels model)
        {

            try
            {

                var user = context.Members.FirstOrDefault(x => x.Password == model.Member.Password && x.Email == model.Member.Email);
                if (user != null)
                {
                    Session["LogonUser"] = user;
                    return RedirectToAction("index", "i");
                }
                else
                {
                    ViewBag.ReError = "Kullanıcı Bilgileriniz yanlış";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }
        public ActionResult LogOut()
        {
            Session["LogonUser"] = null;
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        public ActionResult Profil( int id=0)
        {
            if (id == 0)
            {
                id=base.GetCurrentUserId();
            }


            var user = context.Members.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return RedirectToAction("index", "i");
            }
            ProfileModels model = new ProfileModels()
            {
                Members = user
            };
            return View(model);
        }
    }
}