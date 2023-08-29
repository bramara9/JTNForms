using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace JTNForms.Models
{
    public class IssuesModel
    {
        public IssuesModel()
        {
            lstWindowsName = new List<SelectListItem>();
        }
        public int RepairId { get; set; }
        [Required]
        public int WindowId { get; set; }
        [Required]
        public string WindowName { get; set; }
        public string? Notes { get; set; }

        public int IndexVal { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public string? IssueType { get; set; }

        public List<SelectListItem> lstWindowsName { get; set; }
    }
}
