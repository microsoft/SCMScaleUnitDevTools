using System;

namespace CLI.Utilities
{
    public class ArgumentParser
    {
        public bool Deploy { get; set; } = false;
        public bool CleanStorage { get; set; } = false;
        public bool DrainPipelines { get; set; } = false;
        public bool StartPipelines { get; set; } = false;

        private bool[] matched;
        private string[] arguments;

        public void Parse(string[] args)
        {
            arguments = args;
            matched = new bool[args.Length];

            Deploy = ParseArgument("--single-box-deploy");
            CleanStorage = ParseArgument("--clean-storage");
            DrainPipelines = ParseArgument("--drain-pipelines");
            StartPipelines = ParseArgument("--start-pipelines");

            for (int i = 0; i < matched.Length; i++)
            {
                if (!matched[i])
                {
                    throw new Exception($"Did not recognize argument {args[i]}.");
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

        public string HelpMessage()
        {
            return "Usage: program [options]\n" +
                "--clean-storage        Cleans all Azure storage accounts for hub and spoke\n" +
                "--single-box-deploy    Prepares and installs hub and spoke environments\n" +
                "--drain-pipelines      Drains pipelines between hub and spoke\n" +
                "--start-pipelines      Starts pipelines between hub and spoke\n" +
                "If no options are given the UI will run.";
        }
    }
}
