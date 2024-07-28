using Microsoft.EntityFrameworkCore;
using OpenWhistle.Models;

namespace OpenWhistle.Services
{
    public interface IReportService
    {
        public Task<WhistleblowerReport> CreateReportAsync(string description,string accessPin, DateTime? dateOfOccurrence = null,
            List<byte[]>? fileList = null);
        Task<List<WhistleblowerReport>> GetReportsAsync();
        Task<WhistleblowerReport?> FindReportAsync(Guid id);
        IQueryable<WhistleblowerReport> GetReportsQueryable();
    }


    public class ReportService(OpenWhistleDbContext context) : IReportService
    {
        public async Task<WhistleblowerReport> CreateReportAsync(string description, string accessPin, DateTime? dateOfOccurrence = null, List<byte[]>? fileList = null)
        {
            var report = new WhistleblowerReport(description, dateOfOccurrence){AccessPin = accessPin};
            if (fileList != null && fileList.Count != 0)
            {
                report.Files.AddRange(fileList);
            }
            context.Reports.Add(report);
            await context.SaveChangesAsync();
            return report;
        }

        public async Task<List<WhistleblowerReport>> GetReportsAsync()
        {
            return await context.Reports.ToListAsync();
        }

        public async Task<WhistleblowerReport?> FindReportAsync(Guid id)
        {
            return await context.Reports.FindAsync(id);
        }

        public IQueryable<WhistleblowerReport> GetReportsQueryable()
        {
            return context.Reports.AsNoTracking();
        }
    }
}