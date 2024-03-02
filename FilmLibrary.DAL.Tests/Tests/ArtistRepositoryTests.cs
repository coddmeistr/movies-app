using FilmLibrary.Business.Interfaces;
using FilmLibrary.Business.Services;
using FilmLibrary.Core.Entities;
using FilmLibrary.DAL.Context;
using FilmLibrary.DAL.Repositories;
using FilmLibrary.SharedData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Transactions;

namespace FilmLibrary.DAL.Tests.Tests
{
    public class ArtistRepositoryTests
    {
        private ApplicationDbContext _context;
        private ArtistRepository _artistRepository;
        private IUnitOfWork _unitOfWork;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder().UseSqlServer(UnitTestsConstants.TestsConnectionString).Options;
            _context = new ApplicationDbContext(options);
            _artistRepository = new ArtistRepository(_context);
            _unitOfWork = new UnitOfWork(_context);
        }

        [Test]
        public void Create_WhenArtistEntityIsSet_ShouldCreateNewArtist()
        {
            using (var scope = new TransactionScope())
            {
                var expectedArtistName = "Oleg";
                var artist = new Artist() { Name = expectedArtistName };

                _artistRepository.Create(artist);
                _unitOfWork.SaveChanges();

                var result = _artistRepository.GetByName(artist.Name);

                result?.Name.Should().Be(expectedArtistName);
            }
        }

        [Test]
        public void Delete_WhenArtistNotExist_ShouldNotDeleteArtist()
        {
            using (var scope = new TransactionScope())
            {
                var expectedArtistName = "Oleg";
                var incorrectArtistName = "Stsiapan";
                var preresult = _artistRepository.GetByName(expectedArtistName);
                preresult.Should().BeNull();
                var artist = new Artist() { Name = expectedArtistName };

                _artistRepository.Create(artist);
                _unitOfWork.SaveChanges();

                _artistRepository.Delete(incorrectArtistName);
                _unitOfWork.SaveChanges();

                var result = _artistRepository.GetByName(artist.Name);

                result?.Name.Should().Be(expectedArtistName);
            }
        }

        [Test]
        public void Delete_WhenNameIsCorrect_ShouldDeleteArtist()
        {
            using (var scope = new TransactionScope())
            {
                var expectedArtistName = "Oleg";
                var artist = new Artist() { Name = expectedArtistName };
                var preresult = _artistRepository.GetByName(artist.Name);
                preresult.Should().BeNull();

                _artistRepository.Create(artist);
                _unitOfWork.SaveChanges();

                _artistRepository.Delete(artist.Name);
                _unitOfWork.SaveChanges();

                var result = _artistRepository.GetByName(expectedArtistName);

                result.Should().BeNull();
            }
        }

        [Test]
        public void GetByName_WhenNameOfArtistIsIncorrect_ShouldReturnNull()
        {
            using (var scope = new TransactionScope())
            {
                var incorrectArtistName = "Olga";
                var artist = new Artist() { Name = "Oleg" };

                _artistRepository.Create(artist);
                _unitOfWork.SaveChanges();

                var result = _artistRepository.GetByName(incorrectArtistName);

                result.Should().BeNull();
            }
        }

        [Test]
        public void GetByName_WhenNameOfArtistIsCorrect_ShouldReturnArtist()
        {
            using (var scope = new TransactionScope())
            {
                var expectedArtistName = "Oleg";
                var artist = new Artist() { Name = expectedArtistName };

                _artistRepository.Create(artist);
                _unitOfWork.SaveChanges();

                var result = _artistRepository.GetByName(artist.Name);

                result?.Name.Should().Be(expectedArtistName);
            }
        }

        [Test]
        public void GetAll_WhenContextIsNotNull_ShouldReturnAllArtists()
        {
            using (var scope = new TransactionScope())
            {
                var firstArtist = new Artist() { Name = "Oleg" };
                var secondArtist = new Artist() { Name = "Olga" };
                var thirdArtist = new Artist() { Name = "Mikhail" };
                var expectedArtistsList = new List<Artist>() { firstArtist, secondArtist, thirdArtist };

                _artistRepository.Create(firstArtist);
                _artistRepository.Create(secondArtist);
                _artistRepository.Create(thirdArtist);
                _unitOfWork.SaveChanges();

                var result = _artistRepository.GetAll();

                result.Should().Contain(expectedArtistsList);
            }
        }

        [Test]
        public void GetById_WhenIdIsIncorrect_ShouldReturnNull()
        {
            using (var scope = new TransactionScope())
            {
                var incorrectId = 10000000;
                var artist = new Artist() { Name = "Kitty" };

                _artistRepository.Create(artist);
                _unitOfWork.SaveChanges();

                var result = _artistRepository.GetById(incorrectId);

                result.Should().BeNull();
            }
        }

        [Test]
        public void GetById_WhenIdIsCorrect_ShouldReturnMovie()
        {
            using (var scope = new TransactionScope())
            {
                var expectedArtistName = "Kitty";
                var artist = new Artist() { Name = expectedArtistName };

                _artistRepository.Create(artist);
                _unitOfWork.SaveChanges();
                var expectedId = artist.Id;

                var result = _artistRepository.GetById(expectedId);

                result?.Name.Should().Be(expectedArtistName);
            }
        }
    }
}