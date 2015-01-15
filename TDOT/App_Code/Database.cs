using System;
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

public static class DataRecordExtensions
{
	public static bool HasColumn(this IDataRecord dr, string columnName)
	{
		for (int i = 0; i < dr.FieldCount; i++)
		{
			if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

    /// <summary>
    /// Converts a IDataReader to a Dictionary(Of String, String)
    /// </summary>
    /// <param name="row">The base IDataRecord/SqlDataReader object</param>
    /// <returns>A dictionary of the database data with null values being ""</returns>
    /// <remarks></remarks>
    public static Dictionary<string, string> ToDictionary(this SqlDataReader row )
    {
        Dictionary<string, string> meta = new Dictionary<string, string>();
        int i = 0;
        int len = row.FieldCount - 1;

        for(i=0;i<len;i++)
        {
            if (!meta.ContainsKey(row.GetName(i)))
            {
                meta.Add(row.GetName(i), (row.IsDBNull(i)) ? "" : row[i].ToString());
            }
        }

        return meta;
    }
    /// <summary>
    /// Safely returns a string from the database. If the database value is "null", an empty string ("") is returned.
    /// </summary>
    /// <param name="dbcell">The database cell</param>
    /// <returns>A valid string</returns>
    /// <remarks></remarks>
    public static string ToNullableString(this object dbcell)
    {
        return ToNullableString(dbcell, "");
    }

    /// <summary>
    /// Safely returns a string from the database. If the database value is "null", an empty string ("") is returned.
    /// </summary>
    /// <param name="dbcell">The database cell</param>
    /// <param name="default_value">The default value to return if the cell is null</param>
    /// <returns>A valid string</returns>
    /// <remarks></remarks>
    public static string ToNullableString(this object dbcell, string default_value)
    {
        string val;

        val = (dbcell == null || dbcell == DBNull.Value) ? default_value : dbcell.ToString();

        return val;
    }
}

namespace TDOT
{
    public class TDOTDB
	{
		// Central reference to connection string text
		public static string conn_string
		{
			get
			{
                return ConfigurationManager.AppSettings["TDOTDatabase"];
			}
		}

		/// <summary>
		/// Use this function to return a single column of data as a list
		/// </summary>
		/// <param name="sql">sqlQuery String</param>
		/// <returns>List</returns>
		public static List<string> get_column(string sql)
		{
			using (SqlConnection conn = new SqlConnection(conn_string))
			{
				using (SqlCommand comm = new SqlCommand(sql, conn))
				{
					conn.Open();
					SqlDataReader row = comm.ExecuteReader();
					List<string> entries = new List<string>();

					int i = 0;
					while (row.Read())
					{
						entries.Add(row[0].ToString());
						i++;
					}

					return entries;
				}
			}
		}

		/// <summary>
		/// Fills a DataTable based on a SQL or stored procedure with no parameters
		/// </summary>
		/// <param name="sqlQuery">sqlQuery String</param>
		/// <returns>A filled datatable on successful retrieval or rows, an empty table otherwise</returns>
		public static DataTable get_datatable(string sql)
		{
			SqlCommand cmd = new SqlCommand();
			SqlDataAdapter data_adapter = new SqlDataAdapter();
			DataTable data_table = new DataTable();

			using (SqlConnection conn = new SqlConnection(conn_string))
			{
				conn.Open();

				cmd = new SqlCommand();

				cmd.Connection = conn;
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = sql;

				data_adapter.SelectCommand = cmd;
				data_adapter.Fill(data_table);
			}

			return data_table;
		}

		/// <summary>
		/// Fills a datatable based on the SQL or stored procedure provided and a sqlparameter key/values
		/// </summary>
		/// <param name="sqlQuery">The SQL string or the string of the stored procedure</param>
		/// <param name="parameter">A SqlParameter</param>
		/// <returns>A filled datatable on successful retrieval or rows, an empty table otherwise</returns>
		public static DataTable query(string sql, SqlParameter parameter)
		{
			SqlCommand cmd = new SqlCommand();
			SqlDataAdapter data_adapter = new SqlDataAdapter();
			DataTable data_table = new DataTable();

			using (SqlConnection conn = new SqlConnection(conn_string))
			{
				conn.Open();

				cmd = new SqlCommand();

				cmd.Connection = conn;
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = sql;
				cmd.Parameters.Add(parameter);

				data_adapter.SelectCommand = cmd;
				data_adapter.Fill(data_table);
			}

			return data_table;
		}

		/// <summary>
		/// Fills a datatable based on the SQL or stored procedure provided and a list of sqlparameter key/values
		/// </summary>
		/// <param name="sqlQuery">The SQL string or the string of the stored procedure</param>
		/// <param name="parameters">A list of SqlParameters</param>
		/// <returns>A filled datatable on successful retrieval or rows, an empty table otherwise</returns>
		public static DataTable query(string sql, List<SqlParameter> parameters)
		{
			SqlCommand cmd = new SqlCommand();
			SqlDataAdapter data_adapter = new SqlDataAdapter();
			DataTable data_table = new DataTable();

			using (SqlConnection conn = new SqlConnection(conn_string))
			{
				conn.Open();

				cmd = new SqlCommand();

				cmd.Connection = conn;
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = sql;

				foreach (SqlParameter p in parameters)
				{
					cmd.Parameters.Add(p);
				}

				data_adapter.SelectCommand = cmd;
				data_adapter.Fill(data_table);
			}

			return data_table;
		}

		public static int sp_query(string sp)
		{
			int affected_rows = 0;
			SqlParameter returned = new SqlParameter();

			using (SqlConnection conn = new SqlConnection(conn_string))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					conn.Open();

					cmd.Connection = conn;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = sp;

					returned.ParameterName = "@return_value";
					returned.Direction = ParameterDirection.ReturnValue;
					returned.DbType = DbType.Int32;
					cmd.Parameters.Add(returned);

					try
					{
						cmd.ExecuteNonQuery();
						affected_rows = Int32.Parse(cmd.Parameters["@return_value"].Value.ToString());
					}
					catch (SqlException ex)
					{
						ErrorLog.save_to_error_log(ex);
						affected_rows = (ex.Message.Contains("duplicate key")) ? -2 : 0;
					}
					catch (Exception ex)
					{
						ErrorLog.save_to_error_log(ex);
					}
				}
			}

			return affected_rows;
		}

		public static int sp_query(string sp, SqlParameter parameters)
		{
			int affected_rows = 0;
			SqlParameter returned = new SqlParameter();

			using (SqlConnection conn = new SqlConnection(conn_string))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					conn.Open();

					cmd.Connection = conn;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = sp;

					//add our parameters if they exist
					if (parameters != null)
					{
						cmd.Parameters.Add(parameters);
					}

					returned.ParameterName = "@return_value";
					returned.Direction = ParameterDirection.ReturnValue;
					returned.DbType = DbType.Int32;
					cmd.Parameters.Add(returned);

					try
					{
						cmd.ExecuteNonQuery();
						affected_rows = Int32.Parse(cmd.Parameters["@return_value"].Value.ToString());
					}
					catch (SqlException ex)
					{
						ErrorLog.save_to_error_log(ex);
						affected_rows = (ex.Message.Contains("duplicate key")) ? -2 : 0;
					}
					catch (Exception ex)
					{
						ErrorLog.save_to_error_log(ex);
					}
				}
			}

			return affected_rows;
		}

		public static int sp_query(string sp, List<SqlParameter> parameters)
		{
			int affected_rows = 0;
			SqlParameter returned = new SqlParameter();

			using (SqlConnection conn = new SqlConnection(conn_string))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					conn.Open();

					cmd.Connection = conn;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = sp;

					//add our parameters if they exist
					if (parameters != null)
					{
						foreach (SqlParameter p in parameters)
						{
							cmd.Parameters.Add(p);
						}
					}

					returned.ParameterName = "@return_value";
					returned.Direction = ParameterDirection.ReturnValue;
					returned.DbType = DbType.Int32;
					cmd.Parameters.Add(returned);

					try
					{
						cmd.ExecuteNonQuery();
						affected_rows = Int32.Parse(cmd.Parameters["@return_value"].Value.ToString());
					}
					catch (SqlException ex)
					{
						ErrorLog.save_to_error_log(ex);
						affected_rows = (ex.Message.Contains("duplicate key")) ? -2 : 0;
					}
					catch (Exception ex)
					{
						ErrorLog.save_to_error_log(ex);
					}
				}
			}

			return affected_rows;
		}

		public static Dictionary<string, string> sp_get_row(string sp)
		{
			Dictionary<string, string> data_row = new Dictionary<string, string>();

			using (SqlConnection conn = new SqlConnection(conn_string))
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
							for (int i = 0; i < row.FieldCount; i++)
							{
								if (!data_row.ContainsKey(row.GetName(i)))
								{
									data_row.Add(row.GetName(i), (row.IsDBNull(i)) ? "" : row[i].ToString());
								}
							}
						}
					}
					catch (Exception ex)
					{
						ErrorLog.save_to_error_log(ex);
					}
				}
			}

			return data_row;
		}

		public static Dictionary<string, string> sp_get_row(string sp, SqlParameter parameter)
		{
			Dictionary<string, string> data_row = new Dictionary<string, string>();

			using (SqlConnection conn = new SqlConnection(conn_string))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					conn.Open();

					cmd.Connection = conn;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = sp;
					cmd.Parameters.Add(parameter);

					try
					{
						SqlDataReader row = cmd.ExecuteReader();

						while (row.Read())
						{
							for (int i = 0; i < row.FieldCount; i++)
							{
								if (!data_row.ContainsKey(row.GetName(i)))
								{
									data_row.Add(row.GetName(i), (row.IsDBNull(i)) ? "" : row[i].ToString());
								}
							}
						}
					}
					catch (Exception ex)
					{
						ErrorLog.save_to_error_log(ex);
					}
				}
			}

			return data_row;
		}

		public static Dictionary<string, string> sp_get_row(string sp, List<SqlParameter> parameters)
		{
			Dictionary<string, string> data_row = new Dictionary<string, string>();

			using (SqlConnection conn = new SqlConnection(conn_string))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					conn.Open();

					cmd.Connection = conn;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = sp;

					foreach (SqlParameter p in parameters)
					{
						cmd.Parameters.Add(p);
					}

					try
					{
						SqlDataReader row = cmd.ExecuteReader();

						while (row.Read())
						{
							for (int i = 0; i < row.FieldCount; i++)
							{
								if (!data_row.ContainsKey(row.GetName(i)))
								{
									data_row.Add(row.GetName(i), (row.IsDBNull(i)) ? "" : row[i].ToString());
								}
							}
						}
					}
					catch (Exception ex)
					{
						ErrorLog.save_to_error_log(ex);
					}
				}
			}

			return data_row;
		}

		public static List<string> sp_get_column(string sp, string column_name)
		{
			List<string> data_col = new List<string>();
			int i = 0;

			using (SqlConnection conn = new SqlConnection(conn_string))
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
						i = row.GetOrdinal(column_name);

						while (row.Read())
						{
							data_col.Add((row.IsDBNull(i)) ? "" : row[column_name].ToString());
						}
					}
					catch (Exception ex)
					{
						ErrorLog.save_to_error_log(ex);
					}
				}
			}

			return data_col;
		}

		public static List<string> sp_get_column(string sp, SqlParameter parameter, string column_name)
		{
			List<string> data_col = new List<string>();
			int i = 0;

			using (SqlConnection conn = new SqlConnection(conn_string))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					conn.Open();

					cmd.Connection = conn;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = sp;
					cmd.Parameters.Add(parameter);

					try
					{
						SqlDataReader row = cmd.ExecuteReader();
						i = row.GetOrdinal(column_name);

						while (row.Read())
						{
							data_col.Add((row.IsDBNull(i)) ? "" : row[column_name].ToString());
						}
					}
					catch (Exception ex)
					{
						ErrorLog.save_to_error_log(ex);
					}
				}
			}

			return data_col;
		}

		public static List<string> sp_get_column(string sp, List<SqlParameter> parameters, string column_name)
		{
			List<string> data_col = new List<string>();
			int i = 0;

			using (SqlConnection conn = new SqlConnection(conn_string))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					conn.Open();

					cmd.Connection = conn;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = sp;

					foreach (SqlParameter p in parameters)
					{
						cmd.Parameters.Add(p);
					}

					try
					{
						SqlDataReader row = cmd.ExecuteReader();
						i = row.GetOrdinal(column_name);

						while (row.Read())
						{
							data_col.Add((row.IsDBNull(i)) ? "" : row[column_name].ToString());
						}
					}
					catch (Exception ex)
					{
						ErrorLog.save_to_error_log(ex);
					}
				}
			}

			return data_col;
		}


		public static string sp_get_var(string sp)
		{
			string v = "";

			using (SqlConnection conn = new SqlConnection(conn_string))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					conn.Open();

					cmd.Connection = conn;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = sp;

					try
					{
						v = cmd.ExecuteScalar().ToString();
					}
					catch (Exception ex)
					{
						ErrorLog.save_to_error_log(ex);
					}
				}
			}

			return v;
		}

		public static string sp_get_var(string sp, SqlParameter parameter)
		{
			string v = "";

			using (SqlConnection conn = new SqlConnection(conn_string))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					conn.Open();

					cmd.Connection = conn;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = sp;
					cmd.Parameters.Add(parameter);

					try
					{
						v = cmd.ExecuteScalar().ToString();
					}
					catch (Exception ex)
					{
						ErrorLog.save_to_error_log(ex);
					}
				}
			}

			return v;
		}

		/// <summary>
		/// Gets the last IDENTITY value produced on a connection, regardless of the table that produced the value, and regardless of the scope of the statement that produced the value.
		/// </summary>
		/// <param name="table"></param>
		/// <returns>Return the last identity value entered into a table in your current session. Returns 0 on error.</returns>
		public static int get_last_insert_id(string table)
		{
			int id = 0;

			try
			{
				id = Int32.Parse(sp_get_var(StoredProcedures.Utility.GetLastInsertId, new SqlParameter("@table", table)));
			}
			catch (Exception ex)
			{
				id = 0;
				ErrorLog.save_to_error_log(ex);
			}

			return id;
		}
	}
	
	public class StoredProcedures
	{
		public class Files
		{
			public const string Insert = "Files_Insert";
            public const string Update = "Files_Update";
			public const string Search = "Files_Search";
			public const string SearchSimple = "Files_SearchSimple";
			public const string GetAll = "Files_GetAll";
			public const string GetById = "Files_GetById";
            public const string Delete = "Files_Delete";
            public const string GetDataTable = "Files_GetDataTable";
            public const string GetDataTableCount = "Files_GetDataTableCount";
		}

		public class FileCategories
		{
			public const string Insert = "";
			public const string GetAll = "FileCategories_GetAll";
		}

		public class Organizations
		{
			public const string Insert = "";
			public const string Update = "";
			public const string GetAll = "Organizations_GetAll";
		}

		public class Utility
		{
			public const string GetLastInsertId = "_GetLastInsertId";
		}

        public class Regions
        {
            public const string GetAll = "Regions_GetAll";
        }

        public class Counties
        {
            public const string GetAll = "Counties_GetAll";
        }
	}
}