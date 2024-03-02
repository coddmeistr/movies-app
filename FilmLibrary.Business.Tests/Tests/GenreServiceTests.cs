using FilmLibrary.Business.Services;
using FilmLibrary.Core.Entities;
using FilmLibrary.DAL.Interfaces;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using FluentAssertions;

namespace FilmLibrary.Business.Tests.Tests
{
    public class GenreServiceTests
    {
        private Mock<IGenreRepository> _genreRepository;
        private GenreService _genreService;

        [SetUp]
        public void Setup()
        {
            _genreRepository = new Mock<IGenreRepository>();
            _genreService = new GenreService(_genreRepository.Object);
        }

        [Test]
        public void GetAll_WhenMethodIsInvoked_ShouldReturnAllGenres()
        {
            var genres = new List<Genre>() { new Genre() { Name = "Test genre" } };
            _genreRepository.Setup(a => a.GetAll()).Returns(genres);

            var result = _genreService.GetAll();

            result.Should().BeEquivalentTo(genres);
        }
    }
}
