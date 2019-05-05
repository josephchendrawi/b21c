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
    public class RajaOngkir
    {
        string api_key = "deead3362e48cb29571df3a4924fb938";

        public List<City> GetCityList()
        {
            using (var wc = new WebClient())
            {
                wc.Headers["key"] = api_key;
                var cities_jsonstring = wc.DownloadString("http://pro.rajaongkir.com/api/city");
                var cities_jobject = JObject.Parse(cities_jsonstring)["rajaongkir"];
                var cities = cities_jobject.ToObject<CityListResponse>();

                return cities.results;
            }
        }

        public List<Subdistrict> GetSubdistrictList(string city_id)
        {
            using (var wc = new WebClient())
            {
                wc.Headers["key"] = api_key;
                var subdistricts_jsonstring = wc.DownloadString("http://pro.rajaongkir.com/api/subdistrict?city=" + city_id);
                var subdistricts_jobject = JObject.Parse(subdistricts_jsonstring)["rajaongkir"];
                var subdistricts = subdistricts_jobject.ToObject<SubdistrictListResponse>();

                return subdistricts.results;
            }
        }
        
        public List<CourierCost> GetShippingFee(string dest_subdistrict_id)
        {
            using (var wc = new WebClient())
            {
                var CostRequest = new CostRequest()
                {
                    origin = "48",
                    originType = "city",
                    destination = dest_subdistrict_id,
                    destinationType = "subdistrict",
                    weight = 1000,
                    courier = "jne:pos:tiki:jnt",
                };

                wc.Headers["key"] = api_key;
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                var reues = JsonConvert.SerializeObject(CostRequest);
                var costs_jsonstring = wc.UploadString("http://pro.rajaongkir.com/api/cost", reues);
                var costs_jobject = JObject.Parse(costs_jsonstring)["rajaongkir"];
                var costs = costs_jobject.ToObject<CostListResponse>();

                return costs.results;
            }
        }

        public List<CourierCost> GetShippingFee(string dest_subdistrict_id, string courier)
        {
            using (var wc = new WebClient())
            {
                var CostRequest = new CostRequest()
                {
                    origin = "48",
                    originType = "city",
                    destination = dest_subdistrict_id,
                    destinationType = "subdistrict",
                    weight = 1000,
                    courier = courier.ToLower(),
                };

                wc.Headers["key"] = api_key;
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                var reues = JsonConvert.SerializeObject(CostRequest);
                var costs_jsonstring = wc.UploadString("http://pro.rajaongkir.com/api/cost", reues);
                var costs_jobject = JObject.Parse(costs_jsonstring)["rajaongkir"];
                var costs = costs_jobject.ToObject<CostListResponse>();

                return costs.results;
            }
        }

        public Waybill GetTracking(string waybillno, string courier)
        {
            using (var wc = new WebClient())
            {
                var WaybillRequest = new WaybillRequest()
                {
                    courier = courier,
                    waybill = waybillno
                };

                wc.Headers["key"] = api_key;
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                var request = JsonConvert.SerializeObject(WaybillRequest);
                var waybill_jsonstring = wc.UploadString("http://pro.rajaongkir.com/api/waybill", request);
                var waybill_jobject = JObject.Parse(waybill_jsonstring)["rajaongkir"];
                var waybill = waybill_jobject.ToObject<WaybillResponse>();

                return waybill.result;
            }
        }

    }
    
    //Class

    public class Response
    {
        public ResponseStatus status { get; set; }
        public List<object> results { get; set; }
    }

    public class ResponseStatus
    {
        public int code { get; set; }
        public string description { get; set; }
    }

    //////////////

    public class CityListResponse : Response
    {
        public List<City> results { get; set; }
    }

    public class City
    {
        public string city_id { get; set; }
        public string province_id { get; set; }
        public string province { get; set; }
        public string type { get; set; }
        public string city_name { get; set; }
        public string postal_code { get; set; }
    }

    //////////////

    public class SubdistrictListResponse : Response
    {
        public List<Subdistrict> results { get; set; }
    }

    public class Subdistrict
    {
        public string subdistrict_id { get; set; }
        public string province_id { get; set; }
        public string province { get; set; }
        public string city_id { get; set; }
        public string city { get; set; }
        public string type { get; set; }
        public string subdistrict_name { get; set; }
    }

    //////////////

    public class CostRequest
    {
        public string origin { get; set; }
        public string destination { get; set; }
        public string originType { get; set; }
        public string destinationType { get; set; }
        public int weight { get; set; }
        public string courier { get; set; }
    }

    public class CostListResponse : Response
    {
        public List<CourierCost> results { get; set; }
    }

    public class CourierCost
    {
        public string code { get; set; }
        public string name { get; set; }
        public List<Cost> costs { get; set; }
    }

    public class Cost
    {
        public string service { get; set; }
        public string description { get; set; }
        public List<CostDetails> cost { get; set; }
    }

    public class CostDetails
    {
        public int value { get; set; }
        public string etd { get; set; }
        public string note { get; set; }
    }

    //////////////

    public class WaybillRequest
    {
        public string waybill { get; set; }
        public string courier { get; set; }
    }

    public class WaybillResponse : Response
    {
        public Waybill result { get; set; }
    }

    public class Waybill
    {
        public bool delivered { get; set; }
        public WaybillSummary summary { get; set; }
        public WaybillDetails details { get; set; }
        public DeliveryStatus delivery_status { get; set; }
        public List<Manifest> manifest { get; set; }
    }

    public class WaybillSummary
    {
        public string courier_code { get; set; }
        public string courier_name { get; set; }
        public string waybill_number { get; set; }
        public string service_code { get; set; }
        public string waybill_date { get; set; }
        public string shipper_name { get; set; }
        public string receiver_name { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public string status { get; set; }
    }
    public class WaybillDetails
    {
        public string waybill_number { get; set; }
        public string waybill_date { get; set; }
        public string waybill_time { get; set; }
        public string weight { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public string shippper_name { get; set; }
        public string shipper_address1 { get; set; }
        public object shipper_address2 { get; set; }
        public object shipper_address3 { get; set; }
        public string shipper_city { get; set; }
        public string receiver_name { get; set; }
        public string receiver_address1 { get; set; }
        public string receiver_address2 { get; set; }
        public string receiver_address3 { get; set; }
        public string receiver_city { get; set; }
    }
    public class DeliveryStatus
    {
        public string status { get; set; }
        public string pod_receiver { get; set; }
        public string pod_date { get; set; }
        public string pod_time { get; set; }
    }

    public class Manifest
    {
        public string manifest_code { get; set; }
        public string manifest_description { get; set; }
        public string manifest_date { get; set; }
        public string manifest_time { get; set; }
        public string city_name { get; set; }
    }
}