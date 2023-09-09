using Org.BouncyCastle.Asn1.X509;

namespace Jobs_Consultation_Backend.Models
{

    public class AddConsultantTime
    {
        public int Cons_Id { get; set; }
        public TimeOnly Time_From { get; set; }
        public TimeOnly Time_To { get; set; }

    }

    public class GetConsultantTime
    {
        public int Con_Time_Id { get; set; }
        public int Cons_Id { get; set; }
        public string Time_From { get; set; }
        public string Time_To { get; set; }
        public int Status { get; set; }
        public string Status_type { get; set; }

    }

    public class FindConsultantTime
    {
        public int Cons_Id { get; set; }
        public DateOnly RequestDate { get; set; }

    }

    public class UpdateConsultantTime
    {
        public TimeOnly Time_From { get; set; }
        public TimeOnly Time_To { get; set; }
    }
}
