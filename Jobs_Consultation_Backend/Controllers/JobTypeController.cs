using Jobs_Consultation_Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Jobs_Consultation_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobTypeController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public JobTypeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
         
        [HttpPost]
        [Route("jobtype")]
        public Response JobType(AddJobType job)
        {
            Response response = new Response();

            string connectionString = _configuration.GetConnectionString("MySqlConnection");
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string query = "INSERT INTO consultant_spec_tbl (Spec_Name)" +
                               "VALUES (@Spec_Name)";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Spec_Name", job.Spec_Name);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        response.StatusCode = 200;
                        response.StatusMessage = "Job Type added successfully..!";
                    }
                    else
                    {
                        response.StatusCode = 100;
                        response.StatusMessage = "Job Type details added failed..!";
                    }
                }
            }

            return response;
        }

        [HttpGet]
        [Route("Getjobtype")]
        public IActionResult GetJobType()
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection");
            List<GetJobType> jobtypes = new List<GetJobType>();

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string query = "SELECT * FROM consultant_spec_tbl WHERE Status = 1";
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var JobType = new GetJobType
                            {
                                Spec_Id = Convert.ToInt32(reader["Spec_Id"]),
                                Spec_Name = reader["Spec_Name"].ToString(),
                                Status = Convert.ToInt32(reader["Status"])
                            };
                            jobtypes.Add(JobType);
                        }
                    }
                }
            }

            return Ok(jobtypes);
        }

        [HttpPut]
        [Route("jobtype/{id}")]
        public IActionResult UpdateJobType(int id, UpdateJobType jobtype)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection");

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string query = "UPDATE consultant_spec_tbl SET Spec_Name = @Spec_Name, updated_at = CURRENT_TIMESTAMP() WHERE Spec_Id = @Spec_Id";


                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Spec_Id", id);
                    cmd.Parameters.AddWithValue("@Spec_Name", jobtype.Spec_Name);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok("Job type updated successfully!");
                    }
                    else
                    {
                        return NotFound("Job type update failed.");
                    }
                }
            }
        }

        [HttpDelete]
        [Route("jobtype/{id}")]
        public IActionResult DeleteJobType(int id)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection");

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string query = "UPDATE consultant_spec_tbl SET Status = 0, updated_at = CURRENT_TIMESTAMP() WHERE Spec_Id = @Spec_Id";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Spec_Id", id);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok("Job type deleted successfully!");
                    }
                    else
                    {
                        return NotFound("Job type delete failed.");
                    }
                }
            }
        }
    }
}
