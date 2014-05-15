using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace rbJustDial
{
    public partial class CallStatus : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            /*CallSid
            Status
            RecordingUrl
            DateUpdated*/

            Debug.WriteLine("rbjustdial CallSid - " + Request.Params["CallSid"].ToString());
            Debug.WriteLine("rbjustdial Status - " + Request.Params["Status"].ToString());
            /*Debug.WriteLine("rbjustdial RecordingURL - " + Request.Params["RecordingUrl"].ToString());
            Debug.WriteLine("rbjustdial DateUpdated - " + Request.Params["DateUpdated"].ToString());*/

        }

	}
}