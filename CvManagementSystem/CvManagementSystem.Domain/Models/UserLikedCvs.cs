namespace UserService.Domain.Models;

public class UserLikedCvs
{
    public User User { get; set; } = null!;
    public Guid UserId { get; set; }
    public Cv Cv { get; set; } = null!;
    public Guid CvId { get; set; }
}