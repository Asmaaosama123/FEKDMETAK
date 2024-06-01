using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration.UserSecrets;
namespace FEKDMETAK.Models
{
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RId { get; set; }
        public int ReviewedUserId { get; set; }
        public int ReviewerId { get; set; }
        public decimal RateofUser { set; get; }
        public decimal TotalRate { set; get; }
        //[ForeignK
        //ey("ReviewedUserId")]
        public bool Complaint { get; set; }
        public User ReviewedUser { get; set; }
        //[ForeignKey("ReviewerId")]
        public User Reviewer { get; set; }
    }
}
