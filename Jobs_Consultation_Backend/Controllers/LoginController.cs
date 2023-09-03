using Jobs_Consultation_Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using user_api.Common;

namespace Jobs_Consultation_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpPost]
        [Route("login")]
        public LoginResult Login(Login login)
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection");
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                string encryptedPassword = CommonMethods.ConvertToEncrypt(login.Password);

                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM user WHERE Email = '" + login.Email + "' AND Password = '" + encryptedPassword + "' AND Status = 1", con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    int userId = Convert.ToInt32(row["User_Id"]);
                    string UserName = row["UserName"].ToString();
                    string userEmail = row["Email"].ToString();
                    int userPermission = Convert.ToInt32(row["Permission"]);
                    int userStatus = Convert.ToInt32(row["Status"]);

                    return new LoginResult
                    {
                        Id = userId,
                        UserName = UserName,
                        Email = userEmail,
                        Permission = userPermission,
                        Status = userStatus,
                        Token = CommonMethods.ConvertToEncrypt(userEmail),
                        Message = "Data Found"
                    };
                }
                else
                {
                    return new LoginResult
                    {
                        Message = "Invalid User"
                    };
                }
            }
        }


    }
}
