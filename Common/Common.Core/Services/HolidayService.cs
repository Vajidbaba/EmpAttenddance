using Common.Data.Context;
using Common.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core.Services
{
    public interface IHolidayService
    {
        Task<List<Holidays>> GetAllAsync();
        Task<Holidays?> GetByIdAsync(int id);
        Task SaveAsync(Holidays holiday);
    }
    public class HolidayService : IHolidayService
    {
        private readonly LogisticContext _dbcontext;

        public HolidayService(LogisticContext dbcontext, IContextHelper contextHelper)
        {
            _dbcontext = dbcontext;
        }
        public async Task<List<Holidays>> GetAllAsync()
        {
            return await _dbcontext.Holidays.OrderByDescending(x =>x.Id).ToListAsync();
        }

        public async Task<Holidays?> GetByIdAsync(int id)
        {
            return await _dbcontext.Holidays.FindAsync(id);
        }
        public async Task SaveAsync(Holidays holiday)
        {
            if (holiday.Id == 0)
            {
                _dbcontext.Holidays.Add(holiday);
            }
            else
            {
                _dbcontext.Holidays.Update(holiday);
            }
            await _dbcontext.SaveChangesAsync();
        }

    }
}
