using GeoLocateX.Api.Attributes;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;

namespace GeoLocateX.Api.UnitTests.Attributes;

public class IpAddressListAttributeTests
{
    private readonly IpAddressListAttribute _attribute;
    private readonly ValidationContext _validationContext;

    public IpAddressListAttributeTests()
    {
        _attribute = new IpAddressListAttribute();
        _validationContext = new ValidationContext(new object());
    }

    [Fact]
    public void IsValid_WithValidIpAddresses_ShouldReturnSuccess()
    {
        // Arrange
        var ipAddresses = new List<string> { "192.168.1.1", "10.0.0.1" };

        // Act
        var result = _attribute.GetValidationResult(ipAddresses, _validationContext);

        // Assert
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_WithEmptyList_ShouldReturnSuccess()
    {
        // Arrange
        var ipAddresses = new List<string>();

        // Act
        var result = _attribute.GetValidationResult(ipAddresses, _validationContext);

        // Assert
        result.Should().Be(ValidationResult.Success);
    }
}
