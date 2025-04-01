namespace API_PCC.ApplicationModels
{
    public class UserBaseSearchModel
    {
        public string? searchParam { get; set; }
        public string? centerId { get; set; }
        public string? herdId { get; set; }
        public bool? farmerSearching { get; set; }
        public string? dateRegisteredFrom { get; set; }
        public string? dateRegisteredTo { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
    }
}
