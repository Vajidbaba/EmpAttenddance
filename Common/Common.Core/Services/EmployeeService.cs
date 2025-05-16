using Common.Data.Context;
using Common.Data.Models;
using Common.Data.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Common.Core.Services
{
    public interface IEmployeeService
    {
        Task<List<EmployeeModel>> GetAllEmployees();
        int GetEmployeeCount();
        int CreateEmployee(EmployeeModel employee);
        bool UpdateEmployee(int id, EmployeeModel employee, string userId);
        EmployeeModel GetEmployeeDetails(int? Id);
        Task<EmployeeModel?> GetEmployeeById(int id);

        public class EmployeeService : IEmployeeService
        {
            private readonly IGenericRepository<EmployeeModel> _repository;
            private readonly LogisticContext _dbcontext;
            private readonly IContextHelper _contextHelper;

            public EmployeeService(IGenericRepository<EmployeeModel> repository, LogisticContext dbcontext, IContextHelper contextHelper)
            {
                _repository = repository;
                _dbcontext = dbcontext;
                _contextHelper = contextHelper;
            }

            public async Task<List<EmployeeModel>> GetAllEmployees()
            {
                try
                {
                    return await _dbcontext.Employees.ToListAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public async Task<EmployeeModel?> GetEmployeeById(int id)
            {
                return await _dbcontext.Employees.FirstOrDefaultAsync(e => e.Id == id);
            }

            public int GetEmployeeCount()
            {
                try
                {
                    var lastEmployee = _dbcontext.Employees.OrderByDescending(e => e.Id).FirstOrDefault();
                    return lastEmployee?.Id ?? 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error fetching last employee ID", ex);
                }
            }

            public int CreateEmployee(EmployeeModel entity)
            {
                try
                {
                    var userId = _contextHelper.GetUsername();
                    EmployeeModel employee = new EmployeeModel
                    {
                        Active = true,
                        AddedBy = userId,
                        AddedOn = DateTime.Now,
                        Name = entity.Name,
                        FatherName = entity.FatherName,
                        Email = entity.Email,
                        Contact = entity.Contact,
                        Department = entity.Department,
                        Role = entity.Role,
                        DateOfJoining = entity.DateOfJoining,
                        DateOfResign = entity.DateOfResign,
                        City = entity.City,
                        State = entity.State,
                        Pin = entity.Pin,
                        Address = entity.Address,
                    };
                    _dbcontext.Employees.Add(employee);
                    _dbcontext.SaveChanges();
                    return employee.Id;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public bool UpdateEmployee(int id, EmployeeModel model, string userId)
            {
                try
                {
                    var entity = _dbcontext.Employees.SingleOrDefault(x => x.Id == id);
                    if (entity == null) return false;
                    entity.UpdatedBy = userId;
                    entity.UpdatedOn = DateTime.Now;
                    entity.Active = model.Active;
                    entity.Name = model.Name;
                    entity.FatherName = model.FatherName;
                    entity.Email = model.Email;
                    entity.Contact = model.Contact;
                    entity.Department = model.Department;
                    entity.Role = model.Role;
                    entity.DateOfJoining = model.DateOfJoining;
                    entity.DateOfResign = model.DateOfResign;
                    entity.City = model.City;
                    entity.State = model.State;
                    entity.Pin = model.Pin;
                    entity.Address = model.Address;
                    _dbcontext.Update(entity);
                    _dbcontext.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public EmployeeModel GetEmployeeDetails(int? Id)
            {
                try
                {
                    return _dbcontext.Employees.FirstOrDefault(x => x.Id == Id);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
