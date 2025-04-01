
using API_PCC.ApplicationModels;
using API_PCC.ApplicationModels.Common;
using API_PCC.Data;
using API_PCC.Manager;
using API_PCC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using Org.BouncyCastle.Utilities;
using System.Data;
using System.Drawing.Printing;
using System.Data;
using System.Data.SqlClient;
using API_PCC.Utils;
using API_PCC.EntityModels;
using static API_PCC.Controllers.UserManagementController;
namespace API_PCC.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly PCC_DEVContext _context;
        DbManager db = new DbManager();
        public class BirthTypesSearchFilter
        {
            public string? BirthTypeCode { get; set; }
            public string? BirthTypeDesc { get; set; }
            public int page { get; set; }
            public int pageSize { get; set; }
        }

        public TestController(PCC_DEVContext context)
        {
            _context = context;
        }
        // POST: BirthTypes/list
        [HttpPost]
        public async Task<ActionResult<IEnumerable<ABirthType>>> listsss(BirthTypesSearchFilterModel searchFilter)
        {
            try
            {
                string sql = $@"SELECT [Id]
                      ,[Module]
                      ,[ParentModule]
                      ,[DateCreated]
                  FROM [dbo].[User_ModuleTable]";
                var result = new List<Module_Model>();
                DataTable table = db.SelectDb(sql).Tables[0];

                foreach (DataRow dr in table.Rows)
                {
                    var item = new Module_Model();
                    item.ModuleName = dr["Module"].ToString();
                    item.ParentModule = dr["ParentModule"].ToString();
                    item.DateCreated = dr["DateCreated"].ToString();
                    string sql_actions = $@"SELECT TOP (1000) [Id]
                                      ,[ActionName]
                                      ,[Module]
                                      ,[DateCreated]
                                  FROM [PCC_DEV].[dbo].[User_ActionTable] where Module ='" + dr["Id"].ToString() + "'";
                    DataTable action_tbl = db.SelectDb(sql_actions).Tables[0];
                    var action_item = new List<ActionModel>();
                    foreach (DataRow drw in action_tbl.Rows)
                    {
                        var items = new ActionModel();
                        items.Actions = drw["ActionName"].ToString();
                        action_item.Add(items);

                    }
                    item.Actions = action_item;
                    result.Add(item);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }
    }
}
