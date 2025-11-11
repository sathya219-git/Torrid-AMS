namespace Incident.Application.Exceptions
{
    public class HeaderValidationException : Exception
    { public HeaderValidationException(string m) : base(m) { } }



    public class FileTooLargeException : Exception
    { public FileTooLargeException(string m) : base(m) { } }



    public class UnsupportedFormatException : Exception
    { public UnsupportedFormatException(string m) : base(m) { } }

}