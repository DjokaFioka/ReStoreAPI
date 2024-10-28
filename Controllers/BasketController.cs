using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReStoreAPI.Data;
using ReStoreAPI.DTOs;
using ReStoreAPI.Entities;
using ReStoreAPI.Extensions;

namespace ReStoreAPI.Controllers
{
    public class BasketController : BaseApiController
    {
        private readonly StoreContext _context;

        public BasketController(StoreContext context)
        {
            _context = context;
        }

        [HttpGet(Name = "GetBasket")]
        public async Task<ActionResult<BasketDto>> GetBasket()
        {
            var basket = await RetrieveBasket(GetBuyerId());

            if (basket == null)
                return NotFound();
            return basket.MapBasketToDto();
        }

        [HttpPost] // api/basket?productId=3&quantity=2
        public async Task<ActionResult<BasketDto>> AddItemToBasket(int productId, int quantity)
        {
            // get basket or create basket
            var basket = await RetrieveBasket(GetBuyerId());
            if (basket == null)
                basket = CreateBasket();
            
            // get product
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return BadRequest(new ProblemDetails
                {
                    Title = "Product Not Found"
                });

            // add item
            basket.AddItem(product, quantity);
            
            // save changes
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
                return CreatedAtRoute("GetBasket", basket.MapBasketToDto()); //GetBasket is named the GET method
            
            return BadRequest(new ProblemDetails { Title = "Problem saving item to basket"});
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveBasketItem(int productId, int quantity)
        {
            // get basket
            var basket = await RetrieveBasket(GetBuyerId());
            if (basket == null)
                return NotFound();

            // remove item or reduce qty
            basket.RemoveItem(productId, quantity);

            // save changes
            var result = await _context.SaveChangesAsync() > 0;
            
            if (result)
                return Ok();

            return BadRequest(new ProblemDetails { Title = "Problem removing item from the basket" });
        }

        private async Task<Basket> RetrieveBasket(string buyerId)
        {
            if (string.IsNullOrEmpty(buyerId))
            {
                Response.Cookies.Delete("buyerId");
                return null;
            }

            return await _context.Baskets
                .Include(i => i.Items)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(x => x.BuyerId == buyerId); //we store it in a cookie

        }

        private string GetBuyerId()
        {
            string buyerId = User.Identity?.Name ?? Request.Cookies["buyerId"];
            return buyerId;
        }


        private Basket CreateBasket()
        {
            var buyerId = User.Identity?.Name;//Guid.NewGuid().ToString();
            if (string.IsNullOrEmpty(buyerId))
            {
                buyerId = Guid.NewGuid().ToString();
                var cookieOptions = new CookieOptions { IsEssential = true, Expires = DateTime.Now.AddDays(30) };
                Response.Cookies.Append("buyerId", buyerId, cookieOptions);
            }

            var basket = new Basket { BuyerId = buyerId };

            _context.Baskets.Add(basket);
            
            return basket;
        }
    }
}
