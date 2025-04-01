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
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq.Dynamic.Core;
using System.Text.Json;
using static AngouriMath.Entity.Number;
using static API_PCC.Controllers.UserManagementController;

namespace API_PCC.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class BuffAnimalsController : ControllerBase
    {
        private readonly PCC_DEVContext _context;
        private readonly BloodCalculator _bloodCalculator;
        DBMethods dbmet = new DBMethods();
        DbManager db = new DbManager();
        public BuffAnimalsController(PCC_DEVContext context)
        {
            _context = context;
            _bloodCalculator = new BloodCalculator(context);
        }
        public class animalresult
        {
            //SIRE
            public string Id { get; set; }
            public string Animal_ID_Number { get; set; }
            public string Animal_Name { get; set; }
            public string Photo { get; set; }
            public string Herd_Code { get; set; }
            public string Farmer_Id { get; set; }
            public string Group_Id { get; set; }
            public string RFID_Number { get; set; }
            public string Date_of_Birth { get; set; }
            public string Sex { get; set; }
            public string Birth_Type { get; set; }
            public string Country_Of_Birth { get; set; }
            public string Origin_Of_Acquisition { get; set; }
            public string Date_Of_Acquisition { get; set; }
            public string Marking { get; set; }
            public string Type_Of_Ownership { get; set; }
            public string Delete_Flag { get; set; }
            public string StatusName { get; set; }
            public string Status { get; set; }
            public string Created_By { get; set; }
            public string Created_Date { get; set; }
            public string Updated_By { get; set; }
            public string Update_Date { get; set; }
            public string Date_Deleted { get; set; }
            public string Deleted_By { get; set; }
            public string Date_Restored { get; set; }
            public string Restored_By { get; set; }
            public string BreedRegistryNumber { get; set; }
            public string Breed_Code { get; set; }
            public string Blood_Code { get; set; }
        }

        public class animalsearchfilter
        {
            public string searchParam { get; set; }
            public string Sex { get; set; }
            public int? centerid { get; set; }
            public string? userid { get; set; }
            public int page { get; set; }
            public int pageSize { get; set; }
        }
        [HttpPost]
        public async Task<ActionResult<IEnumerable<BuffAnimalPagedModel>>> animalsearch(animalsearchfilter searchFilter)
        {
            //get specific assigned herd in center or farmer <>

            if (_context.ABuffAnimals == null)
            {
                return Problem("Entity set 'PCC_DEVContext.ABuffAnimals' is null!");
            }

            int pagesize = searchFilter.pageSize == 0 ? 10 : searchFilter.pageSize;
            int page = searchFilter.page == 0 ? 1 : searchFilter.page;
            var items = (dynamic)null;
            int totalItems = 0;
            int totalPages = 0;
         
          var  buffanimal = dbmet.getanimallist(searchFilter.centerid, searchFilter.userid).ToList();

          

           
            try
            {

                //if (searchFilter.searchParam != null && searchFilter.searchParam != "")
                if (searchFilter.Sex != null && searchFilter.Sex != "" && searchFilter.searchParam != null && searchFilter.searchParam != "")
                {
                    buffanimal = buffanimal.Where(a =>
                 a.Sex.ToUpper() == searchFilter.Sex.ToUpper()
                 && a.Animal_ID_Number.ToUpper().Contains(searchFilter.searchParam.ToUpper())
                 || a.Animal_Name.ToUpper().Contains(searchFilter.searchParam.ToUpper())
                 || a.BreedRegistryNumber.ToUpper().Contains(searchFilter.searchParam.ToUpper())
                 || a.RFID_Number.ToUpper().Contains(searchFilter.searchParam)).ToList();
                }
                else if (searchFilter.Sex != null && searchFilter.Sex != "")
                {
                    buffanimal = buffanimal.Where(a => a.Sex.ToUpper() == searchFilter.Sex.ToUpper() || a.Sex == searchFilter.Sex).ToList();
                }
                else if (searchFilter.searchParam != null && searchFilter.searchParam != "")
                {
                    buffanimal = buffanimal.Where(a =>
                             a.Animal_ID_Number.ToUpper().Contains(searchFilter.searchParam.ToUpper())
                 || a.Animal_Name.ToUpper().Contains(searchFilter.searchParam.ToUpper())
                 || a.BreedRegistryNumber.ToUpper().Contains(searchFilter.searchParam.ToUpper())
                 || a.RFID_Number.ToUpper().Contains(searchFilter.searchParam)).ToList();


                }

                totalItems = buffanimal.ToList().Count();
                totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
                items = buffanimal.ToList().Skip((page - 1) * pagesize).Take(pagesize).ToList();
                //var buffAnimal = convertDataRowListToBuffAnimalResponseModelList(items);
                var result = new List<buffanimalpagemodel>();
                var item = new buffanimalpagemodel();

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
                item.items = buffanimal;
                result.Add(item);
                return Ok(result);
            }

            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }
        // POST: BuffAnimals/list
        [HttpPost]
        public async Task<ActionResult<IEnumerable<BuffAnimalPagedModel>>> list(BuffAnimalSearchFilterModel searchFilter)
        {

            try
            {
                List<ABuffAnimal> buffAnimalList = await buildAnimalSearchQuery(searchFilter).ToListAsync();
                var result = buildBuffAnimalPagedModel(searchFilter, buffAnimalList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }
        [HttpPost]
        public async Task<ActionResult<IEnumerable<BuffAnimalPagedModel>>> archievelist(BuffAnimalSearchFilterModel searchFilter)
        {

            try
            {
                List<ABuffAnimal> buffAnimalList = await buildAnimalarchieveSearchQuery(searchFilter).ToListAsync();
                var result = buildBuffAnimalPagedModel(searchFilter, buffAnimalList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HerdPagedModel>>> Export()
        {
            var result = (from a in _context.ABuffAnimals
                          join b in _context.ABreeds on a.BreedCode equals b.BreedCode into breed
                          from b in breed.DefaultIfEmpty()
                          join c in _context.ABirthTypes on a.BirthType equals c.BirthTypeCode into birth
                          from c in birth.DefaultIfEmpty()
                          join d in _context.ATypeOwnerships on a.TypeOfOwnership equals d.TypeOwnCode into typeOfOwn
                          from d in typeOfOwn.DefaultIfEmpty()
                          join e in _context.ABloodComps on a.BloodCode.ToString() equals e.BloodCode into blood
                          from e in blood.DefaultIfEmpty()
                          join f in _context.OriginOfAcquisitionModels on a.OriginOfAcquisition equals f.Id into origin
                          from f in origin.DefaultIfEmpty()
                          join g in _context.family on a.Id equals g.animalId into family
                          from g in family.DefaultIfEmpty()
                          join sire in _context.ABuffAnimals on g.sireId equals sire.Id into sireJoin
                          from sire in sireJoin.DefaultIfEmpty()
                          join dam in _context.ABuffAnimals on g.damId equals dam.Id into damJoin
                          from dam in damJoin.DefaultIfEmpty()

                          select new
                          {
                              a.AnimalIdNumber,
                              a.AnimalName,
                              a.Photo,
                              a.HerdCode,
                              a.RfidNumber,
                              a.DateOfBirth,
                              a.Sex,
                              a.BreedCode,
                              BreedDesc = b == null ? null : b.BreedDesc, // breed code desc
                              a.BloodResult,
                              a.BirthType,
                              BirthTypeDesc = c == null ? null : c.BirthTypeDesc, // birth type desc
                              a.CountryOfBirth,
                              a.OriginOfAcquisition,
                              a.DateOfAcquisition,
                              a.Marking,
                              a.TypeOfOwnership,
                              TypeOwnDesc = d == null ? null : d.TypeOwnDesc, // type of own desc
                              a.BloodCode,
                              BloodDesc = e == null ? null : e.BloodDesc, // blood code desc
                              Region = f == null ? null : f.Region, // region
                              Province = f == null ? null : f.Province, // province
                              City = f == null ? null : f.City, // municipality
                              Barangay = f == null ? null : f.Barangay, // barangay
                              SireBreedRegistry = sire == null ? null : sire.breedRegistryNumber, // sire breed registry
                              DamBreedRegistry = dam == null ? null : dam.breedRegistryNumber, // dam breed registry
                              a.DeleteFlag,
                              a.Status,
                              a.CreatedBy,
                              a.CreatedDate,
                              a.UpdatedBy,
                              a.UpdateDate,
                              a.DateDeleted,
                              a.DeletedBy,
                              a.DateRestored,
                              a.RestoredBy,
                              a.breedRegistryNumber
                          }).ToList();

            return Ok(result);
        }
        private IQueryable<ABuffAnimal> buildAnimalSearchQuery(BuffAnimalSearchFilterModel searchFilter)
        {
            IQueryable<ABuffAnimal> query = _context.ABuffAnimals;
            IQueryable<HBuffHerd> queryh = _context.HBuffHerds;
            IQueryable<TblCenterModel> queryc = _context.TblCenterModels;
            IQueryable<TblFarmers> queryfo = _context.Tbl_Farmers;
            IQueryable<TblUsersModel> queryu = _context.TblUsersModels;

            query = query.Where(animal => !animal.DeleteFlag);
            // assuming that you return all records when nothing is specified in the filter

            if (searchFilter.centerid != 0)
                query = query.Join(queryh,
                                  animal => animal.HerdCode,
                                  herd => herd.HerdCode,
                                  (animal, herd) => new { animal, herd })
                             .Join(_context.TblCenterModels,
                                   combined => combined.herd.Center,
                                   center => center.Id,
                                   (combined, center) => new { combined.animal, center })
                             .Where(result => result.center.Id== searchFilter.centerid)
                             .Select(result => result.animal);

            //if (!searchFilter.userid.IsNullOrEmpty())
            //    query = query.Join(queryh,
            //                       animal => animal.HerdCode,
            //                       herd => herd.HerdCode,
            //                       (animal, herd) => new { animal, herd })
            //                 .Join(queryfo,
            //                       combined => combined.herd.Owner,
            //                       owner => owner.Id,
            //                       (combined, farmOwner) => new { combined.animal, farmOwner }) 
            //                 .Join(queryu,
            //                       combined => combined.farmOwner.Id, 
            //                       ownerUser => ownerUser.Id,
            //                       (combined, farmOwnerUser) => new { combined.animal, combined.farmOwner, farmOwnerUser }) 
            //                 .Where(result => result.farmOwner.FirstName == result.farmOwnerUser.Fname &&
            //                                  result.farmOwner.LastName == result.farmOwnerUser.Lname &&
            //                                  result.farmOwner.Address == result.farmOwnerUser.Address &&
            //                                  result.farmOwnerUser.isFarmer == true &&
            //                                  result.farmOwnerUser.Id.ToString().Contains(searchFilter.userid))
            //.Select(result => result.animal);

            if (!string.IsNullOrEmpty(searchFilter.herdcode))
                query = query.Where(animal => animal.HerdCode.Equals(searchFilter.herdcode));

            if (!string.IsNullOrEmpty(searchFilter.userid))
                query = query.Where(animal => animal.FarmerId == int.Parse(searchFilter.userid)); 
            
            if (searchFilter.groupid.HasValue && searchFilter.groupid != 0)
                query = query.Where(animal => animal.GroupId.Equals(searchFilter.groupid));

            if (!searchFilter.searchValue.IsNullOrEmpty())
                query = query.Where(animal =>
                               animal.AnimalIdNumber.Contains(searchFilter.searchValue) ||
                               animal.AnimalName.Contains(searchFilter.searchValue) ||
                               animal.HerdCode.Contains(searchFilter.searchValue));

            if (!searchFilter.filterBy.BloodCode.IsNullOrEmpty())
                query = query.Where(animal => animal.BloodCode.Equals(searchFilter.filterBy.BloodCode));

            if (!searchFilter.filterBy.BreedCode.IsNullOrEmpty())
                query = query.Where(animal => animal.BreedCode.Equals(searchFilter.filterBy.BreedCode));

            if (!searchFilter.filterBy.TypeOfOwnership.IsNullOrEmpty())
                query = query.Where(animal => animal.TypeOfOwnership.Equals(searchFilter.filterBy.TypeOfOwnership));

            if (!searchFilter.sex.IsNullOrEmpty())
                query = query.Where(animal => animal.Sex.Equals(searchFilter.sex));

            if (!searchFilter.status.IsNullOrEmpty())
                query = query.Where(animal => animal.Status.Equals(searchFilter.status));


            if (!searchFilter.sortBy.Field.IsNullOrEmpty())
            {

                if (!searchFilter.sortBy.Sort.IsNullOrEmpty())
                {
                    query = query.OrderBy(searchFilter.sortBy.Field + " " + searchFilter.sortBy.Sort);
                }
                else
                {
                    query = query.OrderBy(searchFilter.sortBy.Field + " asc");

                }
            }
            else
            {
                query = query.OrderByDescending(animal => animal.Id);
            }

            return query;
        }
        private IQueryable<ABuffAnimal> buildAnimalarchieveSearchQuery(BuffAnimalSearchFilterModel searchFilter)
        {
            IQueryable<ABuffAnimal> query = _context.ABuffAnimals;

            query = query.Where(animal => animal.DeleteFlag);
            // assuming that you return all records when nothing is specified in the filter

            if (!searchFilter.searchValue.IsNullOrEmpty())
                query = query.Where(animal =>
                               animal.AnimalIdNumber.Contains(searchFilter.searchValue) ||
                               animal.AnimalName.Contains(searchFilter.searchValue));

            if (searchFilter.centerid != 0) {
                query = query.Where(animal =>
                               animal.HerdCode.Equals("test01"));
            }

            if (!searchFilter.filterBy.BloodCode.IsNullOrEmpty())
                query = query.Where(animal => animal.BloodCode.Equals(searchFilter.filterBy.BloodCode));

            if (!searchFilter.filterBy.BreedCode.IsNullOrEmpty())
                query = query.Where(animal => animal.BreedCode.Equals(searchFilter.filterBy.BreedCode));

            if (!searchFilter.filterBy.TypeOfOwnership.IsNullOrEmpty())
                query = query.Where(animal => animal.TypeOfOwnership.Equals(searchFilter.filterBy.TypeOfOwnership));

            if (!searchFilter.sex.IsNullOrEmpty())
                query = query.Where(animal => animal.Sex.Equals(searchFilter.sex));

            if (!searchFilter.status.IsNullOrEmpty())
                query = query.Where(animal => animal.Status.Equals(searchFilter.status));


            if (!searchFilter.sortBy.Field.IsNullOrEmpty())
            {

                if (!searchFilter.sortBy.Sort.IsNullOrEmpty())
                {
                    query = query.OrderBy(searchFilter.sortBy.Field + " " + searchFilter.sortBy.Sort);
                }
                else
                {
                    query = query.OrderBy(searchFilter.sortBy.Field + " asc");

                }
            }
            else
            {
                query = query.OrderByDescending(animal => animal.Id);
            }

            return query;
        }
        // GET: BuffAnimals/search/5
        // search by registrationNumber and RFID number
        [HttpGet("{referenceNumber}")]
        public async Task<ActionResult<BuffAnimalBaseModel>> search(String referenceNumber)
        {
            string filePath = @"C:\data\buffanimalsearch.json"; // Replace with your desired file path

            string refno = Uri.UnescapeDataString(referenceNumber);
            dbmet.insertlgos(filePath, JsonSerializer.Serialize(referenceNumber));
            var buffAnimal = _context.ABuffAnimals.Where(animal =>
                        !animal.DeleteFlag && (animal.FarmerId.Equals(refno) || animal.FarmerId.Equals(int.Parse(refno))))
                        .FirstOrDefault();
            if (buffAnimal == null)
            {
                return Conflict("No records found!");
            }

            var animalModel = convertBuffAnimalToResponseModel(buffAnimal);

            return Ok(animalModel);
        }

        // GET: BuffAnimals/view
        // view all
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BuffAnimalListResponseModel>>> view()
        {
            try
            {
                var buffAnimalList = _context.ABuffAnimals.Where(animal => !animal.DeleteFlag).AsEnumerable().ToList();

                if (buffAnimalList.Count == 0)
                {
                    return Conflict("No records found!");
                }

                var animalModelResponseList = convertBuffAnimalListToResponseModel(buffAnimalList);

                return Ok(animalModelResponseList);

            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // PUT: BuffAnimals/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, BuffAnimalUpdateModel updateModel)
        {

            string filePath = @"C:\data\updatebuffanimal.json"; // Replace with your desired file path



            dbmet.insertlgos(filePath, JsonSerializer.Serialize(updateModel));
            dbmet.InsertAuditTrail("Updated BuffAnimal ID: " + id + "", DateTime.Now.ToString("yyyy-MM-dd"), "Animal Module", updateModel.UpdatedBy, "0");

            int sireid = 0;
            int damid = 0;

            if (_context.ABuffAnimals == null)
            {
                return Problem("Buff Animal entity Set is null!");
            }

            var buffAnimal = _context.ABuffAnimals
                                        .Where(buffAnimal => !buffAnimal.DeleteFlag &&
                                                buffAnimal.Id.Equals(id))
                                        .FirstOrDefault();

            if (buffAnimal == null)
            {
                return Conflict("No records matched!");
            }

            var buffAnimalDuplicateCheck = _context.ABuffAnimals
                                        .Where(buffAnimal => !buffAnimal.DeleteFlag &&
                                                !buffAnimal.Id.Equals(id) &&
                                                buffAnimal.AnimalIdNumber.Equals(updateModel.AnimalIdNumber) &&
                                                buffAnimal.AnimalName.Equals(updateModel.AnimalName) &&
                                                buffAnimal.HerdCode.Equals(updateModel.HerdCode) &&
                                                 buffAnimal.BreedCode.Equals(updateModel.BreedCode) &&
                                                 buffAnimal.Sex.Equals(updateModel.Sex) &&
                                                 buffAnimal.OriginOfAcquisition.Equals(updateModel.OriginOfAcquisition) &&
                                                 buffAnimal.BloodCode.Equals(updateModel.BloodCode))
                                        .FirstOrDefault();

            // check for duplication
            if (buffAnimalDuplicateCheck != null)
            {
                return Conflict("Entity already exists");
            }

            var familyRecords = _context.family.AsEnumerable();

            var sire = _context.ABuffAnimals
                               .Where(animal => !animal.DeleteFlag &&
                                                animal.breedRegistryNumber.Equals(updateModel.Sire.RegistrationNumber));
            var sireRecord = (dynamic)null;
            if (sire.IsNullOrEmpty() == false)
            {
                //    return Conflict("Sire does not exists");
                //}
                //else
                //{

                sireRecord = sire.Join(familyRecords, sire => sire.Id, family => family.animalId, (animal, family) => new { animal = animal })
           .FirstOrDefault()
           .animal;
                sireid = sireRecord.Id;
                populateAnimal(sireRecord, updateModel.Sire);
                _context.Entry(sireRecord).State = EntityState.Modified;
            }


            var dam = _context.ABuffAnimals
                               .Where(animal => !animal.DeleteFlag &&
                                                animal.breedRegistryNumber.Equals(updateModel.Dam.RegistrationNumber));
            var damRecord = (dynamic)null;
            if (dam.IsNullOrEmpty() == false)
            {
                //    return Conflict("Dam does not exists");
                //}
                //else
                //{
                damRecord = dam.Join(familyRecords, dam => dam.Id, family => family.animalId, (animal, family) => new { animal = animal })
                              .FirstOrDefault()
                              .animal;
                damid = damRecord.Id;
                populateAnimal(damRecord, updateModel.Dam);
                _context.Entry(damRecord).State = EntityState.Modified;
            }


            TblOriginOfAcquisitionModel? originOfAcquisition = null;

            if (!isOriginOfAcquisitionEmpty(updateModel.OriginOfAcquisition))
            {
                originOfAcquisition = _context.OriginOfAcquisitionModels
                                        .Where(originOfAcquisition =>
                                                originOfAcquisition.City.Equals(updateModel.OriginOfAcquisition.City))
                                        .FirstOrDefault();

                if (originOfAcquisition == null)
                {
                    //return Conflict("Origin of Acquisition does not exists");
                    string Insert = $@"
                    INSERT INTO [dbo].[tbl_OriginOfAcquisitionModel]
                               ([City]
                               ,[Province]
                               ,[Barangay]
                               ,[Region])
                         VALUES
                           ('" + updateModel.OriginOfAcquisition.City + "'," +
                           "'" + updateModel.OriginOfAcquisition.Province + "'," +
                           "'" + updateModel.OriginOfAcquisition.Barangay + "'," +
                            "'" + updateModel.OriginOfAcquisition.Region + "') ";
                    db.DB_WithParam(Insert);
                }
                else
                {
                    populateOriginOfAcquistion(originOfAcquisition, updateModel.OriginOfAcquisition);
                    _context.Entry(originOfAcquisition).State = EntityState.Modified;
                }

            }

            await _context.SaveChangesAsync();

            try
            {
                buffAnimal = populateBuffAnimal(buffAnimal, updateModel);
                if (originOfAcquisition != null)
                {
                    buffAnimal.OriginOfAcquisition = originOfAcquisition.Id;
                }
                buffAnimal.UpdateDate = DateTime.Now;
                buffAnimal.UpdatedBy = updateModel.UpdatedBy;
                if (sireRecord != null)
                {
                    var bloodCalculatorModel = new BloodCalculatorModel()
                    {
                        sireBreedRegistryNumber = sireRecord.breedRegistryNumber,
                        damBreedRegistryNumber = damRecord.breedRegistryNumber
                    };
                    var bloodCompDetails = _bloodCalculator.compute(bloodCalculatorModel);



                    if (bloodCompDetails != null)
                    {
                        buffAnimal.BloodCode = bloodCompDetails.Id;
                        buffAnimal.BloodResult = decimal.Parse(bloodCompDetails.BloodResult.ToString());
                    }
                }


                _context.Entry(buffAnimal).State = EntityState.Modified;
                await _context.SaveChangesAsync();


                // Update family record using animal ID
                //var family = _context.family.Where(a => a.animalId == buffAnimal.Id).FirstOrDefault();
                string sql1 = $@"SELECT *
                    FROM [dbo].[A_Family] where AnimalId='" + buffAnimal.Id + "'";
                var result1 = new List<UserTypeAction_Model>();
                DataTable family = db.SelectDb(sql1).Tables[0];

                if (family.Rows.Count == 0)
                {
                    var family1 = new A_Family()
                    {
                        sireId = sireid,
                        damId = damid,
                        animalId = buffAnimal.Id,
                        status = 1,
                        deleteFlag = false,
                        CreatedBy = buffAnimal.CreatedBy,
                        CreatedDate = DateTime.Now
                    };

                    _context.family.Add(family1);

                    await _context.SaveChangesAsync();
                }
                else
                {
                    string Insert = $@"UPDATE [dbo].[A_Family]
                               SET [SireId] = '" + sireid + "', " +
                               "[AnimalId] = '" + buffAnimal.Id + "', " +
                               "[DamId] = '" + damid + "', " +
                               "[Status] ='1', " +
                               "[Delete_Flag] ='false', " +
                               "[Updated_By] ='" + updateModel.UpdatedBy + "', " +
                               "[Update_Date] = '" + DateTime.Now.ToString("yyyy-MM-dd") + "'" +
                                 "where AnimalId='" + id + "'";
                    db.DB_WithParam(Insert);
                }

                //await _context.SaveChangesAsync();

                return Ok("Update Successful!");
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // POST: BuffAnimals/save
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ABuffAnimal>> save(BuffAnimalRegistrationModel buffAnimalRegistrationModel)
        {

            string filePath = @"C:\data\savebuffanimal.json"; // Replace with your desired file path
            dbmet.insertlgos(filePath, JsonSerializer.Serialize(buffAnimalRegistrationModel));

            if (_context.ABuffAnimals == null)
            {
                return Problem("Buff Animal entity Set is null!");
            }

            try
            {
                var duplicateRecordCheck = _context.ABuffAnimals
                                            .Where(buffAnimal => !buffAnimal.DeleteFlag &&
                                                   buffAnimal.HerdCode.Equals(buffAnimalRegistrationModel.HerdCode) &&
                                                   buffAnimal.AnimalIdNumber.Equals(buffAnimalRegistrationModel.AnimalIdNumber))
                                            .FirstOrDefault();

                if (duplicateRecordCheck != null)
                {
                    return Conflict("Buff Animal already exists");
                }

                var buffAnimal = buildBuffAnimal(buffAnimalRegistrationModel);
                ABuffAnimal? sireRecord = null;
                ABuffAnimal? damRecord = null;
                TblOriginOfAcquisitionModel? originOfAcquisitionRecord = null;

                // Required Fields to generate breed registry number
                // Animal_ID_Number, Sex, Breed_Code
                if (!isRequiredFieldEmpty(buffAnimalRegistrationModel.Sire))
                {
                    sireRecord = animalRecordCheck(buffAnimalRegistrationModel.Sire);

                    if (sireRecord == null)
                    {
                        var sire = buildBuffAnimal(buffAnimalRegistrationModel.Sire);
                        var sireModel = _context.ABuffAnimals.Add(sire);
                        sireRecord = sireModel.Entity;
                    }
                }

                if (!isRequiredFieldEmpty(buffAnimalRegistrationModel.Dam))
                {
                    damRecord = animalRecordCheck(buffAnimalRegistrationModel.Dam);

                    if (damRecord == null)
                    {
                        var dam = buildBuffAnimal(buffAnimalRegistrationModel.Dam);
                        var damModel = _context.ABuffAnimals.Add(dam);
                        damRecord = damModel.Entity;
                    }
                }

                if (!isOriginOfAcquisitionEmpty(buffAnimalRegistrationModel.OriginOfAcquisition))
                {
                    originOfAcquisitionRecord = originOfAcquistionRecordCheck(buffAnimalRegistrationModel.OriginOfAcquisition);

                    if (originOfAcquisitionRecord == null)
                    {
                        var originOfAcquistion = buildOriginOfAcquistion(buffAnimalRegistrationModel.OriginOfAcquisition);

                        var originOfAcquistionModel = _context.OriginOfAcquisitionModels.Add(originOfAcquistion);
                        originOfAcquisitionRecord = originOfAcquistionModel.Entity;
                    }
                }


                await _context.SaveChangesAsync();


                if (originOfAcquisitionRecord != null)
                {
                    buffAnimal.OriginOfAcquisition = originOfAcquisitionRecord.Id;
                }

                buffAnimal.CreatedBy = buffAnimalRegistrationModel.CreatedBy;
                buffAnimal.CreatedDate = DateTime.Now;
                buffAnimal.Status = "1";

                if (sireRecord != null && damRecord != null)
                {
                    var bloodCalculatorModel = new BloodCalculatorModel()
                    {
                        sireBreedRegistryNumber = sireRecord.breedRegistryNumber,
                        damBreedRegistryNumber = damRecord.breedRegistryNumber
                    };
                    var bloodCompDetails = _bloodCalculator.compute(bloodCalculatorModel);

                    if (bloodCompDetails != null)
                    {
                        buffAnimal.BloodCode = bloodCompDetails.Id;
                        buffAnimal.BloodResult = decimal.Parse(bloodCompDetails.BloodResult.ToString());
                    }
                }

                var savedEntity = _context.ABuffAnimals.Add(buffAnimal).Entity;

                await _context.SaveChangesAsync();

                var family = new A_Family()
                {
                    sire = sireRecord,
                    dam = damRecord,
                    animalId = buffAnimal.Id,
                    status = 1,
                    deleteFlag = false,
                };

                _context.family.Add(family);

                await _context.SaveChangesAsync();

                ABuffAnimal? savedBuffAnimal = _context.ABuffAnimals
                                            .Where(animal => animal.Id.Equals(savedEntity.Id))
                                            .FirstOrDefault();
                var entry = _context.Entry(family);
                var lastInsertedId = entry.Property(e => e.Id).CurrentValue;
                dbmet.InsertAuditTrail("Save New BuffAnimal ID: " + lastInsertedId + "", DateTime.Now.ToString("yyyy-MM-dd"), "Animal Module", buffAnimal.CreatedBy, "0");
                return CreatedAtAction("save", new { id = savedBuffAnimal.Id }, savedBuffAnimal);
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.GetBaseException().ToString());
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }
        private bool isRequiredFieldEmpty(List<Animal> animal)
        {
            bool isRequiredFieldEmpty = false;
            if (animal[0].IdNumber.IsNullOrEmpty() &&
                animal[0].Sex.IsNullOrEmpty() &&
                animal[0].BreedCode.IsNullOrEmpty())
            {
                isRequiredFieldEmpty = !isRequiredFieldEmpty;
            }
            return isRequiredFieldEmpty;
        }

        private bool isOriginOfAcquisitionEmpty(List<OriginOfAcquisitionModel> originOfAcquisition)
        {
            bool isOriginOfAcquisitionEmpty = false;
            if (originOfAcquisition[0].City.IsNullOrEmpty() &&
                originOfAcquisition[0].Province.IsNullOrEmpty() &&
                originOfAcquisition[0].Barangay.IsNullOrEmpty() &&
                originOfAcquisition[0].Region.IsNullOrEmpty())
            {
                isOriginOfAcquisitionEmpty = !isOriginOfAcquisitionEmpty;
            }
            return isOriginOfAcquisitionEmpty;
        }


        private void populateAnimal(ABuffAnimal animal, Animal animalUpdateModel)
        {
            string sql1 = $@"SELECT Id,Blood_Code
                    FROM [dbo].[A_Blood_Comp] where Blood_Code='" + animalUpdateModel.BloodCode + "'";

            DataTable bloodid = db.SelectDb(sql1).Tables[0];

            string sql2 = $@"SELECT Id,Breed_Code
                    FROM [dbo].[A_Breed] where Breed_Code='" + animalUpdateModel.BreedCode + "'";

            DataTable breedid = db.SelectDb(sql2).Tables[0];

            int blood_id = bloodid.Rows.Count != 0 ? int.Parse(bloodid.Rows[0]["Id"].ToString()) : 0;
            int breed_id = breedid.Rows.Count != 0 ? int.Parse(breedid.Rows[0]["Id"].ToString()) : 0;
            animal.RfidNumber = animalUpdateModel.RegistrationNumber;
            animal.AnimalIdNumber = animalUpdateModel.IdNumber;
            animal.AnimalName = animalUpdateModel.Name;
            animal.BreedCode = breed_id.ToString();
            animal.BloodCode = blood_id;
        }
        [HttpPost]
        public async Task<ActionResult<ABuffAnimal>> import(List<BuffAnimalRegistrationModel> buffAnimalRegistrationModel)
        {

            //added list of imported animals
            List<ABuffAnimal> listOfImportedAnimal = new List<ABuffAnimal>();

            string filePath = @"C:\data\savebuffanimal.json"; // Replace with your desired file path
            try
            {
                for (int x = 0; x < buffAnimalRegistrationModel.Count; x++)
                {

                    //dbmet.insertlgos(filePath, JsonSerializer.Serialize(buffAnimalRegistrationModel[x]));



                    if (_context.ABuffAnimals == null)
                    {
                        return Problem("Buff Animal entity Set is null!");
                    }

                    try
                    {
                        var duplicateRecordCheck = _context.ABuffAnimals
                                                    .Where(buffAnimal => !buffAnimal.DeleteFlag &&
                                                           buffAnimal.HerdCode.Equals(buffAnimalRegistrationModel[x].HerdCode) &&
                                                           buffAnimal.AnimalIdNumber.Equals(buffAnimalRegistrationModel[x].AnimalIdNumber))
                                                    .FirstOrDefault();

                        if (duplicateRecordCheck != null)
                        {
                            return Conflict("Buff Animal already exists");
                        }

                        var buffAnimal = buildBuffAnimal(buffAnimalRegistrationModel[x]);
                        ABuffAnimal? sireRecord = null;
                        ABuffAnimal? damRecord = null;
                        TblOriginOfAcquisitionModel? originOfAcquisitionRecord = null;

                        // Required Fields to generate breed registry number
                        // Animal_ID_Number, Sex, Breed_Code
                        if (!isRequiredFieldEmpty(buffAnimalRegistrationModel[x].Sire))
                        {
                            sireRecord = animalRecordCheck(buffAnimalRegistrationModel[x].Sire);

                            if (sireRecord == null)
                            {
                                var sire = buildBuffAnimal(buffAnimalRegistrationModel[x].Sire);
                                var sireModel = _context.ABuffAnimals.Add(sire);
                                sireRecord = sireModel.Entity;
                            }
                        }

                        if (!isRequiredFieldEmpty(buffAnimalRegistrationModel[x].Dam))
                        {
                            damRecord = animalRecordCheck(buffAnimalRegistrationModel[x].Dam);

                            if (damRecord == null)
                            {
                                var dam = buildBuffAnimal(buffAnimalRegistrationModel[x].Dam);
                                var damModel = _context.ABuffAnimals.Add(dam);
                                damRecord = damModel.Entity;
                            }
                        }

                        if (!isOriginOfAcquisitionEmpty(buffAnimalRegistrationModel[x].OriginOfAcquisition))
                        {
                            originOfAcquisitionRecord = originOfAcquistionRecordCheck(buffAnimalRegistrationModel[x].OriginOfAcquisition);

                            if (originOfAcquisitionRecord == null)
                            {
                                var originOfAcquistion = buildOriginOfAcquistion(buffAnimalRegistrationModel[x].OriginOfAcquisition);

                                var originOfAcquistionModel = _context.OriginOfAcquisitionModels.Add(originOfAcquistion);
                                originOfAcquisitionRecord = originOfAcquistionModel.Entity;
                            }
                        }


                        await _context.SaveChangesAsync();


                        if (originOfAcquisitionRecord != null)
                        {
                            buffAnimal.OriginOfAcquisition = originOfAcquisitionRecord.Id;
                        }

                        buffAnimal.CreatedBy = buffAnimalRegistrationModel[x].CreatedBy;
                        buffAnimal.CreatedDate = DateTime.Now;
                        buffAnimal.Status = "1";

                        if (sireRecord != null && damRecord != null)
                        {
                            var bloodCalculatorModel = new BloodCalculatorModel()
                            {
                                sireBreedRegistryNumber = sireRecord.breedRegistryNumber,
                                damBreedRegistryNumber = damRecord.breedRegistryNumber
                            };
                            var bloodCompDetails = _bloodCalculator.compute(bloodCalculatorModel);

                            if (bloodCompDetails != null)
                            {
                                buffAnimal.BloodCode = bloodCompDetails.Id;
                                buffAnimal.BloodResult = decimal.Parse(bloodCompDetails.BloodResult.ToString());
                            }
                        }


                        var savedEntity = _context.ABuffAnimals.Add(buffAnimal).Entity;

                        await _context.SaveChangesAsync();

                        var family = new A_Family()
                        {
                            sire = sireRecord,
                            dam = damRecord,
                            animalId = buffAnimal.Id,
                            status = 1,
                            deleteFlag = false,
                        };

                        _context.family.Add(family);

                        await _context.SaveChangesAsync();

                        ABuffAnimal? savedBuffAnimal = _context.ABuffAnimals
                                                    .Where(animal => animal.Id.Equals(savedEntity.Id))
                                                    .FirstOrDefault();
                        var entry = _context.Entry(family);
                        var lastInsertedId = entry.Property(e => e.Id).CurrentValue;
                        dbmet.InsertAuditTrail("Save New BuffAnimal ID: " + lastInsertedId + "", DateTime.Now.ToString("yyyy-MM-dd"), "Animal Module", buffAnimal.CreatedBy, "0");

                        //added list of imported animals
                        listOfImportedAnimal.Add(savedEntity);

                    }
                    catch (BadHttpRequestException ex)
                    {
                        return BadRequest(ex.GetBaseException().ToString());
                    }
                    catch (Exception ex)
                    {
                        return Problem(ex.GetBaseException().ToString());
                    }
                }
                //added list of imported animals
                return CreatedAtAction("import", listOfImportedAnimal);
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.GetBaseException().ToString());
            }
        }
        private bool isRequiredFieldEmpty(Animal animal)
        {
            bool isRequiredFieldEmpty = false;
            if (animal.IdNumber.IsNullOrEmpty() &&
                animal.Sex.IsNullOrEmpty() &&
                animal.BreedCode.IsNullOrEmpty())
            {
                isRequiredFieldEmpty = !isRequiredFieldEmpty;
            }
            return isRequiredFieldEmpty;
        }

        private bool isOriginOfAcquisitionEmpty(OriginOfAcquisitionModel originOfAcquisition)
        {
            bool isOriginOfAcquisitionEmpty = false;
            if (originOfAcquisition.City.IsNullOrEmpty() &&
                originOfAcquisition.Province.IsNullOrEmpty() &&
                originOfAcquisition.Barangay.IsNullOrEmpty() &&
                originOfAcquisition.Region.IsNullOrEmpty())
            {
                isOriginOfAcquisitionEmpty = !isOriginOfAcquisitionEmpty;
            }
            return isOriginOfAcquisitionEmpty;
        }


        private void populateAnimalList(ABuffAnimal animal, Animal animalUpdateModel)
        {
            string sql1 = $@"SELECT Id,Blood_Code
                    FROM [dbo].[A_Blood_Comp] where Blood_Code='" + animalUpdateModel.BloodCode + "'";

            DataTable bloodid = db.SelectDb(sql1).Tables[0];

            string sql2 = $@"SELECT Id,Breed_Code
                    FROM [dbo].[A_Breed] where Breed_Code='" + animalUpdateModel.BreedCode + "'";

            DataTable breedid = db.SelectDb(sql2).Tables[0];

            int blood_id = bloodid.Rows.Count != 0 ? int.Parse(bloodid.Rows[0]["Id"].ToString()) : 0;
            int breed_id = breedid.Rows.Count != 0 ? int.Parse(breedid.Rows[0]["Id"].ToString()) : 0;
            animal.RfidNumber = animalUpdateModel.RegistrationNumber;
            animal.AnimalIdNumber = animalUpdateModel.IdNumber;
            animal.AnimalName = animalUpdateModel.Name;
            animal.BreedCode = breed_id.ToString();
            animal.BloodCode = blood_id;
        }
        private void populateOriginOfAcquistion(TblOriginOfAcquisitionModel originOfAcquisition, OriginOfAcquisitionModel originOfAcquisitionModel)
        {
            originOfAcquisition.City = originOfAcquisitionModel.City;
            originOfAcquisition.Province = originOfAcquisitionModel.Province;
            originOfAcquisition.Barangay = originOfAcquisitionModel.Barangay;
            originOfAcquisition.Region = originOfAcquisitionModel.Region;
        }

        private TblOriginOfAcquisitionModel buildOriginOfAcquistion(OriginOfAcquisitionModel originOfAcquisitionModel)
        {
            var originOfAcquistionModel = new TblOriginOfAcquisitionModel()
            {
                City = originOfAcquisitionModel.City,
                Province = originOfAcquisitionModel.Province,
                Barangay = originOfAcquisitionModel.Barangay,
                Region = originOfAcquisitionModel.Region
            };

            return originOfAcquistionModel;
        }


        private ABuffAnimal animalRecordCheck(Animal animal)
        {
            var animalRecord = _context.ABuffAnimals
                                        .Where(buffAnimal => buffAnimal.breedRegistryNumber.Equals(animal.RegistrationNumber) &&
                                                buffAnimal.AnimalIdNumber.Equals(animal.IdNumber) &&
                                                buffAnimal.AnimalName.Equals(animal.Name) &&
                                                buffAnimal.BreedCode.Equals(animal.BreedCode) &&
                                                buffAnimal.BloodCode.Equals(animal.BloodCode))
                                        .FirstOrDefault();
            return animalRecord;
        }

        private TblOriginOfAcquisitionModel originOfAcquistionRecordCheck(OriginOfAcquisitionModel originOfAcquisitionModel)
        {
            var originOfAcquisitionRecord = _context.OriginOfAcquisitionModels
                                        .Where(originOfAcquistion => originOfAcquistion.City.Equals(originOfAcquisitionModel.City) &&
                                                originOfAcquistion.Province.Equals(originOfAcquisitionModel.Province) &&
                                                originOfAcquistion.Barangay.Equals(originOfAcquisitionModel.Barangay) &&
                                                originOfAcquistion.Region.Equals(originOfAcquisitionModel.Region))
                                        .FirstOrDefault();
            return originOfAcquisitionRecord;
        }

        // POST: BuffAnimals/delete/5
        [HttpPost]
        public async Task<IActionResult> delete(DeletionModel deletionModel)
        {
            if (_context.ABuffAnimals == null)
            {
                return NotFound();
            }
            var aBuffAnimal = await _context.ABuffAnimals.FindAsync(deletionModel.id);
            if (aBuffAnimal == null || aBuffAnimal.DeleteFlag)
            {
                return Conflict("No records matched!");
            }

            try
            {
                aBuffAnimal.DeleteFlag = true;
                aBuffAnimal.DateDeleted = DateTime.Now;
                aBuffAnimal.DeletedBy = deletionModel.deletedBy;
                aBuffAnimal.DateRestored = null;
                aBuffAnimal.RestoredBy = "";
                _context.Entry(aBuffAnimal).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                dbmet.InsertAuditTrail("Deleted BuffAnimal ID: " + aBuffAnimal.Id + "", DateTime.Now.ToString("yyyy-MM-dd"), "Animal Module", aBuffAnimal.DeletedBy, "0");
                return Ok("Deletion Successful!");
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }
        [HttpPost]
        public async Task<IActionResult> ArchieveMultiple(List<DeletionModel> deletionModel)
        {
            string status = "";
            if (_context.ABuffAnimals == null)
            {
                return NotFound();
            }
            for(int x= 0; x<deletionModel.Count;x++)
            {

           
            var aBuffAnimal = await _context.ABuffAnimals.FindAsync(deletionModel[x].id);
            if (aBuffAnimal == null || aBuffAnimal.DeleteFlag)
            {
                return Conflict("No records matched!");
            }

            try
            {
                aBuffAnimal.DeleteFlag = true;
                aBuffAnimal.DateDeleted = DateTime.Now;
                aBuffAnimal.DeletedBy = deletionModel[x].deletedBy;
                aBuffAnimal.DateRestored = null;
                aBuffAnimal.RestoredBy = "";
                _context.Entry(aBuffAnimal).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                dbmet.InsertAuditTrail("Deleted BuffAnimal ID: " + aBuffAnimal.Id + "", DateTime.Now.ToString("yyyy-MM-dd"), "Animal Module", aBuffAnimal.DeletedBy, "0");
           
                    status = "Deletion Successful!";
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
            }
            return Ok(status);
        }

        // POST: BuffAnimals/restore/
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> restore(RestorationModel restorationModel)
        {

            if (_context.ABuffAnimals == null)
            {
                return Problem("Entity set 'PCC_DEVContext.BuffAnimal' is null!");
            }

            var aBuffAnimal = await _context.ABuffAnimals.FindAsync(restorationModel.id);
            if (aBuffAnimal == null || !aBuffAnimal.DeleteFlag)
            {
                return Conflict("No deleted records matched!");
            }

            try
            {
                aBuffAnimal.DeleteFlag = false;
                aBuffAnimal.DateDeleted = null;
                aBuffAnimal.DeletedBy = "";
                aBuffAnimal.DateRestored = DateTime.Now;
                aBuffAnimal.RestoredBy = restorationModel.restoredBy;

                _context.Entry(aBuffAnimal).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                dbmet.InsertAuditTrail("Restore BuffAnimal ID: " + aBuffAnimal.Id + "", DateTime.Now.ToString("yyyy-MM-dd"), "Animal Module", aBuffAnimal.RestoredBy, "0");
                return Ok("Restoration Successful!");
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpPost]
        public async Task<IActionResult> RestoreMultiple(List<RestorationModel> restorationModel)
        {
            string status = "";
            if (_context.ABuffAnimals == null)
            {
                return Problem("Entity set 'PCC_DEVContext.BuffAnimal' is null!");
            }
            for(int x= 0; x < restorationModel.Count; x++)
            {
                var aBuffAnimal = await _context.ABuffAnimals.FindAsync(restorationModel[x].id);
                if (aBuffAnimal == null || !aBuffAnimal.DeleteFlag)
                {
                    dbmet.InsertAuditTrail("Restore BuffAnimal ID: " + aBuffAnimal.Id + " : can't restore, record is not deleted", DateTime.Now.ToString("yyyy-MM-dd"), "Animal Module", aBuffAnimal.RestoredBy, "0");
                    //return Conflict("No deleted records matched!");
                }

                try
                {
                    aBuffAnimal.DeleteFlag = false;
                    aBuffAnimal.DateDeleted = null;
                    aBuffAnimal.DeletedBy = "";
                    aBuffAnimal.DateRestored = DateTime.Now;
                    aBuffAnimal.RestoredBy = restorationModel[x].restoredBy;

                    _context.Entry(aBuffAnimal).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    dbmet.InsertAuditTrail("Restore BuffAnimal ID: " + aBuffAnimal.Id + "", DateTime.Now.ToString("yyyy-MM-dd"), "Animal Module", aBuffAnimal.RestoredBy, "0");
                    status="Restoration Successful!";
                }
                catch (Exception ex)
                {

                    return Problem(ex.GetBaseException().ToString());
                }

            }
            return Ok(status);
        }

        private List<BuffAnimalPagedModel> buildBuffAnimalPagedModel(BuffAnimalSearchFilterModel searchFilter, List<ABuffAnimal> buffAnimalList)
        {

            int pagesize = searchFilter.pageSize == 0 ? 10 : searchFilter.pageSize;
            int page = searchFilter.page == 0 ? 1 : searchFilter.page;
            var items = (dynamic)null;

            int totalItems = buffAnimalList.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
            items = buffAnimalList.Skip((page - 1) * pagesize).Take(pagesize).ToList();

            List<BuffAnimalListResponseModel> buffAnimalModels = convertBuffAnimalListToResponseModel(items);
            //List<BuffAnimalListResponseModel> buffAnimalModels = convertBuffAnimalListToResponseModel(items);

            var result = new List<BuffAnimalPagedModel>();
            var item = new BuffAnimalPagedModel();

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
            item.items = buffAnimalModels;
            result.Add(item);

            return result;
        }

        private List<BuffAnimalListResponseModel> convertBuffAnimalListToResponseModel(List<ABuffAnimal> buffAnimalList)
        {
            var buffAnimalResponseModels = new List<BuffAnimalListResponseModel>();

            foreach (ABuffAnimal buffAnimal in buffAnimalList)
            {

                var buffHerds = _context.HBuffHerds;
                var farmOwners = _context.Tbl_Farmers;

                var ownerDetails = buffHerds
                                    .Where(herd => herd.HerdCode.Equals(buffAnimal.HerdCode))
                                    .Join(farmOwners, herd => herd.Owner, owner => owner.Id,
                                    (herd, owner) => new { Id = owner.Id, FirstName = owner.FirstName, LastName = owner.LastName });
                string ownerName = "N/A";

                if (ownerDetails.Count() > 0)
                {
                    ownerName = ownerDetails.First().FirstName + "" + ownerDetails.First().LastName;
                }

                var buffAnimalResponseModel = new BuffAnimalListResponseModel()
                {
                    Id = buffAnimal.Id,
                    BreedRegNo = buffAnimal.breedRegistryNumber,
                    AnimalIdNumber = buffAnimal.AnimalIdNumber,
                    HerdCode = buffAnimal.HerdCode,
                    Photo = buffAnimal.Photo,
                    Owner = ownerName,
                    DateOfAcquisition = buffAnimal.DateOfAcquisition?.ToString("yyyy-MM-dd")
                };
                buffAnimalResponseModels.Add(buffAnimalResponseModel);
            }

            return buffAnimalResponseModels;
        }

        private OriginOfAcquisitionModel populateOriginOfAcquistionModel(ABuffAnimal buffAnimal)
        {
            var originOfAcquisition = _context.OriginOfAcquisitionModels.Where(originOfAcquistion => originOfAcquistion.Id.Equals(buffAnimal.OriginOfAcquisition)).FirstOrDefault();
            if (originOfAcquisition == null)
            {
                var originOfAcquisitionModel = new OriginOfAcquisitionModel()
                {
                    City = "",
                    Barangay = "",
                    Province = "",
                    Region = "",
                };
                return originOfAcquisitionModel;
            }
            else
            {
                var originOfAcquisitionModel = new OriginOfAcquisitionModel()
                {
                    City = originOfAcquisition.City,
                    Barangay = originOfAcquisition.Barangay,
                    Province = originOfAcquisition.Province,
                    Region = originOfAcquisition.Region
                };
                return originOfAcquisitionModel;
            }


        }

        private Animal populateAnimalModel(int? id)
        {
            var buffAnimal = _context.ABuffAnimals.Where(animal => animal.Id.Equals(id)).FirstOrDefault();
            string sql = $@"SELECT [id]
                      ,[Breed_Code]
                  FROM [dbo].[A_Breed] where Id ='" + buffAnimal.BreedCode + "'";
            DataTable table = db.SelectDb(sql).Tables[0];
            string sql1 = $@"SELECT [id]
                      ,[Blood_Code]
                  FROM [dbo].[A_Blood_Comp] where Id ='" + buffAnimal.BloodCode + "'";
            DataTable table1 = db.SelectDb(sql1).Tables[0];

            string fam = $@"SELECT [AnimalId]
                      ,[SireId]
                      ,[DamId]
                  FROM [dbo].[A_Family] where AnimalId ='" + id + "'";
            DataTable famtbl = db.SelectDb(fam).Tables[0];
            string sirereg = famtbl.Rows.Count == 0 ? "" : famtbl.Rows[0]["SireId"].ToString();
            if (famtbl.Rows.Count != 0)
            {
                sirereg = $@"select BreedRegistryNumber,Animal_ID_Number,Animal_Name from A_Buff_Animal where id='" + famtbl.Rows[0]["SireId"].ToString() + "'";
                DataTable siretbl = db.SelectDb(sirereg).Tables[0];
                if (siretbl.Rows.Count != 0)
                {
                    var sireModel = new Animal()
                    {
                        RegistrationNumber = siretbl.Rows[0]["BreedRegistryNumber"].ToString(),
                        IdNumber = siretbl.Rows[0]["Animal_ID_Number"].ToString(),
                        Name = siretbl.Rows[0]["Animal_Name"].ToString(),
                        BreedCode = table.Rows[0]["Breed_Code"].ToString(),
                        BloodCode = table1.Rows[0]["Blood_Code"].ToString()
                    };
                    return sireModel;
                }
                else
                {
                    var sireModel = new Animal()
                    {
                        RegistrationNumber = "",
                        IdNumber = "",
                        Name = "",
                        BreedCode = "",
                        BloodCode = "",
                    };
                    return sireModel;
                }


            }
            else
            {
                var sireModel = new Animal()
                {
                    RegistrationNumber = "",
                    IdNumber = "",
                    Name = "",
                    BreedCode = "",
                    BloodCode = "",
                };
                return sireModel;
            }


            ////string damreg = $@"select BreedRegistryNumber,Animal_ID_Number,Animal_Name from A_Buff_Animal where id='" + famtbl.Rows[0]["DamId"].ToString() + "'";
            ////DataTable damtbl = db.SelectDb(damreg).Tables[0];
            //if (siretbl.Rows.Count  == 0)
            //{

            //}
            //else
            //{

            //}

        }
        private Animal populatedamAnimalModel(int? id)
        {
            var buffAnimal = _context.ABuffAnimals.Where(animal => animal.Id.Equals(id)).FirstOrDefault();
            string sql = $@"SELECT [id]
                      ,[Breed_Code]
                  FROM [dbo].[A_Breed] where Id ='" + buffAnimal.BreedCode + "'";
            DataTable table = db.SelectDb(sql).Tables[0];
            string sql1 = $@"SELECT [id]
                      ,[Blood_Code]
                  FROM [dbo].[A_Blood_Comp] where Id ='" + buffAnimal.BloodCode + "'";
            DataTable table1 = db.SelectDb(sql1).Tables[0];

            string fam = $@"SELECT [AnimalId]
                      ,[SireId]
                      ,[DamId]
                  FROM [dbo].[A_Family] where AnimalId ='" + id + "'";
            DataTable famtbl = db.SelectDb(fam).Tables[0];
            //string sirereg = $@"select BreedRegistryNumber,Animal_ID_Number,Animal_Name from A_Buff_Animal where id='" + famtbl.Rows[0]["SireId"].ToString() + "'";
            //DataTable siretbl = db.SelectDb(sirereg).Tables[0];
            string fam_damid = famtbl.Rows.Count == 0 ? "" : famtbl.Rows[0]["DamId"].ToString();
            string damreg = $@"select BreedRegistryNumber,Animal_ID_Number,Animal_Name from A_Buff_Animal where id='" + fam_damid + "'";
            DataTable damtbl = db.SelectDb(damreg).Tables[0];
            if (damtbl.Rows.Count == 0)
            {
                var sireModel = new Animal()
                {
                    RegistrationNumber = "",
                    IdNumber = "",
                    Name = "",
                    BreedCode = "",
                    BloodCode = "",
                };
                return sireModel;
            }
            else
            {
                var sireModel = new Animal()
                {
                    RegistrationNumber = damtbl.Rows[0]["BreedRegistryNumber"].ToString(),
                    IdNumber = damtbl.Rows[0]["Animal_ID_Number"].ToString(),
                    Name = damtbl.Rows[0]["Animal_Name"].ToString(),
                    BreedCode = table.Rows[0]["Breed_Code"].ToString(),
                    BloodCode = table1.Rows[0]["Blood_Code"].ToString()
                };
                return sireModel;
            }

        }
        private BuffAnimalBaseModel convertBuffAnimalToResponseModel(ABuffAnimal buffAnimal)
        {


            string sql = $@"SELECT [id]
                      ,[Breed_Code]
                  FROM [dbo].[A_Breed] where Id ='" + buffAnimal.BreedCode + "'";
            DataTable table = db.SelectDb(sql).Tables[0];
            string sql1 = $@"SELECT [id]
                      ,[Blood_Code]
                  FROM [dbo].[A_Blood_Comp] where Id ='" + buffAnimal.BloodCode + "'";
            DataTable table1 = db.SelectDb(sql1).Tables[0];
            string fam = $@"SELECT [AnimalId]
                      ,[SireId]
                      ,[DamId]
                  FROM [dbo].[A_Family] where AnimalId ='" + buffAnimal.Id + "'";
            DataTable famtbl = db.SelectDb(fam).Tables[0];
            string sire = famtbl.Rows.Count == 0 ? "0" : famtbl.Rows[0]["SireId"].ToString();
            string dam = famtbl.Rows.Count == 0 ? "0" : famtbl.Rows[0]["DamId"].ToString();
            //int dam = int.Parse(famtbl.Rows[0]["DamId"].ToString()) == null ? 0 : int.Parse(famtbl.Rows[0]["DamId"].ToString());
            var buffAnimalResponseModel = new BuffAnimalBaseModel()
            {
                Id = buffAnimal.Id,
                AnimalIdNumber = buffAnimal.AnimalIdNumber,
                AnimalName = buffAnimal.AnimalName,
                Photo = buffAnimal.Photo,
                HerdCode = buffAnimal.HerdCode,
                RfidNumber = buffAnimal.RfidNumber,
                DateOfBirth = buffAnimal?.DateOfBirth,
                Sex = buffAnimal.Sex,
                BreedCode = table.Rows[0]["id"].ToString(),
                BirthType = buffAnimal.BirthType,
                CountryOfBirth = buffAnimal.CountryOfBirth,
                OriginOfAcquisition = populateOriginOfAcquistionModel(buffAnimal),
                DateOfAcquisition = buffAnimal.DateOfAcquisition,
                Marking = buffAnimal.Marking,
                TypeOfOwnership = buffAnimal.TypeOfOwnership,
                BloodCode = int.Parse(table1.Rows[0]["id"].ToString()),
                Sire = populateAnimalModel(buffAnimal.Id),
                Dam = populatedamAnimalModel(buffAnimal.Id),
                breedRegistryNumber = buffAnimal.breedRegistryNumber,
                GroudId = buffAnimal.GroupId,
                FarmerId = buffAnimal.FarmerId


            };

            return buffAnimalResponseModel;
        }

        private ABuffAnimal populateBuffAnimal(ABuffAnimal buffAnimal, BuffAnimalUpdateModel updateModel)
        {
            if (updateModel.AnimalIdNumber != null && updateModel.AnimalIdNumber != "")
            {
                buffAnimal.AnimalIdNumber = updateModel.AnimalIdNumber;
            }
            if (updateModel.AnimalName != null && updateModel.AnimalName != "")
            {
                buffAnimal.AnimalName = updateModel.AnimalName;
            }
            if (updateModel.Photo != null && updateModel.Photo != "")
            {
                buffAnimal.Photo = updateModel.Photo;
            }
            if (updateModel.HerdCode != null && updateModel.HerdCode != "")
            {
                buffAnimal.HerdCode = updateModel.HerdCode;
            }
            if (updateModel.RfidNumber != null && updateModel.RfidNumber != "")
            {
                buffAnimal.RfidNumber = updateModel.RfidNumber;
            }
            if (updateModel.DateOfBirth != null)
            {
                buffAnimal.DateOfBirth = updateModel.DateOfBirth;
            }
            if (updateModel.Sex != null && updateModel.Sex != "")
            {
                buffAnimal.Sex = updateModel.Sex;
            }
            if (updateModel.BreedCode != null && updateModel.BreedCode != "")
            {
                buffAnimal.BreedCode = updateModel.BreedCode;
            }
            if (updateModel.BirthType != null && updateModel.BirthType != "")
            {
                buffAnimal.BirthType = updateModel.BirthType;
            }
            if (updateModel.CountryOfBirth != null && updateModel.CountryOfBirth != "")
            {
                buffAnimal.CountryOfBirth = updateModel.CountryOfBirth;
            }
            if (updateModel.DateOfAcquisition != null)
            {
                buffAnimal.DateOfAcquisition = updateModel.DateOfAcquisition;
            }
            if (updateModel.Marking != null && updateModel.Marking != "")
            {
                buffAnimal.Marking = updateModel.Marking;
            }
            if (updateModel.TypeOfOwnership != null && updateModel.TypeOfOwnership != "")
            {
                buffAnimal.TypeOfOwnership = updateModel.TypeOfOwnership;
            }
            if (updateModel.BloodCode != null && updateModel.BloodCode != 0)
            {
                buffAnimal.BloodCode = updateModel.BloodCode;
            }
            if (updateModel.breedRegistryNumber != null && updateModel.breedRegistryNumber != "")
            {
                buffAnimal.breedRegistryNumber = updateModel.breedRegistryNumber;
            }
            if (updateModel.FarmerId.HasValue)
            {
                buffAnimal.FarmerId = updateModel.FarmerId;
            }
            if (updateModel.GroudId.HasValue)
            {
                buffAnimal.GroupId = updateModel.GroudId;
            }
            return buffAnimal;
        }

        private ABuffAnimal buildBuffAnimal(BuffAnimalRegistrationModel registrationModel)
        {

            string _dateDiff = registrationModel.DateOfBirth.HasValue
                        ? ((registrationModel.DateOfBirth.Value - new DateTime(1899, 12, 30)).Days).ToString()
                        : "0";

            string _animalIdNumber = registrationModel.AnimalIdNumber;

            string _sex = registrationModel.Sex.Substring(0, 1).ToUpper();


            string? _breedCode = _context.ABreeds
                                .Where(b => b.Id.ToString().Equals(registrationModel.BreedCode))
                                .Select(b => b.BreedCode)
                                .FirstOrDefault();

            string BreedRegistryNumber = _animalIdNumber + _dateDiff + _sex + _breedCode;

            var buffAnimal = new ABuffAnimal()
            {
                AnimalIdNumber = registrationModel.AnimalIdNumber,
                AnimalName = registrationModel.AnimalName,
                Photo = registrationModel.Photo,
                HerdCode = registrationModel.HerdCode,
                RfidNumber = registrationModel.RfidNumber,
                FarmerId = registrationModel.FarmerId,
                GroupId = registrationModel.GroudId,
                DateOfBirth = registrationModel.DateOfBirth,
                Sex = registrationModel.Sex,
                BreedCode = registrationModel.BreedCode,
                BirthType = registrationModel.BirthType,
                CountryOfBirth = registrationModel.CountryOfBirth,
                DateOfAcquisition = registrationModel.DateOfAcquisition,
                Marking = registrationModel.Marking,
                TypeOfOwnership = registrationModel.TypeOfOwnership,
                BloodCode = registrationModel.BloodCode,


                breedRegistryNumber = string.IsNullOrEmpty(registrationModel.breedRegistryNumber) ? BreedRegistryNumber : registrationModel.breedRegistryNumber
            };

            return buffAnimal;
        }

        private ABuffAnimal buildBuffAnimal(Animal animal)
        {

            string sql1 = $@"SELECT [id]
                      ,[Blood_Code]
                  FROM [dbo].[A_Blood_Comp] where Blood_Code ='" + animal.BloodCode + "'";
            DataTable table1 = db.SelectDb(sql1).Tables[0];
            int bloodcode = table1.Rows.Count == 0 ? 0 : int.Parse(table1.Rows[0]["id"].ToString());
            var buffAnimal = new ABuffAnimal()
            {
                AnimalIdNumber = animal.IdNumber,
                AnimalName = animal.Name,
                Sex = animal.Sex,
                RfidNumber = animal.RegistrationNumber,
                BreedCode = animal.BreedCode,
                BloodCode = bloodcode,
                CreatedDate = DateTime.Now
            };

            return buffAnimal;
        }
        [HttpPost]
        public async Task<IActionResult> GetAnimalCount(AnimalCountFilter data)
        {
            string sql = $@"select count (*) as count from a_buff_animal";
            //string result = "";

            if (data.sex != null & data.year == null)
            {
                sql += " where sex = '" + data.sex + "'";
            }
            else if (data.sex == null & data.year != null)
            {
                sql += " where year(Date_of_Birth = '" + data.year + "'";
            }
            else if (data.sex != null & data.year != null)
            {
                sql += " where sex = '" + data.sex + "' and year(Date_of_Birth) = '" + data.year + "'";
            }
            DataTable dt = db.SelectDb(sql).Tables[0];
            var result = new AnimalCount();
            foreach (DataRow dr in dt.Rows)
            {
                result.count = dr["count"].ToString();
            }

            return Ok(result);
        }

        public class AnimalCountFilter
        {
            public string? sex { get; set; }
            public string? year { get; set; }
        }
        public class AnimalCount
        {
            public string count { get; set; }
        }
    }
}
