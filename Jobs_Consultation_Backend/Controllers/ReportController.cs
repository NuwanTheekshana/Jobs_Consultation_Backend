using Jobs_Consultation_Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Jobs_Consultation_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ReportController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpGet]
        [Route("Report/AppointmentList/{Status}")]
        public IActionResult GetAppointmentList(int Status)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection");
            List<GetAppointmentList> AppointmentLists = new List<GetAppointmentList>();

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string query = "SELECT JR.Job_Seeker_Req_Id,   \r\n       JS.FName,   \r\n       JS.LName,\r\n       CONCAT(JS.FName, ' ', JS.LName) JobseekerName, CONCAT(c.FName, ' ', c.LName) ConsultantName, \r\n       JR.Request_Date,   \r\n       CONCAT(CT.Time_From, '-', CT.Time_To) TimeSlot,\r\n       CASE\r\n        WHEN JR.Job_Status = '1' THEN 'Pending'\r\n        WHEN JR.Job_Status = '2' THEN 'Accept'\r\n        WHEN JR.Job_Status = '3' THEN 'Reject'\r\n        ELSE 'Unknown'\r\n    END AS Job_Status\r\nFROM job_request jr,    \r\n       job_seeker js,    \r\n       consultant c,    \r\n       consultant_time ct    \r\nWHERE JR.Job_Seeker_Id = JS.Job_Seeker_Id    \r\n       AND JR.Cons_Id = C.Cons_Id    \r\n       AND JR.Con_Time_Id = ct.Con_Time_Id   \r\n       AND JR.Status = 1   \r\n       AND JR.Job_Status = @Status;";
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Status", Status);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime dateTimeValue = (DateTime)reader["Request_Date"];
                            var AppointmentList = new GetAppointmentList
                            {
                                RequestId = reader["Job_Seeker_Req_Id"].ToString(),
                                JobSeeker = reader["JobseekerName"].ToString(),
                                JobConsultant = reader["ConsultantName"].ToString(),
                                AppointmentDate = DateOnly.FromDateTime(dateTimeValue),
                                AppintmentTimeSlot = reader["TimeSlot"].ToString(),
                                Status = reader["Job_Status"].ToString()

                            };
                            AppointmentLists.Add(AppointmentList);
                        }
                    }
                }
            }

            return Ok(AppointmentLists);
        }

    }
}
