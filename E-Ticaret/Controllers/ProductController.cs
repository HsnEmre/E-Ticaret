using System;
using System.Collections.Generic;
using System.IO;
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
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var product = context.Products.FirstOrDefault(x => x.Id == id);

            var categories = context.Categories.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
            }).ToList(); ;
            ViewBag.categories = categories;

            return View(product);
        }
        [HttpPost]
        public ActionResult Edit(DB.Products products)
        {
            var productImagePath = string.Empty;
            products.ProductImageName = string.Empty;

            if (Request.Files != null && Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file.ContentLength > 0)
                {
                    var folder = Server.MapPath("~/İmages/Upload/Product");
                    var fileName = Guid.NewGuid() + ".jpg";
                    file.SaveAs(Path.Combine(folder, fileName));

                    var filePath = "İmages/Uppload/Product/" + fileName;
                    productImagePath = filePath;
                }
            }

            if (products.Id > 0)
            {

                var dbProduct = context.Products.FirstOrDefault(x => x.Id == products.Id);

                dbProduct.Category_Id = products.Category_Id;
                dbProduct.ModifiedDate = products.ModifiedDate;
                dbProduct.Description = products.Description;
                dbProduct.IsContinued = products.IsContinued;
                dbProduct.Name = products.Name;
                dbProduct.Price = products.Price;
                dbProduct.UnitsInStock = products.UnitsInStock;
                if (string.IsNullOrEmpty(productImagePath) == false)
                {
                    dbProduct.ProductImageName = productImagePath;
                }
            }
            else
            {
                products.AddedDate = DateTime.Now;

                products.ProductImageName = productImagePath;

                context.Entry(products).State = System.Data.Entity.EntityState.Added;
            }




            context.SaveChanges();


            return RedirectToAction("i");
        }

        public ActionResult Delete(int id)
        {
            var pro = context.Products.FirstOrDefault(x => x.Id == id);
            pro.IsDeleted = true;
            context.SaveChanges();

            return RedirectToAction("i");
        }
    }
}