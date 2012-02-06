using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using MSBuild.Version.Tasks.Exceptions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MSBuild.Version.Tasks
{
    /// <summary>
    /// This is a modified fork of the MSBuild Versioning project.
    /// </summary>
    /// <remarks>
    /// This was necessary because we use TeamCity as our build server. TeamCity checks out the source code without the '.hg' folder,
    /// which would lead to an exception. Therefore a different exception handling implementation was necessary. Includes all bugfixes
    /// up to changeset 'cec89e827f9a' (10/16/2010 01:32 PM).
    /// </remarks>
    /// <see href="http://versioning.codeplex.com/SourceControl/changeset/view/cec89e827f9a/"/>
    public class HgVersionFile : Task
    {
        /// <example>..Common\CommonAssemblyInfo.cs.tmp</example>
        [Required]
        public string TemplateFile { get; set; }
        /// <example>..Common\CommonAssemblyInfo.cs</example>
        [Required]
        public string DestinationFile { get; set; }
        /// <example>..\..\</example>
        public string WorkingDirectory { get; set; }

        public override bool Execute()
        {
            try
            {
                // set Mercurial version info
                SetVersionInfo();

                // read content of the template file
                string content = File.ReadAllText(TemplateFile);

                // replace tokens in the template file content with version info
                content = content.Replace("$REVISION$", Revision.ToString());
                content = content.Replace("$DIRTY$", Dirty.ToString());
                content = content.Replace("$CHANGESET$", Changeset);

                // write the destination file, only if it needs to be updated
                if (!File.Exists(DestinationFile) || File.ReadAllText(DestinationFile) != content)
                    File.WriteAllText(DestinationFile, content);

                return true;
            }
            catch (Exception ex)
            {
                // logging as error will still cause the build to fail
                Log.LogError(ex.Message);

                return false;
            }
        }

        private int Revision { get; set; }
        private int Dirty { get; set; }
        private string Changeset { get; set; }

        private void SetVersionInfo()
        {
            try
            {
                string revision = ExecuteCommand("hg.exe", "identify -n")[0];

                if (revision.Contains("+"))
                {
                    Revision = int.Parse(revision.Substring(0, revision.IndexOf("+")));
                    Dirty = 1;
                }
                else
                {
                    Revision = int.Parse(revision);
                    Dirty = 0;
                }

                Changeset = ExecuteCommand("hg.exe", "identify -i")[0].Substring(0, 12);
            }
            catch (HgException ex)
            {
                // TeamCity build server does not check out the directory with a .hg folder, so we will get the error
                // 'There is no Mercurial repository here (.hg not found)'. We want to swallow this exception here so
                // the $REVISION$ and $DIRTY$ tokens will be still initialized with their default value '0'.
                Log.LogWarningFromException(ex);
            }
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        private IList<string> ExecuteCommand(string fileName, string arguments)
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
                    if (ex.NativeErrorCode == 2) // file not found
                    {
                        throw new HgException(
                            String.Format("Mercurial command \"{0}\" could not be found. Please ensure that Mercurial is installed.", fileName)
                            );
                    }

                    throw;
                }

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                if (process.ExitCode != 0 || error.Length > 0)
                {
                    throw new HgException(
                        String.Format("Mercurial command \"{0} {1}\" exited with code {2}.\n{3}", fileName, arguments, process.ExitCode, error));
                }
            }

            return output;
        }
    }
}
