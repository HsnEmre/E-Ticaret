using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Ticaret.Controllers
{
    public class ProductController : BaseController
    {
        // GET: Product
        public ActionResult i()
        {
            if (IsLogon() == false) return RedirectToAction("index", "i");
            else if ((int)(CurrentUser().MemberType) < 4)
            {
                return RedirectToAction("i", "Product");
            }

            var products = context.Products.ToList();

            return View(products);
        }
    }
}