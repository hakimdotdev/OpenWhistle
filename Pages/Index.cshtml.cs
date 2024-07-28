using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenWhistle.Models;
using OpenWhistle.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OpenWhistle.Pages
{
    public class DashboardModel(ILogger<DashboardModel> logger, IReportService reportService)
        : PageModel
    {
        private readonly ILogger<DashboardModel> _logger = logger;

        public int OpenReportsCount { get; set; }
        public int UnopenedReportsCount { get; set; }
        public int ConfirmedReceptionCount { get; set; }
        public int ReportsWithoutActionsCount { get; set; }

        public List<WhistleblowerReport> UpcomingDeadlines { get; set; }
        public List<WhistleblowerReport> HistoryItems { get; set; }
        public PaginatedList<WhistleblowerReport> Reports { get; set; }

        public async Task<IActionResult> OnGetAsync(int pageIndex = 1, int pageSize = 10)
        {
            IQueryable<WhistleblowerReport> reportsQuery = reportService.GetReportsQueryable();

            Reports = await PaginatedList<WhistleblowerReport>.CreateAsync(reportsQuery, pageIndex, pageSize);

            var reports = Reports.ToList();
            OpenReportsCount = reportsQuery.Count(r => r.Status == ReportStatus.Read);
            UnopenedReportsCount = reportsQuery.Count(r => r.Status == ReportStatus.Received);
            ConfirmedReceptionCount = reportsQuery.Count(r => r.Status == ReportStatus.Acknowledged);
            ReportsWithoutActionsCount = reportsQuery.Count(r => r.Status == ReportStatus.ActionTaken);
            UpcomingDeadlines = reportsQuery.ToList().Where(r => r.Deadline <= DateTime.Now.AddMonths(3)).OrderBy(r => r.Deadline).Take(5).ToList();
            HistoryItems = reportsQuery.ToList().OrderByDescending(r => r.DateCreated).Take(5).ToList();

            return Page();
        }
    }
}