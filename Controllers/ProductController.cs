using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;
using kursach_4._12._23.Models;

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
        public JsonResult Get()
        {
            string query = "SELECT * FROM Product";
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

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            string query = "SELECT * FROM Product WHERE ID = @id";
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

            return new JsonResult(table);
        }

        // POST api/<ProductController>
        [HttpPost]
        public IActionResult Post([FromForm] string name, [FromForm] string description, [FromForm] int count, [FromForm] decimal price, [FromForm] string category)
        {
            try
            {
                string query = "INSERT INTO Product(Name, Price, Count, Description, Category) VALUES (@Name, @Count, @Price, @Description, @Category)";
                DataTable table = new DataTable();
                string sqlDatasource = _configuration.GetConnectionString("DefaultConnection");
                SqlDataReader myReader;

                using (SqlConnection myCon = new SqlConnection(sqlDatasource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@Name", name);
                        myCommand.Parameters.AddWithValue("@Price", price);
                        myCommand.Parameters.AddWithValue("@Description", description);
                        myCommand.Parameters.AddWithValue("@Count", count);
                        myCommand.Parameters.AddWithValue("@Category", category);


                        myReader = myCommand.ExecuteReader();
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

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                string query = "DELETE FROM Product WHERE ID = @id";
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
