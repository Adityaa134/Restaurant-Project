using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Core.DTO;
using Restaurent.Core.Service;
using Restaurent.Core.ServiceContracts;

namespace RestaurentSolution.UnitTests
{
    public class DishServiceTests
    {
        private readonly IFixture _fixture;
        private readonly IDishGetterService _dishGetterService;
        private readonly IDishAdderService _dishAdderService;
        private readonly IDishUpdateService _dishUpdateService;
        private readonly IDishDeleteService _dishDeleteService;

        private readonly Mock<IDishRepository> _dishRepositoryMock;
        private readonly Mock<IImageAdderService> _imageAdderServiceMock;
        private readonly Mock<IImageUpdateService> _imageUpdateServiceMock;
        private readonly Mock<IImageDeleteService> _imageDeleteServiceMock;
        private readonly Mock<ICategoriesGetterService> _categoriesGetterServiceMock;

        public DishServiceTests()
        {
            _fixture = new Fixture();
            _dishRepositoryMock = new Mock<IDishRepository>();
            _imageAdderServiceMock = new Mock<IImageAdderService>();
            _imageUpdateServiceMock = new Mock<IImageUpdateService>();
            _imageDeleteServiceMock = new Mock<IImageDeleteService>();
            _categoriesGetterServiceMock = new Mock<ICategoriesGetterService>();

            _dishGetterService = new DishGetterService(_dishRepositoryMock.Object,_categoriesGetterServiceMock.Object);
            _dishAdderService = new DishAdderService(_dishRepositoryMock.Object, _imageAdderServiceMock.Object);
            _dishUpdateService = new DishUpdateService(_dishRepositoryMock.Object,_imageUpdateServiceMock.Object);
            _dishDeleteService = new DishDeleteService(_dishRepositoryMock.Object, _imageDeleteServiceMock.Object);
        }


        #region GetDish

        [Fact]
        public async Task GetAllDishes_EmptyList_ShouldBeNull()
        {
            List<Dish> dishes = new List<Dish>();
            _dishRepositoryMock.Setup(temp => temp.GetAllDishes())
                .ReturnsAsync(dishes);

            List<DishResponse> dishResponses = await _dishGetterService.GetAllDishes();

            dishResponses.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllDishes_ProperList_ShouldReturnAllDishes()
        {
            List<Dish> dishes = new List<Dish>()
            {
                _fixture.Build<Dish>()
                .With(t=>t.Category,null as Category)
                .With(t=>t.CartItems,null as List<Carts>)
                .With(t=>t.OrderItems,null as List<OrderItem>)
                .Create(),
                _fixture.Build<Dish>()
                .With(t=>t.Category,null as Category)
                .With(t=>t.CartItems,null as List<Carts>)
                .With(t=>t.OrderItems,null as List<OrderItem>)
                .Create(),
                _fixture.Build<Dish>()
                .With(t=>t.Category,null as Category)
                .With(t=>t.CartItems,null as List<Carts>)
                .With(t=>t.OrderItems,null as List<OrderItem>)
                .Create(),
                _fixture.Build<Dish>()
                .With(t=>t.Category,null as Category)
                .With(t=>t.CartItems,null as List<Carts>)
                .With(t=>t.OrderItems,null as List<OrderItem>)
                .Create()
            };

            List<DishResponse> dishesResponsesExpected = dishes.Select(temp => temp.ToDishResponse()).ToList();

            _dishRepositoryMock.Setup(temp => temp.GetAllDishes())
                .ReturnsAsync(dishes);

            List<DishResponse> dishResponsesActual = await _dishGetterService.GetAllDishes();

            dishResponsesActual.Should().NotBeEmpty();
            dishResponsesActual.Should().BeEquivalentTo(dishesResponsesExpected);
        }

        [Fact]
        public async Task SearchDish_IfSearchStringNotFound_ShouldBeNull()
        {
            string? searchString = "pizza";
            List<Dish> dishes = new List<Dish>();

            _dishRepositoryMock.Setup(temp => temp.SearchDish(It.IsAny<string>()))
                .ReturnsAsync(dishes);

            List<DishResponse>? dishResponses = await _dishGetterService.SearchDish(searchString);

            dishResponses.Should().BeEmpty();
        }

        [Fact]
        public async Task SearchDish_IfSearchStringFound_ShouldReturnMatchingDishes()
        {
            string searchString = "pizza";
            List<Dish> dishes = new List<Dish>()
            {
                _fixture.Build<Dish>()
                .With(t => t.Category, null as Category)
                .With(t => t.CartItems, null as List<Carts>)
                .With(t => t.OrderItems, null as List<OrderItem>)
                .With(t=>t.DishName,"cheese pizza")
                .Create(),
                _fixture.Build<Dish>()
                .With(t => t.Category, null as Category)
                .With(t => t.CartItems, null as List<Carts>)
                .With(t => t.OrderItems, null as List<OrderItem>)
                .With(t=>t.DishName,"onion pizza")
                .Create(),
                _fixture.Build<Dish>()
                .With(t => t.Category, null as Category)
                .With(t => t.CartItems, null as List<Carts>)
                .With(t => t.OrderItems, null as List<OrderItem>)
                .With(t=>t.DishName,"corn pizza")
                .Create()
            };

            List<DishResponse> dishResponsesExpected = dishes.Select(temp => temp.ToDishResponse()).ToList();

            _dishRepositoryMock.Setup(temp => temp.SearchDish(It.IsAny<string>()))
                .ReturnsAsync(dishes);

            List<DishResponse>? dishResponsesActual = await _dishGetterService.SearchDish(searchString);

            dishResponsesActual.Should().NotBeEmpty();
            dishResponsesActual.Should().BeEquivalentTo(dishResponsesExpected);
        }

        [Fact]
        public async Task GetDishByDishId_NullId_ArgumentNullException()
        {
            Guid? dishId = null;

            Func<Task> action = async () =>
            {
                await _dishGetterService.GetDishByDishId(dishId);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GetDishByDishId_VaildDishId_ShouldReturnMatchingDish()
        {
            Dish dish = _fixture.Build<Dish>()
                .With(t => t.Category, null as Category)
                .With(t => t.CartItems, null as List<Carts>)
                .With(t => t.OrderItems, null as List<OrderItem>)
                .Create();

            DishResponse dishResponseExpected = dish.ToDishResponse();

            _dishRepositoryMock.Setup(temp => temp.GetDishByDishId(It.IsAny<Guid>()))
                .ReturnsAsync(dish);

            DishResponse? dishResponseActual = await _dishGetterService.GetDishByDishId(dish.DishId);

            dishResponseActual.Should().NotBeNull();
            dishResponseActual.Should().BeEquivalentTo(dishResponseExpected);
        }

        [Fact]
        public async Task GetDishByDishId_InVaildDishId_ShouldBeNull()
        {
            _dishRepositoryMock.Setup(temp => temp.GetDishByDishId(It.IsAny<Guid>()))
                .ReturnsAsync((Dish?)null);

            DishResponse? dishResponseActual = await _dishGetterService.GetDishByDishId(Guid.NewGuid());

            dishResponseActual.Should().BeNull();
        }

        [Fact]
        public async Task GetDishesBasedOnCategoryId_InvalidCategoryId_ShouldBeNull()
        {
            CategoryResponse? categoryResponse = null;

            _categoriesGetterServiceMock.Setup(temp => temp.GetCategoryByCategoryId(It.IsAny<Guid>()))
                .ReturnsAsync(categoryResponse);

            List<DishResponse>? dishesResponseActual = await _dishGetterService.GetDishesBasedOnCategoryId(Guid.NewGuid());

            dishesResponseActual.Should().BeNull();
        }

        [Fact]
        public async Task GetDishesBasedOnCategoryId_ValidCategoryIdWithNoDishes_ShouldBeNull()
        {
            Category category = _fixture.Build<Category>()
                .With(t => t.Dishes, null as List<Dish>)
                .Create();

            CategoryResponse categoryResponse = category.ToCategoryResponse();

            List<Dish> dishes = new List<Dish>();

            _categoriesGetterServiceMock.Setup(temp => temp.GetCategoryByCategoryId(It.IsAny<Guid>()))
                .ReturnsAsync(categoryResponse);

            _dishRepositoryMock.Setup(temp => temp.GetDishesBasedOnCategoryId(It.IsAny<Guid>()))
                .ReturnsAsync(dishes);

            List<DishResponse>? dishesResponseActual = await _dishGetterService.GetDishesBasedOnCategoryId(category.Id);

            dishesResponseActual.Should().BeEmpty();
        }

        [Fact]
        public async Task GetDishesBasedOnCategoryId_ValidCategoryIdWithDishes_ShouldReturnDishes()
        {
            Category category = _fixture.Build<Category>()
                .With(t => t.Dishes, null as List<Dish>)
                .Create();

            CategoryResponse categoryResponse = category.ToCategoryResponse();

            List<Dish> dishes = new List<Dish>()
            {
                _fixture.Build<Dish>()
                .With(t=>t.Category,null as Category)
                .With(t=>t.CartItems,null as List<Carts>)
                .With(t=>t.OrderItems,null as List<OrderItem>)
                .With(t=>t.CategoryId,category.Id)
                .Create(),
                _fixture.Build<Dish>()
                .With(t=>t.Category,null as Category)
                .With(t=>t.CartItems,null as List<Carts>)
                .With(t=>t.OrderItems,null as List<OrderItem>)
                .With(t=>t.CategoryId,category.Id)
                .Create(),
                _fixture.Build<Dish>()
                .With(t=>t.Category,null as Category)
                .With(t=>t.CartItems,null as List<Carts>)
                .With(t=>t.OrderItems,null as List<OrderItem>)
                .With(t=>t.CategoryId,category.Id)
                .Create(),
                _fixture.Build<Dish>()
                .With(t=>t.Category,null as Category)
                .With(t=>t.CartItems,null as List<Carts>)
                .With(t=>t.OrderItems,null as List<OrderItem>)
                .With(t=>t.CategoryId,category.Id)
                .Create()
            };

            List<DishResponse> dishResponsesExpected_list = dishes.Select(temp => temp.ToDishResponse())
                .ToList();

            _categoriesGetterServiceMock.Setup(temp => temp.GetCategoryByCategoryId(It.IsAny<Guid>()))
                .ReturnsAsync(categoryResponse);

            _dishRepositoryMock.Setup(temp => temp.GetDishesBasedOnCategoryId(It.IsAny<Guid>()))
                .ReturnsAsync(dishes);

            List<DishResponse>? dishResponseActual_list = await _dishGetterService.GetDishesBasedOnCategoryId(category.Id);

            dishResponseActual_list.Should().NotBeNull();
            dishResponseActual_list.Should().BeEquivalentTo(dishResponsesExpected_list);
        }

        #endregion

        #region AddDish

          [Fact]
          public async Task AddDish_NullDishName_ThrowArgumentException()
          {
              var mockImageFile = _fixture.Create<Mock<IFormFile>>();
              DishAddRequest dishAddRequest = _fixture.Build<DishAddRequest>()
                                  .With(t => t.Dish_Image, mockImageFile.Object)
                                  .With(t => t.DishName, null as string)
                                  .Create();

              Func<Task> action = async () =>
              {
                  await _dishAdderService.AddDish(dishAddRequest);
              };

              await action.Should().ThrowAsync<ArgumentException>();
          }

          [Fact]
          public async Task AddDish_ValidDishDetails_ShouldBeSuccessfull()
          {
              var mockImageFile = _fixture.Create<Mock<IFormFile>>();
              mockImageFile.Setup(f => f.FileName).Returns("test.jpg");
              mockImageFile.Setup(f => f.Length).Returns(1024);

              DishAddRequest dishAddRequest = _fixture.Build<DishAddRequest>()
                                  .With(t => t.Dish_Image, mockImageFile.Object)
                                  .With(t=>t.CategoryId,Guid.NewGuid().ToString())
                                  .Create();

              Dish dish = dishAddRequest.ToDish();
              DishResponse dishResponseExpected = dish.ToDishResponse();
              _dishRepositoryMock.Setup(temp => temp.AddDish(It.IsAny<Dish>()))
                  .ReturnsAsync(dish);

              _imageAdderServiceMock.Setup(temp => temp.ImageAdder(It.IsAny<IFormFile>(), It.IsAny<string>()))
                  .ReturnsAsync("dummy_path.jpg");

              DishResponse dishResponseActual = await _dishAdderService.AddDish(dishAddRequest);

              dishResponseExpected.DishId = dishResponseActual.DishId;
              dishResponseActual.Dish_Image_Path = dishResponseExpected.Dish_Image_Path;

              dishResponseActual.Should().NotBeNull();
              dishResponseActual.Should().BeEquivalentTo(dishResponseExpected);
          }

          #endregion

        #region UpdateDish

          [Fact]
          public async Task UpdateDish_InvalidDishId_ArgumentException()
          {
              var mockImageFile = _fixture.Create<Mock<IFormFile>>();
              mockImageFile.Setup(f => f.FileName).Returns("test.jpg");
              mockImageFile.Setup(f => f.Length).Returns(1024);
              DishUpdateRequest dishUpdateRequest = _fixture.Build<DishUpdateRequest>()
                                    .With(t => t.Dish_Image, mockImageFile.Object)
                                    .Create();

              _dishRepositoryMock.Setup(temp => temp.GetDishByDishId(It.IsAny<Guid>()))
                  .ReturnsAsync((Dish?)null);

              Func<Task> action = async () =>
              {
                  await _dishUpdateService.UpdateDish(dishUpdateRequest);
              };

              await action.Should().ThrowAsync<ArgumentException>();
          }

          [Fact]
          public async Task UpdateDish_ProperDetails_ShouldReturnUpdatedDishDetails()
          {

            Dish dish = _fixture.Build<Dish>()
                   .With(t => t.Category, null as Category)
                   .With(t => t.CartItems, null as List<Carts>)
                   .With(t => t.OrderItems, null as List<OrderItem>)
                   .Create();

            var mockImageFile = _fixture.Create<Mock<IFormFile>>();
            mockImageFile.Setup(f => f.FileName).Returns("test.jpg");
            mockImageFile.Setup(f => f.Length).Returns(1024);
            DishUpdateRequest dishUpdateRequest = _fixture.Build<DishUpdateRequest>()
                                  .With(t => t.Dish_Image, mockImageFile.Object)
                                  .With(t=>t.DishId,dish.DishId)
                                  .With(t => t.DishName, dish.DishName)
                                  .With(t => t.Description, dish.Description)
                                  .With(t=>t.Price,(int)dish.Price)
                                  .With(t=>t.Image_Path,dish.Image_Path)
                                  .With(t=>t.CategoryId,dish.CategoryId)
                                  .Create();

            DishResponse dishResponseExpected = dish.ToDishResponse();

              _dishRepositoryMock.Setup(temp => temp.GetDishByDishId(It.IsAny<Guid>()))
                  .ReturnsAsync(dish);

              _dishRepositoryMock.Setup(temp => temp.UpdateDish(It.IsAny<Dish>()))
                  .ReturnsAsync(dish);

              _imageUpdateServiceMock.Setup(temp => temp.ImageUpdater(It.IsAny<IFormFile>(), It.IsAny<string>(),It.IsAny<string>()))
                  .ReturnsAsync("dummy_path.jpg");

              DishResponse? dishResponseActual = await _dishUpdateService.UpdateDish(dishUpdateRequest);
              dishResponseActual.Dish_Image_Path = dishResponseExpected.Dish_Image_Path;

              dishResponseActual.Should().NotBeNull();
              dishResponseActual.Should().BeEquivalentTo(dishResponseExpected);
          }

        #endregion

        #region DeleteDish

        [Fact]
        public async Task DeleteDish_InvalidDishId_ShouldBeFalse()
        {
            bool isDeleted = await _dishDeleteService.DeleteDish(Guid.NewGuid());

            isDeleted.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteDish_ValidDishId_ShouldBeTrue()
        {
            Dish dish = _fixture.Build<Dish>()
                .With(t => t.Category, null as Category)
                .With(t => t.CartItems, null as List<Carts>)
                .With(t => t.OrderItems, null as List<OrderItem>)
                .Create();

            _dishRepositoryMock.Setup(temp => temp.GetDishByDishId(It.IsAny<Guid>()))
                .ReturnsAsync(dish);

            _imageDeleteServiceMock.Setup(temp => temp.ImageDeleter(It.IsAny<string>()))
                .ReturnsAsync(true);

            _dishRepositoryMock.Setup(temp => temp.DeleteDishByDishId(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            bool isDeleted = await _dishDeleteService.DeleteDish(dish.DishId);

            isDeleted.Should().BeTrue();
        }
        #endregion

    }
}
