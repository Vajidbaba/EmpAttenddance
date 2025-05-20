using Common.Data.Context;
using Common.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core.Services
{
    public interface IMasterDepartmentService
    {
        Task<List<DepartmentMaster>> GetActiveDepartmentsAsync();
        SelectList GetDepartmentSelectListAsync();
        Task<DepartmentMaster> GetMasterById(int id);
        Task<DepartmentMaster> SaveMaster(DepartmentMaster leaveMaster, string userId);
    }
    public class MasterDepartmentService: IMasterDepartmentService
    {
        private readonly LogisticContext _dbContext;
        public MasterDepartmentService(LogisticContext dbcontext)
        {
            _dbContext = dbcontext;
        }
        public async Task<List<DepartmentMaster>> GetActiveDepartmentsAsync()
        {
            return await _dbContext.DepartmentMaster.Where(d => d.Active).OrderBy(d => d.Name).ToListAsync();
        }
        public async Task<DepartmentMaster> GetMasterById(int id)
        {
            var result = await _dbContext.DepartmentMaster.FirstOrDefaultAsync(s => s.Id == id);
            return result;
        }
        public SelectList GetDepartmentSelectListAsync()
        {
            var departments =  _dbContext.DepartmentMaster.Where(x => x.Active).OrderBy(x => x.Name).ToList();

            return new SelectList(departments, "Id", "Name");
        }
        public async Task<DepartmentMaster> SaveMaster(DepartmentMaster leaveMaster, string userId)
        {
            if (leaveMaster.Id == 0)
            {
                leaveMaster.AddedBy = userId;
                leaveMaster.AddedOn = DateTime.UtcNow;
                leaveMaster.Active = true;
                _dbContext.DepartmentMaster.Add(leaveMaster);
            }
            else
            {
                var existing = await _dbContext.DepartmentMaster.FindAsync(leaveMaster.Id);
                if (existing == null || existing.Active == false)
                    throw new Exception("Record not found or inactive");

                existing.Name = leaveMaster.Name;
                existing.Description = leaveMaster.Description;
                existing.Active = leaveMaster.Active;
                existing.UpdatedOn = DateTime.UtcNow;
                existing.UpdatedBy = userId;
                _dbContext.DepartmentMaster.Update(existing);

            }

            await _dbContext.SaveChangesAsync();
            return leaveMaster;
        }

    }
}
