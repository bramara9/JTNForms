using JTNForms.Custom;
using JTNForms.DataModels;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace JTNForms.Models
{
    public class Measurement
    {
        public Measurement()
        {
        }
        [Display(Name ="Select Inch or MM")]
        public bool IsInchOrMM { get; set; }
        public int customerId { get; set; }
        public IList<WindowDetails> lstWindowDetails { get; set; }
        //public IList<Window> windows { get; set; }
        //public IList<RoomDetails> lstRoomDetails { get;  set; }
        //public IList<Room> rooms { get;  set; }
    }

    public class WindowDetails {

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

    }

    public class RoomDetails
    {
        public string RoomName { get; set; }
        public Decimal Fabric { get; set; }
        public Decimal BasePrice { get; set; }
        public string Notes { get; set; }

    }



}
