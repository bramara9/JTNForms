using JTNForms.Custom;
using JTNForms.DataModels;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace JTNForms.Models
{
    public class Measurement
    {
        public Measurement()
        {
            lstWindowDetails = new List<WindowDetails>();
        }
        [Display(Name = "Select Inch or MM")]
        public bool? IsInchOrMM { get; set; }
        public int customerId { get; set; }
        public IList<WindowDetails> lstWindowDetails { get; set; }
    }

    public class WindowDetails
    {
        public WindowDetails()
        {
            lstFabricNames = new List<SelectListItem>();
        }
        public int Id { get; set; }
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
        public int? MeasurementId { get; set; }

        public string? ControlType { get; set; }
        public string? ControlPosition { get; set; }
        public string? StackType { get; set; }
        [Range(0.01, 99999999)]
        public Decimal? TotalPrice { get; set; }

        public bool Is2In1 { get; set; } = false;
        public bool IsNoValance { get; set; } = false;
        public bool IsItemSelection { get; set; } = true;
        public bool IsNeedExtension { get; set; } = false;
        public Int32 NoOfPanels { get; set; }
        public int? BasePrice { get; set; }
        public decimal? PricePerItem { get; set; }

        public string? FabricName { get; set; }
        public string? BlindType { get; set; }
        [Range(0.01, 99999999)]
        public double? OrderedWidth { get; set; }
        [Range(0.01, 99999999)]
        public double? OrderedHeight { get; set; }
        [Range(0.01, 99999999)]
        public double? Area { get; set; }
        public int? CordlessOrMotorPrice { get; set; }
        public string? CatalogType { get; set; }
        public List<SelectListItem>? lstFabricNames { get; set; }
        
    }

    public class RoomDetails
    {
        public RoomDetails()
        {
            WindowDetails = new List<WindowDetails>();
        }
        public string RoomName { get; set; }
        public string Fabric { get; set; }
        public int? BasePrice { get; set; }
        public string Notes { get; set; }

        public string BlindType { get; set; }

        public List<WindowDetails> WindowDetails { get; set; }


    }



}
