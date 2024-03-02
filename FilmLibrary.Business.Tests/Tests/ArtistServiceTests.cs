using FilmLibrary.Business.Services;
using FilmLibrary.Core.Entities;
using FilmLibrary.DAL.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace FilmLibrary.Business.Tests.Tests
{
    [TestFixture]
    public class ArtistServiceTests
    {
        private Mock<IArtistRepository> _artistRepository;
        private ArtistService _artistService;

        [SetUp]
        public void Setup()
        {
            _artistRepository = new Mock<IArtistRepository>();
            _artistService = new ArtistService(_artistRepository.Object);
        }

        [Test]
        public void GetAll_WhenMethodIsInvoked_ShouldReturnAllArtists()
        {
            var artists = new List<Artist>() { new Artist() { Name = "Philip" } };
            _artistRepository.Setup(a => a.GetAll()).Returns(artists);

            var result = _artistService.GetAll();

            result.Should().BeEquivalentTo(artists);
        }
    }
}