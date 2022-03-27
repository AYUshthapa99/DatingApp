using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WEBAPI.Entities;

namespace WEBAPI.Interface
{
    public interface ITokenService
    {
      string CreateToken(AppUser user);   
    }
}