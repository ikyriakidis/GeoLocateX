using FluentAssertions;
using GeoLocateX.Data.Entities;

namespace GeoLocateX.Services.UnitTests;

public sealed class QueueServiceTests
{
    [Fact]
    public void Enqueue_ShouldAddItemToQueue()
    {
        // Arrange
        var queueService = new QueueService();
        var item = new BatchProcessItem { Id = Guid.NewGuid(), IpAddress = "192.168.1.1" };

        // Act
        queueService.Enqueue(item);

        // Assert
        queueService.TryDequeue(out var dequeuedItem).Should().BeTrue();
        dequeuedItem.Should().BeEquivalentTo(item); // Ensure the dequeued item matches the enqueued item
    }

    [Fact]
    public void TryDequeue_ShouldReturnFalseIfQueueIsEmpty()
    {
        // Arrange
        var queueService = new QueueService();

        // Act
        var result = queueService.TryDequeue(out _);

        // Assert
        result.Should().BeFalse();
    }
}
