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
    [Authorize]
    public class PostsController : Controller
    {
        private CampusChatDatabaseEntities db = new CampusChatDatabaseEntities();

        // GET: Posts
        public ActionResult Index()
        {
            var posts = db.Posts.Include(p => p.AspNetUser).Include(p => p.Category);
            posts = db.Posts.OrderBy(o => (DbFunctions.DiffHours(o.PostedTime, DateTime.Now))/(o.Upvotes + 1));
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
            Post newPost = new Post();
            if (ModelState.IsValid)
            {
                newPost.UserID = User.Identity.GetUserId();
                newPost.PostedTime = DateTime.Now;
                newPost.Upvotes = 0;
                newPost.Downvotes = 0;
                newPost.Content = post.Content;
                newPost.CategoryID = post.CategoryID;
                newPost.Title = post.Title;
                db.Posts.Add(newPost);
                db.SaveChanges();
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
                Post newPost = post;
                newPost.CategoryID = post.CategoryID;
                newPost.Title = post.Title;
                newPost.Content = post.Content;
                db.Posts.Find(post.PostID).CategoryID = newPost.CategoryID;
                db.Posts.Find(post.PostID).Title = newPost.Title;
                db.Posts.Find(post.PostID).Content = newPost.Content;
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

        public ActionResult Upvote(int? id)
        {
            var post = db.Posts.Find(id);
            post.Upvotes++;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Downvote(int? id)
        {
            var post = db.Posts.Find(id);
            post.Downvotes++;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Sort(string sortOption)
        {
            var posts = db.Posts.Include(p => p.AspNetUser).Include(p => p.Category);
            if(sortOption == "New")
            {
                posts = db.Posts.OrderByDescending(o => o.PostedTime);
            }
            else if(sortOption == "Top")
            {
                posts = db.Posts.OrderByDescending(o => (o.Upvotes/(o.Downvotes + 1)));
            }
            else if(sortOption == "Hot")
            {
                posts = db.Posts.OrderBy(o => (DbFunctions.DiffHours(o.PostedTime, DateTime.Now))/(o.Upvotes + 1));
            }
            return View("Index", posts.ToList());
        }

        [HttpPost]
        public ActionResult Search(string searchTerm)
        {
            return View("Search", db.Posts.Where(p => p.Title.Contains(searchTerm) || searchTerm == null).ToList());
        }
    }
}
