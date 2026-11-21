using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LedgerPro.Models.ViewModels;

/// <summary>Record a Sale (customer) or a Purchase (supplier).</summary>
public class TransactionFormViewModel
{
    public string TransactionType { get; set; } = "Sale"; // "Sale" or "Purchase"

    [Required(ErrorMessage = "Please choose a party.")]
    public int? PartyId { get; set; }

    [Required, Range(0.01, 999999999, ErrorMessage = "Enter an amount greater than zero.")]
    public decimal? Amount { get; set; }

    [Required, DataType(DataType.Date)]
    public DateTime? TransactionDate { get; set; } = DateTime.Today;

    [StringLength(500)]
    public string? Description { get; set; }

    [BindNever, ValidateNever]
    public List<SelectListItem> Parties { get; set; } = new();
}

/// <summary>Record money received from a customer or paid to a supplier.</summary>
public class PaymentFormViewModel
{
    public string PaymentType { get; set; } = "Received"; // "Received" (customer) or "Paid" (supplier)

    [Required(ErrorMessage = "Please choose a party.")]
    public int? PartyId { get; set; }

    [Required, Range(0.01, 999999999, ErrorMessage = "Enter an amount greater than zero.")]
    public decimal? Amount { get; set; }

    [Required, DataType(DataType.Date)]
    public DateTime? PaymentDate { get; set; } = DateTime.Today;

    [StringLength(500)]
    public string? Description { get; set; }

    [BindNever, ValidateNever]
    public List<SelectListItem> Parties { get; set; } = new();
}
