using API_PCC.Data;
using API_PCC.Manager;
using API_PCC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_PCC.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class MailSenderCredentialController : ControllerBase
    {
        private readonly PCC_DEVContext _context;

        public MailSenderCredentialController(PCC_DEVContext context)
        {
            _context = context;
        }

        public class MailSenderCredentialModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        // GET: MailSender/list
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TblMailSenderCredential>>> list()
        {
            if (_context.TblMailSenderCredentials == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TblMailSenderCredentials' is null!");
            }
            return await _context.TblMailSenderCredentials.ToListAsync();
        }

        // GET: MailSender/search/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TblMailSenderCredential>> search(int id)
        {
            if (_context.TblMailSenderCredentials == null)
            {
                return NotFound();
            }
            var mailSenderCredential = await _context.TblMailSenderCredentials.FindAsync(id);

            if (mailSenderCredential == null)
            {
                return NotFound();
            }

            return mailSenderCredential;
        }

        [HttpPut]
        public async Task<IActionResult> update(int id, MailSenderCredentialModel mailSenderCredential)
        {
            if (_context.TblMailSenderCredentials == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TblMailSenderCredentials' is null");
            }

            var mailSenderModel = _context.TblMailSenderCredentials.AsNoTracking().Where(mailSender => mailSender.Id == id).FirstOrDefault();
            
            if (mailSenderModel == null)
            {
                return Conflict("No records found!");
            }

            
            if (id != mailSenderModel.Id)
            {
                return Conflict("Ids mismatched!");
            }

            bool hasDuplicateOnUpdate = (_context.TblMailSenderCredentials?.Any(mailSender => mailSender.Email == Cryptography.Encrypt(mailSenderCredential.Email) && mailSender.Id != id)).GetValueOrDefault();

            // check for duplication
            if (hasDuplicateOnUpdate)
            {
                return Conflict("Entity already exists");
            }

            try
            {
                if (!mailSenderCredential.Email.IsNullOrEmpty())
                {
                    mailSenderModel.Email = Cryptography.Encrypt(mailSenderCredential.Email);
                }

                if (!mailSenderCredential.Password.IsNullOrEmpty())
                {
                    mailSenderModel.Password = Cryptography.Encrypt(mailSenderCredential.Password);
                }
                _context.Entry(mailSenderModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Update Successful!");
            }
            catch (Exception ex)
            {
                
                return Problem(ex.GetBaseException().ToString());
            }

        }

        // POST: MailSender/save
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HFeedingSystem>> save(MailSenderCredentialModel mailSenderCredential)
        {
            if (_context.TblMailSenderCredentials == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TblMailSenderCredentials' is null!");
            }

            bool hasDuplicateOnSave = (_context.TblMailSenderCredentials?.Any(mailSender => mailSender.Email == Cryptography.Encrypt(mailSenderCredential.Email))).GetValueOrDefault();

            if (hasDuplicateOnSave)
            {
                return Conflict("Entity already exists");
            }

            try
            {
                var tblMailSender = new TblMailSenderCredential();

                if (!mailSenderCredential.Email.IsNullOrEmpty())
                {
                    tblMailSender.Email = Cryptography.Encrypt(mailSenderCredential.Email);
                }

                if (!mailSenderCredential.Password.IsNullOrEmpty())
                {
                    tblMailSender.Password = Cryptography.Encrypt(mailSenderCredential.Password);
                }
                tblMailSender.DateCreated = DateTime.Now;
                tblMailSender.Status = 1;
                tblMailSender.ExpiryDate = DateTime.Now.AddYears(1);
                _context.TblMailSenderCredentials?.Add(tblMailSender);
                await _context.SaveChangesAsync();

                return CreatedAtAction("save", new { id = tblMailSender.Id }, tblMailSender);
            }
            catch (Exception ex)
            {
                
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // DELETE: MailSender/delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> delete(int id)
        {
            if (_context.TblMailSenderCredentials == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TblMailSenderCredentials' is null!");
            }
            var mailSender = await _context.TblMailSenderCredentials.FindAsync(id);
            if (mailSender == null)
            {
                return Conflict("No records Matched!");
            }

            _context.TblMailSenderCredentials.Remove(mailSender);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
