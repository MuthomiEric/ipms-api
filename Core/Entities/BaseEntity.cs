using System.ComponentModel.DataAnnotations;

namespace IPMS.API.Models
{
    public class BaseEntity
    {
        [Key]
        public virtual Guid Id { get; set; }

        public virtual string? ModifiedBy { get; set; }

        public virtual DateTime? ModifiedDate { get; set; }

        public virtual string CreatedBy { get; set; }

        public virtual DateTime CreatedDate { get; set; }

        public virtual bool IsActive { get; set; }
    }
}
