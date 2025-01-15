namespace IPMS.API.Dtos
{
    public class InsurancePolicyDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PolicyHolderName { get; set; }
        public decimal Premium { get; set; }
    }
}
