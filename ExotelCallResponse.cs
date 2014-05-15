using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rbJustDial.Exotel
{
    public class ExotelCallResponse
    {
        public string CallSID { get; set; }
        public string PhoneNumberSid { get; set; }
        public string Status { get; set; }
    }
}