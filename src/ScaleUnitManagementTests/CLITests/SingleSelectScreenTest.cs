using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ScaleUnitManagementTests.CLITests
{
    [TestClass]
    public sealed class SingleSelectScreenTest
    {
        private int executedStep;
        private SingleSelectScreen screen;

        [TestInitialize]
        public void setupMenu()
        {
            var options = new List<CLIOption>
            {
                new CLIOption() { Name = "step", Command = ExecuteStep},
                new CLIOption() { Name = "step", Command = ExecuteStep},
                new CLIOption() { Name = "step", Command = ExecuteStep},
                new CLIOption() { Name = "step", Command = ExecuteStep},
                new CLIOption() { Name = "step", Command = ExecuteStep},
            };
            screen = new SingleSelectScreen(options, "Home", "", "");
        }

        private Task ExecuteStep(int input, string selectionHistory)
        {
            executedStep = input;
            return Task.CompletedTask;
        }

        [TestMethod]
        public async Task EnterValidStep_ExecutesStep()
        {
            // Arrange + Act
            await screen.PerformAction("1");

            // Assert
            executedStep.Should().Be(1);
            screen.inputValidationError.Should().BeEmpty();
        }

        [TestMethod]
        public async Task RunUnknownStep_ThrowsException()
        {
            // Arrange + Act
            await screen.PerformAction("19");

            // Assert
            screen.inputValidationError.Should().Be("Operation 19 not found. Please enter the number for the operation you like to start.");
        }

        [TestMethod]
        public async Task RunKnownStep_ClearsError()
        {
            // Arrange + Act
            await screen.PerformAction("19");
            await screen.PerformAction("3");

            // Assert
            screen.inputValidationError.Should().BeEmpty();
            executedStep.Should().Be(3);
        }

        [TestMethod]
        public async Task EnterNonNumber_ThrowsException()
        {
            // Arrange + Act
            await screen.PerformAction("ABC");

            // Assert
            screen.inputValidationError.Should().Be("Invalid input. \"ABC\" is not a number.");
        }
    }
}
