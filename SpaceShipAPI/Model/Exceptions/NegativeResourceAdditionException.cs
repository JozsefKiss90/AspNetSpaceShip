namespace SpaceShipAPI.Model.Exceptions;

public class NegativeResourceAdditionException : Exception
{
    public NegativeResourceAdditionException() 
        : base("Can't add negative resources.")
    {
    }
}
