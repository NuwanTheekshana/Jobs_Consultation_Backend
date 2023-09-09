using Jobs_Consultation_Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Jobs_Consultation_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobRequestController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string attachmentFolderPath = "wwwroot/Attachments/";
        private readonly string attachmentFolderPathdb = "Attachments/";
        
        public int Cons_Id { get; private set; }

        public JobRequestController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("JobRequest")]
        public Response JobRequest([FromForm] AddJobRequest jobseeker)
        {
            Response response = new Response();

            string connectionString = _configuration.GetConnectionString("MySqlConnection");
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                if (jobseeker.Attachment != null)
                {
                    string fileName = Guid.NewGuid() + Path.GetExtension(jobseeker.Attachment.FileName);
                    string filePath = Path.Combine(attachmentFolderPath, fileName);
                    string filePathdb = Path.Combine(attachmentFolderPathdb, fileName);

                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        jobseeker.Attachment.CopyTo(stream);
                    }

                    // Update the database query to include the attachment path
                    string query = "INSERT INTO job_request(Job_Seeker_Id, Job_Seeker_Doc_Path, Request_Date, Description, Con_Time_Id, Cons_Id, Remarks)" +
                                   "VALUES (@Cons_Id, @Job_Seeker_Doc_Path, @RequestDate, @Description, @ConsultantAvailabilityId, @ConsultantId, @Remarks)";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {

                        int consID = getCons_id(jobseeker.jobseekerId, con);

                        cmd.Parameters.AddWithValue("@Cons_Id", jobseeker.jobseekerId);
                        cmd.Parameters.AddWithValue("@Job_Seeker_Doc_Path", filePathdb);
                        cmd.Parameters.AddWithValue("@Description", jobseeker.Description);
                        cmd.Parameters.AddWithValue("@RequestDate", jobseeker.request_date.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@ConsultantAvailabilityId", jobseeker.consultantAvailability);
                        cmd.Parameters.AddWithValue("@ConsultantId", jobseeker.consultant);
                        cmd.Parameters.AddWithValue("@Remarks", jobseeker.Remarks);


                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            response.StatusCode = 200;
                            response.StatusMessage = "Job request sent successfully..!";
                        }
                        else
                        {
                            response.StatusCode = 100;
                            response.StatusMessage = "Job request failed..!";
                        }
                    }
                }
                else
                {
                    response.StatusCode = 400;
                    response.StatusMessage = "Attachment is missing.";
                }
            }

            return response;
        }

        private int getCons_id(int jobseekerId, MySqlConnection connection)
        {
            string query = "SELECT Cons_Id FROM consultant WHERE User_id = @User_id";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@User_id", jobseekerId);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return Convert.ToInt32(reader["Cons_Id"]);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        [HttpGet]
        [Route("JobRequest/{id}")]
        public IActionResult GetJobRequest(int id)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection");
            List<GetJobRequest> jobrequests = new List<GetJobRequest>();

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string query = "SELECT JR.Job_Seeker_Req_Id," +
                    "                  JS.FName," +
                    "                  JS.LName," +
                    "                  JR.Request_Date," +
                    "                  CT.Time_From," +
                    "                  CT.Time_To," +
                    "                  JR.Job_Seeker_Doc_Path," +
                    "                  JR.Job_Status," +
                    "                  JR.Description" +
                    "                  FROM job_request jr, " +
                    "                       job_seeker js, " +
                    "                       consultant c, " +
                    "                       consultant_time ct " +
                    "                  WHERE JR.Job_Seeker_Id = JS.Job_Seeker_Id " +
                    "                  AND JR.Cons_Id = C.Cons_Id " +
                    "                  AND JR.Con_Time_Id = ct.Con_Time_Id" +
                    "                  AND JR.Cons_Id = @Cons_Id" +
                    "                  AND JR.Status = 1" +
                    "                  AND JR.Job_Status = 1";
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    int cons_id = getCons_id(id, con);
                    cmd.Parameters.AddWithValue("@Cons_Id", cons_id);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            DateTime dateTimeValue = (DateTime)reader["request_date"];

                            int Job_Status = Convert.ToInt32(reader["Job_Status"]);
                            string JobStatusType = "";
                            if (Job_Status == 1)
                            {
                                JobStatusType = "Pending";
                            }
                            else if (Job_Status == 2)
                            {
                                JobStatusType = "Accepted";
                            }
                            else if (Job_Status == 3)
                            {
                                JobStatusType = "Rejected";
                            }

                            var jobrequest = new GetJobRequest
                            {

                                jobseekerRequestId = Convert.ToInt32(reader["Job_Seeker_Req_Id"]),
                                UserName = reader["FName"].ToString() + " " + reader["LName"].ToString(),
                                Description = reader["Description"].ToString(),
                                Attachment = reader["Job_Seeker_Doc_Path"].ToString(),
                                request_date = DateOnly.FromDateTime(dateTimeValue),
                                Time_slot = reader["Time_From"].ToString() + "-" + reader["Time_To"].ToString(),
                                Job_Status = JobStatusType
                            };
                            jobrequests.Add(jobrequest);
                        }
                    }
                }
            }

            return Ok(jobrequests);
        }



        [HttpPut]
        [Route("JobRequest/{id}")]
        public IActionResult UpdateConsultantTime(int id, int status)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection");

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string query = "UPDATE job_request SET Job_Status = @status, updated_at = CURRENT_TIMESTAMP() WHERE Job_Seeker_Req_Id = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@status", status);


                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        string msg = "";
                        if (status == 2)
                        {
                            msg = "Request accepted..!";
                        }
                        else if (status == 3)
                        {
                            msg = "Request rejected..!";
                        }
                        return Ok(msg);
                    }
                    else
                    {
                        return NotFound("Consultant details update failed.");
                    }
                }
            }
        }
    }
}
