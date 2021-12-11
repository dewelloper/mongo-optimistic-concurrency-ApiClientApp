using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiClientApp.Controllers
{
    internal class UserInfo: BaseDto// : HttpContent
    {
        public Int64 Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string refreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }


        //protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        //{
        //    //throw new System.NotImplementedException();
        //    return Task.FromResult(1);
        //}

        //protected override bool TryComputeLength(out long length)
        //{
        //    //throw new System.NotImplementedException();
        //    length = 1;
        //    return true;
        //}
    }
}