using API_PCC.EntityModels;
using API_PCC.ApplicationModels.Common;

namespace API_PCC.ApplicationModels
{
    public class TransferPaginationModel : PaginationModel
    {
        public List<TransferModel> items { get; set; }
    }
}
