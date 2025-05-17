using Common.Data.Context;
using Common.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Common.Core.Services
{
    public interface IMasterOvertimeService
    {
        Task<List<OvertimeMaster>> GetAllAsync();
        Task<OvertimeMaster?> GetByIdAsync(int id);
        Task AddOrUpdateAsync(OvertimeMaster model, string userId);
        SelectList GetDropdownListAsync(); // No changes needed here
    }
    public class MasterOvertimeService : IMasterOvertimeService
    {
        private readonly LogisticContext _db;

        public MasterOvertimeService(LogisticContext dbcontext)
        {
            _db = dbcontext;
        }

        public async Task<List<OvertimeMaster>> GetAllAsync()
        {
            return await _db.OvertimeMaster.Where(x => x.Active).ToListAsync();
        }

        public async Task<OvertimeMaster?> GetByIdAsync(int id)
        {
            return await _db.OvertimeMaster.FindAsync(id);
        }

        public async Task AddOrUpdateAsync(OvertimeMaster model, string userId)
        {
            if (model.Id == 0)
            {
                model.AddedBy = userId;
                model.AddedOn = DateTime.Now;
                model.Active = true;
                _db.OvertimeMaster.Add(model);
            }
            else
            {
                var existing = await _db.OvertimeMaster.FindAsync(model.Id);
                if (existing != null)
                {
                    existing.Active = model.Active;
                    existing.Type = model.Type;
                    existing.RatePerHour = model.RatePerHour;
                    existing.Description = model.Description;
                    existing.UpdatedBy = userId;
                    existing.UpdatedOn = DateTime.Now;
                    _db.OvertimeMaster.Update(existing);
                }
            }
            await _db.SaveChangesAsync();
        }

        public SelectList GetDropdownListAsync()
        {
            var list = _db.OvertimeMaster.ToList();
            return new SelectList(list, "Id", "Type");
        }
    }
}

