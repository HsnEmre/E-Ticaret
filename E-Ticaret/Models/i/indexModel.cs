using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Ticaret.Models.i
{
    public class indexModel
    {
        public List<DB.Products> Products { get; set; }
        public DB.Categories Category { get; set; }
    }
}