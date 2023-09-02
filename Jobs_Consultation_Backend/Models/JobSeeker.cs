namespace Jobs_Consultation_Backend.Models
{
    public class JobSeeker
    {
        public int Job_Seeker_Id { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string NIC { get; set; }
        public DateOnly DOB { get; set; }
        public string Email { get; set; }
        public int Tel_No { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Image_Path { get; set; }
        public string Doc_Path { get; set; }
        public int Status { get; set; }
        public int User_Id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
