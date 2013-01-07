using System;
using System.Collections;
using System.Net;
using System.Reflection;
using VersionTasks.Exceptions;
using VersionTasks.Tfs.Proxies;

namespace VersionTasks
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
        /// <example>2012</example>
        public string TfsVersion { get; set; }

        private static Assembly _clientAssembly;
        private static Assembly _versionAssembly;

        internal override void SetVersionInfo()
        {
            string tfsAssemblyVersion;

            // TFS 2010 is default for backward compatibility
            switch (TfsVersion)
            {
                case "2012":
                    tfsAssemblyVersion = "11.0.0.0";
                    break;
                default:
                    tfsAssemblyVersion = "10.0.0.0";
                    break;
            }

            _clientAssembly = Assembly.Load(
                String.Format("Microsoft.TeamFoundation.Client, Version={0}, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", tfsAssemblyVersion)
                );
            _versionAssembly = Assembly.Load(
                String.Format("Microsoft.TeamFoundation.VersionControl.Client, Version={0}, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", tfsAssemblyVersion)
                );

            try
            {
                VersionControlServer versionControlServer = new VersionControlServer(
                    _clientAssembly,
                    _versionAssembly,
                    new Workstation(_versionAssembly).GetLocalWorkspaceInfo(WorkingDirectory).ServerUri.ToString(),
                    CredentialCache.DefaultNetworkCredentials
                    );

                // 1234
                Changeset = GetChangeset(versionControlServer, WorkingDirectory).ToString();
                // 1234
                ChangesetShort = Changeset;
                // true/false
                DirtyBuild = GetDirtyBuild(versionControlServer);
            }
            catch (TfsException ex)
            {
                // TeamCity build server check outs are not associated with a local TFS workspace, so we will get the
                // error 'The local path is not associated with a TFS Workspace'. We want to swallow this exception
                // here so the $CHANGESET$ token will be still initialized with it default value '0'.
                Log.LogWarningFromException(ex);
            }
        }

        private static int GetChangeset(VersionControlServer versionControlServer, string workingDirectory)
        {
            int changesetId = 0;

            WorkspaceVersionSpec workspaceVersionSpec = new WorkspaceVersionSpec(
                _versionAssembly,
                versionControlServer.GetWorkspace(workingDirectory)
                );

            IEnumerable history = versionControlServer.QueryHistory(
                workingDirectory,
                new VersionSpec(_versionAssembly).Latest,
                new RecursionType(_versionAssembly).Full,
                workspaceVersionSpec
                );

            IEnumerator historyEnumerator = history.GetEnumerator();
            Changeset changeset = new Changeset(_versionAssembly);

            if (historyEnumerator.MoveNext())
                changeset = new Changeset(_versionAssembly, historyEnumerator.Current);

            if (changeset.Instance != null)
                changesetId = changeset.ChangesetId;

            return changesetId;
        }

        private static bool GetDirtyBuild(VersionControlServer versionControlServer)
        {
            dynamic pendingSets = versionControlServer.GetPendingSets(new RecursionType(_versionAssembly).Full);

            foreach (var pendingSet in pendingSets)
            {
                // compare pending sets with local machine name
                if (String.Equals(pendingSet.Computer, Environment.MachineName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}
