using GeoGaurd.API.Models;
using GeoGaurd.API.Repositories;
using GeoGaurd.API.Services;
using Moq;
using Xunit;

namespace GeoGaurd.API.Tests;

public class CountryBlockServiceTests
{
    private readonly Mock<ICountryBlockRepository> _repositoryMock;
    private readonly Mock<ICountryCatalogService> _catalogMock;
    private readonly CountryBlockService _service;

    public CountryBlockServiceTests()
    {
        _repositoryMock = new Mock<ICountryBlockRepository>();
        _catalogMock = new Mock<ICountryCatalogService>();
        _service = new CountryBlockService(_repositoryMock.Object, _catalogMock.Object);
    }

    [Fact]
    public void AddPermanentBlock_ShouldAddSuccessfully_WhenCountryNotBlocked()
    {
        // Arrange
        var countryCode = "EG";
        var countryName = "Egypt";
        _catalogMock.Setup(c => c.TryGetCountryName(countryCode, out countryName)).Returns(true);
        _repositoryMock.Setup(r => r.TryAddPermanent(It.IsAny<BlockedCountry>())).Returns(true);

        // Act
        var result = _service.AddPermanentBlock(countryCode);

        // Assert
        Assert.Equal(countryCode, result.CountryCode);
        Assert.Equal(countryName, result.CountryName);
        _repositoryMock.Verify(r => r.TryAddPermanent(It.Is<BlockedCountry>(c => c.CountryCode == countryCode)), Times.Once);
        _repositoryMock.Verify(r => r.TryRemoveTemporal(countryCode), Times.Once);
    }

    [Fact]
    public void AddPermanentBlock_ShouldThrow_WhenAlreadyPermanentlyBlocked()
    {
        // Arrange
        var countryCode = "EG";
        var countryName = "Egypt";
        _catalogMock.Setup(c => c.TryGetCountryName(countryCode, out countryName)).Returns(true);
        _repositoryMock.Setup(r => r.TryAddPermanent(It.IsAny<BlockedCountry>())).Returns(false);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _service.AddPermanentBlock(countryCode));
    }

    [Fact]
    public void AddTemporalBlock_ShouldThrow_WhenPermanentlyBlocked()
    {
        // Arrange
        var countryCode = "EG";
        var countryName = "Egypt";
        _catalogMock.Setup(c => c.TryGetCountryName(countryCode, out countryName)).Returns(true);
        _repositoryMock.Setup(r => r.IsBlocked(countryCode)).Returns(true);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => _service.AddTemporalBlock(countryCode, 60));
        Assert.Equal("Country is already blocked.", ex.Message);
    }

    [Fact]
    public void AddTemporalBlock_ShouldAddSuccessfully_WhenNotBlocked()
    {
        // Arrange
        var countryCode = "EG";
        var countryName = "Egypt";
        _catalogMock.Setup(c => c.TryGetCountryName(countryCode, out countryName)).Returns(true);
        _repositoryMock.Setup(r => r.IsBlocked(countryCode)).Returns(false);
        _repositoryMock.Setup(r => r.TryAddTemporal(It.IsAny<TemporalBlockedCountry>())).Returns(true);

        // Act
        var result = _service.AddTemporalBlock(countryCode, 60);

        // Assert
        Assert.Equal(countryCode, result.CountryCode);
        _repositoryMock.Verify(r => r.TryAddTemporal(It.IsAny<TemporalBlockedCountry>()), Times.Once);
    }

    [Fact]
    public void RemovePermanentBlock_ShouldTryRemoveFromBoth_WhenValidCodeProvided()
    {
        // Arrange
        var countryCode = "EG";
        _repositoryMock.Setup(r => r.TryRemovePermanent(countryCode)).Returns(true);
        _repositoryMock.Setup(r => r.TryRemoveTemporal(countryCode)).Returns(false);

        // Act
        _service.RemovePermanentBlock(countryCode);

        // Assert
        _repositoryMock.Verify(r => r.TryRemovePermanent(countryCode), Times.Once);
        _repositoryMock.Verify(r => r.TryRemoveTemporal(countryCode), Times.Once);
    }

    [Fact]
    public void RemovePermanentBlock_ShouldThrow_WhenCountryNotBlockedInEitherStorage()
    {
        // Arrange
        var countryCode = "EG";
        _repositoryMock.Setup(r => r.TryRemovePermanent(countryCode)).Returns(false);
        _repositoryMock.Setup(r => r.TryRemoveTemporal(countryCode)).Returns(false);

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => _service.RemovePermanentBlock(countryCode));
    }

    [Fact]
    public void GetBlockedCountries_ShouldCombineAndFilterResults()
    {
        // Arrange
        var permanent = new[] { new BlockedCountry { CountryCode = "US", CountryName = "USA" } };
        var temporal = new[] { new TemporalBlockedCountry { CountryCode = "EG", CountryName = "Egypt", ExpiresAtUtc = DateTimeOffset.UtcNow.AddHours(1) } };

        _repositoryMock.Setup(r => r.GetPermanentCountries()).Returns(permanent);
        _repositoryMock.Setup(r => r.GetTemporalCountries()).Returns(temporal);

        // Act
        var result = _service.GetBlockedCountries(1, 10, null);

        // Assert
        Assert.Equal(2, result.TotalCount);
        Assert.Contains(result.Items, i => i.CountryCode == "US" && !i.IsTemporal);
        Assert.Contains(result.Items, i => i.CountryCode == "EG" && i.IsTemporal);
    }
}
