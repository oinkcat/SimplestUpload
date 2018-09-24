using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace SimplestUpload
{
    /// <summary>
    /// Global web application class
    /// </summary>
    public class Global : System.Web.HttpApplication
    {
        private static FileUploaderApp app;

        protected void Application_Start(object sender, EventArgs e)
        {
            app = new FileUploaderApp();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Set handler for current request
            Context.RemapHandler(app);
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}