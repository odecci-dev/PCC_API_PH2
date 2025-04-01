namespace API_PCC.ApplicationModels
{
    public class FarmOwnerSearchFilterModel
    {
        //public string? Name { get; set; }
        //public string? LastName { get; set; }
        public string? searchParam { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
    }
}
