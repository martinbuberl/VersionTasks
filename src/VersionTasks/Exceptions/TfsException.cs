using System;

namespace VersionTasks.Exceptions
{
    [Serializable]
    public class TfsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the TfsException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public TfsException(string message)
            : base(message)
        {
        }
    }
}
