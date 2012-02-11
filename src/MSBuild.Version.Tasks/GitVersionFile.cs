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
                // InitRevision
                var bla = ExecuteCommand("git.exe", "rev-list");
                // IsWorkingCopyDirty
                var foo = ExecuteCommand("git.exe", "diff-index --quiet HEAD");
                // GetBranch
                var branch = ExecuteCommand("git.exe", "describe --all")[0];
                // GetTags
                var tags = ExecuteCommand("git.exe", "describe")[0];
            }
            catch (ExecuteCommandException ex)
            {
                
                throw;
            }
        }
    }
}
