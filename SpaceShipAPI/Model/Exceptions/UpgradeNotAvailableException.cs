namespace SpaceShipAPI.Model.Exceptions;

public class UpgradeNotAvailableException : Exception
{
    public UpgradeNotAvailableException(string message) : base(message)
    {
    }
}
