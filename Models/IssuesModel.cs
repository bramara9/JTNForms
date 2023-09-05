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
        public decimal? RepairId { get; set; }
        [Required]
        public int WindowId { get; set; }
        public string? WindowName { get; set; }
        public string? Notes { get; set; }

        public int IndexVal { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public string? IssueType { get; set; }

        public List<SelectListItem> lstWindowsName { get; set; }
    }

    public class AllIssues {
        public string? RoomName { get; set; }
        public string? WindowName { get; set; }
        public string? Notes { get; set; }

        public int CustomerId { get; set; }
        public string? Description { get; set; }
        public string? CustomerName { get; set; }
    }

}
