namespace Jobs_Consultation_Backend.Models
{

    public class AddJobRequest
    {
        public int jobseekerId { get; set; }
        public string Description { get; set; }
        public IFormFile Attachment { get; set; }
        public DateTime request_date { get; set; }
        public int Job_Category { get; set; }
        public int country { get; set; }
        public int consultant { get; set; }
        public int consultantAvailability { get; set; }
        public string Remarks { get; set; }

    }

    public class GetJobRequest
    {
        public int jobseekerRequestId { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public string Attachment { get; set; }
        public DateOnly request_date { get; set; }
        public string Time_slot { get; set; }
        public string Job_Status { get; set; }

    }
}
