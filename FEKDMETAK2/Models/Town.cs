using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration.UserSecrets;
namespace FEKDMETAK2.Models
{
    public class Town
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int CityId { get; set; }

        // Navigation property
        public City City { get; set; }
    }
}
