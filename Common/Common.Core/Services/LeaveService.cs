using Common.Core.ViewModels;
using Common.Data.Context;
using Common.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Common.Core.Services
{
    public interface ILeaveService
    {
        Task<List<LeaveMasterViewModel>> GetLeaveMasterAsync();
        Task<List<LeaveMaster>> AllLeaveMasterAsync();
        Task<LeaveMaster> SaveMaster(LeaveMaster leaveMaster, string userId);
        Task<LeaveMaster> GetMasterById(int id);

        public class LeaveService : ILeaveService
        {
            private readonly LogisticContext _dbcontext;
            private readonly IContextHelper _contextHelper;

            public LeaveService(LogisticContext dbcontext, IContextHelper contextHelper)
            {
                _dbcontext = dbcontext;
                _contextHelper = contextHelper;
            }
            public async Task<List<LeaveMaster>> AllLeaveMasterAsync()
            {
                int currentYear = DateTime.Now.Year;
                var data = await _dbcontext.LeaveMaster.Where(x => x.Active == true && x.Year == currentYear).ToListAsync();
                return data;
            }
            public async Task<List<LeaveMasterViewModel>> GetLeaveMasterAsync()
            {
                int currentYear = DateTime.Now.Year;

                var data = await _dbcontext.LeaveMaster.Where(x => x.Active == true && x.Year == currentYear)
                    .Join(_dbcontext.DepartmentMaster, leave => leave.DepartmentId, dept => dept.Id, (leave, dept) => new LeaveMasterViewModel
                          {
                              Id = leave.Id,
                              DepartmentName = dept.Name,
                              SickLeaves = leave.SickLeaves,
                              CasualLeaves = leave.CasualLeaves,
                              PaidLeaves = leave.PaidLeaves,
                              UnpaidLeaves = leave.UnpaidLeaves,
                              Year = leave.Year
                          })
                    .ToListAsync();

                return data;

        }


        public async Task<LeaveMaster> GetMasterById(int id)
            {
                var result = await _dbcontext.LeaveMaster.FirstOrDefaultAsync(s => s.Id == id);
                return result;
            }
            public async Task<LeaveMaster> SaveMaster(LeaveMaster leaveMaster, string userId)
            {
                if (leaveMaster.Id == 0)
                {
                    leaveMaster.AddedBy = userId;
                    leaveMaster.AddedOn = DateTime.UtcNow;
                    leaveMaster.Active = true;
                    _dbcontext.LeaveMaster.Add(leaveMaster);
                }
                else
                {
                    var existing = await _dbcontext.LeaveMaster.FindAsync(leaveMaster.Id);
                    if (existing == null || existing.Active == false)
                        throw new Exception("Record not found or inactive");

                    existing.DepartmentId = leaveMaster.DepartmentId;
                    existing.SickLeaves = leaveMaster.SickLeaves;
                    existing.CasualLeaves = leaveMaster.CasualLeaves;
                    existing.PaidLeaves = leaveMaster.PaidLeaves;
                    existing.UnpaidLeaves = leaveMaster.UnpaidLeaves;
                    existing.Year = leaveMaster.Year;
                    existing.Active = leaveMaster.Active;
                    existing.UpdatedOn = DateTime.UtcNow;
                    existing.UpdatedBy = userId;
                    _dbcontext.LeaveMaster.Update(existing);

                }

                await _dbcontext.SaveChangesAsync();
                return leaveMaster;
            }
        }
    }
}
