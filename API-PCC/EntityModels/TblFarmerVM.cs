using System.ComponentModel.DataAnnotations.Schema;

namespace API_PCC.EntityModels
{
    public class TblFarmerVM
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string TelephoneNumber { get; set; }

        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public int UserId { get; set; }
        public int? GroupId { get; set; }
        public int? HerdId { get; set; }
        public int? Center { get; set; }
        public bool IsManager { get; set; }
        public int FarmerClassificationId { get; set; }
        public int FarmerAffliationId { get; set; }
        public List<string> FarmerBreedTypes { get; set; }
        public List<string> FarmerFeedingSystems { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }

    }
}
