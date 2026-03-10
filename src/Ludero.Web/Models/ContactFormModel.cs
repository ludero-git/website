using System.ComponentModel.DataAnnotations;

namespace Ludero.Web.Models;

public class ContactFormModel
{
    [Required(ErrorMessage = "Naam is verplicht")]
    [Display(Name = "Naam")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Bedrijf")]
    public string? Company { get; set; }

    [Required(ErrorMessage = "E-mailadres is verplicht")]
    [EmailAddress(ErrorMessage = "Ongeldig e-mailadres")]
    [Display(Name = "E-mailadres")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Ongeldig telefoonnummer")]
    [Display(Name = "Telefoonnummer")]
    public string? Phone { get; set; }

    [Display(Name = "Onderwerp")]
    public string? Subject { get; set; }

    [Required(ErrorMessage = "Bericht is verplicht")]
    [Display(Name = "Bericht")]
    public string Message { get; set; } = string.Empty;
}
