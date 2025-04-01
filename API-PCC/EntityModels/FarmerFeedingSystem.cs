using API_PCC.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_PCC.EntityModels
{
    public class FarmerFeedingSystem
    {

        public int Id { get; set; }

        public int FarmerId { get; set; }
        public int FeedingSystemId { get; set; }
        public int CreatedBy { get; set; } 
        public DateTime CreatedAt { get; set; }
        public int? DeletedBy { get; set; } 
        public bool IsDeleted { get; set; }



    }
}