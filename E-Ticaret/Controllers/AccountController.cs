using E_Ticaret.DB;
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
                if (user.rePassword !=user.Member.Password)
                {
                    throw new Exception("Şifreler aynı değildir!");
                }

                user.Member.MemberType = DB.Membertypes.Customer;
                user.Member.AddedDate = DateTime.Now;
                context.Members.Add(user.Member);
                context.SaveChanges();
                return RedirectToAction("Login","Account");
            }
            catch (Exception ex)
            {
                ViewBag.ReError = ex.Message;
                return View();
            }
           
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult LogOut()
        {
            return View();
        }
        public ActionResult Profil()
        {
            return View();
        }
    }
}