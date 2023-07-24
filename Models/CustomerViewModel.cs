using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace JTNForms.Models
{
    public class CustomerViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string State { get; set; }

        public CustomerStatus CustomerStatus { get; set; }

    }

    public enum CustomerStatus {
        contacted,
        AppointmentScheduled,
        MeasurementScheduled,
        SendInvoice,
        PaymentDone,
        PlaceOrder,
        OrderShipped,
        InstallationScheduled
        }

}
