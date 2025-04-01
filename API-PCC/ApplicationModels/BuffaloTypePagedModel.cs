using API_PCC.ApplicationModels.Common;
using API_PCC.Models;

namespace API_PCC.ApplicationModels
{
    public class BuffaloTypePagedModel : PaginationModel
    {
        public List<BuffaloTypeResponseModel> items { get; set; }

    }
}
