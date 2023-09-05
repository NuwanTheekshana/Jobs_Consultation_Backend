namespace Jobs_Consultation_Backend.Models
{
    public class AddCountry
    {
        public string Country_Code { get; set; }
        public string Country_Name { get; set; }

    }

    public class GetCountries
    {
        public int Country_Id { get; set; }
        public string Country_Code { get; set; }
        public string Country_Name { get; set; }
        public int Status { get; set; }
    }

    public class UpdateCountry
    {
        public int Country_Id { get; set; }
        public string Country_Code { get; set; }
        public string Country_Name { get; set; }
    }
}
