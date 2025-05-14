using Common.Core.ViewModels;
using Common.Data.Context;
using Common.Data.Models;
using Common.Data.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Common.Core.Services
{
    public interface IUsersService
    {
        Task<List<Users>> GetAllUsers();
        int GetUserCount();
        int CreateUser(UsersModel users);
        bool UpdateUser(int id, UsersModel users);
        Users GetUsersDetails(int? Id);
    }
    public class UsresService : IUsersService
    {
        private readonly IGenericRepository<Users> _repository;
        private readonly LogisticContext _dbcontext;
        private readonly IContextHelper _contextHelper;


        public UsresService(IGenericRepository<Users> repository, LogisticContext dbcontext, IContextHelper contextHelper)
        {
            _repository = repository;
            _dbcontext = dbcontext;
            _contextHelper = contextHelper;
        }
        public async Task<List<Users>> GetAllUsers()
        {
            try
            {
                return await _dbcontext.Users
                    .Where(x => x.Active)
                    .ToListAsync(); 
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public int GetUserCount()
        {
            try
            {
                var lastUser = _dbcontext.Users.OrderByDescending(u => u.Id).FirstOrDefault();

                if (lastUser != null)
                {
                    return lastUser.Id;
                }

                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching last user ID", ex);
            }
        }

        public int CreateUser(UsersModel entity)
        {
            try
            {
                var userId = _contextHelper.GetUsername();
                Users logistic = new Users();
                logistic.Active = true;
                logistic.AddedBy = userId;
                logistic.AddedOn = DateTime.Now;
                logistic.UpdatedBy = userId;
                logistic.UpdatedOn = DateTime.Now;
                logistic.Username = entity.Username;
                logistic.UserId = entity.UserId;
                logistic.Password = entity.Password;
                logistic.Role = entity.Role;
                _dbcontext.Users.Add(logistic);
                _dbcontext.SaveChanges();
                return logistic.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool UpdateUser(int id, UsersModel model)
        {
            try
            {
                var userId = _contextHelper.GetUsername();
                var entity = _dbcontext.Users.Where(x => x.Id == id).SingleOrDefault();
                if (entity == null)
                {
                    return false;
                }
                entity.UpdatedBy = userId;
                entity.UpdatedOn = DateTime.Now;
                entity.Active = model.isActive;
                entity.Username = model.Username;
                entity.Password = model.Password;
                entity.Role = model.Role;
                _dbcontext.Update(entity);
                _dbcontext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Users GetUsersDetails(int? Id)
        {
            try
            {
                var result = new Users();
                var data = _dbcontext.Users.Where(x => x.Id == Id);
                foreach (var user in data)
                {
                    result.Id = user.Id;
                    result.Active = user.Active;
                    result.Username = user.Username;
                    result.Password = user.Password;
                    result.Role = user.Role;
                    _dbcontext.Add(result);
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
