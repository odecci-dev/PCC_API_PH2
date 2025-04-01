namespace API_PCC.ApplicationModels
{
    public class HerdClassificationBaseModel
    {
        public string HerdClassCode { get; set; }
        public string HerdClassDesc { get; set; }
        public string? LevelFrom { get; set; }
        public string? LevelTo { get; set; }
    }
}
