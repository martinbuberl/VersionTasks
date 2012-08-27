# VersionTasks

VersionTasks is a MSBuild Tasks library to automatically add the current changeset to your project.

The following source control systems are supported: [Git][git], [Mercurial][mercurial] and [Team Foundation Server 2010][tfs2010]).

## Get it on NuGet

[NuGet][nuget] is a Visual Studio extension that makes it easy to install and update third-party libraries 
and tools in Visual Studio.

To install [VersionTasks][package], run the following command in the [Package Manager Console][pmc]:

    PM> Install-Package VersionTasks

## Usage

###MSBuild Tasks

There are three MSBuild Tasks to support different source control systems:

- Git: `GitVersionFile`
- Mercurial: `HgVersionFile`
- TFS2010: `TfsVersionFile`

###Installation

The setup is currently not fully automated via NuGet - I'd love to do so when I find the time. Therefore some simple steps are necessary to get it up and running after the package is installed. Don't be scared, it's a piece of cake:

1. In VisualStudio's *Solution Explorer* right-click the project to which you want to add the MSBuild Tasks and select *Unload Project*.<br/>
If you have multiple projects in your solution you probably want to pick your most generic project (e.g. Common, Core).

2. If your project has been unloaded it should be designated as *(unavailable)*.<br/>
Right-click the project again and select *Edit ProjectName*.

3. The project will open as an XML document in your editor.<br/>
Scroll to the the end of the document where you'll most likely see something like the following code:

<pre><code>&lt;!-- To modify your build process, add your task inside one of the targets below and uncomment it. 
    Other similar extension points exist, see Microsoft.Common.targets.
&lt;Target Name="BeforeBuild"&gt;
&lt;/Target&gt;
&lt;Target Name="AfterBuild"&gt;
&lt;/Target&gt;
--&gt;</code></pre>

4. Edit this section to the below code. Some of the paths might be a bit different for you based on your project's structure :

<pre><code>&lt;Import Project="..\..\packages\VersionTasks.*\tools\VersionTasks.targets" /&gt;
&lt;Target Name="BeforeBuild"&gt;
  &lt;GitVersionFile TemplateFile="Properties\VersionInfo.cs.tmp" DestinationFile="Properties\VersionInfo.cs" /&gt;
&lt;/Target&gt;
&lt;!-- To modify your build process, add your task inside one of the targets below and uncomment it. 
    Other similar extension points exist, see Microsoft.Common.targets.
&lt;Target Name="AfterBuild"&gt;
&lt;/Target&gt;
--&gt;</code></pre>

## Give back

If you found this project useful you can [buy me a beer][donate].

## License
NUnitHelpers is released under the [MIT license][mit].





[git]:       http://git-scm.com/
[mercurial]: http://mercurial.selenic.com/
[tfs2010]:   http://www.microsoft.com/visualstudio/en-us/products/2010-editions/team-foundation-server/overview
[nuget]:     http://nuget.org
[package]: http://nuget.org/packages/VersionTasks
[pmc]:     http://docs.nuget.org/docs/start-here/using-the-package-manager-console
[donate]:  https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=2AGHGEL2X4VSQ
[mit]:     https://github.com/martinbuberl/NUnitHelpers/blob/master/LICENSE