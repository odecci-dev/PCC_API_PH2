using API_PCC.ApplicationModels.Common;
using API_PCC.Models;
using static API_PCC.Controllers.BuffAnimalsController;

namespace API_PCC.ApplicationModels
{
    public class buffanimalpagemodel : PaginationModel
    {
        public List<animalresult> items { get; set; }

    }
}
