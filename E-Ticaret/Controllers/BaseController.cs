﻿using E_Ticaret.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Ticaret.Controllers
{
    public class BaseController : Controller
    {
        protected EticaretEntities context { get; private set; }
        public BaseController()
        {
            context = new EticaretEntities();
            ViewBag.MenuCategories = context.Categories.Where(x => x.Parent_Id == null).ToList();
           
        }

    }
}