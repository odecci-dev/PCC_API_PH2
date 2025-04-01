using API_PCC.ApplicationModels;
using API_PCC.Data;
using API_PCC.EntityModels;
using API_PCC.Manager;
using API_PCC.Models;
using API_PCC.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Text;
using static API_PCC.Manager.DBMethods;
using System.Data.SqlClient;
using NuGet.Packaging;
using static API_PCC.Controllers.BloodCompsController;
using System.Drawing.Printing;
using static API_PCC.Controllers.UserController;
using static API_PCC.Controllers.UserManagementController;
using System;

namespace API_PCC.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        string sql = "";
        string Stats = "";
        string Mess = "";
        string JWT = "";
        DbManager db = new DbManager();
        MailSender _mailSender;
        private readonly PCC_DEVContext _context;
        DBMethods dbmet = new DBMethods();
        private readonly EmailSettings _emailsettings;
        public UserController(PCC_DEVContext context, IOptions<EmailSettings> emailsettings)
        {
            _context = context;
            _emailsettings = emailsettings.Value;
            try
            {
                TblMailSenderCredential tblMailSenderCredential = _context.TblMailSenderCredentials.First();

                _emailsettings.username = tblMailSenderCredential.Email;
                _emailsettings.password = tblMailSenderCredential.Password;
            }
            catch (Exception e)
            {
                if (e.Message == "Sequence contains no elements")
                {
                    throw new Exception("No records found for email credentials!!");
                }
                throw e;
            }

        }
        public class EmailSettings
        {
            public Title Title { get; set; }
            public string Host { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string sender { get; set; }

        }

        public class Title
        {
            public string OTP { get; set; }
            public string ForgotPassword { get; set; }
        }
        public class LoginModel
        {
            public string? email { get; set; }
            public string? password { get; set; }
        }
        public class loginCredentials
        {
            public string? username { get; set; }
            public string? password { get; set; }
            public string? ipaddress { get; set; }
            public string? location { get; set; }
            public string? rememberToken { get; set; }
        }
        public class StatusReturns
        {
            public string? Id { get; set; }
            public string? Status { get; set; }
            public string? Message { get; set; }
            public string? JwtToken { get; set; }
            public bool? isFarmer { get; set; }
            public int? Center { get; set; }
            //public List<Module_Model> Modules { get; set; }
        }
        public class Module_Model
        {
            public string? ModuleId { get; set; }
            public string? ModuleName { get; set; }
            public string? ParentModule { get; set; }
            public string? DateCreated { get; set; }
            public List<ActionModel> Actions { get; set; }
        }
        public class ForgotPasswordModel
        {
            public string Email { get; set; }
            public string ForgotPasswordLink { get; set; }
        }
        public class ResetPasswordModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
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

            public int? HerdId { get; set; }
            public int? CenterId { get; set; }

            public bool? AgreementStatus { get; set; }

            public int UserType { get; set; }
            public bool isFarmer { get; set; }
            //public Dictionary<string, List<int>>? userAccess { get; set; }
        }

        // POST: user/login
        [HttpPost]
        public async Task<ActionResult<IEnumerable<TblUsersModel>>> ViewProfile(int id)
        {

            var userlist = _context.TblUsersModels.Where(a => a.Id == id).FirstOrDefault();
            return Ok(userlist);
        }
        [HttpPost]
        public async Task<ActionResult<IEnumerable<TblUsersModel>>> UpdateProfile(int id, string profileimage, string fName, string mName, string lName, string email, string contNum, string address, string username, string herdId)
        {
            string tbl_UsersModel_update = $@"UPDATE [dbo].[tbl_UsersModel] SET 
                                             [FilePath] = '" + profileimage + "'," +
                                             "[Fname] = '" + fName + "'," +
                                             "[Mname] = '" + mName + "'," +
                                             "[Lname] = '" + lName + "'," +
                                             "[Fullname] = '" + fName + " " + mName + " " + lName + "'," +
                                             "[Email] = '" + email + "'," +
                                             "[Cno] = '" + contNum + "'," +
                                             "[Address] = '" + address + "'," +
                                             "[Username] = '" + username + "', " +
                                             "[HerdId] = '" + herdId + "'" +
                                         " WHERE id = '" + id + "'";
            string result = db.DB_WithParam(tbl_UsersModel_update);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<TblUsersModel>>> UserJWT(string JWT)
        {
            var result = (dynamic)null;
            if(JWT != null)
            {
                 result = dbmet.getUserList().Where(a => a.Jwtoken == JWT).FirstOrDefault();
            }
     

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<TblUsersModel>>> login(loginCredentials data)
        {
            var userModel = (dynamic)null;
            int usertype = 0;
            var loginstats = dbmet.GetUserLogIn(data.username, data.password, data.ipaddress, data.location);
            var item = new StatusReturns();

            if (!data.rememberToken.IsNullOrEmpty())
            {
                userModel = dbmet.getUserList().Where(userModel => userModel.Username == data.username).FirstOrDefault();
                
                if (userModel == null) {

                    item.Status = loginstats.Status;
                    item.Message = loginstats.Message;
                    item.JwtToken = loginstats.JwtToken;
                    item.Id = loginstats.Id;
                    item.isFarmer = loginstats.isFarmer;
                    item.Center = loginstats.Center;

                    return Ok(item);

                    //return NotFound("User not found");
                }

                usertype = int.Parse(userModel.UserType);

                //userModel.RememberToken = data.rememberToken;
                //_context.Entry(userModel).State = EntityState.Modified;

                //await _context.SaveChangesAsync();

                string tbl_UsersModel_update = $@"UPDATE [dbo].[tbl_UsersModel] SET 
                                             [FirstName] = '" + data.rememberToken + "'" +
                                        " WHERE id = '" + userModel.Id + "'";
                string result = db.DB_WithParam(tbl_UsersModel_update);
            }
            //var res = dbmet.UserTypeParams(usertype).FirstOrDefault();
            item.Status = loginstats.Status;
            item.Message = loginstats.Message;
            item.JwtToken = loginstats.JwtToken;
            item.Id = loginstats.Id;
            item.isFarmer = loginstats.isFarmer;
            item.Center = loginstats.Center;

            return Ok(item);
        }

        //POST: user/info
        [HttpPost]
        public async Task<ActionResult<IEnumerable<TblUsersModel>>> info(String username, String password)
        {
            //
            if (_context.TblUsersModels == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TblUsersModels' is null!");
            }

            var userInfo = dbmet.getUserList_list(username, password).FirstOrDefault();

            if (userInfo == null)
            {
                return Conflict("User not Found !!");
            }

            return Ok(userInfo);
        }
        public class PaginationModel
        {
            public string? CurrentPage { get; set; }
            public string? NextPage { get; set; }
            public string? PrevPage { get; set; }
            public string? TotalPage { get; set; }
            public string? PageSize { get; set; }
            public string? TotalRecord { get; set; }
            public List<TblUsersModel_List> items { get; set; }


        }
        //private List<PaginationModel> buildUserPagedModel(CommonSearchFilterModel searchFilter)
        //{
        //    var items = (dynamic)null;
        //    int totalItems = 0;
        //    int totalPages = 0;
        //    string page_size = searchFilter.pageSize == 0 ? "10" : searchFilter.pageSize.ToString();

        //    if (searchFilter.searchParam == null || searchFilter.searchParam == string.Empty)
        //    {
        //        var userlist = dbmet.getUserList().ToList();
        //        totalItems = userlist.Count;
        //        totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));
        //        items = userlist.Skip((searchFilter.page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
        //    }
        //    else
        //    {
        //        var userlist = dbmet.getUserList().Where(a => a.Username.ToLower().Contains( searchFilter.searchParam.ToLower()) || a.Fname.ToLower().Contains(searchFilter.searchParam.ToLower()) ||
        //        a.Lname.ToLower().Contains(searchFilter.searchParam.ToLower()) || a.Mname.ToLower().Contains(searchFilter.searchParam.ToLower())).ToList();
        //        totalItems = userlist.Count;
        //        totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));
        //        items = userlist.Skip((searchFilter.page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
        //    }

        //    var result = new List<PaginationModel>();
        //    var item = new PaginationModel();
        //    int pages = searchFilter.page == 0 ? 1 : searchFilter.page;
        //    item.CurrentPage = searchFilter.page == 0 ? "1" : searchFilter.page.ToString();

        //    int page_prev = pages - 1;
        //    //int t_record = int.Parse(items.Count.ToString()) / int.Parse(page_size);

        //    double t_records = Math.Ceiling(double.Parse(totalItems.ToString()) / double.Parse(page_size));
        //    int page_next = searchFilter.page >= t_records ? 0 : pages + 1;
        //    item.NextPage = items.Count % int.Parse(page_size) >= 0 ? page_next.ToString() : "0";
        //    item.PrevPage = pages == 1 ? "0" : page_prev.ToString();
        //    item.TotalPage = t_records.ToString();
        //    item.PageSize = page_size;
        //    item.TotalRecord = totalItems.ToString();
        //    item.items = items;
        //    result.Add(item);

        //    return result;
        //}

        private List<PaginationModel> buildUserPagedModel(UserBaseSearchModel searchFilter)
        {
            var items = (dynamic)null;
            int totalItems = 0;
            int totalPages = 0;
            string page_size = searchFilter.pageSize == 0 ? "10" : searchFilter.pageSize.ToString();

            var userlist = dbmet.getUserList().ToList();

            //no center to base the list on
            //if (string.IsNullOrEmpty(searchFilter.centerId))
            //{
            //    userlist.Clear();
            //}
            if (!searchFilter.centerId.Equals("0") && !string.IsNullOrEmpty(searchFilter.centerId))
            {
                userlist = userlist.Where(a => a.CenterId.ToString().Equals(searchFilter.centerId)).ToList();
            }

            if (!searchFilter.herdId.Equals("0") && !string.IsNullOrEmpty(searchFilter.herdId))
            {
                userlist = userlist.Where(a => a.HerdId.ToString().Equals(searchFilter.herdId)).ToList();
            }

            if (searchFilter.farmerSearching == true)
            {
                userlist = userlist.Where(a => a.isFarmer == true).ToList();
            }

            if (!string.IsNullOrEmpty(searchFilter.dateRegisteredFrom) && !string.IsNullOrEmpty(searchFilter.dateRegisteredTo))
            {
                if (DateTime.TryParse(searchFilter.dateRegisteredFrom, out DateTime dateFrom) &&
                    DateTime.TryParse(searchFilter.dateRegisteredTo, out DateTime dateTo))
                {
                    userlist = userlist.Where(a => DateTime.Parse(a.DateCreated).Date >= dateFrom.Date &&
                                                   DateTime.Parse(a.DateCreated).Date <= dateTo.Date).ToList();
                }
                else
                {
                    userlist.Clear(); 
                }
            }

            if (searchFilter.searchParam == null || searchFilter.searchParam == string.Empty)
                {
                    totalItems = userlist.Count;
                    totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));
                    items = userlist.Skip((searchFilter.page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
                }
                else
                {
                    userlist = userlist.Where(a => a.Username.ToLower().Contains(searchFilter.searchParam.ToLower()) || a.Fname.ToLower().Contains(searchFilter.searchParam.ToLower()) ||
                    a.Lname.ToLower().Contains(searchFilter.searchParam.ToLower()) || a.Mname.ToLower().Contains(searchFilter.searchParam.ToLower())).ToList();
                    totalItems = userlist.Count;
                    totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));
                    items = userlist.Skip((searchFilter.page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();
                }


                var result = new List<PaginationModel>();
                var item = new PaginationModel();
                int pages = searchFilter.page == 0 ? 1 : searchFilter.page;
                item.CurrentPage = searchFilter.page == 0 ? "1" : searchFilter.page.ToString();

                int page_prev = pages - 1;
                //int t_record = int.Parse(items.Count.ToString()) / int.Parse(page_size);

                double t_records = Math.Ceiling(double.Parse(totalItems.ToString()) / double.Parse(page_size));
                int page_next = searchFilter.page >= t_records ? 0 : pages + 1;
                item.NextPage = items.Count % int.Parse(page_size) >= 0 ? page_next.ToString() : "0";
                item.PrevPage = pages == 1 ? "0" : page_prev.ToString();
                item.TotalPage = t_records.ToString();
                item.PageSize = page_size;
                item.TotalRecord = totalItems.ToString();
                item.items = items;
                result.Add(item);

                return result;
            
        }

        //POST: user/listAll
        [HttpPost]
        public async Task<ActionResult<IEnumerable<TblUsersModel>>> listAll(UserBaseSearchModel searchFilter)
        {
            {
                try
                {
                    var result = buildUserPagedModel(searchFilter);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return Problem(ex.GetBaseException().ToString());
                }
            }
        }
        private SqlParameter[] populateSearchSqlParameters(CommonSearchFilterModel searchFilterModel)
        {

            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "SearchParam",
                Value = searchFilterModel.searchParam ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            return sqlParameters.ToArray();
        }
        public partial class approvalId
        {
            public int Id { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> ApproveRegistration(List<approvalId> data)
        {
            try
            {
                string status = "";
                string username = "";
                if (_context.TblUsersModels == null)
                {
                    return Problem("Entity set 'PCC_DEVContext.OTP' is null!");
                }
                for (int x = 0; x < data.Count; x++)
                {
                    var registOtpModels = _context.TblUsersModels.Where(otpModel => otpModel.Id == data[x].Id).OrderByDescending(a => a.Id).FirstOrDefault();

                    if (registOtpModels != null)
                    {
                        if (registOtpModels.Id == data[x].Id)
                        {
                            registOtpModels.Status = 5;
                            _context.Entry(registOtpModels).State = EntityState.Modified;

                            //var userModel = dbmet.getUserList().Where(user => user.Id == data[x].Id).FirstOrDefault();
                            //_context.Entry(userModel).State = EntityState.Modified;
                            //userModel.Status = 5;
                            await _context.SaveChangesAsync();
                            string sql1 = $@"SELECT [id],Username
                                            FROM [dbo].[tbl_UsersModel] where Email ='" + registOtpModels.Email + "'";
                            DataTable userid = db.SelectDb(sql1).Tables[0];
                            username = userid.Rows[0]["Username"].ToString();
                            string tbl_UsersModel_update = $@"UPDATE [dbo].[tbl_UsersModel] SET 
                                             [Status] = '4'" +
                               " WHERE id = '" + userid.Rows[0]["id"].ToString() + "'";
                            string result = db.DB_WithParam(tbl_UsersModel_update);
                            status = "Approved Registration";
                            return Ok(status);
                        }
                        else
                        {
                            //var userModel = _context.TblUsersModels.Where(user => user.Id == data.Id).FirstOrDefault();
                            //_context.Entry(userModel).State = EntityState.Modified;
                            //userModel.Status = 4;
                            //await _context.SaveChangesAsync();
                            status = "No record found.";
                            return Problem(status);
                        }

                    }
                    else
                    {
                        status = "Record not found on database!";
                        return BadRequest(status);
                    }

                }

                dbmet.InsertAuditTrail("ApproveRegistration" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "User Module", username, "0");

                return Ok(status);
            }

            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult<TblUsersModel>> register(RegistrationModel userTbl)
        {
            string filepath = "";
            bool? _IsFarmer = false;
            int? _Center = 0;
            var user_list = dbmet.getUserList().AsEnumerable().Where(a => a.Username.Equals(userTbl.Username, StringComparison.Ordinal)).ToList();
            if (user_list.Count == 0)
            {
                var email_count = dbmet.getUserList().Where(a => a.Email == userTbl.Email).ToList();
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
                    string userModel = buildUserModel(userTbl, fullname, strtokenresult);
                    //populateUserAccess(userModel, userTbl);

                    //_context.TblUsersModels.Add(userModel);

                    //await _context.SaveChangesAsync();

                    const string chars = "0123456789";
                    Random random_OTP = new Random();
                    string otp_res = "";
                    char[] randomArray = new char[8];
                    for (int i = 0; i < 6; i++)
                    {
                        otp_res += chars[random.Next(chars.Length)];
                    }
                    TblRegistrationOtpmodel items = new TblRegistrationOtpmodel();
                    items.Email = userTbl.Email;
                    items.Otp = otp_res.ToString();

                    MailSender email = new MailSender(_emailsettings);
                    email.sendOtpMail(items);

                    string OTPInsert = $@"insert into tbl_RegistrationOTPModel (email,OTP,status) values ('" + userTbl.Email + "','" + otp_res + "','4')";
                    db.DB_WithParam(OTPInsert);

                    Stats = "Ok";
                    Mess = "User is for Verification, OTP Already Send!";
                    JWT = string.Concat(strtokenresult.TakeLast(15));
                    _IsFarmer = userTbl.isFarmer;
                    _Center = userTbl.CenterId;
                    
                    
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
                JwtToken = JWT,
                isFarmer = _IsFarmer,
                Center = _Center
            };
            dbmet.InsertAuditTrail("Register " + Stats + " " + Mess, DateTime.Now.ToString("yyyy-MM-dd"), "User Module", userTbl.Username, "0");
            return Ok(result);
        }

        public static string buildUserModel(RegistrationModel registrationModel, string fullName, string strtokenresult)
        {
            DbManager db = new DbManager();
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
                               ,[Delete_Flag]
                               ,[Created_By]
                               ,[CenterId]
                               ,[AgreementStatus]
                               ,[isFarmer]
                               ,[UserType]
                               ,[HerdId])
                                 VALUES
                                       ('" + registrationModel.Username + "'," +
                                       "'" + Cryptography.Encrypt(registrationModel.Password) + "'," +
                                       "'" + fullName + "'," +
                                       "'" + registrationModel.Fname + "'," +
                                       "'" + registrationModel.Lname + "'," +
                                       "'" + registrationModel.Mname + "'," +
                                       "'" + registrationModel.Email + "'," +
                                       "'" + registrationModel.Gender + "'," +
                                       "'" + registrationModel.EmployeeId + "'," +
                                       "'" + string.Concat(strtokenresult.TakeLast(15)) + "'," +
                                       "'" + registrationModel.FilePath + "'," +
                                       "'1'," +
                                       "'" + registrationModel.Cno + "'," +
                                       "'" + registrationModel.Address + "'," +
                                       "'" + registrationModel.Status + "'," +
                                       "'" + DateTime.Now.ToString("yyyy-MM-dd") + "'," +
                                       "'0'," +
                                       "'" + registrationModel.CreatedBy + "'," +
                                        "'" + registrationModel.CenterId + "'," +
                                       "'" + registrationModel.AgreementStatus + "'," +
                                       "'" + registrationModel.isFarmer + "'," +
                                       "'" + registrationModel.UserType + "'," +
                                       "'" + registrationModel.HerdId + "' )";
            string test = db.DB_WithParam(user_insert);

            return test;
        }

        
        // POST: user/rememberPassword
        [HttpPost]
        public async Task<IActionResult> rememberPassword(String token)
        {
            if (_context.TblUsersModels == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TblUsersModels'  is null!");
            }

            var userModel = await _context.TblUsersModels.Where(userModel => userModel.RememberToken == token).FirstAsync();
            if (userModel == null)
            {
                return NotFound("Token: " + token + " not found");
            }
            return Ok(userModel?.Jwtoken);
        }

        [HttpPost]
        public async Task<IActionResult> resetPassword(ResetPasswordModel resetPasswordModel)
        {
            if (_context.TblUsersModels == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TblUsersModels'  is null!");
            }

            var userModel = await _context.TblUsersModels.Where(user => user.Email == resetPasswordModel.Email).FirstAsync();
            if (userModel == null)
            {
                return Conflict("User not Found !!");
            }

            userModel.Password = Cryptography.Encrypt(resetPasswordModel.Password);
            _context.Entry(userModel).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            dbmet.InsertAuditTrail("Reset Password ", DateTime.Now.ToString("yyyy-MM-dd"), "User Module", userModel.Username, "0");
            return Ok("Password Reset successful for user: " + resetPasswordModel.Email);
        }

        [HttpPost]
        public async Task<IActionResult> forgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            if (_context.TblUsersModels == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TblUsersModels'  is null!");
            }

            var isEmailExists = _context.TblUsersModels.Any(user => !user.DeleteFlag != false && user.Email == forgotPasswordModel.Email);

            if (!isEmailExists)
            {
                return BadRequest("Email does not exists!!");
            }

            try
            {
                DateTime dateCreated = DateTime.Now;
                DateTime expirydate = dateCreated.AddDays(1);
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(forgotPasswordModel.Email);
                string emailBase64 = System.Convert.ToBase64String(plainTextBytes);

                var tokenModel = new TblTokenModel
                {
                    Token = emailBase64,
                    ExpiryDate = expirydate,
                    Status = "5",
                    DateCreated = dateCreated

                };

                _context.TblTokenModels.Add(tokenModel);
                _context.SaveChanges();

                MailSender _mailSender = new MailSender(_emailsettings);
                _mailSender.sendForgotPasswordMail(forgotPasswordModel.Email, forgotPasswordModel.ForgotPasswordLink + "?token=" + emailBase64);
                dbmet.InsertAuditTrail("Reset Password ", DateTime.Now.ToString("yyyy-MM-dd"), "User Module", _context.TblUsersModels.Where(a => a.Email == forgotPasswordModel.Email).FirstOrDefault().Username, "0");
                return Ok("Password Reset Email sent successfully!");
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpPost]
        public async Task<IActionResult> resendForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            if (_context.TblUsersModels == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TblUsersModels' is null!");
            }

            var isEmailExists = _context.TblUsersModels.Any(user => user.Email == forgotPasswordModel.Email);
            //var isEmailExists = _context.TblUsersModels.Any(user => !user.DeleteFlag && user.Email == email);

            if (!isEmailExists)
            {
                return BadRequest("Email does not exists!!");
            }

            try
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(forgotPasswordModel.Email);
                string emailBase64 = System.Convert.ToBase64String(plainTextBytes);

                MailSender _mailSender = new MailSender(_emailsettings);
                _mailSender.sendForgotPasswordMail(forgotPasswordModel.Email, forgotPasswordModel.ForgotPasswordLink + "?token=" + emailBase64);
                dbmet.InsertAuditTrail("Resend Password ", DateTime.Now.ToString("yyyy-MM-dd"), "User Module", _context.TblUsersModels.Where(a => a.Email == forgotPasswordModel.Email).FirstOrDefault().Username, "0");

                return Ok("Password Reset Email resent successfully!");
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpGet("{token}")]
        public async Task<IActionResult> CheckResetPasswordLinkExpiry(String token)
        {
            if (_context.TblUsersModels == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TblUsersModels' is null!");
            }

            var tokenModel = await _context.TblTokenModels.Where(tokenModel => tokenModel.Token == token).OrderByDescending(tokenModel => tokenModel.Id).FirstOrDefaultAsync();

            if (tokenModel == null)
            {
                return Conflict("Token does not exists !!");
            }

            return Ok(tokenModel.ExpiryDate.ToString("yyyy-MM-dd hh:mm:ss tt"));
        }

        private bool UserTblExists(int id)
        {
            return (_context.TblUsersModels?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
