using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using XSpy.Database.Entities;
using XSpy.Database.Services.Base;
using XSpy.Database.XSpy.Shared.Models.Interfaces;

namespace XSpy.Database.Services
{
    public class UserService : BaseEntityService
    {
        private readonly IConfiguration _configuration;

        public UserService(DatabaseContext context, IConfiguration configuration,
            IDistributedCache cache) : base(context, cache)
        {
            _configuration = configuration;
        }


        private string GetPassBlow()
        {
            var authSettings = _configuration.GetSection("AuthSettings");
            return authSettings.GetValue<string>("PassBlow");
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password + GetPassBlow());
        }


        public Task<User> GetById(Guid id)
        {
            var query = DbContext.Users
                .Include(s => s.RankData)
                .ThenInclude(s => s.Roles).AsQueryable();

            return query.FirstOrDefaultAsync(s => s.Id == id);
        }


        public bool VerifyHash(string password, string userHash)
        {
            return BCrypt.Net.BCrypt.Verify(password + GetPassBlow(), userHash);
        }

        public async Task<bool> IsValidToken(Guid token)
        {
            return await DbContext.Users
                .AnyAsync(x => x.DeviceToken == token && x.IsActive);
        }

        public async Task<IUserEntity> GetUserByToken(Guid token)
        {
            return await DbContext.Users
                .FirstOrDefaultAsync(x => x.DeviceToken == token && x.IsActive)
                .ConfigureAwait(false);
        }

        public async Task<IUserEntity> Login(string username, string password)
        {
            var userByName = await DbContext.Users
                .FirstOrDefaultAsync(x => (x.Username == username || x.Email == username) && x.IsActive)
                .ConfigureAwait(false);

            if (userByName == null)
                return null;

            if (!VerifyHash(password, userByName.Password))
            {
                return null;
            }


            return userByName;
        }
    }
}