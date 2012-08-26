using System;

namespace VersionTasks.Exceptions
{
    [Serializable]
    public class ExecuteCommandException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ExecuteCommandException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ExecuteCommandException(string message)
            : base(message)
        {
        }
    }
}
