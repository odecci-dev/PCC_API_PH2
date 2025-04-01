using API_PCC.ApplicationModels.Common;
using API_PCC.Models;

namespace API_PCC.ApplicationModels
{
    public class BreedsPagedModel : PaginationModel
    {
        public List<BreedResponseModel> items { get; set; }

    }
}
