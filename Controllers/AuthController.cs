using Microsoft.AspNetCore.Mvc;
using kursach_4._12._23.Models;
using System.Data;
using System.Data.SqlClient;
using Microsoft.CodeAnalysis.Scripting;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace kursach_4._12._23.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        /*readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }*/
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
        public IActionResult Post([FromForm] LoginModel loginModel)
        {
            try
            {
                string query = "SELECT Password FROM [User] WHERE Name = @Name";
                if (loginModel.Name.Contains("@"))
                {
                    query = "SELECT Password FROM [User] WHERE Email = @Name";
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
                                    return Ok("User found.");
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
    }

}
