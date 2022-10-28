﻿using AutoMapper;
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
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;

        public UserController(IUserService userService, IMapper mapper, IUriService uriService)
        {
            _userService = userService;
            _mapper = mapper;
            _uriService = uriService;
        }

        /// <summary>
        /// Retrieve all users
        /// </summary>
        /// <param name="filters">Filters to apply</param>
        /// <returns></returns>
        [HttpGet(Name = nameof(GetUsers))]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<UserDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetUsers([FromQuery]QueryFilter filters)
        {
            var users = _userService.GetUsers(filters);
            var usersDtos = _mapper.Map<IEnumerable<UserDto>>(users);

            var metadata = new Metadata
            {
                TotalCount = users.TotalCount,
                PageSize = users.PageSize,
                CurrentPage = users.CurrentPage,
                TotalPages = users.TotalPages,
                HasNextPage = users.HasNextPage,
                HasPreviousPage = users.HasPreviousPage,
                NextPageUrl = "",
                PreviousPageUrl = ""
            };

            var response = new ApiResponse<IEnumerable<UserDto>>(usersDtos)
            {
                Meta = metadata
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUser(id);
            var userDto = _mapper.Map<UserDto>(user);
            var response = new ApiResponse<UserDto>(userDto);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> User(UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);

            await _userService.InsertUser(user);

            userDto = _mapper.Map<UserDto>(user);
            var response = new ApiResponse<UserDto>(userDto);
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> Put(int id, UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);
            user.Id = id;

            var result = await _userService.UpdateUser(user);
            var response = new ApiResponse<bool>(result);
            return Ok(response);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.DeleteUser(id);
            var response = new ApiResponse<bool>(result);
            return Ok(response);
        }
    }
}