using System;

namespace CLI.Utilities
{
    public class ArgumentParser
    {
        public bool SingleBoxDeploy { get; set; } = false;
        public bool HubDeploy { get; set; } = false;
        public bool SpokeDeploy { get; set; } = false;
        public bool CleanStorage { get; set; } = false;
        public bool DrainPipelines { get; set; } = false;
        public bool StartPipelines { get; set; } = false;

        private bool[] matched;
        private string[] arguments;

        public void Parse(string[] args)
        {
            arguments = args;
            matched = new bool[args.Length];

            SingleBoxDeploy = ParseArgument("--single-box-deploy");
            HubDeploy = ParseArgument("--hub-deploy");
            SpokeDeploy = ParseArgument("--spoke-deploy");
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

            Validate();
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

        private void Validate()
        {
            if ((SingleBoxDeploy && HubDeploy) || (SingleBoxDeploy && SpokeDeploy) || (HubDeploy && SpokeDeploy))
            {
                throw new Exception("You can only use one deploy option at a time.");
            }
        }

        public string HelpMessage()
        {
            return "Usage: program [options]\n" +
                "--single-box-deploy    Prepares and installs hub and spoke environments\n" +
                "--hub-deploy           Prepares and installs the hub\n" +
                "--spoke-deploy         Prepares and installs the spoke. Only use this after the hub has been successfully deployed\n" +
                "--clean-storage        Cleans all Azure storage accounts for hub and spoke\n" +
                "--drain-pipelines      Drains pipelines between hub and spoke\n" +
                "--start-pipelines      Starts pipelines between hub and spoke\n" +
                "If no options are given the UI will run.";
        }
    }
}
