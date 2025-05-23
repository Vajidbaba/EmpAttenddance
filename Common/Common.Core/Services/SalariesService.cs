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
        Task<SalaryViewModel> CalculateSalary(int employeeId);

        Task<SalaryViewModel> GetSalaryDetailsAsync(int employeeId, int year, int month);
        Task SaveSalaryAsync(SalaryViewModel model);
        Task<SalaryResultDto> CalculateEmployeeSalaryAsync(int employeeId, int year, int month);
        Task<List<SalaryListDto>> GetSalaryListAsync(int year, int month);

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

        public async Task<SalaryViewModel> CalculateSalary(int employeeId)
        {
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            var employees = await _dbcontext.Employees.ToListAsync();
            var salaries = await _dbcontext.Salaries.ToListAsync();

            var result = (from e in employees
                          join s in salaries on e.Id equals s.EmployeeId into empSalaries
                          from sal in empSalaries.DefaultIfEmpty()
                          select new SalaryViewModel
                          {
                              EmployeeId = e.Id,
                              EmployeeName = e.Name,
                              BaseSalary = e.BaseSalary,
                              NetSalary = sal?.NetSalary ?? 0,
                              OvertimeHours = sal?.OvertimeHours ?? 0,
                              WorkingDays = sal?.WorkingDays ?? 0,
                              PresentDays = sal?.PresentDays ?? 0,
                              OvertimePay = sal?.OvertimePay ?? 0,
                              Deduction = sal?.Deduction ?? 0,
                              Year = sal?.Year ?? 0,
                              Month = sal?.Month ?? 0,
                              IsPaid = sal?.IsPaid ?? false,
                          }).OrderBy(x => x.EmployeeName).FirstOrDefault();


            return result;
        }


        public async Task<List<SalaryViewModel>> GetAllSalariesWithEmployeeNameAsync()
        {
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            var employees = await _dbcontext.Employees.ToListAsync();
            var salaries = await _dbcontext.Salaries
                // .Where(s => s.Month == currentMonth && s.Year == currentYear)
                .ToListAsync();

            var result = (from e in employees
                          join s in salaries on e.Id equals s.EmployeeId into empSalaries
                          from sal in empSalaries.DefaultIfEmpty()
                          select new SalaryViewModel
                          {
                              EmployeeId = e.Id,
                              EmployeeName = e.Name,
                              BaseSalary = e.BaseSalary,
                              NetSalary = sal?.NetSalary ?? 0,
                              OvertimeHours = sal?.OvertimeHours ?? 0,
                              WorkingDays = sal?.WorkingDays ?? 0,
                              PresentDays = sal?.PresentDays ?? 0,
                              OvertimePay = sal?.OvertimePay ?? 0,
                              Deduction = sal?.Deduction ?? 0,
                              Year = sal?.Year ?? 0,
                              Month = sal?.Month ?? 0,
                              IsPaid = sal?.IsPaid ?? false,
                          }).OrderBy(x => x.EmployeeName).ToList();

            return result;
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
        public async Task<List<SalaryListDto>> GetSalaryListAsync(int year, int month)
        {
            var employees = await _dbcontext.Employees.ToListAsync();

            var salaryList = new List<SalaryListDto>();

            foreach (var emp in employees)
            {
                var salary = await CalculateEmployeeSalaryAsync(emp.Id, year, month);

                salaryList.Add(new SalaryListDto
                {
                    EmployeeId = emp.Id,
                    EmployeeName = emp.Name,
                    Year = year,
                    Month = month,
                    NetSalary = salary.NetSalary
                });
            }

            return salaryList;
        }

        public async Task<SalaryResultDto> CalculateEmployeeSalaryAsync(int employeeId, int year, int month)
        {
            var employee = await _dbcontext.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
            if (employee == null)
                throw new Exception("Employee not found");

            var baseSalary = employee.BaseSalary;

            // 1. Get all days of the month
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            int totalDays = DateTime.DaysInMonth(year, month);

            // 2. Get holidays
            var holidays = await _dbcontext.Holidays
                .Where(h => h.Date >= startDate && h.Date <= endDate && (h.IsWeekend || h.IsCompanyHoliday))
                .Select(h => h.Date)
                .ToListAsync();

            int totalHolidays = holidays.Count;
            int workingDays = totalDays - totalHolidays;

            // 3. Get attendance
            var attendanceRecords = await _dbcontext.MarkAttendance
                .Where(a => a.EmployeeId == employeeId && a.AttendanceDate >= startDate && a.AttendanceDate <= endDate)
                .ToListAsync();

            int presentDays = attendanceRecords.Count(a => a.AttendanceStatus == "Present");
            int halfDays = attendanceRecords.Count(a => a.AttendanceStatus == "Half Day");
            int paidLeaves = attendanceRecords.Count(a => a.AttendanceStatus == "Leave");
            int absentDays = attendanceRecords.Count(a => a.AttendanceStatus == "Absent");

            // 4. Final attendance stats
            int totalHalfDays = halfDays;

            // 5. Per day salary
            decimal perDaySalary = baseSalary / workingDays;
            decimal earnedSalary = (presentDays * perDaySalary)
                                 + (paidLeaves * perDaySalary)
                                 + ((totalHalfDays * 0.5m) * perDaySalary);

            decimal deduction = absentDays * perDaySalary;

            // 6. Overtime
            var overtimeRecords = await _dbcontext.OvertimeRecords
                .Where(o => o.EmployeeId == employeeId && o.Date >= startDate && o.Date <= endDate)
                .ToListAsync();

            decimal totalOtHours = overtimeRecords.Sum(o => o.TotalOvertimeHours ?? 0);
            decimal ratePerHour = await GetRatePerHourAsync(employee.OTtype ?? 0);
            decimal overtimePay = totalOtHours * ratePerHour;

            // 7. Final net salary
            decimal netSalary = earnedSalary + overtimePay - deduction;

            return new SalaryResultDto
            {
                EmployeeId = employeeId,
                Month = month,
                Year = year,
                WorkingDays = workingDays,
                PresentDays = presentDays,
                PaidLeaveDays = paidLeaves,
                HalfDays = halfDays,
                UnpaidLeaves = 0,
                AbsentDays = absentDays,
                OvertimeHours = totalOtHours,
                OvertimePay = overtimePay,
                EarnedSalary = earnedSalary,
                Deduction = deduction,
                NetSalary = netSalary
            };
        }



        public async Task<decimal> GetRatePerHourAsync(int otTypeId)
        {
            var ot = await _dbcontext.OvertimeMaster.FindAsync(otTypeId);
            return ot?.RatePerHour ?? 0;
        }


        public async Task<SalaryViewModel> GetSalaryDetailsAsync(int employeeId, int year, int month)
        {
            var employee = await _dbcontext.Employees
                .FirstOrDefaultAsync(e => e.Id == employeeId && e.Active);

            if (employee == null) return null;

            var otMaster = await _dbcontext.OvertimeMaster
                .FirstOrDefaultAsync(x => x.Id == employee.OTtype);

            var attendances = await _dbcontext.MarkAttendance
                .Where(a => a.EmployeeId == employeeId && a.AttendanceDate.Year == year && a.AttendanceDate.Month == month)
                .ToListAsync();

            var overtimes = await _dbcontext.OvertimeRecords
                .Where(o => o.EmployeeId == employeeId && o.Date.HasValue && o.Date.Value.Year == year && o.Date.Value.Month == month)
                .ToListAsync();

            int workingDays = attendances.Select(a => a.AttendanceDate.Date).Distinct().Count();
            int presentDays = attendances.Count(a => a.AttendanceStatus == "Present");
            int leaveDays = attendances.Count(a => a.AttendanceStatus == "Leave");
            int absentDays = workingDays - presentDays - leaveDays;

            decimal totalOtHours = overtimes.Sum(x => x.TotalOvertimeHours ?? 0);
            decimal ratePerHour = otMaster?.RatePerHour ?? 0;
            decimal overtimePay = totalOtHours * ratePerHour;

            var salaryModel = new SalaryViewModel
            {
                EmployeeId = employee.Id,
                EmployeeName = employee.Name,
                Year = year,
                Month = month,
                PaymentDate = DateTime.Now,
                BaseSalary = employee.BaseSalary,
                WorkingDays = workingDays,
                PresentDays = presentDays,
                LeaveDays = leaveDays,
                AbsentDays = absentDays,
                OvertimeHours = totalOtHours,
                OvertimePay = overtimePay,
                Bonus = 0, // Future use
                Advance = 0,
                Deduction = 0,
                NetSalary = employee.BaseSalary + overtimePay, // Bonus - Advance - Deduction handled while saving
                IsPaid = false,
                OTtype = employee.OTtype
            };

            return salaryModel;
        }

        public async Task SaveSalaryAsync(SalaryViewModel model)
        {
            var salary = new Salaries
            {
                EmployeeId = model.EmployeeId,
                Year = model.Year,
                Month = model.Month,
                PaymentDate = model.PaymentDate,
                BaseSalary = model.BaseSalary,
                WorkingDays = model.WorkingDays,
                PresentDays = model.PresentDays,
                AbsentDays = model.AbsentDays,
                LeaveDays = model.LeaveDays,
                OvertimeHours = model.OvertimeHours,
                OvertimePay = model.OvertimePay,
                Bonus = model.Bonus,
                Advance = model.Advance,
                Deduction = model.Deduction,
                NetSalary = model.BaseSalary + model.OvertimePay + model.Bonus - model.Advance - model.Deduction,
                Active = true,
                AddedOn = DateTime.Now,
                AddedBy = "USR0002", // You can pass this dynamically
                IsPaid = model.IsPaid
            };

            _dbcontext.Salaries.Add(salary);
            await _dbcontext.SaveChangesAsync();
        }
    }
}
