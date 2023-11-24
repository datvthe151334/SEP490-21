using Microsoft.AspNetCore.Mvc;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repositories;

namespace RecruitXpress_BE.Controllers;

[Route("api/WishList/")]
[ApiController]
public class WishListController : ControllerBase
{
    private readonly IWishListRepository _wishListRepository;

    public WishListController(IWishListRepository wishListRepository)
    {
        _wishListRepository = wishListRepository;
    }

    //GET: api/WishList
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WishListDTO>>> GetListWishLists(int accountId, string? searchString, string? orderBy,
        bool? isSortAscending, int page, int size) =>
        await _wishListRepository.GetListWishLists(accountId, searchString, orderBy, isSortAscending, page, size);

    // //GET: api/WishList/{accountId}
    // [HttpGet("accountId")]
    // public async Task<ActionResult<List<WishList?>>> GetWishList(int accountId)
    // {
    //     var wishLists = await _wishListRepository.GetWishList(accountId);
    //     return wishLists;
    // }

    //POST: api/WishList
    [HttpPost]
    public async Task<ActionResult<WishList>> AddWishList(WishList wishList)
    {
        try
        {
            var result = await _wishListRepository.AddWishList(wishList);
            return CreatedAtAction(nameof(AddWishList), new { id = result.WishlistId }, result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e.Message);
        }
    }

    //DELETE: api/WishList/{id}
    [HttpDelete("id")]
    public async Task<IActionResult> DeleteWishList(int id)
    {
        try
        {
            var deleted = await _wishListRepository.DeleteWishList(id);
            if (deleted)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500);
        }
    }
}