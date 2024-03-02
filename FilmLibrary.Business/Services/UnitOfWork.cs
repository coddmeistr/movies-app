using FilmLibrary.Business.Interfaces;
using FilmLibrary.DAL.Context;

namespace FilmLibrary.Business.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public UnitOfWork(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
        }

        public void SaveChanges()
        {
            _applicationDbContext.SaveChanges();
        }
    }
}