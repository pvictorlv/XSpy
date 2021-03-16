using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace XSpy.Socket.Auth
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        readonly IHttpContextAccessor _httpContextAccessor;

        public CustomUserIdProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserId(HubConnectionContext connection)
        {
            var user = _httpContextAccessor.HttpContext.Request.Headers["user-token"].ToString();
            var deviceId = _httpContextAccessor.HttpContext.Request.Headers["device-id"].ToString();
            
            var claim = connection.User.Identities.FirstOrDefault();
            claim?.AddClaim(new Claim(ClaimTypes.Sid, user));
            claim?.AddClaim(new Claim(ClaimTypes.SerialNumber, deviceId));

            return user;
        }
    }
}