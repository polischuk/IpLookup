using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Data
{
    public interface IIpService
    {
        Task<List<IpEntity>> GetLocationsByIp(string ip);
    }

    public class IpService : IIpService
    {
        private static readonly ulong MaxIpV4 = 4294967295;
        private readonly IDbContext _context;

        public IpService(IDbContext context)
        {
            _context = context;
        }

        public async Task<List<IpEntity>> GetLocationsByIp(string ip)
        {
            var ipAddress = IPAddress.Parse(ip);
            FilterDefinition<IpEntity> finalFilter;
            double number;
            switch (ipAddress.AddressFamily)
            {
                case System.Net.Sockets.AddressFamily.InterNetwork:
                    number = (double)IpV4ToNumber(ip);
                    finalFilter = Builders<IpEntity>.Filter.And(new List<FilterDefinition<IpEntity>>
                    {
                        Builders<IpEntity>.Filter.Lte(m => m.StartIpNumber, number),
                        Builders<IpEntity>.Filter.Gte(m => m.EndIpNumber, number),
                        Builders<IpEntity>.Filter.Lte(m => m.EndIpNumber, MaxIpV4)
                    });
                    break;
                case System.Net.Sockets.AddressFamily.InterNetworkV6:
                    number = (double)IpV6ToNumber(ipAddress);
                    finalFilter = Builders<IpEntity>.Filter.And(new List<FilterDefinition<IpEntity>>
                    {
                        Builders<IpEntity>.Filter.Lte(m => m.StartIpNumber, number),
                        Builders<IpEntity>.Filter.Gte(m => m.EndIpNumber, number)
                    });
                    break;
                default:
                    throw new Exception("Bad Ip Address");
            }

            return await GetDataFromDb(finalFilter);
        }

        private async Task<List<IpEntity>> GetDataFromDb(FilterDefinition<IpEntity> finalFilter)
        {
            var result = await _context.IpList.FindAsync(finalFilter);
            return result.ToList();
        }

        private static BigInteger IpV4ToNumber(string originalIpV4)
        {
            string[] ipList = originalIpV4.Split(".".ToCharArray());
            string ipNumber = "";
            foreach (string ip in ipList)
            {
                ipNumber += Convert.ToInt16(ip) < 16
                    ? "0" + Convert.ToInt16(ip).ToString("x")
                    : Convert.ToInt16(ip).ToString("x");
            }

            return new BigInteger(ulong.Parse(ipNumber, NumberStyles.HexNumber));
        }

        private static BigInteger IpV6ToNumber(IPAddress originalIpV6)
        {
            var addrBytes = originalIpV6.GetAddressBytes();
            System.Numerics.BigInteger result;
            if (System.BitConverter.IsLittleEndian)
            {
                System.Collections.Generic.List<byte> byteList = new System.Collections.Generic.List<byte>(addrBytes);
                byteList.Reverse();
                addrBytes = byteList.ToArray();
            }

            if (addrBytes.Length > 8)
            {
                result = System.BitConverter.ToUInt64(addrBytes, 8);
                result <<= 64;
                result += System.BitConverter.ToUInt64(addrBytes, 0);
            }

            return result;
        }
    }
}
