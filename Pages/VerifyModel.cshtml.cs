using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OpenWhistle.Models;
using OpenWhistle.Services;

namespace OpenWhistle.Pages;

public class VerifyModel : PageModel
{
    private readonly OpenWhistleDbContext _context;
    private readonly ITokenService _tokenService;

    public VerifyModel(OpenWhistleDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [BindProperty]
    public string EnteredPin { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        var reportIdStr = TempData["ReportId"]?.ToString();
        if (reportIdStr == null)
        {
            return BadRequest();
        }

        var reportId = Guid.Parse(reportIdStr);
        var report = await _context.Reports.FirstOrDefaultAsync(r => r.Id == reportId);

        if (report == null || report.AccessPin != EnteredPin)
        {
            ModelState.AddModelError(string.Empty, "Invalid PIN.");
            TempData["InvalidPin"] = "true";
            return Page();
        }

        var token = _tokenService.GenerateToken(report.Id, 10);

        TempData["AccessToken"] = token;
        return RedirectToPage("/ReportDetails", new { id = reportId });
    }
}