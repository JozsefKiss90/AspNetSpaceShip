using SpaceshipAPI.Spaceship.Model.Station;

namespace SpaceshipAPI;

using Microsoft.AspNetCore.Identity; 
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class UserEntity : IdentityUser
{
   
    public int? SpaceStationId { get; set; } // Foreign key, nullable
    public virtual SpaceStation SpaceStation { get; set; } // Navigation property
    
    //public Role Role { get; set; }
}
