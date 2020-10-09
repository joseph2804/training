using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Test1Api.Models;
using Test1Api.Models.Requests;
using Test1Api.Models.Responses;
using Test1Api.Utils;

namespace Test1Api.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly TodoContext _db;
        private readonly IWebHostEnvironment _hostingEnv;
        public UsersController(TodoContext db, IWebHostEnvironment hostingEnv)
        {
            _db = db;
            _hostingEnv = hostingEnv;
        }
        //[AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<BaseResponse>> Login (LoginRequest login)
        {
            //way 1
            //var user = await _db.users.firstordefaultasync(x => x.username == login.username && x.password == helper.genhash(login.password));
            //if (user == null) return nocontent();
            //return new loginresponse
            //{
            //    id = user.id,
            //    username = user.username,
            //    fullname = user.fullname
            //};
            //way 2
            var user = await _db.Users
                                .Where(x => x.UserName == login.UserName &&
                               x.Password == Helper.GenHash(login.Password))
                                .Select(x => new LoginResponse
                                {
                                    Id = x.Id,
                                    UserName = x.UserName,
                                    FullName = x.FullName,
                                    Role = x.Role
                                })
                                .FirstOrDefaultAsync();
            if (user == null)
            {
                var result = new BaseResponse
                {
                    ErrorCode = 1,
                    Message = "wrong username or password"
                };
                return Unauthorized(result);
            }
            List<Claim> claimData = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Helper.AppKey));
            var signingCredential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Helper.Issuer,
                audience: Helper.Issuer,
                expires: DateTime.Now.AddMinutes(5),
                claims: claimData,
                signingCredentials: signingCredential
                );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            user.Token = tokenString;

            return new BaseResponse {
                Data = user
            };

            
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponse>> UpImage([FromForm]User user)
        {
            var file = user.File;
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            if ( file != null)
            {
                string newFileName = DateTime.Now.Ticks + "_" + file.FileName;
                string path = Path.Combine(_hostingEnv.ContentRootPath, "Data", newFileName);

                using ( var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    user.Avatar = newFileName;
                    _db.Entry(user).Property(x => x.Avatar).IsModified = true;
                    await _db.SaveChangesAsync();
                    user.File = null;
                }

            }
            // update avatar url before returning
            var avatarUrl = Utils.Helper.GetBaseUrl(Request) + "/Data/" + user.Avatar;
            user.Avatar = String.IsNullOrEmpty(user.Avatar) ? null : avatarUrl;
            return new BaseResponse { Data = user };
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<BaseResponse>> UpDateImage(int id, [FromForm] User user)
        {
            var file = user.File;
            var aUser = await _db.Users.FindAsync(id);
            if ( aUser != null )
            {
                if (!String.IsNullOrEmpty(user.Password))
                    aUser.Password = Utils.Helper.GenHash(user.Password); //update password if any
                aUser.FullName = user.FullName;
                aUser.Role = user.Role;
                await _db.SaveChangesAsync();
                if (file != null)
                {
                    string oldAvatar = Path.Combine(_hostingEnv.ContentRootPath, "Data", aUser.Avatar); //keep old file
                    //update new file
                    string newFileName = DateTime.Now.Ticks + "_" + file.FileName;
                    string newPath = Path.Combine(_hostingEnv.ContentRootPath, "Data", newFileName);
                    using (var stream = new FileStream(newPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                        user.Avatar = newFileName;
                        _db.Entry(user).Property(x => x.Avatar).IsModified = true;
                        await _db.SaveChangesAsync();
                        user.File = null;
                        //delete old file
                        if (System.IO.File.Exists(oldAvatar))
                            try { System.IO.File.Delete(oldAvatar); } catch { }
                    }
                }
                var avatarUrl = Utils.Helper.GetBaseUrl(Request) + "/Data/" + user.Avatar;
                user.Avatar = String.IsNullOrEmpty(user.Avatar) ? null : avatarUrl;
                return new BaseResponse { Data = user };
            }
            return NotFound();
        }
        //[HttpDelete("{id}")]
        [HttpGet("getAvatar/{id}")]
        public FileResult GetAvatar( int id)
        {
            var aUser = _db.Users.AsNoTracking().FirstOrDefault(x => x.Id == id);
            if ( aUser != null )
            {
                if ( !String.IsNullOrEmpty(aUser.Avatar))
                {
                    string path = Path.Combine(_hostingEnv.ContentRootPath, "Data", aUser.Avatar);
                    try
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(path);
                        return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, aUser.Avatar);
                    }
                    catch { }
                }
            }
            return null;
        }
    }
}
