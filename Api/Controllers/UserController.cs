using Api.Models;
using Api.Services;
using Api.Services.Abstract;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    // Добавляем [action] для того, чтобы оперировать всеми возможными запросами (глаголами) 
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost]
        public async Task CreateUser(CreateUserModel model) => await userService.CreateUser(model);

        [HttpGet]
        [Authorize]
        public async Task<List<UserModel>> GetUsers() => await userService.GetUsers();

        [HttpGet]
        [Authorize]
        public async Task<UserModel> GetCurrentUser()
        {
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {

                return await userService.GetUser(userId);
            }
            else
                throw new Exception("you are not authorized");

        }
    }
}
