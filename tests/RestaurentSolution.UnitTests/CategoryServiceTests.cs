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
    public class CategoryServiceTests
    {
        private readonly IFixture _fixture;
        private readonly ICategoriesGetterService _categoriesGetterService;
        private readonly ICategoryAdderService _categoryAdderService;
        private readonly ICategoryUpdateService _categoryUpdateService;

        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly Mock<IImageAdderService> _imageAdderServiceMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        public CategoryServiceTests() 
        {
            _fixture = new Fixture();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _imageAdderServiceMock = new Mock<IImageAdderService>();
            _cacheServiceMock = new Mock<ICacheService>();

            _categoriesGetterService = new CategoriesGetterService(_categoryRepositoryMock.Object,_cacheServiceMock.Object);
            _categoryAdderService = new CategoryAdderService(_categoryRepositoryMock.Object,_imageAdderServiceMock.Object,_cacheServiceMock.Object);
            _categoryUpdateService = new CategoryUpdateService(_categoryRepositoryMock.Object,_cacheServiceMock.Object);
        }


        #region GetCategory

        [Fact]
        public async Task GetAllCategories_EmptyList_ShouldBeEmpty()
        {
            List<Category> categories = new List<Category>();
            _categoryRepositoryMock.Setup(temp=>temp.GetAllCategories())
                .ReturnsAsync(categories);

            List<CategoryResponse> categoryResponses = await _categoriesGetterService.GetAllCategories();

            categoryResponses.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllCategories_ProperList_ShouldReturnCategories()
        {
            List<Category> categories = new List<Category>()
            {
                _fixture.Build<Category>()
                .With(t=>t.Dishes,null as List<Dish>)
                .Create(),
                _fixture.Build<Category>()
                .With(t=>t.Dishes,null as List<Dish>)
                .Create(),
                _fixture.Build<Category>()
                .With(t=>t.Dishes,null as List<Dish>)
                .Create(),
                _fixture.Build<Category>()
                .With(t=>t.Dishes,null as List<Dish>)
                .Create()
            };

            List<CategoryResponse> categoryResponsesExpected = categories.Select(temp=>temp.ToCategoryResponse()).ToList();

            _categoryRepositoryMock.Setup(temp => temp.GetAllCategories())
                .ReturnsAsync(categories);

            List<CategoryResponse> categoryResponsesActual = await _categoriesGetterService.GetAllCategories();

            categoryResponsesActual.Should().NotBeEmpty();
            categoryResponsesActual.Should().BeEquivalentTo(categoryResponsesExpected);
        }
        
        [Fact]
        public async Task GetAllCategories_CacheHit_ShouldReturnCategories()
        {
            List<Category> categories = new List<Category>()
            {
                _fixture.Build<Category>()
                .With(t => t.Dishes, null as List<Dish>)
                .Create(),
                _fixture.Build<Category>()
                .With(t => t.Dishes, null as List<Dish>)
                .Create(),
                _fixture.Build<Category>()
                .With(t => t.Dishes, null as List<Dish>)
                .Create(),
                _fixture.Build<Category>()
                .With(t => t.Dishes, null as List<Dish>)
                .Create()
            };

            List<CategoryResponse> categoryResponsesExpected = categories.Select(temp => temp.ToCategoryResponse()).ToList();

            _cacheServiceMock.Setup(temp => temp.GetAsync<List<CategoryResponse>>("categories:all"))
                .ReturnsAsync(categoryResponsesExpected);

            List<CategoryResponse> categoryResponsesActual = await _categoriesGetterService.GetAllCategories();

            categoryResponsesActual.Should().NotBeEmpty();
            categoryResponsesActual.Should().BeEquivalentTo(categoryResponsesExpected);
            _categoryRepositoryMock.Verify(temp => temp.GetAllCategories(), Times.Never);
        }

        [Fact]
        public async Task GetAllCategories_CacheMiss_ShouldCallRepositoryAndSetCache()
        {
            List<Category> categories = new List<Category>()
            {
                _fixture.Build<Category>()
                .With(t => t.Dishes, null as List<Dish>)
                .Create(),
                _fixture.Build<Category>()
                .With(t => t.Dishes, null as List<Dish>)
                .Create(),
                _fixture.Build<Category>()
                .With(t => t.Dishes, null as List<Dish>)
                .Create(),
                _fixture.Build<Category>()
                .With(t => t.Dishes, null as List<Dish>)
                .Create()
            };

            List<CategoryResponse> categoryResponsesExpected = categories.Select(temp => temp.ToCategoryResponse()).ToList();

            _cacheServiceMock.Setup(temp => temp.GetAsync<List<CategoryResponse>>("categories:all"))
                .ReturnsAsync((List<CategoryResponse>?)null);

            _categoryRepositoryMock.Setup(temp => temp.GetAllCategories())
                .ReturnsAsync(categories);

            List<CategoryResponse> categoryResponsesActual = await _categoriesGetterService.GetAllCategories();

            categoryResponsesActual.Should().NotBeEmpty();
            categoryResponsesActual.Should().BeEquivalentTo(categoryResponsesExpected);
            _categoryRepositoryMock.Verify(temp => temp.GetAllCategories(), Times.Once);
            _cacheServiceMock.Verify(temp => temp.SetAsync("categories:all", It.IsAny<List<CategoryResponse>>(), It.IsAny<TimeSpan?>()), Times.Once);
        }

        [Fact]
        public async Task GetAllCategoriesAdmin_EmptyList_ShouldBeEmpty()
        {
            List<Category> categories = new List<Category>();
            _categoryRepositoryMock.Setup(temp => temp.GetAllCategoriesAdmin())
                .ReturnsAsync(categories);

            List<CategoryResponse> categoryResponses = await _categoriesGetterService.GetAllCategoriesAdmin();

            categoryResponses.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllCategoriesAdmin_ProperList_ShouldReturnCategories()
        {
            List<Category> categories = new List<Category>()
            {
                _fixture.Build<Category>()
                .With(t=>t.Dishes,null as List<Dish>)
                .Create(),
                _fixture.Build<Category>()
                .With(t=>t.Dishes,null as List<Dish>)
                .Create(),
                _fixture.Build<Category>()
                .With(t=>t.Dishes,null as List<Dish>)
                .Create(),
                _fixture.Build<Category>()
                .With(t=>t.Dishes,null as List<Dish>)
                .Create()
            };

            List<CategoryResponse> categoryResponsesExpected = categories.Select(temp => temp.ToCategoryResponse()).ToList();

            _categoryRepositoryMock.Setup(temp => temp.GetAllCategoriesAdmin())
                .ReturnsAsync(categories);

            List<CategoryResponse> categoryResponsesActual = await _categoriesGetterService.GetAllCategoriesAdmin();

            categoryResponsesActual.Should().NotBeEmpty();
            categoryResponsesActual.Should().BeEquivalentTo(categoryResponsesExpected);
        }

        [Fact]
        public async Task GetCategoryById_NullId_ArgumentNullException()
        {
            Guid? categoryId = null;

            Func<Task> action = async () =>
            {
                await _categoriesGetterService.GetCategoryByCategoryId(categoryId);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GetCategoryById_VaildId_ShouldReturnMatchingCategory()
        {
            Category category = _fixture.Build<Category>()
                .With(t=>t.Dishes,null as List<Dish>)
                .Create();
            CategoryResponse categoryResponseExpected = category.ToCategoryResponse();
            _categoryRepositoryMock.Setup(temp => temp.GetCategoryByCategoryId(It.IsAny<Guid>()))
                .ReturnsAsync(category);

            CategoryResponse? categoryResponseActual = await _categoriesGetterService.GetCategoryByCategoryId(category.Id);

            categoryResponseActual.Should().NotBeNull();
            categoryResponseActual.Should().BeEquivalentTo(categoryResponseExpected);
        }

        [Fact]
        public async Task GetCategoryById_InVaildId_ShouldBeNull()
        {
            _categoryRepositoryMock.Setup(temp => temp.GetCategoryByCategoryId(It.IsAny<Guid>()))
                .ReturnsAsync((Category?)null);

            CategoryResponse? categoryResponseActual = await _categoriesGetterService.GetCategoryByCategoryId(Guid.NewGuid());

            categoryResponseActual.Should().BeNull();
        }

        [Fact]
        public async Task GetCategoryById_CacheHit_ShouldReturnMatchingCategory()
        {
            Category category = _fixture.Build<Category>()
                .With(t => t.Dishes, null as List<Dish>)
                .Create();

            CategoryResponse categoryResponseExpected = category.ToCategoryResponse();

            _cacheServiceMock.Setup(temp => temp.GetAsync<CategoryResponse>($"category:{category.Id}"))
                .ReturnsAsync(categoryResponseExpected);

            CategoryResponse? categoryResponseActual = await _categoriesGetterService.GetCategoryByCategoryId(category.Id);

            categoryResponseActual.Should().NotBeNull();
            categoryResponseActual.Should().BeEquivalentTo(categoryResponseExpected);
            _categoryRepositoryMock.Verify(temp => temp.GetCategoryByCategoryId(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task IsCategoryExist_ValidDishId_ShouldReturnTrue()
        {
            Category category = _fixture.Build<Category>()
                .With(t => t.Dishes, null as List<Dish>)
                .Create();

            _categoryRepositoryMock.Setup(temp => temp.IsCategoryExist(It.IsAny<Guid>()))
               .ReturnsAsync(true);

            bool isExist = await _categoriesGetterService.IsCategoryExist(category.Id);

            isExist.Should().BeTrue();
        }

        [Fact]
        public async Task IsCategoryExist_InValidDishId_ShouldReturnFalse()
        {
            _categoryRepositoryMock.Setup(temp => temp.IsCategoryExist(It.IsAny<Guid>()))
               .ReturnsAsync(false);

            bool isExist = await _categoriesGetterService.IsCategoryExist(Guid.NewGuid());

            isExist.Should().BeFalse();
        }

        #endregion

        #region AddCategory

        [Fact] 
        public async Task AddCategory_NullCategoryName_ThrowArgumentException()
        {
            var mockImageFile = _fixture.Create<Mock<IFormFile>>();
            CategoryAddRequest categoryAddRequest = _fixture.Build<CategoryAddRequest>()
                                .With(t=>t.Cat_Image,mockImageFile.Object)
                                .With(t => t.CategoryName, null as string)
                                .Create();

            Func<Task> action = async () =>
            {
                await _categoryAdderService.AddCategory(categoryAddRequest);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task AddCategory_ValidCategoryDetails_ShouldBeSuccessfull()
        {
            var mockImageFile = _fixture.Create<Mock<IFormFile>>();
            mockImageFile.Setup(f => f.FileName).Returns("test.jpg");
            mockImageFile.Setup(f => f.Length).Returns(1024);

            CategoryAddRequest categoryAddRequest = _fixture.Build<CategoryAddRequest>()
                                .With(t => t.Cat_Image, mockImageFile.Object)
                                .Create();

            Category category = categoryAddRequest.ToCategory();
            CategoryResponse categoryResponseExpected = category.ToCategoryResponse();
            _categoryRepositoryMock.Setup(temp=>temp.AddCategory(It.IsAny<Category>()))
                .ReturnsAsync(category);

            _imageAdderServiceMock.Setup(temp => temp.ImageAdder(It.IsAny<IFormFile>()))
                .ReturnsAsync("dummy_path.jpg");

            CategoryResponse categoryResponseActual =  await _categoryAdderService.AddCategory(categoryAddRequest);

            categoryResponseExpected.CategoryId = categoryResponseActual.CategoryId;
            categoryResponseActual.Cat_Image = categoryResponseExpected.Cat_Image;

            categoryResponseActual.Should().NotBeNull();
            categoryResponseActual.Should().BeEquivalentTo(categoryResponseExpected);
        }

        [Fact]
        public async Task AddCategory_ValidCategoryDetails_ShouldInvalidateCategoriesCache()
        {
            var mockImageFile = _fixture.Create<Mock<IFormFile>>();
            mockImageFile.Setup(f => f.FileName).Returns("test.jpg");
            mockImageFile.Setup(f => f.Length).Returns(1024);

            CategoryAddRequest categoryAddRequest = _fixture.Build<CategoryAddRequest>()
                .With(t => t.Cat_Image, mockImageFile.Object)
                .Create();

            Category category = categoryAddRequest.ToCategory();
            CategoryResponse categoryResponseExpected = category.ToCategoryResponse();

            _categoryRepositoryMock.Setup(temp => temp.AddCategory(It.IsAny<Category>()))
                .ReturnsAsync(category);

            _imageAdderServiceMock.Setup(temp => temp.ImageAdder(It.IsAny<IFormFile>()))
                .ReturnsAsync("dummy_path.jpg");

            CategoryResponse categoryResponseActual = await _categoryAdderService.AddCategory(categoryAddRequest);

            categoryResponseActual.Should().NotBeNull();
            _cacheServiceMock.Verify(temp => temp.RemoveAsync("categories:all"), Times.Once);
        }

        #endregion

        #region UpdateCategoryStatus

        [Fact]
        public async Task UpdateCategoryStatus_InvalidCategoryId_ArgumentException()
        {
            CategoryStatusUpdateRequest request =  _fixture.Build<CategoryStatusUpdateRequest>().Create();
            Category? category = null;
            _categoryRepositoryMock.Setup(temp=>temp.GetCategoryByCategoryId(It.IsAny<Guid>()))
                .ReturnsAsync(category);

            Func<Task> action = async()=>
            {
                await _categoryUpdateService.UpdateCategoryStatus(request);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdateCategoryStatus_ProperDetails_ShouldReturnUpdatedCategoryDetails()
        {
            CategoryStatusUpdateRequest request = _fixture.Build<CategoryStatusUpdateRequest>().Create();
            Category category = _fixture.Build<Category>()
                .With(t => t.Dishes, null as List<Dish>)
                .Create();
            CategoryResponse categoryResponseExpected = category.ToCategoryResponse();
            _categoryRepositoryMock.Setup(temp => temp.GetCategoryByCategoryId(It.IsAny<Guid>()))
                .ReturnsAsync(category);

            _categoryRepositoryMock.Setup(temp=>temp.UpdateCategoryStatus(It.IsAny<bool>(),It.IsAny<Guid>()))
                .ReturnsAsync(category);

            CategoryResponse? categoryResponseActual =  await _categoryUpdateService.UpdateCategoryStatus(request);

            categoryResponseActual.Should().NotBeNull();
            categoryResponseActual.Should().BeEquivalentTo(categoryResponseExpected);
        }

        [Fact]
        public async Task UpdateCategoryStatus_ProperDetails_ShouldInvalidateBothIndividualAndListCache()
        {
            CategoryStatusUpdateRequest request = _fixture.Build<CategoryStatusUpdateRequest>().Create();
            Category category = _fixture.Build<Category>()
                .With(t => t.Dishes, null as List<Dish>)
                .Create();

            CategoryResponse categoryResponseExpected = category.ToCategoryResponse();

            _categoryRepositoryMock.Setup(temp => temp.GetCategoryByCategoryId(It.IsAny<Guid>()))
                .ReturnsAsync(category);

            _categoryRepositoryMock.Setup(temp => temp.UpdateCategoryStatus(It.IsAny<bool>(), It.IsAny<Guid>()))
                .ReturnsAsync(category);

            CategoryResponse? categoryResponseActual = await _categoryUpdateService.UpdateCategoryStatus(request);

            categoryResponseActual.Should().NotBeNull();
            categoryResponseActual.Should().BeEquivalentTo(categoryResponseExpected);
            _cacheServiceMock.Verify(temp => temp.RemoveAsync($"category:{request.CategoryId}"), Times.Once);
            _cacheServiceMock.Verify(temp => temp.RemoveAsync("categories:all"), Times.Once);
        }

        #endregion
    }
}
