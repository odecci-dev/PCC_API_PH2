namespace API_PCC.ApplicationModels
{
    public class CommonSearchFilterModel 
    {
        public string? searchParam { get; set; }
        //public string? centerId { get; set; }
        //public string? dateRegistered { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }

    }
}