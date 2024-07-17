using GeoLocateX.Data;
using GeoLocateX.Data.Entities;
using GeoLocateX.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace GeoLocateX.Services.UnitTests;

public class GeoIpBackgroundServiceTests
{
    private readonly Mock<IServiceScopeFactory> _mockScopeFactory;
    private readonly Mock<IServiceScope> _mockScope;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IBatchProcessRepository> _mockRepository;
    private readonly Mock<IQueueService> _mockQueueService;

    public GeoIpBackgroundServiceTests()
    {
        _mockScopeFactory = new Mock<IServiceScopeFactory>();
        _mockScope = new Mock<IServiceScope>();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockRepository = new Mock<IBatchProcessRepository>();
        _mockQueueService = new Mock<IQueueService>();
    }

    [Fact]
    public async Task LoadUnfinishedBatchProcessItems_EnqueuesItems()
    {
        // Arrange
        var batchProcessItems = new List<BatchProcessItem>
        {
            new BatchProcessItem { Id = Guid.NewGuid(), IpAddress = "127.0.0.1", Status = BatchProcessItemStatus.Queued },
            new BatchProcessItem { Id = Guid.NewGuid(), IpAddress = "192.168.0.1", Status = BatchProcessItemStatus.Queued }
        };

        var batches = new List<BatchProcess>
        {
            new BatchProcess { Id = Guid.NewGuid(), Status = BatchProcessStatus.Queued, BatchProcessItems = batchProcessItems }
        };

        _mockRepository.Setup(repo => repo.GetBatchProcessByStatusAsync(BatchProcessStatus.Queued, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(batches);

        _mockScopeFactory.Setup(factory => factory.CreateScope()).Returns(_mockScope.Object);
        _mockScope.Setup(scope => scope.ServiceProvider).Returns(_mockServiceProvider.Object);
        _mockServiceProvider.Setup(provider => provider.GetService(typeof(IBatchProcessRepository)))
                            .Returns(_mockRepository.Object);

        var service = new GeoIpBackgroundService(
            _mockScopeFactory.Object,
            Mock.Of<ILogger<GeoIpBackgroundService>>(),
            _mockQueueService.Object);

        var cancellationToken = new CancellationToken();

        // Act
        await service.LoadUnfinishedBatchProcessItems(cancellationToken);

        // Assert
        foreach (var item in batchProcessItems)
        {
            _mockQueueService.Verify(q => q.Enqueue(It.Is<BatchProcessItem>(b => b.Id == item.Id && b.IpAddress == item.IpAddress)), Times.Once);
        }
    }
}
