using Jobs_Consultation_Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
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
    }
}
