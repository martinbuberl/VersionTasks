using System;
using System.Reflection;

namespace VersionTasks.Tfs.Proxies
{
    internal class WorkspaceVersionSpec
    {
        readonly Assembly _assembly;
        readonly Type _type;
        readonly object _instance;

        public WorkspaceVersionSpec(Assembly versionControlClientAssembly, Workspace workspace)
        {
            _assembly = versionControlClientAssembly;
            _type = _assembly.GetType("Microsoft.TeamFoundation.VersionControl.Client.WorkspaceVersionSpec");
            _instance = CreateInstance(workspace);
        }

        public Type Type { get { return _type; } }
        public object Instance { get { return _instance; } }

        private object CreateInstance(Workspace workspace)
        {
            ConstructorInfo workspaceVersionSpecTypeContstructor = _type.GetConstructor(new[] { workspace.Type });
            object workspaceVersionSpec = workspaceVersionSpecTypeContstructor.Invoke(new[] { workspace.Instance });

            return workspaceVersionSpec;
        }

        public object Latest
        {
            get
            {
                PropertyInfo versionSpecTypeLatestProperty = _type.GetProperty("Latest");
                object latestVersionSpec = versionSpecTypeLatestProperty.GetValue(null, null);

                return latestVersionSpec;
            }
        }
    }
}
