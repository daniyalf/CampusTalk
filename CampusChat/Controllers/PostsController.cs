using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CampusChat.Models;
using Microsoft.AspNet.Identity;

namespace CampusChat.Controllers
{
    public class PostsController : Controller
    {
        private CampusChatDatabaseEntities db = new CampusChatDatabaseEntities();

        // GET: Posts
        public ActionResult Index()
        {
            var posts = db.Posts.Include(p => p.AspNetUser).Include(p => p.Category);
            return View(posts.ToList());
        }

        // GET: Posts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // GET: Posts/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Content,CategoryID,Title")] Post post)
        {
            string userID = User.Identity.GetUserId();
            Post newPost = new Post();
            newPost.UserID = userID;
            newPost.PostedTime = DateTime.Now;
            newPost.Upvotes = 0;
            newPost.Downvotes = 0;
            newPost.Title = post.Title;
            newPost.Content = post.Content;
            newPost.CategoryID = post.CategoryID;
            newPost.PostID = post.PostID;
            if (ModelState.IsValid)
            {
                db.Posts.Add(newPost);
                try {
                    db.SaveChanges();
                }
                catch(DbEntityValidationException e)
                {
                    foreach(var eve in e.EntityValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("Entity of type \" {0} \"in StateChangeEventArgs \" {1} \" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach(var ve in eve.ValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine("- Property: \" {0} \", Error: \"{1}\" ", ve.PropertyName, ve.ErrorMessage);
                        }

                    }
                    throw;
                }
                return RedirectToAction("Index");
            }
            return View(newPost);
        }

        // GET: Posts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", post.UserID);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", post.CategoryID);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Post post)
        {
            if (ModelState.IsValid)
            {
                var currentPost = db.Posts.FirstOrDefault(o => o.PostID == post.PostID);
                currentPost.Content = post.Content;
                currentPost.Title = post.Title;
                currentPost.CategoryID = post.CategoryID;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", post.UserID);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", post.CategoryID);
            return View(post);
        }

        // GET: Posts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
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

        public ActionResult Upvote(int id)
        {
            Post post = db.Posts.Find(id);
            post.Upvotes++;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Downvote(int id)
        {
            Post post = db.Posts.Find(id);
            post.Downvotes++;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
