using System.Collections;
using System.Net;
using System.Reflection;
using MSBuild.Version.Tasks.Exceptions;
using MSBuild.Version.Tasks.Tfs.Proxies;

namespace MSBuild.Version.Tasks
{
    /// <summary>
    /// This is a modified fork of the TfsVersion task of the MSBuild.Community.Tasks project.
    /// </summary>
    /// <remarks>
    /// This was necessary because we use TeamCity as our build server. TeamCity runs into an error if it does not find a local TFS
    /// workspace. Therefore a different exception handling implementation was necessary. Includes all bugfixes up to the version
    /// '1.3.0.511' (11/30/2010).
    /// </remarks>
    /// <see href="http://msbuildtasks.tigris.org/"/>
    public class TfsVersionFile : VersionInfoBase
    {
        internal override void SetVersionInfo()
        {
            try
            {
                // TODO: Eplain why TFS changeset is a revision
                Revision = GetChangeset(WorkingDirectory);
            }
            catch (TfsException ex)
            {
                // TeamCity build server check outs are not associated with a local TFS workspace, so we will get the
                // error 'The local path is not associated with a TFS Workspace'. We want to swallow this exception
                // here so the $CHANGESET$ token will be still initialized with it default value '0'.
                Log.LogWarningFromException(ex);
            }
        }

        private static int GetChangeset(string workingDirectory)
        {
            Assembly clientAssembly = Assembly.Load("Microsoft.TeamFoundation.Client, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            Assembly versionAssembly = Assembly.Load("Microsoft.TeamFoundation.VersionControl.Client, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");

            int changesetId = 0;

            VersionControlServer sourceControl = new VersionControlServer(
                clientAssembly,
                versionAssembly,
                new Workstation(versionAssembly).GetLocalWorkspaceInfo(workingDirectory).ServerUri.ToString(),
                CredentialCache.DefaultNetworkCredentials
                );

            WorkspaceVersionSpec workspaceVersionSpec = new WorkspaceVersionSpec(
                versionAssembly,
                sourceControl.GetWorkspace(workingDirectory)
                );

            IEnumerable history = sourceControl.QueryHistory(
                workingDirectory,
                new VersionSpec(versionAssembly).Latest,
                new RecursionType(versionAssembly).Full,
                workspaceVersionSpec
                );

            IEnumerator historyEnumerator = history.GetEnumerator();
            Changeset changeset = new Changeset(versionAssembly);

            if (historyEnumerator.MoveNext())
                changeset = new Changeset(versionAssembly, historyEnumerator.Current);

            if (changeset.Instance != null)
                changesetId = changeset.ChangesetId;

            return changesetId;
        }
    }
}
