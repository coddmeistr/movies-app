using System.Runtime.Serialization;

namespace FilmLibrary.Core.Exceptions
{
    [Serializable]
    public class UserDbSetNullException : ArgumentException
    {
        public UserDbSetNullException(string message) : base(message) { }

        protected UserDbSetNullException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}