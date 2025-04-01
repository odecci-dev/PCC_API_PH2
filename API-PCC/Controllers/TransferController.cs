using API_PCC.ApplicationModels;
using API_PCC.ApplicationModels.Common;
using API_PCC.Data;
using API_PCC.EntityModels;
using API_PCC.Manager;
using API_PCC.Models;
using API_PCC.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Core.Types;
using System.Data;
using System.Data.SqlClient;
using static API_PCC.Controllers.BuffAnimalsController;

namespace API_PCC.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private readonly PCC_DEVContext _context;
        DBMethods dbmet = new DBMethods();
        public TransferController(PCC_DEVContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<ActionResult<IEnumerable<TransferModel>>> Export()
        {
            bool isArchive = false;
            try
            {
                List<TransferModel> transferList = await buildTransferModelSearchQueryv1(isArchive).ToListAsync();

                var result = buildTransferPagedModelv2(transferList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }
        private List<TransferPaginationModel> buildTransferPagedModelv2(List<TransferModel> transferList)
        {
            var searchFilter = 0;
            int pagesize = searchFilter == 0 ? 10 : searchFilter;
            int page = searchFilter == 0 ? 1 : searchFilter;
            var items = (dynamic)null;


            //List<TransferModel> TransferModel = await buildTransferModelSearchQueryv2().Where(a => a.DeleteFlag == false).ToListAsync();
            int totalItems = transferList.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pagesize);

            items = transferList.Skip((page - 1) * pagesize).Take(pagesize).ToList();

            //var herdModels = convertDataRowListToHerdModelList(items);
            //List<BuffHerdListResponseModel> buffHerdBaseModels = convertBuffHerdToResponseModelList(buffHerdList);

            var result = new List<TransferPaginationModel>();
            var item = new TransferPaginationModel();

            int pages = searchFilter == 0 ? 1 : searchFilter;
            item.CurrentPage = searchFilter == 0 ? "1" : searchFilter.ToString();
            int page_prev = pages - 1;

            double t_records = Math.Ceiling(Convert.ToDouble(totalItems) / Convert.ToDouble(pagesize));
            int page_next = searchFilter >= t_records ? 0 : pages + 1;
            item.NextPage = items.Count % pagesize >= 0 ? page_next.ToString() : "0";
            item.PrevPage = pages == 1 ? "0" : page_prev.ToString();
            item.TotalPage = t_records.ToString();
            item.PageSize = pagesize.ToString();
            item.TotalRecord = totalItems.ToString();
            item.items = transferList;
            result.Add(item);

            return result;
        }
        [HttpPost]
        public async Task<IActionResult> RestoreMultiple(List<RestorationModel> restorationModel)
        {
            string status = "";
            if (_context.TransferModels == null)
            {
                return NotFound();
            }
            for (int x = 0; x < restorationModel.Count; x++)
            {


                var transfers = await _context.TransferModels.FindAsync(restorationModel[x].id);
                if (transfers == null || !transfers.DeleteFlag)
                {
                    return Conflict("No records matched!");
                }

                try
                {
                    transfers.DeleteFlag = false;
                    transfers.DateDeleted = null;
                    transfers.DeletedBy = restorationModel[x].restoredBy;
                    transfers.DateRestored = DateTime.Now;
                    transfers.RestoredBy = "";
                    _context.Entry(transfers).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    dbmet.InsertAuditTrail("Restores Transfer Id: " + transfers.Id + "", DateTime.Now.ToString("yyyy-MM-dd"), "Transfer Module", transfers.RestoredBy, "0");

                    status = "Restoration Successful!";
                }
                catch (Exception ex)
                {

                    return Problem(ex.GetBaseException().ToString());
                }
            }
            return Ok(status);
        }
        [HttpPost]
        public async Task<ActionResult<IEnumerable<TransferModel>>> TransferArchive(animalsearchfilter searchFilter)
        {
            bool isArchive = true;
            try
            {
                List<TransferModel> transferList = await buildTransferModelSearchQueryv2(isArchive, searchFilter).ToListAsync();

                var result = buildTransferPagedModelv2(transferList);
                return Ok(result);
            
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }
        private IQueryable<TransferModel> buildTransferModelSearchQueryv2(bool isArchive , animalsearchfilter searchFilter)
        {
            IQueryable<TransferModel> query = _context.TransferModels;

            if (isArchive)
                query = query.Where(transfer => transfer.DeleteFlag);

            query = query
                .Include(transferModel => transferModel.Animal)
                .Include(transferModel => transferModel.Owner);
            if (!searchFilter.searchParam.IsNullOrEmpty())
                query = query.Where(animal =>
                               animal.transferNumber.Contains(searchFilter.searchParam));


            return query;
        }
        private IQueryable<TransferModel> buildTransferModelSearchQueryv1(bool isArchive)
        {
            IQueryable<TransferModel> query = _context.TransferModels;

            if (isArchive)
                query = query.Where(transfer => transfer.DeleteFlag);

            query = query
                .Include(transferModel => transferModel.Animal)
                .Include(transferModel => transferModel.Owner);
         


            return query;
        }
        // POST: BirthTypes/list
        [HttpPost]
        public async Task<ActionResult<IEnumerable<TransferModel>>> list(CommonSearchFilterModel searchFilter)
        {
            try
            {
                List<TransferModel> transferModels = await buildTransferModelSearchQuery(searchFilter).ToListAsync();
                var result = buildTransferPagedModel(searchFilter, transferModels);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // GET: Transfer/search/5
        [HttpGet]
        public async Task<ActionResult<ABirthType>> search(string transfernumber)
        {
            if (_context.TransferModels == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TransferModels' is null!");
            }
            var transferModel = await _context.TransferModels
                .Include(transferModel => transferModel.Animal)
                .Include(transferModel => transferModel.Owner)
                .Where(transferModel => transferModel.transferNumber.Equals(transfernumber))
                .FirstOrDefaultAsync();

            if (transferModel == null)
            {
                return Conflict("No records found!");
            }
            return Ok(transferModel);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteMultiple(List<DeletionModel> deletionModel)
        {
            string status = "";
            if (_context.TransferModels == null)
            {
                return NotFound();
            }
            for (int x = 0; x < deletionModel.Count; x++)
            {


                var transfers = await _context.TransferModels.FindAsync(deletionModel[x].id);
                if (transfers == null || transfers.DeleteFlag)
                {
                    return Conflict("No records matched!");
                }

                try
                {
                    transfers.DeleteFlag = true;
                    transfers.DateDeleted = DateTime.Now;
                    transfers.DeletedBy = deletionModel[x].deletedBy;
                    transfers.DateRestored = null;
                    transfers.RestoredBy = "";
                    _context.Entry(transfers).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    dbmet.InsertAuditTrail("Restores Transfer Id: " + transfers.Id + "", DateTime.Now.ToString("yyyy-MM-dd"), "Transfer Module", transfers.DeletedBy, "0");

                    status = "Deletion Successful!";
                }
                catch (Exception ex)
                {

                    return Problem(ex.GetBaseException().ToString());
                }
            }
            return Ok(status);
        }

        // PUT: TransferModel/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{id}")]
        public async Task<IActionResult> update(int id, TransferUpdateModel transferUpdateModel)
        {
            if (_context.TransferModels == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TransferModels' is null!");
            }

            var transferModel = _context.TransferModels
                                    .Include(transferModel => transferModel.Animal)
                                    .Include(transferModel => transferModel.Owner)
                                    .AsNoTracking().Where(transferModel => transferModel.Id == id)
                                    .FirstOrDefault();

            if (transferModel == null)
            {
                return Conflict("No records matched!");
            }

            if (id != transferModel.Id)
            {
                return Conflict("Ids mismatched!");
            }

            var animalRecord = _context.ABuffAnimals.Where(animal => animal.breedRegistryNumber.Equals(transferUpdateModel.BreedRegistrationNumber)
                                                                     && animal.AnimalIdNumber.Equals(transferUpdateModel.AnimalIdNumber)).FirstOrDefault();

            if (animalRecord == null)
            {
                return Conflict("No animal found for registry number: " + transferUpdateModel.BreedRegistrationNumber + " and animal Id number: " + transferUpdateModel.AnimalIdNumber);
            }

            var farmOwner = _context.TblFarmOwners.Where(owner => (owner.FirstName + " " + owner.LastName).Equals(transferUpdateModel.Owner)).FirstOrDefault();

            if (farmOwner == null)
            {
                return Conflict("No Owner found with name: " + transferUpdateModel.Owner);
            }

            bool hasDuplicateOnUpdate = (_context.TransferModels?.Any(transferModel => transferModel.Animal.Equals(animalRecord) &&
                                                                                    transferModel.Id != id)).GetValueOrDefault();

            // check for duplication
            if (hasDuplicateOnUpdate)
            {
                return Conflict("Entity already exists");
            }

            try
            {
                transferModel.Animal = animalRecord;
                transferModel.Owner = farmOwner;
                transferModel.Address = transferUpdateModel.Address;
                transferModel.TelephoneNumber = transferUpdateModel.TelephoneNumber;
                transferModel.MobileNumber = transferUpdateModel.MobileNumber;
                transferModel.Email = transferUpdateModel.Email;
                transferModel.UpdatedBy = transferUpdateModel.UpdatedBy;
                transferModel.DateUpdated = DateTime.Now;

                _context.Entry(transferModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Update Successful!");
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }

        // POST: Transfer/save
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TransferBaseModel>> save(TransferSaveModel transferSaveModel)
        {
            if (_context.TransferModels == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TransferModels' is null!");
            }

            var animalRecord = _context.ABuffAnimals.Where(animal => animal.breedRegistryNumber.Equals(transferSaveModel.BreedRegistrationNumber)
                                                                     && animal.AnimalIdNumber.Equals(transferSaveModel.AnimalIdNumber)).FirstOrDefault();

            if (animalRecord == null)
            {
                return Conflict("No animal found for registry number: " + transferSaveModel.BreedRegistrationNumber + " and animal Id number: " + transferSaveModel.AnimalIdNumber);
            }

            var farmOwner = _context.TblFarmOwners.Where(owner => (owner.FirstName + " " + owner.LastName).Equals(transferSaveModel.Owner)).FirstOrDefault();

            if (farmOwner == null)
            {
                return Conflict("No Owner found with name: " + transferSaveModel.Owner);
            }

            bool hasDuplicateOnSave = (_context.TransferModels?.Any(transferModel => transferModel.Animal.Equals(animalRecord))
                                        ).GetValueOrDefault();

            // check for duplication
            if (hasDuplicateOnSave)
            {
                return Conflict("Entity already exists");
            }

            try
            {
                var transferModel = new TransferModel()
                {
                    transferNumber = transferSaveModel.transferNumber,
                    Animal = animalRecord,
                    Owner = farmOwner,
                    Address = transferSaveModel.Address,
                    TelephoneNumber = transferSaveModel.TelephoneNumber,
                    MobileNumber = transferSaveModel.MobileNumber,
                    Email = transferSaveModel.Email,
                    CreatedBy = transferSaveModel.CreatedBy,
                    DateCreated = DateTime.Now,
                    Status = 1,
                    DeleteFlag = false
                };

                _context.TransferModels.Add(transferModel);
                await _context.SaveChangesAsync();

                return Ok("Transfer successfully registered!");
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }
        [HttpPost]
        public async Task<ActionResult<TransferBaseModel>> import(List<TransferSaveModel> transferSaveModel)
        {
            string res = "";
            if (_context.TransferModels == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TransferModels' is null!");
            }
            try
            {
                for (int x = 0; x < transferSaveModel.Count; x++)
                {
                    var animalRecord = _context.ABuffAnimals.Where(animal => animal.breedRegistryNumber.Equals(transferSaveModel[x].BreedRegistrationNumber)
                                                                             && animal.AnimalIdNumber.Equals(transferSaveModel[x].AnimalIdNumber)).FirstOrDefault();

                    if (animalRecord == null)
                    {
                        return Conflict("No animal found for registry number: " + transferSaveModel[x].BreedRegistrationNumber + " and animal Id number: " + transferSaveModel[x].AnimalIdNumber);
                    }

                    var farmOwner = _context.TblFarmOwners.Where(owner => (owner.FirstName + " " + owner.LastName).Equals(transferSaveModel[x].Owner)).FirstOrDefault();

                    if (farmOwner == null)
                    {
                        return Conflict("No Owner found with name: " + transferSaveModel[x].Owner);
                    }

                    bool hasDuplicateOnSave = (_context.TransferModels?.Any(transferModel => transferModel.Animal.Equals(animalRecord))
                                                ).GetValueOrDefault();

                    // check for duplication
                    if (hasDuplicateOnSave)
                    {
                        return Conflict("Entity already exists");
                    }

                    try
                    {
                        var transferModel = new TransferModel()
                        {
                            transferNumber = transferSaveModel[x].transferNumber,
                            Animal = animalRecord,
                            Owner = farmOwner,
                            Address = transferSaveModel[x].Address,
                            TelephoneNumber = transferSaveModel[x].TelephoneNumber,
                            MobileNumber = transferSaveModel[x].MobileNumber,
                            Email = transferSaveModel[x].Email,
                            CreatedBy = transferSaveModel[x].CreatedBy,
                            DateCreated = DateTime.Now,
                            Status = 1,
                            DeleteFlag = false
                        };

                        _context.TransferModels.Add(transferModel);
                        await _context.SaveChangesAsync();
                        res = "Transfer successfully registered!";


                    }
                    catch (Exception ex)
                    {
                        return Problem(ex.GetBaseException().ToString());
                    }
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // POST: Transfer/delete/
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> delete(DeletionModel deletionModel)
        {

            if (_context.TransferModels == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TransferModels' is null!");
            }

            var transferModel = await _context.TransferModels.FindAsync(deletionModel.id);
            if (transferModel == null || transferModel.DeleteFlag)
            {
                return Conflict("No records matched!");
            }

            try
            {
                transferModel.DeleteFlag = true;
                transferModel.DateDeleted = DateTime.Now;
                transferModel.DeletedBy = deletionModel.deletedBy;
                transferModel.DateRestored = null;
                transferModel.RestoredBy = "";
                _context.Entry(transferModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Deletion Successful!");
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransferModel>>> view()
        {
            if (_context.TransferModels == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TransferModels' is null.");
            }
            return await _context.TransferModels
                                .Include(transferModel => transferModel.Animal)
                                .Include(transferModel => transferModel.Owner)
                                .ToListAsync();
        }

        // POST: BirthTypes/restore/
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> restore(RestorationModel restorationModel)
        {

            if (_context.TransferModels == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TransferModels' is null!");
            }

            var transferModel = await _context.TransferModels.FindAsync(restorationModel.id);
            if (transferModel == null || !transferModel.DeleteFlag)
            {
                return Conflict("No deleted records matched!");
            }

            try
            {
                transferModel.DeleteFlag = !transferModel.DeleteFlag;
                transferModel.DateDeleted = null;
                transferModel.DeletedBy = "";
                transferModel.DateRestored = DateTime.Now;
                transferModel.RestoredBy = restorationModel.restoredBy;

                _context.Entry(transferModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Restoration Successful!");
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }

        private IQueryable<TransferModel> buildTransferModelSearchQuery(CommonSearchFilterModel searchFilter)
        {
            IQueryable<TransferModel> query = _context.TransferModels;

            query = query
                .Include(transferModel => transferModel.Animal)
                .Include(transferModel => transferModel.Owner);

            if (!searchFilter.searchParam.IsNullOrEmpty())
            {
                query = query.Where(transferModel => transferModel.transferNumber.Equals(searchFilter.searchParam));
            }

            return query;
        }

        private List<TransferPagedModel> buildTransferPagedModel(CommonSearchFilterModel searchFilter, List<TransferModel> transferModelList)
        {
            int pagesize = searchFilter.pageSize == 0 ? 10 : searchFilter.pageSize;
            int page = searchFilter.page == 0 ? 1 : searchFilter.page;
            var items = (dynamic)null;
            int totalItems = 0;
            int totalPages = 0;

            totalItems = transferModelList.Count;
            totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
            items = transferModelList.Skip((page - 1) * pagesize).Take(pagesize).ToList();

            List<TransferResponseModel> transferResponseModel = convertTransferModelToResponseModel(transferModelList);
            var result = new List<TransferPagedModel>();
            var item = new TransferPagedModel();

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
            item.items = transferResponseModel;
            result.Add(item);

            return result;
        }

        private List<TransferResponseModel> convertTransferModelToResponseModel(List<TransferModel> transferModelList)
        {
            var transferResponseModels = new List<TransferResponseModel>();

            foreach (TransferModel transferModel in transferModelList)
            {
                var transferResponseModel = new TransferResponseModel()
                {
                    Id = transferModel.Id,
                    transferNumber = transferModel.transferNumber,
                    BreedRegistrationNumber = transferModel.Animal.breedRegistryNumber,
                    AnimalIdNumber = transferModel.Animal.AnimalIdNumber,
                    Owner = transferModel.Owner == null ? "" : transferModel.Owner.FirstName + " " + transferModel.Owner.LastName,
                    Address = transferModel.Address,
                    TelephoneNumber = transferModel.TelephoneNumber,
                    MobileNumber = transferModel.MobileNumber,
                    Email = transferModel.Email
                };

                transferResponseModels.Add(transferResponseModel);
            }

            return transferResponseModels;
        }
    }
}
