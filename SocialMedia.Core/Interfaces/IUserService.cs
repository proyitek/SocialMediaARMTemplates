using SocialMedia.Core.CustomEntities;
using SocialMedia.Core.Entities;
using SocialMedia.Core.QueryFilters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialMedia.Core.Interfaces
{
    public interface IUserService
    {
        PagedList<User> GetUsers(QueryFilter filters);

        Task<User> GetUser(int id);

        Task InsertUser(User post);

        Task<bool> UpdateUser(User post);

        Task<bool> DeleteUser(int id);
    }
}