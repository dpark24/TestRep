using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Ionic.Zip;
using Ionic.Zlib;
using TDOT;

public partial class GetFile : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		if (!VirtualPathUtility.GetDirectory(HttpContext.Current.Request.Path).Contains("/Account/") && !HttpContext.Current.User.Identity.IsAuthenticated)
		{
			HttpContext.Current.Response.Redirect("~/Account/Login.aspx");
		}
    }

	public static void get_requested_file()
	{
        string file_ids = String.Empty;
		List<SqlParameter> parameters = new List<SqlParameter>();
        Dictionary<string, string> meta = new Dictionary<string,string>();
		string zip_name;
		Dictionary<string, string> file_names = new Dictionary<string, string>();

		if(HttpContext.Current.User.Identity.IsAuthenticated)
		{
			try
			{
				file_ids = HttpContext.Current.Request.QueryString["files"];

				if (file_ids.Length > 0)
				{
					if (file_ids.Contains(","))
					{
                        using (SqlConnection conn = new SqlConnection(TDOTDB.conn_string))
						{
							using (SqlCommand cmd = new SqlCommand())
							{
								conn.Open();

								cmd.Connection = conn;
								cmd.CommandType = CommandType.StoredProcedure;
								cmd.CommandText = StoredProcedures.Files.GetById;
								cmd.Parameters.AddWithValue("@file_ids", file_ids);

								try
								{
									SqlDataReader row = cmd.ExecuteReader();
									while (row.Read())
									{
										//first the full path/name.. then the nice name
										file_names.Add(row["file_path"].ToString() + row["file_unique_name"].ToString(), row["file_name"].ToString());
									}
								}
								catch (Exception ex)
								{
									ErrorLog.save_to_error_log(ex);
								}
							}
						}

                        zip_name = String.Format("TDOTPortalArchive-{0}.zip", DateTime.Now.ToString("yyyyMMdd-HHmmss"));

						//zipping the files
						HttpContext.Current.Response.Clear();

						// no buffering - allows large zip files to download as they are zipped
						HttpContext.Current.Response.BufferOutput = false;
						HttpContext.Current.Response.ContentType = "application/zip";

						HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + zip_name);
						using (ZipFile zip = new ZipFile())
						{
							// add the set of files to the zip

							//todo: create list of files that weren't found and then add them to the zip file as a readme
							foreach (var f in file_names)
							{
								if (File.Exists(f.Key))
								{
									zip.AddFile(f.Key, "").FileName = f.Value;
								}
							}

							// compress and write the output to OutputStream
							zip.Save(HttpContext.Current.Response.OutputStream);
						}

						HttpContext.Current.Response.Close();
					}
					else
					{
                        meta = TDOTDB.sp_get_row(StoredProcedures.Files.GetById, new SqlParameter("@file_ids", file_ids));
						HttpContext.Current.Response.Clear();

						HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=" + meta["file_name"]);
						HttpContext.Current.Response.ContentType = "application/octet-stream";
						HttpContext.Current.Response.Expires = -1;
						HttpContext.Current.Response.TransmitFile(meta["file_path"] + meta["file_unique_name"]);

						HttpContext.Current.Response.Flush();
						HttpContext.Current.Response.Close();
					}
				}
				else
				{
					HttpContext.Current.Response.Write("There was an error..");
				}
			}
			catch (Exception ex)
			{
				HttpContext.Current.Response.Clear();
				HttpContext.Current.Response.ClearHeaders();
				HttpContext.Current.Response.ContentType = "text/plain";

				//HttpContext.Current.Response.Write(ex.ToString());
				HttpContext.Current.Response.Write("Sorry, we've encountered an error. Please contact your system administrator.");

				ErrorLog.save_to_error_log(ex);
				HttpContext.Current.Response.Close();
			}
		}
		else
		{
			HttpContext.Current.Response.Clear();
			HttpContext.Current.Response.ClearHeaders();
			HttpContext.Current.Response.ContentType = "text/plain";

			HttpContext.Current.Response.Write("Sorry, it appears that you are not authorized to access this file. If you believe you're receiving this message in error, please contact the system administrator.");
			HttpContext.Current.Response.Close();
		}
    }
}