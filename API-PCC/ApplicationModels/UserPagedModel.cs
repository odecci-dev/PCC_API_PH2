using API_PCC.ApplicationModels.Common;
using API_PCC.Models;

namespace API_PCC.ApplicationModels
{
    public class UserPagedModel : PaginationModel
    {
        public List<UserResponseModel> items { get; set; }

    }
}
