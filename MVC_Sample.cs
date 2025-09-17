using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.EntityClient;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Data;
using System.Threading.Tasks;

namespace HSE.Controllers
{
    public partial class AccountController : Controller
    {

        private MainEntities Context = new MainEntities();
        private ObjectContext ObjectContext = new ObjectContext(new EntityConnection("name=MainEntities"));
        

        #region Account

        [HttpPost]
      
        [AllowAnonymous]
        public ActionResult AppLogin(string UserName, string Password, string Data, string ApplicationName)
        {
            //  return RedirectToAction("Index", "Home");
            int result = 0;
            string Message = "";
            try
            {
                Session.Clear();
                Session.RemoveAll();
                System.Collections.Specialized.NameValueCollection v = Request.QueryString;
                string LinkData = "";
                long LoginRequestID = 0;
                if (new LinkManager().CheckLinkPassword("HSE", UserName, Password, ref LoginRequestID, ref LinkData, "",ApplicationName) != "OK")//Utility.GetClientIP() //10.16.12.156
                {
                    ViewBag.Message = "Invalid UserName or Password !";
                    //AddLoginFailed();
                    return View();
                }
                var query = from u in Context.Users
                            where u.UserName == UserName
                            && u.Password == strPassword 
                            && u.Active == true
                            select u;

                if (query.Count() == 0)
                {
                    // return RedirectToAction("login");
                    ViewBag.Message = "Invalid UserName or Password !";
                    AddLoginFailed();
                    return View();
                    //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    User user = query.First();
                    Session["CurrentUser"] = user;                                                           
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        ////////////////////////////////////////////
        public String CreateLink(string ApplicationName,string ApplicationPass, string TargetApplicationName)
        {
            LinkManager LinkManager = new LinkManager();
            DataSet ds = new DataSet();
            String xmlData = Context.Database.SqlQuery<String>("Select Cast ((Select * from [Table] for xml auto) as nvarchar(Max))").FirstOrDefault();
            xmlData = "<ROOT>" + xmlData + "</ROOT>";
            long LoginRequestID = 0;
            string LinkPassword = "";
            string Link = LinkManager.CreateLink(ApplicationName,ApplicationPass,TargetApplicationName, General.GetCurrentUser().UserName, ref LinkPassword, ref LoginRequestID, xmlData, "", true,true);//
            Session["LoginRequestID"] = LoginRequestID;
            Session["LinkPassword"] = LinkPassword;
            return Link;
        }
        ////////////////////////////////////////////
        public void RedirectToLink()
        {
            LinkManager LinkManager = new LinkManager();
            string Link = CreateLink("HSE","HSE@3223", "JobExamination");
            Response.Redirect(Link);
            long LoginRequestID = long.Parse(Session["LoginRequestID"].ToString());
            string LinkPassword = Session["LinkPassword"].ToString();           
        }
    
    }
}
