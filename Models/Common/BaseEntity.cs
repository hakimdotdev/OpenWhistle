namespace OpenWhistle.Models.Common;

public abstract class BaseEntity
{
    public Guid Id = Guid.NewGuid();
    public DateTime DateCreated = DateTime.UtcNow;
}