﻿using Jobs_Consultation_Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Globalization;
using System.Reflection.Metadata;

namespace Jobs_Consultation_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultantTimeController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ConsultantTimeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpPost]
        [Route("ConsultantTime")]
        public Response ConsultantTime(AddConsultantTime consultant)
        {
            Response response = new Response();

            string connectionString = _configuration.GetConnectionString("MySqlConnection");
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string queryconsultant = "SELECT Cons_Id FROM consultant WHERE User_Id = @User_Id";
                object Cons_Id;

                using (MySqlCommand cmd2 = new MySqlCommand(queryconsultant, con))
                {
                    cmd2.Parameters.AddWithValue("@User_Id", consultant.Cons_Id);
                    Cons_Id = cmd2.ExecuteScalar();
                }

                if (Cons_Id != null)
                {
                    string query = "INSERT INTO consultant_time (Cons_Id, Time_From, Time_To)" +
                                   "VALUES (@Cons_Id, @Time_From, @Time_To)";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {

                        cmd.Parameters.AddWithValue("@Cons_Id", Cons_Id);
                        cmd.Parameters.Add("@Time_From", MySqlDbType.Time).Value = consultant.Time_From;
                        cmd.Parameters.Add("@Time_To", MySqlDbType.Time).Value = consultant.Time_To;

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            response.StatusCode = 200;
                            response.StatusMessage = "Consultant time added successfully..!";
                        }
                        else
                        {
                            response.StatusCode = 100;
                            response.StatusMessage = "Consultant time added failed..!";
                        }
                    }
                }
            }
            return response;
        }

        [HttpGet]
        [Route("ConsultantTime")]
        public IActionResult GetConsultantTime()
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection");
            List<GetConsultantTime> ConsultantTimes = new List<GetConsultantTime>();

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();
                    string query = "SELECT ct.*, \r\n\t   CONCAT(c.FName, ' ', c.LName) consultantName\r\nFROM consultant_time ct,\r\n\t consultant c\r\nWHERE c.Cons_Id = ct.Cons_Id\r\nand ct.Status = 1";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                var GetConsultantTime = new GetConsultantTime
                                {
                                    Con_Time_Id = Convert.ToInt32(reader["Con_Time_Id"]),
                                    consultantName = reader["consultantName"].ToString(),
                                    Cons_Id = Convert.ToInt32(reader["Cons_Id"]),
                                    Time_From = reader["Time_From"].ToString(),
                                    Time_To = reader["Time_To"].ToString(),
                                };
                                ConsultantTimes.Add(GetConsultantTime);
                            }
                        }

                    return Ok(ConsultantTimes);
                }
            }
        }

        [HttpGet]
        [Route("ConsultantTime/{id}")]
        public IActionResult GetConsultantTime(int id)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection");
            List<GetConsultantTime> ConsultantTimes = new List<GetConsultantTime>();

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string queryconsultant = "SELECT Cons_Id FROM consultant WHERE User_Id = @User_Id";
                object Cons_Id;

                using (MySqlCommand cmd2 = new MySqlCommand(queryconsultant, con))
                {
                    cmd2.Parameters.AddWithValue("@User_Id", id); 
                    Cons_Id = cmd2.ExecuteScalar();
                }

                if (Cons_Id != null)
                {
                    string query = "SELECT * FROM consultant_time WHERE Cons_Id = @id and Status = 1";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", Cons_Id); 

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int Status = Convert.ToInt32(reader["Status"]);
                                string Status_type = "";

                                if (Status == 1)
                                {
                                    Status_type = "Active";
                                }
                                else
                                {
                                    Status_type = "Deactive";
                                }

                                var GetConsultantTime = new GetConsultantTime
                                {
                                    Con_Time_Id = Convert.ToInt32(reader["Con_Time_Id"]),
                                    Cons_Id = Convert.ToInt32(reader["Cons_Id"]),
                                    Time_From = reader["Time_From"].ToString(),
                                    Time_To = reader["Time_To"].ToString(),
                                    Status = Convert.ToInt32(reader["Status"]),
                                    Status_type = Status_type
                                };
                                ConsultantTimes.Add(GetConsultantTime);
                            }
                        }
                    }

                    return Ok(ConsultantTimes);
                }
            }

            return NotFound();
        }


        [HttpGet]
        [Route("GetConsultantAvailbleTime/{id}/{RequestDate}")]
        public IActionResult GetConsultantAvailbleTime(int id, string RequestDate)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection");
            List<GetConsultantTime> ConsultantTimes = new List<GetConsultantTime>();

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string queryConsultant = "SELECT Con_Time_Id FROM job_request WHERE Cons_Id = @Cons_Id AND Request_Date = @RequestDate AND Job_Status != 3 AND Status = 1";
                List<int> consIdList = new List<int>();

                using (MySqlCommand cmd2 = new MySqlCommand(queryConsultant, con))
                {
                    cmd2.Parameters.AddWithValue("@Cons_Id", id);
                    cmd2.Parameters.AddWithValue("@RequestDate", RequestDate);

                    using (MySqlDataReader reader = cmd2.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int consId = Convert.ToInt32(reader["Con_Time_Id"]);
                            consIdList.Add(consId);
                        }
                    }
                }

                if (consIdList.Count > 0)
                {
                    string inClause = string.Join(",", consIdList);

                    string query = $"SELECT * FROM consultant_time WHERE Con_Time_Id NOT IN ({inClause}) AND Status = 1";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var GetConsultantTime = new GetConsultantTime
                                {
                                    Con_Time_Id = Convert.ToInt32(reader["Con_Time_Id"]),
                                    Cons_Id = Convert.ToInt32(reader["Cons_Id"]),
                                    Time_From = reader["Time_From"].ToString(),
                                    Time_To = reader["Time_To"].ToString(),
                                    Status = Convert.ToInt32(reader["Status"])
                                };
                                ConsultantTimes.Add(GetConsultantTime);
                            }
                        }
                    }

                    return Ok(ConsultantTimes);
                }
                else
                {
                    string query = $"SELECT * FROM consultant_time WHERE Cons_Id = @Cons_Id and Status = 1";
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Cons_Id", id);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var GetConsultantTime = new GetConsultantTime
                                {
                                    Con_Time_Id = Convert.ToInt32(reader["Con_Time_Id"]),
                                    Cons_Id = Convert.ToInt32(reader["Cons_Id"]),
                                    Time_From = reader["Time_From"].ToString(),
                                    Time_To = reader["Time_To"].ToString(),
                                    Status = Convert.ToInt32(reader["Status"])
                                };
                                ConsultantTimes.Add(GetConsultantTime);
                            }
                        }
                    }

                    return Ok(ConsultantTimes);
                }

            }

            return NotFound();
        }



        [HttpPut]
        [Route("ConsultantTime/{id}")]
        public IActionResult UpdateConsultantTime(int id, UpdateConsultantTime constime)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection");

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string query = "UPDATE consultant_time SET Time_From = @Time_From, Time_To = @Time_To, updated_at = CURRENT_TIMESTAMP() WHERE Con_Time_Id = @Cons_time_id";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Cons_time_id", id);
                    cmd.Parameters.AddWithValue("@Time_From", constime.Time_From);
                    cmd.Parameters.AddWithValue("@Time_To", constime.Time_To);

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
        [Route("ConsultantTime/{id}")]
        public IActionResult DeleteCountry(int id)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection");

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string query = "UPDATE consultant_time SET Status = 0, updated_at = CURRENT_TIMESTAMP() WHERE Con_Time_Id = @Con_Time_Id";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Con_Time_Id", id);

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
