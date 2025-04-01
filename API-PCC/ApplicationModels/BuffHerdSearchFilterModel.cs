using System.ComponentModel.DataAnnotations;

namespace API_PCC.ApplicationModels
{
    public class BuffHerdSearchFilterModel
    {
        public string? searchValue { get; set; }
        public BuffHerdFilterByModel? filterBy { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
        public String dateFrom { get; set; }
        public String dateTo { get; set; }
        public SortByModel sortBy { get; set; }
        
    }
}
