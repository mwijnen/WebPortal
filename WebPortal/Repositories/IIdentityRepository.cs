using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebPortal.Repositories
{
    public interface IIdentityRepository
    {
        public Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken);

        public Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken);

        public Task<IdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken);

        public Task<IdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken);

        public Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken);

        public Task<IdentityUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken);

        public Task AddToRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken);

        public Task RemoveFromRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken);

        public Task<IList<string>> GetRolesAsync(IdentityUser user, CancellationToken cancellationToken);

        public Task<bool> IsInRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken);

        public Task<IList<IdentityUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken);
    }
}
