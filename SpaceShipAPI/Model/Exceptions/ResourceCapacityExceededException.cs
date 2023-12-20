namespace SpaceShipAPI.Model.Exceptions;

public class ResourceCapacityExceededException : Exception
{
    public ResourceCapacityExceededException(string message) : base(message)
    {
    }
}
