using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

public partial class Account_Logout : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
		Response.Cache.SetCacheability(HttpCacheability.NoCache);
		Response.Cache.SetNoStore();

		// Put user code to initialize the page here
		Session.Abandon();
		FormsAuthentication.SignOut();

		Response.Redirect(FormsAuthentication.LoginUrl);
    }
}
