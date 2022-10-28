using Microsoft.Extensions.Options;
using SocialMedia.Core.CustomEntities;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Exceptions;
using SocialMedia.Core.Interfaces;
using SocialMedia.Core.QueryFilters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.Core.Services
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOptions _paginationOptions;

        public CommentService(IUnitOfWork unitOfWork, IOptions<PaginationOptions> options)
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
        }

        public async Task<Comment> GetComment(int id)
        {
            return await _unitOfWork.CommentRepository.GetById(id);
        }

        public PagedList<Comment> GetComments(PostQueryFilter filters)
        {
            filters.PageNumber = filters.PageNumber == 0 ? _paginationOptions.DefaultPageNumber : filters.PageNumber;
            filters.PageSize = filters.PageSize == 0 ? _paginationOptions.DefaultPageSize : filters.PageSize;

            var comments = _unitOfWork.CommentRepository.GetAll();

            if(filters.UserId != null)
            {
                comments = comments.Where(x => x.UserId == filters.UserId);
            }

            if (filters.Date != null)
            {
                comments = comments.Where(x => x.Date.ToShortDateString() == filters.Date?.ToShortDateString());
            }

            if (filters.Description != null)
            {
                comments = comments.Where(x => x.Description.ToLower().Contains(filters.Description.ToLower()));
            }

            var pagedComments = PagedList<Comment>.Create(comments, filters.PageNumber, filters.PageSize);
            return pagedComments;
        }

        public async Task InsertComment(Comment comment,int postId)
        {
            var user = await _unitOfWork.UserRepository.GetById(comment.UserId);
            if (user == null)
            {
                throw new BusinessException("User doesn't exist");
            }
            var post = await _unitOfWork.PostRepository.GetPostsByUser(user.Id);

            var findPostId = post.FirstOrDefault(x => x.Id == postId).Id;
          
            if (findPostId!=null)
            {
                var userComment = await _unitOfWork.CommentRepository.GetCommentsByUserPost(user.Id, findPostId); 
                if (userComment.Count() < 10)
                {
                    var lastComment = userComment.OrderByDescending(x=> x.Date).FirstOrDefault();
                    if (lastComment != null)
                    {
                        if ((DateTime.Now - lastComment.Date).TotalDays < 7)
                        {
                        
                            throw new BusinessException("You are not able to publish the comment");
                        }
                    
                    };
                }
            
                if (comment.Description.Contains("Sexo"))
                {
                    throw new BusinessException("Content not allowed");
                }

                await _unitOfWork.CommentRepository.Add(comment);
                await _unitOfWork.SaveChangesAsync();
            }
            
        }

        public async Task<bool> UpdateComment(Comment comment)
        {
            var existingComment = await _unitOfWork.CommentRepository.GetById(comment.Id);
            existingComment.Date = comment.Date;
            existingComment.Description = comment.Description;

            _unitOfWork.CommentRepository.Update(existingComment);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteComment(int id)
        {
            await _unitOfWork.CommentRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
