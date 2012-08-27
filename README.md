# VersionTasks

VersionTasks is a MSBuild Tasks library to automatically add the current changeset to your project. Currently, this supports [Git][git], [Mercurial][mercurial] and [Team Foundation Server 2010][tfs2010]).

## Get it on NuGet

[NuGet][nuget] is a Visual Studio extension that makes it easy to install and update third-party libraries 
and tools in Visual Studio.

To install [VersionTasks][package], run the following command in the [Package Manager Console][pmc]:

    PM> Install-Package VersionTasks

## Usage

###Installation

The setup is currently not fully automated via NuGet (I'd love to do so when I find the time), therefore some simple steps are necessary to get it up and running after the package is installed. Don't be scared, it's a piece of cake:

1. In VisualStudio's *Solution Explorer* right-click the project you want to add the VersionTasks tasks to and select *Unload Project*.<br/>
If you have multiple projects in your solution you probably want to pick your most generic project (e.g. Common, Core etc.).

2. There should be an *(unavailable)* behind your project's name. Right-click the project again and select *Edit ProjectName*.

3. You'll see an XML document which is your project's MSBuild file. Scroll to the the end of this XML document until you see the following code:

    <!-- To modify your build process, add your task inside one of the targets below and uncomment it. 
         Other similar extension points exist, see Microsoft.Common.targets.
    <Target Name="BeforeBuild">
    </Target>
    <Target Name="AfterBuild">
    </Target>
    -->

2. If you haven't changed this section of your project yet, you'll most likely see something like this at the bottom of 

###



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