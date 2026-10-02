using System.ComponentModel.DataAnnotations;

namespace Ludero.Web.Models;

public class PartnerFormModel
{
    [Required(ErrorMessage = "Naam is verplicht")]
    [Display(Name = "Naam")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Bedrijf is verplicht")]
    [Display(Name = "Bedrijf")]
    public string Company { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-mailadres is verplicht")]
    [EmailAddress(ErrorMessage = "Ongeldig e-mailadres")]
    [Display(Name = "E-mailadres")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Ongeldig telefoonnummer")]
    [Display(Name = "Telefoonnummer")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Kies minimaal één focusgebied")]
    [MinLength(1, ErrorMessage = "Kies minimaal één focusgebied")]
    [Display(Name = "Waar ligt jullie focus?")]
    public List<string> FocusAreas { get; set; } = [];
}
