using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StaffEmailSignatures.Models;

namespace StaffEmailSignatures.Controllers
{
    public class SignatureHtmlsController : Controller
    {
        private StaffDirectoryEntities db = new StaffDirectoryEntities();

        // GET: SignatureHtmls
        public ActionResult Index()
        {
            IEnumerable<SignatureHtml> signatures = db.SignatureHtmls.ToList().OrderByDescending(t => t.LastDateModified);

            return View(signatures);
        }

        // GET: SignatureHtmls/Create
        public ActionResult Create()
        {
            string signatureHtmlText = (from c in db.SignatureHtmls
                                           orderby c.LastDateModified descending
                                           select c.htmlString).First();

            SignatureHtml newSignature = new SignatureHtml();
            newSignature.htmlString = signatureHtmlText;

            return View(newSignature);
        }

        // POST: SignatureHtmls/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HTMLID,htmlString,LastDateModified")] SignatureHtml signatureHtml)
        {
            if (ModelState.IsValid)
            {
                var timeUtc = DateTime.UtcNow;
                TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);
                signatureHtml.LastDateModified = easternTime;
                db.SignatureHtmls.Add(signatureHtml);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(signatureHtml);
        }

        // GET: SignatureHtmls/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SignatureHtml signatures = db.SignatureHtmls.Find(id);

            if (signatures == null)
            {
                return HttpNotFound();
            }

            return View(signatures);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HTMLID,htmlString,modifiedHtmlString,LastDateModified")] SignatureHtml signatureHtml)
        {
            db.Entry(signatureHtml).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        // GET: SignatureHtmls/EditPersonal/5
        public ActionResult EditPersonal(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SignatureHtml signatureHtml = db.SignatureHtmls.Find(id);

            /*
            int StaffID = (from c in db.StaffLists
                          where c.workemail == @User.Identity.Name
                          select c.StaffID).First();
            StaffList person = db.StaffLists.Find(StaffID);

            if (person.personalHtml != null)
            {
                signatureHtml.modifiedHtmlString = person.personalHtml;
                return View(signatureHtml);
            }
            */

            if (signatureHtml == null)
            {
                return HttpNotFound();
            }

            return View(signatureHtml);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPersonal([Bind(Include = "HTMLID,htmlString,LastDateModified,AOF")] SignatureHtml signatureHtml)
        {
            db.Entry(signatureHtml).State = EntityState.Modified;   
            signatureHtml.LastDateModified = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetPrimary([Bind(Include = "HTMLID,htmlString,LastDateModified")] int? id)
        {
            var timeUtc = DateTime.UtcNow;
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);

            SignatureHtml signatureHtml = (from c in db.SignatureHtmls
                                            where c.HTMLID == id
                                            select c).First();
            signatureHtml.LastDateModified = easternTime;
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        // GET: SignatureHtmls/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SignatureHtml signatureHtml = db.SignatureHtmls.Find(id);
            if (signatureHtml == null)
            {
                return HttpNotFound();
            }
            return View(signatureHtml);
        }

        // POST: SignatureHtmls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SignatureHtml signatureHtml = db.SignatureHtmls.Find(id);
            db.SignatureHtmls.Remove(signatureHtml);
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
