using System;
using FluentAssertions;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;

namespace GameStore.UnitTests.Mapping;

public class GameMappingTests
{
    // Naming pattern: MethodName_StateUnderTest_ExpectedBehavior
    // Structure pattern?: Arrange, Act, Assert
    // _game: it is the subject of the test

    private readonly Game _game;

    public GameMappingTests()
    {
        var genre = new Genre()
        {
            Id = 1,
            Name = "Sample Genre"
        };
        // _game
        _game = new Game
        {
            Id = 1,
            Name = "Sample Game",
            GenreId = genre.Id,
            Price = 29.99m,
            ReleaseDate = new DateOnly(),
            Genre = genre
        };
    }

    [Fact]
    public void ToGameDetailsDto_Game_ReturnsCorrectValues()
    {
        // Arrange
        var expected = new GameDetailsDto(
            _game.Id,
            _game.Name,
            _game.GenreId,
            _game.Price,
            _game.ReleaseDate
        );

        // Act
        var result = _game.ToGameDetailsDto();

        // Assert
        // Default Xunit syntax
            // Testing each attribute
        Assert.NotNull(result);
        Assert.Equal(_game.Id, result.Id);
        Assert.Equal(_game.Name, result.Name);
        Assert.Equal(_game.GenreId, result.GenreId);
        Assert.Equal(_game.Price, result.Price);
        Assert.Equal(_game.ReleaseDate, result.ReleaseDate);
        Assert.IsType<GameDetailsDto>(result);
            // Testing the entire object
        Assert.Equivalent(expected, result);
        // Using fluent assertions package
        result.Should().NotBeNull();
        result.Id.Should().Be(_game.Id);
        result.Name.Should().Be(_game.Name);
        result.GenreId.Should().Be(_game.GenreId);
        result.Price.Should().Be(_game.Price);
        result.ReleaseDate.Should().Be(_game.ReleaseDate);
        result.Should().BeOfType<GameDetailsDto>();
            // Testing the entire object
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ToGameSummaryDto_Game_ReturnsCorrectValues()
    {
        var expected = new GameSummaryDto(
            _game.Id,
            _game.Name,
            _game.Genre!.Name,
            _game.Price,
            _game.ReleaseDate
        );

        var result = _game.ToGameSummaryDto();

        Assert.NotNull(result);
        Assert.Equal(_game.Id, result.Id);
        Assert.Equal(_game.Name, result.Name);
        Assert.Equal(_game.Genre?.Name, result.Genre);
        Assert.Equal(_game.Price, result.Price);
        Assert.Equal(_game.ReleaseDate, result.ReleaseDate);
    }
}
