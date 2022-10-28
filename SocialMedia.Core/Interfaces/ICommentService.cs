using SocialMedia.Core.CustomEntities;
using SocialMedia.Core.Entities;
using SocialMedia.Core.QueryFilters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialMedia.Core.Interfaces
{
    public interface ICommentService
    {
        PagedList<Comment> GetComments(PostQueryFilter filters);

        Task<Comment> GetComment(int id);
        Task InsertComment(Comment comment, int postId);

       Task<bool> UpdateComment(Comment post);

        Task<bool> DeleteComment(int id);
    }
}