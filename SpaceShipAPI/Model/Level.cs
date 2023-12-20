using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SpaceShipAPI;
using SpaceShipAPI.Model;

[Table("Levels")]
public class Level
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Column("Type")]
    [EnumDataType(typeof(UpgradeableType))]
    public UpgradeableType Type { get; set; }

    [Column("Level")]
    public int LevelValue { get; set; }

    [Column("Effect")]
    public int Effect { get; set; }

    [Column("IsMax")]
    public bool Max { get; set; }

    public virtual ICollection<LevelCost> Costs { get; set; }

    public Level() 
    {
        Costs = new HashSet<LevelCost>();
    }
}

public class LevelCost
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey("Level")]
    public long LevelId { get; set; }
    public virtual Level Level { get; set; }

    public ResourceType Resource { get; set; }
    public int Amount { get; set; }
}