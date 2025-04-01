
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
using static API_PCC.Manager.DBMethods;
using Microsoft.IdentityModel.Tokens;
namespace API_PCC.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class AuditTrailController : ControllerBase
    {
        private readonly PCC_DEVContext _context;
        DbManager db = new DbManager();
        DBMethods dbmet = new DBMethods();
        public class BirthTypesSearchFilter
        {
            public string? BirthTypeCode { get; set; }
            public string? BirthTypeDesc { get; set; }
            public int page { get; set; }
            public int pageSize { get; set; }
        }

        public AuditTrailController(PCC_DEVContext context)
        {
            _context = context;
        }
        public class PaginationModel
        {
            public string? CurrentPage { get; set; }
            public string? NextPage { get; set; }
            public string? PrevPage { get; set; }
            public string? TotalPage { get; set; }
            public string? PageSize { get; set; }
            public string? TotalRecord { get; set; }
            public List<AuditTrailModel> items { get; set; }


        }
        public class CommonSearchFilterModel1
        {
            //public string? searchParam { get; set; }
            //public string? Module { get; set; }
            //public int page { get; set; }
            //public int pageSize { get; set; }

            public string? searchValue { get; set; }
            public AuditSearch? filterBy { get; set; }
            public int page { get; set; }
            public int pageSize { get; set; }
            public String dateFrom { get; set; }
            public String dateTo { get; set; }
            public SortByModel sortBy { get; set; }

        }
        // POST: BirthTypes/list
        [HttpPost]
        public async Task<ActionResult<IEnumerable<AuditTrailModel>>> list(CommonSearchFilterModel1 searchFilter)
        {
            try
            {
                List<AuditTrailModel> buffHerdList =  buildauditfilter(searchFilter).ToList();
                var result = buildUserPagedModel(searchFilter, buffHerdList);
                //var result = buildUserPagedModel(searchFilter);
             //   var animal_details221 = _context.ABuffAnimals.Where(a=>a.Id == 3).FirstOrDefault();
                return Ok(result);

            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }
        public class filterdate
        {
           
            public DateTime dateFrom { get; set; }
            public DateTime dateTo { get; set; }

        }
        [HttpPost]
        public async Task<ActionResult<IEnumerable<AuditTrailModel>>> export(filterdate date)
        {
            try
            {
                var result = new List<AuditTrailModel>();
                string sql = $@"SELECT [Id]
                              ,[Actions]
                              ,[Module]
                              ,[DateCreated]
                              ,[UserId]
                          FROM [dbo].[tbl_audittrail] 
                          WHERE [DateCreated] >= '"+date.dateFrom.ToString("yyyy-MM-dd")+"' AND [DateCreated] <= DATEADD(DAY, 1, '"+date.dateFrom.ToString("yyyy-MM-dd") + "')";

                DataTable table = db.SelectDb(sql).Tables[0];
                foreach( DataRow dr in table.Rows)
                {
                    var item = new AuditTrailModel();
                    item.Actions = dr["Actions"].ToString();
                    item.Module = dr["Module"].ToString();
                    item.DateCreated = dr["DateCreated"].ToString();
                    item.Username = dr["UserId"].ToString();

                    result.Add(item);
                }
                    //var result = buildUserPagedModel(searchFilter);
                    //   var animal_details221 = _context.ABuffAnimals.Where(a=>a.Id == 3).FirstOrDefault();
                    return Ok(result);

            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }
        public class AuditSearch
        {
            public string? Username { get; set; }
            public string? Module { get; set; }
        }
        private List<AuditTrailModel> buildauditfilter(CommonSearchFilterModel1 searchFilter)
        {
            var query = dbmet.GetAuditTrail();



            // assuming that you return all records when nothing is specified in the filter

            if (!searchFilter.searchValue.IsNullOrEmpty())
                query = query.Where(a => a.Username.Contains(searchFilter.searchValue)).ToList();
            if (!searchFilter.filterBy.Module.IsNullOrEmpty())
                query = query.Where(a=>a.Module == searchFilter.filterBy.Module).ToList();
            if (!searchFilter.filterBy.Username.IsNullOrEmpty())
                query = query.Where(herd => herd.Username.Contains(searchFilter.filterBy.Username)).ToList();

            if (!searchFilter.dateFrom.IsNullOrEmpty())
                query = query.Where(a => DateTime.Parse(a.DateCreated) >= DateTime.Parse(searchFilter.dateFrom)).ToList();

            if (!searchFilter.dateTo.IsNullOrEmpty())
                query = query.Where(herd => DateTime.Parse(herd.DateCreated) <= DateTime.Parse(searchFilter.dateTo)).ToList();


            if (!searchFilter.sortBy.Field.IsNullOrEmpty())
            {

                if (!searchFilter.sortBy.Sort.IsNullOrEmpty())
                {
                    //query = query.OrderBy(searchFilter.sortBy.Field + " " + searchFilter.sortBy.Sort);
                    query = query.OrderBy(a => searchFilter.sortBy.Field + " " + searchFilter.sortBy.Sort.ToString()).ToList();
                }
                else
                {
                    query = query.OrderBy(a=> searchFilter.sortBy.Field + " asc").ToList();

                }
            }
            else
            {
                query = query.OrderByDescending(herd => herd.Id).ToList();
            }

            return query;
        }
        private List<AuditTrailModel> convertauditlist(List<AuditTrailModel> list)
        {
            var buffHerdResponseModels = new List<AuditTrailModel>();
            foreach (AuditTrailModel audit in list)
            {

                //item.Id = int.Parse(dr["Id"].ToString());
                //item.Actions = dr["Actions"].ToString();
                //item.Module = dr["Module"].ToString();
                //item.DateCreated = dr["DateCreated"].ToString();
                //item.Username = dr["Username"].ToString();
                //item.UserType = dr["UserType"].ToString();
                //result.Add(item);
                var buffHerdResponseModel = new AuditTrailModel()
                    {
                       Id= audit.Id,
                       Actions= audit.Actions,
                       Module= audit.Module,
                       DateCreated= audit.DateCreated,
                       Username= audit.Username,
                       UserType= audit.UserType,
                    };
                    buffHerdResponseModels.Add(buffHerdResponseModel);

                
            }

            return buffHerdResponseModels;
        }
        private List<PaginationModel> buildUserPagedModel(CommonSearchFilterModel1 searchFilter, List<AuditTrailModel> listitem)
        {
            int pagesize = searchFilter.pageSize == 0 ? 10 : searchFilter.pageSize;
            int page = searchFilter.page == 0 ? 1 : searchFilter.page;
            var items = (dynamic)null;

            //if (searchFilter.searchParam == null || searchFilter.searchParam == string.Empty)
            //{
            int totalItems = listitem.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
            items = listitem.Skip((page - 1) * pagesize).Take(pagesize).ToList();

            //var userlist = new List<AuditTrailModel>();
            List<AuditTrailModel> auditresult = convertauditlist(listitem);
            //List<AuditTrailModel> userlist = convertBuffHerdToResponseModelList(buffHerdList);
            totalItems = auditresult.Count;
            totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(pagesize.ToString()));
            items = auditresult.Skip((searchFilter.page - 1) * int.Parse(pagesize.ToString())).Take(int.Parse(pagesize.ToString())).ToList();
            //}
            
                var userlist = dbmet.GetAuditTrail().Where(a => a.Username.ToUpper().Contains(searchFilter.searchValue.ToUpper())).ToList();
                totalItems = auditresult.Count;
                totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(pagesize.ToString()));
                items = userlist.Skip((searchFilter.page - 1) * int.Parse(pagesize.ToString())).Take(int.Parse(pagesize.ToString())).ToList();
            

            var result = new List<PaginationModel>();
            var item = new PaginationModel();

            int pages = searchFilter.page == 0 ? 1 : searchFilter.page;
            item.CurrentPage = searchFilter.page == 0 ? "1" : searchFilter.page.ToString();
            int page_prev = pages - 1;

            double t_records = Math.Ceiling(Convert.ToDouble(totalItems) / Convert.ToDouble(pagesize));
            int page_next = searchFilter.page >= t_records ? 0 : pages + 1;
            item.NextPage = items.Count % pagesize >= 0 ? page_next.ToString() : "0";
            item.PrevPage = pages == 1 ? "0" : page_prev.ToString();
            item.TotalPage = t_records.ToString();
            item.PageSize = pagesize.ToString();
            item.TotalRecord = totalItems.ToString();
            item.items = items;
            result.Add(item);

            return result;
        }
    }
}
