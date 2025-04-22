namespace API_PCC.ApplicationModels
{
    public class CommonSearchFilterModel 
    {
        public string? searchParam { get; set; }
        public int centerId { get; set; }
        public string? dateofApplication { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
		public SortByModel sortBy { get; set; }

	}
}