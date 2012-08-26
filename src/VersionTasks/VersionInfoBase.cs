using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using VersionTasks.Exceptions;

namespace VersionTasks
{
    public abstract class VersionInfoBase : Task
    {
        /// <example>..Common\CommonAssemblyInfo.cs.tmp</example>
        [Required]
        public string TemplateFile { get; set; }
        /// <example>..Common\CommonAssemblyInfo.cs</example>
        [Required]
        public string DestinationFile { get; set; }
        /// <example>..\..\</example>
        public string WorkingDirectory { get; set; }

        internal string Changeset { get; set; }
        internal string ChangesetShort { get; set; }
        internal int DirtyBuild { get; set; }

        public override bool Execute()
        {
            try
            {
                SetVersionInfo();

                // read content of the template file
                string content = File.ReadAllText(TemplateFile);

                // replace tokens in the template file content with version info
                content = Regex.Replace(content, "%Changeset%", Changeset, RegexOptions.IgnoreCase);
                content = Regex.Replace(content, "%ChangesetShort%", ChangesetShort, RegexOptions.IgnoreCase);
                content = Regex.Replace(content, "%DirtyBuild%", DirtyBuild.ToString(), RegexOptions.IgnoreCase);

                // write the destination file, only if it needs to be updated
                if (!File.Exists(DestinationFile) || File.ReadAllText(DestinationFile) != content)
                    File.WriteAllText(DestinationFile, content);
            }
            catch (Exception ex)
            {
                // logging as error will still cause the build to fail
                Log.LogError(ex.Message);

                return false;
            }

            return true;
        }

        internal virtual void SetVersionInfo()
        {
            throw new NotImplementedException();
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        internal IList<string> ExecuteCommand(string fileName, string arguments)
        {
            IList<string> output = new List<string>();
            StringBuilder error = new StringBuilder();

            using (Process process = new Process())
            {
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.WorkingDirectory = WorkingDirectory;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                process.OutputDataReceived += (o, e) =>
                {
                    if (e.Data != null)
                        output.Add(e.Data);
                };

                process.ErrorDataReceived += (o, e) =>
                {
                    if (e.Data != null)
                        error.AppendLine(e.Data);
                };

                try
                {
                    process.Start();
                }
                catch (Win32Exception ex)
                {
                    if (ex.NativeErrorCode == 2) // the system cannot find the file specified
                    {
                        throw new ExecuteCommandException(
                            String.Format("Command \"{0}\" could not be found. Please ensure that the source control is installed and part of the system's path environment variable.", fileName)
                            );
                    }

                    throw;
                }

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                if (process.ExitCode != 0 || error.Length > 0)
                {
                    throw new ExecuteCommandException(
                        String.Format("Command \"{0} {1}\" exited with code {2}.\n{3}", fileName, arguments, process.ExitCode, error)
                        );
                }
            }

            return output;
        }
    }
}
