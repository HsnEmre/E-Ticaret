using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Ticaret.Controllers
{
    public class MessageController : BaseController
    {
        // GET: Message
        public ActionResult i()
        {
            Models.Messages.iModels model = new Models.Messages.iModels();
            model.Users = new List<SelectListItem>();
            var users = context.Members.Where(x => ((int)x.MemberType) > 0).ToList();
            model.Users = users.Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = string.Format("{0} {1} ({2})", x.Name, x.Surname, x.MemberType.ToString())

            }).ToList();
            return View(model);
        }
        public ActionResult SendMessage(DB.Messages message)
        {
            return View();
        }
    }
}