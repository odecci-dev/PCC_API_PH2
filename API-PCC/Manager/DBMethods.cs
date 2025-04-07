using API_PCC.ApplicationModels;
using API_PCC.Data;
using API_PCC.EntityModels;
using API_PCC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PeterO.Numbers;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text;
using static API_PCC.Controllers.BloodCompsController;
using static API_PCC.Controllers.BreedRegistryHerdController;
using static API_PCC.Controllers.BuffAnimalsController;
using static API_PCC.Controllers.PedigreeController;
using static API_PCC.Controllers.UserController;
using static API_PCC.Controllers.UserManagementController;
using static System.Net.Mime.MediaTypeNames;
using Module_Model = API_PCC.Controllers.UserManagementController.Module_Model;
namespace API_PCC.Manager
{
    public class DBMethods
    {
        string sql = "";
        string Stats = "";
        string Id = "";
        string Mess = "";
        string JWT = "";
        DbManager db = new DbManager();

        //DBMethods dbmet = new DBMethods();
        //private readonly PCC_DEVContext _context;

        #region Models
        public class AuditTrailModel
        {
            public int Id { get; set; }
            public string Actions { get; set; }
            public string Module { get; set; }
            public string DateCreated { get; set; }
            public string UserType { get; set; }
            public string Username { get; set; }

        }
        public string insertlgos(string filepath, string logs)
        {
            //System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(data));


            //System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(data[0]));
            //



            // Read the existing content of the file
            string existingContent = System.IO.File.ReadAllText(filepath);

            // Create a StringBuilder to manipulate the content
            StringBuilder sb = new StringBuilder(existingContent);

            // Insert the new text at a specific position (e.g., at the beginning of the file)
            sb.Insert(0, logs + " \n------------------" + DateTime.Now + "--------------- \n");

            // Write the modified content back to the file
            System.IO.File.WriteAllText(filepath, sb.ToString());


            return "";


        }
        public string insertlgosList(string filepath, List<string> logs)
        {
            //System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(data));


            //System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(data[0]));
            //



            // Read the existing content of the file
            string existingContent = System.IO.File.ReadAllText(filepath);

            // Create a StringBuilder to manipulate the content
            StringBuilder sb = new StringBuilder(existingContent);

            // Insert the new text at a specific position (e.g., at the beginning of the file)
            sb.Insert(0, logs + " \n------------------" + DateTime.Now + "--------------- \n");

            // Write the modified content back to the file
            System.IO.File.WriteAllText(filepath, sb.ToString());


            return "";


        }
        public partial class RegistrationModel
        {
            public string Username { get; set; }

            public string Password { get; set; }

            public string Fname { get; set; }

            public string? Lname { get; set; }

            public string? Mname { get; set; }

            public string Email { get; set; }

            public string Gender { get; set; }

            public string? EmployeeId { get; set; }

            public string Jwtoken { get; set; }

            public string? FilePath { get; set; }

            public int? Active { get; set; }

            public string? Cno { get; set; }

            public string? Address { get; set; }

            public int? Status { get; set; }
            public string? CreatedBy { get; set; }

            public int? CenterId { get; set; }

            public bool? AgreementStatus { get; set; }
            public string? UserType { get; set; }
        }

        #endregion

        public string BreedCode(string id)
        {
            string tbl = $@"SELECT  Breed_Code FROM A_Breed where id='" + id + "'";
            DataTable breedcode = db.SelectDb(tbl).Tables[0];
            string result = breedcode.Rows.Count != 0 ? breedcode.Rows[0]["Breed_Code"].ToString() : "";
            return result;
        }
        public string BirthType(string id)
        {
            //string tbl1 = $@"SELECT  Birth_Type_Code FROM A_Birth_Type where id='" + id + "'";
            string tbl1 = $@"SELECT  Birth_Type_Desc FROM A_Birth_Type where Birth_Type_Code = '" + id + "'";
            DataTable btype = db.SelectDb(tbl1).Tables[0];
            string result = btype.Rows.Count != 0 ? btype.Rows[0]["Birth_Type_Desc"].ToString() : "";
            return result;
        }
        public string BloodCode(string id)
        {
            string tbl2 = $@"SELECT  Blood_Code FROM A_Blood_Comp where id='" + id + "'";
            DataTable bcode = db.SelectDb(tbl2).Tables[0];
            string result = bcode.Rows.Count != 0 ? bcode.Rows[0]["Blood_Code"].ToString() : "";
            return result;
        }
        public string InsertAuditTrail(string actions, string datecreated, string module, string userid, string read)
        {
            string Insert = $@"INSERT INTO [dbo].[tbl_audittrail]
                           ([Actions]
                           ,[Module]
                           ,[DateCreated]
                           ,[UserId]
                           ,[status])
                         VALUES
                               ('" + actions + "'," +
                             "'" + module + "'," +
                             "'" + datecreated + "'," +
                             "'" + userid + "'," +
                              "'" + read + "') ";

            return db.DB_WithParam(Insert);
        }

        public string InsertAuditTrailv2(string actions, string datecreated, string module, string userid, string read, string refno)
        {
            string Insert = $@"INSERT INTO [dbo].[tbl_audittrail]
                           ([Actions]
                           ,[Module]
                           ,[DateCreated]
                           ,[UserId]
                           ,[status]
                           ,[Reference])
                         VALUES
                               ('" + actions + "'," +
                             "'" + module + "'," +
                             "'" + datecreated + "'," +
                             "'" + userid + "'," +
                             "'" + read + "'," +
                              "'" + refno + "') ";

            return db.DB_WithParam(Insert);
        }

        public List<UserTypeAction_Model> UserTypeParams(int id)
        {
            string sql = $@"SELECT [Id]
                      ,Name as UserType
                  FROM [dbo].[tbl_UserTypeModel] where Id='" + id + "'";
            var result = new List<UserTypeAction_Model>();
            DataTable table = db.SelectDb(sql).Tables[0];

            if (table.Rows.Count != 0)
            {
                var item = new UserTypeAction_Model();
                item.UserTypeId = table.Rows[0]["Id"].ToString();
                item.UserType = table.Rows[0]["UserType"].ToString();
                string sql_usertype = $@"SELECT        User_ModuleTable.Id as ModuleId,User_UserTypeAccessTable.UserTypeId, User_ModuleTable.Module, User_ModuleTable.ParentModule,User_UserTypeAccessTable.DateCreated
                                        FROM            User_UserTypeAccessTable INNER JOIN
                                                                 User_ModuleTable ON User_UserTypeAccessTable.Module = User_ModuleTable.Id  where User_UserTypeAccessTable.UserTypeId ='" + table.Rows[0]["Id"].ToString() + "' ";
                DataTable tbl_usertype = db.SelectDb(sql_usertype).Tables[0];
                var usertype_item = new List<Module_Model>();
                foreach (DataRow drw in tbl_usertype.Rows)
                {
                    var item1 = new Module_Model();
                    item1.ModuleId = drw["ModuleId"].ToString();
                    item1.ModuleName = drw["Module"].ToString();
                    item1.ParentModule = drw["ParentModule"].ToString();
                    item1.DateCreated = drw["DateCreated"].ToString();

                    string sql_actions = $@"SELECT  [Id]
                                      ,[ActionName]
                                      ,[DateCreated]
                                  FROM [PCC_DEV].[dbo].[User_ActionTable] where Module ='" + drw["ModuleId"].ToString() + "'";
                    DataTable action_tbl = db.SelectDb(sql_actions).Tables[0];
                    var action_item = new List<ActionModel>();
                    foreach (DataRow dra in action_tbl.Rows)
                    {
                        var items = new ActionModel();
                        items.ActionId = dra["Id"].ToString();
                        items.Actions = dra["ActionName"].ToString();
                        action_item.Add(items);

                    }
                    item1.Actions = action_item;
                    usertype_item.Add(item1);

                }
                item.Module = usertype_item;

                result.Add(item);
            }

            return result;
        }

		public List<animalresult> GetAnimalList(int? centerid, string? userid)
		{
			var result = new List<animalresult>();

			if (centerid != null && centerid != 0)
			{
				var herdCodes = GetHerdCodes(centerid.Value);
				if (herdCodes.Count == 0)
				{
					result.Add(new animalresult()); // Optional: return empty result instead
					return result;
				}

				if (!string.IsNullOrEmpty(userid))
				{
					var user = GetUserDetails(userid);
					var farmerExists = FarmerExists(user);

					if (!farmerExists)
					{
						result.Add(new animalresult()); // Optional: return empty result instead
						return result;
					}
				}

				var herdCodeStr = string.Join(",", herdCodes.Select(h => $"'{h}'"));
				var sql = GetAnimalQuery($"WHERE (A_Buff_Animal.Delete_Flag <> 1) AND A_Buff_Animal.Herd_Code IN ({herdCodeStr})");
				var table = db.SelectDb(sql).Tables[0];
				result = table.AsEnumerable().Select(MapAnimalResult).ToList();
			}
			else
			{
				var sql = GetAnimalQuery("WHERE (A_Buff_Animal.Delete_Flag <> 1)");
				var table = db.SelectDb(sql).Tables[0];
				result = table.AsEnumerable().Select(MapAnimalResult).ToList();
			}

			return result;
		}

		private List<string> GetHerdCodes(int centerId)
		{
			var herdCodes = new List<string>();
			string sql = $"SELECT Herd_Code FROM H_Buff_Herd WHERE Center = '{centerId}'";
			DataTable dt = db.SelectDb(sql).Tables[0];

			foreach (DataRow row in dt.Rows)
			{
				herdCodes.Add(row["Herd_Code"].ToString()!);
			}
			return herdCodes;
		}

		private (string Fname, string Lname, string Address) GetUserDetails(string userId)
		{
			string sql = $"SELECT Fname, Lname, Address FROM tbl_UsersModel WHERE Id = '{userId}'";
			var dt = db.SelectDb(sql).Tables[0];
			if (dt.Rows.Count > 0)
			{
				var row = dt.Rows[0];
				return (row["Fname"].ToString(), row["Lname"].ToString(), row["Address"].ToString())!;
			}
			return ("", "", "");
		}

		private bool FarmerExists((string Fname, string Lname, string Address) user)
		{
			string sql = $@"
                SELECT Id FROM tbl_Farmers 
                WHERE FirstName LIKE '%{user.Fname}%' 
                AND LastName LIKE '%{user.Lname}%'
                AND Address LIKE '%{user.Address}%'";

			var dt = db.SelectDb(sql).Tables[0];
			return dt.Rows.Count > 0;
		}

		private string GetAnimalQuery(string whereClause)
		{
			return $@"
                SELECT 
                    A_Buff_Animal.id, A_Buff_Animal.Animal_ID_Number, A_Buff_Animal.Animal_Name, A_Buff_Animal.Photo, 
                    A_Buff_Animal.Herd_Code, A_Buff_Animal.RFID_Number, A_Buff_Animal.Date_of_Birth, A_Buff_Animal.Sex,
                    A_Buff_Animal.Birth_Type, A_Buff_Animal.Country_Of_Birth, A_Buff_Animal.Origin_Of_Acquisition,
                    A_Buff_Animal.Date_Of_Acquisition, A_Buff_Animal.Marking, A_Buff_Animal.Type_Of_Ownership,
                    A_Buff_Animal.Delete_Flag, A_Buff_Animal.Status, A_Buff_Animal.Created_By, A_Buff_Animal.Created_Date,
                    A_Buff_Animal.Updated_By, A_Buff_Animal.Update_Date, A_Buff_Animal.Date_Deleted, A_Buff_Animal.Deleted_By,
                    A_Buff_Animal.Date_Restored, A_Buff_Animal.Restored_By, A_Buff_Animal.BreedRegistryNumber,
                    A_Breed.Breed_Code, A_Blood_Comp.Blood_Code, tbl_StatusModel.Status AS StatusName,
                    A_Buff_Animal.FarmerId, A_Buff_Animal.GroupId
                    FROM A_Buff_Animal
                    INNER JOIN A_Breed ON A_Buff_Animal.Breed_Code = CAST(A_Breed.Id AS VARCHAR(255))
                    INNER JOIN A_Blood_Comp ON A_Buff_Animal.Blood_Code = CAST(A_Blood_Comp.Id AS VARCHAR(255))
                    INNER JOIN tbl_StatusModel ON A_Buff_Animal.Status = tbl_StatusModel.id
                    {whereClause}";
		}

		private animalresult MapAnimalResult(DataRow dr)
		{
			return new animalresult
			{
				Id = dr["Id"]?.ToString()!,
				Animal_ID_Number = dr["Animal_ID_Number"]?.ToString()!,
				Animal_Name = dr["Animal_Name"]?.ToString()!,
				Photo = dr["Photo"]?.ToString()!,
				Herd_Code = dr["Herd_Code"]?.ToString()!,
				Farmer_Id = dr["FarmerId"]?.ToString()!,
				Group_Id = dr["GroupId"]?.ToString()!,
				RFID_Number = dr["RFID_Number"]?.ToString()!,
				Date_of_Birth = dr["Date_of_Birth"]?.ToString()!,
				Sex = dr["Sex"]?.ToString()!,
				Birth_Type = dr["Birth_Type"]?.ToString()!,
				Country_Of_Birth = dr["Country_Of_Birth"]?.ToString()!,
				Origin_Of_Acquisition = dr["Origin_Of_Acquisition"]?.ToString()!,
				Date_Of_Acquisition = dr["Date_Of_Acquisition"]?.ToString()!,
				Marking = dr["Marking"]?.ToString()!,
				Type_Of_Ownership = dr["Type_Of_Ownership"]?.ToString()!,
				Delete_Flag = dr["Delete_Flag"]?.ToString()!,
				BreedRegistryNumber = dr["BreedRegistryNumber"]?.ToString()!,
				Breed_Code = dr["Breed_Code"]?.ToString()!,
				Blood_Code = dr["Blood_Code"]?.ToString()!,
				Restored_By = dr["Restored_By"]?.ToString()!,
				Date_Restored = dr["Date_Restored"]?.ToString()!,
				Date_Deleted = dr["Date_Deleted"]?.ToString()!,
				Update_Date = dr["Update_Date"]?.ToString()!,
				Updated_By = dr["Updated_By"]?.ToString()!,
				Created_Date = dr["Created_Date"]?.ToString()!,
				Created_By = dr["Created_By"]?.ToString()!,
				Deleted_By = dr["Deleted_By"]?.ToString()!,
				Status = dr["Status"]?.ToString()!,
				StatusName = dr["StatusName"]?.ToString()!
			};
		}

		public List<TblCenterModel> getcenterlist()
        {



            string sql = $@"SELECT        tbl_CenterModel.id, tbl_CenterModel.CenterCode, tbl_CenterModel.CenterName, tbl_CenterModel.Center_desc, tbl_CenterModel.Address, 
                            tbl_CenterModel.ContactPerson, tbl_CenterModel.MobileNumber, 
                         tbl_CenterModel.TelNumber, tbl_CenterModel.Email, tbl_CenterModel.Created_By, 
                         tbl_CenterModel.Date_Created, tbl_CenterModel.Updated_By, tbl_CenterModel.Date_Updated, 
                         tbl_CenterModel.Deleted_By, tbl_CenterModel.Date_Deleted, tbl_CenterModel.Restored_By,
                         tbl_CenterModel.Date_Restored, tbl_CenterModel.Delete_Flag, tbl_CenterModel.Status, tbl_StatusModel.Status AS StatusId
                         FROM            tbl_CenterModel INNER JOIN
                         tbl_StatusModel ON tbl_CenterModel.Status = tbl_StatusModel.id";
            var result = new List<TblCenterModel>();
            DataTable table = db.SelectDb(sql).Tables[0];

            foreach (DataRow dr in table.Rows)
            {
                var datedeleted = dr["Date_Deleted"].ToString() == "" ? "1990-01-01" : Convert.ToDateTime(dr["Date_Deleted"].ToString()).ToString("yyyy-MM-dd");
                var daterestored = dr["Date_Restored"].ToString() == "" ? "1990-01-01" : Convert.ToDateTime(dr["Date_Restored"].ToString()).ToString("yyyy-MM-dd");
                var item = new TblCenterModel();
                item.Id = int.Parse(dr["id"].ToString());
                item.CenterCode = dr["CenterCode"].ToString();
                item.CenterName = dr["CenterName"].ToString();
                item.CenterDesc = dr["Center_desc"].ToString();
                item.Address = dr["Address"].ToString();
                item.ContactPerson = dr["ContactPerson"].ToString();
                item.MobileNumber = dr["MobileNumber"].ToString();
                item.TelNumber = dr["TelNumber"].ToString();
                item.Email = dr["Email"].ToString();
                item.CreatedBy = dr["Created_By"].ToString();
                item.DeletedBy = dr["Deleted_By"].ToString();
                item.DateDeleted = DateTime.Parse(datedeleted);
                item.RestoredBy = dr["Restored_By"].ToString();
                item.DateRestored = DateTime.Parse(daterestored);
                item.DeleteFlag = bool.Parse(dr["Delete_Flag"].ToString());
                //item.StatusName = dr["StatusId"].ToString();
                //item.StatusId = int.Parse(dr["Status"].ToString());


                result.Add(item);
            }

            return result;
        }
        public List<Module_Model> GetUserModuleActionList()
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
                item.ModuleName = dr["ActionName"].ToString();
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
                    items.Actions = dr["ActionName"].ToString();
                    action_item.Add(items);

                }
                item.Actions = action_item;
                result.Add(item);
            }
            return result;
        }

        public List<A_Family> getfamily()
        {

            string sql = $@"SELECT [Id]
                          ,[AnimalId]
                          ,[SireId]
                          ,[DamId]
                      FROM [dbo].[A_Family]";
            var result = new List<A_Family>();
            DataTable table = db.SelectDb(sql).Tables[0];

            foreach (DataRow dr in table.Rows)
            {
                var item = new A_Family();
                item.Id = int.Parse(dr["Id"].ToString());
                item.animalId = int.Parse(dr["AnimalId"].ToString());
                item.sireId = int.Parse(dr["SireId"].ToString());
                item.damId = int.Parse(dr["DamId"].ToString());
                result.Add(item);
            }
            return result;
        }
        public List<AuditTrailModel> GetAuditTrail()
        {

            string sql = $@"SELECT        tbl_audittrail.Id, tbl_audittrail.Actions, tbl_audittrail.Module, tbl_audittrail.DateCreated, tbl_audittrail.UserId, tbl_UserTypeModel.name AS UserType, tbl_UsersModel.Username
FROM            tbl_UserTypeModel INNER JOIN
                         tbl_UsersModel ON tbl_UserTypeModel.id = tbl_UsersModel.UserType INNER JOIN
                         tbl_audittrail ON tbl_UsersModel.Username = tbl_audittrail.UserId";
            var result = new List<AuditTrailModel>();
            DataTable table = db.SelectDb(sql).Tables[0];

            foreach (DataRow dr in table.Rows)
            {
                var item = new AuditTrailModel();
                item.Id = int.Parse(dr["Id"].ToString());
                item.Actions = dr["Actions"].ToString();
                item.Module = dr["Module"].ToString();
                item.DateCreated = dr["DateCreated"].ToString();
                item.Username = dr["Username"].ToString();
                item.UserType = dr["UserType"].ToString();
                result.Add(item);
            }
            return result;
        }
        public List<ABuffAnimal> getsirefamilybuff()
        {

            string sql = $@"SELECT     A_Family.SireId, A_Buff_Animal.BreedRegistryNumber, A_Buff_Animal.Photo, A_Buff_Animal.Animal_Name, A_Buff_Animal.Date_of_Birth, A_Buff_Animal.Country_Of_Birth
                         FROM            A_Family INNER JOIN
                         A_Buff_Animal ON A_Family.SireId = A_Buff_Animal.id where A_Buff_Animal.Delete_Flag =0";
            var result = new List<ABuffAnimal>();
            DataTable table = db.SelectDb(sql).Tables[0];

            foreach (DataRow dr in table.Rows)
            {
                var item = new ABuffAnimal();
                item.Id = int.Parse(dr["SireId"].ToString());
                item.breedRegistryNumber = dr["BreedRegistryNumber"].ToString();
                item.Photo = dr["Photo"].ToString();
                item.AnimalName = dr["Animal_Name"].ToString();
                item.DateOfBirth = Convert.ToDateTime(dr["Date_of_Birth"].ToString());
                item.CountryOfBirth = dr["Country_Of_Birth"].ToString();
                result.Add(item);
            }
            return result;
        }
        public List<ABuffAnimal> getchildfamilybuff()
        {

            string sql = $@"SELECT     A_Family.AnimalId, A_Buff_Animal.BreedRegistryNumber, A_Buff_Animal.Photo, A_Buff_Animal.Animal_Name, A_Buff_Animal.Date_of_Birth, A_Buff_Animal.Country_Of_Birth
                         FROM            A_Family INNER JOIN
                         A_Buff_Animal ON A_Family.AnimalId = A_Buff_Animal.id where A_Buff_Animal.Delete_Flag =0";
            var result = new List<ABuffAnimal>();
            DataTable table = db.SelectDb(sql).Tables[0];

            foreach (DataRow dr in table.Rows)
            {
                var item = new ABuffAnimal();
                item.Id = int.Parse(dr["AnimalId"].ToString());
                item.breedRegistryNumber = dr["BreedRegistryNumber"].ToString();
                item.Photo = dr["Photo"].ToString();
                item.AnimalName = dr["Animal_Name"].ToString();
                item.DateOfBirth = Convert.ToDateTime(dr["Date_of_Birth"].ToString());
                item.CountryOfBirth = dr["Country_Of_Birth"].ToString();
                result.Add(item);
            }
            return result;
        }

        public List<A_Family> getdamfamilybuff()
        {

            string sql = $@"SELECT      A_Family.DamId, A_Buff_Animal.BreedRegistryNumber, A_Buff_Animal.Photo, A_Buff_Animal.Animal_Name, A_Buff_Animal.Date_of_Birth, A_Buff_Animal.Country_Of_Birth
                         FROM            A_Family INNER JOIN
                         A_Buff_Animal ON A_Family.DamId = A_Buff_Animal.id where A_Buff_Animal.Delete_Flag =0";
            var result = new List<A_Family>();
            DataTable table = db.SelectDb(sql).Tables[0];

            foreach (DataRow dr in table.Rows)
            {
                var item = new A_Family();
                item.dam.Id = int.Parse(dr["DamId"].ToString());
                item.dam.breedRegistryNumber = dr["BreedRegistryNumber"].ToString();
                item.dam.Photo = dr["Photo"].ToString();
                item.dam.AnimalName = dr["Animal_Name"].ToString();
                item.dam.DateOfBirth = Convert.ToDateTime(dr["Date_of_Birth"].ToString());
                item.dam.CountryOfBirth = dr["Country_Of_Birth"].ToString();
                result.Add(item);
            }
            return result;
        }
        public class ListFarmerVM
        {
            public string? Id { get; set; }
            public string HerdId { get; set; }
            public string FarmerId { get; set; }
            public string FarmerName { get; set; }
            public string BreedType { get; set; }
            public string FeedingType { get; set; }
            public string FarmerClassification { get; set; }
            public string FarmerAffiliation { get; set; }
            public string CowLevel { get; set; }
        }
        public List<ListFarmerVM> FarmerListView()
        {

            string sql = $@"SELECT  tbl_HerdFarmer.Farmer_Id, tbl_HerdFarmer.Herd_Id, tbl_HerdFarmer.Id, Tbl_Farmers.FirstName, Tbl_Farmers.LastName, H_Feeding_System.FeedingSystemDesc, A_Breed.Breed_Desc, 
                         H_Herd_Classification.Herd_Class_Desc, H_Farmer_Affiliation.F_Desc, H_Buff_Herd.Herd_Code
                            FROM            H_Buff_Herd INNER JOIN
                         tbl_HerdFarmer ON H_Buff_Herd.id = tbl_HerdFarmer.Herd_Id LEFT OUTER JOIN
                         tbl_FarmerBreedType INNER JOIN
                         Tbl_Farmers ON tbl_FarmerBreedType.Farmer_Id = Tbl_Farmers.Id INNER JOIN
                         A_Breed ON tbl_FarmerBreedType.BreedType_Id = A_Breed.id INNER JOIN
                         H_Herd_Classification ON Tbl_Farmers.FarmerClassification_Id = H_Herd_Classification.id INNER JOIN
                         H_Farmer_Affiliation ON Tbl_Farmers.FarmerAffliation_Id = H_Farmer_Affiliation.id ON tbl_HerdFarmer.Farmer_Id = Tbl_Farmers.Id LEFT OUTER JOIN
                         tbl_FarmerFeedingSystem ON Tbl_Farmers.Id = tbl_FarmerFeedingSystem.Farmer_Id LEFT OUTER JOIN
                         H_Feeding_System ON tbl_FarmerFeedingSystem.FeedingSystem_Id = H_Feeding_System.id";
            var result = new List<ListFarmerVM>();
            DataTable table = db.SelectDb(sql).Tables[0];

            foreach (DataRow dr in table.Rows)
            {
                string sql_cowlvl = $@"select Count(*) as Cowlevel from A_Buff_Animal where Herd_Code ='"+ dr["Herd_Code"].ToString() + "'";
             
                DataTable table_cowlvl = db.SelectDb(sql_cowlvl).Tables[0];

                var item = new ListFarmerVM();
                //item.Id = int.Parse(dr["Id"].ToString());
                item.Id = dr["Id"].ToString();
                item.HerdId = dr["Herd_Id"].ToString();
                item.FarmerId = dr["Farmer_Id"].ToString();
                item.FarmerName = dr["LastName"].ToString() + ", "+ dr["FirstName"].ToString();
                item.BreedType = dr["Breed_Desc"].ToString();
                item.FeedingType = dr["FeedingSystemDesc"].ToString();
                item.FarmerClassification = dr["Herd_Class_Desc"].ToString();
                item.FarmerAffiliation = dr["F_Desc"].ToString();
                item.CowLevel = table_cowlvl.Rows[0]["Cowlevel"].ToString();
                result.Add(item);
            }
            return result;
        }
        public List<TblUsersModel_List> getUserList()
        {

            string sqls = $@"SELECT     tbl_UsersModel.Id, tbl_UsersModel.Username, tbl_UsersModel.Password, tbl_UsersModel.Fullname, tbl_UsersModel.Fname, tbl_UsersModel.Lname, tbl_UsersModel.Mname, tbl_UsersModel.Email, 
                         tbl_UsersModel.Gender, tbl_UsersModel.EmployeeID, tbl_UsersModel.JWToken, tbl_UsersModel.FilePath, tbl_UsersModel.Active, tbl_UsersModel.Cno, tbl_UsersModel.Address, tbl_UsersModel.Status, 
                         tbl_UsersModel.Date_Created, tbl_UsersModel.Date_Updated, tbl_UsersModel.Delete_Flag, tbl_UsersModel.Created_By, tbl_UsersModel.Updated_By, tbl_UsersModel.Date_Deleted, tbl_UsersModel.Deleted_By, 
                         tbl_UsersModel.Date_Restored, tbl_UsersModel.Restored_By, tbl_UsersModel.CenterId, tbl_UsersModel.AgreementStatus, tbl_UsersModel.RememberToken, tbl_UsersModel.UserType, tbl_UsersModel.HerdId, tbl_UserTypeModel.code, 
                         tbl_UserTypeModel.name, tbl_StatusModel.Status AS StatusName, tbl_CenterModel.CenterName,isFarmer
                    FROM            tbl_UsersModel LEFT JOIN
                         tbl_UserTypeModel ON tbl_UsersModel.UserType = tbl_UserTypeModel.id INNER JOIN
                         tbl_StatusModel ON tbl_UsersModel.Status = tbl_StatusModel.id LEFT JOIN
                         tbl_CenterModel ON tbl_UsersModel.CenterId = tbl_CenterModel.id
                    WHERE        (tbl_UsersModel.Delete_Flag = 0)";


            var result = new List<TblUsersModel_List>();
            DataTable table = db.SelectDb(sqls).Tables[0];

            foreach (DataRow dr in table.Rows)
            {
                var item = new TblUsersModel_List();
                item.Id = int.Parse(dr["Id"].ToString());
                item.Username = dr["Username"].ToString();
                item.Password = dr["Password"].ToString();
                item.Fullname = dr["Fullname"].ToString();
                item.Fname = dr["Fname"].ToString();
                item.Lname = dr["Lname"].ToString();
                item.Mname = dr["Mname"].ToString();
                item.Email = dr["Email"].ToString();
                item.Gender = dr["Gender"].ToString();
                item.EmployeeId = dr["EmployeeID"].ToString();
                item.Jwtoken = dr["JWToken"].ToString();
                item.FilePath = dr["FilePath"].ToString();
                item.Active = int.Parse(dr["Active"].ToString());
                item.Cno = dr["Cno"].ToString();
                item.Address = dr["Address"].ToString();
                item.Status = int.Parse(dr["Status"].ToString());
                item.DateCreated = dr["Date_Created"].ToString();
                item.DateUpdated = dr["Date_Updated"].ToString();
                item.DeleteFlag = Convert.ToBoolean(dr["Delete_Flag"].ToString());
                item.CreatedBy = dr["Created_By"].ToString();
                item.UpdatedBy = dr["Updated_By"].ToString();
                item.DateDeleted = dr["Date_Deleted"].ToString();
                item.CreatedBy = dr["Created_By"].ToString();
                item.UpdatedBy = dr["Updated_By"].ToString();
                item.DateDeleted = dr["Date_Deleted"].ToString();
                item.DeletedBy = dr["Deleted_By"].ToString();
                item.DateRestored = dr["Date_Restored"].ToString();
                item.RestoredBy = dr["Restored_By"].ToString();
                item.CenterId = int.Parse(dr["CenterId"].ToString());
                item.CenterName = item.CenterId == 0 ? "ALL CENTER" : dr["CenterName"].ToString();
                item.AgreementStatus = bool.Parse(dr["AgreementStatus"].ToString());
                item.RememberToken = dr["RememberToken"].ToString();
                item.UserType = dr["UserType"].ToString();
                item.UserTypeCode = dr["code"].ToString();
                item.UserTypeName = dr["name"].ToString();
                item.StatusName = dr["StatusName"].ToString();
                item.isFarmer = bool.Parse(dr["isFarmer"].ToString());
                item.HerdId = dr["HerdId"].ToString();

                string sql = $@"SELECT tbl_UserTypeModel.[Id],Name as UserType
                            FROM [dbo].[tbl_UserTypeModel] inner join 
                            User_UserTypeAccessTable on tbl_UserTypeModel.Id = User_UserTypeAccessTable.UserTypeId
                             where tbl_UserTypeModel.[Id] ='" + dr["UserType"].ToString() + "'";
                var results = new List<UserTypeAction_Model>();
                DataTable tables = db.SelectDb(sql).Tables[0];
                if (tables.Rows.Count != 0)
                {
                    //    foreach (DataRow dr1 in tables.Rows)
                    //{
                    var r_item = new UserTypeAction_Model();
                    r_item.UserTypeId = tables.Rows[0]["Id"].ToString();
                    r_item.UserType = tables.Rows[0]["UserType"].ToString();

                    string sql_usertype = $@"SELECT tbl_UserTypeModel.[Id],
                                            Name as UserType,
                                            User_UserTypeAccessTable.Module as ModuleId,
                                            User_ModuleTable.Module as ModuleName,
                                            User_ModuleTable.ParentModule as ParentModule,
                                            User_ModuleTable.DateCreated
                                            FROM [dbo].[tbl_UserTypeModel] inner join 
                                            User_UserTypeAccessTable on tbl_UserTypeModel.Id = User_UserTypeAccessTable.UserTypeId inner join
                                            User_ModuleTable on User_UserTypeAccessTable.Module = User_ModuleTable.Id
                                            where tbl_UserTypeModel.[Id]='" + tables.Rows[0]["Id"].ToString() + "'" +
                                            " group by tbl_UserTypeModel.[Id],Name ," +
                                            " User_UserTypeAccessTable.Module ," +
                                            "User_ModuleTable.Module ," +
                                            "User_ModuleTable.ParentModule ," +
                                            "User_ModuleTable.DateCreated";
                    DataTable tbl_usertype = db.SelectDb(sql_usertype).Tables[0];
                    var usertype_item = new List<Module_Model>();
                    //if(tbl_usertype.Rows.Count != 0)
                    //{ 
                    foreach (DataRow dr1 in tbl_usertype.Rows)
                    {
                        var item1 = new Module_Model();
                        item1.ModuleId = dr1["ModuleId"].ToString();
                        item1.ModuleName = dr1["ModuleName"].ToString();
                        item1.ParentModule = dr1["ParentModule"].ToString();
                        item1.DateCreated = dr1["DateCreated"].ToString();
                        string sql_actions = $@"select distinct
                                    Action_Id,
                                    Action_tbl.Action_name,
                                    User_UserTypeAccessTable.Module
                                    from 
                                    User_UserTypeAccessTable inner join
                                    Action_tbl on Action_tbl.Action_Id = User_UserTypeAccessTable.ActionId

                                     where Module ='" + dr1["ModuleId"].ToString() + "'";
                        DataTable action_tbl = db.SelectDb(sql_actions).Tables[0];
                        var action_item = new List<ActionModel>();
                        foreach (DataRow dra in action_tbl.Rows)
                        {
                            var items = new ActionModel();
                            items.ActionId = dra["Action_Id"].ToString();
                            items.Actions = dra["Action_name"].ToString();
                            action_item.Add(items);

                        }
                        item1.Actions = action_item;
                        usertype_item.Add(item1);

                    }
                    r_item.Module = usertype_item;

                    results.Add(r_item);
                }
                item.userAccessModels = results;
                result.Add(item);
            }
            return result;
        }
        public List<Module_Model> modulelist()
        {
            string sql = $@"SELECT [Id]
                              ,[Module]
                              ,[ParentModule]
                              ,[DateCreated]
                          FROM [dbo].[User_ModuleTable] ";
            var result = new List<Module_Model>();
            DataTable table = db.SelectDb(sql).Tables[0];
            foreach (DataRow dr in table.Rows)
            {
                var item = new Module_Model();
                item.ModuleId = dr["Id"].ToString();
                item.ModuleName = dr["Module"].ToString();
                item.ParentModule = dr["ParentModule"].ToString();
                item.DateCreated = dr["DateCreated"].ToString();
                string sql_actions = $@"select 
                                    Action_Id,
                                    Action_tbl.Action_name,
                                    User_ActionTable.Module
                                    from 
                                    User_ActionTable inner join
                                    Action_tbl on Action_tbl.Action_Id = User_ActionTable.ActionId

                                     where User_ActionTable.Module ='" + dr["Id"].ToString() + "'";
                DataTable action_tbl = db.SelectDb(sql_actions).Tables[0];
                var action_item = new List<ActionModel>();
                foreach (DataRow drw in action_tbl.Rows)
                {
                    var items = new ActionModel();
                    items.ActionId = drw["Action_Id"].ToString();
                    items.Actions = drw["Action_name"].ToString();
                    action_item.Add(items);

                }
                item.Actions = action_item;
                result.Add(item);
            }
            return result;
        }


		public List<TblUsersModel_List> GetUserList_All(String username, String password)
		{
			//var decryptedPassword = Cryptography.Decrypt(password);
			var encryptedPassword = Cryptography.Encrypt(password);
            

			string query = @"SELECT U.*, F.Id AS FarmerId, UT.code, UT.name, UT.userAccesasId, 
                S.Status AS StatusName, C.CenterName, H.Herd_Code
                FROM tbl_UsersModel U
                INNER JOIN tbl_UserTypeModel UT ON U.UserType = UT.Id
                LEFT JOIN tbl_StatusModel S ON U.Status = S.Id
                LEFT JOIN tbl_CenterModel C ON U.CenterId = C.Id
                LEFT JOIN Tbl_Farmers F ON U.Id = F.User_Id
                LEFT JOIN H_Buff_Herd H ON U.HerdId = H.Id
                WHERE U.Delete_Flag = 0 AND U.Username = @Username AND U.Password = @Password";

			var parameters = new[]
			{
				new SqlParameter("@Username", username),
				new SqlParameter("@Password", encryptedPassword)
			};

			var result = new List<TblUsersModel_List>();
			DataTable table = db.SelectDb(query, parameters).Tables[0];

			foreach (DataRow dr in table.Rows)
			{
				var user = MapUser(dr);
				user.userAccessModels = GetUserAccess(user.UserType);
				result.Add(user);
			}

			return result;
		}

		private TblUsersModel_List MapUser(DataRow dr)
		{
			return new TblUsersModel_List
			{
				Id = Convert.ToInt32(dr["Id"]),
				FarmerId = dr["FarmerId"]?.ToString()!,
				Fullname = dr["Fullname"].ToString()!,
				Fname = dr["Fname"].ToString()!,
				Lname = dr["Lname"].ToString()!,
				Mname = dr["Mname"].ToString()!,
				Email = dr["Email"].ToString()!,
				Gender = dr["Gender"].ToString()!,
				EmployeeId = dr["EmployeeID"].ToString()!,
				Jwtoken = dr["JWToken"].ToString()!,
				FilePath = dr["FilePath"].ToString()!,
				Active = Convert.ToInt32(dr["Active"]),
				Cno = dr["Cno"].ToString()!,
				Address = dr["Address"].ToString()!,
				Status = Convert.ToInt32(dr["Status"]),
				DateCreated = dr["Date_Created"].ToString(),
				DateUpdated = dr["Date_Updated"].ToString(),
				DeleteFlag = Convert.ToBoolean(dr["Delete_Flag"]),
				CreatedBy = dr["Created_By"].ToString()!,
				UpdatedBy = dr["Updated_By"].ToString()!,
				DateDeleted = dr["Date_Deleted"].ToString(),
				DeletedBy = dr["Deleted_By"].ToString()!,
				DateRestored = dr["Date_Restored"].ToString(),
				RestoredBy = dr["Restored_By"].ToString()!,
				CenterId = Convert.ToInt32(dr["CenterId"]),
				CenterName = string.IsNullOrEmpty(dr["CenterId"].ToString()) ? "ALL CENTER" : dr["CenterName"].ToString()!,
				AgreementStatus = Convert.ToBoolean(dr["AgreementStatus"]),
				RememberToken = dr["RememberToken"].ToString()!,
				UserType = dr["UserType"].ToString()!,
				UserTypeCode = dr["code"].ToString()!,
				UserTypeName = dr["name"].ToString()!,
				StatusName = dr["StatusName"].ToString(),
				UserAccessId = dr["userAccesasId"].ToString()!,
				isFarmer = Convert.ToBoolean(dr["isFarmer"]),
				HerdId = string.IsNullOrEmpty(dr["HerdId"].ToString()) ? "0" : dr["HerdId"].ToString()!,
				HerdCode = string.IsNullOrEmpty(dr["Herd_Code"].ToString()) ? "0" : dr["Herd_Code"].ToString()!
			};
		}

		private List<UserTypeAction_Model> GetUserAccess(string userTypeId)
		{
			var result = new List<UserTypeAction_Model>();

			string userTypeQuery = @"SELECT Id, Name AS UserType, userAccesasId FROM tbl_UserTypeModel WHERE Id = @Id";
			var userTypeParams = new[] { new SqlParameter("@Id", userTypeId) };
			var userTypeTable = db.SelectDb(userTypeQuery, userTypeParams).Tables[0];

			foreach (DataRow row in userTypeTable.Rows)
			{
				var access = new UserTypeAction_Model
				{
					UserTypeId = row["Id"].ToString(),
					UserType = row["UserType"].ToString(),
					userAccesasId = row["userAccesasId"].ToString(),
					Module = GetModulesWithActions(row["Id"].ToString()!)
				};

				result.Add(access);
			}

			return result;
		}

		private List<Module_Model> GetModulesWithActions(string userTypeId)
		{
			var modules = new List<Module_Model>();

			string query = @"SELECT M.Id AS ModuleId, M.Module, M.ParentModule, A.DateCreated
                FROM User_UserTypeAccessTable A
                JOIN User_ModuleTable M ON A.Module = M.Id
                WHERE A.UserTypeId = @UserTypeId";

			var parameters = new[] { new SqlParameter("@UserTypeId", userTypeId) };
			var table = db.SelectDb(query, parameters).Tables[0];

			foreach (DataRow dr in table.Rows)
			{
                var module = new Module_Model();
                module.ModuleId = dr["ModuleId"].ToString();
                module.ModuleName = dr["Module"].ToString();
                module.ParentModule = dr["ParentModule"].ToString();
                module.DateCreated = dr["DateCreated"].ToString();

				string queryUserActions = @"SELECT A.ActionId, B.Action_name
                FROM User_ActionTable A
                INNER JOIN Action_tbl B ON A.ActionId = B.Action_Id
                WHERE A.Module ='" + dr["ModuleId"].ToString() + "'";

				DataTable actiontable = db.SelectDb(queryUserActions).Tables[0];
				var action_item = new List<ActionModel>();

				foreach (DataRow obj in actiontable.Rows)
				{
					var items = new ActionModel();
					items.ActionId = obj["ActionId"].ToString();
					items.Actions = obj["Action_name"].ToString();
					action_item.Add(items);
				}
				module.Actions = action_item;
				modules.Add(module);
			}

			return modules;
		}

		
		public StatusReturns GetUserLogIn(string username, string password, string? ipaddress, string? location)
        {
            var result = new List<TblUsersModel>();
            bool compr_user = false;
            bool _isFarmer = false;
            int? _center = null;
            if (username.Length != 0 || password.Length != 0)
            {
                var param = new IDataParameter[]
                {
                    new SqlParameter("@Username",username),
                    new SqlParameter("@Password",Cryptography.Encrypt(password))
                };
                DataTable dt = db.SelectDb_SP("GetUserLogIn", param).Tables[0];
                if (dt.Rows.Count != 0)
                {
                    string user_statId = dt.Rows[0]["StatusId"].ToString();
                    string user_activeId = dt.Rows[0]["ActiveStatusId"].ToString();
                    bool user_deleteFlag = Convert.ToBoolean(dt.Rows[0]["Delete_Flag"]);
                    if (user_deleteFlag)
                    {
                        Id = dt.Rows[0]["Id"].ToString();
                        Stats = "Error";
                        Mess = "Invalid Log In";
                        JWT = "";
                    }
                    else if (user_activeId == "1")
                    {
                        //active
                        switch (user_statId)
                        {
                            case "3":
                                //VERIFIED
                                Id = dt.Rows[0]["Id"].ToString();
                                Stats = "Error";
                                Mess = "Your account is under screening. Please contact administrator.";
                                JWT = "";
                                break;
                            case "4":
                                //UNVERIFIED
                                Id = dt.Rows[0]["Id"].ToString();
                                Stats = "Error";
                                Mess = "Your account is unverified. Please contact administrator.";
                                JWT = "";
                                break;
                            case "5":
                                //APPROVED


                                compr_user = String.Equals(dt.Rows[0]["Username"].ToString().Trim(), username, StringComparison.Ordinal);

                                if (compr_user)
                                {
                                    string pass = Cryptography.Decrypt(dt.Rows[0]["password"].ToString().Trim());
                                    if ((pass).Equals(password))
                                    {
                                        StringBuilder str_build = new StringBuilder();
                                        Random random = new Random();
                                        int length = 8;
                                        char letter;

                                        _isFarmer = Convert.ToBoolean(dt.Rows[0]["isFarmer"]);
                                        _center = (int)dt.Rows[0]["CenterId"];

                                        for (int i = 0; i < length; i++)
                                        {
                                            double flt = random.NextDouble();
                                            int shift = Convert.ToInt32(Math.Floor(25 * flt));
                                            letter = Convert.ToChar(shift + 2);
                                            str_build.Append(letter);
                                        }
                                        //gv.AudittrailLogIn("Successfully", "Log In Form", dt.Rows[0]["EmployeeID"].ToString(), 7);
                                        var token = Cryptography.Encrypt(str_build.ToString());
                                        string strtokenresult = token;
                                        string[] charsToRemove = new string[] { "/", ",", ".", ";", "'", "=", "+" };
                                        foreach (var c in charsToRemove)
                                        {
                                            strtokenresult = strtokenresult.Replace(c, string.Empty);
                                        }

                                        string query = $@"update tbl_UsersModel set JWToken='" + string.Concat(strtokenresult.TakeLast(15)) + "' where id = '" + dt.Rows[0]["id"].ToString() + "'";
                                        db.DB_WithParam(query);
                                        Id = dt.Rows[0]["Id"].ToString();
                                        Stats = "Ok";
                                        Mess = "Successfully Log In";
                                        JWT = string.Concat(strtokenresult.TakeLast(15));
                                    }
                                    else
                                    {
                                        string sql = $@"select * from tbl_Attempts where UserId ='" + dt.Rows[0]["Id"].ToString() + "'";
                                        DataTable user_dt = db.SelectDb(sql).Tables[0];
                                        if (user_dt.Rows.Count != 0)
                                        {
                                            //update
                                            int attemp_count = int.Parse(user_dt.Rows[0]["AttemptCount"].ToString());
                                            if (attemp_count > 5)
                                            {
                                                string update_attempts = $@"update tbl_Attempts set AttemptCount ='" + attemp_count + 1 + "'  where id ='" + dt.Rows[0]["Id"].ToString() + "'";
                                                db.DB_WithParam(update_attempts);
                                            }
                                            else
                                            {
                                                Id = dt.Rows[0]["Id"].ToString();
                                                Stats = "Error";
                                                Mess = "User LogIn Attempts Exceeded. Please contact admin";
                                                JWT = "";
                                            }


                                        }
                                        else
                                        {
                                            string OTPInsert = $@"INSERT INTO [dbo].[tbl_Attempts]
                                                               ([UserId]
                                                               ,[AttemptCount]
                                                               ,[IPAddress]
                                                               ,[Location])
                                                                VALUES
                                                               ('" + dt.Rows[0]["Id"].ToString() + "'" +
                                                               ",'1'," +
                                                               "'" + ipaddress + "'," +
                                                               "'" + location + "')";
                                            db.DB_WithParam(OTPInsert);
                                            Id = dt.Rows[0]["Id"].ToString();
                                            Stats = "Error";
                                            Mess = "Invalid Log In";
                                            JWT = "";
                                            //insert

                                        }
                                        //update login attempts

                                    }
                                }
                                break;
                            case "6":
                                //REGISTERED
                                Id = dt.Rows[0]["Id"].ToString();
                                Stats = "Error";
                                Mess = "Your account is for approval. Please contact administrator.";
                                JWT = "";

                                break;
                            default:
                                break;
                        }


                    }
                    else
                    {
                        //inactive
                        Id = dt.Rows[0]["Id"].ToString();
                        Stats = "Error";
                        Mess = "User is InActive. Please contact your administrator. ";
                        JWT = "";

                    }

                }
                else
                {
                    Id = "0";
                    Stats = "Error";
                    Mess = "Invalid LogIn";
                    JWT = "";
                }
                string sqls = $@"select Username from tbl_UsersModel where Username ='" + username + "'";
                DataTable table = db.SelectDb(sqls).Tables[0];
                InsertAuditTrail("Log In " + Stats + " " + Mess, DateTime.Now.ToString("yyyy-MM-dd"), "LogIn Module", username, "0");


            }
            else
            {


            }
            StatusReturns results = new StatusReturns
            {
                Id = Id,
                Status = Stats,
                Message = Mess,
                isFarmer = _isFarmer,
                Center = _center,
                JwtToken = JWT
            };
            return results;
        }

        public List<ABloodComp_2> getBlodyList()
        {



            string sql = $@"select Id, Blood_Code, Blood_Desc, Status,Date_Created from A_Blood_Comp where Delete_Flag ='False'";
            var result = new List<ABloodComp_2>();
            DataTable table = db.SelectDb(sql).Tables[0];

            foreach (DataRow dr in table.Rows)
            {
                var item = new ABloodComp_2();
                item.Id = int.Parse(dr["id"].ToString());
                item.BloodCode = dr["Blood_Code"].ToString();
                item.BloodDesc = dr["Blood_Desc"].ToString();
                item.Status = dr["Status"].ToString();
                item.DateCreated = dr["Date_Created"].ToString();

                result.Add(item);
            }

            return result;
        }


    }
}