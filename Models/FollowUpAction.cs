using OpenWhistle.Models.Common;

namespace OpenWhistle.Models;

public class FollowUpAction : BaseEntity
{
    private FollowUpAction(){}
    public string ActionDescription { get; set; }
    public WhistleblowerReport Report { get; set; }
}