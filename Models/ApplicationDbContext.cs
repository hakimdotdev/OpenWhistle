using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OpenWhistle.Models
{
    public interface IOpenWhistleDbContext
    {
        public DbSet<WhistleblowerReport> Reports { get; set; }
        DbSet<FollowUpAction> FollowUpActions { get; }
        DbSet<ChatMessage> ChatMessages { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public class ApplicationUser : IdentityUser<Guid>
    {
    }


    public class OpenWhistleDbContext(DbContextOptions<OpenWhistleDbContext> options)
        : IdentityDbContext(options), IOpenWhistleDbContext
    {
        public DbSet<WhistleblowerReport> Reports { get; set; }
        public DbSet<FollowUpAction> FollowUpActions { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
    }
}