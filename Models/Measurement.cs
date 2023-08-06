using JTNForms.Custom;
using JTNForms.DataModels;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JTNForms.Models
{
    public class Measurement
    {
        public Measurement()
        {
        }
        [Display(Name ="Select Inch or MM")]
        public bool? IsInchOrMM { get; set; }
        public int customerId { get; set; }
        public IList<RWDetails> lstWindowDetails { get; set; }
    }

    public class RWDetails {
        public int RoomId { get; set; }
        public int WindowId { get; set; }
        [Required]
        public string WindowName { get; set; }
        [Required]
        [Range(0.01, 99999999)]
        public decimal? Width { get; set; }
        [Required]
        [Range(0.01, 99999999)]
        public Decimal? Height { get; set; }
        public string? Notes { get; set; }
        [Required]
        public string RoomName { get; set; }

        public int IndexVal { get; set; }
    }

    public class WindowDetails {
        public WindowDetails()
        {
            ControlTypes = new List<SelectListItem>();
            ControlPositions = new List<SelectListItem>();
            StackTypes = new List<SelectListItem>();
        }
        public int Id { get; set; }
        [Required]
        public string WindowName  { get; set; }
        [Required]
        [Range(0.01, 99999999)]
        public decimal? Width { get; set; }
        [Required]
        [Range(0.01, 99999999)]
        public Decimal? Height { get; set; }
        public string? Notes { get; set; }
        [Required]
        public string RoomName { get; set; }

        public int IndexVal { get; set; }
        public int? MeasurementId { get; set; }
        public string? ControlType { get; set; }
        public string? ControlPosition { get; set; }
        public string? StackType { get; set; }
        [Range(0.01, 99999999)]
        public Decimal? TotalPrice { get; set; }

        public bool Is2In1 { get; set; }
        public bool IsNoValance { get; set; }
        public bool  IsItemSelection { get; set; }
        public bool IsNeedExtension { get; set; }
        public Int32 NoOfPanels { get; set; }
        public List<SelectListItem> ControlTypes { get; set; }
        public List<SelectListItem> StackTypes { get; set; }
        public List<SelectListItem> ControlPositions { get; set; }
    }

    public class RoomDetails
    {
        public RoomDetails()
        {
            WindowDetails = new List<WindowDetails>();
            BlindTypes = new List<SelectListItem>();
        }
        public int Id { get; set; }
        public string RoomName { get; set; }
        public string Fabric { get; set; }
        public int? BasePrice { get; set; }
        public string Notes { get; set; }

        public string BlindType { get; set; }
        public List<SelectListItem> BlindTypes { get; set; }

        public List<WindowDetails>  WindowDetails { get; set; }


    }



}
