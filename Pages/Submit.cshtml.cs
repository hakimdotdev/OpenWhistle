using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenWhistle.Services;

namespace OpenWhistle.Pages;

public class SubmitModel(IReportService reportService) : PageModel
{
    [BindProperty] public string ReportDescription { get; set; }

    [BindProperty] public DateTime? DateOfOccurrence { get; set; }

    [BindProperty] public TimeSpan? TimeOfOccurrence { get; set; }

    [BindProperty] public IFormFileCollection Upload { get; set; }

    public Guid? Id { get; set; }

    public string? GeneratedPin { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        var totalSize = Upload.Sum(f => f.Length);

        var fileBytesList = new List<byte[]>();

        foreach (var formFile in Upload.Where(f => f.Length > 0))
        {
            using var memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            fileBytesList.Add(memoryStream.ToArray());
        }

        var dateTimeOfOccurrence = DateOfOccurrence?.Date ?? DateTime.UtcNow;

        if (TimeOfOccurrence.HasValue) dateTimeOfOccurrence = dateTimeOfOccurrence.Add(TimeOfOccurrence.Value);
        
        GeneratedPin = new string(Array.ConvertAll(RandomNumberGenerator.GetBytes(16), b => "0123456789"[b % 10]));


        if (fileBytesList.Count != 0)
        {
            Id = reportService.CreateReportAsync(ReportDescription, GeneratedPin, dateTimeOfOccurrence, fileBytesList).Result.Id;
            return Page();
        }

        Id = reportService.CreateReportAsync(ReportDescription, GeneratedPin, dateTimeOfOccurrence).Result.Id;
        
        return Page();
    }
}