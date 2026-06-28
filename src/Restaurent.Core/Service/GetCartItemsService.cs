using System;
using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Core.DTO;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Core.Service
{
    public class GetCartItemsService : IGetCartItemsService
    {
        private readonly ICartsRepository _cartRepository;
        public GetCartItemsService(ICartsRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }


        public async Task<List<CartResponse>> GetAllCartItems(Guid? userId)
        {
            if (userId == null)
            {
                List<Carts> cartsItemsUserIdIsNull = await _cartRepository.GetAllCartItemsWithoutUserId();
                return cartsItemsUserIdIsNull.Select(temp => temp.CartResponse())
                    .ToList();
            }

            List<Carts> cartsItems = await _cartRepository.GetAllCartItemsWithUserId(userId.Value);
            return cartsItems.Select(temp => temp.CartResponse())
                .ToList();
        }

        public async Task<bool> IsCartItemExist(Guid? userId, Guid dishId)
        {
            if (dishId == Guid.Empty)
                return false;

            bool isProductPresent = await _cartRepository.IsCartItemExist(userId, dishId);

            if (isProductPresent)
                return true;
            return false;
        }
    }
}
