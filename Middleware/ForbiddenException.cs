namespace Demo_Course_Management.Middleware
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message)
            : base(message)
        {

        }
    
    }
}
