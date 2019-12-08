using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data
{
    public class IpEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public double StartIpNumber { get; set; }
        public double EndIpNumber { get; set; }
        public string CountryPrefix { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string ZipCode { get; set; }
        public string TimeZone {get;set;}
    }
}