using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test1Api.Models;

namespace Test1Api.Utils
{
    public class UserSeeder
    {
        private readonly TodoContext _db;
        public UserSeeder(TodoContext db)
        {
            _db = db;
        }
        public void SeedData()
        {
            if(_db.Users.Count() == 0)
            {
                _db.Users.Add(new User
                {
                    UserName = "admin",
                    Password = Helper.GenHash("123456"),
                    FullName = "Administrator",
                    Role = "Admin"
                });
                _db.Users.Add(new User
                {
                    UserName = "member",
                    Password = Helper.GenHash("123456"),
                    FullName = "Member",
                    Role = "Member"
                });
                _db.SaveChanges();
            }
        }
    }
}
