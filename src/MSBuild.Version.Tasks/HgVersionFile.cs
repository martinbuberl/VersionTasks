using System;
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
                string revision = ExecuteCommand("hg.exe", "identify -n")[0];

                if (revision.Contains("+"))
                {
                    Revision = int.Parse(revision.Substring(0, revision.IndexOf("+", StringComparison.Ordinal)));
                    Dirty = 1;
                }
                else
                {
                    Revision = int.Parse(revision);
                    Dirty = 0;
                }

                Changeset = ExecuteCommand("hg.exe", "identify -i")[0].Substring(0, 12);
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
