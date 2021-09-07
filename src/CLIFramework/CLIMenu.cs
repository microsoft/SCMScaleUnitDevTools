using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CLIFramework
{
    public class CLIMenu
    {
        static CLIScreen currentScreen;

        /// <summary>
        /// This function displays the provided screen.
        /// </summary>
        public static async Task ShowScreen(CLIScreen screen)
        {
            screen.PreviousScreen = currentScreen;
            currentScreen = screen;

            while (screen.state == CLIScreenState.Incomplete)
            {
                PrintScreen(screen);
                await PerformScreenAction(screen);
            }

            currentScreen = screen.PreviousScreen;
        }

        /// <summary>
        /// Prints the contents of the provided screen.
        /// </summary>
        /// <param name="screen">The <c>CLIScreen</c> object to print.</param>
        private static void PrintScreen(CLIScreen screen)
        {
            int totalOptions = screen.Options.Count;

            if (totalOptions == 0)
            {
                screen.state = CLIScreenState.Complete;
            }

            if (String.IsNullOrEmpty(screen.InputValidationError))
            {
                Console.Clear();

                if (!String.IsNullOrEmpty(screen.SelectionHistory))
                    Console.WriteLine("Selection History: " + screen.SelectionHistory + "\n");

                Console.WriteLine(String.IsNullOrEmpty(screen.InfoBeforeOptions) ? "The list of options to choose from:\n" : screen.InfoBeforeOptions);

                DisplayOptions(screen);
            }
            else
            {
                Console.WriteLine(screen.InputValidationError);
            }

            Console.Write(String.IsNullOrEmpty(screen.InfoAfterOptions) ? "Please enter the number(1/2/..) for the operation you'd like to start: " : screen.InfoAfterOptions);
        }

        private static void DisplayOptions(CLIScreen screen)
        {
            int currOptionNumber = 1;

            foreach (CLIOption option in screen.Options)
            {
                Console.WriteLine("\t" + currOptionNumber.ToString() + ". " + option.Name);
                currOptionNumber++;
            }

            if (screen.PreviousScreen != null)
            {
                Console.WriteLine("\t" + currOptionNumber.ToString() + ". " + "Back");
            }
        }

        /// <summary>
        /// This function performs the action indicated by the user's input. 
        /// </summary>
        private static async Task PerformScreenAction(CLIScreen screen)
        {
            int totalOptions = screen.Options.Count;
            string input = Console.ReadLine();
            if (int.TryParse(input, out int enteredNumber))
            {
                if (enteredNumber >= 1 && enteredNumber <= totalOptions)
                {
                    screen.state = CLIScreenState.Complete;
                    await screen.Options[enteredNumber - 1].Command(enteredNumber, String.IsNullOrEmpty(screen.SelectionHistory) ? screen.Options[enteredNumber - 1].Name : screen.SelectionHistory + " -> " + screen.Options[enteredNumber - 1].Name);
                }

                else if (screen.PreviousScreen != null && enteredNumber == totalOptions + 1)
                {
                    screen.PreviousScreen.state = CLIScreenState.Incomplete; // show previous screen again.
                    screen.state = CLIScreenState.Complete;
                    return;
                }

                else
                {
                    screen.InputValidationError = "Operation " + enteredNumber + " not found. Please enter the number for the operation you like to start.";
                    return;
                }
            }
            else
            {
                screen.InputValidationError = "Invalid input. Please enter a number.";
                return;
            }
        }

        public static bool YesNoPrompt(string message)
        {
            string input = EnterValuePrompt(message).ToLower();
            return String.IsNullOrEmpty(input) || input == "y" || input == "yes";
        }

        public static void PressToContinue()
        {
            Console.WriteLine("\nPress enter to continue");
            string input = Console.ReadLine();
        }

        public static string EnterValuePrompt(string message)
        {
            Console.WriteLine(message);
            return Console.ReadLine();
        }
    }
}
