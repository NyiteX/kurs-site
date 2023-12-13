using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;
using kursach_4._12._23.Models;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace kursach_4._12._23.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/<UserController>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<JsonResult> Get()
        {
            string query = "SELECT * FROM [User]";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("DefaultConnection");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        //register user
        // POST api/<UserController>
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] UserModel userModel)
        {
            try
            {
                if(!IsUnique(userModel.Name, "name"))
                {
                    return BadRequest("Логин занят.");
                }
                else if(!IsUnique(userModel.Email, "email")) 
                {
                    return BadRequest("Email занят.");
                }

                string query = "INSERT INTO [User] (Name, Password, Email) VALUES (@Name, @Password, @Email)";
                string sqlDatasource = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection myCon = new SqlConnection(sqlDatasource))
                {
                    myCon.Open();
                    
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@Name", userModel.Name);
                        myCommand.Parameters.AddWithValue("@Email", userModel.Email);
                        myCommand.Parameters.AddWithValue("@Password", userModel.Password);


                        if (myCommand.ExecuteNonQuery() > 0)
                        {
                            return Ok("User added successfully");
                        }
                        else
                        {
                            return BadRequest("User not added.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error adding user: {ex.Message}");
            }
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]

        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                string query = "DELETE FROM [User] WHERE ID = @id";
                DataTable table = new DataTable();
                string sqlDatasource = _configuration.GetConnectionString("DefaultConnection");
                SqlDataReader myReader;

                using (SqlConnection myCon = new SqlConnection(sqlDatasource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@id", id);

                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                        myCon.Close();
                    }
                }
                return Ok("User deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting user: {ex.Message}");
            }
        }

        bool IsUnique(string value, string type)
        {
            string query = "SELECT COUNT(*) FROM [User] WHERE Email = @Email";
            if(type == "name")
                query = "SELECT COUNT(*) FROM [User] WHERE Name = @Email";
            string sqlDatasource = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@Email", value);

                    int count = (int)myCommand.ExecuteScalar();

                    if (count == 0)
                        return true;
                    else
                        return false;
                }
            }
        }
    }
}
