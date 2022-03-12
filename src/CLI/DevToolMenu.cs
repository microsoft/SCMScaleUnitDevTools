using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;

namespace CLI
{
    internal abstract class DevToolMenu : CLIMenu
    {
        protected List<ScaleUnitInstance> sortedScaleUnits = null;

        public abstract string Label { get; }

        public abstract Task Show(int input, string selectionHistory);

        protected List<ScaleUnitInstance> GetSortedScaleUnits()
        {
            if (sortedScaleUnits is null)
            {
                sortedScaleUnits = Config.ScaleUnitInstances();
                sortedScaleUnits.Sort();
            }
            return sortedScaleUnits;
        }

        protected string GetScaleUnitId(int userInput)
        {
            return GetSortedScaleUnits()[userInput - 1].ScaleUnitId;
        }

        protected CLIOption Option(string name, Func<int, string, Task> command)
        {
            return new CLIOption { Name = name, Command = command };
        }

        protected List<CLIOption> SelectScaleUnitOptions(List<ScaleUnitInstance> scaleUnits, Func<int, string, Task> command)
        {
            var options = new List<CLIOption>();
            foreach (ScaleUnitInstance scaleUnit in scaleUnits)
            {
                options.Add(Option(scaleUnit.PrintableName(), command));
            }
            return options;
        }
    }
}
