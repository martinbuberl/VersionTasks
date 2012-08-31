using VersionTasks.Exceptions;

namespace VersionTasks
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
                // 89f104a77996c8011e49e37b61f2d23599cedac1
                Changeset = ExecuteCommand("git.exe", "rev-parse --verify HEAD")[0];
                // 89f104a779
                ChangesetShort = Changeset.Substring(0, 10);
                // true/false
                DirtyBuild = ExecuteCommand("git.exe", "diff").Count > 0;
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
