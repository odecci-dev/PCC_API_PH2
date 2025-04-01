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
using NuGet.Configuration;
using PeterO.Numbers;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Dynamic.Core;
using System.Security.Policy;
using System.Reflection.Metadata;
using static API_PCC.Manager.DBMethods;
namespace API_PCC.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class PedigreeController : ControllerBase
    {
        private readonly PCC_DEVContext _context;
        DBMethods dbmet = new DBMethods();
        DbManager db = new DbManager();
        public PedigreeController(PCC_DEVContext context)
        {
            _context = context;
        }

        //[HttpGet("{id}")]
        //public async Task<ActionResult> view(int id)
        //{
        //    var pedigreeTree = createPedigreeTree(id);
        //    return Ok(pedigreeTree);
        //}
        //private AnimalPedigreeTree<AnimalPedigreeModel> createPedigreeTree(int id)
        //{

        //}
        private void generatePedigrees(Node<AnimalPedigreeModel> parentNode, int sireid, int damid)
        {
            if (parentNode.level == 3)
            {
                return;
            }
            //var sire = _context.ABuffAnimals.Find(buffAnimal.SireId);
            //var dam = _context.ABuffAnimals.Find(buffAnimal.DamId);


            var sire = dbmet.getfamily().Find(a => a.sireId == sireid);
            var dam = dbmet.getfamily().Find(a => a.damId == damid);

            var sireToAnimalModel = convertToAnimalsirePedigreeModels(sire);
            var damToAnimalModel = convertToAnimaldamPedigreeModels(dam);

            if (sireToAnimalModel != null)
            {
                Node<AnimalPedigreeModel> sireNode = new Node<AnimalPedigreeModel>(convertToAnimalsirePedigreeModels(sire));
                sireNode.level = parentNode.level + 1;
                parentNode.AddSire(sireNode);
                generatePedigrees(sireNode, sire.sireId, dam.damId);
            }

            if (damToAnimalModel != null)
            {
                Node<AnimalPedigreeModel> damNode = new Node<AnimalPedigreeModel>(convertToAnimaldamPedigreeModels(dam));
                damNode.level = parentNode.level + 1;
                parentNode.AddDam(damNode);
                generatePedigrees(damNode, sire.sireId, dam.damId);
            }
        }
        private AnimalPedigreeModel convertToAnimalsirePedigreeModels(A_Family buffAnimal)
        {
            if (buffAnimal != null)
            {
                var buff = _context.ABuffAnimals.Find(buffAnimal.sireId);
                var animalPedigreeModel = new AnimalPedigreeModel()
                {
                    RegistrationNumber = buff.breedRegistryNumber,
                    Photo = buff.Photo,
                    Name = buff.AnimalName,
                    DateOfBirth = buff.DateOfBirth,
                    PlaceOfBirth = buff.CountryOfBirth
                };
                return animalPedigreeModel;
            }

            return null;
        }
        private AnimalPedigreeModel convertToAnimalchildPedigreeModels(A_Family buffAnimal)
        {
            if (buffAnimal != null)
            {
                var buff = _context.ABuffAnimals.Find(buffAnimal.animalId);
                var animalPedigreeModel = new AnimalPedigreeModel()
                {
                    RegistrationNumber = buff.breedRegistryNumber,
                    Photo = buff.Photo,
                    Name = buff.AnimalName,
                    DateOfBirth = buff.DateOfBirth,
                    PlaceOfBirth = buff.CountryOfBirth
                };
                return animalPedigreeModel;
            }

            return null;
        }
        private AnimalPedigreeModel convertToAnimaldamPedigreeModels(A_Family buffAnimal)
        {
            if (buffAnimal != null)
            {
                var buff = _context.ABuffAnimals.Find(buffAnimal.damId);
                var animalPedigreeModel = new AnimalPedigreeModel()
                {
                    RegistrationNumber = buff.breedRegistryNumber,
                    Photo = buff.Photo,
                    Name = buff.AnimalName,
                    DateOfBirth = buff.DateOfBirth,
                    PlaceOfBirth = buff.CountryOfBirth
                };
                return animalPedigreeModel;
            }

            return null;
        }
        public class childlv0
        {

            //public int level { get; set; }
            public int Id { get; set; }
            public string animalId { get; set; }
            
            public string breedRegistryNumber { get; set; }
            public string Photo { get; set; }
            public string AnimalName { get; set; }
            public string DateOfBirth { get; set; }
            public string CountryOfBirth { get; set; }
            public string bloodComp { get; set; }
            public string breedCode { get; set; }
            public string bloodResult { get; set; }
            public int level { get; set; }
            public parentsirelv0 parentsirelv0 { get; set; }
            public parentdamlv0 parentdamlv0 { get; set; }


        }
        public class parentsirelv0
        {
            public int Id { get; set; }
            public string animalId { get; set; }
            public string breedRegistryNumber { get; set; }
            public string Photo { get; set; }
            public string AnimalName { get; set; }
            public string DateOfBirth { get; set; }
            public string CountryOfBirth { get; set; }
            public string bloodComp { get; set; }
            public string breedCode { get; set; }
            public string bloodResult { get; set; }
            public int level { get; set; }
            public parentdamlv1 parentdamlv1 { get; set; }
            public parentsirelv1 parentsirelv1 { get; set; }

        }
        public class parentdamlv0
        {
            public int Id { get; set; }
            public string animalId { get; set; }
            public string breedRegistryNumber { get; set; }
            public string Photo { get; set; }
            public string AnimalName { get; set; }
            public string DateOfBirth { get; set; }
            public string CountryOfBirth { get; set; }
            public string bloodComp { get; set; }
            public string breedCode { get; set; }
            public string bloodResult { get; set; }
            public int level { get; set; }
            public parentdamlv1 parentdam { get; set; }
            public parentsirelv1 parentsire { get; set; }

        }
        //public class childlv1
        //{
        //    public int Id { get; set; }
        //    public string animalId { get; set; }
        //    public int sireId { get; set; }
        //    public int damId { get; set; }
        //    public int level { get; set; }
        //    public List<parentsirelv1> parentsirelv1 { get; set; }
        //    public List<parentdamlv1> parentdamlv1 { get; set; }


        //}
        public class parentsirelv1
        {
            public int Id { get; set; }
            public string animalId { get; set; }
            public string breedRegistryNumber { get; set; }
            public string Photo { get; set; }
            public string AnimalName { get; set; }
            public string DateOfBirth { get; set; }
            public string CountryOfBirth { get; set; }
            public string bloodComp { get; set; }
            public string breedCode { get; set; }
            public string bloodResult { get; set; }
            public int level { get; set; }
            public parentdamlv2 parentdamlv2 { get; set; }
            public parentsirelv2 parentsirelv2 { get; set; }

        }
        public class parentdamlv1
        {
            public int Id { get; set; }
            public string animalId { get; set; }
            public string breedRegistryNumber { get; set; }
            public string Photo { get; set; }
            public string AnimalName { get; set; }
            public string DateOfBirth { get; set; }
            public string CountryOfBirth { get; set; }
            public string bloodComp { get; set; }
            public string breedCode { get; set; }
            public string bloodResult { get; set; }
            public int level { get; set; }
            public parentdamlv2 parentdamlv2 { get; set; }
            public parentsirelv2 parentsirelv2 { get; set; }

        }
        public class childlv2
        {
            public int Id { get; set; }
            public string animalId { get; set; }
            public string breedRegistryNumber { get; set; }
            public string Photo { get; set; }
            public string AnimalName { get; set; }
            public string DateOfBirth { get; set; }
            public string CountryOfBirth { get; set; }
            public string bloodComp { get; set; }
            public string breedCode { get; set; }
            public string bloodResult { get; set; }
            public int level { get; set; }
            public List<parentsirelv3> parentsirelv3 { get; set; }
            public List<parentdamlv3> parentdamlv3 { get; set; }


        }
        public class parentsirelv2
        {
            public int Id { get; set; }
            public string animalId { get; set; }
            public string breedRegistryNumber { get; set; }
            public string Photo { get; set; }
            public string AnimalName { get; set; }
            public string DateOfBirth { get; set; }
            public string CountryOfBirth { get; set; }
            public string bloodComp { get; set; }
            public string breedCode { get; set; }
            public string bloodResult { get; set; }
            public int level { get; set; }
            public parentdamlv3 parentdamlv3 { get; set; }
            public parentsirelv3 parentsirelv3 { get; set; }

        }
        public class parentdamlv2
        {
            public int Id { get; set; }
            public string animalId { get; set; }
            public string breedRegistryNumber { get; set; }
            public string Photo { get; set; }
            public string AnimalName { get; set; }
            public string DateOfBirth { get; set; }
            public string CountryOfBirth { get; set; }
            public string bloodComp { get; set; }
            public string breedCode { get; set; }
            public string bloodResult { get; set; }
            public int level { get; set; }
            public List<parentdamlv3> parentdamlv3 { get; set; }
            public List<parentsirelv3> parentsirelv3 { get; set; }
        }
        public class childlv3
        {
            public int Id { get; set; }
            public string animalId { get; set; }
            public string breedRegistryNumber { get; set; }
            public string Photo { get; set; }
            public string AnimalName { get; set; }
            public string DateOfBirth { get; set; }
            public string CountryOfBirth { get; set; }
            public string bloodComp { get; set; }
            public string breedCode { get; set; }
            public string bloodResult { get; set; }
            public int level { get; set; }
            public List<parentsirelv3> parentsire { get; set; }
            public List<parentdamlv3> parentdam { get; set; }


        }
        public class parentsirelv3
        {
            public int Id { get; set; }
            public string animalId { get; set; }
            public string breedRegistryNumber { get; set; }
            public string Photo { get; set; }
            public string AnimalName { get; set; }
            public string DateOfBirth { get; set; }
            public string CountryOfBirth { get; set; }
            public string bloodComp { get; set; }
            public string breedCode { get; set; }
            public string bloodResult { get; set; }
            public int level { get; set; }

        }
        public class parentdamlv3
        {
            public int Id { get; set; }
            public string animalId { get; set; }
            public string breedRegistryNumber { get; set; }
            public string Photo { get; set; }
            public string AnimalName { get; set; }
            public string DateOfBirth { get; set; }
            public string CountryOfBirth { get; set; }
            public string bloodComp { get; set; }
            public string breedCode { get; set; }
            public string bloodResult { get; set; }
            public int level { get; set; }

        }
        [HttpGet("{id}")]
        public async Task<ActionResult> view(int id)
        {

            var animal = generatepedigree(id);


            return Ok(animal);
            //return Ok(root);
        }
        public class AnimalPedigreePrintResponses
        {
            public AnimalDetails animalDetails { get; set; }
            public childlv0 animalPedigrees { get; set; }
            public HerdDetails herdDetails { get; set; }

        }
       
        [HttpGet("{id}")]
        public async Task<ActionResult> print(int id)
        {
            //
            var item = new AnimalPedigreePrintResponses();
            var animal_details = _context.ABuffAnimals.Where(a => a.Id == id).FirstOrDefault();
            string str_breedcode = dbmet.BreedCode(animal_details.BreedCode);
            string str_btype = dbmet.BirthType(animal_details.BirthType);
            string str_bcode = dbmet.BloodCode(animal_details.BloodCode.ToString());
            var animalDetails = new AnimalDetails();
            animalDetails.DateOfRegistration = animal_details.CreatedDate;
            animalDetails.BreedRegistrationNumber = animal_details.breedRegistryNumber;
            animalDetails.HerdCode = animal_details.HerdCode;
            animalDetails.AnimalIdNumber = animal_details.AnimalIdNumber;
            animalDetails.Name = animal_details.AnimalName;
            animalDetails.Rfid = animal_details.RfidNumber;
            animalDetails.Sex = animal_details.Sex;
            animalDetails.Breed = str_breedcode;
            animalDetails.BloodComp = str_bcode;
            animalDetails.BloodResult = animal_details.BloodResult.ToString();
            animalDetails.DateOfBirth = animal_details.DateOfBirth;
            animalDetails.CountryOfBirth = animal_details.CountryOfBirth;
            animalDetails.BirthType = str_btype;
            var originOfAcquisition = populateOriginOfAcquistionModel(animal_details);

            if (originOfAcquisition != null)
            {
                animalDetails.OriginOfAcquisition = originOfAcquisition;
            }
            animalDetails.DateOfAcquisition = animal_details.DateOfAcquisition;

            var typeOwnership = _context.ATypeOwnerships.FirstOrDefault(t => t.TypeOwnCode == animal_details.TypeOfOwnership);
            animalDetails.TypeOfOWnership = typeOwnership.TypeOwnDesc;


            var herdDetails = new HerdDetails();
            var buffHerd = getHerdRecord(animal_details.HerdCode);
            if (buffHerd != null)
            {
                herdDetails.DateOfApplication = buffHerd.DateCreated;
                herdDetails.HerdName = buffHerd.HerdName;
                herdDetails.HerdType = buffHerd.HerdClassDesc;
                herdDetails.HerdSize = buffHerd.HerdSize;

                var buffaloTypeList = new List<string>();
                var feedingSystemList = new List<string>();
                if (buffHerd.buffaloType != null)
                {
                    foreach (HBuffaloType buffaloType in buffHerd.buffaloType)
                    {
                        //buffaloTypeList.Add(buffaloType.BreedTypeCode);
                        buffaloTypeList.Add(buffaloType.BreedTypeDesc);
                    }
                    herdDetails.TypeOfBuffalo = string.Join(",", buffaloTypeList);

                }
                if (buffHerd.feedingSystem != null)
                {
                    foreach (HFeedingSystem feedingSystem in buffHerd.feedingSystem)
                    {
                        //feedingSystemList.Add(feedingSystem.FeedingSystemCode);
                        feedingSystemList.Add(feedingSystem.FeedingSystemDesc);
                    }
                    herdDetails.FeedingSystem = string.Join(",", feedingSystemList);
                }
                herdDetails.FarmManager = buffHerd.FarmManager;
                herdDetails.FarmAddress = buffHerd.FarmAddress;
            }
            item.animalDetails = animalDetails;
            item.herdDetails = herdDetails;
            item.animalPedigrees = generatepedigree(id);

            return Ok(item);
            //return Ok(root);
        }
 

        private HBuffHerd getHerdRecord(string herdCode)
        {
            var buffHerd = _context.HBuffHerds
                                   .Include(herd => herd.buffaloType)
                                   .Include(herd => herd.feedingSystem)
                                   .Where(herd => herd.HerdCode.Equals(herdCode)).FirstOrDefault();
            if (buffHerd == null)
            {
                buffHerd.Center = 0;
                buffHerd.CreatedBy = "";
                buffHerd.DateCreated = DateTime.Now;
                buffHerd.DateDeleted = null;
                buffHerd.DateRestored = null;
                buffHerd.DateUpdated = null;
                buffHerd.DeleteFlag = true;
                buffHerd.DeletedBy = "";
                buffHerd.FarmAddress = "";
                buffHerd.FarmAffilCode = "";
                buffHerd.FarmManager = "";
                buffHerd.HerdClassDesc = "";
                buffHerd.HerdCode = "";
                buffHerd.HerdName = "";
                buffHerd.HerdSize = 0;
                buffHerd.Id = 0;
                buffHerd.OrganizationName = "";
                buffHerd.Owner = 0;
                buffHerd.Photo = "";
                buffHerd.RestoredBy = "";
                buffHerd.Status = 0;
                buffHerd.UpdatedBy = "";
                buffHerd.buffaloType = null;
                buffHerd.feedingSystem = null;

                //throw new Exception("Herd record not found!");
            }
            else
            {
                string tbl = $@"SELECT  Herd_Class_Desc FROM H_Herd_Classification where Herd_Class_Code='" + buffHerd.HerdClassDesc + "'";
                DataTable tbl_hc = db.SelectDb(tbl).Tables[0];

                string cow_lvl = $@"SELECT  count(*) as cowlevel FROM A_Buff_Animal where Herd_Code ='" + buffHerd.HerdCode + "' ";
                DataTable cow_lvl_tbl = db.SelectDb(cow_lvl).Tables[0];
                string cow_res = cow_lvl_tbl.Rows.Count == 0 ? "0" : cow_lvl_tbl.Rows[0]["cowlevel"].ToString();
                //var buffHerdResponseModel = new BuffHerdListResponseModel()
                //{
                //    Id = buffHerd.Id.ToString(),
                //    HerdName = buffHerd.HerdName,
                //    HerdClassification = tbl_hc.Rows[0]["Herd_Class_Desc"].ToString(),
                //    CowLevel = cow_res,
                //    FarmManager = buffHerd.FarmManager,
                //    HerdCode = buffHerd.HerdCode,
                //    Photo = buffHerd.Photo,
                //    DateOfApplication = buffHerd.DateCreated.ToString("yyyy-MM-dd")
                //};
                buffHerd.Center = buffHerd.Center;
                buffHerd.CreatedBy = buffHerd.CreatedBy;
                buffHerd.DateCreated = buffHerd.DateCreated;
                buffHerd.DateDeleted = buffHerd.DateDeleted;
                buffHerd.DateRestored = buffHerd.DateRestored;
                buffHerd.DateUpdated = buffHerd.DateUpdated;
                buffHerd.DeleteFlag = buffHerd.DeleteFlag;
                buffHerd.DeletedBy = buffHerd.DeletedBy;
                buffHerd.FarmAddress = buffHerd.FarmAddress;
                buffHerd.FarmAffilCode = buffHerd.FarmAffilCode;
                buffHerd.FarmManager = buffHerd.FarmManager;
                buffHerd.HerdClassDesc = tbl_hc == null ? tbl_hc.Rows[0]["Herd_Class_Desc"].ToString() : "";
                buffHerd.HerdCode = buffHerd.HerdCode;
                buffHerd.HerdName = buffHerd.HerdName;
                buffHerd.HerdSize = int.Parse(cow_res);
                buffHerd.Id = buffHerd.Id;
                buffHerd.OrganizationName = buffHerd.OrganizationName;
                buffHerd.Owner = buffHerd.Owner;
                buffHerd.Photo = buffHerd.Photo;
                buffHerd.RestoredBy = buffHerd.RestoredBy;
                buffHerd.Status = buffHerd.Status;
                buffHerd.UpdatedBy = buffHerd.UpdatedBy;
                buffHerd.buffaloType = buffHerd.buffaloType;
                buffHerd.feedingSystem = buffHerd.feedingSystem;
            }
            return buffHerd;
        }

        private OriginOfAcquisitionModel populateOriginOfAcquistionModel(ABuffAnimal buffAnimal)
        {
            var originOfAcquisition = _context.OriginOfAcquisitionModels.Where(originOfAcquistion => originOfAcquistion.Id.Equals(buffAnimal.OriginOfAcquisition)).FirstOrDefault();
            if (originOfAcquisition == null)
            {
                return null;
            }
            var originOfAcquisitionModel = new OriginOfAcquisitionModel()
            {
                City = originOfAcquisition.City,
                Barangay = originOfAcquisition.Barangay,
                Province = originOfAcquisition.Province,
                Region = originOfAcquisition.Region
            };
            return originOfAcquisitionModel;

        }

        private AnimalPedigreeTree<AnimalPedigreeModel> createPedigreeTree(int id)
        {
            var animal = _context.ABuffAnimals.Find(id);
            var root = new AnimalPedigreeTree<AnimalPedigreeModel>();

            if (animal != null)
            {
                Node<AnimalPedigreeModel> rootNode = new Node<AnimalPedigreeModel>(convertToAnimalPedigreeModel(animal));
                rootNode.level = 0;
                root.Add(rootNode);

                generatePedigree(rootNode, animal);
            }
            return root;
        }

        private void generatePedigree(Node<AnimalPedigreeModel> parentNode, ABuffAnimal buffAnimal)
        {
            if (parentNode.level == 3)
            {
                return;
            }
            /*var sire = _context.ABuffAnimals.Find(buffAnimal.SireId);
           var dam =  _context.ABuffAnimals.Find(buffAnimal.DamId);*/
            var res_sire = dbmet.getsirefamilybuff().Find(a => a.Id == buffAnimal.Id);
            var sire_res = new ABuffAnimal
            {
                breedRegistryNumber = res_sire.breedRegistryNumber,
                Photo = res_sire.Photo,
                AnimalName = res_sire.AnimalName,
                DateOfBirth = res_sire.DateOfBirth,
                CountryOfBirth = res_sire.CountryOfBirth
            };
            var dam = new ABuffAnimal
            {

            };
            //ABuffAnimal sire = new ABuffAnimal();
            //ABuffAnimal dam = new ABuffAnimal();

            var sireToAnimalModel = convertToAnimalPedigreeModel(sire_res);
            var damToAnimalModel = convertToAnimalPedigreeModel(dam);

            if (sireToAnimalModel != null)
            {
                Node<AnimalPedigreeModel> sireNode = new Node<AnimalPedigreeModel>(convertToAnimalPedigreeModel(sire_res));
                sireNode.level = parentNode.level + 1;
                parentNode.AddSire(sireNode);
                generatePedigree(sireNode, sire_res);
            }

            if (damToAnimalModel != null)
            {
                Node<AnimalPedigreeModel> damNode = new Node<AnimalPedigreeModel>(convertToAnimalPedigreeModel(dam));
                damNode.level = parentNode.level + 1;
                parentNode.AddDam(damNode);
                generatePedigree(damNode, dam);
            }
        }
        private AnimalPedigreeModel convertToAnimalPedigreeModel(ABuffAnimal buffAnimal)
        {
            if (buffAnimal != null)
            {
                var animalPedigreeModel = new AnimalPedigreeModel()
                {
                    RegistrationNumber = buffAnimal.breedRegistryNumber,
                    Photo = buffAnimal.Photo,
                    Name = buffAnimal.AnimalName,
                    DateOfBirth = buffAnimal.DateOfBirth,
                    PlaceOfBirth = buffAnimal.CountryOfBirth
                };
                return animalPedigreeModel;
            }

            return null;
        }
        private childlv0 generatepedigree(int id)
        {
            var animal = dbmet.getfamily().Where(a => a.animalId == id).ToList();

            var item = new childlv0();
            int counter = 0;
            for (int x = 0; x < animal.Count; x++)
            {
                item.Id = animal[x].animalId;
                item.level = 0;
                var animal_details = _context.ABuffAnimals.Where(a => a.Id == animal[x].animalId).FirstOrDefault();
                item.animalId = animal_details.AnimalIdNumber;
                item.breedRegistryNumber = animal_details.breedRegistryNumber;
                item.Photo = animal_details.Photo;
                item.AnimalName = animal_details.AnimalName;
                item.DateOfBirth = animal_details.DateOfBirth.ToString();
                item.CountryOfBirth = animal_details.CountryOfBirth.ToString();
                var animal_blood_details = _context.ABloodComps.Where(b => b.Id == animal_details.BloodCode).FirstOrDefault();
                item.breedCode = dbmet.BreedCode(animal_details.BreedCode);
                item.bloodResult = animal_details.BloodResult.ToString();
                item.bloodComp = animal_blood_details.BloodDesc.ToString();

                //=====================SIRE START HERE
                var s_item = new parentsirelv0();
                var sirelist = dbmet.getfamily().Find(a => a.animalId ==  animal[x].sireId);
                if (sirelist != null)
                {
                    //var s_item = new parentsirelv0();
                    var animal_details0 = _context.ABuffAnimals.Where(a => a.Id == animal[x].sireId).FirstOrDefault();
                    s_item.breedRegistryNumber = animal_details0.breedRegistryNumber;
                    s_item.Photo = animal_details0.Photo;
                    s_item.AnimalName = animal_details0.AnimalName;
                    s_item.DateOfBirth = animal_details0.DateOfBirth.ToString();
                    s_item.CountryOfBirth = animal_details0.CountryOfBirth.ToString();
                    s_item.animalId = animal_details0.AnimalIdNumber;
                    s_item.Id = sirelist.sireId;
                    s_item.level = 1;
                    var animal_blood_details0 = _context.ABloodComps.Where(b => b.Id == animal_details0.BloodCode).FirstOrDefault();
                    s_item.breedCode = dbmet.BreedCode(animal_details0.BreedCode);
                    s_item.bloodResult = animal_details0.BloodResult.ToString();
                    s_item.bloodComp = animal_blood_details0.BloodDesc.ToString();

                    //------------------------------------------- level1 sire 
                    var s_item1 = new parentsirelv1();
                    var sirelistlvl1 = dbmet.getfamily().Find(a => a.animalId == sirelist.sireId );
                    if (sirelistlvl1 != null)
                    {
               
                        //var s_item1 = new parentsirelv1();

                        var animal_details1 = _context.ABuffAnimals.Where(a => a.Id == sirelist.sireId).FirstOrDefault();
                        s_item1.breedRegistryNumber = animal_details1.breedRegistryNumber;
                        s_item1.Photo = animal_details1.Photo;
                        s_item1.AnimalName = animal_details1.AnimalName;
                        s_item1.DateOfBirth = animal_details1.DateOfBirth.ToString();
                        s_item1.CountryOfBirth = animal_details1.CountryOfBirth.ToString();
                        s_item1.Id = sirelistlvl1.sireId;
                        s_item1.animalId = animal_details1.AnimalIdNumber;
                        s_item1.level = 2;
                        var animal_blood_details1 = _context.ABloodComps.Where(b => b.Id == animal_details1.BloodCode).FirstOrDefault();
                        s_item1.breedCode = dbmet.BreedCode(animal_details1.BreedCode);
                        s_item1.bloodResult = animal_details1.BloodResult.ToString();
                        s_item1.bloodComp = animal_blood_details1.BloodDesc.ToString();
                        //-----------------------------------level 2 sire
                        var s_item2 = new parentsirelv2();
                        var sirelistlvl2 = dbmet.getfamily().Find(a => a.animalId == sirelistlvl1.sireId );
                        if (sirelistlvl2 != null)
                        {
                            //var s_item2 = new parentsirelv2();

                            var animal_details2 = _context.ABuffAnimals.Where(a => a.Id == sirelistlvl1.sireId).FirstOrDefault();
                            s_item2.breedRegistryNumber = animal_details2.breedRegistryNumber;
                            s_item2.Photo = animal_details2.Photo;
                            s_item2.AnimalName = animal_details2.AnimalName;
                            s_item2.DateOfBirth = animal_details2.DateOfBirth.ToString();
                            s_item2.CountryOfBirth = animal_details2.CountryOfBirth.ToString();
                            s_item2.Id = sirelistlvl2.animalId;
                            s_item2.animalId = animal_details2.AnimalIdNumber;
                            s_item2.level = 3;
                            var animal_blood_details2 = _context.ABloodComps.Where(b => b.Id == animal_details2.BloodCode).FirstOrDefault();
                            s_item2.breedCode = dbmet.BreedCode(animal_details2.BreedCode);
                            s_item2.bloodResult = animal_details2.BloodResult.ToString();
                            s_item2.bloodComp = animal_blood_details2.BloodDesc.ToString();
                            //------------------------------------ lvel 3 sire
                            var s_item3 = new parentsirelv3();
                            var sirelistlvl3 = dbmet.getfamily().Find(a => a.animalId == sirelistlvl2.sireId );
                            if (sirelistlvl3 != null)
                            {
                                //var s_item3 = new parentsirelv3();

                                var animal_details3 = _context.ABuffAnimals.Where(a => a.Id == sirelistlvl2.sireId).FirstOrDefault();
                                s_item3.breedRegistryNumber = animal_details3.breedRegistryNumber;
                                s_item3.Photo = animal_details3.Photo;
                                s_item3.AnimalName = animal_details3.AnimalName;
                                s_item3.DateOfBirth = animal_details3.DateOfBirth.ToString();
                                s_item3.CountryOfBirth = animal_details3.CountryOfBirth.ToString();
                                s_item3.animalId = animal_details3.AnimalIdNumber;
                                s_item3.Id = sirelistlvl3.animalId;
                                s_item3.level = 4;
                                var animal_blood_details3 = _context.ABloodComps.Where(b => b.Id == animal_details3.BloodCode).FirstOrDefault();
                                s_item3.breedCode = dbmet.BreedCode(animal_details3.BreedCode);
                                s_item3.bloodResult = animal_details3.BloodResult.ToString();
                                s_item3.bloodComp = animal_blood_details3.BloodDesc.ToString();
                                //parentsirelv2.Add(s_item2);

                            }
                            //------------------------------------ lvel 3 dam
                            var d_item3 = new parentdamlv3();
                            if (sirelistlvl3 != null)
                            {
                                var damlistlvl3 = dbmet.getfamily().Find(a => a.animalId == sirelistlvl3.damId ) ;
                                if (damlistlvl3 != null)
                                {
                                    //var d_item3 = new parentdamlv3();


                                    var animal_details4 = _context.ABuffAnimals.Where(a => a.Id == sirelistlvl3.damId).FirstOrDefault();
                                    d_item3.breedRegistryNumber = animal_details4.breedRegistryNumber;
                                    d_item3.Photo = animal_details4.Photo;
                                    d_item3.AnimalName = animal_details4.AnimalName;
                                    d_item3.DateOfBirth = animal_details4.DateOfBirth.ToString();
                                    d_item3.CountryOfBirth = animal_details4.CountryOfBirth.ToString();
                                    d_item3.animalId = animal_details4.AnimalIdNumber;
                                    d_item3.Id = sirelistlvl3.animalId;
                                    d_item3.level = 4;
                                    var animal_blood_details4 = _context.ABloodComps.Where(b => b.Id == animal_details4.BloodCode).FirstOrDefault();
                                    d_item3.breedCode = dbmet.BreedCode(animal_details4.BreedCode);
                                    d_item3.bloodResult = animal_details4.BloodResult.ToString();
                                    d_item3.bloodComp = animal_blood_details4.BloodDesc.ToString();
                                    //parentdamlv3.Add(d_item3);
                                }
                            }
                            s_item2.parentdamlv3 = d_item3;
                            s_item2.parentsirelv3 = s_item3;


                            //parentsirelv2.Add(s_item2);

                        }

                        //------------------------------------ lvel 2 dam
                        var d_item2 = new parentdamlv2();
                        if (sirelistlvl2 != null)
                        {
                            var damlistlvl2 = dbmet.getfamily().Find(a => a.animalId == sirelistlvl1.damId );
                            if (damlistlvl2 != null)
                            {
                                //var d_item2 = new parentdamlv2();

                                var animal_details_dam2 = _context.ABuffAnimals.Where(a => a.Id == sirelistlvl1.damId).FirstOrDefault();
                                d_item2.breedRegistryNumber = animal_details_dam2.breedRegistryNumber;
                                d_item2.Photo = animal_details_dam2.Photo;
                                d_item2.AnimalName = animal_details_dam2.AnimalName;
                                d_item2.DateOfBirth = animal_details_dam2.DateOfBirth.ToString();
                                d_item2.CountryOfBirth = animal_details_dam2.CountryOfBirth.ToString();
                                d_item2.animalId = animal_details_dam2.AnimalIdNumber;
                                d_item2.Id = damlistlvl2.animalId;
                                d_item2.level = 3;
                                var animal_blood_dam2 = _context.ABloodComps.Where(b => b.Id == animal_details_dam2.BloodCode).FirstOrDefault();
                                d_item2.breedCode = dbmet.BreedCode(animal_details_dam2.BreedCode);
                                d_item2.bloodResult = animal_details_dam2.BloodResult.ToString();
                                d_item2.bloodComp = animal_blood_dam2.BloodDesc.ToString();
                                //parentdamlv2.Add(d_item2);
                            }

                        }
                        s_item1.parentdamlv2 = d_item2; //dto
                        s_item1.parentsirelv2 = s_item2;

                        //parentsirelv1.Add(s_item1);

                    }

                    //---------------------------------------------level 1 dam
                    var d_item1 = new parentdamlv1();
                    var damlistlvl1 = dbmet.getfamily().Find(a => a.animalId == sirelist.damId ); //here
                    if (damlistlvl1 != null)
                    {
                        //var d_item1 = new parentdamlv1();
                        var animal_details_dam1 = _context.ABuffAnimals.Where(a => a.Id == sirelist.damId).FirstOrDefault();
                        d_item1.breedRegistryNumber = animal_details_dam1.breedRegistryNumber;
                        d_item1.Photo = animal_details_dam1.Photo;
                        d_item1.AnimalName = animal_details_dam1.AnimalName;
                        d_item1.DateOfBirth = animal_details_dam1.DateOfBirth.ToString();
                        d_item1.CountryOfBirth = animal_details_dam1.CountryOfBirth.ToString();
                        d_item1.Id = damlistlvl1.animalId;
                        d_item1.animalId = animal_details_dam1.AnimalIdNumber;
                        d_item1.level = 2;
                        var animal_blood_dam1 = _context.ABloodComps.Where(b => b.Id == animal_details_dam1.BloodCode).FirstOrDefault();
                        d_item1.breedCode = dbmet.BreedCode(animal_details_dam1.BreedCode);
                        d_item1.bloodResult = animal_details_dam1.BloodResult.ToString();
                        d_item1.bloodComp = animal_blood_dam1.BloodDesc.ToString();
                        var s_item2 = new parentsirelv2();
                        var sirelistlvl2 = dbmet.getfamily().Find(a => a.animalId == damlistlvl1.sireId ); 
                        if (sirelistlvl2 != null)
                        {
                            //var s_item2 = new parentsirelv2();

                            var animal_details2 = _context.ABuffAnimals.Where(a => a.Id == damlistlvl1.sireId).FirstOrDefault();
                            s_item2.breedRegistryNumber = animal_details2.breedRegistryNumber;
                            s_item2.Photo = animal_details2.Photo;
                            s_item2.AnimalName = animal_details2.AnimalName;
                            s_item2.DateOfBirth = animal_details2.DateOfBirth.ToString();
                            s_item2.CountryOfBirth = animal_details2.CountryOfBirth.ToString();
                            s_item2.Id = sirelistlvl2.animalId;
                            s_item2.animalId = animal_details2.AnimalIdNumber;
                            s_item2.level = 3;
                            var animal_blood_details2 = _context.ABloodComps.Where(b => b.Id == animal_details2.BloodCode).FirstOrDefault();
                            s_item2.breedCode = dbmet.BreedCode(animal_details2.BreedCode);
                            s_item2.bloodResult = animal_details2.BloodResult.ToString();
                            s_item2.bloodComp = animal_blood_details2.BloodDesc.ToString();
                            //parentsirelv2.Add(s_item2);

                        }
                        var d_item2 = new parentdamlv2(); // here 2
                        if (damlistlvl1 != null)
                        {
                            var damlistlvl2 = dbmet.getfamily().Find(a => a.animalId == damlistlvl1.damId );
                            if (damlistlvl2 != null)
                            {
                                //var d_item2 = new parentdamlv2();
                                var animal_details_dam2 = _context.ABuffAnimals.Where(a => a.Id == damlistlvl1.damId).FirstOrDefault();
                                d_item2.breedRegistryNumber = animal_details_dam2.breedRegistryNumber;
                                d_item2.Photo = animal_details_dam2.Photo;
                                d_item2.AnimalName = animal_details_dam2.AnimalName;
                                d_item2.DateOfBirth = animal_details_dam2.DateOfBirth.ToString();
                                d_item2.CountryOfBirth = animal_details_dam2.CountryOfBirth.ToString();
                                d_item2.Id = damlistlvl2.animalId;
                                d_item2.animalId = animal_details_dam2.AnimalIdNumber;
                                d_item2.level = 3;
                                var animal_blood_dam2 = _context.ABloodComps.Where(b => b.Id == animal_details_dam2.BloodCode).FirstOrDefault();
                                d_item2.breedCode = dbmet.BreedCode(animal_details_dam2.BreedCode);
                                d_item2.bloodResult = animal_details_dam2.BloodResult.ToString();
                                d_item2.bloodComp = animal_blood_dam2.BloodDesc.ToString();
                                //parentdamlv2.Add(d_item2);
                            }
                        }

                        d_item1.parentdamlv2 = d_item2;
                        d_item1.parentsirelv2 = s_item2;


                    }
                    s_item.parentsirelv1 = s_item1;
                    s_item.parentdamlv1 = d_item1;
                }

                //=================================================== DAM START HERE
                var d_item = new parentdamlv0();
                var damlist = dbmet.getfamily().Find(a => a.animalId == animal[x].damId);
                if (damlist != null)
                {
                    //var d_item = new parentdamlv0();


                    var animal_details_dam0 = _context.ABuffAnimals.Where(a => a.Id == animal[x].damId).FirstOrDefault();
                    d_item.breedRegistryNumber = animal_details_dam0.breedRegistryNumber;
                    d_item.Photo = animal_details_dam0.Photo;
                    d_item.AnimalName = animal_details_dam0.AnimalName;
                    d_item.DateOfBirth = animal_details_dam0.DateOfBirth.ToString();
                    d_item.CountryOfBirth = animal_details_dam0.CountryOfBirth.ToString();

                    d_item.Id = damlist.damId;
                    d_item.animalId =animal_details_dam0.AnimalIdNumber;
                    //d_item.sireId = damlist.sireId;
                    //d_item.damId = damlist.damId;
                    d_item.level = 1;
                    var animal_blood_dam0 = _context.ABloodComps.Where(b => b.Id == animal_details_dam0.BloodCode).FirstOrDefault();
                    d_item.breedCode = dbmet.BreedCode(animal_details_dam0.BreedCode);
                    d_item.bloodResult = animal_details_dam0.BloodResult.ToString();
                    d_item.bloodComp = animal_blood_dam0.BloodDesc.ToString();
                    //------------------------------------------- level1 sire 
                    var s_item1 = new parentsirelv1();
                    var sirelistlvl1 = dbmet.getfamily().Find(a => a.animalId == damlist.sireId    );
                    if (sirelistlvl1 != null)
                    {
                        //var s_item1 = new parentsirelv1();

                        var animal_details_sire1 = _context.ABuffAnimals.Where(a => a.Id == damlist.sireId).FirstOrDefault();
                        s_item1.breedRegistryNumber = animal_details_sire1.breedRegistryNumber;
                        s_item1.Photo = animal_details_sire1.Photo;
                        s_item1.AnimalName = animal_details_sire1.AnimalName;
                        s_item1.DateOfBirth = animal_details_sire1.DateOfBirth.ToString();
                        s_item1.CountryOfBirth = animal_details_sire1.CountryOfBirth.ToString();
                        s_item1.Id = sirelistlvl1.animalId;
                        s_item1.animalId = animal_details_sire1.AnimalIdNumber;
                        s_item1.level = 2;
                        var animal_blood_sire1 = _context.ABloodComps.Where(b => b.Id == animal_details_sire1.BloodCode).FirstOrDefault();
                        s_item1.breedCode = dbmet.BreedCode(animal_details_sire1.BreedCode);
                        s_item1.bloodResult = animal_details_sire1.BloodResult.ToString();
                        s_item1.bloodComp = animal_blood_sire1.BloodDesc.ToString();
                        //-----------------------------------level 2 sire
                        var s_item2 = new parentsirelv2();
                        var sirelistlvl2 = dbmet.getfamily().Find(a => a.animalId == sirelistlvl1.sireId);
                        if (sirelistlvl2 != null)
                        {
                            //var s_item2 = new parentsirelv2();

                            var animal_details_sire2 = _context.ABuffAnimals.Where(a => a.Id == sirelistlvl1.sireId).FirstOrDefault();
                            s_item2.breedRegistryNumber = animal_details_sire2.breedRegistryNumber;
                            s_item2.Photo = animal_details_sire2.Photo;
                            s_item2.AnimalName = animal_details_sire2.AnimalName;
                            s_item2.DateOfBirth = animal_details_sire2.DateOfBirth.ToString();
                            s_item2.CountryOfBirth = animal_details_sire2.CountryOfBirth.ToString();

                            s_item2.Id = sirelistlvl2.animalId;
                            s_item2.animalId =animal_details_sire2.AnimalIdNumber;
                            s_item2.level = 3;
                            var animal_blood_sire2 = _context.ABloodComps.Where(b => b.Id == animal_details_sire2.BloodCode).FirstOrDefault();
                            s_item2.breedCode = dbmet.BreedCode(animal_details_sire2.BreedCode);
                            s_item2.bloodResult = animal_details_sire2.BloodResult.ToString();
                            s_item2.bloodComp = animal_blood_sire2.BloodDesc.ToString();
                            //------------------------------------ lvel 3 sire
                            var s_item3 = new parentsirelv3();
                            var sirelistlvl3 = dbmet.getfamily().Find(a => a.animalId == sirelistlvl2.sireId);
                            if (sirelistlvl3 != null)
                            {
                                //var s_item3 = new parentsirelv3();

                                var animal_details_sire3 = _context.ABuffAnimals.Where(a => a.Id == sirelistlvl2.sireId).FirstOrDefault();
                                s_item3.breedRegistryNumber = animal_details_sire3.breedRegistryNumber;
                                s_item3.Photo = animal_details_sire3.Photo;
                                s_item3.AnimalName = animal_details_sire3.AnimalName;
                                s_item3.DateOfBirth = animal_details_sire3.DateOfBirth.ToString();
                                s_item3.CountryOfBirth = animal_details_sire3.CountryOfBirth.ToString();

                                s_item3.Id = sirelistlvl3.animalId;
                                s_item3.animalId =animal_details_sire3.AnimalIdNumber;
                                s_item3.level = 4;
                                var animal_blood_sire3 = _context.ABloodComps.Where(b => b.Id == animal_details_sire3.BloodCode).FirstOrDefault();
                                s_item3.breedCode = dbmet.BreedCode(animal_details_sire3.BreedCode);
                                s_item3.bloodResult = animal_details_sire3.BloodResult.ToString();
                                s_item3.bloodComp = animal_blood_sire3.BloodDesc.ToString();
                                //parentsirelv2.Add(s_item2);

                            }
                            //------------------------------------ lvel 3 dam
                            var d_item3 = new parentdamlv3();
                            if (sirelistlvl3 != null)
                            {
                                var damlistlvl3 = dbmet.getfamily().Find(a => a.animalId == sirelistlvl3.damId );
                                if (damlistlvl3 != null)
                                {
                                    //var d_item3 = new parentdamlv3();

                                    var animal_details_dam3 = _context.ABuffAnimals.Where(a => a.Id == sirelistlvl3.damId).FirstOrDefault();
                                    d_item3.breedRegistryNumber = animal_details_dam3.breedRegistryNumber;
                                    d_item3.Photo = animal_details_dam3.Photo;
                                    d_item3.AnimalName = animal_details_dam3.AnimalName;
                                    d_item3.DateOfBirth = animal_details_dam3.DateOfBirth.ToString();
                                    d_item3.CountryOfBirth = animal_details_dam3.CountryOfBirth.ToString();

                                    d_item3.Id = damlistlvl3.animalId;
                                    d_item3.animalId =animal_details_dam3.AnimalIdNumber;
                                    d_item3.level = 4;
                                    var animal_blood_dam3 = _context.ABloodComps.Where(b => b.Id == animal_details_dam3.BloodCode).FirstOrDefault();
                                    d_item3.breedCode = dbmet.BreedCode(animal_details_dam3.BreedCode);
                                    d_item3.bloodResult = animal_details_dam3.BloodResult.ToString();
                                    d_item3.bloodComp = animal_blood_dam3.BloodDesc.ToString();
                                    //parentdamlv3.Add(d_item3);
                                }
                            }
                            s_item2.parentdamlv3 = d_item3;
                            s_item2.parentsirelv3 = s_item3;


                            //parentsirelv2.Add(s_item2);

                        }

                        //------------------------------------ lvel 2 dam
                        var d_item2 = new parentdamlv2();

                        if (sirelistlvl1 != null)
                        {
                            var damlistlvl2 = dbmet.getfamily().Find(a => a.animalId == sirelistlvl1.damId );
                            if (damlistlvl2 != null)
                            {
                                //var d_item2 = new parentdamlv2();

                                var animal_details_dam2 = _context.ABuffAnimals.Where(a => a.Id == sirelistlvl1.damId).FirstOrDefault();
                                d_item2.breedRegistryNumber = animal_details_dam2.breedRegistryNumber;
                                d_item2.Photo = animal_details_dam2.Photo;
                                d_item2.AnimalName = animal_details_dam2.AnimalName;
                                d_item2.DateOfBirth = animal_details_dam2.DateOfBirth.ToString();
                                d_item2.CountryOfBirth = animal_details_dam2.CountryOfBirth.ToString();

                                d_item2.Id = damlistlvl2.animalId;
                                d_item2.animalId = animal_details_dam2.AnimalIdNumber;
                                d_item2.level = 3;
                                var animal_blood_dam2 = _context.ABloodComps.Where(b => b.Id == animal_details_dam2.BloodCode).FirstOrDefault();
                                d_item2.breedCode = dbmet.BreedCode(animal_details_dam2.BreedCode);
                                d_item2.bloodResult = animal_details_dam2.BloodResult.ToString();
                                d_item2.bloodComp = animal_blood_dam2.BloodDesc.ToString();
                                //parentdamlv2.Add(d_item2);
                            }

                        }
                        s_item1.parentdamlv2 = d_item2;
                        s_item1.parentsirelv2 = s_item2;

                        //parentsirelv1.Add(s_item1);

                    }

                    //---------------------------------------------level 1 dam
                    var d_item1 = new parentdamlv1();
                    var damlistlvl1 = dbmet.getfamily().Find(a => a.animalId == damlist.damId ); // here 1
                    if (damlistlvl1 != null)
                    {
                        //var d_item1 = new parentdamlv1();


                        var animal_details_dam1 = _context.ABuffAnimals.Where(a => a.Id == damlist.damId).FirstOrDefault();
                        d_item1.breedRegistryNumber = animal_details_dam1.breedRegistryNumber;
                        d_item1.Photo = animal_details_dam1.Photo;
                        d_item1.AnimalName = animal_details_dam1.AnimalName;
                        d_item1.DateOfBirth = animal_details_dam1.DateOfBirth.ToString();
                        d_item1.CountryOfBirth = animal_details_dam1.CountryOfBirth.ToString();

                        d_item1.Id = damlistlvl1.animalId;
                        d_item1.animalId = animal_details_dam1.AnimalIdNumber;
                        d_item1.level = 2;
                        var animal_blood_dam1 = _context.ABloodComps.Where(b => b.Id == animal_details_dam1.BloodCode).FirstOrDefault();
                        item.breedCode = dbmet.BreedCode(animal_details_dam1.BreedCode);
                        item.bloodResult = animal_details_dam1.BloodResult.ToString();
                        item.bloodComp = animal_blood_dam1.BloodDesc.ToString();
                        var s_item2 = new parentsirelv2();
                        var sirelistlvl2 = dbmet.getfamily().Find(a => a.animalId == damlistlvl1.sireId);
                        if (sirelistlvl2 != null)
                        {
                            //var s_item2 = new parentsirelv2();

                            var animal_details_sire1 = _context.ABuffAnimals.Where(a => a.Id == damlistlvl1.sireId).FirstOrDefault();
                            s_item2.breedRegistryNumber = animal_details_sire1.breedRegistryNumber;
                            s_item2.Photo = animal_details_sire1.Photo;
                            s_item2.AnimalName = animal_details_sire1.AnimalName;
                            s_item2.DateOfBirth = animal_details_sire1.DateOfBirth.ToString();
                            s_item2.CountryOfBirth = animal_details_sire1.CountryOfBirth.ToString();
                            s_item2.Id = sirelistlvl2.animalId;
                            s_item2.animalId = animal_details_sire1.AnimalIdNumber;
                            s_item2.level = 3;
                            var animal_blood_sire1 = _context.ABloodComps.Where(b => b.Id == animal_details_sire1.BloodCode).FirstOrDefault();
                            s_item2.breedCode = dbmet.BreedCode(animal_details_sire1.BreedCode);
                            s_item2.bloodResult = animal_details_sire1.BloodResult.ToString();
                            s_item2.bloodComp = animal_blood_sire1.BloodDesc.ToString();

                        }
                        var d_item2 = new parentdamlv2();
                        if (sirelistlvl2 != null)
                        {
                            var damlistlvl2 = dbmet.getfamily().Find(a => a.animalId == damlistlvl1.damId );
                            if (damlistlvl2 != null)
                            {
                                //var d_item2 = new parentdamlv2();

                                var animal_details_sire2 = _context.ABuffAnimals.Where(a => a.Id == damlistlvl1.damId).FirstOrDefault();
                                d_item2.breedRegistryNumber = animal_details_sire2.breedRegistryNumber;
                                d_item2.Photo = animal_details_sire2.Photo;
                                d_item2.AnimalName = animal_details_sire2.AnimalName;
                                d_item2.DateOfBirth = animal_details_sire2.DateOfBirth.ToString();
                                d_item2.CountryOfBirth = animal_details_sire2.CountryOfBirth.ToString();
                                d_item2.Id = damlistlvl2.animalId;
                                d_item2.animalId = animal_details_sire2.AnimalIdNumber;
                                d_item2.level = 3;
                                var animal_blood_sire2 = _context.ABloodComps.Where(b => b.Id == animal_details_sire2.BloodCode).FirstOrDefault();
                                d_item2.breedCode = dbmet.BreedCode(animal_details_sire2.BreedCode);
                                d_item2.bloodResult = animal_details_sire2.BloodResult.ToString();
                                d_item2.bloodComp = animal_blood_sire2.BloodDesc.ToString();
                            }
                        }

                        d_item1.parentdamlv2 = d_item2;
                        d_item1.parentsirelv2 = s_item2;

                        //parentdamlv1.Add(d_item1);


                    }
                    d_item.parentsire = s_item1;
                    d_item.parentdam = d_item1;



                    //parentdam.Add(d_item);
                }
                item.parentdamlv0 = d_item;
                item.parentsirelv0 = s_item;


            }

            return item;
        }
    }
}
