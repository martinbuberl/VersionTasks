# VersionTasks

VersionTasks is a [MSBuild Tasks][msbuildtasks] library to automatically insert the current repository's changeset into to your project.

The following source control systems are supported: [Git][git], [Mercurial][mercurial] and [Team Foundation Server 2010][tfs2010].

## Get it on NuGet

[NuGet][nuget] is a Visual Studio extension that makes it easy to install and update third-party libraries 
and tools in Visual Studio.

To install [VersionTasks][package], run the following command in the [Package Manager Console][pmc]:

    PM> Install-Package VersionTasks

## Usage

### MSBuild Tasks

There are three tasks to support different source control systems: `GitVersionFile`, `HgVersionFile` and `TfsVersionFile`.

**Attributes**

- `TemplateFile`: Path of the template file to parse (Required).
- `DestinationFile`: Path of the file to get generated from the template file (Required).
- `WorkingDirectory`: Path to the Team Foundation Server's working directory (Optional).

All paths are relative from your project's root directory. The `WorkingDirectory` attribute is only necessary if you are using the `TfsVersionFile` task. 

**Examples**

<pre><code>&lt;GitVersionFile TemplateFile="Properties\Version.tmp" DestinationFile="Properties\Version.cs" /&gt;
&lt;HgVersionFile TemplateFile="Properties\Version.tmp" DestinationFile="Properties\Version.cs" /&gt;
&lt;TfsVersionFile TemplateFile="Properties\Version.tmp" DestinationFile="Properties\Version.cs" WorkingDirectory="\" /&gt;</code></pre>

### Templating 

The template contains the placeholder which are replaced on every build with the corresponding values from your repository. There are no restrictions on the file type or how a template needs to look like.

For example a template written in C# could look like the following:

**TemplateFile: Version.tmp**

<pre><code>using System;

public static class Version
{
    public const string Changeset = "$changeset$";
    public const string ChangesetShort = "$changesetshort$";
    public static bool DirtyBuild = Convert.ToBoolean($dirtybuild$);
}</code></pre>

The build task will then create a file based on this template under the path and with the file name you specified in the `DestinationFile` attribute. For the above example using e.g. the `GitVersionFile` task  a file like the following gets generated:

**DestinationFile: Version.cs**

<pre><code>using System;

public static class Version
{
    public const string Changeset = "8d596df194b12b6d66baad2f16a240afbf7627d6";
    public const string ChangesetShort = "8d596df194";
    public static bool DirtyBuild = Convert.ToBoolean(0);
}</code></pre>

**Placeholder**

- `$changeset$`: Changeset of the repository.
- `$changesetshort$`: Shortened changeset of the repository.
- `$dirtybuild$`: Indicates a dirty build. `1` if there are uncommitted changes; otherwise, `0`.

All placeholder are delimited using dollar signs ($) and get replaced with the currently checked-out values of your repository. Note that Team Foundation Server's shortened changeset will be the same as the changeset due to their increase number format.

### Check out the Sample

If you want to see everything in action working together you can dig into this small C# console application I created:

https://github.com/martinbuberl/VersionTasks.Sample

## Installation

The setup is currently not fully automated via NuGet - I'd love to do so when I find the time. Therefore some simple steps are necessary to get it up and running after the package is installed. Don't be scared, it's a piece of cake:

- In Visual Studio add a new text file to the root of your project and name it `VersionInfo.tmp`.<br/>
If you have multiple projects in your solution you probably want to pick your most generic project (e.g. Common, Core).<br/>
For a C# project, I usually use a file like the following, but all that matters is the `$changeset$` placeholder:

<pre><code>public static class VersionInfo
{
    public const string Changeset = "$changeset$";
}</code></pre>

- In the *Solution Explorer* right-click the project to which you want to add the MSBuild Tasks and select *Unload Project*.

- If your project has been unloaded it should be designated as *(unavailable)*.<br/>
Right-click the project again and select *Edit ProjectName*.

- The project will open as an XML document in your editor.<br/>
Scroll to the the end of the document where you'll most likely see something like the following code:

<pre><code>&lt;!-- To modify your build process, add your task inside one of the targets below and uncomment it. 
    Other similar extension points exist, see Microsoft.Common.targets.
&lt;Target Name="BeforeBuild"&gt;
&lt;/Target&gt;
&lt;Target Name="AfterBuild"&gt;
&lt;/Target&gt;
--&gt;</code></pre>

- Edit this section to the below code. Note that we added the import `VersionTasks.targets`, uncommented the `BeforeBuild` target and added the `HgVersionFile` task to be executed before the build. Some of the paths might be a bit different for you based on your project's structure :

<pre><code>&lt;Import Project="..\packages\VersionTasks.*\tools\VersionTasks.targets" /&gt;
&lt;Target Name="BeforeBuild"&gt;
  &lt;GitVersionFile TemplateFile="VersionInfo.tmp" DestinationFile="VersionInfo.cs" /&gt;
&lt;/Target&gt;
&lt;!-- To modify your build process, add your task inside one of the targets below and uncomment it. 
    Other similar extension points exist, see Microsoft.Common.targets.
&lt;Target Name="AfterBuild"&gt;
&lt;/Target&gt;
--&gt;</code></pre>

- When you're done right-click the project again and select *Reload Project*.

## Give back

If you found this project useful you can [buy me a beer][donate].

## License
NUnitHelpers is released under the [MIT license][mit].



[msbuildtasks]: http://msdn.microsoft.com/en-us/library/ms171466.aspx
[git]:          http://git-scm.com/
[mercurial]:    http://mercurial.selenic.com/
[tfs2010]:      http://www.microsoft.com/visualstudio/en-us/products/2010-editions/team-foundation-server/overview
[nuget]:        http://nuget.org
[package]:      http://nuget.org/packages/VersionTasks
[pmc]:          http://docs.nuget.org/docs/start-here/using-the-package-manager-console
[donate]:       https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=2AGHGEL2X4VSQ
[mit]:          https://github.com/martinbuberl/NUnitHelpers/blob/master/LICENSE