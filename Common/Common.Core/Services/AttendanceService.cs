using Common.Core.ViewModels;
using Common.Data.Context;
using Common.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Web.WebPages;

namespace Common.Core.Services
{
    public interface IAttendanceService
    {
        Task<List<AttendanceModel>> GetAllAttendanceRecords();
        Task<AttendanceModel> GetAttendanceById(int id);
        Task<bool> AddAttendance(AttendanceModel attendance);
        Task<bool> UpdateAttendance(int id, AttendanceModel attendance);
        DataTableResultModel GetDataTable(DataTableModel model);

        //new
        Task SaveOrUpdateAttendance(AttendanceFormViewModel model, string userId);
        Task<MarkAttendance?> GetTodayAttendanceAsync(int Id);
        Task<List<EmployeeWithAttendanceStatus>> GetEmployeesWithTodayAttendanceAsync();
        Task<List<MarkAttendance>> GetAttendanceSummery();
        Task<OvertimeRecords?> GetTodayOvertime(int Id);
    }
    public class AttendanceService : IAttendanceService
    {
        private readonly LogisticContext _dbcontext;
        private readonly IContextHelper _contextHelper;
        private readonly IConfiguration _configuration;

        public AttendanceService(LogisticContext dbcontext, IContextHelper contextHelper, IConfiguration configration)
        {
            _dbcontext = dbcontext;
            _configuration = configration;
            _contextHelper = contextHelper;
        }
        public async Task SaveOrUpdateAttendance(AttendanceFormViewModel model, string userId)
        {
            // Attendance Save/Update
            var attendance = await _dbcontext.MarkAttendance.FirstOrDefaultAsync(a => a.EmployeeId == model.EmployeeId && a.AttendanceDate.Date == model.AttendanceDate.Date);

            if (attendance == null)
            {
                attendance = new MarkAttendance
                {
                    AddedBy = userId,
                    EmployeeId = model.EmployeeId,
                    AttendanceDate = model.AttendanceDate,
                    AttendanceStatus = model.AttendanceStatus,
                };
                _dbcontext.MarkAttendance.Add(attendance);
            }
            else
            {
                attendance.UpdatedBy = userId;
                attendance.UpdatedOn = DateTime.Now;
                attendance.AttendanceStatus = model.AttendanceStatus;
                _dbcontext.MarkAttendance.Update(attendance);
            }

            // Overtime Save/Update
            if (model.TotalOvertimeHours > 0)
            {
                var overtime = await _dbcontext.OvertimeRecords.FirstOrDefaultAsync(o => o.EmployeeId == model.EmployeeId && o.Date.Value.Date == model.AttendanceDate.Date);
                if (overtime == null)
                {
                    overtime = new OvertimeRecords
                    {
                        EmployeeId = model.EmployeeId,
                        Date = model.AttendanceDate,
                        TotalOvertimeHours = model.TotalOvertimeHours,
                        AdvancePay = model.AdvancePay,
                        Bonus = model.Bonus,
                        Deducation = model.Deducation,
                        AddedBy = userId,
                        AddedOn = DateTime.Now,
                        Active = true
                    };
                    _dbcontext.OvertimeRecords.Add(overtime);
                }
                else
                {
                    overtime.TotalOvertimeHours = model.TotalOvertimeHours;
                    overtime.AdvancePay = model.AdvancePay;
                    overtime.Bonus = model.Bonus;
                    overtime.Deducation = model.Deducation;
                    overtime.UpdatedBy = userId;
                    overtime.UpdatedOn = DateTime.Now;
                    _dbcontext.OvertimeRecords.Update(overtime);
                }
            }
            if (model.LeaveType != null && model.TotalDays > 0)
            {
                var leave = await _dbcontext.Leaves.FirstOrDefaultAsync(l => l.EmployeeId == model.EmployeeId && l.StartDate <= model.AttendanceDate.Date && l.EndDate >= model.AttendanceDate.Date);
                if (leave == null)
                {
                    leave = new Leaves
                    {
                        EmployeeId = model.EmployeeId,
                        LeaveId = model.LeaveType,
                        TotalDays = model.TotalDays,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        Reason = model.Reason,
                        ApplyDate = model.AttendanceDate,
                        AddedBy = userId,
                        Status = "Pending",
                        AddedOn = DateTime.Now,
                        Active = true
                    };
                    _dbcontext.Leaves.Add(leave);
                }
                else
                {
                    leave.LeaveId = model.LeaveType;
                    leave.TotalDays = model.TotalDays;
                    leave.StartDate = model.StartDate;
                    leave.EndDate = model.EndDate;
                    leave.Reason = model.Reason;
                    leave.UpdatedBy = userId;
                    leave.UpdatedOn = DateTime.Now;
                    _dbcontext.Leaves.Update(leave);
                }
            }
            await _dbcontext.SaveChangesAsync();
        }

        public async Task<MarkAttendance?> GetTodayAttendanceAsync(int Id)
        {
            return await _dbcontext.MarkAttendance
                .FirstOrDefaultAsync(a => a.EmployeeId == Id && a.AttendanceDate.Date == DateTime.Today);
        }
        public async Task<OvertimeRecords?> GetTodayOvertime(int Id)
        {
            return await _dbcontext.OvertimeRecords.FirstOrDefaultAsync(a =>
                    a.EmployeeId == Id &&
                    a.Date.HasValue &&
                    a.Date.Value.Date == DateTime.Today);
        }

        public async Task<List<MarkAttendance>> GetAttendanceSummery()
        {
            return await _dbcontext.MarkAttendance.Where(x => x.Active).ToListAsync();
        }


        public async Task<List<AttendanceModel>> GetAllAttendanceRecords()
        {
            return await _dbcontext.Attendance.Where(x => x.Active).ToListAsync();
        }
        public async Task<AttendanceModel> GetAttendanceById(int id)
        {
            return await _dbcontext.Attendance.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<List<EmployeeWithAttendanceStatus>> GetEmployeesWithTodayAttendanceAsync()
        {
            var today = DateTime.Today;

            var data = await (from e in _dbcontext.Employees
                              where e.Active
                              join a in _dbcontext.MarkAttendance.Where(x => x.AttendanceDate.Date == today)
                                  on e.Id equals a.EmployeeId into attendanceGroup
                              from att in attendanceGroup.DefaultIfEmpty()
                              select new EmployeeWithAttendanceStatus
                              {
                                  Id = e.Id,
                                  Name = e.Name,
                                  Contact = e.Contact,
                                  AttendanceStatus = att != null ? att.AttendanceStatus : "Not Marked"
                              }).ToListAsync();

            return data;
        }
        public async Task<bool> AddAttendance(AttendanceModel attendance)
        {
            var userId = _contextHelper.GetUsername();

            bool alreadyExists = await _dbcontext.Attendance.AnyAsync(a => a.EmployeeId == attendance.EmployeeId && a.Date == attendance.Date);

            if (alreadyExists)
            {
                return false;
            }
            decimal workHours = 0;
            if (attendance.CheckInTime.HasValue && attendance.CheckOutTime.HasValue)
            {
                workHours = Convert.ToDecimal((attendance.CheckOutTime.Value - attendance.CheckInTime.Value).TotalHours);
            }

            var newRecord = new AttendanceModel
            {
                EmployeeId = attendance.EmployeeId,
                Date = attendance.Date,
                CheckInTime = attendance.CheckInTime,
                CheckOutTime = attendance.CheckOutTime,
                WorkHours = workHours,
                Status = attendance.Status,
                Active = true,
                AddedBy = userId,
                AddedOn = DateTime.Now,
                UpdatedBy = userId,
                UpdatedOn = DateTime.Now
            };

            _dbcontext.Attendance.Add(newRecord);
            await _dbcontext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateAttendance(int id, AttendanceModel attendance)
        {
            var record = await _dbcontext.Attendance.FirstOrDefaultAsync(x => x.Id == id);
            if (record == null) return false;

            var userId = _contextHelper.GetUsername();

            record.CheckInTime = attendance.CheckInTime;
            record.CheckOutTime = attendance.CheckOutTime;

            if (attendance.CheckInTime.HasValue && attendance.CheckOutTime.HasValue)
            {
                record.WorkHours = Convert.ToDecimal((attendance.CheckOutTime.Value - attendance.CheckInTime.Value).TotalHours);
            }
            else
            {
                record.WorkHours = 0;
            }

            record.Status = attendance.Status;
            record.UpdatedBy = userId;
            record.UpdatedOn = DateTime.Now;

            _dbcontext.Attendance.Update(record);
            await _dbcontext.SaveChangesAsync();
            return true;
        }
        public DataTableResultModel GetDataTable(DataTableModel model)
        {
            var data = GetDataTables(model);
            int recordTotal = data.Count > 0 ? data.Select(x => x.TotalCount).FirstOrDefault() : 0;

            var result = new DataTableResultModel
            {
                draw = model.draw,
                recordsFiltered = recordTotal,
                recordsTotal = recordTotal,
                data = data
            };
            return result;
        }
        private List<DataTableModel> GetDataTables(DataTableModel model)
        {
            List<DataTableModel> result = new List<DataTableModel>();

            var _connectionString = _configuration.GetConnectionString("Sql");

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetAttendanceDataTable", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Start", model.start);
                    command.Parameters.AddWithValue("@Length", model.length);
                    command.Parameters.AddWithValue("@SearchValue", model.searchValue ?? "");
                    command.Parameters.AddWithValue("@StartDate", model.StartDate);
                    command.Parameters.AddWithValue("@EndDate", model.EndDate);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new DataTableModel
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                CheckInTime = reader["CheckInTime"].ToString(),
                                CheckOutTime = reader["CheckOutTime"].ToString(),
                                WorkHours = reader["WorkHours"].ToString(),
                                Date = reader["Date"].ToString(),
                                Status = reader["Status"].ToString(),
                                TotalCount = Convert.ToInt32(reader["TotalCount"])
                            });
                        }
                    }
                }
            }
            return result;
        }
    }
}
