using SpaceshipAPI;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SpaceshipAPI.Model.Ship;

namespace SpaceShipAPI.Model.Mission;
public abstract class Mission
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime CurrentObjectiveTime { get; set; }
    public DateTime ApproxEndTime { get; set; }
    
    [Column(TypeName = "varchar(255)")]
    public MissionStatus CurrentStatus { get; set; }
    public long TravelDurationInSecs { get; set; }
    public long ActivityDurationInSecs { get; set; }
    
    public SpaceShip Ship { get; set; }
    
    [ForeignKey("UserId")]
    public string? UserId { get; set; }
    public UserEntity User { get; set; }
    
    public List<Event> Events { get; set; }
}
