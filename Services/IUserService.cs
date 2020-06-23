using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using InventoryApi.Entities;
using InventoryApi.Helpers;

namespace InventoryApi.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private List<User> _users = new List<User>
        {
            new User { Id = 1, FirstName = "Steven", LastName = "Smith", Username = "steves", Password = "098F6BCD4621D373CADE4E832627B4F6" },
            new User { Id = 1, FirstName = "Graham", LastName = "Smith", Username = "grahams", Password = "098F6BCD4621D373CADE4E832627B4F6" },
            new User { Id = 1, FirstName = "Ian", LastName = "Smith", Username = "ians", Password = "098F6BCD4621D373CADE4E832627B4F6" },
            new User { Id = 1, FirstName = "Robin", LastName = "Smith", Username = "robins", Password = "098F6BCD4621D373CADE4E832627B4F6" },
            new User { Id = 1, FirstName = "Emily", LastName = "Smith", Username = "emilys", Password = "098F6BCD4621D373CADE4E832627B4F6" },
            new User { Id = 1, FirstName = "Dwayne", LastName = "Smith", Username = "dwaynes", Password = "098F6BCD4621D373CADE4E832627B4F6" },
            new User { Id = 1, FirstName = "Andrew", LastName = "Smith", Username = "andrews", Password = "098F6BCD4621D373CADE4E832627B4F6" },
            new User { Id = 1, FirstName = "TEST", LastName = "TEST", Username = "TEST", Password = "TEST" },
        };

    
        public UserService()
        {
        }

        public User Authenticate(string username, string password)
        {
            // var user = _users.SingleOrDefault(x => x.Username == username && x.Password.CreateMD5() == password);
            var user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);
            var secret = "JKSB186VauxhallStreetCMB00200LK";
            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(5), 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            return user.WithoutPassword();
        }

        public IEnumerable<User> GetAll()
        {
            return _users.WithoutPasswords();
        }
    }
}