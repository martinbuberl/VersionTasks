using System;

namespace MSBuild.Version.Tasks.Exceptions
{
    [Serializable]
    public class GitException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the GitException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public GitException(string message)
            : base(message)
        {
        }
    }
}
