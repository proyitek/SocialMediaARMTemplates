using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SocialMedia.Api.Responses;
using SocialMedia.Core.CustomEntities;
using SocialMedia.Core.DTOs;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Interfaces;
using SocialMedia.Core.QueryFilters;
using SocialMedia.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SocialMedia.Api.Controllers
{
    // [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;

        public CommentController(ICommentService commentService, IMapper mapper, IUriService uriService)
        {
            _commentService = commentService;
            _mapper = mapper;
            _uriService = uriService;
        }

        /// <summary>
        /// Retrieve all comments
        /// </summary>
        /// <param name="filters">Filters to apply</param>
        /// <returns></returns>
        [HttpGet(Name = nameof(GetComments))]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<CommentDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetComments([FromQuery]PostQueryFilter filters)
        {
            var comments = _commentService.GetComments(filters);
            var commentsDtos = _mapper.Map<IEnumerable<CommentDto>>(comments);

            var metadata = new Metadata
            {
                TotalCount = comments.TotalCount,
                PageSize = comments.PageSize,
                CurrentPage = comments.CurrentPage,
                TotalPages = comments.TotalPages,
                HasNextPage = comments.HasNextPage,
                HasPreviousPage = comments.HasPreviousPage,
                NextPageUrl = _uriService.GetPostPaginationUri(filters, Url.RouteUrl(nameof(GetComments))).ToString(),
                PreviousPageUrl = _uriService.GetPostPaginationUri(filters, Url.RouteUrl(nameof(GetComments))).ToString()
            };

            var response = new ApiResponse<IEnumerable<CommentDto>>(commentsDtos)
            {
                Meta = metadata
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetComment(int id)
        {
            var comment = await _commentService.GetComment(id);
            var commentDto = _mapper.Map<CommentDto>(comment);
            var response = new ApiResponse<CommentDto>(commentDto);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Comment(CommentDto commentDto,int postId)
        {
            var comment = _mapper.Map<Comment>(commentDto);

            await _commentService.InsertComment(comment,postId);

            commentDto = _mapper.Map<CommentDto>(comment);
            var response = new ApiResponse<CommentDto>(commentDto);
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> Put(int id, CommentDto commentDto)
        {
            var comment = _mapper.Map<Comment>(commentDto);
            comment.Id = id;

            var result = await _commentService.UpdateComment(comment);
            var response = new ApiResponse<bool>(result);
            return Ok(response);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _commentService.DeleteComment(id);
            var response = new ApiResponse<bool>(result);
            return Ok(response);
        }
    }
}