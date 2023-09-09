using System.ComponentModel.DataAnnotations;

namespace JTNForms.Models
{
    public class FabricModel
    {
        
        public int? Id { get; set; }
        [Required]
        public string FabricType { get; set; }
        [Required]
        public string FabricName { get; set; }
        [Required]
        public string FabricCode { get; set; }
        [Required]
        public string CatalogName { get; set; }

        
        public IFormFile? File { get; set; }
        public string? FileName { get; set; }
        public List<SkuData>? lstSkuData { get; set; }
        public Byte[]? FileBytes { get; set; }
    }

    public class SkuData {
        public string? BlindType { get; set; }

        public int? Price { get; set; }
    }


}
