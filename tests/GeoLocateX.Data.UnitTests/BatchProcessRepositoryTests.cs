using Microsoft.EntityFrameworkCore;
using Moq;
using GeoLocateX.Data.Entities;

namespace GeoLocateX.Data.UnitTests;

public class BatchProcessRepositoryTests
{
    private readonly Mock<ApplicationContext> _mockContext;
    private readonly IBatchProcessRepository _repository;

    public BatchProcessRepositoryTests()
    {
        _mockContext = new Mock<ApplicationContext>(new DbContextOptions<ApplicationContext>());
        _repository = new BatchProcessRepository(_mockContext.Object);
    }

    [Fact]
    public async Task AddBatchProcessAsync_ShouldAddBatchProcess()
    {
        var batchProcess = new BatchProcess();

        var dbSet = new Mock<DbSet<BatchProcess>>();
        _mockContext
            .Setup(c => c.BatchProcesses)
            .Returns(dbSet.Object);

        await _repository.AddBatchProcessAsync(batchProcess, CancellationToken.None);

        dbSet.Verify(d => d.AddAsync(batchProcess, It.IsAny<CancellationToken>()), Times.Once());
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
    }
}


