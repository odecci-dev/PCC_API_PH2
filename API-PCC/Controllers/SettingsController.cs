using API_PCC.ApplicationModels;
using API_PCC.ApplicationModels.Common;
using API_PCC.Data;
using API_PCC.Manager;
using API_PCC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http.Results;

namespace API_PCC.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly PCC_DEVContext _context;
		private readonly string _connectionString;

		DBMethods dbmet = new DBMethods();
        DbManager db = new DbManager();

        public SettingsController(PCC_DEVContext context, IConfiguration configuration)
        {
            _context = context;
			_connectionString = configuration.GetConnectionString("DevConnection");
		}

        [HttpPost]
        public async Task<ActionResult<TblSettings>> ViewSettings(int id)
        {
            var settings = _context.TblSettings.FirstOrDefault(a => a.Id == id);

            if (settings == null)
            {
                return NotFound();
            }

            return Ok(settings);
        }

		//[HttpPost]
		//public async Task<ActionResult<string>> UpdateSettingsBusinessInfo(BusinessInfoModel businessInfo)
		//{
		//    string query = $@"UPDATE [dbo].[Tbl_Settings] SET 
		//              [Business_Name] = '" + businessInfo.BusinessName + "'," +
		//              "[Address] = '" + businessInfo.Address + "'," +
		//              "[Contact_Number] = '" + businessInfo.ContactNumber + "'," +
		//              "[Email] = '" + businessInfo.Email + "'," +
		//              "[Date_Updated] = GETDATE()," +
		//              "[Updated_By] = '" + businessInfo.UpdatedBy + "'" +
		//              " WHERE [Id] = '" + businessInfo.Id + "'";

		//    string result = db.DB_WithParam(query);
		//    return Ok(result);
		//}

		//public async Task<ActionResult<string>> UpdateSettingsConfiguration(ConfigurationModel configuration)
		//{
		//          string query = $@"UPDATE [dbo].[Tbl_Settings] SET 
		//	                 [Herd_Code_Length] = '" + configuration.HerdCodeLength + "'," +
		//                    "[ID_Number_Length] = '" + configuration.IdNumberLength + "'," +
		//                    "[Breed_Registry_Number_Length] = '" + configuration.BreedRegistryNumberLength + "'," +
		//                    "[Date_Updated] = GETDATE()," +
		//                    "[Updated_By] = '" + configuration.UpdatedBy + "'" +
		//                    " WHERE [Id] = '" + configuration.Id + "'";
		//}

		//[HttpPost]
		//public async Task<ActionResult<string>> UpdateSettingsResources(ResourcesModel resources)
		//{
		//    string query = $@"UPDATE [dbo].[TblSettings] SET 
		//      [PedigreeCertSignatoryFirstName] = '" + resources.PedigreeCertSignatoryFirstName+ "'," +
		//      "[PedigreeCertSignatoryLastName] = '" + resources.PedigreeCertSignatoryLastName + "'," +
		//      "[PedigreeCertSignatorySignature] = '" + resources.PedigreeCertSignatorySignature + "'," +
		//      "[PedigreeSignatoryFirstName] = '" + resources.PedigreeSignatoryFirstName + "'," +
		//      "[PedigreeSignatoryLastName] = '" + resources.PedigreeCertSignatoryLastName + "'," +
		//      "[PedigreeSignatorySignature] = '" + resources.PedigreeSignatorySignature + "'," +
		//      "[Watermark] = '" + resources.Watermark + "'," +
		//      "[DateUpdated] = GETDATE()," +
		//      "[UpdatedBy] = '" + resources.UpdatedBy + "'" +
		//      " WHERE [Id] = '" + resources.Id + "'";

		//    string result = db.DB_WithParam(query);
		//    return Ok(result);
		//}


        //Optimized ni Nest

		[HttpPost]
		public async Task<ActionResult<string>> UpdateSettingsBusinessInfo(BusinessInfoModel businessInfo)
		{
			string query = @"
                    UPDATE [dbo].[Tbl_Settings] 
                    SET 
                        [Business_Name] = @BusinessName,
                        [Address] = @Address,
                        [Contact_Number] = @ContactNumber,
                        [Email] = @Email,
                        [Date_Updated] = GETDATE(),
                        [Updated_By] = @UpdatedBy
                    WHERE [Id] = @Id";

			var parameters = new SqlParameter[]
			{
				new SqlParameter("@BusinessName", businessInfo.BusinessName),
				new SqlParameter("@Address", businessInfo.Address),
				new SqlParameter("@ContactNumber", businessInfo.ContactNumber),
				new SqlParameter("@Email", businessInfo.Email),
				new SqlParameter("@UpdatedBy", businessInfo.UpdatedBy),
				new SqlParameter("@Id", businessInfo.Id)
			};
			string result = await db.DB_WithParamAsync(query, parameters);
			return Ok(result);
		}
		
		[HttpPost]
        public async Task<ActionResult<string>> UpdateSettingsConfiguration(ConfigurationModel configuration)
        {
			string query = @"
                    UPDATE [dbo].[Tbl_Settings] 
                    SET 
                        [Herd_Code_Length] = @HerdCodeLength,
                        [ID_Number_Length] = @IdNumberLength,
                        [Breed_Registry_Number_Length] = @BreedRegistryNumberLength,
                        [Date_Updated] = GETDATE(),
                        [Updated_By] = @UpdatedBy
                    WHERE [Id] = @Id";

			var parameters = new SqlParameter[]
			{
				new SqlParameter("@HerdCodeLength", configuration.HerdCodeLength),
				new SqlParameter("@IdNumberLength", configuration.IdNumberLength),
				new SqlParameter("@BreedRegistryNumberLength", configuration.BreedRegistryNumberLength),
				new SqlParameter("@UpdatedBy", configuration.UpdatedBy),
				new SqlParameter("@Id", configuration.Id)
			};

			string result = await db.DB_WithParamAsync(query, parameters);
			return Ok(result);
		}


		[HttpPost]
		public async Task<ActionResult<string>> UpdateSettingsResources(ResourcesModel resources)
		{
			string query = @"
            UPDATE [dbo].[Tbl_Settings] 
            SET 
                [Pedigree_Cert_Signatory_FirstName] = @PedigreeCertSignatoryFirstName,
                [Pedigree_Cert_Signatory_LastName] = @PedigreeCertSignatoryLastName,
                [Pedigree_Cert_Signatory_Signature] = @PedigreeCertSignatorySignature,
                [Pedigree_Signatory_FirstName] = @PedigreeSignatoryFirstName,
                [Pedigree_Signatory_LastName] = @PedigreeSignatoryLastName,
                [Pedigree_Signatory_Signature] = @PedigreeSignatorySignature,
                [Watermark] = @Watermark,
                [Date_Updated] = GETDATE(),
                [Updated_By] = @UpdatedBy
            WHERE [Id] = @Id";

			var parameters = new SqlParameter[]
			{
		        new SqlParameter("@PedigreeCertSignatoryFirstName", resources.PedigreeCertSignatoryFirstName),
		        new SqlParameter("@PedigreeCertSignatoryLastName", resources.PedigreeCertSignatoryLastName),
		        new SqlParameter("@PedigreeCertSignatorySignature", resources.PedigreeCertSignatorySignature),
		        new SqlParameter("@PedigreeSignatoryFirstName", resources.PedigreeSignatoryFirstName),
		        new SqlParameter("@PedigreeSignatoryLastName", resources.PedigreeSignatoryLastName),
                new SqlParameter("@PedigreeSignatorySignature", resources.PedigreeSignatorySignature),
		        new SqlParameter("@Watermark", resources.Watermark),
		        new SqlParameter("@UpdatedBy", resources.UpdatedBy),
		        new SqlParameter("@Id", resources.Id)
			};

			string result = await db.DB_WithParamAsync(query, parameters);
			return Ok(result);
		}


		[HttpPost]
        public async Task<ActionResult<string>> HerdCodeLengthCheck(int id, string herdCode)
        {
            var herdCodeLengthString = _context.TblSettings
                .Where(settings => settings.Id == id)
                .Select(settings => settings.HerdCodeLength)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(herdCodeLengthString) || !int.TryParse(herdCodeLengthString, out int requiredLength))
            {
                return NotFound("Herd code length not found or invalid.");
            }

            if (herdCode.Length > requiredLength)
            {
                return BadRequest($"Herd code must be at least {requiredLength} characters long.");
            }

            return Ok("Herd code length is valid.");
        }

        [HttpPost]
        public async Task<ActionResult<string>> IdNumberLengthCheck(int id, string idNumber)
        {
            var idNumberLengthString = _context.TblSettings
                .Where(settings => settings.Id == id)
                .Select(settings => settings.IdNumberLength)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(idNumberLengthString) || !int.TryParse(idNumberLengthString, out int requiredLength))
            {
                return NotFound("ID Number length not found or invalid.");
            }

            if (idNumber.Length > requiredLength)
            {
                return BadRequest($"ID Number must be at least {requiredLength} characters long.");
            }

            return Ok("ID Number length is valid.");
        }

        [HttpPost]
        public async Task<ActionResult<string>> BreedRegNumLengthCheck(int id, string breedRegNumber)
        {
            var breedRegNumLengthString = _context.TblSettings
                .Where(settings => settings.Id == id)
                .Select(settings => settings.BreedRegistryNumberLength)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(breedRegNumLengthString) || !int.TryParse(breedRegNumLengthString, out int requiredLength))
            {
                return NotFound("Breed registry number length not found or invalid.");
            }

            if (breedRegNumber.Length > requiredLength)
            {
                return BadRequest($"Breed registry number must be at least {requiredLength} characters long.");
            }

            return Ok("Breed registry number length is valid.");
        }
        
		public class BusinessInfoModel
		{
			public int Id { get; set; }
			public string? BusinessName { get; set; }
			public string? Address { get; set; }
			public string? ContactNumber { get; set; }
			public string? Email { get; set; }
			public string? UpdatedBy { get; set; }
		}

		public class ConfigurationModel
		{
			public int Id { get; set; }
			public string? HerdCodeLength { get; set; }
			public string? IdNumberLength { get; set; }
			public string? BreedRegistryNumberLength { get; set; }
			public string? UpdatedBy { get; set; }
		}

		public class ResourcesModel
		{
			public int Id { get; set; }
			public string? PedigreeCertSignatoryFirstName { get; set; }
			public string? PedigreeCertSignatoryLastName { get; set; }
			public string? PedigreeCertSignatorySignature { get; set; }
			public string? PedigreeSignatoryFirstName { get; set; }
			public string? PedigreeSignatoryLastName { get; set; }
			public string? PedigreeSignatorySignature { get; set; }
			public string? Watermark { get; set; }
			public string? UpdatedBy { get; set; }
		}
	}
}
