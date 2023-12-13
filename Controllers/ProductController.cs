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
    public class ProductController : ControllerBase
    {
        private IConfiguration _configuration;

        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/<ProductController>
        [HttpGet]
        public async Task<JsonResult> Get()
        {
            string query = "SELECT * FROM [Product]";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("DefaultConnection");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = await myCommand.ExecuteReaderAsync();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            if (table.Rows.Count == 0)
            {
                return new JsonResult(new { error = "Nothing here" });
            }

            return new JsonResult(table);
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<JsonResult> Get(int id)
        {
            string query = "SELECT * FROM [Product] WHERE ID = @id";
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

        // POST api/<ProductController>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Post([FromForm] ProductModel productModel)
        {

            if (!double.TryParse(productModel.Price, NumberStyles.Any, CultureInfo.InvariantCulture, out var convertedPrice))
            {
                return BadRequest("Invalid price format");
            }


            try
            {
                string query = "INSERT INTO [Product] (Name, Price, Count, Description, Category) VALUES (@Name, @Price, @Count, @Description, @Category)";
                DataTable table = new DataTable();
                string sqlDatasource = _configuration.GetConnectionString("DefaultConnection");
                SqlDataReader myReader;

                using (SqlConnection myCon = new SqlConnection(sqlDatasource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@Name", productModel.Name);
                        myCommand.Parameters.AddWithValue("@Price", convertedPrice);
                        myCommand.Parameters.AddWithValue("@Description", productModel.Description);
                        myCommand.Parameters.AddWithValue("@Count", productModel.Count);
                        myCommand.Parameters.AddWithValue("@Category", productModel.Category);


                        myReader = await myCommand.ExecuteReaderAsync();
                        table.Load(myReader);
                        myReader.Close();
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
        [HttpPost("Search")]
        public async Task<JsonResult> Search([FromForm] string word)
        {
            string query = "SELECT * FROM [Product] WHERE Name LIKE '%' + @name + '%'";

            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("DefaultConnection");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@Name", word);

                    myReader = await myCommand.ExecuteReaderAsync();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            if (table.Rows.Count == 0)
            {
                return new JsonResult(new { error = "Nothing here" });
            }

            return new JsonResult(table);
        }


        // PUT api/<ProductController>/5
        [HttpPut]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Put([FromForm] ProductModel productModel)
        {
            if (!double.TryParse(productModel.Price, NumberStyles.Any, CultureInfo.InvariantCulture, out var convertedPrice))
            {
                return BadRequest("Invalid price format");
            }
            await Console.Out.WriteLineAsync(productModel.Name + " ID: " + productModel.ID);
            try
            {
                string query = "UPDATE [Product] SET Name = @Name, Price = @Price, Count = @Count, Description = @Description, Category = @Category WHERE ID = @Id";
                string sqlDatasource = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection myCon = new SqlConnection(sqlDatasource))
                {
                    await myCon.OpenAsync();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@Id", productModel.ID);
                        myCommand.Parameters.AddWithValue("@Name", productModel.Name);
                        myCommand.Parameters.AddWithValue("@Price", convertedPrice);
                        myCommand.Parameters.AddWithValue("@Description", productModel.Description);
                        myCommand.Parameters.AddWithValue("@Count", productModel.Count);
                        myCommand.Parameters.AddWithValue("@Category", productModel.Category);

                        myCommand.ExecuteNonQuery();
                    }
                }

                return Ok("Product updated successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating product: {ex}");
                return BadRequest($"Error updating product: {ex.Message}");
            }
        }



        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                string query = "DELETE FROM [Product] WHERE ID = @id";
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
                return Ok("Product deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting product: {ex.Message}");
            }
        }
    }
}
