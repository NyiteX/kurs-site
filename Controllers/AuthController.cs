using Microsoft.AspNetCore.Mvc;
using kursach_4._12._23.Models;
using System.Data;
using System.Data.SqlClient;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

        // GET: api/<AuthController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AuthController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
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
                                    string userName = reader["Name"].ToString();
                                    string userEmail = reader["Email"].ToString();
                                    bool isuserAdmin = Convert.ToBoolean(reader["isAdmin"]);
                                    UserModel user = new UserModel
                                    {
                                        Name = userName,
                                        Email = userEmail,
                                        isAdmin = isuserAdmin,
                                    };

                                    var token = GenerateJwtToken(user,user.isAdmin);
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

        // PUT api/<AuthController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AuthController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }


        //
        private string GenerateJwtToken(UserModel user, bool isAdmin)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
            };

            if (isAdmin)
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
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /*private string GenerateJwtToken(UserModel user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            };

            var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }*/
    }

}
