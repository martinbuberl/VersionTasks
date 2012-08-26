using System;
using System.Reflection;

namespace MSBuild.Version.Tasks.Tfs.Proxies
{
    internal class Workspace
    {
        readonly Assembly _assembly;
        readonly Type _type;
        readonly object _instance;

        public Workspace(Assembly versionControlClientAssembly, object instance)
        {
            _assembly = versionControlClientAssembly;
            _type = _assembly.GetType("Microsoft.TeamFoundation.VersionControl.Client.Workspace");
            _instance = instance;
        }

        public Type Type { get { return _type; } }
        public object Instance { get { return _instance; } }
    }
}
