using Common.Data.Context;
using Common.Data.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace Common.Data.Repositories
{
    public class GenericRepository<TModel> : IGenericRepository<TModel> where TModel : class
    {

        private readonly LogisticContext _dbcontext;

        public GenericRepository(LogisticContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<List<TModel>> GetEmployees()
        {
            try
            {
                return await _dbcontext.Set<TModel>().ToListAsync();
            }
            catch
            {
                throw;
            }
        }
        public async Task<List<TModel>> GetAllUsers()
        {
            try
            {
                return await _dbcontext.Set<TModel>().ToListAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}
