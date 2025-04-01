using API_PCC.ApplicationModels.Common;
using API_PCC.EntityModels;
using API_PCC.Models;

namespace API_PCC.ApplicationModels
{
    public class FarmerPagedModel : PaginationModel
    {
        public List<TblFarmerVM> items { get; set; }

    }
}
