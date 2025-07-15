namespace Business.Exceptions;

public class PlatformException : Exception
{
    public PlatformException(string message)
        : base(message)
    {
    }
}
