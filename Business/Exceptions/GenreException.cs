namespace Business.Exceptions;

public class GenreException : Exception
{
    public GenreException(string message)
        : base(message)
    {
    }
}