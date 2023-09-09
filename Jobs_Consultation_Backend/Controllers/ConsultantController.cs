using Jobs_Consultation_Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Jobs_Consultation_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultantController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ConsultantController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("Consultant")]
        public IActionResult GetConsultant()
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection");
            List<GetConsultant> consultants = new List<GetConsultant>();

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string query = "SELECT con.*, " +
                    "                  cou.Country_Name, " +
                    "                  spc.Spec_Name " +
                    "           FROM consultant con, " +
                    "                country cou, " +
                    "               consultant_spec_tbl spc " +
                    "           WHERE  con.Country_Id = cou.Country_Id " +
                    "           and con.Spec_Id = spc.Spec_Id " +
                    "           and con.Status = 1";
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var consultant = new GetConsultant
                            {
                                Cons_Id = Convert.ToInt32(reader["Cons_Id"]),
                                FName = reader["FName"].ToString(),
                                LName = reader["LName"].ToString(),
                                UserName = reader["FName"].ToString() + " " + reader["LName"].ToString(),
                                Email = reader["Email"].ToString(),
                                Tel_No = Convert.ToInt32(reader["Tel_No"]),
                                Description = reader["Description"].ToString(),
                                Image_Path = reader["Image_Path"].ToString(),
                                Country = reader["Country_Name"].ToString(),
                                Job_Category = reader["Spec_Name"].ToString(),
                                Job_Category_id = Convert.ToInt32(reader["Spec_Id"]),
                                Country_id = Convert.ToInt32(reader["Country_Id"]),
                                Status = Convert.ToInt32(reader["Status"])
                            };
                            consultants.Add(consultant);
                        }
                    }
                }
            }

            return Ok(consultants);
        }


        [HttpGet]
        [Route("Consultant/{jobId}/{CountryId}")]
        public IActionResult FindConsultantList(int jobId, int CountryId)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection");
            List<FindConsultant> consultants = new List<FindConsultant>();

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string query = "SELECT Cons_Id, CONCAT(FName, ' ', LName) UserName FROM consultant WHERE Country_Id = @CountryId and Spec_Id = @jobId and Status = 1";
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@jobId", jobId);
                    cmd.Parameters.AddWithValue("@CountryId", CountryId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var consultant = new FindConsultant
                            {
                                Cons_Id = Convert.ToInt32(reader["Cons_Id"]),
                                UserName = reader["UserName"].ToString()
                            };
                            consultants.Add(consultant);
                        }
                    }
                }
            }

            return Ok(consultants);
        }


        [HttpPut]
        [Route("Consultant/{id}")]
        public IActionResult UpdateConsultant(int id, UpdateConsultant consultant)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection");

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string query = "UPDATE consultant SET FName = @FName, LName = @LName, Email = @Email, Tel_No = @Tel_No, Spec_Id = @Job_Category, Country_Id = @Country, updated_at = CURRENT_TIMESTAMP() WHERE Cons_Id = @Cons_Id";


                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Cons_Id", id);
                    cmd.Parameters.AddWithValue("@FName", consultant.FName);
                    cmd.Parameters.AddWithValue("@LName", consultant.LName);
                    cmd.Parameters.AddWithValue("@Email", consultant.Email);
                    cmd.Parameters.AddWithValue("@Tel_No", consultant.Tel_No);
                    cmd.Parameters.AddWithValue("@Job_Category", consultant.Job_Category);
                    cmd.Parameters.AddWithValue("@Country", consultant.Country);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok("Consultant details updated successfully!");
                    }
                    else
                    {
                        return NotFound("Consultant details update failed.");
                    }
                }
            }
        }

        [HttpDelete]
        [Route("Consultant/{id}")]
        public IActionResult DeleteConsultant(int id)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection");

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string query = "UPDATE consultant SET Status = 0, updated_at = CURRENT_TIMESTAMP() WHERE Cons_Id = @Cons_Id";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Cons_Id", id);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok("Consultant deleted successfully!");
                    }
                    else
                    {
                        return NotFound("Consultant delete failed.");
                    }
                }
            }
        }

    }
}
