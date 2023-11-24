using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories;

public interface IWishListRepository
{
    Task<List<WishListDTO>> GetListWishLists(int accountId, string? searchString, string? orderBy, bool? isSortAscending, int page, int size);
    Task<List<WishList?>> GetWishList(int accountId);
    Task<WishList> AddWishList(WishList wishList);
    Task<bool> DeleteWishList(int jobId);
}