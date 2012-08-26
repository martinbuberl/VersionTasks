using VersionTasks.Exceptions;

namespace VersionTasks
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
                string output = ExecuteCommand("hg.exe", "identify -i --debug")[0];

                // 19b6fbd0b6102f669ed451de21b5e14a
                Changeset = output.Substring(0, 40);
                // 19b6fbd0b610
                ChangesetShort = output.Substring(0, 12);
                // 0 if false, 1 if true
                DirtyBuild = output.Contains("+") ? 1 : 0;
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
