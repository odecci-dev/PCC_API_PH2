using API_PCC.ApplicationModels;
using API_PCC.ApplicationModels.Common;
using API_PCC.Data;
using API_PCC.Manager;
using API_PCC.Models;
using API_PCC.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;

namespace API_PCC.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class BloodCompsController : ControllerBase
    {
        private readonly PCC_DEVContext _context;
        DBMethods dbmet = new DBMethods();
        public BloodCompsController(PCC_DEVContext context)
        {
            _context = context;
        }


        public class BloodCompSearchFilter
        {
            public string searchParam { get; set; }
            public int page { get; set; }
            public int pageSize { get; set; }
        }

        // POST: BloodComps/list
        public class BloodCompositionModel
        {
            public string? CurrentPage { get; set; }
            public string? NextPage { get; set; }
            public string? PrevPage { get; set; }
            public string? TotalPage { get; set; }
            public string? PageSize { get; set; }
            public string? TotalRecord { get; set; }
            public List<ABloodComp_2> items { get; set; }


        }
        [HttpPost]
        public async Task<ActionResult<IEnumerable<ABloodComp>>> list(BloodCompSearchFilter searchFilter)
        {
            if (_context.ABloodComps == null)
            {
                return Problem("Entity set 'PCC_DEVContext.Feeding Sytem' is null!");
            }

            int pagesize = searchFilter.pageSize == 0 ? 10 : searchFilter.pageSize;
            int page = searchFilter.page == 0 ? 1 : searchFilter.page;
            var items = (dynamic)null;
            int totalItems = 0;
            int totalPages = 0;


            //var bloodCompList = _context.ABloodComps.AsNoTracking();
            //bloodCompList = bloodCompList.Where(bloodComp => !bloodComp.DeleteFlag);
            try
            {
                //if (searchFilter.searchParam != null && searchFilter.searchParam != "")
                //{
                //    //bloodCompList = dbmet.getBlodyList().Where(bloodComp => bloodComp.BloodCode.Contains(searchFilter.searchParam));

                //}

                var bloodCompList = dbmet.getBlodyList().Where(a => a.BloodCode.ToUpper().Contains(searchFilter.searchParam.ToUpper())).ToList();
                totalItems = bloodCompList.ToList().Count();
                totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
                items = bloodCompList.Skip((page - 1) * pagesize).Take(pagesize).ToList();
                //var bloodcompo = convertDataRowListToBloodCompList(items);
                var result = new List<BloodCompositionModel>();
                var item = new BloodCompositionModel();
         

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
        public partial class ABloodComp_2
        {
            public int Id { get; set; }

            public string BloodCode { get; set; }

            public string BloodDesc { get; set; }

            public string Status { get; set; }
            public string DateCreated { get; set; }
        }
        
        // GET: BloodComps/search/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ABloodComp>> search(int id)
        {
            if (_context.ABloodComps == null)
            {
                return Problem("Entity set 'PCC_DEVContext.ABloodCOmps' is null!");
            }
            var bloodComp = await _context.ABloodComps.FindAsync(id);

            if (bloodComp == null || bloodComp.DeleteFlag)
            {
                return Conflict("No records found!");
            }

            return bloodComp;
        }

        // PUT: BloodComps/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, ABloodComp aBloodComp)
        {
            if (_context.ABloodComps == null)
            {
                return Problem("Entity set 'PCC_DEVContext.ABloodCOmps' is null!");
            }

            var bloodComp = _context.ABloodComps.AsNoTracking().Where(bloodComp => !bloodComp.DeleteFlag && bloodComp.Id == id).FirstOrDefault();

            if (bloodComp == null)
            {
                return Conflict("No records matched!");
            }

            if (id != aBloodComp.Id)
            {
                return Conflict("Ids mismatched!");
            }

            bool hasDuplicateOnUpdate = (_context.ABloodComps?.Any(bloodComp => !bloodComp.DeleteFlag && bloodComp.BloodCode == aBloodComp.BloodCode && bloodComp.BloodDesc == aBloodComp.BloodDesc && bloodComp.Id != id)).GetValueOrDefault();

            // check for duplication
            if (hasDuplicateOnUpdate)
            {
                return Conflict("Entity already exists");
            }

            try
            {
                _context.Entry(aBloodComp).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Update Successful!");
            }
            catch (Exception ex)
            {
                
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // POST: BloodComps/save
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ABloodComp>> save(ABloodComp aBloodComp)
        {
            if (_context.ABloodComps == null)
            {
                return Problem("Entity set 'PCC_DEVContext.ABloodCOmps' is null!");
            }

            bool hasDuplicateOnSave = (_context.ABloodComps?.Any(bloodComp => !bloodComp.DeleteFlag && bloodComp.BloodCode == aBloodComp.BloodCode && bloodComp.BloodDesc == aBloodComp.BloodDesc)).GetValueOrDefault();

            if (hasDuplicateOnSave)
            {
                return Conflict("Entity already exists");
            }

            try
            {
                _context.ABloodComps.Add(aBloodComp);
                await _context.SaveChangesAsync();

                return CreatedAtAction("save", new { id = aBloodComp.Id }, aBloodComp);
            }
            catch (Exception ex)
            {
                
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // POST: BloodComps/delete/
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> delete(DeletionModel deletionModel)
        {

            if (_context.ABloodComps == null)
            {
                return Problem("Entity set 'PCC_DEVContext.ABloodComps' is null!");
            }

            var bloodComp = await _context.ABloodComps.FindAsync(deletionModel.id);
            if (bloodComp == null || bloodComp.DeleteFlag)
            {
                return Conflict("No records matched!");
            }

            try
            {
                bloodComp.DeleteFlag = true;
                bloodComp.DateDeleted = DateTime.Now;
                bloodComp.DeletedBy = deletionModel.deletedBy;
                bloodComp.DateRestored = null;
                bloodComp.RestoredBy = "";
                _context.Entry(bloodComp).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Deletion Successful!");
            }
            catch (Exception ex)
            {
                
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // GET: BloodComps/view
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ABloodComp>>> view()
        {
            if (_context.ABloodComps == null)
            {
                return Problem("Entity set 'PCC_DEVContext.ABloodComps' is null.");
            }
            return await _context.ABloodComps.Where(bloodComp => !bloodComp.DeleteFlag).ToListAsync();
        }

        // POST: BloodComps/restore/
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> restore(RestorationModel restorationModel)
        {

            if (_context.ABloodComps == null)
            {
                return Problem("Entity set 'PCC_DEVContext.ABloodComps' is null!");
            }

            var bloodComp = await _context.ABloodComps.FindAsync(restorationModel.id);
            if (bloodComp == null || !bloodComp.DeleteFlag)
            {
                return Conflict("No deleted records matched!");
            }

            try
            {
                bloodComp.DeleteFlag = !bloodComp.DeleteFlag;
                bloodComp.DateDeleted = null;
                bloodComp.DeletedBy = "";
                bloodComp.DateRestored = DateTime.Now;
                bloodComp.RestoredBy = restorationModel.restoredBy;

                _context.Entry(bloodComp).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Restoration Successful!");
            }
            catch (Exception ex)
            {
                
                return Problem(ex.GetBaseException().ToString());
            }
        }

        private bool ABloodCompExists(int id)
        {
            return (_context.ABloodComps?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
