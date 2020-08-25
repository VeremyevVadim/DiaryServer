using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FoodDiary.Models;
using Nancy.Json;
using MySql.Data.MySqlClient;
using System.Data;

namespace FoodDiary.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        string connectionString = @"Server=localhost;Database=fooddiary;Uid=root;";

        [Route("token")]
        [HttpPost]
        public IActionResult Token([FromBody]User person)
        {

            var identity = GetIdentity(person.Login, person.Password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            JavaScriptSerializer serializerJS = new JavaScriptSerializer();
            string responseString = serializerJS.Serialize(response);

            return Ok(response);
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            List<User> people = new List<User>();
            try
            {
                using (MySqlConnection sqlConnection = new MySqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    MySqlDataAdapter sqlDa = new MySqlDataAdapter("selectUser", sqlConnection);
                    sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                    sqlDa.SelectCommand.Parameters.AddWithValue("_Login", username);

                    DataTable dtbl = new DataTable();
                    sqlDa.Fill(dtbl);

                    foreach (DataRow row in dtbl.Rows)
                    {
                        var cells = row.ItemArray;
                        people.Add(new User(
                            (string)cells[0],
                            (string)cells[1]));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            User person = people.FirstOrDefault(x => x.Login == username && x.Password == password);
            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Login),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }

        [Route("reg")]
        [HttpPost]
        public IActionResult Registration([FromBody]User person)
        {

            var identity = GetIdentity(person.Login, person.Password);
            if (identity == null)
            {
                try
                {
                    using (MySqlConnection sqlConnection = new MySqlConnection(connectionString))
                    {
                        sqlConnection.Open();
                        MySqlCommand sqlCmd = new MySqlCommand("addUser", sqlConnection);
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("_Login", person.Login);
                        sqlCmd.Parameters.AddWithValue("_Password", person.Password);
                        sqlCmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                return Token(person);
            }
            else 
            {
                return BadRequest(new { errorText = "User exists." });
            }

        }

    }
}