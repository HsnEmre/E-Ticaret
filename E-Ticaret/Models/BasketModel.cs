﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Ticaret.Models
{
    public class BasketModel
    {
        public BasketModel()
        {
            this.Products = new Dictionary<int, int>();
        }
        public Dictionary<int,int> Products { get; set; }
    }
}