using System;

namespace MSBuild.Version.Tasks.Exceptions
{
    [Serializable]
    public class HgException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the HgException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public HgException(string message)
            : base(message)
        {
        }
    }
}
