using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEBAPI.Data;
using WEBAPI.DTOs;
using WEBAPI.Entities;
using WEBAPI.Interface;

namespace WEBAPI.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }
        
        [HttpPost("register")]

        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username))
            {
                return BadRequest("User already Exists");
            }
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            return new UserDto
            {
                Username =user.UserName,
                Token =_tokenService.CreateToken(user)
            };
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>>Login(LoginDtos loginDtos)
        {
            var user    = await _context.Users
            .SingleOrDefaultAsync(x =>x.UserName == loginDtos.Username);

            if(user == null) return Unauthorized("Invalid Username");

            using var hmac = new  HMACSHA512(user.PasswordSalt);

            var ComputeHash  = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDtos.Password));

            for (var i=0; i< ComputeHash.Length; i++)
            {
                if(ComputeHash[i] != user.PasswordHash[i] ) return Unauthorized("Invalid Password");
            }



            return new UserDto
            {
                Username =user.UserName,
                Token =_tokenService.CreateToken(user)
            };

            

        }  
        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x=> x.UserName == username.ToLower());
        }

    }
}