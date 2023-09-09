using Jobs_Consultation_Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using System.Diagnostics.Metrics;

namespace Jobs_Consultation_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public CountryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("country")]
        public Response Country(AddCountry country)
        {
            Response response = new Response();

            string connectionString = _configuration.GetConnectionString("MySqlConnection");
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string query = "INSERT INTO country (Country_Code, Country_Name)" +
                               "VALUES (@Country_Code, @Country_Name)";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Country_Code", country.Country_Code);
                    cmd.Parameters.AddWithValue("@Country_Name", country.Country_Name);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        response.StatusCode = 200;
                        response.StatusMessage = "Country details added successfully..!";
                    }
                    else
                    {
                        response.StatusCode = 100;
                        response.StatusMessage = "Country details added failed..!";
                    }
                }
            }

            return response;
        }

        [HttpGet]
        [Route("countries")]
        public IActionResult GetCountries()
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection");
            List<GetCountries> countries = new List<GetCountries>();

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string query = "SELECT * FROM country WHERE Status = 1";
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var country = new GetCountries
                            {
                                Country_Id = Convert.ToInt32(reader["Country_Id"]),
                                Country_Code = reader["Country_Code"].ToString(),
                                Country_Name = reader["Country_Name"].ToString(),
                                Status = Convert.ToInt32(reader["Status"])
                            };
                            countries.Add(country);
                        }
                    }
                }
            }

            return Ok(countries);
        }



        [HttpGet]
        [Route("countries/{id}")]
        public IActionResult GetCountry(int id)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection");
            List<GetCountries> countries = new List<GetCountries>();

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string query = "SELECT * FROM country WHERE Country_Id = @Country_Id and Status = 1";
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Country_Id", id);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var country = new GetCountries
                            {
                                Country_Id = Convert.ToInt32(reader["Country_Id"]),
                                Country_Code = reader["Country_Code"].ToString(),
                                Country_Name = reader["Country_Name"].ToString(),
                                Status = Convert.ToInt32(reader["Status"])
                            };
                            countries.Add(country);
                        }
                    }
                }
            }

            return Ok(countries);
        }


        [HttpPut]
        [Route("country/{id}")]
        public IActionResult UpdateCountry(int id, UpdateCountry country)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection");

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string query = "UPDATE country SET Country_Code = @Country_Code, Country_Name = @Country_Name, updated_at = CURRENT_TIMESTAMP() WHERE Country_Id = @Country_Id";


                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Country_Id", id);
                    cmd.Parameters.AddWithValue("@Country_Code", country.Country_Code);
                    cmd.Parameters.AddWithValue("@Country_Name", country.Country_Name);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok("Country details updated successfully!");
                    }
                    else
                    {
                        return NotFound("Country details update failed.");
                    }
                }
            }
        }

        [HttpDelete]
        [Route("country/{id}")]
        public IActionResult DeleteCountry(int id)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection");

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string query = "UPDATE country SET Status = 0, updated_at = CURRENT_TIMESTAMP() WHERE Country_Id = @Country_Id";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Country_Id", id);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok("Country deleted successfully!");
                    }
                    else
                    {
                        return NotFound("Country delete failed.");
                    }
                }
            }
        }


    }

}
