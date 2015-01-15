using System.Linq;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Text;
using System.IO;
using System.Data;
using System.Globalization;
using System.Configuration;
using System.Security.Cryptography;
using AccountProfiles;
using System.Web.Profile;
using TDOT;
using System.Web.Security;
using System.Drawing.Drawing2D;

/// <summary>
/// Summary description for Objects
/// </summary>
namespace TDOT
{
	public class ErrorLog
	{
		public static void save_to_error_log(Exception ex)
		{
			string error_file = HttpContext.Current.Server.MapPath("./") + "errors.txt";
			StreamWriter writer;
			System.IO.FileInfo current_page = new System.IO.FileInfo(HttpContext.Current.Request.PhysicalPath);

			try
			{
				if (!System.IO.File.Exists(error_file))
				{
					writer = System.IO.File.CreateText(error_file);
				}
				else
				{
					writer = new StreamWriter(error_file, true);
				}

				writer.WriteLine("================");
				writer.WriteLine("Page Requested: " + current_page.FullName);
				writer.WriteLine("Error: " + DateTime.Today.ToShortDateString() + " || " + DateTime.Now.ToLongTimeString());
				writer.WriteLine("Message: " + ex.Message);
				writer.WriteLine("Source: " + ex.Source);
				writer.WriteLine("Full: " + ex.ToString());
				writer.Close();
			}
			catch (Exception e)
			{
			}
		}
	}

	public class RoleType
	{
		public string Name {get; set;}
		public string RoleLevel { get; set; }
	}

	public class AppRoles
	{
		public class Hierarchy
		{
			public Dictionary<string, int> Roles
			{
				get
				{
					Dictionary<string, int> roles = new Dictionary<string,int>();

					roles.Add(Administrator.Name, Administrator.RoleLevel);
					roles.Add(Editor.Name, Editor.RoleLevel);

					return roles;
				}
			}

			public bool GetHighestRole()
			{
				return false;
			}
		}

		public class Administrator : RoleType
		{
			public static string Name { get { return "Administrator"; } }
			public static int RoleLevel { get { return 100; } }
		}

		public class Editor : RoleType
		{
			public static string Name { get { return "Editor"; } }
			public static int RoleLevel { get { return 50; } }
		}
	}

	public class DocumentCategory
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
        public string Path { get; set; }
        public string FlatOrder { get; set; }
        public int Level { get; set; }
		public string ModifiedBy { get; set; }
		public DateTime ModifiedOn { get; set; }
		public bool Deleted { get; set; }

		public DocumentCategory()
		{
			this.Id = 0;
			this.Name = "";
			this.Description = "";
			this.ModifiedBy = HttpContext.Current.User.Identity.Name;
			this.ModifiedOn = DateTime.Today;
			this.Deleted = false;
		}

		public DocumentCategory(SqlDataReader row)
		{
			DateTime modified_date;

			this.Id = Int32.Parse(row["category_id"].ToString());
			this.Name = row["category_name"].ToString();
			this.Description = row["category_description"].ToString();
			this.ModifiedBy = row["category_modified_by"].ToString();

			DateTime.TryParse(row["category_modified_on"].ToString(), out modified_date);
			this.ModifiedOn = modified_date;
			this.Deleted = Int32.Parse(row["category_deleted"].ToString()) > 0 ? true : false;

            if (row.HasColumn("category_flat_order"))
            {
                this.FlatOrder = row["category_flat_order"].ToString();
            }

            if (row.HasColumn("category_level"))
            {
                this.Level = Int32.Parse(row["category_level"].ToString());
            }

            if (row.HasColumn("category_path"))
            {
                this.Path = row["category_path"].ToString();
            }
		}
	}

	public class Organization
	{
		public int Id { get; set; }
		public int TypeId { get; set; }
        public int Order { get; set; }
		public string Name { get; set; }
		public string ModifiedBy { get; set; }
		public DateTime ModifiedOn { get; set; }
		public bool Deleted { get; set; }

		public Organization()
		{
			this.Id = 0;
			this.TypeId = 0;
            this.Order = 0;
			this.Name = "";
			this.ModifiedBy = HttpContext.Current.User.Identity.Name;
			this.ModifiedOn = DateTime.Today;
			this.Deleted = false;
		}

		public Organization(SqlDataReader row)
		{
			DateTime modified_date;

			this.Id = Int32.Parse(row["organization_id"].ToString());
			this.TypeId = Int32.Parse(row["type_id"].ToString());
            this.Order = Int32.Parse(row["organization_order"].ToString());
			this.Name = row["organization_name"].ToString();
			this.ModifiedBy = row["organization_modified_by"].ToString();

			DateTime.TryParse(row["organization_modified_on"].ToString(), out modified_date);
			this.ModifiedOn = modified_date;
			this.Deleted = Int32.Parse(row["organization_deleted"].ToString()) > 0 ? true : false;
		}
	}

    public class Region
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Region()
        {
            this.Id = 0;
            this.Name = "";
        }

        public Region(SqlDataReader row)
        {
            this.Id = Int32.Parse(row["region_id"].ToString());
            this.Name = row["region_name"].ToString();
        }

        public Region(Dictionary<string, string> row)
        {
            this.Id = Int32.Parse(row["region_id"]);
            this.Name = row["region_name"];
        }
    }

    public class County
    {
        public int Id { get; set; }
        public int RegionId { get; set; }
        public string Name { get; set; }
        public string RegionName { get; set; }

        public County()
        {
            this.Id = 0;
            this.RegionId = 0;
            this.Name = "";
            this.RegionName = "";
        }

        public County(SqlDataReader row)
        {
            this.Id = Int32.Parse(row["county_id"].ToString());
            this.RegionId = Int32.Parse(row["region_id"].ToString());
            this.Name = row["county_name"].ToString();

            if (row.HasColumn("region_name"))
            {
                this.RegionName = row["region_name"].ToString();
            }
        }

        public County(Dictionary<string, string> row)
        {
            this.Id = Int32.Parse(row["county_id"]);
            this.RegionId = Int32.Parse(row["region_id"]);
            this.Name = row["county_name"];

            if (row.ContainsKey("region_name"))
            {
                this.RegionName = row["region_name"];
            }
        }
    }

	public class Utilities
	{
		public static string GetUniqueKey()
		{
			int maxSize = 10;
			char[] chars = new char[62];
			string a;
			int size = maxSize;
			byte[] data = new byte[1];
			RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
			StringBuilder result = new StringBuilder();

			a = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
			chars = a.ToCharArray();
			
			crypto.GetNonZeroBytes(data);
			size =  maxSize;
			data = new byte[size];
			crypto.GetNonZeroBytes(data);
			result = new StringBuilder(size);

			foreach(byte b in data )
			{
				result.Append(chars[b % (chars.Length - 1)]); 
			}
			
			return result.ToString();
		}

        public void ResizeImage(double scale_factor, Stream from_stream, Stream to_stream)
        {
            var image = System.Drawing.Image.FromStream(from_stream);
            var newWidth = (int)(image.Width * scale_factor);
            var newHeight = (int)(image.Height * scale_factor);
            var thumbnailBitmap = new Bitmap(newWidth, newHeight);

            var thumbnailGraph = Graphics.FromImage(thumbnailBitmap);
            thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
            thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
            thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

            var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
            thumbnailGraph.DrawImage(image, imageRectangle);

            thumbnailBitmap.Save(to_stream, image.RawFormat);

            thumbnailGraph.Dispose();
            thumbnailBitmap.Dispose();
            image.Dispose();
        }

        public static string ThumbnailImage(string file_name, string image_path, int max_width, int max_height)
        {
            System.Drawing.Image original_image;
            System.Drawing.Image.GetThumbnailImageAbort abort_callback = new System.Drawing.Image.GetThumbnailImageAbort(AbortThumbnailCallback);
            System.Drawing.Image image_to_save;

            string file_path = HttpContext.Current.Server.MapPath("./") + "document_library\\";
            string new_name;

            float ratio;
            int final_width = 0;
            int final_height = 0;

            try
            {
                original_image = System.Drawing.Image.FromFile(image_path);

                ratio = (float)original_image.Width / (float)original_image.Height;
                final_width = 0;
                final_height = 0;

                if (ratio < 1)
                {
                    final_width = max_width;
                    final_height = (int)(max_width * ratio);
                }
                else
                {
                    final_height = max_height;
                    final_width = (int)(max_height * ratio);
                }

                if (file_name.Contains("."))
                {
                    new_name = file_name.Insert(file_name.LastIndexOf('.'), "-" + final_width.ToString() + "x" + final_height.ToString());
                }
                else
                {
                    new_name = file_name + "-" + final_width.ToString() + "x" + final_height.ToString();
                }

                file_path = file_path + new_name;

                //need to check for and deal with files existing/overwrites
                if (!File.Exists(file_path))
                {
                    //file_path = Utilities.GetUniqueKey();
                    if (final_width > 0 && final_height > 0)
                    {
                        image_to_save = original_image.GetThumbnailImage(final_width, final_height, abort_callback, IntPtr.Zero);

                        image_to_save.Save(file_path, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
                else
                {
                    new_name = file_name;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.save_to_error_log(ex);
                new_name = file_name;
            }

            return new_name;
        }

        private static bool AbortThumbnailCallback() { return false; }

		public static string GetUserTable()
		{
			StringBuilder html = new StringBuilder();
			MembershipUserCollection all_users = Membership.GetAllUsers();
			string[] user_roles;
			string[] all_roles = Roles.GetAllRoles();
			string nice_name;

			foreach (MembershipUser user in all_users)
			{
				user_roles = Roles.GetRolesForUser(user.UserName);
				nice_name = AccountProfile.GetUserProfile(user.UserName).LastName + ", " + AccountProfile.GetUserProfile(user.UserName).FirstName;

				html.Append("<tr data-username=\"" + user.UserName + "\">");
				html.Append("<td class=\"center-text\">"); //<a href=\"#\" title=\"Manage this user's status and information\" class=\"manage-user-access\">Manage</a><br />");
				html.Append("<a href=\"#\" class=\"delete-user-account\" data-user_name=\"" + user.UserName + "\" title=\"Delete this user\"><span class=\"margin-center ui-icon ui-icon-trash\"></span></a></td>");
				html.Append("<td class=\"center-text\"><a href=\"#\" class=\"toggle-user-approval\" data-user_name=\"" + user.UserName + "\"><span class=\"tooltip-dark-right margin-center ui-icon ui-icon-");
				html.Append((user.IsApproved ? "check" : "notice") + "\" title=\"This user is " + (user.IsApproved ? "" : "not ") + "Approved\"></span></a></td>");
				html.Append("<td><a href=\"mailto:" + user.Email + "\" title=\"Send an Email to " + user.UserName + "\">" + user.UserName + "</a></td>");

				foreach (string r in all_roles)
				{
					html.Append("<td data-role_level=\"" + r + "\"><span class=\"sprite-block\">");
					html.Append("<span class=\"access-level-toggle sprite-" + (Array.IndexOf(user_roles, r) >= 0 ? "accept" : "delete") + "\"></span></span></td>");
				}

				html.Append("<td>" + user.LastLoginDate.ToShortDateString() + "</td>");
				html.Append("</tr>");
			}

			return html.ToString();
		}
	}

	public class DocumentManagement
	{
		public class ProjectFile
		{
			public int Id { get; set; }
			public int CategoryId { get; set; }
			public int OrganizationId { get; set; }
            public int CountyId { get; set; }
            public string Pin { get; set; }
            public string TdecPermit { get; set; }
            public string USACEPermit { get; set; }
            public string PENumber { get; set; }
            public string PENumber2 { get; set; }
            public string MonitoringYear { get; set; }
            public string Title { get; set; }
			public string Name { get; set; }
			public string Description { get; set; }
			public string UniqueName { get; set; }
			public string Type { get; set; }
			public string FilePath { get; set; }
			public int Size { get; set; }
			public string ModifiedBy { get; set; }
			public DateTime ModifiedOn { get; set; }
			public bool Deleted { get; set; }
            public Dictionary<string, string> Meta { get; set; }
			public HttpPostedFile PostedFile { get; set; }

			public ProjectFile()
			{
				this.Id = 0;
				this.CategoryId = 0;
				this.OrganizationId = 0;
                this.CountyId = 0;
                this.Pin = "";
                this.TdecPermit = "";
                this.USACEPermit = "";
                this.PENumber = "";
                this.PENumber2 = "";
                this.MonitoringYear = "N/A";
				this.Title = "";
				this.Name = "";
				this.Description = "";
				this.UniqueName = Utilities.GetUniqueKey();
				this.Type = "";
				this.FilePath = "";
				this.Size = 0;
				this.ModifiedBy = HttpContext.Current.User.Identity.Name;
				this.ModifiedOn = DateTime.Today;
				this.Deleted = false;

                this.Meta = new Dictionary<string, string>();
				this.PostedFile = null;
			}

			public ProjectFile(SqlDataReader row)
			{
				DateTime modified_on;

                this.Id = Int32.Parse(row["file_id"].ToNullableString("-1"));
                this.CategoryId = Int32.Parse(row["category_id"].ToNullableString("-1"));
                this.OrganizationId = Int32.Parse(row["organization_id"].ToNullableString("-1"));

                this.CountyId = Int32.Parse(row["county_id"].ToNullableString("-1"));
                this.Pin = row["file_pin"].ToNullableString();
                this.TdecPermit = row["file_tdec_permit"].ToNullableString();
                this.USACEPermit = row["file_usace_permit"].ToNullableString();
                this.PENumber = row["file_pe_number"].ToNullableString();
                this.PENumber2 = row["file_pe_number2"].ToNullableString();
                this.MonitoringYear = row["file_monitoring_year"].ToNullableString();

                this.Title = row["file_title"].ToNullableString();
                this.Name = row["file_name"].ToNullableString();
                this.Description = row["file_description"].ToNullableString();
                this.UniqueName = row["file_unique_name"].ToNullableString();
                this.Type = row["file_type"].ToNullableString();
                this.FilePath = row["file_path"].ToNullableString();
                this.Size = Int32.Parse(row["file_size"].ToNullableString("-1"));

                this.ModifiedBy = row["file_modified_by"].ToNullableString();
                DateTime.TryParse(row["file_modified_on"].ToNullableString(), out modified_on);
                this.ModifiedOn = modified_on;

                this.Meta = row.ToDictionary();
                this.PostedFile = null;

			}

            public ProjectFile(Dictionary<string, string> meta)
            {
                DateTime modified_on;

                this.Id = Int32.Parse(meta["file_id"]);
                this.CategoryId = Int32.Parse(meta["category_id"]);
                this.OrganizationId = Int32.Parse(meta["organization_id"]);

                this.CountyId = Int32.Parse(meta["county_id"]);
                this.Pin = meta["file_pin"];
                this.TdecPermit = meta["file_tdec_permit"];
                this.USACEPermit = meta["file_usace_permit"];
                this.PENumber = meta["file_pe_number"];
                this.PENumber2 = meta["file_pe_number2"];
                this.MonitoringYear = meta["file_monitoring_year"];

                this.Title = meta["file_title"];
                this.Name = meta["file_name"];
                this.Description = meta["file_description"];
                this.UniqueName = meta["file_unique_name"];
                this.Type = meta["file_type"];
                this.FilePath = meta["file_path"];
                this.Size = Int32.Parse(meta["file_size"]);

                this.ModifiedBy = meta["file_modified_by"];

                if (!DateTime.TryParse(meta["file_modified_on"], out modified_on))
                {
                    modified_on = DateTime.MinValue;
                }
                this.ModifiedOn = modified_on;

                this.Meta = meta;
                this.PostedFile = null;
            }

            public string GetMeta(string key)
            {
                string result = "";

                if (this.Meta.ContainsKey(key))
                {
                    result = this.Meta[key];
                }

                return result;
            }

			public string GetExtensionImageClass()
			{
				string ext_class = "";
				string ext = "";

				if (this.Name.Length > 0)
				{
					ext = Path.GetExtension(this.Name);

					switch (ext)
					{
						case ".pdf":
							{
								ext_class = "pdf-file";
								break;
							}
						case ".xls":
						case ".xlsx":
							{
								ext_class = "xls-file";
								break;
							}
						case ".doc":
						case ".docx":
							{
								ext_class = "doc-file";
								break;
							}
						case ".ppt":
						case ".pptx":
							{
								ext_class = "ppt-file";
								break;
							}
						default:
							{
								ext_class = "";
								break;
							}
					}
				}

				return ext_class;
			}

			public bool Save()
			{
				bool status = false;
				bool is_uploaded = false;
				string path;
				List<SqlParameter> parameters = new List<SqlParameter>();
				string basepath = HttpContext.Current.Server.MapPath("./");
				List<string> file_names = new List<string>();
				string file_name = "";
				int counter = 0;

				//need to fix up the path
				path = basepath + "document_library\\";

				//need to check for and deal with files existing/overwrites
				while (File.Exists(path + this.UniqueName))
				{
					this.UniqueName = Utilities.GetUniqueKey();
				}

				//name it just like the user, then make sure it's kosher
				this.Name = this.PostedFile.FileName;

				this.Name = this.Name.Trim().Replace(" ", "_");
				foreach(char c in System.IO.Path.GetInvalidFileNameChars())
				{
					if(this.Name.Contains(c.ToString()))
					{
						this.Name.Replace(c.ToString(), "");
					}
				}

				//go get all the file names currently in the database
                file_names = TDOTDB.sp_get_column(StoredProcedures.Files.GetAll, "file_name");

				//hold on to our name temporarily
				file_name = this.Name;

				//if that name already exists in the database, we're going to create a new one
				// note that this doesn't take folder locations into account
				while (file_names.Contains(this.Name))
				{
					counter++;

					//create our new name
					this.Name = System.IO.Path.GetFileNameWithoutExtension(file_name) + "_" + counter + System.IO.Path.GetExtension(file_name);
				}

				parameters.Add(new SqlParameter("@category_id", this.CategoryId));
				parameters.Add(new SqlParameter("@organization_id", this.OrganizationId));
                parameters.Add(new SqlParameter("@county_id", this.CountyId));
                parameters.Add(new SqlParameter("@file_pin", this.Pin));
                parameters.Add(new SqlParameter("@file_tdec_permit", this.TdecPermit));
                parameters.Add(new SqlParameter("@file_usace_permit", this.USACEPermit));
                parameters.Add(new SqlParameter("@file_pe_number", this.PENumber));
                parameters.Add(new SqlParameter("@file_pe_number2", this.PENumber2));
                parameters.Add(new SqlParameter("@file_monitoring_year", this.MonitoringYear));

				parameters.Add(new SqlParameter("@file_title", this.Title));
				parameters.Add(new SqlParameter("@file_description", this.Description));
				parameters.Add(new SqlParameter("@file_name", this.Name));
				parameters.Add(new SqlParameter("@file_unique_name", this.UniqueName));
				parameters.Add(new SqlParameter("@file_type", this.PostedFile.ContentType));
				parameters.Add(new SqlParameter("@file_path", path));
				parameters.Add(new SqlParameter("@file_size", this.PostedFile.ContentLength));
				parameters.Add(new SqlParameter("@file_modified_on", DateTime.Today.ToShortDateString()));
				parameters.Add(new SqlParameter("@file_modified_by", HttpContext.Current.User.Identity.Name));

                is_uploaded = (TDOTDB.sp_query(StoredProcedures.Files.Insert, parameters) > 0) ? true : false;

				if (is_uploaded)
				{
					//get the current location of the requested file
					try
					{
						if(!System.IO.Directory.Exists(path))
						{
							System.IO.Directory.CreateDirectory(path);
						}
						
						this.PostedFile.SaveAs(path + this.UniqueName);
						status = true;
					}
					catch (Exception ex)
					{
						status = false;
						ErrorLog.save_to_error_log(ex);
					}
				}
				else
				{
					status = false;
				}

				return status;
			}

			/// <summary>
			/// Updates the metadata associated with a file. Does not change the file or it's discreet attributes
			/// </summary>
			/// <returns>True or false depending on success. If an error is thrown, it is recorded in the error log.</returns>
			public bool Update()
			{
				bool status = false;
				List<SqlParameter> parameters = new List<SqlParameter>();

				try
				{
                    parameters.Add(new SqlParameter("@file_id", this.Id));
					parameters.Add(new SqlParameter("@category_id", this.CategoryId));
                    parameters.Add(new SqlParameter("@organization_id", this.OrganizationId));
                    parameters.Add(new SqlParameter("@county_id", this.CountyId));
                    parameters.Add(new SqlParameter("@file_pin", this.Pin));
                    parameters.Add(new SqlParameter("@file_tdec_permit", this.TdecPermit));
                    parameters.Add(new SqlParameter("@file_usace_permit", this.USACEPermit));
                    parameters.Add(new SqlParameter("@file_pe_number", this.PENumber));
                    parameters.Add(new SqlParameter("@file_pe_number2", this.PENumber2));
                    parameters.Add(new SqlParameter("@file_monitoring_year", this.MonitoringYear));
					parameters.Add(new SqlParameter("@file_title", this.Title));
					parameters.Add(new SqlParameter("@file_description", this.Description));
					parameters.Add(new SqlParameter("@file_name", this.Name));
                    parameters.Add(new SqlParameter("@file_size", this.Size));
					parameters.Add(new SqlParameter("@modified_on", DateTime.Today.ToShortDateString()));
					parameters.Add(new SqlParameter("@modified_by", HttpContext.Current.User.Identity.Name));

                    status = (TDOTDB.sp_query(StoredProcedures.Files.Update, parameters) > 0) ? true : false;
				}
				catch(Exception ex)
				{
					status = false;
					ErrorLog.save_to_error_log(ex);
				}

				return status;
			}

            /// <summary>
            /// Delets the record and file
            /// </summary>
            /// <returns>True or false depending on success. If an error is thrown, it is recorded in the error log.</returns>
            public bool Delete()
            {
                bool status = false;
                List<SqlParameter> parameters = new List<SqlParameter>();

                try
                {
                    parameters.Add(new SqlParameter("@file_id", this.Id));
                    status = (TDOTDB.sp_query(StoredProcedures.Files.Delete, parameters) > 0) ? true : false;
                }
                catch (Exception ex)
                {
                    status = false;
                    ErrorLog.save_to_error_log(ex);
                }

                return status;
            }

			/// <summary>
			/// Gets an List of PlainsFile objects based on the given search text
			/// </summary>
			/// <param name="search_text">Text to search for in the stored procedure</param>
			/// <returns>List of Plains Files</returns>
			public static List<DocumentManagement.ProjectFile> GetFiles(string search_text)
			{
				List<DocumentManagement.ProjectFile> Files = new List<DocumentManagement.ProjectFile>();
				DocumentManagement.ProjectFile File = new DocumentManagement.ProjectFile();

                using (SqlConnection conn = new SqlConnection(TDOTDB.conn_string))
				{
					using (SqlCommand cmd = new SqlCommand())
					{
						conn.Open();

						cmd.Connection = conn;
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.CommandText = StoredProcedures.Files.SearchSimple;
						cmd.Parameters.AddWithValue("@search_text", search_text);

						try
						{
							SqlDataReader row = cmd.ExecuteReader();
							while (row.Read())
							{
								File = new DocumentManagement.ProjectFile(row);
								Files.Add(File);
							}
						}
						catch (Exception ex)
						{
							ErrorLog.save_to_error_log(ex);
						}
					}
				}

				return Files;
			}

			private static string _get_file_subtree(List<DocumentManagement.ProjectFile> FileList)
			{
				StringBuilder html = new StringBuilder();

				foreach (DocumentManagement.ProjectFile PlainsFile in FileList)
				{
					html.Append("<li data-file_id=\"" + PlainsFile.Id + "\"><div class=\"file-container\"><span class=\"file " + PlainsFile.GetExtensionImageClass() + "\"></span>");
					html.Append("<span class=\"name\">" + PlainsFile.Name + "</span>");
					html.Append("<span class=\"description hidden\">" + PlainsFile.Description + "</li>");
				}

				return html.ToString();
			}

			/// <summary>
			/// Gets the CSS class type for a specified file type.
			/// </summary>
			/// <param name="ext">The file extension. Eg: .pdf</param>
			/// <returns>The CSS class for the file type.</returns>
			/// <remarks></remarks>
			public static string get_extension_image_class(string ext)
			{
				string ext_class = "";

				switch (ext)
				{
					case ".pdf":
						{
							ext_class = "pdf-file";
							break;
						}
					case ".xls":
					case ".xlsx":
						{
							ext_class = "xls-file";
							break;
						}
					case ".doc":
					case ".docx":
						{
							ext_class = "doc-file";
							break;
						}
					case ".ppt":
					case ".pptx":
						{
							ext_class = "ppt-file";
							break;
						}
					default:
						{
							ext_class = "";
							break;
						}
				}

				return ext_class;
			}

			/// <summary>
			/// Gets the HTML LI element for addition into a UL tag for a file tree
			/// </summary>
			/// <param name="meta">The file metadata.</param>
			/// <returns>Return a HTML string containing the LI item with the file metadata</returns>
			/// <remarks></remarks>
			public static string get_file_tree_html(ProjectFile f, bool show)
			{
				StringBuilder html = new StringBuilder();

				html.Append("<li id=\"file-" + f.Id + "\">");
				html.Append("<span class=\"file " + f.GetExtensionImageClass() + "\"></span>");
				html.Append("<a href=\"" + VirtualPathUtility.ToAbsolute("~/" + f.FilePath + f.Name) + "\" title=\"View File\" class=\"external\">");
				html.Append(Path.GetFileName(f.Name) + "</a> " + (show ? "" : "{<a href=\"#\" class=\"show-file-details\">Details</a>}"));
				html.Append("<div class=\"file-details " + (show ? "" : "hidden") + "\">");
				html.Append("<div class=\"file-info\"><strong>Title: </strong><span class=\"file-title\">" + f.Title + "</span></div>");
				html.Append("<div class=\"file-description\"><strong>Description: </strong><span>" + f.Description + "</span></div>");
				html.Append("<div><em>Uploaded on " + f.ModifiedOn.ToShortDateString() + " by " + f.ModifiedBy + "</em></div>");
				html.Append("</div></li>");

				return html.ToString();
			}

			/// <summary>
			/// Gets the HTML LI element for addition into a UL tag for a file tree
			/// </summary>
			/// <param name="meta">The file metadata.</param>
			/// <param name="editable">Indicates if the file metadata should be editable</param>
			/// <returns>Return a HTML string containing the LI item with the file metadata</returns>
			/// <remarks></remarks>
			public static string get_file_tree_html(ProjectFile f, bool editable, bool show)
			{
				StringBuilder html = new StringBuilder();

				if (editable)
				{
					html.Append("<li id=\"file-" + f.Id + "\">");
					html.Append("<span class=\"file " + f.GetExtensionImageClass() + "\"></span>");
					html.Append("<a href=\"" + VirtualPathUtility.ToAbsolute("~/" + f.FilePath + f.Name) + "\" title=\"View File\" class=\"external\">");
					html.Append(Path.GetFileName(f.Name) + "</a> " + (show ? "" : "{<a href=\"#\" class=\"show-file-details\">Details</a>}"));
					html.Append("<div class=\"file-details " + (show ? "" : "hidden") + "\">");

					html.Append("<div class=\"fright\"><a href=\"#\" class=\"file-delete redtext smaller\" id=\"delete-" + f.Id + "\">Delete</a> ");
					html.Append("<a href=\"#\" class=\"sexybutton sexysimple sexygreen sexysmall file-edit\" id=\"edit-" + f.Id + "\">Edit Details</a></div>");

					html.Append("<div class=\"file-info\"><strong>Title: </strong><span class=\"file-title\">" + f.Title + "</span></div>");
					html.Append("<div class=\"file-description\"><strong>Description: </strong><span>" + f.Description + "</span></div>");
					html.Append("<div><em>Uploaded on " + f.ModifiedOn.ToShortDateString() + " by " + f.ModifiedBy + "</em></div>");
					html.Append("</div></li>");
				}
				else
				{
					html.Append("<li id=\"file-" + f.Id + "\">");
					html.Append("<span class=\"file " + f.GetExtensionImageClass() + "\"></span>");
					html.Append("<a href=\"" + VirtualPathUtility.ToAbsolute("~/" + f.FilePath + f.Name) + "\" title=\"View File\" class=\"external\">");
					html.Append(Path.GetFileName(f.Name) + "</a> " + (show ? "" : "{<a href=\"#\" class=\"show-file-details\">Details</a>}"));
					html.Append("<div class=\"file-details " + (show ? "" : "hidden") + "\">");
					
					html.Append("<div class=\"file-info\"><strong>Title: </strong><span class=\"file-title\">" + f.Title + "</span></div>");
					html.Append("<div class=\"file-description\"><strong>Description: </strong><span>" + f.Description + "</span></div>");
					html.Append("<div><em>Uploaded on " + f.ModifiedOn.ToShortDateString() + " by " + f.ModifiedBy + "</em></div>");
					html.Append("</div></li>");
				}

				return html.ToString();
			}
		}

        public static Dictionary<int,DocumentCategory> GetCategoryHierarchy()
        {
            Dictionary<int, DocumentCategory> Categories = new Dictionary<int, DocumentCategory>();
            DocumentCategory Category;

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

                            if (!Categories.ContainsKey(Category.Id))
                            {
                                Categories.Add(Category.Id, Category);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.save_to_error_log(ex);
                    }
                }
            }

            return Categories;
        }

		public static string GetDocumentsTable()
		{
			StringBuilder html = new StringBuilder();
			DocumentManagement.ProjectFile File = new DocumentManagement.ProjectFile();
			decimal size_kb = 0;
			string excerpt = "";
			bool shortened = false;
			int max_length = 100;
            int category_id;

            Dictionary<int, DocumentCategory> Categories = GetCategoryHierarchy();

            using (SqlConnection conn = new SqlConnection(TDOTDB.conn_string))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					conn.Open();

					cmd.Connection = conn;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = StoredProcedures.Files.GetAll;

					try
					{
						SqlDataReader row = cmd.ExecuteReader();
						while (row.Read())
						{
							File = new DocumentManagement.ProjectFile(row);
							shortened = false;
                            size_kb = (decimal)File.Size / (decimal)1024;
                            category_id = Int32.Parse(row["category_id"].ToString());

							html.Append("<tr id=\"file-" + File.Id + "\" data-file_id=\"" + File.Id + "\">");
							html.Append("<td class=\"center-text\"><input type=\"checkbox\" class=\"add-to-download-package\" value=\"" + File.Id + "\" /></td>");

							html.Append("<td><div class=\"mh4\"><a href=\"GetFile.aspx?files=" + File.Id + "\" title=\"Click to download " + File.Title + "\">" + File.Title + "</a>");
							html.Append("<br />(<a href=\"GetFile.aspx?files=" + File.Id + "\" title=\"Click to download " + File.Name + "\" class=\"smaller\">" + File.Name + "</a>)</div>");

							if (File.Description.Length > max_length)
							{
								excerpt = File.Description.Substring(0, max_length);
								shortened = true;
							}

							if (shortened)
							{
								html.Append("<div class=\"excerpt\">" + excerpt + "[...]</div><a href=\"#\" title=\"Read the complete description\" class=\"toggle-full-text\">Show more</a>");
								html.Append(" <div class=\"full-text hidden\">" + File.Description + "</div>");
							}
							else
							{
								html.Append(File.Description);
							}

                            html.Append("</td>");
                            html.Append("<td>" + File.MonitoringYear + "</td>");
                            html.Append("<td>" + File.GetMeta("region_name") + "</td>");
                            html.Append("<td>" + File.GetMeta("county_name") + "</td>");
                            html.Append("<td>" + File.Pin + "</td>");
                            html.Append("<td>" + File.GetMeta("organization_name") + "</td>");
							html.Append("<td class=\"center-text\">" + row["category_name"].ToString() + "</td>");
							html.Append("<td class=\"center-text\">" + File.ModifiedOn.ToShortDateString() + "</td>");
                            html.Append("<td class=\"center-text\">" + File.ModifiedBy + "</td>");
							html.Append("</tr>");
						}
					}
					catch (Exception ex)
					{
						ErrorLog.save_to_error_log(ex);
					}
				}
			}

			return html.ToString();
		}
	}
}
