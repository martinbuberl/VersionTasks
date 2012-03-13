using MSBuild.Version.Tasks.Exceptions;

namespace MSBuild.Version.Tasks
{
    /// <remarks>
    /// This is a heavily modified fork of the corresponding task in the MSBuild Versioning project <see href="http://versioning.codeplex.com/" />.
    /// This was necessary because we use TeamCity as our build server. TeamCity checks out the source code without the '.hg' folder, which would
    /// lead to an exception. Therefore a different exception handling implementation was necessary.
    /// </remarks>
    public class HgVersionFile : VersionInfoBase
    {
        internal override void SetVersionInfo()
        {
            try
            {
                string changeset = ExecuteCommand("hg.exe", "identify -i")[0]; // -i (global revision id); -n (local revision number)

                Dirty = changeset.Contains("+") ? 1 : 0;
                Changeset = changeset.Substring(0, 12);
            }
            catch (ExecuteCommandException ex)
            {
                // TeamCity does not check out the directory with a .hg folder, so we will get the error 'There is no Mercurial repository here
                // (.hg not found)'. We want to swallow the exception so the tokens will be still initialized with their default values.
                Log.LogWarningFromException(ex);
            }
        }
    }
}
