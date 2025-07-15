namespace Business.Exceptions;

public class CustomerException : Exception
{
    public CustomerException(string message)
        : base(message)
    {
    }
}
