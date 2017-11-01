﻿using System.Linq;
using System.Web.Mvc;
using SlarkInc.DAL;
using System;
using PagedList;
using SlarkInc.Models;
using System.Data;
using System.Net;
using System.Data.Entity;

namespace SlarkInc.Controllers
{
    public class CompanyController : Controller
    {
        private CompanyContext db = new CompanyContext();

        //Index
        public ViewResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.FirstNameSortParm = String.IsNullOrEmpty(sortOrder) ? "first_desc" : "";
            ViewBag.LastNameSortParm = sortOrder == "last" ? "last_desc" : "last";
            if(searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var workers = from w in db.Workers
                        select w;
            if (!string.IsNullOrEmpty(searchString))
            {
                workers = workers.Where(w => w.FirstName.Contains(searchString)
                                        || w.LastName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "first_desc":
                    workers = workers.OrderByDescending(w => w.FirstName);
                    break;
                case "last_desc":
                    workers = workers.OrderByDescending(w => w.LastName);
                    break;
                case "last":
                    workers = workers.OrderBy(w => w.LastName);
                    break;
                default:
                    workers = workers.OrderBy(w => w.FirstName);
                    break;
            }
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(workers.ToPagedList(pageNumber,pageSize));
        }

        //Create Get
        public ViewResult Create()
        {
            return View();
        }

        //Create Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FirstName, LastName, Sex, Rating")] Worker worker)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Workers.Add(worker);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                ModelState.AddModelError("","Unable to save changes.Try again, and if the problem persists see your system administrator.");
            }
            return View(worker);
        }

        //Edit Get
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Worker worker = db.Workers.Find(id);
            if (worker == null)
            {
                return HttpNotFound();
            }
            return View(worker);
        }

        //Edit Post
[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult Edit([Bind(Include = "ID, FirstName, LastName, Sex, Rating")] Worker worker)
{
    if (ModelState.IsValid)
    {
        db.Entry(worker).State = EntityState.Modified;
        db.SaveChanges();
        return RedirectToAction("Index");
    }
    return View(worker);
}

        //Delete Post
public ActionResult Delete(int id)
{
    try
    {
        Worker workerToDelete = new Worker() { ID = id };
        db.Entry(workerToDelete).State = EntityState.Deleted;
        db.SaveChanges();
    }
    catch(DataException/*dex*/)
    {
        return RedirectToAction("Index", new { id = id, saveChangesError = true });
    }
    return RedirectToAction("Index");
}

        //Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Worker worker = db.Workers.Find(id);
            if (worker == null)
            {
                return HttpNotFound();
            }
            return View(worker);
            
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