using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Ticaret.Models.Messages
{
    public class RenderMessageModel
    {
        public List<DB.Messages> Messages { get; set; }
        public int Count { get; set; }
    }
}