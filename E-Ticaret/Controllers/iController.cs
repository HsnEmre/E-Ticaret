using E_Ticaret.DB;
using E_Ticaret.Models;
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
        [HttpGet]
        public ActionResult AddBasket(int id, bool remove = false)
        {
            List<Models.i.BasketModel> basket = null;
            if (Session["Basket"] == null)
            {
                basket = new List<Models.i.BasketModel>();
            }
            else
            {
                basket = (List<Models.i.BasketModel>)Session["Basket"];
            }

            if (basket.Any(x => x.Product.Id == id))
            {
                var pro = basket.FirstOrDefault(x => x.Product.Id == id);
                if (remove && pro.Count > 0)
                {
                    pro.Count -= 1;


                }
                else
                {
                    if (pro.Product.UnitsInStock > pro.Count)
                    {
                        pro.Count += 1;
                    }
                    else
                    {
                        TempData["MyError"] = "Yeterli Stok Yok";
                    }
                }

            }
            else
            {
                var pro = context.Products.FirstOrDefault(x => x.Id == id);
                if (pro != null && pro.IsContinued && pro.UnitsInStock > 0)
                {
                    basket.Add(new Models.i.BasketModel()
                    {
                        Count = 1,
                        Product = pro,
                    });
                }
                else if (pro.IsContinued == false && pro != null)
                {
                    TempData["MyError"] = "Bu ürünün satışı durduruldu";
                }

            }
            basket.RemoveAll(x => x.Count < 1);
            Session["Basket"] = basket;

            return RedirectToAction("Basket", "i");
        }

        [HttpGet]
        public ActionResult Basket()
        {
            List<Models.i.BasketModel> model = (List<Models.i.BasketModel>)Session["Basket"];
            if (model == null)
            {
                model = new List<Models.i.BasketModel>();
            }
            if (base.IsLogon())
            {
                int currentId = GetCurrentUserId();

                ViewBag.currentAddresses = context.Addresses.Where(x => x.Member_Id == currentId).Select(x => new
                SelectListItem()
                {
                    Text=x.Name,
                    Value=x.Id.ToString()
                }).ToList();
            }
            ViewBag.TotalPrice = model.Select(x => x.Product.Price * x.Count).Sum();


            return View(model);
        }

        [HttpGet]
        public ActionResult RemoveBasket(int id)
        {
            List<Models.i.BasketModel> basket = (List<Models.i.BasketModel>)Session["Basket"];
            if (basket != null)
            {
                if (id > 0)
                {
                    basket.RemoveAll(x => x.Product.Id == id);
                }
                else if (id == 0)
                {
                    basket.Clear();
                }
                Session["Basket"] = basket;
            }
            return RedirectToAction("Basket", "i");
        }

        [HttpGet]
        public ActionResult Buy()
        {
            if (base.IsLogon())
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

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