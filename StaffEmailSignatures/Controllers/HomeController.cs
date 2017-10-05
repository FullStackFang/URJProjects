using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StaffEmailSignatures.Models;
using System.Data.Entity;

namespace StaffSignatures.Controllers
{
    public class HomeController : Controller
    {
        private StaffDirectoryEntities db = new StaffDirectoryEntities();

        [Authorize]
        public ActionResult Index()
        {
            IEnumerable<SelectListItem> selectTitleList =
            (from c in db.StaffLists
             where c.workemail == @User.Identity.Name
             select new SelectListItem
             {
                 Text = c.title.ToString(),
                 Value = c.title.ToString()
             }).Distinct();

            IEnumerable<SignatureHtml> aofList = (from c in db.SignatureHtmls
                                                  orderby c.AOF descending
                                                  select c);

            ViewData["ARZA"] = aofList.Where(t => t.AOF == "ARZA").First().htmlString;
            ViewData["CAMP"] = aofList.Where(t => t.AOF == "CAMP").First().htmlString;
            ViewData["CCRJ"] = aofList.Where(t => t.AOF == "CCRJ").First().htmlString;
            ViewData["NFTY"] = aofList.Where(t => t.AOF == "NFTY").First().htmlString;
            ViewData["RAC"] = aofList.Where(t => t.AOF == "RAC").First().htmlString;
            ViewData["URJ"] = aofList.Where(t => t.AOF == "URJ").First().htmlString;
            ViewData["WUPJ"] = aofList.Where(t => t.AOF == "WUPJ").First().htmlString;

            Dictionary<string, string> aofDictionary = new Dictionary<string, string>();
            aofDictionary.Add("ARZA", aofList.Where(t => t.AOF == "ARZA").First().htmlString);
            aofDictionary.Add("CAMP", aofList.Where(t => t.AOF == "CAMP").First().htmlString);
            aofDictionary.Add("CCRJ", aofList.Where(t => t.AOF == "NFTY").First().htmlString);
            aofDictionary.Add("RAC", aofList.Where(t => t.AOF == "RAC").First().htmlString);
            aofDictionary.Add("URJ", aofList.Where(t => t.AOF == "URJ").First().htmlString);
            aofDictionary.Add("WUPJ", aofList.Where(t => t.AOF == "WUPJ").First().htmlString);

            ViewData["aofDictionary"] = aofDictionary;

            string fullName = (from c in db.StaffLists
                               where c.workemail == @User.Identity.Name
                               select c.Name).First();

            string workContact = (from c in db.StaffLists
                                  where c.workemail == @User.Identity.Name
                                  select c.workphone).First();

            string htmlString = (from c in db.SignatureHtmls
                                 orderby c.LastDateModified descending
                                 select c.htmlString).First();

            ViewData["defaultContact"] = workContact;
            ViewData["defaultName"] = fullName;
            string defaultTitle = selectTitleList.First().Value;
            ViewData["defaultTitle"] = defaultTitle;
            ViewData["defaultDepartment"] = (from c in db.StaffLists
                                             where c.title == defaultTitle &&
                                                   c.workemail == @User.Identity.Name
                                             select c.department).ToList();

            //ViewData["htmlString"] = htmlString;
            /*
            IList<SelectListItem> deptList = new List<SelectListItem>
            {
                new SelectListItem{Text = "Information Technology", Value = "Information Technology"},
                new SelectListItem{Text = "Business Systems", Value = "Business Systems"},
            };
            */

            IEnumerable<SelectListItem> selectDeptList =
            (from c in db.StaffLists
             where c.workemail == @User.Identity.Name
             select new SelectListItem
             {
                 Text = c.department.ToString(),
                 Value = c.department.ToString()
             }).Distinct();

            IEnumerable<SelectListItem> selectAOFList =
            (from c in db.SignatureHtmls
             join sl in db.StaffLists on c.AOF.ToLower() equals sl.AOF.ToLower()
             where sl.workemail == @User.Identity.Name
             select new SelectListItem
             {
                 Text = c.AOF.ToString(),
                 Value = c.htmlString.ToString()
             }).Distinct();

            ViewData["titles"] = selectTitleList;
            ViewData["departments"] = selectDeptList;
            ViewData["aof"] = selectAOFList;

            string primaryAOF = (from c in db.StaffLists
                                 where c.workemail == @User.Identity.Name
                                 select c.AOF).First();

            SignatureHtml signatureHtml = (from c in db.SignatureHtmls
                                           where c.AOF == primaryAOF
                                           orderby c.LastDateModified descending
                                           select c).First();

            return View(signatureHtml);
            //db.Entry(signatureHtml).State = EntityState.Modified;
        }

        public JsonResult GetDepartments(string title)
        {
            List<string> departments = new List<string>();
           
            var getData = (from c in db.StaffLists
                          where c.workemail == @User.Identity.Name 
                          && c.title == title
                          select c.department).ToList();

            foreach(var item in getData)
            {
                departments.Add(item);
            }

            return Json(departments);
        }

        public JsonResult GetHTML(string department)
        {
            string areaOfFocus = (from c in db.StaffLists
                                  where c.department == department &&
                                        c.workemail == @User.Identity.Name
                                  select c.AOF).First();

           IEnumerable<SelectListItem> selectAOFList =
           (from c in db.SignatureHtmls
            where c.AOF == areaOfFocus
            select new SelectListItem
            {
                Text = c.AOF.ToString(),
                Value = c.htmlString.ToString()
            }).ToList();

           List<string> htmlStrings = new List<string>();

            foreach(var item in selectAOFList)
            {
                htmlStrings.Add(item.Value);
            }

            return Json(htmlStrings, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpGet] // this action result returns the partial containing the modal
        public ActionResult EditSignature(int id)
        {
            SignatureHtml signatureHtmls = (from c in db.SignatureHtmls
                                            where c.HTMLID == id
                                            select c).First();

            return PartialView("_EditSignature", signatureHtmls);
        }
    }
}
