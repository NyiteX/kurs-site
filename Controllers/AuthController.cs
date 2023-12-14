using Microsoft.AspNetCore.Mvc;
using kursach_4._12._23.Models;
using System.Data;
using System.Data.SqlClient;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace kursach_4._12._23.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    
        // POST api/<AuthController>
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] LoginModel loginModel)
        {
            try
            {
                string query = "SELECT * FROM [User] WHERE Name = @Name";
                if (loginModel.Name.Contains("@"))
                {
                    query = "SELECT * FROM [User] WHERE Email = @Name";
                }
                string sqlDatasource = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection myCon = new SqlConnection(sqlDatasource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@Name", loginModel.Name);

                        SqlDataReader reader = myCommand.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string storedPasswordHash = reader["Password"].ToString();

                                bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(loginModel.Password, storedPasswordHash);

                                if (isPasswordCorrect)
                                {
                                    int userID = Convert.ToInt32(reader["ID"]);
                                    string userName = reader["Name"].ToString();
                                    string userEmail = reader["Email"].ToString();
                                    bool isuserAdmin = Convert.ToBoolean(reader["isAdmin"]);
                                    UserModel user = new UserModel
                                    {
                                        ID = userID,
                                        Name = userName,
                                        Email = userEmail,
                                        isAdmin = isuserAdmin,
                                    };

                                    var token = GenerateJwtToken(user);
                                    return Ok(new {Token = token});
                                }
                                else
                                {
                                    return BadRequest("Wrong password.");
                                }
                            }
                        }
                        else
                        {
                            return BadRequest("User not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error login: {ex.Message}");
            }
            return BadRequest("Unexpected error.");
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                

                return Ok(new { message = "Logout successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during logout." });
            }
        }



        //
        private string GenerateJwtToken(UserModel user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString())
            };

            if (user.isAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "admin"));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "user"));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials
            );
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    
    }

}
