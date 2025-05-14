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
        bool UpdateEmployee(int id, EmployeeModel employee);
        EmployeeModel GetEmployeeDetails(int? Id);

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
                    return await _dbcontext.Employees.Where(x => x.Active).ToListAsync();
                }
                catch (Exception)
                {
                    throw;
                }
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
                        UpdatedBy = userId,
                        UpdatedOn = DateTime.Now,
                        Name = entity.Name,
                        Email = entity.Email,
                        Contact = entity.Contact,
                        Department = entity.Department,
                        Role = entity.Role,
                        DateOfJoining = entity.DateOfJoining,
                        SalaryType = entity.SalaryType
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

            public bool UpdateEmployee(int id, EmployeeModel model)
            {
                try
                {
                    var userId = _contextHelper.GetUsername();
                    var entity = _dbcontext.Employees.SingleOrDefault(x => x.Id == id);
                    if (entity == null) return false;

                    entity.UpdatedBy = userId;
                    entity.UpdatedOn = DateTime.Now;
                    entity.Active = model.Active;
                    entity.Name = model.Name;
                    entity.Email = model.Email;
                    entity.Contact = model.Contact;
                    entity.Department = model.Department;
                    entity.Role = model.Role;
                    entity.DateOfJoining = model.DateOfJoining;
                    entity.SalaryType = model.SalaryType;

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
                    return _dbcontext.Employees.SingleOrDefault(x => x.Id == Id);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
