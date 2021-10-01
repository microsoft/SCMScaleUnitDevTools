using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CLIFramework
{
    public class MultiSelectScreen : CLIScreen
    {
        public MultiSelectScreen(List<CLIOption> options, string selectionHistory, string infoBeforeOptions = "", string infoAfterOptions = "")
            : base(options, selectionHistory, infoBeforeOptions, infoAfterOptions) { }

        public override async Task PerformAction(string input)
        {
            var allOptions = new List<CLIOption>();
            allOptions.AddRange(options);
            allOptions.AddRange(navigationOptions);
            int totalOptions = allOptions.Count;

            if (int.TryParse(input, out int enteredNumber))
            {
                if (enteredNumber > options.Count && enteredNumber <= totalOptions)
                {
                    state = CLIScreenState.Complete;
                    await RunCommand(allOptions, enteredNumber);
                    return;
                }
            }

            var runSteps = new List<int>();
            var skipSteps = new List<int>();

            if (input.Equals(""))
            {
                for (int i = 1; i <= options.Count; i++)
                {
                    runSteps.Add(i);
                }
            }
            else
            {
                string[] commaSeparatedInput = input.Split(',');
                try
                {
                    int[] enteredNumbers = ParseInputList(commaSeparatedInput);
                    PartitionBySign(enteredNumbers, positives: runSteps, negatives: skipSteps);
                }
                catch (Exception ex)
                {
                    inputValidationError = ex.Message;
                    return;
                }
            }

            if (runSteps.Count > 0 && skipSteps.Count > 0)
            {
                inputValidationError = "Either choose a set of steps to skip or a set of steps to run.";
                return;
            }

            state = CLIScreenState.Complete;

            if (runSteps.Count == 0)
            {
                runSteps = FindStepsNotSkipped(skipSteps);
            }

            runSteps.Sort();
            for (int i = 0; i < runSteps.Count; i++)
            {
                await RunCommand(options, runSteps[i]);
            }
        }

        private List<int> FindStepsNotSkipped(List<int> skipSteps)
        {
            var runSteps = new List<int>();

            for (int step = 1; step <= options.Count; step++)
            {
                if (!skipSteps.Contains(step))
                {
                    runSteps.Add(step);
                }
            }
            return runSteps;
        }

        private void PartitionBySign(int[] enteredNumbers, List<int> positives, List<int> negatives)
        {
            foreach (int number in enteredNumbers)
            {
                if (number < 0 && !negatives.Contains(-number))
                {
                    negatives.Add(-number);
                }
                if (number > 0 && !positives.Contains(number))
                {
                    positives.Add(number);
                }
            }
        }

        private int[] ParseInputList(string[] commaSeparatedInput)
        {
            int[] numbers = new int[commaSeparatedInput.Length];
            for (int i = 0; i < commaSeparatedInput.Length; i++)
            {
                string substring = commaSeparatedInput[i];
                substring.Trim();
                if (int.TryParse(substring, out int number))
                {
                    numbers[i] = number;
                }
                else
                {
                    throw new Exception($"Invalid input. \"{ substring }\" is not a number.");
                }
            }
            foreach (int number in numbers)
            {
                if (number < -options.Count || number > options.Count || number == 0)
                {
                    throw new Exception($"Operation {number} not found.");
                }
            }
            return numbers;
        }
    }
}
