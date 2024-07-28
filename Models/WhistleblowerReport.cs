using System;
using System.Collections.Generic;
using OpenWhistle.Models.Common;

namespace OpenWhistle.Models
{
    public class WhistleblowerReport : BaseEntity
    {
        public WhistleblowerReport(){}
        public WhistleblowerReport(string description)
        {
            Description = description;
            DateOfOccurrence = null;
            Files = new List<byte[]>();
            Status = ReportStatus.Received;
            IsAssigned = false;
        }
        
        public WhistleblowerReport(string description, DateTime ?dateOfOccurrence)
        {
            Description = description;
            DateOfOccurrence = dateOfOccurrence;
            Files = new List<byte[]>();
            Status = ReportStatus.Received;
            IsAssigned = false;
        }

        public bool IsAssigned { get; set; } = false;
        public string AssignedTo { get; set; } = String.Empty;
        public string Description { get; init; }
        public DateTime? DateOfOccurrence { get; init; }
        public List<byte[]> Files { get; }
        public ReportStatus Status { get; set; } = ReportStatus.Received;
        public List<FollowUpAction> ActionsTaken { get; set; }

        public DateTime Deadline => Status switch
        {
            ReportStatus.Acknowledged => DateCreated.AddMonths(3),
            ReportStatus.Received or ReportStatus.Read => DateCreated.AddDays(7),
            _ => DateCreated.AddDays(7),
        };

        public List<ChatMessage> ChatMessages { get; set; }
        public string AccessPin { get; set; }

        public void AddFile(byte[] file)
        {
            Files.Add(file);
        }
    }
    
}