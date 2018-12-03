using CampusChat.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace CampusChat.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private CampusChatDatabaseEntities db = new CampusChatDatabaseEntities();

        // GET: Messages
        public ActionResult Index()
        {
            //only want to show messages in data where reciverID == UserID
            string user = User.Identity.GetUserId();
            var messages = db.Messages.Where(x => x.RecieverID == user).OrderBy(o => o.SentTime);
          
            return View(messages.ToList());
        }

        //Will Get only show Send Mesages 
        public ActionResult ViewMessage()
        {
            // shows messages send from user only
            string user = User.Identity.GetUserId();
            var message = db.Messages.Where(m => m.SenderID == user).OrderBy(o => o.SentTime);


            //return View(messages.ToList());
            return View(message);
        }

        // GET: Messages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // GET: Messages/Create
        public ActionResult Create()
        {
            ViewBag.SenderID = new SelectList(db.AspNetUsers, "Id", "UserName");
            ViewBag.RecieverID = new SelectList(db.AspNetUsers, "Id", "UserName");
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MessageID,SenderID,RecieverID,Content,SentTime")] Message message)
        {
            if (ModelState.IsValid)
            {
                Message newMessage = new Message();

                //should get the name/username of the current user and store it as the SenderID in the table
                newMessage.SenderID = User.Identity.GetUserId();
                newMessage.RecieverID = message.RecieverID;
                newMessage.Content = message.Content;
                newMessage.SentTime = DateTime.Now;
              
                db.Messages.Add(newMessage);
                db.SaveChanges();

                return RedirectToAction("Index"); 
            }

            ViewBag.SenderID = new SelectList(db.AspNetUsers, "Id", "UserName", message.SenderID);
            ViewBag.RecieverID = new SelectList(db.AspNetUsers, "Id", "UserName", message.RecieverID);
            return View(message);
        }

        // GET: Messages/Reply
        public ActionResult Edit(int? id) 
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //while(db.Messages ) where i would put algorithm to display all prev messages from both
 
            //algorithm
            //String user = User.Identity.GetUserId();
            //var message = db.Messages.Where(o => user == o.RecieverID && o.SenderID == user).OrderBy(o => o.SentTime);
            Message message = db.Messages.Find(id);

            if (message == null)
            {
                return HttpNotFound();
            }

           // ViewBag.SenderID = new SelectList(db.AspNetUsers, "Id", "Email", message.SenderID);
            //ViewBag.RecieverID = new SelectList(db.AspNetUsers, "Id", "Email", message.RecieverID);
            return View(message);
        }

        // POST: Messages/Reply
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, string content)
        {
            Message replyMessage = new Message();
            if (ModelState.IsValid)
            {

                Message originalMessage = db.Messages.Find(id);
                replyMessage.SenderID = User.Identity.GetUserId();
                replyMessage.RecieverID = originalMessage.SenderID;
                replyMessage.SentTime = DateTime.Now;
                replyMessage.Content = content;
                db.Messages.Add(replyMessage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SenderID = new SelectList(db.AspNetUsers, "Id", "Email", replyMessage.SenderID);
            ViewBag.RecieverID = new SelectList(db.AspNetUsers, "Id", "Email", replyMessage.RecieverID);
            return View(replyMessage);
        }

        public ActionResult Reply(int id, string content)
        {
            Message originalMessage = db.Messages.Find(id);
            Message replyMessage = new Message();
            replyMessage.SenderID = User.Identity.GetUserId();
            replyMessage.RecieverID = originalMessage.SenderID;
            replyMessage.SentTime = DateTime.Now;
            replyMessage.Content = content;
            db.Messages.Add(replyMessage);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //test for reply post method
        public ActionResult ReplyTest(int? id)
        {
            Message message = db.Messages.Find(id);
            return View("Reply", message);
        }

        // GET: Messages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           
            Message message = db.Messages.Find(id);


            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Message message = db.Messages.Find(id);
            db.Messages.Remove(message);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult SentMessages()
        {
            string userID = User.Identity.GetUserId();
            var sentMessages = db.Messages.Where(o => o.SenderID == userID);
            return View("SentMessages", sentMessages.ToList());
        }
    }
}
