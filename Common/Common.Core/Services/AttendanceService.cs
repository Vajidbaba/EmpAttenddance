using Common.Core.ViewModels;
using Common.Data.Context;
using Common.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Common.Core.Services
{
    public interface IAttendanceService
    {
        Task<List<AttendanceModel>> GetAllAttendanceRecords();
        Task<AttendanceModel> GetAttendanceById(int id);
        Task<bool> AddAttendance(AttendanceModel attendance);
        Task<bool> UpdateAttendance(int id, AttendanceModel attendance);
        DataTableResultModel GetDataTable(DataTableModel model);
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

        public async Task<List<AttendanceModel>> GetAllAttendanceRecords()
        {
            return await _dbcontext.Attendance.Where(x => x.Active).ToListAsync();
        }

        public async Task<AttendanceModel> GetAttendanceById(int id)
        {
            return await _dbcontext.Attendance.FirstOrDefaultAsync(x => x.Id == id);
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
