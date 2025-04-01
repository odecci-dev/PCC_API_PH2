using API_PCC.ApplicationModels.Common;
using API_PCC.Models;

namespace API_PCC.ApplicationModels
{
    public class BloodCompsPagedModel : PaginationModel
    {
        public List<ABloodComp> items { get; set; }

    }
}
