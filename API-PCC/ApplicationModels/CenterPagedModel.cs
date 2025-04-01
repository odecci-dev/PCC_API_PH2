using API_PCC.ApplicationModels.Common;
using API_PCC.Models;

namespace API_PCC.ApplicationModels
{
    public class CenterPagedModel : PaginationModel
    {
        public List<TblCenterModel> items { get; set; }

    }
}
