using Common.Core.ViewModels;
using Common.Data.Context;
using Common.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace Common.Core.Services
{
    public interface ISalariesService
    {
        Task<List<Salaries>> GetAllSalaries();
        Task<Salaries> GetSalaryByEmployeeId(int employeeId);
        Task<bool> AddOrUpdateSalary(Salaries salary, string UserId);
        Task<List<Salaries>> GetMonthSalariesList();
        Task<List<SalaryViewModel>> GetAllSalariesWithEmployeeNameAsync();
        Task<bool> AddOrUpdateSalaryAsync(SalaryViewModel model);
    }
    public class SalariesService : ISalariesService
    {
        private readonly LogisticContext _dbcontext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SalariesService(LogisticContext dbcontext, IHttpContextAccessor httpContextAccessor)
        {
            _dbcontext = dbcontext;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<Salaries>> GetMonthSalariesList()
        {
            return await _dbcontext.Salaries.OrderByDescending(s => s.Year).ThenByDescending(s => s.Month).ToListAsync();
        }
        public async Task<bool> AddOrUpdateSalaryAsync(SalaryViewModel model)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";

            // ✅ Duplicate Check (Same Employee, Year, Month - but ignore if updating same record)
            var duplicate = await _dbcontext.Salaries
                .FirstOrDefaultAsync(s => s.EmployeeId == model.EmployeeId
                    && s.Month == model.Month
                    && s.Year == model.Year
                    && s.Id != model.Id);

            if (duplicate != null)
            {
                throw new Exception("Salary already exists for this employee for the selected month.");
            }

            Salaries entity;
            if (model.Id > 0)
            {
                entity = await _dbcontext.Salaries.FindAsync(model.Id);
                if (entity == null)
                    return false;

                entity.UpdatedOn = DateTime.Now;
                entity.UpdatedBy = userId;
            }
            else
            {
                entity = new Salaries
                {
                    AddedOn = DateTime.Now,
                    AddedBy = userId,
                    Active = true
                };
                await _dbcontext.Salaries.AddAsync(entity);
            }

            entity.EmployeeId = model.EmployeeId;
            entity.Month = model.Month;
            entity.Year = model.Year;
            entity.BaseSalary = model.BaseSalary;
            entity.OvertimePay = model.OvertimePay;
            entity.Bonus = model.Bonus;
            entity.Advance = model.Advance;
            entity.Deduction = model.Deduction;
            entity.NetSalary = model.NetSalary;
            entity.PaymentDate = model.PaymentDate;
            entity.IsPaid = model.IsPaid;

            await _dbcontext.SaveChangesAsync();
            return true;
        }


        public async Task<List<SalaryViewModel>> GetAllSalariesWithEmployeeNameAsync()
        {
            return await (from s in _dbcontext.Salaries
                          join e in _dbcontext.Employees on s.EmployeeId equals e.Id
                          select new SalaryViewModel
                          {
                              Id = s.Id,
                              EmployeeId = s.EmployeeId,
                              EmployeeName = e.Name,
                              Year = s.Year,
                              Month = s.Month,
                              PaymentDate = s.PaymentDate,
                              BaseSalary = s.BaseSalary,
                              WorkingDays = s.WorkingDays,
                              PresentDays = s.PresentDays,
                              AbsentDays = s.AbsentDays,
                              LeaveDays = s.LeaveDays,
                              IsPaid = s.IsPaid,
                              OvertimeHours = s.OvertimeHours,
                              OvertimePay = s.OvertimePay,
                              Bonus = s.Bonus,
                              Advance = s.Advance,
                              Deduction = s.Deduction,
                              NetSalary = s.NetSalary
                          }).OrderByDescending(x => x.Year)
                            .ThenByDescending(x => x.Month)
                            .ToListAsync();
        }

        public async Task<List<Salaries>> GetAllSalaries()
        {
            return await _dbcontext.Salaries.ToListAsync();
        }

        public async Task<Salaries> GetSalaryByEmployeeId(int employeeId)
        {
            return await _dbcontext.Salaries.FirstOrDefaultAsync(s => s.EmployeeId == employeeId);
        }

        public async Task<bool> AddOrUpdateSalary(Salaries salary, string UserId)
        {
            var salaryEntity = await _dbcontext.Salaries
                .FirstOrDefaultAsync(s => s.Id == salary.Id && s.EmployeeId == salary.EmployeeId);

            if (salary.Id == 0 || salaryEntity == null)
            {
                salaryEntity = new Salaries
                {
                    EmployeeId = salary.Id,
                    BaseSalary = salary.BaseSalary,
                    OvertimePay = salary.OvertimePay,
                    Bonus = salary.Bonus,
                    NetSalary = salary.NetSalary,
                    Active = true,
                    AddedBy = UserId,
                    AddedOn = DateTime.Now
                };
                _dbcontext.Salaries.Add(salaryEntity);
            }
            else
            {
                salaryEntity.BaseSalary = salary.BaseSalary;
                salaryEntity.OvertimePay = salary.OvertimePay;
                salaryEntity.Bonus = salary.Bonus;
                salaryEntity.Active = salary.Active;
                salaryEntity.UpdatedOn = DateTime.Now;
                salaryEntity.UpdatedBy = UserId;
                _dbcontext.Salaries.Update(salaryEntity);
            }

            await _dbcontext.SaveChangesAsync();
            return true;
        }

    }
}
