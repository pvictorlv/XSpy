using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Stock.Shared.Models.Data;
using XSpy.Database.Entities;
using XSpy.Database.Interfaces;
using XSpy.Database.Models.Requests.Users;
using XSpy.Database.Models.Responses;
using XSpy.Database.Models.Responses.Users;
using XSpy.Database.Models.Tables;
using XSpy.Database.Services.Base;

namespace XSpy.Database.Services.Users
{
    public class UserService : BaseEntityService
    {
        private readonly IConfiguration _configuration;

        public UserService(DatabaseContext context, IConfiguration configuration,
            IMemoryCache cache) : base(context, cache)
        {
            _configuration = configuration;
        }


        public async Task<DataTableResponse<UserListResponse>> GetTable(DataTableRequest<SearchUserRequest> request,
            User user)
        {
            var query = DbContext.Users.Include(s => s.RankData).AsQueryable();

            var countTotal = await query.LongCountAsync();

            if (request.Filter != null)
            {
                if (!string.IsNullOrEmpty(request.Filter.FullName))
                {
                    query = query.Where(s => s.Name.ToUpper().Contains(request.Filter.FullName.ToUpper()));
                }

                if (request.Filter.RankId != null)
                {
                    query = query.Where(s => s.RankId == request.Filter.RankId);
                }


                if (request.Filter.IsActive != null)
                {
                    query = query.Where(s => s.IsActive == request.Filter.IsActive);
                }

                if (request.Filter.DateFrom != null)
                {
                    query = query.Where(s => s.CreatedAt >= request.Filter.DateFrom);
                }

                if (request.Filter.DateTo != null)
                {
                    query = query.Where(s => s.CreatedAt <= request.Filter.DateTo.Value.AddHours(23.99));
                }
            }


            if (request.Search != null && !string.IsNullOrEmpty(request.Search.Value))
            {
                var search = request.Search.Value.ToUpper();
                query = query.Where(s =>
                    s.Name.ToUpper().Contains(search) ||
                    s.Email.ToUpper().Contains(search));
            }

            var queryData = query.Select(s => new UserListResponse
            {
                Id = s.Id,
                Enabled = s.IsActive,
                Name = s.Name,
                RankName = s.RankData.Name,
                Email = s.Email
            });

            queryData = OrderResult(queryData, request);
            var total = await queryData.LongCountAsync();
            queryData = queryData.ApplyTableLimit(request);

            return new DataTableResponse<UserListResponse>()
            {
                Data = await queryData.ToListAsync(),
                Draw = request.Draw,
                RecordsFiltered = total,
                RecordsTotal = countTotal
            };
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


        public Task<UserData> GetById(Guid userId)
        {
            var query = DbContext.Users.Where(s => s.Id == userId)
                .AsQueryable();

            return query.Select(s => new UserData()
            {
                Document = s.Document,
                Email = s.Email,
                BirthDate = s.BirthDate,
                Name = s.Name,
                DeviceToken = s.DeviceToken,
                ProfilePhoto = s.ProfilePhoto,
            }).FirstOrDefaultAsync();
        }

        public Task<UserAddressData> GetUserAddressByUserId(Guid userId)
        {
            var query = DbContext.UserAddresses.Where(s => s.UserId == userId)
                .AsQueryable();

            return query.Select(s => new UserAddressData()
            {
                State = s.State,
                Street = s.Street,
                City = s.City,
                Complement = s.Complement,
                Neighborhood = s.Neighborhood,
                Zip = s.Zip
            }).FirstOrDefaultAsync();
        }

        public Task<User> GetEditableById(Guid id)
        {
            var query = DbContext.Users
                .AsQueryable();

            return query.FirstOrDefaultAsync(s => s.Id == id);
        }


        public bool VerifyHash(string password, string userHash)
        {
            return BCrypt.Net.BCrypt.Verify(password + GetPassBlow(), userHash);
        }

        public async Task<bool> IsValidToken(Guid token)
        {
            return await DbContext.Users
                .AnyAsync(x =>
                    x.DeviceToken == token && x.IsActive &&
                    (x.PlanExpireDate == null || x.PlanExpireDate >= DateTime.UtcNow));
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
                .FirstOrDefaultAsync(x => x.Email == username && x.IsActive)
                .ConfigureAwait(false);

            if (userByName == null)
                return null;

            if (!VerifyHash(password, userByName.Password))
            {
                return null;
            }


            return userByName;
        }

        public async Task<IUserEntity> RegisterUser(RegisterRequest request)
        {
            var userExists = await DbContext.Users
                .AnyAsync(x => x.Email == request.Email)
                .ConfigureAwait(false);

            if (userExists)
                return null;


            var user = new User()
            {
                Id = Guid.NewGuid(),
                PlanExpireDate = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.Now,
                DeviceToken = Guid.NewGuid(),
                Email = request.Email,
                IsActive = true,
                Name = request.Fullname,
                Password = HashPassword(request.Password),
                ProfilePhoto = null,
                RankId = Guid.Parse("4288aa01-036a-47e4-9db8-61e425ac2d43")
            };

            await DbContext.Users.AddAsync(user);
            await DbContext.SaveChangesAsync();

            return user;
        }
    }
}