using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using UserService.Application.Abstractions.Repositories;
using UserService.Application.Exceptions;
using UserService.Application.Settings;
using UserService.DataAccess.Context;
using UserService.DataAccess.Entitites;
using UserService.Domain.Models;

namespace UserService.DataAccess.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly CvManagementDbContext _dbDbContext;
    private readonly RolesSettings _rolesSettings;

    public UsersRepository(CvManagementDbContext dbDbContext, IOptions<RolesSettings> rolesSettings)
    {
        _dbDbContext = dbDbContext;
        _rolesSettings = rolesSettings.Value;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _dbDbContext.Users.AsNoTracking().Include(x=>x.Role).ToListAsync(cancellationToken);
    }

    public async Task<User?> GetUserByIdBasicAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbDbContext.Users.AsNoTracking().Include(x=>x.Role).FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetUserByIdFullAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbDbContext.Users
            .Include(x=>x.Role)
            .Include(x=>x.Projects)
            .ThenInclude(x=>x.Technologies)
            .Include(x=>x.Attributes)
            .AsNoTracking()
            .FirstOrDefaultAsync(x=>x.Id == id, cancellationToken);
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbDbContext.Users.AsNoTracking().Include(x=>x.Role).FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }

    public async Task<int> GetCandidatesAmountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbDbContext.Users.AsNoTracking().Where(x=>x.Role.Id == _rolesSettings.DefaultRoleId).CountAsync(cancellationToken);
    }

    public async Task<int> GetRecruitersAmountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbDbContext.Users.AsNoTracking().Where(x=>x.Role.Id == _rolesSettings.RecruiterRoleId).CountAsync(cancellationToken);
    }

    public async Task AddUserAsync(User user, CancellationToken cancellationToken = default)
    {
        await _dbDbContext.Users.AddAsync(user, cancellationToken);
        await _dbDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbDbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var userEntry = _dbDbContext.Users.Update(user);
            await AdjustChangeTrackerEntries(cancellationToken, userEntry);

            await _dbDbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException e)
        {
            throw new EntityUpdatingException("Failed to update user due to invalid version." + e.Message);
        }
        catch (Exception e)
        {
            throw new EntityUpdatingException("Failed to update user." + e.Message);
        }
    }

    private async Task AdjustChangeTrackerEntries(CancellationToken cancellationToken, EntityEntry<User> userEntry)
    {
        userEntry.Property(u => u.Version).CurrentValue += 1;
        userEntry.Property(u => u.IsBlocked).IsModified = false;
        userEntry.Property(u => u.PasswordHash).IsModified = false;
        userEntry.Property(u => u.IsConfirmed).IsModified = false;
    }

    public async Task UploadUserPhotoAsync(Guid userId, string photo, CancellationToken cancellationToken = default)
    {
        await _dbDbContext.Users.Where(x=>x.Id == userId).ExecuteUpdateAsync(s=>s.SetProperty(p=>p.ProfileData.PersonalPhoto, photo),cancellationToken);
    }

    public async Task DeleteUsersAsync(Guid[] ids, CancellationToken cancellationToken = default)
    {
        await _dbDbContext.Users.Where(x => ids.Contains(x.Id)).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task BlockUsersAsync(Guid[] ids, CancellationToken cancellationToken = default)
    {
        await _dbDbContext.Users.Where(x => ids.Contains(x.Id))
            .ExecuteUpdateAsync(x => x
                .SetProperty(p => p.IsBlocked, true), cancellationToken);
    }

    public async Task UnblockUsersAsync(Guid[] ids, CancellationToken cancellationToken = default)
    {
        await _dbDbContext.Users.Where(x => ids.Contains(x.Id))
            .ExecuteUpdateAsync(x => x
                .SetProperty(p => p.IsBlocked, false), cancellationToken);
    }

    public async Task ConfirmUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _dbDbContext.Users.Where(x => x.Id == id)
            .ExecuteUpdateAsync(s=>s
                .SetProperty(p=>p.IsConfirmed, true), cancellationToken);
    }
}