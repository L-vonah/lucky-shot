namespace LuckShot.Infrastructure.Exceptions;

public class RepositoryException : Exception
{
    protected RepositoryException(string message) : base(message) { }
    public RepositoryException(string message, Exception innerException) : base(message, innerException) { }
}

public class RepositoryConcurrencyException : RepositoryException
{
    public RepositoryConcurrencyException(string message) : base(message) { }
    public RepositoryConcurrencyException(string message, Exception innerException) : base(message, innerException) { }
}