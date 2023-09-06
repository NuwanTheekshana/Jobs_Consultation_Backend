namespace Jobs_Consultation_Backend.Models
{
    public class GetConsultant
    {
        public int Cons_Id { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int Tel_No { get; set; }
        public string Description { get; set; }
        public string Image_Path { get; set; }
        public string Job_Category { get; set; }
        public int Job_Category_id { get; set; }
        public string Country { get; set; }
        public int Country_id { get; set; }
        public int Status { get; set; }
    }

    public class AddConsultant
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int Tel_No { get; set; }
        public int Job_Category { get; set; }
        public int Country { get; set; }
    }

    public class UpdateConsultant
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public int Tel_No { get; set; }
        public int Job_Category { get; set; }
        public int Country { get; set; }
    }

    
}
