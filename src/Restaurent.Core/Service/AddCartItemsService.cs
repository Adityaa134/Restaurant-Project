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

        public async Task<AddToCartResponse> AddItemToCart(AddToCartRequest addToCart)
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

            return cartItem.ToAddToCartResponse();
        }
    }
}
