using System;

namespace CLI
{
    public class ArgumentParser
    {
        public bool Deploy { get; set; } = false;
        public bool CleanStorage { get; set; } = false;
        private bool[] matched;
        private string[] arguments;

        public void Parse(string[] args)
        {
            arguments = args;
            matched = new bool[args.Length];

            Deploy = ParseArgument("--single-box-deploy");
            CleanStorage = ParseArgument("--clean-storage");

            for (int i = 0; i < matched.Length; i++)
            {
                if (!matched[i])
                {
                    throw new Exception($"Did not recognize argument {args[i]}. Allowed arguments are \"--single-box-deploy\" and \"--clean-storage\"");
                }
            }
        }

        private bool ParseArgument(string argument)
        {
            int cliOption = Array.FindIndex(arguments, option => option.Equals(argument));
            if (cliOption != -1)
            {
                matched[cliOption] = true;
                return true;
            }
            return false;
        }
    }
}
