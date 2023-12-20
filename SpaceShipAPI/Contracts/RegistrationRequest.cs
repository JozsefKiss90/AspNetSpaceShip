using System.ComponentModel.DataAnnotations;

namespace SpaceshipAPI.Contracts;

public record RegistrationRequest(
    [Required]string Email, 
    [Required]string Username, 
    [Required]string Password);