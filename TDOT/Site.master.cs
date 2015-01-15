using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using TDOT;

public partial class SiteMaster : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
		if(!VirtualPathUtility.GetDirectory(HttpContext.Current.Request.Path).Contains("/Account/") && !HttpContext.Current.User.Identity.IsAuthenticated)
		{
			HttpContext.Current.Response.Redirect("~/Account/Login.aspx");
		}
    }

	public string get_categories()
	{
		StringBuilder options = new StringBuilder();
		DocumentCategory Category = new DocumentCategory();

        using (SqlConnection conn = new SqlConnection(TDOTDB.conn_string))
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				conn.Open();

				cmd.Connection = conn;
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = StoredProcedures.FileCategories.GetAll;

				try
				{
					SqlDataReader row = cmd.ExecuteReader();
					while (row.Read())
					{
						Category = new DocumentCategory(row);
						
						options.Append("<option value=\"" + Category.Id + "\">" + Category.Name + "</option>");
					}
				}
				catch (Exception ex)
				{
					ErrorLog.save_to_error_log(ex);
				}
			}
		}

		return options.ToString();
	}

	public string get_organization()
	{
		StringBuilder options = new StringBuilder();
		Organization Org = new Organization();

        using (SqlConnection conn = new SqlConnection(TDOTDB.conn_string))
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				conn.Open();

				cmd.Connection = conn;
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = StoredProcedures.Organizations.GetAll;

				try
				{
					SqlDataReader row = cmd.ExecuteReader();
					while (row.Read())
					{
						Org = new Organization(row);

						options.Append("<option value=\"" + Org.Id + "\">" + Org.Name + "</option>");
					}
				}
				catch (Exception ex)
				{
					ErrorLog.save_to_error_log(ex);
				}
			}
		}

		return options.ToString();
	}
}
