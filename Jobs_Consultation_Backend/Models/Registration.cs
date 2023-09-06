namespace Jobs_Consultation_Backend.Models
{
    public class Registration
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public string UserName { get; set; }
        public string NIC { get; set; }
        public DateTime DOB { get; set; }
        public string Email { get; set; }
        public int Tel_No { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
    }

    
        public class AddUsers
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public int Permission { get; set; }
        public string Password { get; set; }
    }

    public class GetUsers
    {
        public int User_Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int Permission { get; set; }
        public string Permission_Type { get; set; }
        public string Status_Type { get; set; }
        public int Status { get; set; }

    }

    public class UpdateUser
    {
        public string Email { get; set; }
        public int Permission { get; set; }
    }
}
