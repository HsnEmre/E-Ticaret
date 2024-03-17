using E_Ticaret.Models.Messages;
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
        [HttpGet]
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
            var mList = context.Messages.Where(x => x.ToMemberId == currentId || x.MessageReplies.Any(y => y.Member_Id == currentId)).ToList();
            model.Messages = mList;
            #endregion
            return View(model);
        }
        //[HttpGet]
        [HttpPost]
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

        [HttpGet]
        public ActionResult MessageReplies(string id)
        {
            var guid = new Guid(id);

            if (IsLogon() == false)
            {
                return RedirectToAction("index", "i");
            }
            var currentid = GetCurrentUserId();

            DB.Messages messages = context.Messages.FirstOrDefault(x => x.Id == guid);
            if (messages.ToMemberId == currentid)
            {
                messages.IsRead = true;
                context.SaveChanges();
            }

            MessageRepliesModel model = new MessageRepliesModel();

            model.MessageReplies = context.MessageReplies.Where(x => x.MessageId == guid).OrderBy(x => x.AddedDate).ToList();

            return View(model);
        }
        [HttpPost]
        public ActionResult MessageReplies(DB.MessageReplies message)
        {
            if (IsLogon() == false) return RedirectToAction("index", "i");



            message.AddedDate = DateTime.Now;
            message.Id = Guid.NewGuid();
            message.Member_Id = GetCurrentUserId();
            context.MessageReplies.Add(message);
            context.SaveChanges();

            return RedirectToAction("MessageReplies", "Message", new { id = message.MessageId });
        }

        [HttpGet]
        public ActionResult RenderMessage()
        {
            RenderMessageModel model = new RenderMessageModel();
            var currentId = GetCurrentUserId();
            var mList = context.Messages.Where(x => x.ToMemberId == currentId || x.MessageReplies.Any(y => y.Member_Id == currentId)).OrderBy(x => x.AddedDate);
            model.Messages = mList.Take(5).ToList();
            model.Count = mList.Count();
            return PartialView("_Message", model);
        }

        public ActionResult RemoveMessageReplies(string id)
        {
            var guid = new Guid(id);
            var mrelies = context.MessageReplies.Where(x => x.MessageId == guid);

            context.MessageReplies.RemoveRange(mrelies);

            var message = context.Messages.FirstOrDefault(x => x.Id == guid);
            context.Messages.Remove(message);
            context.SaveChanges();

            return RedirectToAction("i", "Message");
        }
    }
}