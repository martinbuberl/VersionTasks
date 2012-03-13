using MSBuild.Version.Tasks.Exceptions;

namespace MSBuild.Version.Tasks
{
    /// <remarks>
    /// This is a heavily modified fork of the corresponding task in the MSBuild Versioning project <see href="http://versioning.codeplex.com/" />.
    /// This was necessary because we use TeamCity as our build server. TeamCity checks out the source code without the '.git' folder, which would
    /// lead to an exception. Therefore a different exception handling implementation was necessary.
    /// </remarks>
    public class GitVersionFile : VersionInfoBase
    {
        internal override void SetVersionInfo()
        {
            try
            {
                Changeset = ExecuteCommand("git.exe", "log --pretty=format:'%H' -n 1")[0].Substring(1, 40); ;
                Dirty = ExecuteCommand("git.exe", "diff-index HEAD").Count > 0 ? 1 : 0;
            }
            catch (ExecuteCommandException ex)
            {
                // TeamCity does not check out the directory with a .git folder, so we will get the error 'Not a git repository (or any of the
                // parent directories)'. We want to swallow the exception so the tokens will be still initialized with their default values.
                Log.LogWarningFromException(ex);
            }
        }
    }
}
