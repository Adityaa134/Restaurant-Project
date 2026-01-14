using System;
using Microsoft.EntityFrameworkCore;
using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Infrastructure.DBContext;

namespace Restaurent.Infrastructure.Repositories
{
    public class CartsRepository : ICartsRepository
    {
        private readonly ApplicationDBContext _db;
        public CartsRepository(ApplicationDBContext db)
        {
            _db = db;
        }

        public async Task<Carts> AddItemToCart(Carts cart)
        {
            await _db.Carts.AddAsync(cart);
            await _db.SaveChangesAsync();
            Carts? cartItem = await GetCartItemByCartId(cart.Id);
            return cartItem;
        }

        public async Task<List<Carts>> GetAllCartItemsWithUserId(Guid userId)
        {
            return await _db.Carts
                .Include(t => t.Dishes)
                .Where(t => t.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Carts>> GetAllCartItemsWithoutUserId()
        {
            return await _db.Carts
                .Include(t => t.Dishes)
                .Where(t => t.UserId == null)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Carts?> GetCartItemByCartId(Guid cartId)
        {
            return await _db.Carts
                  .Include(t => t.Dishes)
                  .Include(t=>t.Users)
                  .AsNoTracking()
                  .FirstOrDefaultAsync(temp => temp.Id == cartId);
        }

        public async Task<bool> RemoveItemFromCartByCartId(Guid cartId)
        {
            _db.Carts.RemoveRange(_db.Carts.Where(t => t.Id == cartId));
            int rowsDeleted = await _db.SaveChangesAsync();
            return rowsDeleted > 0;
        }

        public async Task<bool> RemoveItemsFromCartByUserId(Guid userId)
        {
            _db.Carts.RemoveRange(_db.Carts.Where(t => t.UserId == userId));
            int rowsDeleted = await _db.SaveChangesAsync();
            return rowsDeleted > 0;
        }

        public async Task<Carts> UpdateCartItemQuantity(Carts cart, int updatedQuantity)
        {
            Carts? mathcingCart = await _db.Carts.Include(t => t.Dishes).FirstOrDefaultAsync(temp => temp.Id == cart.Id);

            if (mathcingCart == null)
            {
                return cart;
            }
            mathcingCart.Quantity += updatedQuantity;
            await _db.SaveChangesAsync();
            if (mathcingCart.Quantity == 0)
            {
                await RemoveItemFromCartByCartId(mathcingCart.Id);
            }
            return mathcingCart;
        }

        public async Task<bool> IsCartItemExist(Guid? userId, Guid dishId)
        {
            Carts? cart = await _db.Carts.FirstOrDefaultAsync(t => t.UserId == userId && t.DishId == dishId);
            if (cart == null)
                return false;
            return true;
        }

        public async Task<Carts?> GetcartItemByUserIdDishId(Guid userId, Guid dishId)
        {
            return await _db.Carts
                .AsNoTracking()
                .FirstOrDefaultAsync(temp => temp.UserId == userId && temp.DishId == dishId);

        }
    }
}
