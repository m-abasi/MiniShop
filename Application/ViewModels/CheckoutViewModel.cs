using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels;

public class CheckoutViewModel
{
    [Required, MaxLength(300)]
    [Display(Name = "Street Address")]
    public string ShippingAddress { get; set; } = null!;

    [Required, MaxLength(100)]
    [Display(Name = "City")]
    public string ShippingCity { get; set; } = null!;

    [Required, MaxLength(20)]
    [Display(Name = "ZIP / Postal Code")]
    public string ShippingZip { get; set; } = null!;
}
