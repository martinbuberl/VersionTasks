using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplicationSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Changeset:         " + VersionInfo.Changeset);
            Console.WriteLine("Changeset (short): " + VersionInfo.ChangesetShort);
            Console.WriteLine("Dirty Build:       " + VersionInfo.DirtyBuild);

            Console.ReadKey();
        }
    }
}
