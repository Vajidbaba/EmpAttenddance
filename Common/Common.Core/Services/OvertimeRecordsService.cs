

using Common.Data.Context;
using Common.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Common.Core.Services
{
    public interface IOvertimeRecordsService
    {
        Task<List<OvertimeRecords>> GetAllOvertimes();
        Task<OvertimeRecords> GetOvertimesById(int employeeId);
    }
    public class OvertimeRecordsService : IOvertimeRecordsService
    {
        private readonly LogisticContext _dbcontext;

        public OvertimeRecordsService(LogisticContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<List<OvertimeRecords>> GetAllOvertimes()
        {
            var result = await _dbcontext.OvertimeRecords.ToListAsync();
            return result;
        }

        public async Task<OvertimeRecords> GetOvertimesById(int employeeId)
        {
            var result = await _dbcontext.OvertimeRecords.FirstOrDefaultAsync(s => s.EmployeeId == employeeId);
            return result; 
        }

  
    }
}
