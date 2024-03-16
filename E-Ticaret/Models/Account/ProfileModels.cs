using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Ticaret.Models.Account
{
    public class ProfileModels
    {
        public DB.Members Members { get; set; }
        public List<DB.Addresses> Addresses { get; set; }
    }

}