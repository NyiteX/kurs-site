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
using System.Globalization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace kursach_4._12._23.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        IConfiguration _configuration;

        public CartController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        // GET: api/<ValuesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<JsonResult> Get(int id)
        {
            string query = "SELECT * FROM [Cart] WHERE UserID = @id";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("DefaultConnection");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id", id);
                    myReader = await myCommand.ExecuteReaderAsync();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

        [HttpGet("{id}/Details")]
        [Authorize]
        public async Task<JsonResult> GetDetails(int id)
        {
            string query = @"
        SELECT [User].Name AS UserName, [Product].Name AS ProductName, [Cart].Count
        FROM [Cart]
        JOIN [User] ON [Cart].UserID = [User].ID
        JOIN [Product] ON [Cart].ProductID = [Product].ID
        WHERE [User].ID = @id";

            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("DefaultConnection");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id", id);
                    myReader = await myCommand.ExecuteReaderAsync();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }


        // POST api/<ValuesController>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromForm] CartModel cartModel)
        {
            try
            {
                string query = "INSERT INTO [Cart] (ProductID, UserID, Count) VALUES (@productID, @userID, @count)";
                string sqlDatasource = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection myCon = new SqlConnection(sqlDatasource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@productID", cartModel.ProductID);
                        myCommand.Parameters.AddWithValue("@userID", cartModel.UserID);
                        myCommand.Parameters.AddWithValue("@count", cartModel.Count);

                        await myCommand.ExecuteNonQueryAsync();
                        myCon.Close();
                    }
                }
                return Ok("Product added successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error adding product: {ex.Message}");
            }
        }

        // PUT api/<ValuesController>/5
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Put([FromForm] CartModel cartModel)
        {
            try
            {
                string query = "UPDATE [Cart] SET Count = @Count WHERE ID = @Id";
                string sqlDatasource = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection myCon = new SqlConnection(sqlDatasource))
                {
                    await myCon.OpenAsync();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@Id", cartModel.ID);
                        myCommand.Parameters.AddWithValue("@Count", cartModel.Count);

                        myCommand.ExecuteNonQuery();
                    }
                }

                await Console.Out.WriteLineAsync("Cart updated successfully");
                return Ok("Cart updated successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating Cart: {ex}");
                return BadRequest($"Error updating Cart: {ex.Message}");
            }
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                string query = "DELETE FROM [Cart] WHERE UserID = @id";
                string sqlDatasource = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection myCon = new SqlConnection(sqlDatasource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@id", id);

                        await myCommand.ExecuteNonQueryAsync();

                        myCon.Close();
                    }
                }
                return Ok("Cart cleared successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error clearing cart: {ex.Message}");
            }
        }
    }
}
