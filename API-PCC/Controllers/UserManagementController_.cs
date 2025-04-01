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
using NuGet.Packaging;
using NuGet.Protocol.Core.Types;
using System.Data;
using System.Data.SqlClient;
using static API_PCC.Controllers.UserController;

namespace API_PCC.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly PCC_DEVContext _context;
        DbManager db = new DbManager();
        DBMethods dbmet = new DBMethods();
        string Stats = "";
        string Mess = "";
        string JWT = "";
        public UserManagementController(PCC_DEVContext context)
        {
            _context = context;
        }
        public class Module_Model
        {
            public string? ModuleName { get; set; }
            public string? ParentModule { get; set; }
            public string? DateCreated { get; set; }
            public List<ActionModel> Actions { get; set; }
        }  
        public class ActionModel
        {
            public string? Actions { get; set; }
        }


        [HttpPost]
        public async Task<ActionResult<IEnumerable<ABirthType>>> listsss (BirthTypesSearchFilterModel searchFilter)
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

        [HttpPost]
        public async Task<ActionResult<IEnumerable<UserPagedModel>>> List(CommonSearchFilterModel searchFilter)
        {
            //try
            //{
            //    DataTable queryResult = db.SelectDb_WithParamAndSorting(QueryBuilder.buildUserSearchQuery(searchFilter), null, populateSqlParameters(searchFilter));
            //    var result = buildUserPagedModel(searchFilter, queryResult);
            //    return Ok(result);
            //}
            //catch (Exception ex)
            //{

            //    return Problem(ex.GetBaseException().ToString());
            //}
            try
            {
                var filter = new Dictionary<string, object>();
                filter.Add("searchParam", searchFilter.searchParam);
                List<TblUsersModel> userList = await buildUserManagementSearchQuery(filter).ToListAsync();
                var result = buildUserPagedModel(searchFilter, userList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }
        private IQueryable<TblUsersModel> buildUserManagementSearchQuery(Dictionary<string, object> filter)
        {
            IQueryable<TblUsersModel> query = _context.TblUsersModels;

            query = query
                .Include(user => user.userAccessModels)
                .ThenInclude(userAccessModel => userAccessModel.userAccess)
                .Where(user => !user.DeleteFlag);


            // assuming that you return all records when nothing is specified in the filter

            if (filter.ContainsKey("searchParam"))
            {
                var searchParam = filter["searchParam"].ToString();
                query = query.Where(user =>
                               user.Fname.Contains(searchParam) ||
                               user.Lname.Contains(searchParam) ||
                               user.Mname.Contains(searchParam) ||
                               user.Email.Contains(searchParam));
            }

            if (filter.ContainsKey("forApproval") && Convert.ToBoolean(filter["forApproval"]))
            {
                query = query.Where(user => user.Status.Equals(3));
            }

            if (filter.ContainsKey("username"))
            {
                var username = filter["username"].ToString();
                query = query.Where(user => user.Username.Equals(username));
            }


            if (filter.ContainsKey("Id"))
            {
                var id = filter["Id"];
                query = query.Where(user => user.Id.Equals(id));
            }

            query = query.OrderByDescending(e => e.Id);

            return query;
        }

        [HttpPost]
        public async Task<ActionResult<TblUsersModel>> msbuff_Registration(RegistrationModel userTbl)
        {
            string filepath = "";
            var user_list = _context.TblUsersModels.AsEnumerable().Where(a => a.Username.Equals(userTbl.Username, StringComparison.Ordinal)).ToList();
            if (user_list.Count == 0)
            {
                var email_count = _context.TblUsersModels.Where(a => a.Email == userTbl.Email).ToList();
                if (email_count.Count != 0)
                {
                    Stats = "Error";
                    Mess = "Email Already Used!";
                    JWT = "";
                }
                else
                {
                    StringBuilder str_build = new StringBuilder();
                    Random random = new Random();
                    int length = 8;
                    char letter;

                    for (int i = 0; i < length; i++)
                    {
                        double flt = random.NextDouble();
                        int shift = Convert.ToInt32(Math.Floor(25 * flt));
                        letter = Convert.ToChar(shift + 2);
                        str_build.Append(letter);
                    }

                    var token = Cryptography.Encrypt(str_build.ToString());
                    string strtokenresult = token;
                    string[] charsToRemove = new string[] { "/", ",", ".", ";", "'", "=", "+" };
                    foreach (var c in charsToRemove)
                    {
                        strtokenresult = strtokenresult.Replace(c, string.Empty);
                    }
                    if (userTbl.FilePath == null)
                    {
                        filepath = "";
                    }
                    else
                    {
                        filepath = userTbl.FilePath.Replace(" ", "%20");
                    }
                    string fullname = userTbl.Fname + ", " + userTbl.Mname + ", " + userTbl.Lname;
                    string user_insert = $@"INSERT INTO [dbo].[tbl_UsersModel]
                                           ([Username]
                                           ,[Password]
                                           ,[Fullname]
                                           ,[Fname]
                                           ,[Lname]
                                           ,[Mname]
                                           ,[Email]
                                           ,[Gender]
                                           ,[EmployeeID]
                                           ,[JWToken]
                                           ,[FilePath]
                                           ,[Active]
                                           ,[Cno]
                                           ,[Address]
                                           ,[Status]
                                           ,[Date_Created]
                                           ,[CenterId]
                                           ,[AgreementStatus]
                                           ,[UserType]
                                           ,[Delete_Flag])
                                     VALUES
                                           ('" + userTbl.Username + "'" +
                                            ",'" + Cryptography.Encrypt(userTbl.Password) + "'," +
                                           "'" + fullname + "'," +
                                           "'" + userTbl.Fname + "'," +
                                           "'" + userTbl.Lname + "'," +
                                           "'" + userTbl.Mname + "'," +
                                           "'" + userTbl.Email + "'," +
                                           "'" + userTbl.Gender + "'," +
                                           "'" + userTbl.EmployeeId + "'," +
                                           "'" + string.Concat(strtokenresult.TakeLast(15)) + "'," +
                                           "'" + filepath + "'," +
                                           "'1'," +
                                           "'" + userTbl.Cno + "'," +
                                           "'" + userTbl.Address + "'," +
                                           "'5'," +
                                           "'" + DateTime.Now.ToString("yyyy-MM-dd") + "'," +
                                           "'" + userTbl.CenterId + "'," +
                                           "'" + userTbl.AgreementStatus + "'," +
                                           "'" + userTbl.UserType + "'," +
                                           "'0')";
                    db.DB_WithParam(user_insert);




                    Stats = "Ok";
                    Mess = "Successfully Registered!";
                    JWT = string.Concat(strtokenresult.TakeLast(15));
                }
            }
            else
            {
                Stats = "Error";
                Mess = "Username Already Exist!";
                JWT = "";
            }
            StatusReturns result = new StatusReturns
            {
                Status = Stats,
                Message = Mess,
                JwtToken = JWT
            };

            return Ok(result);
        }
        public class ChangePassword
        {
            public int Id { get; set; }
            public string Password { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> msbuff_Changepassword(ChangePassword data)
        {


            try
            {
                string sql = $@"select  * from tbl_UsersModel  where   Id='" + data.Id + "' and Delete_Flag = 'False'";
                DataTable dt = db.SelectDb(sql).Tables[0];
                if (dt.Rows.Count == 0)
                {

                    return BadRequest("No Record found");
                }
                else
                {
                    string query = $@"Update  tbl_UsersModel set Password = '" + Cryptography.Encrypt(data.Password) + "' where  Id='" + data.Id + "'";
                    db.DB_WithParam(query);
                    return Ok("Password Successfully Changed");
                }
            }

            catch (Exception ex)
            {
                return BadRequest("Error!");
            }

        }
        [HttpPost]
        public async Task<ActionResult<IEnumerable<UserPagedModel>>> UserForApprovalList(CommonSearchFilterModel searchFilter)
        {
           
            try
            {
                var filter = new Dictionary<string, object>();
                filter.Add("forApproval", true);
                List<TblUsersModel> userList = await buildUserManagementSearchQuery(filter).ToListAsync();
                var result = buildUserPagedModel(searchFilter, userList);
                return Ok(result);
            }

            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }

        // GET: UserManagemetn/search/5
        [HttpGet("{username}")]
        public async Task<ActionResult<IEnumerable<UserResponseModel>>> search(string username)
        {
            try
            {
                var filter = new Dictionary<string, object>();
                filter.Add("username", username);
                List<TblUsersModel> userList = await buildUserManagementSearchQuery(filter).ToListAsync();

                if (userList.Count == 0)
                {
                    return Conflict("No records found!");
                }

                List<UserResponseModel> userResponseModels = convertUserListToResponseModelList(userList);

                return Ok(userResponseModels);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }
        [HttpGet]
        [Route("/UserManagement/useraccess/list/{username}")]
        public async Task<IActionResult> list(string username)
        {
            var userModel = await _context.TblUsersModels
                .Include(user => user.userAccessModels)
                .ThenInclude(userAccessModel => userAccessModel.userAccess)
                .Where(user => user.Username.Equals(username))
                .FirstOrDefaultAsync();

            if (userModel == null)
            {
                return Problem("Username does not exists!");
            }

            var userAccessListModel = populateUserAccessListModel(userModel);
            return Ok(userAccessListModel);
        }
        private UserAccessListModel populateUserAccessListModel(TblUsersModel usersModel)
        {
            var userAccessModels = new UserAccessListModel();
            userAccessModels.username = usersModel.Username;

            var userAccessList = new Dictionary<string, List<int>>();
            foreach (UserAccessModel userAccessModel in usersModel.userAccessModels)
            {

                var userAccessTypeList = new List<int>();
                foreach (UserAccessType userAccessType in userAccessModel.userAccess)
                {
                    userAccessTypeList.Add(userAccessType.Code);
                }

                userAccessList.Add(userAccessModel.module, userAccessTypeList);
            }

            userAccessModels.userAccessList = userAccessList;
            return userAccessModels;
        }

        // GET: usermanagement/useraccess/update/{username}
        [HttpPut]
        [Route("/UserManagement/useraccess/update/{username}")]
        public async Task<IActionResult> update(string username, UserAccessListModel userAccessListModel)
        {
            var userModel = await _context.TblUsersModels
                .Include(user => user.userAccessModels)
                .ThenInclude(userAccessModel => userAccessModel.userAccess)
                .Where(user => user.Username.Equals(username))
                .FirstOrDefaultAsync();

            if (userModel == null)
            {
                return Problem("Username does not exists!");
            }

            userModel.userAccessModels.Clear();
            userModel.userAccessModels.AddRange(populateUserAccessList(userAccessListModel.userAccessList));
            _context.Entry(userModel).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok("Update Successful!");
        }
        // PUT: UserManagement/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, UserUpdateModel userUpdateModel)
        {
            //DataTable userRecord = db.SelectDb_WithParamAndSorting(QueryBuilder.buildUserSearchQueryById(), null, populateSqlParameters(id));
            var filter = new Dictionary<string, object>();
            filter.Add("Id", id);
            var userModel = await buildUserManagementSearchQuery(filter).FirstOrDefaultAsync();

            if (userModel == null)
            {
                return Conflict("No records matched!");
            }

            DataTable userDuplicateCheck = db.SelectDb_WithParamAndSorting(QueryBuilder.buildUserDuplicateCheckUpdateQuery(), null, populateSqlParameters(id, userUpdateModel));

            // check for duplication
            if (userDuplicateCheck.Rows.Count > 0)
            {
                return Conflict("Entity already exists");
            }

            try
            {
                userModel.userAccessModels.Clear();
                populateUser(userModel, userUpdateModel);
                populateUserAccess(userModel, userUpdateModel);
                _context.Entry(userModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Update Successful!");
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }
        private void populateUserAccess(TblUsersModel userModel, UserUpdateModel updateModel)
        {
            userModel.userAccessModels.AddRange(populateUserAccessList(updateModel.userAccess));
        }

        private List<UserAccessModel> populateUserAccessList(Dictionary<string, List<int>> userAccessModelList)
        {
            var userAccessModels = new List<UserAccessModel>();

            foreach (var access in userAccessModelList)
            {
                var userAccessTypeList = new List<UserAccessType>();
                foreach (int userAccess in access.Value)
                {
                    var userAccessType = new UserAccessType()
                    {
                        Code = userAccess
                    };
                    _context.Attach(userAccessType);

                    userAccessTypeList.Add(userAccessType);
                }

                var userAccessModel = new UserAccessModel()
                {
                    module = access.Key
                };

                _context.Attach(userAccessModel);

                userAccessModel.userAccess.AddRange(userAccessTypeList);
                userAccessModels.Add(userAccessModel);
            }
            return userAccessModels;
        }
        private void populateUser(TblUsersModel userModel, UserUpdateModel userUpdateModel)
        {
            userModel.Username = userUpdateModel.Username;
            userModel.Password = Cryptography.Encrypt(userUpdateModel.Password);
            userModel.Fullname = userUpdateModel.Fullname;
            userModel.Fname = userUpdateModel.Fname;
            userModel.Lname = userUpdateModel.Lname;
            userModel.Mname = userUpdateModel.Mname;
            userModel.Email = userUpdateModel.Email;
            userModel.Gender = userUpdateModel.Gender;
            userModel.EmployeeId = userUpdateModel.EmployeeId;
            userModel.Active = userUpdateModel.Active;
            userModel.Cno = userUpdateModel.Cno;
            userModel.Address = userUpdateModel.Address;
            userModel.CenterId = userUpdateModel.CenterId;
            userModel.AgreementStatus = userUpdateModel.AgreementStatus;
    }

        // POST: UserManagement/delete/5
        [HttpPost]
        public async Task<IActionResult> delete(DeletionModel deletionModel)
        {
            DataTable userRecord = db.SelectDb_WithParamAndSorting(QueryBuilder.buildUserSearchQueryById(), null, populateSqlParameters(deletionModel.id));

            if (userRecord.Rows.Count == 0)
            {
                return Conflict("No records found!");
            }

            var userModel = convertDataRowToUser(userRecord.Rows[0]);

            try
            {
                userModel.DeleteFlag = true;
                userModel.DateDeleted = DateTime.Now;
                userModel.DeletedBy = deletionModel.deletedBy;
                userModel.DateRestored = null;
                userModel.RestoredBy = "";
                _context.Entry(userModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Deletion Successful!");
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }

        // POST: UserManagemetn/restore/
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> restore(RestorationModel restorationModel)
        {


            DataTable userRecord = db.SelectDb_WithParamAndSorting(QueryBuilder.buildUserDeletedSearchQueryById(), null, populateSqlParameters(restorationModel.id));

            if (userRecord.Rows.Count == 0)
            {
                return Conflict("No deleted records found!");
            }

            var userModel = convertDataRowToUser(userRecord.Rows[0]);

            try
            {
                userModel.DeleteFlag = !userModel.DeleteFlag;
                userModel.DateDeleted = null;
                userModel.DeletedBy = "";
                userModel.DateRestored = DateTime.Now;
                userModel.RestoredBy = restorationModel.restoredBy;

                _context.Entry(userModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Restoration Successful!");
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }


        private SqlParameter[] populateSqlParameters(CommonSearchFilterModel searchFilter)
        {

            var sqlParameters = new List<SqlParameter>();

            if (searchFilter.searchParam != null && searchFilter.searchParam != "")
            {
                sqlParameters.Add(new SqlParameter
                {
                    ParameterName = "SearchParam",
                    Value = searchFilter.searchParam ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.VarChar,
                });
            }

            return sqlParameters.ToArray();
        }

        private SqlParameter[] populateSqlParameters(int id)
        {
            var sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "Id",
                Value = id,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });
            return sqlParameters.ToArray();
        }

        private SqlParameter[] populateSqlParameters(string username)
        {
            var sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "Username",
                Value = username ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });
            return sqlParameters.ToArray();
        }
        private SqlParameter[] populateSqlParameters(int id, UserUpdateModel userUpdateModel)
        {
            var sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "Id",
                Value = id,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "Username",
                Value = userUpdateModel.Username,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "Fullname",
                Value = userUpdateModel.Fullname,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "Fname",
                Value = userUpdateModel.Fname,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "Lname",
                Value = userUpdateModel.Lname,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "Mname",
                Value = userUpdateModel.Mname,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "Email",
                Value = userUpdateModel.Email,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            return sqlParameters.ToArray();
        }

        private List<UserPagedModel> buildUserPagedModel(CommonSearchFilterModel searchFilter, List<TblUsersModel> userList)
        {
            int pagesize = searchFilter.pageSize == 0 ? 10 : searchFilter.pageSize;
            int page = searchFilter.page == 0 ? 1 : searchFilter.page;
            var items = (dynamic)null;

            int totalItems = userList.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
            items = userList.AsEnumerable().Skip((page - 1) * pagesize).Take(pagesize).ToList();

            List<UserResponseModel> userResponseModels = convertUserListToResponseModelList(userList);

            var result = new List<UserPagedModel>();
            var item = new UserPagedModel();

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
            item.items = userResponseModels;
            result.Add(item);

            return result;
        }

        private List<TblUsersModel> convertDataRowToUserList(List<DataRow> dataRowList)
        {
            var userList = new List<TblUsersModel>();

            foreach (DataRow dataRow in dataRowList)
            {
                var user = DataRowToObject.ToObject<TblUsersModel>(dataRow);
                userList.Add(user);
            }

            return userList;
        }

        private TblUsersModel convertDataRowToUser(DataRow dataRow)
        {
            return DataRowToObject.ToObject<TblUsersModel>(dataRow);
        }

        private List<UserResponseModel> convertUserListToResponseModelList(List<TblUsersModel> userList)
        {
            var userResponseModels = new List<UserResponseModel>();

            foreach (TblUsersModel user in userList)
            {
                var userResponseModel = new UserResponseModel()
                {
                    Id= user.Id,
                    FilePath= user.FilePath,
                    Username = user.Username,
                    Fullname = user.Fullname,
                    Fname = user.Fname,
                    Lname = user.Lname,
                    Mname = user.Mname,
                    Email = user.Email,
                    Gender = user.Gender,
                    EmployeeId = user.EmployeeId,
                    Active = user.Active,
                    Cno = user.Cno,
                    Address = user.Address,
                    CenterId = user.CenterId,
                    AgreementStatus = user.AgreementStatus,
                    UserType = user.UserType
                };
                userResponseModels.Add(userResponseModel);
            }

            return userResponseModels;
        }

    }
}
