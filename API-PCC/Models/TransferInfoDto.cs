namespace API_PCC.Models
{
	public class TransferInfoDto
	{
		public string? TransferNumber { get; set; }
		public string? Address { get; set; }
		public string? Email { get; set; }
		public int Status { get; set; }
		public DateTime? DateCreated { get; set; }

		public string? AnimalName { get; set; }
		public string? AnimalBreed { get; set; }

		public string? OwnerFullName { get; set; }
		public string? OwnerContact { get; set; }
	}
}
