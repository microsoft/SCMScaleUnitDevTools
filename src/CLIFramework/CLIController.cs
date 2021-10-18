using System;
using System.Threading.Tasks;

namespace CLIFramework
{
    public class CLIController
    {

        private static CLIScreen currentScreen;
        private static bool repeat;

        /// <summary>
        /// Runs to CLI with root menu until exited
        /// </summary>
        /// <param name="rootMenu">The <c>CLIScreen</c> to return to.after each iteration</param>
        public static async Task Run(CLIMenu rootMenu)
        {
            repeat = true;
            while (repeat)
            {
                await rootMenu.Show(-1, "Home");
                PressToContinue();
            }
        }

        /// <summary>
        /// This function displays the provided screen.
        /// </summary>
        public static async Task ShowScreen(CLIScreen screen)
        {
            screen.previousScreen = currentScreen;
            currentScreen = screen;
            AddNavigationOptions(screen);

            while (screen.state == CLIScreenState.Incomplete)
            {
                PrintScreen(screen);

                string input = Console.ReadLine();
                await screen.PerformAction(input);
            }

            currentScreen = screen.previousScreen;
        }

        private static void AddNavigationOptions(CLIScreen screen)
        {
            if (screen.navigationOptions.Count == 0)
            {
                if (screen.previousScreen is null)
                {
                    screen.navigationOptions.Add(new CLIOption() { Name = "Exit", Command = ExitCLI });
                }
                else
                {
                    screen.navigationOptions.Add(new CLIOption() { Name = "Back", Command = BackToPreviousMenu(screen) });
                }
            }
        }

        private static Task ExitCLI(int input, string selectionHistory)
        {
            repeat = false;
            return Task.CompletedTask;
        }

        private static Func<int, string, Task> BackToPreviousMenu(CLIScreen screen)
        {
            return (input, selectionHistory) =>
            {
                screen.state = CLIScreenState.Complete;
                screen.previousScreen.state = CLIScreenState.Incomplete;
                return Task.CompletedTask;
            };
        }

        /// <summary>
        /// Prints the contents of the provided screen.
        /// </summary>
        /// <param name="screen">The <c>CLIScreen</c> object to print.</param>
        private static void PrintScreen(CLIScreen screen)
        {
            int totalOptions = screen.options.Count;

            if (totalOptions == 0)
            {
                screen.state = CLIScreenState.Complete;
            }

            if (string.IsNullOrEmpty(screen.inputValidationError))
            {
                Console.Clear();

                if (!string.IsNullOrEmpty(screen.selectionHistory))
                    Console.WriteLine("Selection History: " + screen.selectionHistory + "\n");

                Console.WriteLine(string.IsNullOrEmpty(screen.infoBeforeOptions) ? "The list of options to choose from:\n" : screen.infoBeforeOptions);

                DisplayOptions(screen);
            }
            else
            {
                Console.WriteLine(screen.inputValidationError);
            }

            Console.Write(string.IsNullOrEmpty(screen.infoAfterOptions) ? "Please enter the number(1/2/..) for the operation you'd like to start: " : screen.infoAfterOptions);
        }

        private static void DisplayOptions(CLIScreen screen)
        {
            int currOptionNumber = 1;

            foreach (CLIOption option in screen.options)
            {
                Console.WriteLine("\t" + currOptionNumber.ToString() + ". " + option.Name);
                currOptionNumber++;
            }

            foreach (CLIOption option in screen.navigationOptions)
            {
                Console.WriteLine("\t" + currOptionNumber.ToString() + ". " + option.Name);
                currOptionNumber++;
            }

        }

        public static bool YesNoPrompt(string message)
        {
            string input = EnterValuePrompt(message).ToLower();
            return string.IsNullOrEmpty(input) || input == "y" || input == "yes";
        }

        private static void PressToContinue()
        {
            Console.WriteLine("\nPress enter to continue");
            _ = Console.ReadLine();
        }

        public static string EnterValuePrompt(string message)
        {
            Console.WriteLine(message);
            return Console.ReadLine();
        }
    }
}
