using System;
using System.Threading.Tasks;

namespace CLIFramework
{
    public class CLIOption
    {
        public string Name { get; set; }
        public Func<int, string, Task> Command { get; set; }
    }
}
