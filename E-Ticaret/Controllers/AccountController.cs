using E_Ticaret.DB;
using E_Ticaret.Models.Account;
using System;
using System.Collections.Generic;
using System.IO;
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

                user.Member.MemberType = DB.Membertypess.Customer;
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
        public ActionResult Profil(int id = 0, string ad = "")
        {
            List<DB.Addresses> addresses = null;
            DB.Addresses currentAddress = new DB.Addresses();
            if (id == 0)
            {
                id = base.GetCurrentUserId();
                addresses = context.Addresses.Where(x => x.Member_Id == id).ToList();
                if (!string.IsNullOrEmpty(ad))
                {
                    var guid = new Guid(ad);
                    currentAddress = context.Addresses.FirstOrDefault(x => x.Id == guid);
                }
            }
            var user = context.Members.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return RedirectToAction("index", "i");
            }
            ProfileModels model = new ProfileModels()
            {
                Members = user,
                Addresses = addresses,
                CurrentAddres = currentAddress
            };
            return View(model);
        }
        [HttpGet]
        public ActionResult ProfileEdit()
        {
            int id = base.GetCurrentUserId();

            var user = context.Members.FirstOrDefault(x => x.Id == id);
            if (user == null) return RedirectToAction("index", "i");
            ProfileModels model = new ProfileModels()
            {
                Members = user
            };
            return View(model);
        }
        [HttpPost]
        public ActionResult ProfileEdit(ProfileModels model)
        {
            try
            {
                int id = GetCurrentUserId();
                var updateMember = context.Members.FirstOrDefault(x => x.Id == id);
                updateMember.ModifiedDate = DateTime.Now;
                updateMember.Bio = model.Members.Bio;
                updateMember.Name = model.Members.Name;
                updateMember.Surname = model.Members.Surname;

                if (string.IsNullOrEmpty(model.Members.Password) == false)
                {
                    updateMember.Password = model.Members.Password;
                }
                if (Request.Files != null && Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file.ContentLength > 0)
                    {
                        var folder = Server.MapPath("~/İmages/Upload");
                        var fileName = Guid.NewGuid() + ".jpg";
                        file.SaveAs(Path.Combine(folder, fileName));

                        var filePath = "images/upload/" + fileName;
                        updateMember.ProfileImageName = filePath;
                    }
                }
                context.SaveChanges();

                return RedirectToAction("Profil", "Account");
            }
            catch (Exception ex)
            {
                ViewBag.MyError = ex.Message;
                int id = GetCurrentUserId();
                var viewModel = new Models.Account.ProfileModels()
                {
                    Members = context.Members.FirstOrDefault(x => x.Id == id)
                };
                return View(viewModel);
            }
            //try
            //{
            //    int id=GetCurrentUserId();
            //    var updateMember = context.Members.FirstOrDefault(x => x.Id == id);
            //    updateMember.ModifiedDate = DateTime.Now;
            //    updateMember.Bio = model.Members.Bio;
            //    updateMember.Name = model.Members.Name;
            //    updateMember.Surname = model.Members.Surname;


            //    if (string.IsNullOrEmpty(model.Members.Password) == false)
            //    {
            //        updateMember.Password = model.Members.Password;
            //    }
            //    if (Request.Files != null && Request.Files.Count > 0)
            //    {
            //        var file = Request.Files[0];
            //        var folder = Server.MapPath("~/Images/Upload/");
            //        var filename = Guid.NewGuid() + ".jpg";

            //        file.SaveAs(Path.Combine(folder, filename));

            //        var filepath = "images/upload/" + filename;
            //        updateMember.ProfileImageName = filepath;
            //    }

            //    context.SaveChanges();


            //    return RedirectToAction("Profil", "Account");
            //}
            //catch (Exception ex)
            //{
            //    ViewBag.MyError = ex.Message;
            //    int id = GetCurrentUserId();
            //    var viewModel = new Models.Account.ProfileModels()
            //    {
            //        Members = context.Members.FirstOrDefault(x => x.Id == id)
            //    };
            //    return View(viewModel);
            //}
        }


        [HttpPost]
        public ActionResult Addresses(DB.Addresses adres)
        {
            DB.Addresses _addresses = null;

            if (adres.Id == Guid.Empty)
            {
                adres.Id = Guid.NewGuid();
                adres.AddedDate = DateTime.Now;
                adres.Member_Id = base.GetCurrentUserId();
                context.Addresses.Add(adres);
            }
            else
            {
                _addresses = context.Addresses.FirstOrDefault(x => x.Id == adres.Id);
                _addresses.ModifiedDate = DateTime.Now;
                _addresses.Name = adres.Name;
                _addresses.AdresDescription = adres.AdresDescription;

            }
            context.SaveChanges();
            return RedirectToAction("Profil", "Account");
        }

        [HttpGet]
        public ActionResult RemoveAdres(string id)
        {
            var guid = new Guid(id);
            var address = context.Addresses.FirstOrDefault(x => x.Id == guid);
            context.Addresses.Remove(address);
            context.SaveChanges();

            return RedirectToAction("Profil", "Account");

        }
    }
}