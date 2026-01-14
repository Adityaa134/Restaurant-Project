using System;
using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Core.Service
{
    public class RemoveCartItemsService : IRemoveCartItemsService
    {
        private readonly ICartsRepository _cartsRepository;

        public RemoveCartItemsService(ICartsRepository cartsRepository)
        {
            _cartsRepository = cartsRepository;
        }

        public async Task<bool> RemoveCartItem(Guid cartId)
        {
            if (cartId == Guid.Empty)
                throw new ArgumentNullException(nameof(cartId));

            Carts? matchingCart = await _cartsRepository.GetCartItemByCartId(cartId);
            if (matchingCart == null)
                return false;

            bool isDeleted = await _cartsRepository.RemoveItemFromCartByCartId(matchingCart.Id);
            if (isDeleted)
                return true;
            return false;
        }

        public async Task<bool> RemoveItemsFromCartByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentNullException(nameof(userId));
            return await _cartsRepository.RemoveItemsFromCartByUserId(userId);
        }
    }
}
