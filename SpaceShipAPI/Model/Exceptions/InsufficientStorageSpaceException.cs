namespace SpaceShipAPI.Model.Exceptions;

public class InsufficientStorageSpaceException : Exception
{
    public InsufficientStorageSpaceException()
        : base("Not enough storage space.")
    {
    }
}
