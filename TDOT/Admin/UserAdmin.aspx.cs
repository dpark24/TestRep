using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Web.Profile;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using System.IO;
using AccountProfiles;
using TDOT;

public partial class UserAdmin : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
		Response.Cache.SetCacheability(HttpCacheability.NoCache);
		Response.Cache.SetNoStore();
	}

	public static string get_user_table_headers()
	{
		StringBuilder html = new StringBuilder();
		string[] all_roles = Roles.GetAllRoles();


		foreach (string r in all_roles)
		{
			html.Append("<th>" + r + "</th>");
		}

		return html.ToString();
	}
}
