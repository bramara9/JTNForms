using System.ComponentModel.DataAnnotations;

namespace JTNForms.Models
{
    public class FabricModel
    {
        public int? Id { get; set; }
        [Required]
        public int BasicPrice { get; set; }
        [Required]
        public string FabricName { get; set; }
        [Required]
        public string CatalogName { get; set; }
        public IFormFile? File { get; set; }
        public string? FileName { get; set; }
    }

}
