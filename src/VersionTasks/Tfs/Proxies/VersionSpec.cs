using System;
using System.Reflection;

namespace MSBuild.Version.Tasks.Tfs.Proxies
{
    internal class VersionSpec
    {
        readonly Assembly _assembly;
        readonly Type _type;
        readonly object _instance;

        public VersionSpec(Assembly clientAssembly)
            : this(clientAssembly, null)
        {
        }

        public VersionSpec(Assembly clientAssembly, object instance)
        {
            _assembly = clientAssembly;
            _type = _assembly.GetType("Microsoft.TeamFoundation.VersionControl.Client.VersionSpec");
            _instance = instance;
        }

        public Type Type { get { return _type; } }
        public object Instance { get { return _instance; } }

        public VersionSpec Latest
        {
            get
            {
                PropertyInfo versionSpecTypeLatestProperty = _type.GetProperty("Latest");
                object latestVersionSpec = versionSpecTypeLatestProperty.GetValue(null, null);

                return new VersionSpec(_assembly, latestVersionSpec);
            }
        }
    }
}
