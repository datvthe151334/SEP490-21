using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO;

public class WishListDTO
{
    public int WishlistId { get; set; }
    public int? AccountId { get; set; }
    public int? JobId { get; set; }
    public int? Status { get; set; }
    public int TotalCount  { get; set; }

    public virtual Account? Account { get; set; }
    public virtual JobPosting? Job { get; set; }
}