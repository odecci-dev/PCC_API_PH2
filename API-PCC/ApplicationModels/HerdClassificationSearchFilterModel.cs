namespace API_PCC.ApplicationModels
{
    public class HerdClassificationSearchFilterModel
    {
        public string? HerdClassCode { get; set; }
        public string? HerdClassDesc { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
    }
}
