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
        public IList<Window> windows { get; set; }
        public IList<RoomDetails> lstRoomDetails { get;  set; }
        public IList<Room> rooms { get;  set; }
    }

    public class WindowDetails {
        public string WindowName  { get; set; }
        public decimal Weight { get; set; }
        public Decimal Height { get; set; }
        public string Control { get; set; }
        public string Option { get; set; }
        public string RoomName { get; set; }

        public int MeasurementId { get; set; }

    }

    public class RoomDetails
    {
        public string RoomName { get; set; }
        public Decimal Fabric { get; set; }
        public Decimal BasePrice { get; set; }
        public string Notes { get; set; }

    }



}
