using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcKomp3.App_Code
{
    public class MainModule : IHttpModule
    {
        public const string CookiePath = "/Samples/pages/";
        public const string MachCookie = "MC";
        public const string MachId = "mi";
        public const string MachFirst = "mf";
        public const int PageViewBatchSize = 10;
        public const string ConnString =
            "Data Source=.;Initial Catalog=Sample;Integrated Security=True;Async=True";

        public void Init(HttpApplication context)
        {
            WorkItem.Init(WorkItem.Work);
            context.AddOnAuthenticateRequestAsync(this.Sample_BeginAuthenticateRequest,
                this.Sample_EndAuthenticateRequest);
            context.PreRequestHandlerExecute += this.Sample_PreRequestHandlerExecute;
            context.EndRequest += this.Sample_EndRequest;
        }

        public void Dispose()
        {
        }

        private void Sample_PreRequestHandlerExecute(Object source, EventArgs e)
        {
            HttpApplication application = (HttpApplication)source;
            HttpContext context = application.Context;
            Page page = context.Handler as Page;
            if (page != null)
            {
                page.StyleSheetTheme = "mkt";
            }
        }

        private IAsyncResult Sample_BeginAuthenticateRequest(Object source, EventArgs e,
            AsyncCallback cb, Object state)
        {
            IAsyncResult ar = null;
            HttpApplication application = (HttpApplication)source;
            HttpContext context = application.Context;
            string path = context.Request.Url.AbsolutePath;

            if (path.StartsWith(CookiePath, StringComparison.OrdinalIgnoreCase) &&
               (path.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase) ||
                path.EndsWith("/", StringComparison.Ordinal)))
            {

                RequestInfo info;
                HttpCookie machCookie = context.Request.Cookies[MachCookie];
                if ((machCookie == null) || !machCookie.HasKeys ||
                    (machCookie.Values[MachId] == null))
                {
                    //
                    // If the client didn't send a machine cookie, then create a new RequestInfo
                    // object, and store it in the per-request Items cache.  The cookie will be
                    // created later.
                    //
                    info = new RequestInfo(Guid.NewGuid(), true, false);
                }
                else
                {
                    string guidStr = machCookie.Values[MachId];
                    try
                    {
                        Guid machGuid = new Guid(guidStr);

                        //
                        // If the client sent a valid machine cookie, check to see if
                        // the first response flag was set in the cookie
                        //
                        bool firstResp = false;
                        if (machCookie.Values[MachFirst] != null)
                            firstResp = true;
                        info = new RequestInfo(machGuid, false, firstResp);
                    }
                    catch (FormatException)
                    {
                        //
                        // If the old cookie was malformed, then create and cache the 
                        // RequestInfo object and request that a new cookie be created later 
                        //
                        info = new RequestInfo(Guid.NewGuid(), true, false);
                    }
                }
                context.Items[RequestInfo.REQ_INFO] = info;

                //
                // If this is the first time the browser has
                // sent the cookie back to us, then record the
                // ID in the database.
                //
                if (info.FirstResponse)
                {
                    //
                    // Insert or update the machine ID asynchronously.  Don't write the full PageView
                    // record until later, in the worker thread.
                    //
                    SqlConnection conn = new SqlConnection(ConnString);
                    SqlCommand cmd = new SqlCommand("[Traffic].[AddMachine]", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("id", SqlDbType.UniqueIdentifier).Value = info.MachineId;
                    conn.Open();
                    ar = cmd.BeginExecuteNonQuery(cb, cmd);
                }
            }
            return ar ?? CompletedResult.Create(state, cb);
        }

        private void Sample_EndAuthenticateRequest(IAsyncResult ar)
        {
            if (!(ar is CompletedResult))
            {
                SqlCommand cmd = ar.AsyncState as SqlCommand;
                if (cmd != null)
                {
                    try
                    {
                        cmd.EndExecuteNonQuery(ar);
                    }
                    catch (SqlException e)
                    {
                        EventLog.WriteEntry("Application",
                            "SqlException in Sample_EndAuthenticateRequest: " + e.Message,
                            EventLogEntryType.Error, 201);
                    }
                    finally
                    {
                        cmd.Connection.Dispose();
                        cmd.Dispose();
                    }
                }
            }
        }

        private void Sample_EndRequest(Object source, EventArgs e)
        {
            HttpApplication application = (HttpApplication)source;
            HttpContext context = application.Context;
            HttpResponse response = context.Response;
            RequestInfo info = (RequestInfo)context.Items[RequestInfo.REQ_INFO];

            //
            // info is only set for dynamic pages: aspx, asmx, etc.
            //
            if (info != null)
            {
                WorkItem.QueuePageView(info, PageViewBatchSize);

                //
                // If this request did not include a machine ID cookie, then set one
                // and include the MachFirst flag.  If this request did include the
                // machine ID cookie, and the MachFirst flag was set, then set
                // a "forever" expiration date, and re-set the cookie without the flag.
                //
                if (info.FirstResponse || info.First)
                {
                    HttpCookie machCookie = new HttpCookie(MachCookie);
                    machCookie.Path = CookiePath;
                    machCookie.HttpOnly = true;
                    machCookie.Values[MachId] = info.MachineId.ToString();
                    if (info.FirstResponse)
                        machCookie.Expires = DateTime.Now.AddYears(50);
                    else
                        machCookie.Values[MachFirst] = "1";
                    response.AppendCookie(machCookie);
                }
            }
            if (!String.IsNullOrEmpty(context.Request.ServerVariables["SERVER_SOFTWARE"]))
            {
                if ((response.Cookies.Count > 0) && (response.Headers["P3P"] == null))
                {
                    response.AddHeader("P3P", "CP = \"NID DSP CAO COR\"");
                }
            }
        }
    }

}