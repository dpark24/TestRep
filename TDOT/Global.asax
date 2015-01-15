<%@ Application Language="C#" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        // Code that runs on application startup

    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown

    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs

    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }

	protected void Application_BeginRequest(object sender, EventArgs e)
	{
		//not happy about this but it's the only way i've found so far that the session can be maintained while using swfupload
		string auth_param_name = "AUTHID";
		string auth_cookie_name = FormsAuthentication.FormsCookieName;
		string session_param_name = "ASPSESSID";
		string session_cookie_name = "ASP.NET_SessionId";

		/* we guess at this point session is not already retrieved by application so we recreate cookie with the session id... */
		try
		{
			if (HttpContext.Current.Request.Form[session_param_name] != null)
			{
				UpdateCookie(session_cookie_name, HttpContext.Current.Request.Form[session_param_name]);
			}
			else if (HttpContext.Current.Request.QueryString[session_param_name] != null)
			{
				UpdateCookie(session_cookie_name, HttpContext.Current.Request.QueryString[session_param_name]);
			}
		}
		catch
		{
		}

		try
		{
			if (HttpContext.Current.Request.Form[auth_param_name] != null)
			{
				UpdateCookie(auth_cookie_name, HttpContext.Current.Request.Form[auth_param_name]);
			}
			else if (HttpContext.Current.Request.QueryString[auth_param_name] != null)
			{
				UpdateCookie(auth_cookie_name, HttpContext.Current.Request.QueryString[auth_param_name]);
			}

		}
		catch
		{
		}
	}

	private void UpdateCookie(string cookie_name, string cookie_value)
	{
		HttpCookie cookie = HttpContext.Current.Request.Cookies.Get(cookie_name);
		if (null == cookie)
		{
			cookie = new HttpCookie(cookie_name);
		}
		cookie.Value = cookie_value;
		HttpContext.Current.Request.Cookies.Set(cookie);
	}
       
</script>
