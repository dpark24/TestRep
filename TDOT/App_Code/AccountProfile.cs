using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Profile;
using System.Web.Security;

//http://weblogs.asp.net/jgalloway/archive/2008/01/19/writing-a-custom-asp-net-profile-class.aspx
// and http://stackoverflow.com/questions/426609/how-to-assign-profile-values
namespace AccountProfiles
{
	public class AccountProfile : ProfileBase
	{
		static public AccountProfile CurrentUser
		{
			get
			{
				return (AccountProfile)
					(ProfileBase.Create(Membership.GetUser().UserName));
			}
		}

		public static AccountProfile GetUserProfile(string username)
		{
			return Create(username) as AccountProfile;
		}
		public static AccountProfile GetUserProfile()
		{
			return Create(Membership.GetUser().UserName) as AccountProfile;
		}

		[SettingsAllowAnonymous(false)]
		public string FirstName
		{
			get { return ((string)(base["FirstName"])); }
			set { base["FirstName"] = value; Save(); }
		}

		[SettingsAllowAnonymous(false)]
		public string LastName
		{
			get { return ((string)(base["LastName"])); }
			set { base["LastName"] = value; Save(); }
		}

		[SettingsAllowAnonymous(false)]
		public string JobTitle
		{
			get { return ((string)(base["JobTitle"])); }
			set { base["JobTitle"] = value; Save(); }
		}

		[SettingsAllowAnonymous(false)]
		public string Agency
		{
			get { return ((string)(base["Agency"])); }
			set { base["Agency"] = value; Save(); }
		}
		
		/// <summary>
		/// TODO: improve these to dynamically grab the key and such.. unless there is a still better way
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, object> ToDictionary()
		{
			Dictionary<string, object> data = new Dictionary<string,object>();
			
			data.Add("FirstName", this.FirstName);
			data.Add("LastName", this.LastName);
			data.Add("JobTitle", this.JobTitle);
			data.Add("Agency", this.Agency);
			data.Add("Role", get_highest_role());
			data.Add("Email", Membership.GetUser().Email);

			return data;
		}

		public Dictionary<string, object> ToDictionary(string username)
		{
			Dictionary<string, object> data = new Dictionary<string, object>();

			data.Add("FirstName", this.FirstName);
			data.Add("LastName", this.LastName);
			data.Add("JobTitle", this.JobTitle);
			data.Add("Agency", this.Agency);
			data.Add("Role", get_highest_role(username));
			data.Add("Email", Membership.GetUser(username).Email);

			return data;
		}

		public string get_highest_role()
		{
			string role = "Level 0";
			string[] user_roles = Roles.GetRolesForUser();

			if(user_roles.Length > 0)
			{
				Array.Reverse(user_roles);
				role = user_roles[0];
			}

			return role;
		}

		public string get_highest_role(string username)
		{
			string role = "Level 0";
			string[] user_roles = Roles.GetRolesForUser(username);

			if (user_roles.Length > 0)
			{
				Array.Reverse(user_roles);
				role = user_roles[0];
			}

			return role;
		}


	}
}