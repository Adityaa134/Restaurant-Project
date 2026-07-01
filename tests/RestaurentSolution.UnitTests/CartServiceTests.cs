using AutoFixture;
using FluentAssertions;
using Moq;
using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.Identity;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Core.DTO;
using Restaurent.Core.Service;
using Restaurent.Core.ServiceContracts;

namespace RestaurentSolution.UnitTests
{
    public class CartServiceTests
    {
        private readonly IFixture _fixture;
        private readonly IAddCartItemsService _addCartItemsService;
        private readonly IGetCartItemsService _getCartItemsService;
        private readonly IRemoveCartItemsService _removeCartItemsService;
        private readonly IUpdateItemQuantityInCart _updateItemQuantityInCart;

        private readonly Mock<IDishGetterService> _dishGetterServiceMock;
        private readonly Mock<ICartsRepository> _cartsRepositoryMock;

        public CartServiceTests()
        {
            _fixture = new Fixture();
            _cartsRepositoryMock = new Mock<ICartsRepository>();
            _dishGetterServiceMock = new Mock<IDishGetterService>();

            _addCartItemsService = new AddCartItemsService(_cartsRepositoryMock.Object,_dishGetterServiceMock.Object);
            _getCartItemsService = new GetCartItemsService(_cartsRepositoryMock.Object);
            _removeCartItemsService = new RemoveCartItemsService(_cartsRepositoryMock.Object);
            _updateItemQuantityInCart = new UpdateItemQuantityInCart(_cartsRepositoryMock.Object);
        }


        #region GetCartItems

        [Fact]
        public async Task GetAllCartItems_UserIdIsNull_ShouldReturnMatchingCartItemsOrEmpty()
        {
            Guid? userId = null;
            List<Carts> carts = new List<Carts>();
            _cartsRepositoryMock.Setup(temp => temp.GetAllCartItemsWithoutUserId())
                .ReturnsAsync(carts);

            List<CartResponse> cartResponses = await _getCartItemsService.GetAllCartItems(userId);

            cartResponses.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllCartItems_UserIdIsNotNull_ShouldReturnMatchingCartItems()
        {
            Guid userId = Guid.NewGuid();
            Dish dish = _fixture.Build<Dish>()
                 .With(t => t.Category, null as Category)
                 .With(t => t.CartItems, null as List<Carts>)
                 .With(t => t.OrderItems, null as List<OrderItem>)
                 .With(t => t.Ratings, null as List<Rating>)
                 .Create();
            List<Carts> carts = new List<Carts>()
            {
                _fixture.Build<Carts>()
                .With(t => t.Users, null as ApplicationUser)
                .With(t => t.Dishes, dish)
                .With(t=>t.UserId, userId)
                .Create(),
               _fixture.Build<Carts>()
                .With(t => t.Users, null as ApplicationUser)
                .With(t => t.Dishes, dish)
                .With(t=>t.UserId, userId)
                .Create(),
               _fixture.Build<Carts>()
                .With(t => t.Users, null as ApplicationUser)
                .With(t => t.Dishes, dish)
                .With(t=>t.UserId, userId)
                .Create()
            };

            List<CartResponse> cartsResponsesExpected = carts.Select(temp => temp.CartResponse()).ToList();

            _cartsRepositoryMock.Setup(temp => temp.GetAllCartItemsWithUserId(It.IsAny<Guid>()))
                .ReturnsAsync(carts);

            List<CartResponse> cartResponsesActual = await _getCartItemsService.GetAllCartItems(userId);

            cartResponsesActual.Should().NotBeEmpty();
            cartResponsesActual.Should().BeEquivalentTo(cartsResponsesExpected);
        }
 
        #endregion 

        #region AddItemsTocart

        [Fact]
        public async Task AddItemToCart_NullObject_ThrowArgumentNullException()
        {
            AddToCartRequest? request = null;

            Func<Task> action = async () =>
            {
                await _addCartItemsService.AddItemToCart(request);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddItemToCart_ValidCartRequest_ShouldBeSuccessfull()
        {
            Dish dish = _fixture.Build<Dish>()
                 .With(t => t.Category, null as Category)
                 .With(t => t.CartItems, null as List<Carts>)
                 .With(t => t.OrderItems, null as List<OrderItem>)
                 .With(t=>t.Ratings,null as List<Rating>)
                 .Create();

            DishResponse dishResponse = dish.ToDishResponse();

            Carts cart = _fixture.Build<Carts>()
                                       .With(t=>t.Users,null as ApplicationUser)
                                       .With(t=>t.Dishes,dish)
                                       .Create();

            AddToCartRequest request = _fixture.Build<AddToCartRequest>()
                                       .With(t=>t.UserId,cart.UserId)
                                       .With(t=>t.DishId,cart.DishId)
                                       .With(t=>t.Quantity,cart.Quantity)
                                       .Create();

            CartResponse cartResponseExpected = cart.CartResponse();

            _dishGetterServiceMock.Setup(temp=>temp.GetDishByDishId(It.IsAny<Guid>()))
                .ReturnsAsync(dishResponse);

            _cartsRepositoryMock.Setup(temp => temp.AddItemToCart(It.IsAny<Carts>()))
                .ReturnsAsync(cart);

            CartResponse cartResponseActual = await _addCartItemsService.AddItemToCart(request);

            cartResponseExpected.CartId = cartResponseActual.CartId;

            cartResponseActual.Should().NotBeNull();
            cartResponseActual.Should().BeEquivalentTo(cartResponseExpected);
        }

        [Fact]
        public async Task AddOrderItemsToCart_IfDishDoesNotExist_ShouldThrowInvalidOperationException()
        {
            List<ReorderCartItemRequest> items = _fixture
                .Build<ReorderCartItemRequest>()
                .With(t=>t.UserId,Guid.Parse("5CABD4C0-2493-47F7-B0CC-01D113B86420"))
                .CreateMany(2)
                .ToList();

            _dishGetterServiceMock
                .Setup(temp => temp.IsDishExist(It.IsAny<Guid>()))
                .ReturnsAsync(false);

            Func<Task> action = async () =>
            {
                await _addCartItemsService.AddOrderItemsToCart(items);
            };

            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task AddOrderItemsToCart_WithValidItems_ShouldAddItemsToCart()
        {
            Dish dish1 = _fixture.Build<Dish>()
                 .With(t => t.Category, null as Category)
                 .With(t => t.CartItems, null as List<Carts>)
                 .With(t => t.OrderItems, null as List<OrderItem>)
                 .With(t => t.Ratings, null as List<Rating>)
                 .Create();

            Dish dish2 = _fixture.Build<Dish>()
                 .With(t => t.Category, null as Category)
                 .With(t => t.CartItems, null as List<Carts>)
                 .With(t => t.OrderItems, null as List<OrderItem>)
                 .With(t => t.Ratings, null as List<Rating>)
                 .Create();

            List<ReorderCartItemRequest> items = new List<ReorderCartItemRequest>()
            {
                _fixture.Build<ReorderCartItemRequest>()
                        .With(t=>t.DishId,dish1.DishId)
                        .With(t=>t.Quantity,2)
                        .With(t=>t.UserId,Guid.Parse("1C19DEA7-2BB6-4766-A490-D076563BD301"))
                        .Create(),
                _fixture.Build<ReorderCartItemRequest>()
                        .With(t=>t.DishId,dish2.DishId)
                        .With(t=>t.Quantity,1)
                        .With(t=>t.UserId,Guid.Parse("1C19DEA7-2BB6-4766-A490-D076563BD301"))
                        .Create()
            };

            List<Carts> cartsExpected = new List<Carts>()
            {
                _fixture.Build<Carts>()
                        .With(t=>t.Users,null as ApplicationUser)
                        .With(t=>t.DishId,dish1.DishId)
                        .With(t=>t.Dishes,dish1)
                        .With(t=>t.Quantity,2)
                        .Create(),
                _fixture.Build<Carts>()
                        .With(t=>t.Users,null as ApplicationUser)
                        .With(t=>t.DishId,dish2.DishId)
                        .With(t=>t.Dishes,dish2)
                        .With(t=>t.Quantity,1)
                        .Create()
            };

            _dishGetterServiceMock
                .Setup(temp => temp.IsDishExist(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            _cartsRepositoryMock
                .Setup(temp => temp.AddOrderItemsToCart(It.IsAny<List<Carts>>()))
                .ReturnsAsync(cartsExpected);

            List<CartResponse> cartResponseActual =
                await _addCartItemsService.AddOrderItemsToCart(items);

            cartResponseActual.Should().NotBeNull();
            cartResponseActual.Count.Should().Be(items.Count);

            cartResponseActual[0].DishId.Should().Be(dish1.DishId);
            cartResponseActual[0].Quantity.Should().Be(2);

            cartResponseActual[1].DishId.Should().Be(dish2.DishId);
            cartResponseActual[1].Quantity.Should().Be(1);
        }

        #endregion

        #region  UpdateDishQuantityInCartItem

        [Fact]
         public async Task UpdateDishQuantityInCartItem_InvalidCartId_ArgumentException()
         {
            UpdateQuantityRequest request = _fixture.Build<UpdateQuantityRequest>().Create(); 

             _cartsRepositoryMock.Setup(temp => temp.GetCartItemByCartId(It.IsAny<Guid>()))
                 .ReturnsAsync((Carts?)null);

             Func<Task> action = async () =>
             {
                 await _updateItemQuantityInCart.UpdateDishQuantityInCartItem(request);
             };

             await action.Should().ThrowAsync<ArgumentException>();
         }

         [Fact]
         public async Task UpdateDishQuantityInCartItem_ValidCartId_ShouldReturnUpdatedCartDetails()
         {
            UpdateQuantityRequest request = _fixture.Build<UpdateQuantityRequest>().Create();

            Dish dish = _fixture.Build<Dish>()
                 .With(t => t.Category, null as Category)
                 .With(t => t.CartItems, null as List<Carts>)
                 .With(t => t.OrderItems, null as List<OrderItem>)
                 .With(t => t.Ratings, null as List<Rating>)
                 .Create();
            Carts cart = _fixture.Build<Carts>()
                                       .With(t=>t.Id, request.CartId)
                                       .With(t => t.Users, null as ApplicationUser)
                                       .With(t => t.Dishes, dish)
                                       .Create();
            CartResponse cartResponseExpected = cart.CartResponse();

            _cartsRepositoryMock.Setup(temp => temp.GetCartItemByCartId(It.IsAny<Guid>()))
                 .ReturnsAsync(cart);

            _cartsRepositoryMock.Setup(temp => temp.UpdateCartItemQuantity(It.IsAny<Carts>(), It.IsAny<int>()))
                 .ReturnsAsync(cart);

             CartResponse? cartResponseActual = await _updateItemQuantityInCart.UpdateDishQuantityInCartItem(request);

            cartResponseActual.Should().NotBeNull();
            cartResponseActual.Should().BeEquivalentTo(cartResponseExpected);
         }

         #endregion 

        #region RemoveCartItem

        [Fact]
        public async Task RemoveCartItem_InvalidCartId_ShouldBeFalse()
        {
            bool isDeleted = await _removeCartItemsService.RemoveCartItem(Guid.NewGuid());

            isDeleted.Should().BeFalse();
        }

        [Fact]
        public async Task RemoveCartItem_ValidCartId_ShouldBeTrue()
        {
            Carts cart = _fixture.Build<Carts>()
                                       .With(t => t.Users, null as ApplicationUser)
                                       .With(t => t.Dishes, null as Dish)
                                       .Create();

            _cartsRepositoryMock.Setup(temp => temp.GetCartItemByCartId(It.IsAny<Guid>()))
                .ReturnsAsync(cart);

            _cartsRepositoryMock.Setup(temp => temp.RemoveItemFromCartByCartId(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            bool isDeleted = await _removeCartItemsService.RemoveCartItem(cart.Id);

            isDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task RemoveItemsFromCartByUserId_InvalidUserId_ShouldBeFalse()
        {
            bool isDeleted = await _removeCartItemsService.RemoveItemsFromCartByUserId(Guid.NewGuid());

            isDeleted.Should().BeFalse();
        }

        [Fact]
        public async Task RemoveItemsFromCartByUserId_ValidUserId_ShouldBeTrue()
        {
            Carts cart = _fixture.Build<Carts>()
                                       .With(t => t.Users, null as ApplicationUser)
                                       .With(t => t.Dishes, null as Dish)
                                       .Create();

            _cartsRepositoryMock.Setup(temp => temp.RemoveItemsFromCartByUserId(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            bool isDeleted = await _removeCartItemsService.RemoveItemsFromCartByUserId(cart.UserId.Value);

            isDeleted.Should().BeTrue();
        }
        #endregion

        #region MergeCart

        [Fact]
        public async Task MergeCart_WithNewItems_ShouldAddItemsToCart()
        {
            Guid userId = Guid.NewGuid();

            List<MergeCartRequest> items = new()
            {
                new() { DishId = Guid.NewGuid(), Quantity = 2 }
            };

            Dish dish = _fixture.Build<Dish>()
                 .With(t => t.Category, null as Category)
                 .With(t => t.CartItems, null as List<Carts>)
                 .With(t => t.OrderItems, null as List<OrderItem>)
                 .With(t => t.Ratings, null as List<Rating>)
                 .With(t => t.DishId, items.First()?.DishId)
                 .Create();

            Carts carts = _fixture.Build<Carts>()
                                  .With(t => t.Users, null as ApplicationUser)
                                  .With(t => t.Dishes, dish)
                                  .With(t => t.DishId, items.First()?.DishId)
                                  .With(t => t.Quantity, items.First().Quantity)
                                  .Create();

            List<Carts> carts1 = new List<Carts>()
            {
                carts
            };

            List<CartResponse> cartResponsesExpected = new List<CartResponse>()
            {
                carts.CartResponse()
            };

            _cartsRepositoryMock
                .Setup(x => x.AddItemToCart(It.IsAny<Carts>()))
                .ReturnsAsync(carts);

            _cartsRepositoryMock
                .SetupSequence(x => x.GetAllCartItemsWithUserId(It.IsAny<Guid>()))
                .ReturnsAsync(new List<Carts>())
                .ReturnsAsync(carts1);

            List<CartResponse> cartResponsesActual =
                await _addCartItemsService.MergeCart(userId, items);

            cartResponsesActual.Should().NotBeNull();
            cartResponsesActual.Count.Should().Be(1);
            cartResponsesActual.Should().BeEquivalentTo(cartResponsesExpected);
        }

        [Fact]
        public async Task MergeCart_WithExistingItem_ShouldUpdateQuantity()
        {
            Guid userId = Guid.NewGuid();

            Dish dish = _fixture.Build<Dish>()
                 .With(t => t.Category, null as Category)
                 .With(t => t.CartItems, null as List<Carts>)
                 .With(t => t.OrderItems, null as List<OrderItem>)
                 .With(t => t.Ratings, null as List<Rating>)
                 .Create();

            List<MergeCartRequest> items = new()
            {
                new() { DishId = dish.DishId, Quantity = 2 }
            };

            Carts carts = _fixture.Build<Carts>()
                                  .With(t => t.Users, null as ApplicationUser)
                                  .With(t => t.Dishes, dish)
                                  .With(t => t.DishId, dish.DishId)
                                  .With(t => t.Quantity, 1)
                                  .Create();

            Carts updatedCart = _fixture.Build<Carts>()
                                .With(t => t.Users, null as ApplicationUser)
                                .With(t => t.Dishes, dish)
                                .With(t => t.DishId, dish.DishId)
                                .With(t => t.Quantity, carts.Quantity + items.First().Quantity)
                                .Create();

            List<Carts> existingCart = new()
            {
                carts
            };

            List<Carts> cartItems = new List<Carts>()
            {
                updatedCart
            };

            List<CartResponse> cartResponsesExpected = new List<CartResponse>()
            {
                updatedCart.CartResponse()
            };

            _cartsRepositoryMock
                .Setup(x => x.UpdateCartItemQuantity(
                    It.IsAny<Carts>(),
                    It.IsAny<int>()
                ))
                .ReturnsAsync(updatedCart);

            _cartsRepositoryMock
                .SetupSequence(x => x.GetAllCartItemsWithUserId(It.IsAny<Guid>()))
                .ReturnsAsync(existingCart)
                .ReturnsAsync(cartItems);

            List<CartResponse> cartResponsesActual =
                await _addCartItemsService.MergeCart(userId, items);

            cartResponsesActual.Should().NotBeNull();
            cartResponsesExpected.Should().BeEquivalentTo(cartResponsesActual);
        }

        [Fact]
        public async Task MergeCart_WithExistingAndNewItems_ShouldMergeCorrectly()
        {
            Guid userId = Guid.NewGuid();

            Dish existingDish = _fixture.Build<Dish>()
                 .With(t => t.Category, null as Category)
                 .With(t => t.CartItems, null as List<Carts>)
                 .With(t => t.OrderItems, null as List<OrderItem>)
                 .With(t => t.Ratings, null as List<Rating>)
                 .Create();

            Dish newDish = _fixture.Build<Dish>()
                 .With(t => t.Category, null as Category)
                 .With(t => t.CartItems, null as List<Carts>)
                 .With(t => t.OrderItems, null as List<OrderItem>)
                 .With(t => t.Ratings, null as List<Rating>)
                 .Create();

            List<MergeCartRequest> items = new()
            {
                new() { DishId = newDish.DishId, Quantity = 2 },
                new() { DishId = existingDish.DishId, Quantity = 1 }
            };

            Carts existingCart = _fixture.Build<Carts>()
                                  .With(t => t.Users, null as ApplicationUser)
                                  .With(t => t.Dishes, existingDish)
                                  .With(t => t.DishId, existingDish.DishId)
                                  .With(t => t.Quantity, 1)
                                  .Create();

            Carts updatedExistingCart = _fixture.Build<Carts>()
                                .With(t => t.Users, null as ApplicationUser)
                                .With(t => t.Dishes, existingDish)
                                .With(t => t.DishId, existingDish.DishId)
                                .With(t => t.Quantity, existingCart.Quantity + items[1].Quantity)
                                .Create();

            List<Carts> existingCartItems = new()
            {
                existingCart
            };

            Carts newCart = _fixture.Build<Carts>()
                                  .With(t => t.Users, null as ApplicationUser)
                                  .With(t => t.Dishes, newDish)
                                  .With(t => t.DishId, newDish.DishId)
                                  .With(t => t.Quantity, items.First().Quantity)
                                  .Create();

            List<Carts> cartItems = new List<Carts>()
            {
                updatedExistingCart,
                newCart
            };

            List<CartResponse> cartResponsesExpected = cartItems.Select(t => t.CartResponse()).ToList();

            _cartsRepositoryMock
                .Setup(x => x.UpdateCartItemQuantity(
                    It.IsAny<Carts>(),
                    It.IsAny<int>()
                ))
                .ReturnsAsync(updatedExistingCart);

            _cartsRepositoryMock
                .Setup(x => x.AddItemToCart(It.IsAny<Carts>()))
                .ReturnsAsync(newCart);

            _cartsRepositoryMock
                .SetupSequence(x => x.GetAllCartItemsWithUserId(It.IsAny<Guid>()))
                .ReturnsAsync(existingCartItems)
                .ReturnsAsync(cartItems);

            List<CartResponse> cartResponsesActual =
                await _addCartItemsService.MergeCart(userId, items);

            cartResponsesActual.Should().NotBeNull();
            cartResponsesExpected.Should().BeEquivalentTo(cartResponsesActual);
        }

        #endregion
    }
}
