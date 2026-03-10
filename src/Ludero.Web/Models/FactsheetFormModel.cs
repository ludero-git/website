using System.ComponentModel.DataAnnotations;

namespace Ludero.Web.Models;

public class FactsheetFormModel
{
    [Required(ErrorMessage = "Naam is verplicht")]
    [Display(Name = "Naam")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-mailadres is verplicht")]
    [EmailAddress(ErrorMessage = "Ongeldig e-mailadres")]
    [Display(Name = "E-mailadres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefoonnummer is verplicht")]
    [Phone(ErrorMessage = "Ongeldig telefoonnummer")]
    [Display(Name = "Telefoonnummer")]
    public string Phone { get; set; } = string.Empty;

    [Display(Name = "Bedrijf")]
    public string? Company { get; set; }
}
