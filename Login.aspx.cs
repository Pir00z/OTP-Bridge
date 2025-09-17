using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Data.Entity;
using System.Collections.ObjectModel;
using System.Web.SessionState;
using Model;
using System.Web.Configuration;
using System.Collections.Specialized;
using System.Xml;
using System.Collections;

public partial class View_login : System.Web.UI.Page
{   
    protected void Page_Load(object sender, EventArgs e)
    {
        string strUserName = "", strPassword = "", SourceApplicationName = "";
        

        if (IsPostBack) return;
        Session.Clear();

        if (Request.QueryString["UserName"] != null) strUserName = Request.QueryString["UserName"].ToString();
        if (Request.QueryString["Password"] != null) strPassword = Request.QueryString["Password"].ToString();
        if (Request.QueryString["ApplicationName"] != null) SourceApplicationName = Request.QueryString["ApplicationName"].ToString();       
  
        if (Request.QueryString["UserName"] != null)
            AppLogin(strUserName, strPassword, Request.UserHostAddress, SourceApplicationName);
        
    }

    
    //-------------------------------------------------------------------------
    protected void AppLogin(string UserName, string Password,string RequestIP,string SourceApplicationName="")
    {
       int result = 0;
        string Message = "";
        try
        {
            
            //Set EntityConnection and Context;
            var Context = new DBEntities(Utility.EntitiyConnectionString);

            try
            {
            string  Data="";
            long LoginRequestID = 0;
            if (new LinkManager().CheckLinkPassword("JobExamination", UserName, Password, ref LoginRequestID, ref Data,"", SourceApplicationName) != "OK")
            {
                lblNotify.Text = "نام كاربر يا كلمه عبور نا معتبر است";                
                return;
            }

            Session["LinkData"] = Data;
            Session["LoginRequestID"] = LoginRequestID;
            

            var query = from u in Context.vUsers
                        where u.UserName == UserName
                              && u.Password == Password 
                              && u.Active == true
                        select u;

            if (query.Count() == 0)
            {
                lblNotify.Text = "Invalid UserName or Password !"; 
                ResetPassword.Visible = true;
                return;
            }
            else
            {
                Session[User] = query.First();
                lblNotify.Text = "welcome";
               Response.Redirect(@"View\Pages\Index.aspx");
            }
                
        }
        catch (Exception ex)
        {
            lblNotify.Text = ex.Message;
        }

    }
}
