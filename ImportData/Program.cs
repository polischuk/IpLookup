using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using CsvHelper;
using Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ImportData
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new DbContext(new MongoDbConfig(null)
            {
                Database = "iplookup",
                Host = "localhost",
                Password = "example",
                User = "root",
                Port = 27017
            });
            Console.WriteLine("START IpV6");
            var result = ReadInCsv("v6.csv");
            context.IpList.BulkWrite(result);
            Console.WriteLine("END IpV6");
            Console.WriteLine("START IpV4");
            var result2 = ReadInCsv("v4.csv");
            context.IpList.BulkWrite(result2);
            Console.WriteLine("END IpV4");
        }

        public static List<WriteModel<IpEntity>> ReadInCsv(string path) 
        {
            List<WriteModel<IpEntity>> batch = new List<WriteModel<IpEntity>>();
            using (TextReader fileReader = File.OpenText(path))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false;
                var totalCount = 0;
              
                while (csv.Read())
                {
                    try
                    {
                       var record = csv.GetRecord<dynamic>();
                        var upsertOne = new InsertOneModel<IpEntity>(new IpEntity
                        {
                            StartIpNumber = Convert.ToDouble(record.Field1),
                            EndIpNumber = Convert.ToDouble(record.Field2),
                            CountryPrefix = record.Field3,
                            Country = record.Field4,
                            Region = record.Field5,
                            City = record.Field6,
                            Latitude = Convert.ToDecimal(record.Field7),
                            Longitude = Convert.ToDecimal(record.Field8),
                            ZipCode = record.Field9,
                            TimeZone = record.Field10
                        });
                        batch.Add(upsertOne);
                    }
                    finally
                    {
                        Console.WriteLine(++totalCount);
                    }

                }
            }
            return batch;
        }
    }
}
