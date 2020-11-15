using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebPortal.Repositories
{
    public class IdentityRepository : IIdentityRepository
    {
        private readonly string _connectionString;

        public IdentityRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("WebPortalContextConnection");
        }

        public async Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                user.Id = Guid.NewGuid().ToString();
                var sql = $@"INSERT INTO [AspNetUsers] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], 
                    [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], 
                    [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount])
                    VALUES ('{user.Id}', '{user.UserName}', '{user.NormalizedUserName}', '{user.Email}', '{user.NormalizedEmail}', 
                    '{user.EmailConfirmed}', '{user.PasswordHash}', '{user.SecurityStamp}', '{user.ConcurrencyStamp}', 
                    '{user.PhoneNumber}', '{user.PhoneNumberConfirmed}', '{user.TwoFactorEnabled}', '{user.LockoutEnd}', 
                    '{user.LockoutEnabled}', '{user.AccessFailedCount}');";
                await connection.ExecuteAsync(sql);
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                await connection.ExecuteAsync($"DELETE FROM [AspNetUsers] WHERE [Id] = '{user.Id}';");
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<IdentityUser>($@"SELECT * FROM [AspNetUsers]
                    WHERE [id] = '{userId}';");
            }
        }

        public async Task<IdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<IdentityUser>($@"SELECT * FROM [AspNetUsers]
                    WHERE [NormalizedUserName] = '{normalizedUserName}';");
            }
        }

        public async Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                await connection.ExecuteAsync($@"UPDATE [AspNetUsers] SET
                    [UserName] = '{user.UserName}',
                    [NormalizedUserName] = '{user.NormalizedUserName}',
                    [Email] = '{user.Email}',
                    [NormalizedEmail] = '{user.NormalizedEmail}',
                    [EmailConfirmed] = '{user.EmailConfirmed}',
                    [PasswordHash] = '{user.PasswordHash}',
                    [SecurityStamp] = '{user.SecurityStamp}',
                    [ConcurrencyStamp] = '{user.ConcurrencyStamp}',
                    [PhoneNumber] = '{user.PhoneNumber}',
                    [PhoneNumberConfirmed] = '{user.PhoneNumberConfirmed}',
                    [TwoFactorEnabled] = '{user.TwoFactorEnabled}',                   
                    [LockoutEnd] = '{user.LockoutEnd}',
                    [LockoutEnabled] = '{user.LockoutEnabled}',
                    [AccessFailedCount] = '{user.AccessFailedCount}'
                    WHERE [Id] = '{user.Id}';");
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<IdentityUser>($@"SELECT * FROM [AspNetUsers]
                    WHERE [NormalizedEmail] = '{normalizedEmail}';");
            }
        }

        public async Task AddToRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                var normalizedName = roleName.ToUpper();
                var roleId = await connection.QuerySingleAsync<string>($"SELECT [Id] FROM [AspNetRole] WHERE [NormalizedName] = '{normalizedName}'");
                if (String.IsNullOrEmpty(roleId))
                {
                    roleId = Guid.NewGuid().ToString();
                    await connection.ExecuteAsync($"INSERT INTO [AspNetRole]([Id], [Name], [NormalizedName]) " +
                        $"VALUES('{roleId}', '{roleName}', '{normalizedName}'");
                }

                await connection.ExecuteAsync($"IF NOT EXISTS(SELECT 1 FROM [AspNetUserRole] WHERE [UserId] = '{user.Id}' AND [RoleId] = '{roleId}')" +
                    $"INSERT INTO [AspNetUserRole]([UserId], [RoleId]) VALUES('{user.Id}', '{roleId}')");
            }
        }

        public async Task RemoveFromRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                var roleId = await connection.QuerySingleAsync<string>($"SELECT [Id] FROM [AspNetRoles] WHERE [NormalizedName] = '{roleName.ToUpper()}'");
                if (String.IsNullOrEmpty(roleId))
                    await connection.ExecuteAsync($"DELETE FROM [AspNetUserRoles] WHERE [UserId] = '{user.Id}' AND [RoleId] = '{roleId}'");
            }
        }

        public async Task<IList<string>> GetRolesAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                var queryResults = await connection.QueryAsync<string>($@"SELECT r.[Name] FROM [AspNetRoles] r INNER JOIN [AspNetUserRoles] ur ON ur.[RoleId] = r.Id 
                    WHERE ur.UserId = '{user.Id}'");

                return queryResults.ToList();
            }
        }

        public async Task<bool> IsInRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                var roleId = await connection.ExecuteScalarAsync<int?>($"SELECT [Id] FROM [AspNetRoles] WHERE [NormalizedName] = '{roleName.ToUpper()}'");
                if (roleId == default(int)) return false;
                var matchingRoles = await connection.ExecuteScalarAsync<int>(@$"SELECT COUNT(*) FROM [AspNetUserRoles] 
                    WHERE [UserId] = '{user.Id}' AND [RoleId] = '{roleId}'");

                return matchingRoles > 0;
            }
        }

        public async Task<IList<IdentityUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                var queryResults = await connection.QueryAsync<IdentityUser>(@$"SELECT u.* FROM [AspNetUsers] u 
                    INNER JOIN [AspNetUserRoles] ur ON ur.[UserId] = u.[Id] 
                    INNER JOIN [AspNetRoles] r ON r.[Id] = ur.[RoleId] WHERE r.[NormalizedName] = '{roleName.ToUpper()}';");

                return queryResults.ToList();
            }
        }
    }
}
