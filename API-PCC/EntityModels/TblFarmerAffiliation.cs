namespace API_PCC.EntityModels
{
    public class TblFarmerAffiliation
    {
        public int Id { get; set; }

        public int FarmerId { get; set; }
        public int AffiliationId { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? DeletedBy { get; set; }

        public DateTime? DeletedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}
