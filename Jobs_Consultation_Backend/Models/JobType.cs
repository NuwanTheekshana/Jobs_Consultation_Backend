namespace Jobs_Consultation_Backend.Models
{
    public class AddJobType
    {
        public string Spec_Name { get; set; }

    }

    public class GetJobType
    {
        public int Spec_Id { get; set; }
        public string Spec_Name { get; set; }
        public int Status { get; set; }
    }

    public class UpdateJobType
    {
        public int Spec_Id { get; set; }
        public string Spec_Name { get; set; }
    }
}
