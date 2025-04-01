using Org.BouncyCastle.Asn1.Crmf;

namespace API_PCC.Models
{
    public partial class TblMailSenderCredential
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime DateCreated { get; set; }
        public int Status { get; set; }
        public DateTime ExpiryDate { get; set; }

    }
}
