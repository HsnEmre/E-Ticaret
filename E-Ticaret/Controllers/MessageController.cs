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
            if (IsLogon() == false)
            {
                return RedirectToAction("index", "i");
            }

            Models.Messages.iModels model = new Models.Messages.iModels();
            #region Select List Item
            model.Users = new List<SelectListItem>();
            var currentId = GetCurrentUserId();
            var users = context.Members.Where(x => ((int)x.MemberType) > 0 && x.Id != currentId).ToList();
            model.Users = users.Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = string.Format("{0} {1} ({2})", x.Name, x.Surname, x.MemberType.ToString())

            }).ToList();

            #endregion
            #region Mesaj Listesi,
            var mList = context.Messages.Where(x => x.ToMemberId == currentId && x.MessageReplies.Any(y => y.Member_Id == currentId)).ToList();
            model.Messages = mList;
            #endregion
            return View(model);
        }
        public ActionResult SendMessage(Models.Messages.SendMessageModel message)
        {
            if (IsLogon() == false)
            {
                return RedirectToAction("index", "i");
            }

            DB.Messages mesaj = new DB.Messages()
            {
                Id = Guid.NewGuid(),
                AddedDate = DateTime.Now,
                IsRead = false,
                Subject = message.Subject,
                ToMemberId = message.ToUserId
            };
            var mRep = new DB.MessageReplies()
            {
                Id = Guid.NewGuid(),
                AddedDate = DateTime.Now,
                Member_Id = GetCurrentUserId(),
                Text = message.MessageBody
            };

            mesaj.MessageReplies.Add(mRep);

            context.Messages.Add(mesaj);
            context.SaveChanges();

            return RedirectToAction("i", "message");
        }
    }
}