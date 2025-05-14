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
    public interface ISalaryService
    {
        Task<List<SalaryModel>> GetAllSalaries();
        Task<SalaryModel> GetSalaryByEmployeeId(int employeeId);
        Task<bool> AddSalary(SalaryModel salary);
        Task<bool> UpdateSalary(int id, SalaryModel salary);
    }
    public class SalaryService : ISalaryService
    {
        private readonly LogisticContext _dbcontext;
        private readonly IContextHelper _contextHelper;

        public SalaryService(LogisticContext dbcontext, IContextHelper contextHelper)
        {
            _dbcontext = dbcontext;
            _contextHelper = contextHelper;
        }

        public async Task<List<SalaryModel>> GetAllSalaries()
        {
            return await _dbcontext.Salary.ToListAsync();
        }

        public async Task<SalaryModel> GetSalaryByEmployeeId(int employeeId)
        {
            return await _dbcontext.Salary.FirstOrDefaultAsync(s => s.EmployeeId == employeeId);
        }

        public async Task<bool> AddSalary(SalaryModel salary)
        {
            var userId = _contextHelper.GetUsername();

            // ✅ Check if salary already exists for this EmployeeId
            var existingSalary = await _dbcontext.Salary.AnyAsync(s => s.EmployeeId == salary.EmployeeId);

            if (existingSalary)
            {
                throw new Exception($"Salary record already exists for Employee ID {salary.EmployeeId}!");
            }

            // If no existing salary, proceed with insertion
            var newRecord = new SalaryModel
            {
                EmployeeId = salary.EmployeeId,
                SalaryType = salary.SalaryType,
                BaseSalary = salary.BaseSalary,
                OvertimePay = salary.OvertimePay,
                Deductions = salary.Deductions,
                NetSalary = salary.BaseSalary + salary.OvertimePay - salary.Deductions,
                PaymentDate = DateTime.Now,
                AddedBy = userId,
                AddedOn = DateTime.Now,
                UpdatedBy = userId,
                UpdatedOn = DateTime.Now
            };

            _dbcontext.Salary.Add(newRecord);
            await _dbcontext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateSalary(int id, SalaryModel salary)
        {
            var record = await _dbcontext.Salary.FirstOrDefaultAsync(x => x.Id == id);
            if (record == null) return false;

            var userId = _contextHelper.GetUsername();
            record.BaseSalary = salary.BaseSalary;
            record.OvertimePay = salary.OvertimePay;
            record.Deductions = salary.Deductions;
            record.NetSalary = salary.BaseSalary + salary.OvertimePay - salary.Deductions;
            record.UpdatedBy = userId;
            record.UpdatedOn = DateTime.Now;

            _dbcontext.Salary.Update(record);
            await _dbcontext.SaveChangesAsync();
            return true;
        }
    }
}
