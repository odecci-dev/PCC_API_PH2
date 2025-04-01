namespace API_PCC.ApplicationModels
{
    public class BuffHerdListResponseModel
    {
        public string? Id { get; set; }
        public string? HerdName { get; set; }
        public string? HerdClassification { get; set; }
        public string? CowLevel { get; set; }
        public string? FarmManager { get; set; }
        public string? HerdCode { get; set; }
        public string? Photo { get; set; }
        public string? DateOfApplication { get; set; }


        //additional

        //additional
        public string? BreedTypeCode { get; set; }
        public string? BreedType { get; set; }
        public string? FarmerAffiliation { get; set; }
        public string? FarmAddress { get; set; }
        public string? CenterCode { get; set; }
        public string? Center { get; set; }
        public string? FeedingSystemCode { get; set; }
        public string? FeedingSystem { get; set; }
        public string? OwnerId { get; set; }
        public string? OwnerFullName { get; set; }
        public string? OwnerAddress { get; set; }
        public string? OwnerMobile { get; set; }
        public string? OwnerTelNo { get; set; }
        public string? OwnerEmail { get; set; }
    }
}
