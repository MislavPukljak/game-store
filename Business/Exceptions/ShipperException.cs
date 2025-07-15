namespace Business.Exceptions;

public class ShipperException : Exception
{
    public ShipperException(string message)
        : base(message)
    {
    }
}
