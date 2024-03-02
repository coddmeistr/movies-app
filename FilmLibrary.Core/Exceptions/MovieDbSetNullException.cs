using System.Runtime.Serialization;

namespace FilmLibrary.Core.Exceptions
{
    [Serializable]
    public class MovieDbSetNullException : ArgumentException
    {
        public MovieDbSetNullException(string message) : base(message) { }

        protected MovieDbSetNullException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}