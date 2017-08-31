using System.Linq;
using System.Net;

namespace Isometric.Server.Common
{
    public static class LogHelper
    {
        public static string ToLogString(this EndPoint point)
        {
            var ipEndPoint = (IPEndPoint) point;

            return $"{ipEndPoint.Address.GetAddressBytes().Aggregate("", (s, b) => s + "." + b).Substring(1)}:{ipEndPoint.Port}";
        }
    }
}