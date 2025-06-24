using System;

namespace Sep490_Eduseen_BE.Exceptions
{
    public class CourseNotFoundException : Exception
    {
        public CourseNotFoundException() : base() { }
        public CourseNotFoundException(string message) : base(message) { }
        public CourseNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}