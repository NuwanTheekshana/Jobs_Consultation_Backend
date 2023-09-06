using Jobs_Consultation_Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Reflection.Metadata;
using System.Xml.Linq;
using user_api.Common;

namespace Jobs_Consultation_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RegistrationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("registration")]
        public IActionResult Registration(Registration registration)
        {
            Response response = new Response();

            try
            {
                string connectionString = _configuration.GetConnectionString("MySqlConnection");

                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();
                    string encryptedPassword = CommonMethods.ConvertToEncrypt(registration.Password);

                    string insertUserQuery = "INSERT INTO user (UserName, Email, Password)" +
                                            "VALUES (@UserName, @Email, @Password)";

                    using (MySqlCommand cmd = new MySqlCommand(insertUserQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@UserName", registration.UserName);
                        cmd.Parameters.AddWithValue("@Email", registration.Email);
                        cmd.Parameters.AddWithValue("@Password", encryptedPassword);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            string checkUserQuery = "SELECT User_Id FROM user WHERE Email = @Email";

                            using (MySqlCommand cmd2 = new MySqlCommand(checkUserQuery, con))
                            {
                                cmd2.Parameters.AddWithValue("@Email", registration.Email);
                                object userId = cmd2.ExecuteScalar();

                                if (userId != null)
                                {
                                    // User_Id retrieved, continue with job_seeker registration
                                    string insertJobSeekerQuery = "INSERT INTO job_seeker (FName, LName, NIC, DOB, Email, Tel_No, Address, User_Id) " +
                                                                 "VALUES (@FName, @LName, @NIC, @DOB, @Email, @Tel_No, @Address, @User_Id)";

                                    using (MySqlCommand cmd3 = new MySqlCommand(insertJobSeekerQuery, con))
                                    {
                                        cmd3.Parameters.AddWithValue("@FName", registration.FName);
                                        cmd3.Parameters.AddWithValue("@LName", registration.LName);
                                        cmd3.Parameters.AddWithValue("@NIC", registration.NIC);
                                        cmd3.Parameters.AddWithValue("@DOB", registration.DOB.ToString("yyyy-MM-dd"));
                                        cmd3.Parameters.AddWithValue("@Email", registration.Email);
                                        cmd3.Parameters.AddWithValue("@Tel_No", registration.Tel_No);
                                        cmd3.Parameters.AddWithValue("@Address", registration.Address);
                                        cmd3.Parameters.AddWithValue("@User_Id", userId);

                                        int jobSeekerRowsAffected = cmd3.ExecuteNonQuery();

                                        if (jobSeekerRowsAffected > 0)
                                        {
                                            response.StatusCode = 200;
                                            response.StatusMessage = "Registration successful";
                                        }
                                        else
                                        {
                                            response.StatusCode = 100;
                                            response.StatusMessage = "Job Seeker registration failed";
                                        }
                                    }
                                }
                                else
                                {
                                    response.StatusCode = 100;
                                    response.StatusMessage = "User not found for Job Seeker registration";
                                }
                            }
                        }
                        else
                        {
                            response.StatusCode = 100;
                            response.StatusMessage = "User registration failed";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(response);
        }



        [HttpPost]
        [Route("consultant")]
        public IActionResult Consultant(AddConsultant consultant)
        {
            Response response = new Response();

            try
            {
                string connectionString = _configuration.GetConnectionString("MySqlConnection");

                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();
                    string encryptedPassword = CommonMethods.ConvertToEncrypt(consultant.Password);

                    string insertUserQuery = "INSERT INTO user (UserName, Email, Password)" +
                                            "VALUES (@UserName, @Email, @Password)";

                    using (MySqlCommand cmd = new MySqlCommand(insertUserQuery, con))
                    {
                        string UserName = consultant.FName+ " " +consultant.LName;
                        cmd.Parameters.AddWithValue("@UserName", UserName);
                        cmd.Parameters.AddWithValue("@Email", consultant.Email);
                        cmd.Parameters.AddWithValue("@Password", encryptedPassword);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            string checkUserQuery = "SELECT User_Id FROM user WHERE Email = @Email";

                            using (MySqlCommand cmd2 = new MySqlCommand(checkUserQuery, con))
                            {
                                cmd2.Parameters.AddWithValue("@Email", consultant.Email);
                                object userId = cmd2.ExecuteScalar();

                                if (userId != null)
                                {
                                    // User_Id retrieved, continue with job_seeker registration
                                    string insertConsultantQuery = "INSERT INTO consultant (FName, LName, Email, Tel_No, Spec_Id, Country_Id, User_Id) " +
                                                                 "VALUES (@FName, @LName, @Email, @Tel_No, @Job_Category, @Country, @User_Id)";

                                    using (MySqlCommand cmd3 = new MySqlCommand(insertConsultantQuery, con))
                                    {
                                        cmd3.Parameters.AddWithValue("@FName", consultant.FName);
                                        cmd3.Parameters.AddWithValue("@LName", consultant.LName);
                                        cmd3.Parameters.AddWithValue("@Email", consultant.Email);
                                        cmd3.Parameters.AddWithValue("@Tel_No", consultant.Tel_No);
                                        cmd3.Parameters.AddWithValue("@Job_Category", consultant.Job_Category);
                                        cmd3.Parameters.AddWithValue("@Country", consultant.Country);
                                        cmd3.Parameters.AddWithValue("@User_Id", userId);

                                        int jobConsultantRowsAffected = cmd3.ExecuteNonQuery();

                                        if (jobConsultantRowsAffected > 0)
                                        {
                                            response.StatusCode = 200;
                                            response.StatusMessage = "Registration successful";
                                        }
                                        else
                                        {
                                            response.StatusCode = 100;
                                            response.StatusMessage = "Consultant registration failed";
                                        }
                                    }
                                }
                                else
                                {
                                    response.StatusCode = 100;
                                    response.StatusMessage = "User not found for Consultant registration";
                                }
                            }
                        }
                        else
                        {
                            response.StatusCode = 100;
                            response.StatusMessage = "User registration failed";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(response);
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
                                UserName = reader["FName"].ToString() + " "+ reader["LName"].ToString(),
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
