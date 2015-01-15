using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Web.Profile;
using System.Globalization;
using System.Web.Security;
using TDOT;
using AccountProfiles;
using System.IO;

public partial class ajax : System.Web.UI.Page
{
	public static JavaScriptSerializer json = new JavaScriptSerializer();

	protected void Page_Load(object sender, EventArgs e)
	{
		if(!HttpContext.Current.User.Identity.IsAuthenticated)
		{
			Response.End();
		}
	}

	/// <summary>
	/// takes actions from JSON request and executes appropriate function
	/// </summary>
	public static void delegate_action()
	{
		string action = HttpContext.Current.Request.Form["action"];

		if(action == null)
		{
			action = HttpContext.Current.Request.QueryString["action"];
		}

		if (action == null)
		{
			return;
		}
		else if (action == "upload-document")
		{
			upload_document();
		}
		else if (action == "get-document-table")
		{
			get_document_table();
		}
		else if (action == "toggle-user-approval")
		{
			toggle_user_approval();
		}
		else if (action == "delete-user-account")
		{
			delete_user_account();
		}
		else if (action == "create-user-account")
		{
			create_user_account();
		}
		else if (action == "change-user-password")
		{
			change_user_password();
		}
        else if (action == "delete-file")
        {
            delete_file();
        }
        else if (action == "get-file-information")
        {
            get_file_information();
        }
        else if (action == "edit-document-metadata")
        {
            edit_document_metadata();
        }
        else if (action == "toggle-user-role")
        {
            toggle_user_role();
        }
        else if (action == "get-file-management-table")
        {
            get_file_management_table();
        }
        else if (action == "get-region-counties")
        {
            get_region_counties();
        }
	}

    public static void get_region_counties()
    {
        int region_id;
        StringBuilder options = new StringBuilder();
        County Cnty;
        Dictionary<string, object> data = new Dictionary<string, object>();

        try
        {
            region_id = Int32.Parse(HttpContext.Current.Request.Form["region_id"].ToString());

            //if the user is permitted and it's not themselves
            if (region_id > 0)
            {
                using (SqlConnection conn = new SqlConnection(TDOTDB.conn_string))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        conn.Open();

                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = StoredProcedures.Counties.GetAll;
                        cmd.Parameters.Add(new SqlParameter("@region_id", region_id));

                        try
                        {
                            SqlDataReader row = cmd.ExecuteReader();
                            while (row.Read())
                            {
                                Cnty = new County(row);

                                options.Append("<option value=\"" + Cnty.Id + "\">" + Cnty.Name + "</option>");
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.save_to_error_log(ex);
                        }
                    }

                    
                }

                data.Add("options", options.ToString());
                data.Add("status", 1);
            }
            else
            {
                data.Add("status", -1);
            }
        }
        catch (Exception ex)
        {
            data.Add("status", 0);
            ErrorLog.save_to_error_log(ex);
        }

        HttpContext.Current.Response.Write(json.Serialize(data));
    }

    private static void get_file_management_table()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        DocumentManagement.ProjectFile File = new DocumentManagement.ProjectFile();
        Dictionary<string, string> row_data;

        string sort_dir = ""; //ASC or DESC
        string sort_column = "0"; //column to sort by
        int display_start = 0;
        int display_length = 0;
        string search_str = "";
        string total_row_count = "";
        string selected_row_count = "";
        string excerpt = "";
        bool shortened = false;
        int max_length = 100;

        //searches
        string search_year = "";
        int search_region = 0;
        int search_county = 0;
        int search_organization = 0;
        int search_category = 0;

        List<object> table_data = new List<object>();
        StringBuilder cell_data = new StringBuilder();

        try
        {
            //Creation of variables


            //get the sorting direction and column
            sort_dir = HttpContext.Current.Request.QueryString["sSortDir_0"].ToUpper();
            sort_column = HttpContext.Current.Request.QueryString["iSortCol_0"].ToString();

            //get the display data
            Int32.TryParse(HttpContext.Current.Request.QueryString["iDisplayStart"], out display_start);
            Int32.TryParse(HttpContext.Current.Request.QueryString["iDisplayLength"], out display_length);

            //get any search strings
            search_str = HttpContext.Current.Request.QueryString["sSearch"].ToString();
            
            if (HttpContext.Current.Request.QueryString["sSearch_2"] != null)
            {
                search_year = HttpContext.Current.Request.QueryString["sSearch_2"];
            }

            if (!(Int32.TryParse(HttpContext.Current.Request.QueryString["sSearch_3"], out search_region)))
            {
                search_region = -1;
            }

            if (!(Int32.TryParse(HttpContext.Current.Request.QueryString["sSearch_4"], out search_county)))
            {
                search_county = -1;
            }

            if (!(Int32.TryParse(HttpContext.Current.Request.QueryString["sSearch_6"], out search_organization)))
            {
                search_organization = -1;
            }

            if (!(Int32.TryParse(HttpContext.Current.Request.QueryString["sSearch_7"], out search_category)))
            {
                search_category = -1;
            }

            //title = HttpContext.Current.Request.QueryString["sSearch_1"].ToString();
            //description = HttpContext.Current.Request.QueryString["sSearch_2"].ToString();
            
            using (SqlConnection conn = new SqlConnection(TDOTDB.conn_string))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    conn.Open();

                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = StoredProcedures.Files.GetDataTableCount;
                    cmd.Parameters.AddWithValue("@SearchString", search_str);

                    if (search_year.Length > 0)
                    {
                        cmd.Parameters.Add(new SqlParameter("@file_monitoring_year", search_year));
                    }

                    if (search_region > 0)
                    {
                        cmd.Parameters.Add(new SqlParameter("@region_id", search_region));
                    }

                    if (search_county > 0)
                    {
                        cmd.Parameters.Add(new SqlParameter("@county_id", search_county));
                    }

                    if (search_organization > 0)
                    {
                        cmd.Parameters.Add(new SqlParameter("@organization_id", search_organization));
                    }

                    if (search_category > 0)
                    {
                        cmd.Parameters.Add(new SqlParameter("@category_id", search_category));
                    }

                    try
                    {
                        SqlDataReader row = cmd.ExecuteReader();
                        row.Read();
                        selected_row_count = row[0].ToString();
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.save_to_error_log(ex);
                    }
                }
            }

            //gets the row count..?
            //*/
            using (SqlConnection conn = new SqlConnection(TDOTDB.conn_string))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    conn.Open();

                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = StoredProcedures.Files.GetDataTableCount;

                    try
                    {
                        SqlDataReader row = cmd.ExecuteReader();
                        row.Read();
                        total_row_count = row[0].ToString();
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.save_to_error_log(ex);
                    }
                    //cmd.Dispose();
                }
                //conn.Close();
            }
            //*/

            using (SqlConnection conn = new SqlConnection(TDOTDB.conn_string))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    int row_count = 0;

                    conn.Open();

                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = StoredProcedures.Files.GetDataTable;
                    cmd.Parameters.AddWithValue("@SortColumn", sort_column);
                    cmd.Parameters.AddWithValue("@SortDirection", sort_dir);
                    cmd.Parameters.AddWithValue("@DisplayStart", display_start);
                    cmd.Parameters.AddWithValue("@DisplayLength", display_length);
                    cmd.Parameters.AddWithValue("@SearchString", search_str);


                    if (search_year.Length > 0)
                    {
                        cmd.Parameters.Add(new SqlParameter("@file_monitoring_year", search_year));
                    }

                    if (search_region > 0)
                    {
                        cmd.Parameters.Add(new SqlParameter("@region_id", search_region));
                    }

                    if (search_county > 0)
                    {
                        cmd.Parameters.Add(new SqlParameter("@county_id", search_county));
                    }

                    if (search_organization > 0)
                    {
                        cmd.Parameters.Add(new SqlParameter("@organization_id", search_organization));
                    }

                    if (search_category > 0)
                    {
                        cmd.Parameters.Add(new SqlParameter("@category_id", search_category));
                    }

                    try
                    {
                        SqlDataReader row = cmd.ExecuteReader();

                        while (row.Read())
                        {
                            row_data = new Dictionary<string, string>();
                            File = new DocumentManagement.ProjectFile(row);
                            shortened = false;

                            row_count++;

                            if (row_count > display_start)
                            {
                                cell_data = new StringBuilder();

							    File = new DocumentManagement.ProjectFile(row);
                                
                                // this span holds all of our data- for the row since we can't easilty pass it with the tr tag
                                cell_data.Append("<input type=\"checkbox\" class=\"add-to-download-package\" value=\"" + File.Id + "\" />");

                                row_data.Add("0", cell_data.ToString());

                                //file title cell
                                cell_data = new StringBuilder();

                                cell_data.Append("<div class=\"mh4\">");

                                cell_data.Append("<a href=\"GetFile.aspx?files=" + File.Id + "\" title=\"Click to download " + File.Title + "\">" + File.Title + "</a>");
                                cell_data.Append("<br />(<a href=\"GetFile.aspx?files=" + File.Id + "\" title=\"Click to download " + File.Name + "\" class=\"smaller\">" + File.Name + "</a>)");

                                if (File.Description.Length > max_length)
                                {
                                    excerpt = File.Description.Substring(0, max_length);
                                    shortened = true;
                                }

                                if (HttpContext.Current.User.IsInRole("Administrator"))
                                {
                                    cell_data.Append("<div class=\"float-right smaller\"><a href=\"#\" class=\"greentext edit-file\" title=\"Edit File\">Edit</a>");
                                    cell_data.Append(" | <a href=\"#\" class=\"redtext delete-file\" title=\"Delete File\">Delete</a></div>");
                                }

                                if (shortened)
                                {
                                    cell_data.Append("<div class=\"excerpt\">" + excerpt + "[...]</div><a href=\"#\" title=\"Read the complete description\" class=\"toggle-full-text\">Show more</a>");
                                    cell_data.Append(" <div class=\"full-text hidden\">" + File.Description + "</div>");
                                }
                                else
                                {
                                    cell_data.Append("<div>" + File.Description + "</div>");
                                }

                                cell_data.Append("</div>");

                                row_data.Add("1", cell_data.ToString());
                                row_data.Add("2", File.MonitoringYear.ToString());
                                row_data.Add("3", "<div class=\"mh4\" title=\"" + File.GetMeta("region_name") + "\">" + File.GetMeta("region_name").Replace("Region ", "R-") + "</div>");
                                row_data.Add("4", File.GetMeta("county_name"));
                                row_data.Add("5", File.Pin);
                                row_data.Add("6", File.GetMeta("organization_name"));
                                row_data.Add("7", row["category_name"].ToString());
                                row_data.Add("8", File.ModifiedOn.ToShortDateString());
                                row_data.Add("9", File.ModifiedBy);

                                row_data.Add("DT_RowId", "file-" + File.Id);
                                row_data.Add("DT_RowClass", "DT_RowClass");

                                table_data.Add(row_data);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.save_to_error_log(ex);
                    }

                }

            }

            data.Add("sEcho", HttpContext.Current.Request.QueryString["sEcho"].ToString());
            data.Add("iTotalRecords", total_row_count);
            data.Add("iTotalDisplayRecords", selected_row_count);
            data.Add("aaData", table_data);
            data.Add("status", "1");
        }
        catch (Exception ex)
        {
            ErrorLog.save_to_error_log(ex);
            data.Add("status", "0");
        }

        HttpContext.Current.Response.Write(json.Serialize(data));
    }

    public static void toggle_user_role()
    {
        string user_name = "";
        string requested_role = "";
        MembershipUser user;
        Dictionary<string, object> data = new Dictionary<string, object>();

        try
        {
            user_name = HttpContext.Current.Request.Form["user_name"].ToString();
            requested_role = HttpContext.Current.Request.Form["role_level"].ToString();

            //if the user is permitted and it's not themselves
            if (HttpContext.Current.User.IsInRole("Administrator"))
            {
                user = Membership.GetUser(user_name);

                if (Roles.IsUserInRole(user_name, requested_role))
                {
                    Roles.RemoveUserFromRole(user_name, requested_role);
                    data.Add("role", "removed");
                }
                else
                {
                    Roles.AddUserToRole(user_name, requested_role);
                    data.Add("role", "added");
                }

                data.Add("status", 1);
            }
            else
            {
                data.Add("status", -1);
            }
        }
        catch (Exception ex)
        {
            data.Add("status", 0);
            ErrorLog.save_to_error_log(ex);
        }

        HttpContext.Current.Response.Write(json.Serialize(data));
    }

    //edit-document-metadata
    public static void edit_document_metadata()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        bool status = false;
        int file_id = 0;
        Dictionary<string, string> row = new Dictionary<string, string>();
        DocumentManagement.ProjectFile File = new DocumentManagement.ProjectFile();

        try
        {
            file_id = Int32.Parse(HttpContext.Current.Request.Form["file_id"].ToString());

            row = TDOTDB.sp_get_row(StoredProcedures.Files.GetById, new SqlParameter("@file_ids", file_id));

            if (row.Count > 0)
            {
                File = new DocumentManagement.ProjectFile(row);
                File.CategoryId = Int32.Parse(HttpContext.Current.Request.Form["edit_file_category"]);
                File.Description = HttpContext.Current.Request.Form["edit_file_description"].ToString();
                File.Title = HttpContext.Current.Request.Form["edit_file_title"].ToString();
                File.CountyId = Int32.Parse(HttpContext.Current.Request.Form["edit_file_county"]);
                File.Pin = HttpContext.Current.Request.Form["edit_file_pin"].ToString();
                File.MonitoringYear = HttpContext.Current.Request.Form["edit_file_monitoring_year"];
                File.OrganizationId = Int32.Parse(HttpContext.Current.Request.Form["edit_file_organization"]);

                if (HttpContext.Current.Request.Form["edit_file_tdec"] != null)
                {
                    File.TdecPermit = HttpContext.Current.Request.Form["edit_file_tdec"];
                }

                if (HttpContext.Current.Request.Form["edit_file_usace"] != null)
                {
                    File.USACEPermit = HttpContext.Current.Request.Form["edit_file_usace"];
                }

                status = File.Update();
                data.Add("file_information", File);
                data.Add("status", (status) ? 1 : -1);
            }
            else
            {
                data.Add("status", -2);
            }
            
        }
        catch (Exception ex)
        {
            ErrorLog.save_to_error_log(ex);
            data.Add("status", 0);
        }

        HttpContext.Current.Response.Write(json.Serialize(data));
    }

    //get-file-information
    public static void get_file_information()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        Dictionary<string, string> row = new Dictionary<string, string>();
        List<SqlParameter> parameters = new List<SqlParameter>();
        DocumentManagement.ProjectFile File;
        int file_id = 0;

        try
        {
            file_id = Int32.Parse(HttpContext.Current.Request.Form["file_id"]);
            
            parameters.Add(new SqlParameter("@file_ids", file_id));
            row = TDOTDB.sp_get_row(StoredProcedures.Files.GetById, parameters);
            File = new DocumentManagement.ProjectFile(row);

            data.Add("file_information", File);
            data.Add("status", 1);
        }
        catch (Exception ex)
        {
            ErrorLog.save_to_error_log(ex);
            data.Add("status", 0);
        }

        HttpContext.Current.Response.Write(json.Serialize(data));
    }

    public static void delete_file()
    {
        DocumentManagement.ProjectFile File = new DocumentManagement.ProjectFile();
        Dictionary<string, object> data = new Dictionary<string, object>();

        if (HttpContext.Current.User.IsInRole("Administrator"))
        {
            try
            {
                File.Id = Int32.Parse(HttpContext.Current.Request.Form["file_id"]);

                if (File.Delete())
                {
                    data.Add("status", 1);
                }
                else
                {
                    data.Add("status", -1);
                }
            }
            catch (Exception ex)
            {
                data.Add("status", 0);
                ErrorLog.save_to_error_log(ex);
            }
        }
        else
        {
            data.Add("status", -2);
        }

        HttpContext.Current.Response.Write(json.Serialize(data));
    }

	public static void change_user_password()
	{
		string old_password = "";
		string new_password = "";
		Dictionary<string, object> data = new Dictionary<string, object>();

		try
		{
			old_password = HttpContext.Current.Request.Form["old_password"].ToString();
			new_password = HttpContext.Current.Request.Form["new_password"].ToString();

			if(Membership.GetUser(HttpContext.Current.User.Identity.Name).ChangePassword(old_password, new_password))
			{
				data.Add("status", 1);
			}
			else
			{
				data.Add("status", -1);
			}
		}
		catch (Exception ex)
		{
			data.Add("status", 0);
			ErrorLog.save_to_error_log(ex);
		}

		HttpContext.Current.Response.Write(json.Serialize(data));
	}

	public static void create_user_account()
	{
		string user_name = "";
		string password = "";
		string email = "";
		MembershipUser new_user;
		Dictionary<string, object> data = new Dictionary<string, object>();

		try
		{
			user_name = HttpContext.Current.Request.Form["username"].ToString();
			password = HttpContext.Current.Request.Form["password"].ToString();
			email = HttpContext.Current.Request.Form["email"].ToString();

			//if the user is permitted and it's not themselves
			if (HttpContext.Current.User.IsInRole("Administrator"))
			{
				new_user = Membership.CreateUser(user_name, password, email);

				Roles.AddUserToRole(new_user.UserName, "Viewer");
				data.Add("status", 1);
				data.Add("user_table", Utilities.GetUserTable());
			}
			else
			{
				data.Add("status", -1);
			}
		}
		catch (MembershipCreateUserException e)
		{
			data.Add("status", -2);
			switch (e.StatusCode)
			{
				case MembershipCreateStatus.DuplicateUserName:
					data.Add("reason", "Username already exists. Please enter a different user name.");
					break;
				case MembershipCreateStatus.DuplicateEmail:
					data.Add("reason", "A username for that e-mail address already exists. Please enter a different e-mail address.");
					break;
				case MembershipCreateStatus.InvalidPassword:
					data.Add("reason", "The password provided is invalid. Please enter a valid password value.");
					break;
				case MembershipCreateStatus.InvalidEmail:
					data.Add("reason", "The e-mail address provided is invalid. Please check the value and try again.");
					break;
				case MembershipCreateStatus.InvalidAnswer:
					data.Add("reason", "The password retrieval answer provided is invalid. Please check the value and try again.");
					break;
				case MembershipCreateStatus.InvalidQuestion:
					data.Add("reason", "The password retrieval question provided is invalid. Please check the value and try again.");
					break;
				case MembershipCreateStatus.InvalidUserName:
					data.Add("reason", "The user name provided is invalid. Please check the value and try again.");
					break;
				case MembershipCreateStatus.ProviderError:
					data.Add("reason", "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.");
					break;
				case MembershipCreateStatus.UserRejected:
					data.Add("reason", "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.");
					break;
				default:
					data.Add("reason", "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.");
					break;
			}
		}
		catch (Exception ex)
		{
			data.Add("status", 0);
			ErrorLog.save_to_error_log(ex);
		}

		HttpContext.Current.Response.Write(json.Serialize(data));
	}

	public static void delete_user_account()
	{
		string user_name = "";
		Dictionary<string, object> data = new Dictionary<string, object>();

		try
		{
			user_name = HttpContext.Current.Request.Form["user_name"].ToString();

			//if the user is permitted and it's not themselves
			if (HttpContext.Current.User.IsInRole("Administrator") && user_name != HttpContext.Current.User.Identity.Name)
			{
				Membership.DeleteUser(user_name);
				data.Add("status", 1);
			}
			else
			{
				data.Add("status", -1);
			}
		}
		catch (Exception ex)
		{
			data.Add("status", 0);
			ErrorLog.save_to_error_log(ex);
		}

		HttpContext.Current.Response.Write(json.Serialize(data));
	}

	public static void toggle_user_approval()
	{
		string user_name  ="";
		MembershipUser user;
		Dictionary<string, object> data = new Dictionary<string, object>();

		try
		{
			user_name = HttpContext.Current.Request.Form["user_name"].ToString();
			
			//if the user is permitted and it's not themselves
			if(HttpContext.Current.User.IsInRole("Administrator") && user_name != HttpContext.Current.User.Identity.Name)
			{
				user = Membership.GetUser(user_name);
				if(user.IsApproved)
				{
					user.IsApproved = false;
				}
				else
				{
					user.IsApproved = true;
				}

				Membership.UpdateUser(user);
				data.Add("status", 1);
			}
			else
			{
				data.Add("status", -1);
			}
		}
		catch (Exception ex)
		{
			data.Add("status", 0);
			ErrorLog.save_to_error_log(ex);
		}

		HttpContext.Current.Response.Write(json.Serialize(data));
	}

	public static void upload_document()
	{
		bool status = false;
		DocumentManagement.ProjectFile File = new DocumentManagement.ProjectFile();

		try
		{
			File.CategoryId = Int32.Parse(HttpContext.Current.Request.Form["upload_file_category"].ToString());
            File.CountyId = Int32.Parse(HttpContext.Current.Request.Form["upload_file_county"].ToString());
            File.OrganizationId = Int32.Parse(HttpContext.Current.Request.Form["upload_file_organization"].ToString());
            File.Description = HttpContext.Current.Request.Form["upload_file_description"].ToString();
            File.Title = HttpContext.Current.Request.Form["upload_file_title"].ToString();
            File.Pin = HttpContext.Current.Request.Form["upload_file_pin"].ToString();
            File.MonitoringYear = HttpContext.Current.Request.Form["upload_file_monitoring_year"].ToString();

            if (HttpContext.Current.Request.Form["upload_file_tdec"] != null)
            {
                File.TdecPermit = HttpContext.Current.Request.Form["upload_file_tdec"].ToString();
            }

            if (HttpContext.Current.Request.Form["upload_file_usace"] != null)
            {
                File.USACEPermit = HttpContext.Current.Request.Form["upload_file_usace"].ToString();
            }

            File.PostedFile = HttpContext.Current.Request.Files[0];
            status = File.Save();
		}
		catch(Exception ex)
		{
			ErrorLog.save_to_error_log(ex);
		}

		HttpContext.Current.Response.Write((status) ? 1 : 0);
	}

	public static void get_document_table()
	{
		Dictionary<string, object> data = new Dictionary<string,object>();

		try
		{
			data.Add("table", DocumentManagement.GetDocumentsTable());
			data.Add("status", 1);
		}
		catch (Exception ex)
		{
			ErrorLog.save_to_error_log(ex);
			data.Add("status", 0);
		}

		HttpContext.Current.Response.Write(json.Serialize(data));
	}
}