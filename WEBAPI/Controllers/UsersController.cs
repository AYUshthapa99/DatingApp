using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEBAPI.Data;
using WEBAPI.Entities;

namespace WEBAPI.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
    
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult <IEnumerable<AppUser>>> Getusers()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }
         [Authorize]
          [HttpGet("{id}")]
        public async Task<ActionResult <AppUser>> GetuserbyId(int id)
        {
           return await _context.Users.FindAsync(id);
            
        }
    }
}