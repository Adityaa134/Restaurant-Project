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
    public class RatingServiceTests
    {
        private readonly IFixture _fixture;

        private readonly Mock<IRatingsRepository> _ratingRepositoryMock;
        private readonly Mock<IOrdersRepository> _orderRepositoryMock;
        private readonly Mock<IDishGetterService> _dishGetterServiceMock;

        private readonly IRatingsService _ratingService;

        public RatingServiceTests()
        {
            _fixture = new Fixture();

            _ratingRepositoryMock = new Mock<IRatingsRepository>();
            _orderRepositoryMock = new Mock<IOrdersRepository>();
            _dishGetterServiceMock = new Mock<IDishGetterService>();

            _ratingService = new RatingsService(_ratingRepositoryMock.Object,_orderRepositoryMock.Object,_dishGetterServiceMock.Object);
        }

        #region AddRating

        [Fact]
        public async Task AddRating_WhenRatingIsGreaterThan5_ShouldThrowArgumentException()
        {
            RatingRequest ratingRequest = _fixture.Build<RatingRequest>()
                                                  .With(t=>t.Rate,10)
                                                  .Create();

            Func<Task> action = async () =>
            {
                await _ratingService.AddRating(ratingRequest);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task AddRating_WhenOrderDoesNotBelongToUser_ShouldThrowArgumentException()
        {
            RatingRequest ratingRequest = _fixture.Build<RatingRequest>()
                                                  .With(t => t.Rate, 4)
                                                  .Create();

            _orderRepositoryMock.Setup(temp=>temp.IsOrderOwnedByUser(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(false);

            Func<Task> action = async () =>
            {
                await _ratingService.AddRating(ratingRequest);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task AddRating_WhenRatingAlreadyExists_ShouldThrowInvalidOperationException()
        {
            RatingRequest ratingRequest = _fixture.Build<RatingRequest>()
                                                  .With(t => t.Rate, 4)
                                                  .Create();

            _orderRepositoryMock.Setup(temp => temp.IsOrderOwnedByUser(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(true);

            _orderRepositoryMock.Setup(temp => temp.IsDishPartOfOrder(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(true);

            _ratingRepositoryMock.Setup(temp => temp.IsRatingGiven(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(true);

            Func<Task> action = async () =>
            {
                await _ratingService.AddRating(ratingRequest);
            };

            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task AddRating_WhenRequestIsValid_ShouldAddRating()
        {
            RatingRequest ratingRequest = _fixture.Build<RatingRequest>()
                                                  .With(t => t.Rate, 5)
                                                  .Create();
            DishResponse dishResponse = _fixture.Build<DishResponse>()
                                                .With(t=>t.DishId,ratingRequest.DishId)
                                                .Create();

            Rating rating = ratingRequest.ToRating();
            RatingResponse ratingResponseExpected = rating.ToRatingResponse();

            _orderRepositoryMock.Setup(temp => temp.IsOrderOwnedByUser(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(true);

            _orderRepositoryMock.Setup(temp => temp.IsDishPartOfOrder(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(true);

            _ratingRepositoryMock.Setup(temp => temp.IsRatingGiven(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(false);

            _ratingRepositoryMock.Setup(temp=>temp.AddRating(It.IsAny<Rating>()))
                                 .ReturnsAsync(rating);

            _dishGetterServiceMock.Setup(temp => temp.ApplyNewRatingToDish(It.IsAny<Rating>()))
                                 .ReturnsAsync(dishResponse);

            RatingResponse ratingResponseActual =  await _ratingService.AddRating(ratingRequest);

            ratingResponseExpected.Id = ratingResponseActual.Id;

            ratingResponseActual.Should().NotBeNull();
            ratingResponseActual.Should().BeEquivalentTo(ratingResponseExpected);
        }

        #endregion

        #region GetRatingsByOrderId

        [Fact]
        public async Task GetRatingsByOrderId_IfOrderIdIsEmpty_ShouldThrowArgumentNullException()
        {
            Guid orderId = Guid.Empty;

            Func<Task> action = async () =>
            {
                await _ratingService.GetRatingsByOrderId(orderId);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GetRatingsByOrderId_IfOrderIdIsInvalid_ShouldReturnNull()
        {
            Guid orderId = Guid.NewGuid();
            List<Rating>? ratings = null;

            _ratingRepositoryMock.Setup(temp => temp.GetRatingsByOrderId(It.IsAny<Guid>()))
                .ReturnsAsync(ratings);

            List<RatingResponse>? ratingResponseActual = await _ratingService.GetRatingsByOrderId(orderId);

            ratingResponseActual.Should().BeNull();
        }

        [Fact]
        public async Task GetRatingsByOrderId_IfOrderIdIsValid_ShouldReturnRatingsGivenByUserInThatOrder()
        {
            Guid orderId = Guid.NewGuid();
            List<Rating> ratings = new List<Rating>()
            {
                _fixture.Build<Rating>()
                        .With(t=>t.Dish,null as Dish)
                        .With(t=>t.Order,null as Order)
                        .With(t=>t.User,null as ApplicationUser)
                        .With(t=>t.OrderId,orderId)
                        .Create(),
                 _fixture.Build<Rating>()
                        .With(t=>t.Dish,null as Dish)
                        .With(t=>t.Order,null as Order)
                        .With(t=>t.User,null as ApplicationUser)
                        .With(t=>t.OrderId,orderId)
                        .Create(),
                  _fixture.Build<Rating>()
                        .With(t=>t.Dish,null as Dish)
                        .With(t=>t.Order,null as Order)
                        .With(t=>t.User,null as ApplicationUser)
                        .With(t=>t.OrderId,orderId)
                        .Create()
            };

            List<RatingResponse> ratingResponsesExpected = ratings.Select(t => t.ToRatingResponse()).ToList();

            _ratingRepositoryMock.Setup(temp=>temp.GetRatingsByOrderId(It.IsAny<Guid>()))
                .ReturnsAsync(ratings);

            List<RatingResponse>? actualRatingResponse =  await _ratingService.GetRatingsByOrderId(orderId);

            actualRatingResponse.Should().NotBeNull();
            actualRatingResponse.Should().BeEquivalentTo(ratingResponsesExpected);
        }

        #endregion
    }
}
