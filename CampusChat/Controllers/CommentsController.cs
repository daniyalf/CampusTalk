using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CampusChat.Models;
using Microsoft.AspNet.Identity;

namespace CampusChat.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private CampusChatDatabaseEntities db = new CampusChatDatabaseEntities();

        // GET: Comments
        public ActionResult Index()
        {
            var comments = db.Comments.Include(c => c.AspNetUser).Include(c => c.Post);
            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        public ActionResult Create(int? id)
        {
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.PostID = new SelectList(db.Posts, "PostID", "UserID");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int? id, [Bind(Include = "CommentID,UserID,Content,PostedTime,Rating,ParentID,PostID")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                Comment newComment = new Comment();
                newComment.UserID = User.Identity.GetUserId();
                newComment.Content = comment.Content;
                newComment.PostedTime = DateTime.Now;
                newComment.Rating = 0;
                newComment.PostID = db.Posts.Find(id).PostID;
                String postIDParameter = newComment.PostID.ToString();
                newComment.ParentID = comment.ParentID;
                db.Comments.Add(newComment);
                db.SaveChanges();
                return RedirectToAction("Details", "Posts", new { id = newComment.PostID });
            }

            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", comment.UserID);
            ViewBag.PostID = new SelectList(db.Posts, "PostID", "UserID", comment.PostID);
            return View(comment);
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", comment.UserID);
            ViewBag.PostID = new SelectList(db.Posts, "PostID", "UserID", comment.PostID);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CommentID,UserID,Content,PostedTime,Rating,ParentID,PostID")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", comment.UserID);
            ViewBag.PostID = new SelectList(db.Posts, "PostID", "UserID", comment.PostID);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
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
    }
}
