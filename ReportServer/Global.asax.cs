using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Data.SQLite;
using System.Data;

namespace ReportServer
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            fyiReporting.RdlAsp.RdlSession rs =
                Application[fyiReporting.RdlAsp.RdlSession.SessionStat] as fyiReporting.RdlAsp.RdlSession;

            if (rs == null)
            {
                Application["SessionStat"] = rs = new fyiReporting.RdlAsp.RdlSession();
            }
            lock (rs)
            {
                rs.Count++;
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            HttpContext context = app.Context;
            FirstRequestInitialization.Initialize(context);
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {
            fyiReporting.RdlAsp.RdlSession rs =
                Application[fyiReporting.RdlAsp.RdlSession.SessionStat] as fyiReporting.RdlAsp.RdlSession;

            if (rs != null)
            {
                lock (rs)
                {
                    rs.Count--;
                }
            }
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }

    class FirstRequestInitialization

    {

        private static bool s_InitializedAlready = false;

        private static Object s_lock = new Object();

        // Initialize only on the first request

        public static void Initialize(HttpContext context)

        {
            if (s_InitializedAlready)

            {
                return;
            }
            lock (s_lock)
            {
                if (s_InitializedAlready)
                {
                    return;
                }

                // Perform setup here …
                try
                {

                    string sql = "SELECT Email, RoleId FROM users WHERE roleid = 'Admin';";
                    SQLiteCommand cmd = new SQLiteCommand();
                    cmd.Connection = new SQLiteConnection(Code.DAL.ConnectionString);
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;

                    DataTable dt = Code.DAL.ExecuteCmdTable(cmd);

                    if (dt.Rows.Count == 0)
                    {
                        // HttpContext.Current.Response.Redirect("Setup.aspx", true);

                        context.Response.Redirect("Setup.aspx", false);
                        context.ApplicationInstance.CompleteRequest();

                    }

                }
                catch (Exception ex)
                {
                    HttpContext.Current.Response.Redirect("Setup.aspx", true);
                }

                s_InitializedAlready = true;

            }

        }

    }
}