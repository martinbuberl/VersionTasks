# VersionTasks

VersionTasks is an [MSBuild Tasks][msbuildtasks] library to automatically insert the current repository's changeset into your project.

The following source control systems are supported:

- [Git][git]
- [Mercurial][mercurial]
- Team Foundation Server 2010
- [Team Foundation Server 2012][tfs].

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
- `WorkingDirectory`: Path to the source controled working directory (Optional).
- `TfsVersion`: Version of the Team Foundation Server (e.g. `2010 (default)`, `2012`) (Optional).

All paths are relative from your project's root directory. The `WorkingDirectory` attribute is only required if you are using the `TfsVersionFile` task.

**Examples**

<pre><code>&lt;GitVersionFile TemplateFile="Properties\Version.tmp" DestinationFile="Properties\Version.cs" /&gt;
&lt;HgVersionFile TemplateFile="Properties\Version.tmp" DestinationFile="Properties\Version.cs" /&gt;
&lt;TfsVersionFile TemplateFile="Properties\Version.tmp" DestinationFile="Properties\Version.cs"
                WorkingDirectory="\" /&gt;
&lt;TfsVersionFile TemplateFile="Properties\Version.tmp" DestinationFile="Properties\Version.cs"
                WorkingDirectory="\" TfsVersion="2012" /&gt;</code></pre>

### Templating 

The template contains the placeholders, which are replaced on every build with the corresponding values from your repository. There are no restrictions on the file type or what a template needs to look like.

For example a template written in C# could look like the following:

**TemplateFile: Version.tmp**

<pre><code>using System;

public static class Version
{
    public const string Changeset = "$changeset$";
    public const string ChangesetShort = "$changesetshort$";
    public const bool DirtyBuild = $dirtybuild$;
}</code></pre>

The build task will then create a file based on this template under the path and with the file name you specified in the `DestinationFile` attribute. For the above example using e.g. the `GitVersionFile` task  a file like the following gets generated:

**DestinationFile: Version.cs**

<pre><code>using System;

public static class Version
{
    public const string Changeset = "8d596df194b12b6d66baad2f16a240afbf7627d6";
    public const string ChangesetShort = "8d596df194";
    public const bool DirtyBuild = false;
}</code></pre>

Note that you can add this generated file to your solution - if you need it to be compiled - but you don't want to add it to your version control. This file will be different with every new changeset.

**Placeholders**

- `$changeset$`: Changeset of the repository.
- `$changesetshort$`: Shortened changeset of the repository.
- `$dirtybuild$`: Indicates a dirty build. `true` if there are uncommitted changes; otherwise, `false`.

All placeholders are delimited using dollar signs ($) and are replaced with the currently checked-out repository's values. Note that Team Foundation Server's shortened changeset will be the same value as the changeset due to its increased number format.

### Show me the codez

If you want to see everything working together dig into this small C# console application:

- [github.com/martinbuberl/VersionTasks.Sample][sample]

## Installation

The setup is currently not fully automated via NuGet - I'd love to do so when I find the time. Therefore some simple steps are necessary to get it up and running after the package is installed. Don't be scared, it's a piece of cake:

- In Visual Studio add a new text file to the root of your project - or wherever you want, but stick with me here - and name it `Version.tmp`. Insert the placeholder `$changeset$` into that file and save it.<br/>
Tip: If you have multiple projects in your solution you probably want to pick the most generic one (e.g. Common, Core).

- In *Solution Explorer* right-click the project and select *Unload Project*. If it has been unloaded it should be designated as *(unavailable)*. Right-click the project again and select *Edit ProjectName*.

- The project will open as an XML document in your editor. Scroll to the the end of the document where you'll most likely see something like the following code:

<pre><code>&lt;!-- To modify your build process, add your task inside one of the targets below and uncomment it. 
    Other similar extension points exist, see Microsoft.Common.targets.
&lt;Target Name="BeforeBuild"&gt;
&lt;/Target&gt;
&lt;Target Name="AfterBuild"&gt;
&lt;/Target&gt;
--&gt;</code></pre>

- Edit this section to the below code. Note that we added the import `VersionTasks.targets`, uncommented the `BeforeBuild` target and added the `HgVersionFile` task to be executed before build. Change this task appropriately  to `HgVersionFile` or `TfsVersionFile` if your repository is not using Git as source control system:

<pre><code>&lt;Import Project="..\packages\VersionTasks.*\tools\VersionTasks.targets" /&gt;
&lt;Target Name="BeforeBuild"&gt;
  &lt;GitVersionFile TemplateFile="Version.tmp" DestinationFile="Version.txt" /&gt;
&lt;/Target&gt;
&lt;!-- To modify your build process, add your task inside one of the targets below and uncomment it. 
    Other similar extension points exist, see Microsoft.Common.targets.
&lt;Target Name="AfterBuild"&gt;
&lt;/Target&gt;
--&gt;</code></pre>

- When you're done right-click the project again and select *Reload Project*.

- *Build* your project - that's where all the magic is happening.

- In the *Solution Explorer* enable *Show All Files* and click *Refresh*. You should see a new file `Version.txt` in the root of your project. Open that file and you should see the current changeset of your repository. Et voilà!

## Give back

If you found this project useful you can follow me on Twitter ([@martinbuberl][twitter]) or [buy me a beer][donate].

## License
VersionTasks is released under the [MIT license][mit].



[msbuildtasks]: http://msdn.microsoft.com/en-us/library/ms171466.aspx
[git]:          http://git-scm.com/
[mercurial]:    http://mercurial.selenic.com/
[tfs]:          http://www.microsoft.com/visualstudio/eng/products/visual-studio-team-foundation-server-2012
[nuget]:        http://nuget.org
[package]:      http://nuget.org/packages/VersionTasks
[pmc]:          http://docs.nuget.org/docs/start-here/using-the-package-manager-console
[sample]:       https://github.com/martinbuberl/VersionTasks.Sample
[twitter]:      https://twitter.com/martinbuberl
[donate]:       https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=2AGHGEL2X4VSQ
[mit]:          https://github.com/martinbuberl/NUnitHelpers/blob/master/LICENSE