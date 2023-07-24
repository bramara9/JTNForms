using Humanizer;
using Microsoft.CodeAnalysis.Differencing;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JTNForms.Models
{
    public class ExistingProvider
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Office Practice Contact Person")]
        public string OfficePracticeContactPerson { get; set; }

        [Required]
        [Display(Name = "Phone")]
        public string Phone { get; set; }


        [Display(Name = "Fax Completed Form")]
        public string FaxCompletedForm { get; set; }
        [Required]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        [Display(Name = "Provider Name(s)")]
        public string ProviderName { get; set; }
        [Display(Name = "NPI")]
        public string NPI { get; set; }

        [Display(Name = "Group Practice Name")]
        public string GroupPracticeName { get; set; }
        [Display(Name = "Tax Identification Number")]
        public string TaxIdentificationNumber { get; set; }
        [Display(Name = "Effective Date with Practice")]
        [DataType(DataType.Date)]
        public DateTime? EffectiveDateWithPractice { get; set; }
        [Display(Name = "Accepting New")]
        public string AcceptingNew { get; set; }

        [Display(Name = "Closed to New as of")]
        public string ClosedtoNewAsOf { get; set; }

        [Display(Name = "New Group/Practice Name")]
        public string PracticeGroupName { get; set; }

        [Display(Name = "New Tax ID Number")]
        public string NewTaxIDNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Effective Date")]
        public DateTime? NewEffectiveDate { get; set; }

        [Display(Name = "Delete Tax ID Number")]

        public string DeleteTaxIDNumber { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Effective Date")]
        public DateTime? FirstEffectiveDate { get; set; }
        [Display(Name = "Add 2nd Tax ID Number")]
        public string SecondTaxIDNumber { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Effective Date")]
        public DateTime? SecondEffectiveDate { get; set; }



        [Display(Name = "Is Delete All Previous Addresses")]
        public bool IsDeleteAllPrevAdd { get; set; }
        [Display(Name = "Is Delete Only This Address(es)")]
        
        public bool IsDeleteOnlyThisAddress { get; set; } 


        [Display(Name = "Remit to address")]
        public string RemitToAddress { get; set; }
        [Display(Name = "Is Provider Directory")]
        public bool IsProviderDirectory { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Effective")]
        public DateTime? Effective { get; set; }
        [Display(Name = "Previous Phone No")]
        public string PrevPhone{ get; set; }

        [Display(Name = "Fax")]
        public string Fax { get; set; }
        [Display(Name = "1st Physical Address")]
        public string FirstPhysicalAdd { get; set; }
        [Display(Name = "Is 1st Physical Provider Directory")]
        public bool IsFirstProviderDirectory { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "1st Effective")]
        public DateTime? FirstEffective { get; set; }
        [Display(Name = "1st Previous Phone No")]
        public string FirstPrevPhone { get; set; }

        [Display(Name = "1st Fax")]
        public string FirstFax { get; set; }
        [Display(Name = "2st Physical Address")]
        public string SecondhysicalAdd { get; set; }
        [Display(Name = "Is 2st Physical Provider Directory")]
        public bool IsSecondProviderDirectory { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "2st Effective")]
        public DateTime? SecondEffective { get; set; }
        [Display(Name = "2st Previous Phone No")]
        public string SecondPrevPhone { get; set; }

        [Display(Name = "2st Fax")]
        public string SecondFax { get; set; }

    }
}
