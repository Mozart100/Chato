namespace Chato.Server.Infrastracture.Exceptions
{
    public class NoUserFoundException : Exception
    {
        public NoUserFoundException(string message):base(message) { }
    }
}
