using System;
using System.Reflection;
using MSBuild.Version.Tasks.Exceptions;

namespace MSBuild.Version.Tasks.Tfs.Proxies
{
    internal class Workstation
    {
        readonly Assembly _assembly;
        readonly Type _type;
        readonly object _instance;

        public Workstation(Assembly versionControlClientAssembly)
        {
            _assembly = versionControlClientAssembly;
            _type = _assembly.GetType("Microsoft.TeamFoundation.VersionControl.Client.Workstation");
            _instance = createInstance();
        }

        private object createInstance()
        {
            PropertyInfo currentProperty = _type.GetProperty("Current");
            object currentWorkstation = currentProperty.GetValue(null, null);

            return currentWorkstation;
        }

        internal WorkspaceInfo GetLocalWorkspaceInfo(string localPath)
        {
            MethodInfo getLocalWorkspaceInfoMethod = _type.GetMethod("GetLocalWorkspaceInfo", new[] { typeof(string) });
            object workspaceInfoInstance = getLocalWorkspaceInfoMethod.Invoke(_instance, new object[] { localPath });

            if (workspaceInfoInstance == null)
            {
                throw new TfsException(
                    String.Format("The local path {0} is not associated with a TFS Workspace.", localPath)
                    );
            }

            return new WorkspaceInfo(_assembly, workspaceInfoInstance);
        }
    }
}
