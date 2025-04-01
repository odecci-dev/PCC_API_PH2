using API_PCC.ApplicationModels.Common;
using API_PCC.Models;

namespace API_PCC.ApplicationModels
{
    public class TypeOwnershipPagedModel : PaginationModel
    {
        public List<ATypeOwnership> items { get; set; }

    }
}
