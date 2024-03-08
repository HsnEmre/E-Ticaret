using E_Ticaret.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Ticaret.Controllers
{
    public class iController : Controller
    {
        // GET: i
        EticaretEntities context;
        public iController()
        {
            context = new EticaretEntities();
            ViewBag.MenuCategories = context.Categories.Where(x=>x.Parent_Id==null).ToList();
        }
        public ActionResult Index(int id=0)
        {
            //context.Members.ToList();
            IQueryable<DB.Products> products = context.Products;
            DB.Categories category = null;
            if (id>0)
            {
                products = products.Where(x => x.Category_Id == id);
                category = context.Categories.FirstOrDefault(x => x.Id == id);
            }

            var viewsModel = new Models.i.indexModel()
            {
                Products = products.ToList(),
                Category=category
                
            };
            return View(viewsModel);
        }
    }
}