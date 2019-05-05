using B21C.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace B21C.Helper
{
    public class ResiAWB
    {
        string api_key = "demo";

        public TrackingResponse GetTracking(ResiAWB_COURIER Courier, string TrackingNo)
        {
            using (var wc = new WebClient())
            {
                var jsonstring = wc.DownloadString("http://resiawb.com/api/" + Courier.ToString() + "/" + api_key + "/" + TrackingNo);
                
                jsonstring = jsonstring.TrimStart(new char[]{'s','a','v','e'});
                jsonstring = jsonstring.TrimStart('[');
                jsonstring = jsonstring.TrimEnd(']');
                
                var jobject = JObject.Parse(jsonstring);
                var result = jobject.ToObject<TrackingResponse>();

                return result;
            }
        }

    }

    public enum ResiAWB_COURIER
    {
        jne,
        tiki,
        pos_id,
        pos_int,
        ninja_id,
        wahana,
        rpx_id,
        first,
        pandu,
        lionparcel
    }

    public class Detail
    {
        public string date { get; set; }
        public string time { get; set; }
        public string statusdetail { get; set; }
        public string lokasi { get; set; }
    }

    public class TrackingResponse
    {
        public string expedisi { get; set; }
        public string status { get; set; }
        public List<Detail> detail { get; set; }
    }
}