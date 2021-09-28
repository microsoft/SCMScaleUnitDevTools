using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;

namespace CLI
{
    public abstract class DevToolMenu : CLIMenu
    {
        protected List<ScaleUnitInstance> sortedScaleUnits = null;

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

        protected string GetScaleUnitId(int index)
        {
            return GetSortedScaleUnits()[index].ScaleUnitId;
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
                options.Add(Option(scaleUnit.PrintableName(), RunInScaleUnitContext(scaleUnit, command)));
            }
            return options;
        }

        private Func<int, string, Task> RunInScaleUnitContext(ScaleUnitInstance scaleUnit, Func<int, string, Task> command)
        {
            return async (input, selectionHistory) =>
            {
                using var context = ScaleUnitContext.CreateContext(scaleUnit.ScaleUnitId);
                await command(input, selectionHistory);
            };
        }
    }
}
