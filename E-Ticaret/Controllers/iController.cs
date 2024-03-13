using E_Ticaret.DB;
using E_Ticaret.Models.i;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Ticaret.Controllers
{
    public class iController : BaseController
    {
        // GET: i

        [HttpGet]
        public ActionResult Index(int id = 0)
        {
            //context.Members.ToList();
            IQueryable<DB.Products> products = context.Products;
            DB.Categories category = null;
            if (id > 0)
            {
                products = products.Where(x => x.Category_Id == id);
                category = context.Categories.FirstOrDefault(x => x.Id == id);
            }

            var viewsModel = new Models.i.indexModel()
            {
                Products = products.ToList(),
                Category = category

            };
            return View(viewsModel);
        }
        [HttpGet]
        public ActionResult Product(int id = 0)
        {
            var pro = context.Products.FirstOrDefault(x => x.Id == id);
            if (pro == null)
            {
                return RedirectToAction("index", "i");
            }
            ProductModels model = new ProductModels()
            {
                Product = pro,
                Comments = pro.Comments.ToList()
            };
            return View(model);
        }
        [HttpPost]
        public ActionResult Product(DB.Comments yorum)
        {
            try
            {
                //TODO:Test alanı gereklidir
                yorum.Member_Id = base.GetCurrentUserId();
                yorum.AddedDate = DateTime.Now;
                context.Comments.Add(yorum);
                context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        // Hata ayrıntılarını işleyin, örneğin:
                        ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                    }
                }

                ViewBag.MyError = ex.Message;
            }
            return RedirectToAction("Product", "i");
        }
        //public ActionResult Product(DB.Comments comment)
        //{
        //    try
        //    {
        //        comment=new DB.Comments();
        //        int i=base.GetCurrentUserId();
        //        comment.Member_Id = i;
        //        comment.AddedDate = DateTime.Now;
        //        context.Comments.Add(comment);
        //        context.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.MyError=ex.Message;

        //    }
        //    return RedirectToAction("Product", "i");
        //}
    }
}