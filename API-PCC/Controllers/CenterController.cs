using API_PCC.ApplicationModels;
using API_PCC.ApplicationModels.Common;
using API_PCC.Data;
using API_PCC.Manager;
using API_PCC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_PCC.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class CenterController : ControllerBase
    {
        private readonly PCC_DEVContext _context;
        DBMethods dbmet = new DBMethods();
        public class CenterSearchFilter
        {
            public string? searchParam { get; set; }
            public int page { get; set; }
            public int pageSize { get; set; }
        }

        public CenterController(PCC_DEVContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TblCenterModel>>> export()
        {
            var centerList = dbmet.getcenterlist().Where(a => !a.DeleteFlag).ToList();
            return Ok(centerList);
        }
            // GET: Center/list
        [HttpPost]
        public async Task<ActionResult<IEnumerable<TblCenterModel>>> list(CenterSearchFilter searchFilter)
        {
            if (_context.TblCenterModels == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TblCenterModels' is null!");
            }

            int pagesize = searchFilter.pageSize == 0 ? 10 : searchFilter.pageSize;
            int page = searchFilter.page == 0 ? 1 : searchFilter.page;
            var items = (dynamic)null;
            int totalItems = 0;
            int totalPages = 0;


            var centerList = dbmet.getcenterlist().Where(a => !a.DeleteFlag).ToList();
            try
            {
                if (searchFilter.searchParam != null && searchFilter.searchParam != "")
                {
                    centerList = centerList.Where(centerModel => centerModel.CenterName.Contains(searchFilter.searchParam)).ToList();
                }



                totalItems = centerList.ToList().Count();
                totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
                items = centerList.Skip((page - 1) * pagesize).Take(pagesize).ToList();

                var result = new List<CenterPagedModel>();
                var item = new CenterPagedModel();

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
                return Ok(result);
            }

            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }
        [HttpPost]
        public async Task<ActionResult<IEnumerable<TblCenterModel>>> ArchieveList(CenterSearchFilter searchFilter)
        {
            if (_context.TblCenterModels == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TblCenterModels' is null!");
            }

            int pagesize = searchFilter.pageSize == 0 ? 10 : searchFilter.pageSize;
            int page = searchFilter.page == 0 ? 1 : searchFilter.page;
            var items = (dynamic)null;
            int totalItems = 0;
            int totalPages = 0;


            var centerList = dbmet.getcenterlist().Where(a => a.DeleteFlag).ToList();
            try
            {
                if (searchFilter.searchParam != null && searchFilter.searchParam != "")
                {
                    centerList = centerList.Where(centerModel => centerModel.CenterName.Contains(searchFilter.searchParam)).ToList();
                }



                totalItems = centerList.ToList().Count();
                totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
                items = centerList.Skip((page - 1) * pagesize).Take(pagesize).ToList();

                var result = new List<CenterPagedModel>();
                var item = new CenterPagedModel();

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
                return Ok(result);
            }

            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }

        // GET: Center/search/5
        [HttpGet]
        public async Task<ActionResult<TblCenterModel>> Search(string  code)
        {
            if (_context.TblCenterModels == null)
            {
                return Problem("Entity set 'PCC_DEVContext.Tbl Center Models' is null!");
            }
            var centerModel = await _context.TblCenterModels.Where(a=>a.CenterCode == code && a.DeleteFlag == false).ToListAsync();

          

            return Ok( centerModel);
        }

        // PUT: Center/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TblCenterModel tblCenterModel)
        {
            if (_context.HHerdClassifications == null)
            {
                return Problem("Entity set 'PCC_DEVContext.Center' is null!");
            }

            var centerModel = _context.TblCenterModels.AsNoTracking().Where(feedSys => !feedSys.DeleteFlag && feedSys.Id == id).FirstOrDefault();

            if (centerModel == null)
            {
                return Conflict("No records matched!");
            }

            if (id != tblCenterModel.Id)
            {
                return Conflict("Ids mismatched!");
            }

            bool hasDuplicateOnUpdate = (_context.TblCenterModels?.Any(fs => !fs.DeleteFlag && fs.CenterName == tblCenterModel.CenterName && fs.CenterDesc == tblCenterModel.CenterDesc && fs.Id != id)).GetValueOrDefault();

            // check for duplication
            if (hasDuplicateOnUpdate)
            {
                return Conflict("Entity already exists");
            }

            try
            {
                _context.Entry(tblCenterModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                dbmet.InsertAuditTrail("Updated Center ID: " + tblCenterModel.Id + "", DateTime.Now.ToString("yyyy-MM-dd"), "Center Module", tblCenterModel.UpdatedBy, "0");
                return Ok("Update Successful!");
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }

        }

        // POST: Center/save
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TblCenterModel>> save(TblCenterModel tblCenterModel)
        {
            if (_context.TblCenterModels == null)
            {
                return Problem("Entity set 'PCC_DEVContext.Feeding Sytem' is null!");
            }

            bool hasDuplicateOnSave = (_context.TblCenterModels?.Any(fs => !fs.DeleteFlag && fs.CenterName == tblCenterModel.CenterName && fs.CenterDesc == tblCenterModel.CenterDesc)).GetValueOrDefault();

            if (hasDuplicateOnSave)
            {
                return Conflict("Entity already exists");
            }

            try
            {
                _context.TblCenterModels.Add(tblCenterModel);
                var entry = _context.Entry(tblCenterModel);
                var lastInsertedId = entry.Property(e => e.Id).CurrentValue;
                dbmet.InsertAuditTrail("Save New Center ID: " + lastInsertedId + "", DateTime.Now.ToString("yyyy-MM-dd"), "Center Module", tblCenterModel.CreatedBy, "0");
                await _context.SaveChangesAsync();

                return CreatedAtAction("save", new { id = tblCenterModel.Id }, tblCenterModel);
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }

        //center import details model
        public class CenterImportDetailsModel
        {
            public int Id { get; set; }

            public string? CenterName { get; set; }

            public string? CenterDesc { get; set; }
            public string? CenterCode { get; set; }
            public string? Address { get; set; }
            public string? ContactPerson { get; set; }
            public string? MobileNumber { get; set; }
            public string? TelNumber { get; set; }
            public string? Email { get; set; }
            //public string? StatusName { get; set; }
            public int? Status { get; set; }
            //public int? StatusId { get; set; }
            public string? CreatedBy { get; set; }
        }

        [HttpPost]
        public async Task<ActionResult> import(List<CenterImportDetailsModel> centerImportDetails)
        {
            if (_context.TblCenterModels == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TblCenterModels' is null!");
            }

            foreach (var center in centerImportDetails)
            {
                bool hasDuplicate = _context.TblCenterModels
                    .Any(fs => !fs.DeleteFlag && fs.CenterName == center.CenterName && fs.CenterDesc == center.CenterDesc);

                if (hasDuplicate)
                {
                    return Conflict($"Entity with CenterName '{center.CenterName}' and CenterDesc '{center.CenterDesc}' already exists.");
                }

                try
                {
                    var newCenter = new TblCenterModel
                    {
                        CenterName = center.CenterName,
                        CenterDesc = center.CenterDesc,
                        CenterCode = center.CenterCode,
                        Address = center.Address,
                        ContactPerson = center.ContactPerson,
                        MobileNumber = center.MobileNumber,
                        TelNumber = center.TelNumber,
                        Email = center.Email,
                        Status = center.Status,
                        CreatedBy = center.CreatedBy,
                        DateCreated = DateTime.Now
                    };

                    _context.TblCenterModels.Add(newCenter);
                    await _context.SaveChangesAsync();

                    dbmet.InsertAuditTrail($"Save New Center ID: {newCenter.Id}",
                        DateTime.Now.ToString("yyyy-MM-dd"),
                        "Center Module",
                        center.CreatedBy,
                        "0");
                }
                catch (Exception ex)
                {
                    return Problem(ex.GetBaseException().ToString());
                }
            }

            return Ok("Import process completed successfully.");
        }


        //[HttpPost]
        //public async Task<ActionResult<TblCenterModel>> import(List<CenterImportDetailsModel> tblCenterModel)
        //{
        //    if (_context.TblCenterModels == null)
        //    {
        //        return Problem("Entity set 'PCC_DEVContext.Feeding Sytem' is null!");
        //    }
        //    for(int x= 0; x < tblCenterModel.Count;x++)
        //    {
        //        bool hasDuplicateOnSave = (_context.TblCenterModels?.Any(fs => !fs.DeleteFlag && fs.CenterName == tblCenterModel[x].CenterName && fs.CenterDesc == tblCenterModel[x].CenterDesc)).GetValueOrDefault();

        //        if (hasDuplicateOnSave)
        //        {
        //            return Conflict("Entity already exists");
        //        }

        //        try
        //        {
        //            _context.TblCenterModels.Add(tblCenterModel[x]);
        //            var entry = _context.Entry(tblCenterModel[x]);
        //            var lastInsertedId = entry.Property(e => e.Id).CurrentValue;
        //            dbmet.InsertAuditTrail("Save New Center ID: " + lastInsertedId + "", DateTime.Now.ToString("yyyy-MM-dd"), "Center Module", tblCenterModel[x].CreatedBy, "0");
        //            await _context.SaveChangesAsync();

        //            return CreatedAtAction("save", new { id = tblCenterModel[x].Id }, tblCenterModel);
        //        }
        //        catch (Exception ex)
        //        {

        //            return Problem(ex.GetBaseException().ToString());
        //        }

        //    }
        //    return Ok();

        //}

        // DELETE: Center/delete/5
        [HttpPost]
        public async Task<IActionResult> delete(DeletionModel deletionModel)
        {
            if (_context.TblCenterModels == null)
            {
                return Problem("Entity set 'PCC_DEVContext.Feeding Sytem' is null!");
            }
            var tblCenterModel = await _context.TblCenterModels.FindAsync(deletionModel.id);
            if (tblCenterModel == null || tblCenterModel.DeleteFlag)
            {
                return Conflict("No records matched!");
            }

            bool CenterNameExistsInBuffHerd = _context.HBuffHerds.Any(buffHerd => !buffHerd.DeleteFlag && buffHerd.Center == tblCenterModel.Id);

            if (CenterNameExistsInBuffHerd)
            {
                return Conflict("Used by other table!");
            }

            try
            {
                tblCenterModel.DeleteFlag = true;
                tblCenterModel.DateDeleted = DateTime.Now;
                tblCenterModel.DeletedBy = deletionModel.deletedBy;
                tblCenterModel.DateRestored = null;
                tblCenterModel.RestoredBy = "";
                _context.Entry(tblCenterModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Deletion Successful!");
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }

           [HttpPost]
        public async Task<IActionResult> ArchiveMultiple(List<DeletionModel> deletionModelList)
        {
            if (_context.TblCenterModels == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TblCenterModels' is null!");
            }

            try
            {
                var idsToCheck = deletionModelList.Select(d => d.id).ToList();
                var tblCenterModels = await _context.TblCenterModels
                    .Where(model => idsToCheck.Contains(model.Id))
                    .ToListAsync();

                var recordsToDelete = tblCenterModels
                    .Where(model => !model.DeleteFlag)
                    .ToList();

                if (recordsToDelete.Count != idsToCheck.Count)
                {
                    return Conflict("Some records have no match or are already marked for deletion!");
                }

                foreach (var tblCenterModel in recordsToDelete)
                {
                    bool centerNameExistsInBuffHerd = _context.HBuffHerds
                        .Any(buffHerd => !buffHerd.DeleteFlag && buffHerd.Center == tblCenterModel.Id);

                    if (centerNameExistsInBuffHerd)
                    {
                        return Conflict("One or more records are used by other tables!");
                    }
                }

                foreach (DeletionModel deletionModel in deletionModelList)
                {
                    var tblCenterModel = tblCenterModels.FirstOrDefault(model => model.Id == deletionModel.id);

                    if (tblCenterModel == null)
                    {
                        continue;
                    }

                    tblCenterModel.DeleteFlag = true;
                    tblCenterModel.DateDeleted = DateTime.Now;
                    tblCenterModel.DeletedBy = deletionModel.deletedBy;
                    tblCenterModel.DateRestored = null;
                    tblCenterModel.RestoredBy = "";
                    _context.Entry(tblCenterModel).State = EntityState.Modified;
                }

                await _context.SaveChangesAsync();

                return Ok("Deletion Successful!");
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // GET: Center/view
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TblCenterModel>>> view()
        {
            if (_context.TblCenterModels == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TblCenterModel' is null.");
            }
            return await _context.TblCenterModels.Where(centerModel => !centerModel.DeleteFlag).ToListAsync();
        }

        // POST: Center/restore/
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> restore(List<RestorationModel> restorationModel)
        {
            string result = "";
            if (_context.TblCenterModels == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TblCenterModel' is null.");
            }
            for (int x = 0; x < restorationModel.Count; x++)
            {
                var centerModel = await _context.TblCenterModels.FindAsync(restorationModel[x].id);

                if (centerModel == null || !centerModel.DeleteFlag)
                {
                    return Conflict("No deleted records matched!");
                }

                try
                {
                    centerModel.DeleteFlag = !centerModel.DeleteFlag;
                    centerModel.DateDeleted = null;
                    centerModel.DeletedBy = "";
                    centerModel.DateRestored = DateTime.Now;
                    centerModel.RestoredBy = restorationModel[x].restoredBy;

                    _context.Entry(centerModel).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    result = "Restoration Successful!";
                }
                catch (Exception ex)
                {

                    return Problem(ex.GetBaseException().ToString());
                }
            }
             return Ok(result);
        }

        private bool TblCenterModelExists(int id)
        {
            return (_context.TblCenterModels?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
