using Common.Data.Context;
using Common.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Core.Services
{
    public interface ILeaveService
    {
        Task<List<LeaveRequests>> GetAllLeaveRequests();
        Task<LeaveRequests> GetLeaveRequestById(int id);
        Task<bool> ApplyLeave(LeaveRequests leave);
        Task<bool> ApproveLeave(int id, string approvedBy);
        Task<bool> RejectLeave(int id, string rejectedBy);
    }
    public class LeaveService : ILeaveService
    {
        private readonly LogisticContext _dbcontext;
        private readonly IContextHelper _contextHelper;

        public LeaveService(LogisticContext dbcontext, IContextHelper contextHelper)
        {
            _dbcontext = dbcontext;
            _contextHelper = contextHelper;
        }

        public async Task<List<LeaveRequests>> GetAllLeaveRequests()
        {
            var leaveRequests = await _dbcontext.LeaveRequests.ToListAsync();

            return leaveRequests;
        }

        public async Task<LeaveRequests> GetLeaveRequestById(int id)
        {
            return await _dbcontext.LeaveRequests.FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<bool> ApplyLeave(LeaveRequests leave)
        {
            var userId = _contextHelper.GetUsername();
            var newRequest = new LeaveRequests
            {
                EmployeeId = leave.EmployeeId,
                LeaveType = leave.LeaveType,
                StartDate = leave.StartDate,
                TotalDays = leave.TotalDays,
                EndDate = leave.EndDate,
                Status = "Pending",
                Reason = leave.Reason,
                AddedBy = "admin",
                UpdatedBy = "admin",
                AddedOn = DateTime.Now,
                UpdatedOn = DateTime.Now,
                ApprovedBy ="admin",
                ApprovedOn = DateTime.Now,
            };

            _dbcontext.LeaveRequests.Add(newRequest);
            await _dbcontext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ApproveLeave(int id, string approvedBy)
        {
            var request = await _dbcontext.LeaveRequests.FirstOrDefaultAsync(l => l.Id == id);
            if (request == null) return false;

            request.Status = "Approved";
            request.ApprovedBy = approvedBy;
            request.ApprovedOn = DateTime.Now;

            _dbcontext.LeaveRequests.Update(request);
            await _dbcontext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectLeave(int id, string rejectedBy)
        {
            var request = await _dbcontext.LeaveRequests.FirstOrDefaultAsync(l => l.Id == id);
            if (request == null) return false;

            request.Status = "Rejected";
            request.ApprovedBy = rejectedBy;
            request.ApprovedOn = DateTime.Now;

            _dbcontext.LeaveRequests.Update(request);
            await _dbcontext.SaveChangesAsync();
            return true;
        }
    }
}
