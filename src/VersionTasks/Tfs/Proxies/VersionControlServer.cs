using System;
using System.Reflection;
using System.Net;
using System.Collections;
using MSBuild.Version.Tasks.Exceptions;

namespace MSBuild.Version.Tasks.Tfs.Proxies
{
    internal class VersionControlServer
    {
        readonly Assembly _assembly;
        readonly Type _type;
        readonly object _instance;

        public VersionControlServer(Assembly clientAssembly, Assembly versionControlClientAssembly, string server, ICredentials credentials)
        {
            _assembly = versionControlClientAssembly;
            _type = _assembly.GetType("Microsoft.TeamFoundation.VersionControl.Client.VersionControlServer");
            _instance = CreateInstance(clientAssembly, server, credentials);
        }

        private object CreateInstance(Assembly clientAssembly, string server, ICredentials credentials)
        {
            Type teamFoundationServerType = clientAssembly.GetType("Microsoft.TeamFoundation.Client.TeamFoundationServer");
            ConstructorInfo teamFoundationServerConstructor = teamFoundationServerType.GetConstructor(new[] { typeof(string), typeof(ICredentials) });
            object teamFoundationServer = teamFoundationServerConstructor.Invoke(new object[] { server, credentials });
            MethodInfo getServiceMethod = teamFoundationServerType.GetMethod("GetService", new[] { typeof(Type) });
            object versionControlServer = getServiceMethod.Invoke(teamFoundationServer, new object[] { _type });

            return versionControlServer;
        }

        public object GetPendingSets(RecursionType recursion)
        {
            MethodInfo getPendingSetsMethod = _type.GetMethod("GetPendingSets", new[] { typeof(string[]), recursion.Type });

            string[] items = new string[1];
            items[0] = @"$/";

            return getPendingSetsMethod.Invoke(_instance, new[] { items, recursion.Instance });
        }

        public Workspace GetWorkspace(string localPath)
        {
            Type itemNotMappedException = _assembly.GetType("Microsoft.TeamFoundation.VersionControl.Client.ItemNotMappedException");
            MethodInfo getWorkspaceMethod = _type.GetMethod("GetWorkspace", new[] { typeof(string) });
            object workspace;

            try
            {
                workspace = getWorkspaceMethod.Invoke(_instance, new object[] { localPath });
            }
            catch (Exception ex)
            {
                Exception actualException;

                if (ex is TargetInvocationException)
                {
                    actualException = ex.InnerException;
                }
                else
                {
                    actualException = ex;
                }

                if (actualException != null)
                {
                    if (actualException.GetType() == itemNotMappedException)
                        throw new TfsException(actualException.Message);
                }

                throw;
            }

            return new Workspace(_assembly, workspace);
        }

        public IEnumerable QueryHistory(string localPath, VersionSpec version, RecursionType recursion, WorkspaceVersionSpec toVersion)
        {
            Type[] parameterTypes = new[] {
                typeof(string), version.Type, typeof(int), recursion.Type, typeof(string), toVersion.Type, toVersion.Type, typeof(int), typeof(bool), typeof(bool)
                };

            MethodInfo queryHistoryMethod = _type.GetMethod("QueryHistory", parameterTypes);

            IEnumerable history = (IEnumerable)queryHistoryMethod.Invoke(
                _instance, new[] { localPath, version.Instance, 0, recursion.Instance, null, null, toVersion.Instance, int.MaxValue, false, false }
                );

            return history;
        }
    }
}
