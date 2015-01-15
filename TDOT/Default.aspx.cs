using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using TDOT;
using AccountProfiles;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
		Response.Cache.SetCacheability(HttpCacheability.NoCache);
		Response.Cache.SetNoStore();

		if (!HttpContext.Current.User.Identity.IsAuthenticated)
		{
			Response.Redirect("~/Account/Login.aspx");
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

    public string get_regions()
    {
        StringBuilder options = new StringBuilder();
        Region Reg = new Region();

        using (SqlConnection conn = new SqlConnection(TDOTDB.conn_string))
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                conn.Open();

                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = StoredProcedures.Regions.GetAll;

                try
                {
                    SqlDataReader row = cmd.ExecuteReader();
                    while (row.Read())
                    {
                        Reg = new Region(row);

                        options.Append("<option value=\"" + Reg.Id + "\">" + Reg.Name + "</option>");
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
