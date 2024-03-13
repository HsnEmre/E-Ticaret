using E_Ticaret.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Ticaret.Models.i
{
    public class BasketModel
    {
        public DB.Products Product { get; set; }
        public int Count { get; set; }
    }
}