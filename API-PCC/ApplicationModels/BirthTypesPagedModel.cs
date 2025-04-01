using API_PCC.ApplicationModels.Common;
using API_PCC.Models;

namespace API_PCC.ApplicationModels
{
    public class BirthTypesPagedModel : PaginationModel
    {
        public List<ABirthType> items { get; set; }

    }
}
