namespace API_PCC.EntityModels
{
    public class TblFarmOwner
    {
        public int Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Address { get; set; }

        public string? TelephoneNumber { get; set; }

        public string? MobileNumber { get; set; }
        public int User_Id { get; set; }

		public int Group_Id { get; set; }
		public int Is_Manager { get; set; }
		public int FarmerClassification_Id { get; set; }
		public int FarmerAffliation_Id { get; set; }


		public string? Email { get; set; }

    }
}
