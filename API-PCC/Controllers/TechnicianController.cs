using API_PCC.ApplicationModels;
using API_PCC.Data;
using API_PCC.EntityModels;
using API_PCC.Manager;
using API_PCC.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_PCC.Controllers
{
	[Authorize("ApiKey")]
	[Route("[controller]/[action]")]
	[ApiController]
	public class TechnicianController : ControllerBase
	{
		private readonly PCC_DEVContext _context;
		
		DBMethods dbmet = new DBMethods();
		DbManager db = new DbManager();

		public TechnicianController(PCC_DEVContext context)
		{
			_context = context;			
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<TblTechnician>>> GetTechnicians()
		{
			return await _context.TblTechnicians.ToListAsync();
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<TblTechnician>>> SearchTechnicians([FromQuery] string? search)
		{
			var query = _context.TblTechnicians.AsQueryable();

			if (!string.IsNullOrEmpty(search))
			{
				search.Trim();
				query = query.Where(t =>
					t.Name.Contains(search) ||
					t.Address.Contains(search) ||
					t.ContactNumber.Contains(search)
				);
			}

			return await query.ToListAsync();
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<TblTechnician>> GetTechnicianById(int id)
		{
			var technician = await _context.TblTechnicians.FindAsync(id);

			if (technician == null)
			{
				return NotFound();
			}

			return technician;
		}

		[HttpPost]
		public async Task<ActionResult<TblTechnician>> CreateTechnician(TblTechnician technician)
		{
			_context.TblTechnicians.Add(technician);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetTechnicianById), new { id = technician.Id }, technician);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateTechnician(int id, TblTechnician technician)
		{
			if (id != technician.Id)
			{
				return BadRequest();
			}

			_context.Entry(technician).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!TechnicianExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTechnicianById(int id)
		{
			var technician = await _context.TblTechnicians.FindAsync(id);
			if (technician == null)
			{
				return NotFound();
			}

			_context.TblTechnicians.Remove(technician);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool TechnicianExists(int id)
		{
			return _context.TblTechnicians.Any(e => e.Id == id);
		}
	}
}
