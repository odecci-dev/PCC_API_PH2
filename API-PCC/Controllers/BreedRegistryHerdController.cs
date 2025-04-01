
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
using System.Linq.Dynamic.Core;
using System.Linq;
using API_PCC.DtoModels;
using static API_PCC.Controllers.BreedRegistryHerdController;
using Antlr4.Runtime;
using System;
using static API_PCC.Controllers.BloodCompsController;
using System.Globalization;
namespace API_PCC.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class BreedRegistryHerdController : ControllerBase
    {
        private readonly PCC_DEVContext _context;
        DbManager db = new DbManager();
        string status = "";
        DBMethods dbmet = new DBMethods();
        public class BirthTypesSearchFilter
        {
            public string? BirthTypeCode { get; set; }
            public string? BirthTypeDesc { get; set; }
            public int page { get; set; }
            public int pageSize { get; set; }
        }

        public BreedRegistryHerdController(PCC_DEVContext context)
        {
            _context = context;
        }

        public class AuditSearch
        {
            public string? Username { get; set; }
            public string? Module { get; set; }
        }
        public class CommonSearchFilterModel1
        {

            public string? searchValue { get; set; }
            public AuditSearch? filterBy { get; set; }
            public int page { get; set; }
            public int pageSize { get; set; }
            public String dateFrom { get; set; }
            public String dateTo { get; set; }
            public SortByModel sortBy { get; set; }

        }
        public class ViewBreedRegistryHerd
        {
            public int HerdId { get; set; }
            public int? FarmerId { get; set; }
            public int CenterId { get; set; }
            public string HerdName { get; set; }
            public string FarmAddress { get; set; }
            public string Photo { get; set; }
            public string CreatedBy { get; set; }
            public string DateCreated { get; set; }
            public string HerdCode { get; set; }
            public string HerdClassification { get; set; }
            public DateTime? DateofApplication { get; set; }
            public string FarmerName { get; set; }
            public string CowLevel { get; set; }
            public string FarmManager { get; set; }
            public string FarmerAffiliation { get; set; }
            public List<ListFarmer> ListFarmer { get; set; }
        }
        public class ListFarmer
        {
            public int? Id { get; set; }
            public string FarmerName { get; set; }
            public List<BreedType> BreedType { get; set; }
            public List<FeedingType> FeedingType { get; set; }
            public string FarmerClassification { get; set; }
            public string CowLevel { get; set; }
        }
        public class FeedingType
        {
            public string FeedingSystemDesc { get; set; }
        }
        public class BreedType
        {
            public string Breed_Desc { get; set; }
        }
     
        public class BreedRegistryHerd
        {
            public int HerdId { get; set; }
            public int? FarmerId { get; set; }
            public string HerdName { get; set; }
            public string HerdCode { get; set; }
            public DateTime? DateofApplication { get; set; }
            public string FarmerName { get; set; }
            public string FarmAddress { get; set; }
            public string Photo { get; set; }
            public string CreatedBy { get; set; }
            public string DateCreated { get; set; }
            public string CowLevel { get; set; }
            public string FarmManager { get; set; }
        }

        public class BreedRegistryHerd2
        {
            public int HerdId { get; set; }
            //public int? FarmerId { get; set; }
            public string HerdName { get; set; }
            public string HerdCode { get; set; }
            public int Center { get; set; }
            public DateTime? DateofApplication { get; set; }
            public string FarmerName { get; set; }
            public string FarmAddress { get; set; }
            public string Photo { get; set; }
            public string CreatedBy { get; set; }
            public string DateCreated { get; set; }
            public string FarmerCount { get; set; }
            public string FarmManager { get; set; }
        }

        public class FarmerHerdSearch
        {
            public string? searchParam { get; set; }
            public int page { get; set; }
            public int pageSize { get; set; }
            public int center { get; set; }
            public String DateofApplication { get; set; }
            public SortByModel sortBy { get; set; }

        }
        private List<BreedRegistryHerd> FarmerHerdList2()
        {
            // This logic can be anything that generates the list
            var result = (from a in _context.TblHerdFarmers
                          join b in _context.HBuffHerds on a.HerdId equals b.Id into HerdFarmers
                          from b in HerdFarmers.DefaultIfEmpty()
                          join c in _context.Tbl_Farmers on a.FarmerId equals c.Id into farmowner
                          from c in farmowner.DefaultIfEmpty()
                          join d in _context.HHerdClassifications on c.FarmerClassification_Id equals d.Id into Classification
                          from d in Classification.DefaultIfEmpty()
                          join e in _context.tbl_FarmerFeedingSystem on a.FarmerId equals e.Id into FarmerFeedingSystem
                          from e in FarmerFeedingSystem.DefaultIfEmpty()
                          join g in _context.TblUsersModels on b.Id equals g.Id into Users
                          from g in Users.DefaultIfEmpty()
                          join h in _context.Tbl_FarmerAffialiation on a.FarmerId equals h.FarmerId into FarmerAffiliation
                          from h in FarmerAffiliation.DefaultIfEmpty()
                          join i in _context.HFarmerAffiliations on h.AffiliationId equals i.Id into Affiliation
                          from i in Affiliation.DefaultIfEmpty()
                          join j in _context.TblFarmerBreedTypes on a.FarmerId equals j.FarmerId into FarmerBreedType
                          from j in FarmerBreedType.DefaultIfEmpty()
                          let cowLevel = _context.ABuffAnimals.Count(buff => buff.FarmerId == a.FarmerId)
                          let farmManager = _context.Tbl_Farmers.Any(farm => farm.Is_Manager == true && farm.Id == a.FarmerId)

                          select new
                          {
                              HerdId = b != null ? b.Id : 0,
                              HerdCode = b != null ? b.HerdCode : "Unknown Code",
                              HerdName = b != null ? b.HerdName : "Unknown Herd",
                              FarmAddress = b != null ? b.FarmAddress : "Unknown FarmAddress",
                              Photo = b != null ? b.Photo : "Unknown FarmAddress",
                              CreatedBy = b != null ? b.CreatedBy : "Unknown FarmAddress",
                              DateCreated = b != null ? b.DateCreated.ToString("yyyy-MM-dd") : "Unknown FarmAddress",
                              DateofApplication = b != null ? b.DateCreated : DateTime.MinValue,
                              FarmerName = (g != null ? g.Lname : "Unknown") + ", " + (g != null ? g.Fname : "Unknown"),
                              FarmerId = e != null ? e.Id : 0,
                              CowLevel = cowLevel.ToString(),
                              FarmManager = farmManager ? "1" : "0",
                          }).ToList();

            // Process the result into BreedRegistryHerd
            var finalResult = result.Select(r => new BreedRegistryHerd
            {
                HerdId = r.HerdId,
                HerdName = r.HerdName,
                HerdCode = r.HerdCode,
                DateofApplication = r.DateofApplication,
                FarmerName = r.FarmerName,
                FarmAddress = r.FarmAddress,
                Photo = r.Photo,
                DateCreated = r.DateCreated,
                CreatedBy = r.CreatedBy,
                FarmerId = r.FarmerId,
                CowLevel = r.CowLevel,
                FarmManager = r.FarmManager
            }).ToList();

            return finalResult;
        }

        //private IQueryable<BreedRegistryHerd> FarmerHerdList()
        //{
        //    return (from a in _context.TblHerdFarmers
        //            join b in _context.HBuffHerds on a.HerdId equals b.Id into HerdFarmers
        //            from b in HerdFarmers.DefaultIfEmpty()
        //            join c in _context.Tbl_Farmers on a.FarmerId equals c.Id into farmowner
        //            from c in farmowner.DefaultIfEmpty()
        //            join g in _context.TblUsersModels on a.FarmerId equals g.Id into Users
        //            from g in Users.DefaultIfEmpty()
        //            let cowLevel = _context.ABuffAnimals.Count(buff => buff.FarmerId == a.FarmerId)
        //            let farmManager = _context.Tbl_Farmers.Any(farm => farm.Is_Manager && farm.Id == a.FarmerId)

        //            select new BreedRegistryHerd
        //            {
        //                HerdId = b != null ? b.Id : 0,
        //                HerdCode = b != null ? b.HerdCode : "Unknown Herd",
        //                HerdName = b != null ? b.HerdName : "Unknown Herd",
        //                DateofApplication = b != null ? b.DateCreated : DateTime.MinValue,
        //                FarmerName = (g != null ? g.Lname : "Unknown") + ", " + (g != null ? g.Fname : "Unknown"),
        //                FarmerId = a.FarmerId, // This should always be valid since a is not null in this context
        //                CowLevel = cowLevel.ToString(),
        //                FarmManager = farmManager ? "1" : "0",
        //            }).AsQueryable();
        //}

        private IQueryable<BreedRegistryHerd2> FarmerHerdList()
        {
            return (from herd in _context.HBuffHerds
                    join farmer in _context.Tbl_Farmers on herd.FarmerId equals farmer.Id
                    join farmerUser in _context.TblUsersModels on farmer.User_Id equals farmerUser.Id
                    where herd.DeleteFlag == false



                    select new BreedRegistryHerd2
                    {
                        HerdId = herd.Id,
                        HerdCode = herd.HerdCode,
                        HerdName = herd.HerdName,
                        FarmAddress = herd.FarmAddress,
                        Photo = herd.Photo,
                        CreatedBy = herd.CreatedBy,
                        DateCreated = herd.DateCreated.ToString(),
                        Center = (int)herd.Center,
                        DateofApplication = herd.DateCreated,
                        FarmerCount = _context.TblHerdFarmers.Count(farmer => farmer.HerdId == herd.Id).ToString(),
                        FarmManager = farmer != null
                             ? farmerUser.Lname + ", " + farmerUser.Fname
                             : "Unknown Manager",
                        FarmerName = farmer != null
                             ? farmerUser.Lname + ", " + farmerUser.Fname
                             : "Unknown Manager",
                    }).Distinct().AsQueryable();
        }

        private void validateDate(BuffHerdSearchFilterModel searchFilter)
        {

            if (!searchFilter.dateFrom.IsNullOrEmpty())
            {
                if (!DateTime.TryParse(searchFilter.dateFrom, out DateTime dateTimeFrom))
                {
                    throw new System.FormatException("Date From is not a valid Date!");
                }
            }

            if (!searchFilter.dateTo.IsNullOrEmpty())
            {
                if (!DateTime.TryParse(searchFilter.dateTo, out DateTime dateTimeTo))
                {
                    throw new System.FormatException("Date To is not a valid Date!");
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> Search(FarmerHerdSearch searchFilter)
        {
            try
            {
                var farmerHerds = await buildfarmerherd(searchFilter).ToListAsync();
                var result = FormList(searchFilter, farmerHerds);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpPost]
        public async Task<IActionResult> FarmerList(BreedRegistryHerdFarmerSearchFilterModel searchFilter)
        {
            var result = dbmet.FarmerListView();

            if (!string.IsNullOrEmpty(searchFilter.herdId))
            {
                result = result.Where(a => a.HerdId == searchFilter.herdId).ToList();
            }

            if (result.Count == 0)
            {
                return Ok("No Record Found");
            }

            var pagedResult = farmersPagedModel(searchFilter, result);

            return Ok(pagedResult);
        }

        public class BreedRegistryHerdFarmerSearchFilterModel
        {
            public string? herdId { get; set; }
            public int page { get; set; }
            public int pageSize { get; set; }
        }

        public class BreedRegistryHerdFarmerPagedModel : ApplicationModels.Common.PaginationModel
        {
            public List<ListFarmerVM> items { get; set; }
        }

        private BreedRegistryHerdFarmerPagedModel farmersPagedModel(BreedRegistryHerdFarmerSearchFilterModel searchFilter, List<ListFarmerVM> listFarmers)
        {
            int pageSize = searchFilter.pageSize == 0 ? 10 : searchFilter.pageSize;
            int page = searchFilter.page == 0 ? 1 : searchFilter.page;

            int totalItems = listFarmers.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var pagedItems = listFarmers.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new BreedRegistryHerdFarmerPagedModel
            {
                CurrentPage = page.ToString(),
                PageSize = pageSize.ToString(),
                TotalRecord = totalItems.ToString(),
                TotalPage = totalPages.ToString(),
                NextPage = page >= totalPages ? "0" : (page + 1).ToString(),
                PrevPage = page == 1 ? "0" : (page - 1).ToString(),
                items = pagedItems
            };
        }


        public class FeedingTypeId
        {
            public int FarmerFeedId { get; set; }

        }
        public class BreedTypeId
        {
            public int FarmerBreedId { get; set; }
        }
        public class FarmerSaveModel
        {
            public int HerdId { get; set; }
            public int FarmerId { get; set; }
            public List<FeedingTypeId> FeedingSystemId { get; set; }
            public List<BreedTypeId> BreedTypeId { get; set; }
            public int FarmerAffliation_Id { get; set; }
            public int FarmerClassification_Id { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? Address { get; set; }
            public string? TelephoneNumber { get; set; }
            public string? MobileNumber { get; set; }
            public int? UserId { get; set; }
            public int? CreatedBy { get; set; }
            public string? Group_Id { get; set; }
            public string? Is_Manager { get; set; }
            public string? Email { get; set; }

        }
        [HttpPost]
        public async Task<IActionResult> FarmerSave(FarmerSaveModel model)
        {
            string Insert = "";
            string isfarmer = $@"select * from tbl_Farmers where User_Id ='" + model.UserId + "'";

            DataTable tbl_isfarmer = db.SelectDb(isfarmer).Tables[0];
            if (tbl_isfarmer.Rows.Count != 0)
            {
                return BadRequest("User is already a Farmer");
            }
            else
            {
                string isHerd = $@"select * from H_Buff_Herd where Id ='" + model.HerdId + "'";

                DataTable tbl_isHerd = db.SelectDb(isHerd).Tables[0];
                if (tbl_isHerd.Rows.Count == 0)
                {
                    return BadRequest("Herd Id does not Exist");
                }
                else
                {
                    Insert += $@"
                   INSERT INTO [dbo].[tbl_HerdFarmer]
                               ([Herd_Id]
                               ,[Farmer_Id]
                               ,[Created_By]
                               ,[Is_Deleted]
                               ,[Created_At])
                         VALUES
                           ('" + model.HerdId + "'," +
                          "'" + model.FarmerId + "'," +
                          "'" + model.CreatedBy + "'," +
                           "'0'," +
                          "'" + DateTime.Now.ToString("yyyy-MM-dd") + "') ";
                }
                for(int x=0; x>model.FeedingSystemId.Count;x++)
                {
                    string isfeeding = $@"select * from H_Feeding_System where Id ='" + model.FeedingSystemId[x].FarmerFeedId + "'";

                    DataTable tbl_isfeeding = db.SelectDb(isfeeding).Tables[0];
                    if (tbl_isfeeding.Rows.Count == 0)
                    {
                        return BadRequest("Feeding System Id does not Exist");
                    }
                    else
                    {
                        Insert += $@"
                   INSERT INTO [dbo].[tbl_FarmerFeedingSystem]
                                   ([Farmer_Id]
                                   ,[FeedingSystem_Id]
                                   ,[Created_By]
                                   ,[Is_Deleted]
                                   ,[Created_At])
                             VALUES
                           ('" + model.FarmerId + "'," +
                                                   "'" + model.FeedingSystemId[x].FarmerFeedId + "'," +
                                                   "'" + model.CreatedBy + "'," +
                                                    "'0'," +
                                                   "'" + DateTime.Now.ToString("yyyy-MM-dd") + "') ";
                    }
                }
                for(int i=0;i<model.BreedTypeId.Count;i++)
                {
                    string isbreed = $@"select * from A_Breed where Id ='" + model.BreedTypeId[i].FarmerBreedId + "'";

                    DataTable tbl_isbreed = db.SelectDb(isbreed).Tables[0];
                    if (tbl_isbreed.Rows.Count == 0)
                    {
                        return BadRequest("Feeding System Id does not Exist");
                    }
                    else
                    {
                        Insert += $@"
                   INSERT INTO [dbo].[tbl_FarmerBreedType]
                               ([Farmer_Id]
                               ,[BreedType_Id]
                               ,[Created_By]
                               ,[Is_Deleted]
                               ,[Created_At])
                            VALUES
                           ('" + model.FarmerId + "'," +
                                              "'" + model.BreedTypeId[i].FarmerBreedId + "'," +
                                              "'" + model.UserId + "'," +
                                              "'0'," +
                                              "'" + DateTime.Now.ToString("yyyy-MM-dd") + "') ";
                    }
                }
              


                string isAffil = $@"select * from H_Farmer_Affiliation where Id ='" + model.FarmerAffliation_Id + "'";

                DataTable tbl_isAffil = db.SelectDb(isAffil).Tables[0];
                string isclassification = $@"select * from H_Farmer_Affiliation where Id ='" + model.FarmerClassification_Id + "'";

                DataTable tbl_isclassificationl = db.SelectDb(isclassification).Tables[0];
                if (tbl_isAffil.Rows.Count == 0)
                {
                    return BadRequest("Affiliation System Id does not Exist");
                }
                else if (tbl_isclassificationl.Rows.Count == 0)
                {
                    return BadRequest("Classification System Id does not Exist");

                }
                else
                {
                    Insert += $@"
                  INSERT INTO [dbo].[Tbl_Farmers]
                               ([FirstName]
                               ,[LastName]
                               ,[Address]
                               ,[TelephoneNumber]
                               ,[MobileNumber]
                               ,[User_Id]
                               ,[Group_Id]
                               ,[Is_Manager]
                               ,[FarmerClassification_Id]
                               ,[FarmerAffliation_Id]
                               ,[Created_By]
                               ,[Created_At]
                               ,[Email]
                               ,[Deleted_At]
                               ,[Is_Deleted])
                         VALUES
                           ('" + model.FirstName + "'," +
                                          "'" + model.LastName + "'," +
                                          "'" + model.Address + "'," +
                                          "'" + model.TelephoneNumber + "'," +
                                          "'" + model.MobileNumber + "'," +
                                          "'" + model.UserId + "'," +
                                          "'" + model.Group_Id + "'," +
                                          "'" + model.Is_Manager + "'," +
                                          "'" + model.FarmerClassification_Id + "'," +
                                          "'" + model.FarmerAffliation_Id + "'," +
                                          "'" + model.CreatedBy + "'," +
                                          "'" + DateTime.Now.ToString("yyyy-MM-dd") + "'," +
                                          "'" + DateTime.Now.ToString("yyyy-MM-dd") + "'," +
                                          "'0') ";
                }

                

                return Ok("Successfully Saved" + db.DB_WithParam(Insert));
            }



        }

        //[HttpPost]
        //public async Task<IActionResult> View(string HerdCode)
        //{
        //    try
        //    {
        //        var farmer_pivot = (from a in _context.TblHerdFarmers
        //                            join b in _context.HBuffHerds on a.HerdId equals b.Id into Herd
        //                            from b in Herd.DefaultIfEmpty()
        //                            join c in _context.Tbl_Farmers on a.FarmerId equals c.Id into farmers
        //                            from c in farmers.DefaultIfEmpty()
        //                            select new
        //                            {
        //                                FarmerId = a.FarmerId,
        //                                FarmerName = c.LastName + ", " + c.FirstName,
        //                                FarmAddress = b.FarmAddress,
        //                                Photo = b.Photo,
        //                                CreatedBy = b.CreatedBy,
        //                                DateCreated = b.DateCreated

        //                            }).ToList();
        //        var farmer_list = FarmerHerdList2().Where(a => a.HerdCode == HerdCode).FirstOrDefault();
        //        var Feedinglist = _context.HFeedingSystems.ToList();
        //        var cowLevel = _context.ABuffAnimals.Where(buff => buff.FarmerId == farmer_list.FarmerId).ToList().Count();

        //        var item = new ViewBreedRegistryHerd();
        //        item.HerdCode = farmer_list.HerdCode;
        //        item.HerdName = farmer_list.HerdName;
        //        item.DateofApplication = farmer_list.DateofApplication;
        //        item.FarmerName = farmer_list.FarmerName;
        //        item.FarmAddress = farmer_list.FarmAddress;
        //        item.Photo = farmer_list.Photo;
        //        item.CreatedBy = farmer_list.CreatedBy;
        //        item.DateCreated = farmer_list.DateCreated;
        //        item.FarmerId = farmer_list.FarmerId;
        //        item.CowLevel = farmer_list.CowLevel;
        //        item.FarmManager = farmer_list.FarmManager;
        //        var farm = new List<ListFarmer>();
        //        for (int i = 0; i < farmer_pivot.Count; i++)
        //        {
        //            //var BreedList = _context.ABreeds.Where(a=>a.Id == farmer_pivot[i].BreedTypeId).FirstOrDefault();

        //            var b_item = new ListFarmer();
        //            b_item.Id = farmer_pivot[i].FarmerId;
        //            b_item.FarmerName = farmer_pivot[i].FarmerName;
        //            //b_item.BreedType = BreedList.BreedDesc;
        //            var feed = new List<FeedingType>();

        //            var feedinglist = (from a in _context.HFeedingSystems
        //                               join b in _context.tbl_FarmerFeedingSystem on a.Id equals b.FeedingSystem_Id into feeding
        //                               from b in feeding.DefaultIfEmpty()
        //                               select new
        //                               {
        //                                   feedingSystemDesc = a.FeedingSystemDesc,
        //                                   Farmer_Id = b.Farmer_Id

        //                               }).Where(farmowner => farmowner.Farmer_Id == farmer_pivot[i].FarmerId).ToList();
        //            for (int x = 0; x < feedinglist.Count; x++)
        //            {
        //                var f_item = new FeedingType();
        //                f_item.FeedingSystemDesc = feedinglist[x].feedingSystemDesc;
        //                feed.Add(f_item);

        //            }
        //            var breed = new List<BreedType>();

        //            var breedlist = (from a in _context.ABreeds
        //                             join b in _context.TblFarmerBreedTypes on a.Id equals b.BreedTypeId into breeds
        //                             from b in breeds.DefaultIfEmpty()
        //                             select new
        //                             {
        //                                 Breed_Desc = a.BreedDesc,
        //                                 Farmer_Id = b.FarmerId

        //                             }).Where(farmowner => farmowner.Farmer_Id == farmer_pivot[i].FarmerId).ToList();
        //            for (int x = 0; x < breedlist.Count; x++)
        //            {
        //                var bb_item = new BreedType();
        //                bb_item.Breed_Desc = breedlist[x].Breed_Desc;
        //                breed.Add(bb_item);

        //            }
        //            b_item.FeedingType = feed;
        //            b_item.BreedType = breed;
        //            b_item.CowLevel = cowLevel.ToString();
        //            farm.Add(b_item);


        //        }
        //        item.ListFarmer = farm;
        //        //var farmerHerds = await buildfarmerherd(searchFilter).ToListAsync();
        //        //var result = FormList(searchFilter, farmerHerds);
        //        return Ok(item);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Problem(ex.GetBaseException().ToString());
        //    }
        //}

        public class BuffHerdRegistryView
        {
            public int HerdId { get; set; }
            public string HerdCode { get; set; }
            public string HerdName { get; set; }
            public string FarmerId { get; set; }
            public string FarmerManagerName { get; set; }
            public string Center { get; set; }
            public string? Address { get; set; }
            public FarmerPaginationModel FarmerPagination { get; set; }
            public List<HerdFarmers> Farmers { get; set; }

        }

        public class HerdFarmers
        {
            public int FarmerId { get; set; }
            public string FarmerName { get; set; }
            public List<string> BreedType { get; set; }
            public List<string> FeedingSystem { get; set; }
            public string FarmerClassification { get; set; }
            public int CowLevel { get; set; }
        }

        public class FarmerPaginationModel
        {
            public string? CurrentPage { get; set; }
            public string? NextPage { get; set; }
            public string? PrevPage { get; set; }
            public string? TotalPage { get; set; }
            public string? PageSize { get; set; }
            public string? TotalRecord { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> view(string HerdCode)
        {
            try
            {
                //var buffHerd = (from herd in _context.HBuffHerds
                //                join farmer in _context.Tbl_Farmers on herd.FarmerId equals farmer.Id
                //                join herdfarmer in _context.TblHerdFarmers on farmer.Id equals herdfarmer.FarmerId into farmerGroup
                //                from herdfarmer in farmerGroup.DefaultIfEmpty()
                //                where herd.DeleteFlag == false && herd.HerdCode.Contains(HerdCode)

                //var buffHerd = (from herd in _context.HBuffHerds
                //                    //join farmer in _context.Tbl_Farmers on herd.FarmerId equals farmer.Id
                //                join farmer in _context.TblUsersModels on herd.FarmerId equals farmer.Id
                //                where herd.DeleteFlag == false && herd.HerdCode.Contains(HerdCode)

                var buffHerd = (from herd in _context.HBuffHerds
                                join farmer in _context.Tbl_Farmers on herd.FarmerId equals farmer.Id
                                join farmerUser in _context.TblUsersModels on farmer.User_Id equals farmerUser.Id
                                where herd.DeleteFlag == false && herd.HerdCode.Contains(HerdCode)

                                select new BuffHerdRegistryView
                                {
                                    HerdId = herd.Id,
                                    HerdCode = herd.HerdCode,
                                    HerdName = herd.HerdName,
                                    Center = herd.Center.ToString(),
                                    FarmerId = herd.FarmerId.ToString(),
                                    FarmerManagerName = farmerUser.Lname + ", " + farmerUser.Fname,
                                    Address = herd.FarmAddress,
                                }).Distinct().FirstOrDefault();

                if (buffHerd == null)
                {
                    return NotFound("Herd not found.");
                }

                var herdFarmersIds = _context.TblHerdFarmers
                    .Where(hf => hf.HerdId == buffHerd.HerdId)
                    .Select(hf => hf.FarmerId)
                    .ToList();

                var herdFarmersList = new List<HerdFarmers>();
                var farmerPagination = new List<FarmerPaginationModel>();

                int pagesize = 10;
                int page = 1;
                var items = (dynamic)null;
                int totalItems = 0;
                int totalPages = 0;

                var bloodCompList = herdFarmersIds.ToList();
                totalItems = bloodCompList.ToList().Count();
                totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
                items = bloodCompList.Skip((page - 1) * pagesize).Take(pagesize).ToList();
                //var bloodcompo = convertDataRowListToBloodCompList(items);
                var result = new List<FarmerPaginationModel>();
                var fitem = new FarmerPaginationModel();


                int pages = 1;
                fitem.CurrentPage = "1";
                int page_prev = pages - 1;

                double t_records = Math.Ceiling(Convert.ToDouble(totalItems) / Convert.ToDouble(pagesize));
                int page_next = pages + 1;
                fitem.NextPage = items.Count % pagesize >= 0 ? page_next.ToString() : "0";
                fitem.PrevPage = pages == 1 ? "0" : page_prev.ToString();
                fitem.TotalPage = t_records.ToString();
                fitem.PageSize = pagesize.ToString();
                fitem.TotalRecord = totalItems.ToString();

                foreach (var farmerId in herdFarmersIds)
                {
                    var farmer = _context.Tbl_Farmers.FirstOrDefault(f => f.Id == farmerId);
                    if (farmer != null)
                    {
                        var breedTypes = _context.TblFarmerBreedTypes
                            .Where(b => b.FarmerId == farmerId)
                            .Join(_context.ABreeds,
                                  b => b.BreedTypeId,
                                  bt => bt.Id,
                                  (b, bt) => bt.BreedDesc)
                            .ToList();

                        var feedingSystems = _context.tbl_FarmerFeedingSystem
                            .Where(fs => fs.Farmer_Id == farmerId)
                            .Join(_context.HFeedingSystems,
                                  fs => fs.FeedingSystem_Id,
                                  f => f.Id,
                                  (fs, f) => f.FeedingSystemDesc)
                            .ToList();

                        var classificationDesc = _context.HHerdClassifications.Where(c => c.HerdClassCode.Equals(farmer.FarmerClassification_Id.ToString())).FirstOrDefault();

                        herdFarmersList.Add(new HerdFarmers
                        {
                            FarmerId = farmer.Id,
                            FarmerName = farmer.FirstName + " " + farmer.LastName,
                            BreedType = breedTypes,
                            FeedingSystem = feedingSystems,
                            FarmerClassification = classificationDesc.HerdClassDesc,
                            CowLevel = _context.ABuffAnimals.Count(a => a.FarmerId == farmer.Id),
                        });
                    }
                }

                var item = new BuffHerdRegistryView
                {
                    HerdId = buffHerd.HerdId,
                    HerdCode = buffHerd.HerdCode,
                    HerdName = buffHerd.HerdName,
                    Center = buffHerd.Center,
                    FarmerId = buffHerd.FarmerId,
                    FarmerManagerName = buffHerd.FarmerManagerName,
                    Address = buffHerd.Address,
                    FarmerPagination = fitem,
                    Farmers = herdFarmersList 
                };

                return Ok(item);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }


        private IQueryable<BreedRegistryHerd2> buildfarmerherd(FarmerHerdSearch searchFilter)
        {
            // Get the base query as IQueryable
            IQueryable<BreedRegistryHerd2> query = FarmerHerdList().AsQueryable();

            // Apply search parameter if provided
            if (!string.IsNullOrWhiteSpace(searchFilter.center.ToString()) && searchFilter.center != 0)
            {
                query = query.Where(herd => herd.Center == searchFilter.center);
            }

            if (!string.IsNullOrWhiteSpace(searchFilter.searchParam))
            {
                query = query.Where(herd => herd.HerdName.Contains(searchFilter.searchParam));
            }

            // Apply date filter if provided
            if (!string.IsNullOrWhiteSpace(searchFilter.DateofApplication))
            {
                if (DateTime.TryParse(searchFilter.DateofApplication, out DateTime parsedDate))
                {
                    query = query.Where(herd => herd.DateofApplication >= parsedDate);
                }
            }

            // Apply sorting if specified
            if (!string.IsNullOrWhiteSpace(searchFilter.sortBy.Field))
            {
                // Use dynamic sorting based on the provided field and sort order
                string sortDirection = string.IsNullOrWhiteSpace(searchFilter.sortBy.Sort) ? "asc" : searchFilter.sortBy.Sort;
                query = query.OrderBy($"{searchFilter.sortBy.Field} {sortDirection}");
            }
            else
            {
                // Default sort by FarmerId descending
                query = query.OrderByDescending(herd => herd.HerdId);
            }

            return query;
        }

        private List<HerdFarmerPageModel> FormList(FarmerHerdSearch searchFilter, List<BreedRegistryHerd2> farmerherdlist)
        {


            int pagesize = searchFilter.pageSize == 0 ? 10 : searchFilter.pageSize;
            int page = searchFilter.page == 0 ? 1 : searchFilter.page;
            var items = (dynamic)null;

            int totalItems = farmerherdlist.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
            items = farmerherdlist.Skip((page - 1) * pagesize).Take(pagesize).ToList();
            //var herdModels = convertDataRowListToHerdModelList(items);

            var results = new List<HerdFarmerPageModel>();
            var item = new HerdFarmerPageModel();

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
            results.Add(item);

            return results;

        }
        private HBuffHerd buildBuffHerd(BuffHerdBaseModel registrationModel)
        {
            var BuffHerdModel = new HBuffHerd()
            {
                HerdName = registrationModel.HerdName,
                HerdCode = registrationModel.HerdCode,
                HerdSize = registrationModel.HerdSize,
                GroupId = registrationModel.GroupId,
                //FarmAffilCode = registrationModel.FarmAffilCode,
                FarmAffilCode = "0",
                HerdClassDesc = "0",
                FarmManager = registrationModel.FarmManager,
                FarmAddress = registrationModel.FarmAddress,
                FarmerId = int.Parse(registrationModel.FarmManager),
                Owner = int.Parse(registrationModel.FarmManager),
                //OrganizationName = registrationModel.OrganizationName,
                OrganizationName = null,
                Center = int.Parse(registrationModel.Center),
                Photo = registrationModel.Photo
            };

            return BuffHerdModel;
        }
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HBuffHerd>> Save(BuffHerdRegistrationModel registrationModel)
        {
            int? farmerGeneratedId = 0;
            var farmerDetails = new TblUsersModel();
            try
            {
                DataTable buffHerdDuplicateCheck = db.SelectDb_WithParamAndSorting(QueryBuilder.buildHerdDuplicateCheckSaveQuery(), null, populateSqlParameters(registrationModel.HerdName, registrationModel.HerdCode));

                if (buffHerdDuplicateCheck.Rows.Count > 0)
                {
                    status = "Herd already exists";
                    return Conflict(status);
                }

                var isfarmer = _context.Tbl_Farmers.Where(f => f.User_Id == int.Parse(registrationModel.FarmManager)).FirstOrDefault();

                if (isfarmer != null)
                {
                    farmerGeneratedId = isfarmer.Id;
                }
                else
                {
                    farmerDetails = _context.TblUsersModels.FirstOrDefault(f => f.Id == int.Parse(registrationModel.FarmManager));

                    var farmer = new TblFarmers
                    {
                        User_Id = int.Parse(registrationModel.FarmManager),
                        FirstName = farmerDetails.Fname ?? "",
                        LastName = farmerDetails.Lname ?? "",
                        Group_Id = registrationModel.GroupId,
                        Is_Manager = true,
                        FarmerClassification_Id = 0,
                        FarmerAffliation_Id = 0,
                        Address = farmerDetails.Address ?? registrationModel.FarmAddress,
                        Created_By = int.Parse(registrationModel.CreatedBy),
                        Created_At = DateTime.Now,
                        Is_Deleted = false
                    };

                    _context.Tbl_Farmers.Add(farmer);
                    await _context.SaveChangesAsync();

                    //farmerGeneratedId = farmer.User_Id;
                    farmerGeneratedId = farmer.Id;
                }

                registrationModel.FarmManager = farmerGeneratedId.ToString();


                var BuffHerdModel = buildBuffHerd(registrationModel);
                DataTable farmOwnerRecordsCheck = db.SelectDb_WithParamAndSorting(QueryBuilder.buildFarmOwnerSearchQueryByFirstNameAndLastName(), null, populateSqlParametersFarmer(registrationModel.Owner));



                //populateFeedingSystemAndBuffaloType(BuffHerdModel, registrationModel);

                BuffHerdModel.CreatedBy = registrationModel.CreatedBy;
                BuffHerdModel.DateCreated = DateTime.Now;

                _context.HBuffHerds.Add(BuffHerdModel);
                await _context.SaveChangesAsync();




                status = "Herd successfully registered!";
                dbmet.InsertAuditTrailv2("Save Buffalo Herd" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Herd Module", registrationModel.CreatedBy, "0", farmerGeneratedId.ToString());

                return Ok(status);
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Save Buffalo Herd" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Herd Module", registrationModel.CreatedBy, "0");

                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, BuffHerdUpdateModel registrationModel)
        {


            DataTable buffHerdDataTable = db.SelectDb_WithParamAndSorting(QueryBuilder.buildHerdSelectQueryById(), null, populateSqlParameters(id));

            if (buffHerdDataTable.Rows.Count == 0)
            {
                status = "No records matched!";
                return Conflict(status);
            }


            //var buffHerd = convertDataRowToHerdModel(buffHerdDataTable.Rows[0]);

            DataTable buffHerdDuplicateCheck = db.SelectDb_WithParamAndSorting(QueryBuilder.buildHerdSelectDuplicateQueryByIdHerdNameHerdCode(), null, populateSqlParameters(id, registrationModel));

            // check for duplication
            if (buffHerdDuplicateCheck.Rows.Count > 0)
            {
                status = "Entity already exists";
                return Conflict(status);
            }

            var buffHerd = _context.HBuffHerds
                    .Single(x => x.Id == id);





            try
            {

                buffHerd = populateBuffHerd(buffHerd, registrationModel);


                _context.Entry(buffHerd).State = EntityState.Modified;
                _context.SaveChanges();
                status = "Update Successful!";
                dbmet.InsertAuditTrailv2("Update Buffalo Herd" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Herd Module", buffHerd.UpdatedBy, "0",id.ToString());
                return Ok(status);
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }
        private void populateFeedingSystemAndBuffaloType(HBuffHerd buffHerd, BuffHerdBaseModel baseModel)
        {
            var buffaloTypes = new List<HBuffaloType>();
            var feedingSystems = new List<HFeedingSystem>();
            int counter = 0;

            string breed = $@"
                                SELECT [Join_BuffHerd_Id]
                                      ,[Buffalo_Type_Id]
                                      ,[Buff_Herd_id]
                                      ,[Feeding_System_Id]
                                  FROM [dbo].[tbl_Join_BuffHerd] where Buff_Herd_id ='" + buffHerd.Id + "'";
            DataTable breed_tbl = db.SelectDb(breed).Tables[0];
            if (breed_tbl.Rows.Count != 0)
            {

                string delete = $@"DELETE FROM [dbo].[tbl_Join_BuffHerd] where Buff_Herd_id ='" + buffHerd.Id + "'";
                db.DB_WithParam(delete);
                foreach (string breedTypeCode in baseModel.BreedTypeCodes)
                {
                    string user_insert = $@"INSERT INTO [dbo].[tbl_Join_BuffHerd]
                                   ([Buffalo_Type_Id]
                                   ,[Buff_Herd_id]
                                   ,[Feeding_System_Id])
                             VALUES
                                  ('" + breedTypeCode + "'" +
                                     ",'" + buffHerd.Id + "'" +
                                    ",'0')";
                    string test = db.DB_WithParam(user_insert);
                }
                foreach (string feedingSystemCode in baseModel.FeedingSystemCodes)
                {
                    string user_insert = $@"INSERT INTO [dbo].[tbl_Join_BuffHerd]
                                     ([Feeding_System_Id]
                                       ,[Buff_Herd_id]
                                       ,[Buffalo_Type_Id])
                             VALUES
                                  ('" + feedingSystemCode + "'" +
                                     ",'" + buffHerd.Id + "'" +
                                 ",'0')";
                    string test = db.DB_WithParam(user_insert);
                }
            }
            else
            {
                foreach (string breedTypeCode in baseModel.BreedTypeCodes)
                {
                    string user_insert = $@"INSERT INTO [dbo].[tbl_Join_BuffHerd]
                                     ([Buffalo_Type_Id]
                                       ,[Buff_Herd_id]
                                       ,[Feeding_System_Id])
                             VALUES
                                  ('" + breedTypeCode + "'" +
                                     ",'" + buffHerd.Id + "'" +
                                    ",'0')";
                    string test = db.DB_WithParam(user_insert);
                }
                foreach (string feedingSystemCode in baseModel.FeedingSystemCodes)
                {
                    string user_insert = $@"INSERT INTO [dbo].[tbl_Join_BuffHerd]
                                    ([Feeding_System_Id]
                                       ,[Buff_Herd_id]
                                       ,[Buffalo_Type_Id])
                             VALUES
                                  ('" + feedingSystemCode + "'" +
                                     ",'" + buffHerd.Id + "'" +
                                 ",'0')";
                    string test = db.DB_WithParam(user_insert);
                }
            }


        }
        private HBuffHerd populateBuffHerd(HBuffHerd buffHerd, BuffHerdUpdateModel updateModel)
        {

            if (updateModel.HerdName != null && updateModel.HerdName != "")
            {
                buffHerd.HerdName = updateModel.HerdName;
            }
            if (updateModel.HerdCode != null && updateModel.HerdCode != "")
            {
                buffHerd.HerdCode = updateModel.HerdCode;
            }
            if (updateModel.FarmAffilCode != null && updateModel.FarmAffilCode != "")
            {
                buffHerd.FarmAffilCode = updateModel.FarmAffilCode;
            }
            if (updateModel.HerdClassDesc != null && updateModel.HerdClassDesc != "")
            {
                buffHerd.HerdClassDesc = updateModel.HerdClassDesc;
            }
            if (updateModel.FarmManager != null && updateModel.FarmManager != "")
            {
                buffHerd.FarmManager = updateModel.FarmManager;
            }
            if (updateModel.FarmAddress != null && updateModel.FarmAddress != "")
            {
                buffHerd.FarmAddress = updateModel.FarmAddress;
            }
            if (updateModel.OrganizationName != null && updateModel.OrganizationName != "")
            {
                buffHerd.OrganizationName = updateModel.OrganizationName;
            }
            if (updateModel.Photo != null && updateModel.Photo != "")
            {
                buffHerd.Photo = updateModel.Photo;
            }
            if (updateModel.GroupId != null && updateModel.GroupId != null)
            {
                buffHerd.GroupId = updateModel.GroupId;
            }
            return buffHerd;
        }
        private TblFarmOwner convertDataRowToFarmOwnerEntity(DataRow dataRow)
        {
            var farmOwner = DataRowToObject.ToObject<TblFarmOwner>(dataRow);

            return farmOwner;
        }

        private SqlParameter[] populateSqlParameters(String herdCode)
        {

            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "HerdCode",
                Value = herdCode ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            return sqlParameters.ToArray();
        }

        private SqlParameter[] populateSqlParameters(String herdCode, String herdName)
        {

            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "HerdCode",
                Value = herdCode ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "HerdName",
                Value = herdName ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            return sqlParameters.ToArray();
        }

        private SqlParameter[] populateSqlParametersHerdClassDesc(String herdClassDesc)
        {

            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "HerdClassDesc",
                Value = herdClassDesc ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            return sqlParameters.ToArray();
        }

        private SqlParameter[] populateSqlParameters(int id)
        {

            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "Id",
                Value = id,
                SqlDbType = System.Data.SqlDbType.Int,
            });

            return sqlParameters.ToArray();
        }

        private SqlParameter[] populateSqlParameters(int id, BuffHerdUpdateModel registrationModel)
        {

            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "Id",
                Value = id,
                SqlDbType = System.Data.SqlDbType.Int,
            });


            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "HerdName",
                Value = registrationModel.HerdName ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });


            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "HerdCode",
                Value = registrationModel.HerdCode ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            return sqlParameters.ToArray();
        }

        private SqlParameter[] populateSqlParametersFarmer(Owner owner)
        {

            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "FirstName",
                Value = owner.FirstName ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });


            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "LastName",
                Value = owner.LastName ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            return sqlParameters.ToArray();
        }
        private SqlParameter[] populateSqlParametersBuffaloType(String breedTypeCode)
        {

            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "breedTypeCode",
                Value = breedTypeCode ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            return sqlParameters.ToArray();
        }
        private SqlParameter[] populateSqlParametersFeedingSystem(String feedingSystemCode)
        {

            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "FeedCode",
                Value = feedingSystemCode ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            return sqlParameters.ToArray();
        }

        [HttpPost]
        public async Task<IActionResult> ArchiveMultiple(List<DeletionModel> deletionModelList)
        {
            if (_context.HBuffHerds == null)
            {
                return Problem("Entity set 'PCC_DEVContext.HBuffHerds' is null!");
            }

            try
            {
                var idsToCheck = deletionModelList.Select(d => d.id).ToList();
                var tblHBuffHerdsModels = await _context.HBuffHerds
                    .Where(model => idsToCheck.Contains(model.Id))
                    .ToListAsync();

                var recordsToDelete = tblHBuffHerdsModels
                    .Where(model => !model.DeleteFlag)
                    .ToList();

                if (recordsToDelete.Count != idsToCheck.Count)
                {
                    return Conflict("Some records have no match or are already marked for deletion!");
                }

                //foreach (var tblCenterModel in recordsToDelete)
                //{
                //    bool centerNameExistsInBuffHerd = _context.HBuffHerds
                //        .Any(buffHerd => !buffHerd.DeleteFlag && buffHerd.Center == tblCenterModel.Id);

                //    if (centerNameExistsInBuffHerd)
                //    {
                //        return Conflict("One or more records are used by other tables!");
                //    }
                //}

                foreach (DeletionModel deletionModel in deletionModelList)
                {
                    var tblHBuffHerdsModel = tblHBuffHerdsModels.FirstOrDefault(model => model.Id == deletionModel.id);

                    if (tblHBuffHerdsModel == null)
                    {
                        continue;
                    }

                    tblHBuffHerdsModel.DeleteFlag = true;
                    tblHBuffHerdsModel.DateDeleted = DateTime.Now;
                    tblHBuffHerdsModel.DeletedBy = deletionModel.deletedBy;
                    tblHBuffHerdsModel.DateRestored = null;
                    tblHBuffHerdsModel.RestoredBy = "";

                    status = "Delete Successful!";
                    dbmet.InsertAuditTrailv2("Delete Buffalo Herd" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Herd Module", tblHBuffHerdsModel.DeletedBy, "0", deletionModel.id.ToString());

                    _context.Entry(tblHBuffHerdsModel).State = EntityState.Modified;
                }

                await _context.SaveChangesAsync();

                return Ok("Deletion Successful!");
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpPost]
        public async Task<IActionResult> ArchiveSingle(DeletionModel deletionModel)
        {
            if (_context.HBuffHerds == null)
            {
                return Problem("Entity set 'PCC_DEVContext.HBuffHerds' is null!");
            }

            try
            {

                var tblHBuffHerdModel = _context.HBuffHerds.FirstOrDefault(model => model.Id == deletionModel.id && !model.DeleteFlag);

                if (tblHBuffHerdModel == null)
                {
                    return Conflict("Records have no match or are already marked for deletion!");
                }

                tblHBuffHerdModel.DeleteFlag = true;
                tblHBuffHerdModel.DateDeleted = DateTime.Now;
                tblHBuffHerdModel.DeletedBy = deletionModel.deletedBy;
                tblHBuffHerdModel.DateRestored = null;
                tblHBuffHerdModel.RestoredBy = "";
                _context.Entry(tblHBuffHerdModel).State = EntityState.Modified;


                await _context.SaveChangesAsync();

                status = "Delete Successful!";
                dbmet.InsertAuditTrailv2("Delete Buffalo Herd" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Herd Module", tblHBuffHerdModel.DeletedBy, "0", deletionModel.id.ToString());


                return Ok("Deletion Successful!");
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpPost]
        public async Task<IActionResult> RestoreMultiple(List<RestorationModel> restorationModelList)
        {
            if (_context.HBuffHerds == null)
            {
                return Problem("Entity set 'PCC_DEVContext.HBuffHerds' is null!");
            }

            try
            {
                var idsToCheck = restorationModelList.Select(d => d.id).ToList();
                var tblHBuffHerdModels = await _context.HBuffHerds
                    .Where(model => idsToCheck.Contains(model.Id))
                    .ToListAsync();

                var recordsToRestore = tblHBuffHerdModels
                    .Where(model => model.DeleteFlag)
                    .ToList();

                if (recordsToRestore.Count != idsToCheck.Count)
                {
                    return Conflict("Some records have no match or are already active!");
                }

                //foreach (var tblCenterModel in recordsToDelete)
                //{
                //    bool centerNameExistsInBuffHerd = _context.HBuffHerds
                //        .Any(buffHerd => !buffHerd.DeleteFlag && buffHerd.Center == tblCenterModel.Id);

                //    if (centerNameExistsInBuffHerd)
                //    {
                //        return Conflict("One or more records are used by other tables!");
                //    }
                //}

                foreach (RestorationModel restorationModel in restorationModelList)
                {
                    var tblHBuffHerdModel = tblHBuffHerdModels.FirstOrDefault(model => model.Id == restorationModel.id);

                    if (tblHBuffHerdModel == null)
                    {
                        continue;
                    }

                    tblHBuffHerdModel.DeleteFlag = false;
                    tblHBuffHerdModel.DateDeleted = null;
                    tblHBuffHerdModel.DeletedBy = "";
                    tblHBuffHerdModel.DateRestored = DateTime.Now;
                    tblHBuffHerdModel.RestoredBy = restorationModel.restoredBy;

                    status = "Restore Successful!";
                    dbmet.InsertAuditTrailv2("Restore Buffalo Herd" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Herd Module", tblHBuffHerdModel.RestoredBy, "0", restorationModel.id.ToString());


                    _context.Entry(tblHBuffHerdModel).State = EntityState.Modified;
                }

                await _context.SaveChangesAsync();

                return Ok("Restoration Successful!");
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }
        [HttpPost]
        public async Task<IActionResult> RestoreSingle(RestorationModel restorationModel)
        {
            if (_context.HBuffHerds == null)
            {
                return Problem("Entity set 'PCC_DEVContext.HBuffHerds' is null!");
            }

            try
            {

                var tblHBuffHerdModel = _context.HBuffHerds.FirstOrDefault(model => model.Id == restorationModel.id && model.DeleteFlag);

                if (tblHBuffHerdModel == null)
                {
                    return Conflict("Records have no match or are already marked for deletion!");
                }

                tblHBuffHerdModel.DeleteFlag = false;
                tblHBuffHerdModel.DateDeleted = null;
                tblHBuffHerdModel.DeletedBy = "";
                tblHBuffHerdModel.DateRestored = DateTime.Now;
                tblHBuffHerdModel.RestoredBy = restorationModel.restoredBy;

                status = "Restore Successful!";
                dbmet.InsertAuditTrailv2("Restore Buffalo Herd" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Herd Module", tblHBuffHerdModel.RestoredBy, "0", restorationModel.id.ToString());


                _context.Entry(tblHBuffHerdModel).State = EntityState.Modified;


                await _context.SaveChangesAsync();

                return Ok("Restoration Successful!");
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }
        [HttpPost]
        public async Task<ActionResult<HBuffHerd>> Import(List<BuffHerdRegistrationModel> registrationModel)
        {

            List<HBuffHerd> listOfImportedbuffHerd = new List<HBuffHerd>();

            string filePath = @"C:\data\savebuffanimal.json"; // Replace with your desired file path

            try
            {
                for (int x = 0; x < registrationModel.Count; x++)
                {

                    int? farmerGeneratedId = 0;
                    var farmerDetails = new TblUsersModel();
                    try
                    {

                        DataTable buffHerdDuplicateCheck = db.SelectDb_WithParamAndSorting(QueryBuilder.buildHerdDuplicateCheckSaveQuery(), null, populateSqlParameters(registrationModel[x].HerdName, registrationModel[x].HerdCode));

                        if (buffHerdDuplicateCheck.Rows.Count > 0)
                        {
                            status = "Herd already exists";
                            return Conflict(status);
                        }

                        var isfarmer = _context.Tbl_Farmers.Where(f => f.User_Id == int.Parse(registrationModel[x].FarmManager)).FirstOrDefault();

                        if (isfarmer != null)
                        {
                            farmerGeneratedId = isfarmer.Id;
                        }
                        else
                        {
                            farmerDetails = _context.TblUsersModels.FirstOrDefault(f => f.Id == int.Parse(registrationModel[x].FarmManager));

                            var farmer = new TblFarmers
                            {
                                User_Id = int.Parse(registrationModel[x].FarmManager),
                                FirstName = farmerDetails.Fname ?? "",
                                LastName = farmerDetails.Lname ?? "",
                                Group_Id = registrationModel[x].GroupId,
                                Is_Manager = true,
                                FarmerClassification_Id = 0,
                                FarmerAffliation_Id = 0,
                                Address = farmerDetails.Address ?? registrationModel[x].FarmAddress,
                                Created_By = int.Parse(registrationModel[x].CreatedBy),
                                Created_At = DateTime.Now,
                                Is_Deleted = false
                            };

                            _context.Tbl_Farmers.Add(farmer);
                            await _context.SaveChangesAsync();

                            //farmerGeneratedId = farmer.User_Id;
                            farmerGeneratedId = farmer.Id;
                        }

                        registrationModel[x].FarmManager = farmerGeneratedId.ToString();


                        var BuffHerdModel = buildBuffHerd(registrationModel[x]);
                        DataTable farmOwnerRecordsCheck = db.SelectDb_WithParamAndSorting(QueryBuilder.buildFarmOwnerSearchQueryByFirstNameAndLastName(), null, populateSqlParametersFarmer(registrationModel[x].Owner));



                        //populateFeedingSystemAndBuffaloType(BuffHerdModel, registrationModel);

                        BuffHerdModel.CreatedBy = registrationModel[x].CreatedBy;
                        BuffHerdModel.DateCreated = DateTime.Now;

                        _context.HBuffHerds.Add(BuffHerdModel);
                        await _context.SaveChangesAsync();




                        status = "Herd successfully registered!";
                        dbmet.InsertAuditTrailv2("Save Buffalo Herd" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Herd Module", registrationModel[x].CreatedBy, "0", farmerGeneratedId.ToString());

                        return Ok(status);

                    }
                    catch (Exception ex)
                    {
                        dbmet.InsertAuditTrail("Save Buffalo Herd" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Herd Module", registrationModel[x].CreatedBy, "0");

                        return Problem(ex.GetBaseException().ToString());
                    }
                }
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.GetBaseException().ToString());
            }
            return CreatedAtAction("import", listOfImportedbuffHerd);
        }

        [HttpGet]
        public async Task<IActionResult> Export()
        {
            FarmerHerdSearch searchFilter = new FarmerHerdSearch();
            searchFilter.searchParam = "";
            searchFilter.DateofApplication = "";
            SortByModel sortby = new SortByModel();
            sortby.Field = "";
            sortby.Sort = "";
            searchFilter.sortBy = sortby;
            try
            {
                var farmerHerds = await buildfarmerherd(searchFilter).ToListAsync();
                var result = FormList(searchFilter, farmerHerds);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }
    }
}
