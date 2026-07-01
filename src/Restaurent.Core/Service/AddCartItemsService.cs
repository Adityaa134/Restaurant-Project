using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Core.DTO;
using Restaurent.Core.Helpers;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Core.Service
{
    public class AddCartItemsService : IAddCartItemsService
    {
        private readonly ICartsRepository _cartsRepository;
        private readonly IDishGetterService _dishGetterService;
        public AddCartItemsService(ICartsRepository cartsRepository, IDishGetterService dishGetterService)
        {
            _cartsRepository = cartsRepository;
            _dishGetterService = dishGetterService;
        }

        public async Task<CartResponse> AddItemToCart(AddToCartRequest addToCart)
        {
            if (addToCart == null)
                throw new ArgumentNullException(nameof(addToCart));

            ValidationHelper.ModelValidator(addToCart);

            DishResponse? dishResponse  = await _dishGetterService.GetDishByDishId(addToCart.DishId);

            if (dishResponse == null)
                throw new ArgumentException("Invalid Dish Id");

            Carts cart = addToCart.ToCart();
            cart.Id = Guid.NewGuid();

            Carts cartItem = await _cartsRepository.AddItemToCart(cart);

            return cartItem.CartResponse();
        }

        public async Task<List<CartResponse>> AddOrderItemsToCart(List<ReorderCartItemRequest> items)
        {
            foreach (var item in items)
            {
                bool isDishExist = await _dishGetterService.IsDishExist(item.DishId);

                if (!isDishExist)
                    throw new InvalidOperationException($"Dish {item.DishId} is not available");
            }

            List<Carts> carts = items.Select(t=>t.ToCart()).ToList();

            var addedCarts = await _cartsRepository.AddOrderItemsToCart(carts);
            return addedCarts.Select(t=>t.CartResponse()).ToList();
        }

        public async Task<List<CartResponse>> MergeCart(Guid userId, List<MergeCartRequest> items)
        {
            var existingCartItems = await _cartsRepository.GetAllCartItemsWithUserId(userId);

            foreach (var item in items)
            {
                var existingItem = existingCartItems
                    ?.FirstOrDefault(x => x.DishId == item.DishId);

                if (existingItem != null)
                {
                    await _cartsRepository.UpdateCartItemQuantity(existingItem,item.Quantity);
                }
                else
                {
                    var newCart = new Carts
                    {
                        UserId = userId,
                        DishId = item.DishId,
                        Quantity = item.Quantity
                    };

                    await _cartsRepository.AddItemToCart(newCart);
                }
            }

            var updatedCart =
                await _cartsRepository.GetAllCartItemsWithUserId(userId);

            return updatedCart
                .Select(x => x.CartResponse())
                .ToList();
        }
    }
}
