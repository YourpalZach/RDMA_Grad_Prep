using System.ComponentModel.DataAnnotations;

namespace RDMA_Graduation_Prep.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string ClassName { get; set; } // Tigers, Dragons, Eagles

        [Required]
        public string CurrentBelt { get; set; }

        [Range(0, 3)]
        public int StripeCount { get; set; } // 0 to 3 stripes
    }
}
