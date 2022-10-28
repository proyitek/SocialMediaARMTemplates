using Microsoft.EntityFrameworkCore;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Interfaces;
using SocialMedia.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia.Infrastructure.Repositories
{
    public class CommentRepository : BaseRepository<Comment>, ICommentRepository
    {
        public CommentRepository(SocialMediaContext context) : base(context) { }

        public async Task<IEnumerable<Comment>> GetCommentsByUserPost(int userId ,int postId)
        {
            return await _entities.Where(x => x.UserId == userId & x.PostId==postId).ToListAsync();
        }
    }
}
