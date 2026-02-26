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

            List<AddToCartResponse> cartResponses = await _getCartItemsService.GetAllCartItems(userId);

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

            List<AddToCartResponse> cartsResponsesExpected = carts.Select(temp => temp.ToAddToCartResponse()).ToList();

            _cartsRepositoryMock.Setup(temp => temp.GetAllCartItemsWithUserId(It.IsAny<Guid>()))
                .ReturnsAsync(carts);

            List<AddToCartResponse> cartResponsesActual = await _getCartItemsService.GetAllCartItems(userId);

            cartResponsesActual.Should().NotBeEmpty();
            cartResponsesActual.Should().BeEquivalentTo(cartsResponsesExpected);
        }

        [Fact]
        public async Task IsCartItemExist_IfDishIdIsNull_ShouldBeFalse()
        {
            Guid dishId = Guid.Empty;
            Guid userId = Guid.NewGuid();

            bool isExist = await _getCartItemsService.IsCartItemExist(userId,dishId);

            isExist.Should().BeFalse();
        }

        [Fact]
        public async Task IsCartItemExist_IfDishIdAndUserIdRecordIsNotFound_ShouldBeFalse()
        {
            Guid userId = Guid.NewGuid();
            Guid dishId = Guid.NewGuid();

            _cartsRepositoryMock.Setup(temp => temp.IsCartItemExist(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);

            bool isExist = await _getCartItemsService.IsCartItemExist(userId,dishId);

            isExist.Should().BeFalse();
        }

        [Fact]
        public async Task IsCartItemExist_IfDishIdAndUserIdRecordIsFound_ShouldBeTrue()
        {
            Guid userId = Guid.NewGuid();
            Guid dishId = Guid.NewGuid();

            _cartsRepositoryMock.Setup(temp=>temp.IsCartItemExist(It.IsAny<Guid>(), It.IsAny<Guid>()))
                   .ReturnsAsync(true);

            bool isExist = await _getCartItemsService.IsCartItemExist(userId, dishId);

            isExist.Should().BeTrue();
        }

        #endregion 

        #region AddItemTocart

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

            AddToCartResponse cartResponseExpected = cart.ToAddToCartResponse();

            _dishGetterServiceMock.Setup(temp=>temp.GetDishByDishId(It.IsAny<Guid>()))
                .ReturnsAsync(dishResponse);

            _cartsRepositoryMock.Setup(temp => temp.AddItemToCart(It.IsAny<Carts>()))
                .ReturnsAsync(cart);

            AddToCartResponse cartResponseActual = await _addCartItemsService.AddItemToCart(request);

            cartResponseExpected.CartId = cartResponseActual.CartId;

            cartResponseActual.Should().NotBeNull();
            cartResponseActual.Should().BeEquivalentTo(cartResponseExpected);
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
                 .Create();
            Carts cart = _fixture.Build<Carts>()
                                       .With(t=>t.Id, request.CartId)
                                       .With(t => t.Users, null as ApplicationUser)
                                       .With(t => t.Dishes, dish)
                                       .Create();
            AddToCartResponse cartResponseExpected = cart.ToAddToCartResponse();

            _cartsRepositoryMock.Setup(temp => temp.GetCartItemByCartId(It.IsAny<Guid>()))
                 .ReturnsAsync(cart);

            _cartsRepositoryMock.Setup(temp => temp.UpdateCartItemQuantity(It.IsAny<Carts>(), It.IsAny<int>()))
                 .ReturnsAsync(cart);

             AddToCartResponse? cartResponseActual = await _updateItemQuantityInCart.UpdateDishQuantityInCartItem(request);

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
    }
}
