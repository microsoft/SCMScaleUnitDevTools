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

            while (screen.state == CLIScreenState.Incomplete)
            {
                PrintScreen(screen);
                await PerformScreenAction(screen);
            }

            currentScreen = screen.previousScreen;
        }

        /// <summary>
        /// Prints the contents of the provided screen.
        /// </summary>
        /// <param name="screen">The <c>CLIScreen</c> object to print.</param>
        private static void PrintScreen(CLIScreen screen)
        {
            var totalOptions = screen.options.Count;

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
            var currOptionNumber = 1;

            foreach (var option in screen.options)
            {
                Console.WriteLine("\t" + currOptionNumber.ToString() + ". " + option.Name);
                currOptionNumber++;
            }

            if (screen.previousScreen != null)
            {
                Console.WriteLine("\t" + currOptionNumber.ToString() + ". " + "Back");
            }
            else
            {
                Console.WriteLine("\t" + currOptionNumber.ToString() + ". " + "Exit");
            }
        }

        /// <summary>
        /// This function performs the action indicated by the user's input. 
        /// </summary>
        private static async Task PerformScreenAction(CLIScreen screen)
        {
            var totalOptions = screen.options.Count;
            var input = Console.ReadLine();
            if (int.TryParse(input, out var enteredNumber))
            {
                if (enteredNumber >= 1 && enteredNumber <= totalOptions)
                {
                    screen.state = CLIScreenState.Complete;
                    await screen.options[enteredNumber - 1].Command(enteredNumber, string.IsNullOrEmpty(screen.selectionHistory) ? screen.options[enteredNumber - 1].Name : screen.selectionHistory + " -> " + screen.options[enteredNumber - 1].Name);
                }

                else if (enteredNumber == totalOptions + 1)
                {
                    if (screen.previousScreen != null)
                    {
                        screen.previousScreen.state = CLIScreenState.Incomplete; // show previous screen again.
                        screen.state = CLIScreenState.Complete;
                    }
                    else
                    {
                        repeat = false;
                        screen.state = CLIScreenState.Complete;
                    }
                    return;
                }

                else
                {
                    screen.inputValidationError = "Operation " + enteredNumber + " not found. Please enter the number for the operation you like to start.";
                    return;
                }
            }
            else
            {
                screen.inputValidationError = "Invalid input. Please enter a number.";
                return;
            }
        }

        public static bool YesNoPrompt(string message)
        {
            var input = EnterValuePrompt(message).ToLower();
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
