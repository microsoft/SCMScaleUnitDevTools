using System;

namespace CLI
{
    class ArgumentParser
    {
        public bool deploy = false;
        public bool cleanStorage = false;
        private bool[] matched;
        private string[] arguments;

        public void Parse(string[] args)
        {
            arguments = args;
            matched = new bool[args.Length];
            for (int i = 0; i < matched.Length; i++)
            {
                matched[i] = false;
            }

            deploy = ParseArgument("--single-box-deploy");
            cleanStorage = ParseArgument("--clean-storage");

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
