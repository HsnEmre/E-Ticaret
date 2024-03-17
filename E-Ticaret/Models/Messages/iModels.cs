using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Ticaret.Models.Messages
{
    public class iModels
    {
        public List<SelectListItem> Users { get; set; }
        public List<DB.Messages> Messages { get; set; }
    }
}