using Api.Configs;
using Api.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Api.Services
{
    public class CommentService
    {
        private readonly IMapper _mapper;
        private readonly DAL.DataContext _context;
        private readonly AuthConfig _config;
		private readonly UserService userService;

        public CommentService(IMapper mapper, IOptions<AuthConfig> config, DataContext context, UserService userService)
        {
            _mapper = mapper;
            _context = context;
            _config = config.Value;
			this.userService = userService;
        }

		
	}
}
