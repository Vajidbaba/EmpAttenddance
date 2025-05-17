using Common.Data.Context;
using Common.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Common.Core.Services
{
    public interface ISalariesService
    {
        Task<List<Salaries>> GetAllSalaries();
        Task<Salaries> GetSalaryByEmployeeId(int employeeId);
        Task<bool> UpdateSalary(Salaries salary);
    }
    public class SalariesService : ISalariesService
    {
        private readonly LogisticContext _dbcontext;

        public SalariesService(LogisticContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<List<Salaries>> GetAllSalaries()
        {
            return await _dbcontext.Salaries.ToListAsync();
        }

        public async Task<Salaries> GetSalaryByEmployeeId(int employeeId)
        {
            return await _dbcontext.Salaries.FirstOrDefaultAsync(s => s.EmployeeId == employeeId);
        }

        public async Task<bool> UpdateSalary(Salaries salary)
        {
            var salaryEntity = await _dbcontext.Salaries.FirstOrDefaultAsync(s => s.EmployeeId == salary.EmployeeId);
            if (salaryEntity != null)
            {
                salaryEntity.BaseSalary = salary.BaseSalary;
                salaryEntity.OvertimePay = salary.OvertimePay;
                salaryEntity.Deductions = salary.Deductions;
                salaryEntity.NetSalary = salaryEntity.BaseSalary + salaryEntity.OvertimePay - salaryEntity.Deductions;
                salaryEntity.UpdatedOn = DateTime.Now;

                _dbcontext.Update(salaryEntity);
                await _dbcontext.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
