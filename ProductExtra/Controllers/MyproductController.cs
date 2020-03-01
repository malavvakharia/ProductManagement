using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProductExtra.Models;
using PagedList.Mvc;
using PagedList;
using System.Net;

namespace ProductExtra.Controllers
{
    public class MyproductController : Controller
    {
        // GET: Myproduct
        [HttpGet]
        public ActionResult Add()//Add Product(Get Method)
        {
            List<SelectListItem> Category = new List<SelectListItem>() {
                new SelectListItem {Text = "Fashion", Value = "Fashion" },
                new SelectListItem {Text = "Appliances", Value = "Beauty"},
                new SelectListItem {Text = "Beauty", Value = "Mobile"},
                new SelectListItem {Text = "Sports", Value = "Sports"},
                new SelectListItem {Text = "Grocery", Value = "Grocery"}};
            List<SelectListItem> Quantity = new List<SelectListItem>()
            {new SelectListItem {Text = "1", Value = "1"},
                new SelectListItem {Text = "2", Value = "2"},
                new SelectListItem {Text = "3", Value = "3"},
                new SelectListItem {Text = "4", Value = "4"},
                new SelectListItem {Text = "5", Value = "5"},
                new SelectListItem {Text = "6", Value = "6"},
                new SelectListItem {Text = "7", Value = "7"}
            };
                
            ViewData["Quantity"] = Quantity;
            ViewData["Category"] = Category;
            return View();
        }
        [HttpPost]
        public ActionResult Add(Product p)//Add Product(Post Method)
        {
            if (p != null)
            {
                if (p.ImageFile1 != null)//Optional Image
                {
                    String filename1 = Path.GetFileNameWithoutExtension(p.ImageFile1.FileName);
                    String extension1 = Path.GetExtension(p.ImageFile1.FileName);
                    filename1 = filename1 + DateTime.Now.ToString("yymmssfff") + extension1;
                    p.LargeImage = "~/Images/Large/" + filename1;
                    filename1 = Path.Combine(Server.MapPath("~/Images/Large/"), filename1);
                    p.ImageFile1.SaveAs(filename1);

                }
                else
                {
                    p.LargeImage = "NA";
                }
                String filename = Path.GetFileNameWithoutExtension(p.ImageFile.FileName);
                String extension = Path.GetExtension(p.ImageFile.FileName);
                filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                p.SmallImage = "~/Images/Small/" + filename;
                filename = Path.Combine(Server.MapPath("~/Images/Small/"), filename);
                p.ImageFile.SaveAs(filename);
                if (p.LongDescription == null)//Optional Description
                {
                    p.LongDescription = "NA";
                }
                using (UserEntities1 db = new UserEntities1())
                {
                    db.Products.Add(p);//Add all the details
                    db.SaveChanges();
                }

                ModelState.Clear();
                //Toster Message For Add Prouct
                HttpCookie cookie = new HttpCookie("CookieAdd");
                cookie.Value = "Add";
                Response.Cookies.Add(cookie);
                cookie.Expires = DateTime.Now.AddSeconds(10);
                return RedirectToAction("Add", "Myproduct");
            }
            else
            {
                return View();
            }
        }
        [HttpGet]
        public ActionResult Display(string SearchBy, string search, int? i, string sortBy, string advance_search)//Show All The Entry 
        {
            UserEntities1 db = new UserEntities1();
            ViewBag.SortNameParameter = string.IsNullOrEmpty(sortBy) ? "ProductName desc" : "";
            ViewBag.SortCategoryParameter = sortBy == "Category" ? "Category desc" : "Category";
            var p = db.Products.AsQueryable();
         
            if (SearchBy == "ProductName")
            { 
                p = p.Where(x => x.ProductName.StartsWith(search) || search == null);
            }
            else if (SearchBy == "Category")
            { 
                p = p.Where(x => x.Category.StartsWith(search) || search == null);
            }
            else if (advance_search != null)
            {
                return View(db.Products.Where(x => x.ProductName.StartsWith(advance_search) || x.Category.StartsWith(advance_search) || x.ShortDescription
                .StartsWith(advance_search) || x.Price.StartsWith(advance_search) || search == null).ToList().ToPagedList(i ?? 1, 3));
            }
            switch (sortBy)
            {
                case "ProductName desc":
                    p = p.OrderByDescending(x => x.ProductName);
                    break;

                case "Category desc":
                    p = p.OrderByDescending(x => x.Category);
                    break;

                case "Category":
                    p = p.OrderBy(x => x.Category);
                    break;

                default:
                    p = p.OrderBy(x => x.ProductName);
                    break;
            }
            return View(p.ToPagedList(i ?? 1, 3));//Page List(Each page will display 3 entry)
        }
        
        [HttpGet]
        public ActionResult Delete(int? id)//It will take the id as input and remove the product
        {
            using (UserEntities1 db = new UserEntities1())
            {
                Product p = db.Products.Find(id);
                db.Products.Remove(p);
                string currentImage = Request.MapPath(p.SmallImage);
                string currentImage1 = Request.MapPath(p.LargeImage);
                Session["Delete"] = "Deleted";
                db.SaveChanges();
                //It will Remove the image from folder
                if (System.IO.File.Exists(currentImage))
                {
                    System.IO.File.Delete(currentImage);
                }
                if (System.IO.File.Exists(currentImage1))
                {
                    System.IO.File.Delete(currentImage1);
                }
                HttpCookie cookie = new HttpCookie("CookieDelete");
                cookie.Value = "Delete";
                Response.Cookies.Add(cookie);
                cookie.Expires = DateTime.Now.AddSeconds(10);
                return RedirectToAction("Display");
            }
}
        public ActionResult Details(int? id) //When user clicks on Product Name, the id of that will be catched here.
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (UserEntities1 db = new UserEntities1())
            {
                var product = db.Products.Find(id);
                if (product == null) //If the 'product' object's value is 'null'
                {
                    return HttpNotFound();
                }
                return View(product);
            }
        }
        
        public ActionResult Edit(int? id)//Edit Product(Get Method)
        {

            using (UserEntities1 db = new UserEntities1())
            {
                List<SelectListItem> Category = new List<SelectListItem>() {
                new SelectListItem {Text = "Fashion", Value = "Fashion" },
                new SelectListItem {Text = "Appliances", Value = "Appliances"},
                new SelectListItem {Text = "Beauty", Value = "Beauty"},
                new SelectListItem {Text = "Sports", Value = "Sports"},
                new SelectListItem {Text = "Grocery", Value = "Grocery"}};
                List<SelectListItem> Quantity = new List<SelectListItem>()
            {new SelectListItem {Text = "1", Value = "1"},
                new SelectListItem {Text = "2", Value = "2"},
                new SelectListItem {Text = "3", Value = "3"},
                new SelectListItem {Text = "4", Value = "4"},
                new SelectListItem {Text = "5", Value = "5"},
                new SelectListItem {Text = "6", Value = "6"},
                new SelectListItem {Text = "7", Value = "7"}
                };
                ViewData["Quantity"] = Quantity;
                ViewData["Category"] = Category;
                try
                {
                    var xyz = db.Products.Find(id);//Fetch The Data With using id
                    Session["imgpath1"] = xyz.SmallImage;
                    Session["imgpath2"] = xyz.LargeImage;

                    //return Json(xyz, JsonRequestBehavior.AllowGet);
                    if (xyz.LargeImage == "NA")
                    {
                        ViewBag.Largeimg = true;
                    }
                    if (xyz.LongDescription == "NA")
                    {
                        ViewBag.Longdsp = true;
                    }
                }

                catch
                {
                    Console.WriteLine("xyz");
                }
                return View(db.Products.Where(x => x.ProductId == id).FirstOrDefault());
            }

        }

        // POST: Customer/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Product p)//Edit Product(Post Method)
        {
            try
            {
                string oldpath1 = Session["imgpath1"].ToString();
                string oldpath2 = Session["imgpath2"].ToString();
                //If conditions for optional image as well as compulsory image.
                if (p.SmallImage != null && p.LargeImage != null)
                {
                    String filename = Path.GetFileNameWithoutExtension(p.ImageFile.FileName);
                    String extension = Path.GetExtension(p.ImageFile.FileName);
                    filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                    p.SmallImage = "~/Images/Small/" + filename;
                    filename = Path.Combine(Server.MapPath("~/Images/Small/"), filename);
                    p.ImageFile.SaveAs(filename);
                    if (p.SmallImage != oldpath1)
                    {
                        string p1 = Request.MapPath(oldpath1);
                        if (System.IO.File.Exists(p1))
                        {
                            System.IO.File.Delete(p1);
                        }
                    }
                    String filename1 = Path.GetFileNameWithoutExtension(p.ImageFile1.FileName);
                    String extension1 = Path.GetExtension(p.ImageFile1.FileName);
                    filename1 = filename1 + DateTime.Now.ToString("yymmssfff") + extension1;
                    p.LargeImage = "~/Images/Large/" + filename1;
                    filename1 = Path.Combine(Server.MapPath("~/Images/Large/"), filename1);
                    p.ImageFile1.SaveAs(filename1);
                    if (p.LargeImage != oldpath2)
                    {
                        string p2 = Request.MapPath(oldpath2);
                        if (System.IO.File.Exists(p2))
                        {
                            System.IO.File.Delete(p2);
                        }
                    }
                }
                if (p.ImageFile != null)
                {
                    String filename = Path.GetFileNameWithoutExtension(p.ImageFile.FileName);
                    String extension = Path.GetExtension(p.ImageFile.FileName);
                    filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                    p.SmallImage = "~/Images/Small/" + filename;
                    filename = Path.Combine(Server.MapPath("~/Images/Small/"), filename);
                    p.ImageFile.SaveAs(filename);
                    if (p.LargeImage == null)
                    {
                        p.LargeImage = oldpath2;
                    }
                    if (p.SmallImage != oldpath1)
                    {
                        string p1 = Request.MapPath(oldpath1);
                        if (System.IO.File.Exists(p1))
                        {
                            System.IO.File.Delete(p1);
                        }
                    }
                }
                if (p.ImageFile1 != null)
                {
                    String filename1 = Path.GetFileNameWithoutExtension(p.ImageFile1.FileName);
                    String extension1 = Path.GetExtension(p.ImageFile1.FileName);
                    filename1 = filename1 + DateTime.Now.ToString("yymmssfff") + extension1;
                    p.LargeImage = "~/Images/Large/" + filename1;
                    filename1 = Path.Combine(Server.MapPath("~/Images/Large/"), filename1);
                    p.ImageFile1.SaveAs(filename1);
                    if (p.SmallImage == null)
                    {
                        p.SmallImage = oldpath1;
                    }
                    if (p.LargeImage != oldpath2)
                    {
                        string p2 = Request.MapPath(oldpath2);
                        if (System.IO.File.Exists(p2))
                        {
                            System.IO.File.Delete(p2);
                        }
                    }
                }
                if (p.SmallImage == null && p.LargeImage == null)
                {
                    p.SmallImage = oldpath1;
                    p.LargeImage = oldpath2;
                }
                if (p.LongDescription == null)
                {
                    p.LongDescription = "NA";
                }
                UserEntities1 db = new UserEntities1();
                db.Entry(p).State = EntityState.Modified;//Modify The Data
                db.SaveChanges();
                //Toster Message For Edit Product.
                HttpCookie cookie2 = new HttpCookie("CookieEdit");
                cookie2.Value = "Edit";
                Response.Cookies.Add(cookie2);
                cookie2.Expires = DateTime.Now.AddSeconds(5);
                return RedirectToAction("Display");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return View();
        }
     
        [HttpPost]
        public ActionResult Deleteall(string[] id)//Multiple Delete
        {
            int[] getid = null;
            if (id != null)
            {
                getid = new int[id.Length];
                int j = 0;
                foreach (string i in id)
                {
                    int.TryParse(i, out getid[j++]);//Fetch all id
                }
                List<Product> pids = new List<Product>();
                UserEntities1 db = new UserEntities1();
                pids = db.Products.Where(x => getid.Contains(x.ProductId)).ToList();
                foreach (var s in pids)
                {
                    db.Products.Remove(s);//Remove product one by one using product id
                }
                db.SaveChanges();
                return RedirectToAction("Display");
            }
            return RedirectToAction("Display");
        }
        }

    }
