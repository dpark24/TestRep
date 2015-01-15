using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using TDOT;

public partial class ReferenceAdmin : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
		Response.Cache.SetCacheability(HttpCacheability.NoCache);
		Response.Cache.SetNoStore();
	}

	public static string get_reference_table(string reference_type)
	{
		StringBuilder html = new StringBuilder();
		string sp = "";
		int id;
		string name;
        int order = 0;
		string id_field = "";
		string name_field = "";
        string order_field = "";

		if (reference_type == "FileCategory")
		{
			sp = StoredProcedures.FileCategories.GetAll;
			id_field = "category_id";
			name_field = "category_name";
            order_field = "category_order";
		}
		else if (reference_type == "Organization")
		{
			sp = StoredProcedures.Organizations.GetAll;
			id_field = "organization_id";
			name_field = "organization_name";
            order_field = "organization_order";
		}
  	
		if(sp.Length > 0 && (id_field.Length > 0 && name_field.Length > 0))
		{
            using (SqlConnection conn = new SqlConnection(TDOTDB.conn_string))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					conn.Open();

					cmd.Connection = conn;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = sp;

					try
					{
						SqlDataReader row = cmd.ExecuteReader();

						while (row.Read())
						{
							id = Int32.Parse(row[id_field].ToString());
							name = row[name_field].ToString();

                            if (!Int32.TryParse(row[order_field].ToString(), out order))
                            {
                                order++;
                            }

							html.Append(get_option_list_item(id, name, order));
						}
					}
					catch (Exception ex)
					{
						ErrorLog.save_to_error_log(ex);
					}
				}
			}
		}

		return html.ToString();
	}

	public static string get_option_list_item(int id, string name, int order)
	{
		string html;

		html = "<li data-item_id=\"" + id + "\" id=\"" + name.Replace(" ", "") + "-order_" + id + "\">";
		//html += "<span class=\"actions\"><a href=\"#\" class=\"smaller redtext delete-item\" title=\"Delete this item\">Delete</a> ";
        //html += "<a href=\"#\" class=\"smaller greentext edit-item\" title=\"Edit this item\">Edit</a></span> <span class=\"handle ui-icon ui-icon-arrowthick-2-n-s\"></span> ";
        html += "<span class=\"order\">" + (order + 1) + "</span>. <span>" + name + "</span></li>";

		return html;
	}
}
