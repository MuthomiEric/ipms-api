using IPMS.API.Models;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class InsurancePolicy : BaseEntity
    {
        [Required]
        public string PolicyNumber { get; set; }

        [Required]
        public string PolicyHolderName { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Premium { get; set; }
    }
}
