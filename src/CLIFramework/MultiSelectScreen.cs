using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CLIFramework
{
    public class MultiSelectScreen : CLIScreen
    {
        public MultiSelectScreen(List<CLIOption> options, string selectionHistory, string infoBeforeOptions = "", string infoAfterOptions = "")
            : base(options, selectionHistory, infoBeforeOptions, infoAfterOptions) { }

        public override async Task PerformAction(string input)
        {
            inputValidationError = "";

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

            var runOptions = new List<int>();
            var skipOptions = new List<int>();

            if (input.Equals(""))
            {
                for (int i = 1; i <= options.Count; i++)
                {
                    runOptions.Add(i);
                }
            }
            else
            {
                string[] commaSeparatedInput = input.Split(',');
                try
                {
                    List<int> enteredNumbers = ParseInputList(commaSeparatedInput);
                    PartitionBySign(enteredNumbers, positives: runOptions, negatives: skipOptions);
                }
                catch (Exception ex)
                {
                    inputValidationError = ex.Message;
                    return;
                }
            }

            if (runOptions.Count > 0 && skipOptions.Count > 0)
            {
                inputValidationError = "Either choose a set of options to skip or a set of options to run.";
                return;
            }

            state = CLIScreenState.Complete;

            if (runOptions.Count == 0)
            {
                runOptions = FindOptionsNotSkipped(skipOptions);
            }

            runOptions.Sort();

            Console.WriteLine($"You selected: {string.Join(", ", runOptions)} \n");
            for (int i = 0; i < runOptions.Count; i++)
            {
                await RunCommand(options, runOptions[i]);
            }
        }

        private List<int> FindOptionsNotSkipped(List<int> skipOptions)
        {
            var runOptions = new List<int>();

            for (int option = 1; option <= options.Count; option++)
            {
                if (!skipOptions.Contains(option))
                {
                    runOptions.Add(option);
                }
            }
            return runOptions;
        }

        private void PartitionBySign(List<int> enteredNumbers, List<int> positives, List<int> negatives)
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

        private List<int> ParseInputList(string[] commaSeparatedInput)
        {
            var dashSeparatedNumbers = new Regex(@"^(\d+)\s*-\s*(\d+)");

            var numbers = new List<int>();
            for (int i = 0; i < commaSeparatedInput.Length; i++)
            {
                string substring = commaSeparatedInput[i];
                if (int.TryParse(substring, out int number))
                {
                    numbers.Add(number);
                }
                else if (dashSeparatedNumbers.IsMatch(substring))
                {
                    numbers.AddRange(GetNumberInterval(substring));
                }
                else
                {
                    throw new Exception($"Invalid input. \"{substring}\" is not a number or interval.");
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

        private List<int> GetNumberInterval(string intervalString)
        {
            var numbers = new List<int>();

            string[] substrings = intervalString.Split('-');
            if (substrings.Length != 2)
            {
                throw new Exception($"Expected {intervalString} to be a dash-separated interval");
            }

            if (int.TryParse(substrings[0], out int intervalStart) &&
                int.TryParse(substrings[1], out int intervalEnd))
            {
                for (int i = intervalStart; i<= intervalEnd; i++)
                {
                    numbers.Add(i);
                }

                return numbers;
            }
            else
            {
                throw new Exception($"Could not parse the endpoints of the interval {intervalString}");
            }
        }
    }
}
