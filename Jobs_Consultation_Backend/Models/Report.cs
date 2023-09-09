namespace Jobs_Consultation_Backend.Models
{
    public class GetAppointmentList
    {
        public String RequestId { get; set; }
        public String JobSeeker { get; set; }
        public string JobConsultant { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public string AppintmentTimeSlot { get; set; }
        public string Status { get; set; }
    }
}
