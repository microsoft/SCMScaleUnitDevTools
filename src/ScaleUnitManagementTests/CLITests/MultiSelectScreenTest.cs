using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ScaleUnitManagementTests.CLITests
{
    [TestClass]
    public sealed class MultiSelectScreenTest
    {
        private readonly List<int> executedSteps = new List<int>();
        private MultiSelectScreen screen;

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
            screen = new MultiSelectScreen(options, "Home", "", "");
        }

        private Task ExecuteStep(int input, string selectionHistory)
        {
            executedSteps.Add(input);
            return Task.CompletedTask;
        }

        [TestMethod]
        public async Task EnterAllSteps_ExecutesAllSteps()
        {
            // Arrange + Act
            await screen.PerformAction("1,2,3,4,5");

            // Assert
            executedSteps.Should().BeEquivalentTo(new List<int> { 1, 2, 3, 4, 5 });
            screen.inputValidationError.Should().BeEmpty();
        }

        [TestMethod]
        public async Task EnterNothing_ExecutesAllSteps()
        {
            // Arrange + Act
            await screen.PerformAction("");

            // Assert
            executedSteps.Should().BeEquivalentTo(new List<int> { 1, 2, 3, 4, 5 });
            screen.inputValidationError.Should().BeEmpty();
        }

        [TestMethod]
        public async Task EnterAllStepsBackwards_ExecutesAllStepsInOrder()
        {
            // Arrange + Act
            await screen.PerformAction("5,4,3,2,1");

            // Assert
            executedSteps.Should().BeEquivalentTo(new List<int> { 1, 2, 3, 4, 5 });
            screen.inputValidationError.Should().BeEmpty();
        }

        [TestMethod]
        public async Task SkipAllSteps_ExecutesNothing()
        {
            // Arrange + Act
            await screen.PerformAction("-1,-2,-3,-4,-5");

            // Assert
            executedSteps.Should().BeEmpty();
            screen.inputValidationError.Should().BeEmpty();
        }

        [TestMethod]
        public async Task RunStepMultipleTimes_RunsStepOnce()
        {
            // Arrange + Act
            await screen.PerformAction("1,2,3,1");

            // Assert
            executedSteps.Should().BeEquivalentTo(new List<int> { 1, 2, 3 });
            screen.inputValidationError.Should().BeEmpty();
        }

        [TestMethod]
        public async Task RunStepInterval_RunsAllStepsInInterval()
        {
            // Arrange + Act
            await screen.PerformAction("1-4");

            // Assert
            executedSteps.Should().BeEquivalentTo(new List<int> { 1, 2, 3, 4 });
            screen.inputValidationError.Should().BeEmpty();
        }

        [TestMethod]
        public async Task RunCombinationOfNumbersAndIntervals_RunsAllStepsOnce()
        {
            // Arrange + Act
            await screen.PerformAction("1-3,1,5");

            // Assert
            screen.inputValidationError.Should().BeEmpty();
            executedSteps.Should().BeEquivalentTo(new List<int> { 1, 2, 3, 5 });
        }

        [TestMethod]
        public async Task RunIntervalWithUnknownOperation_ThrowsException()
        {
            // Arrange + Act
            await screen.PerformAction("1 - 6");

            // Assert
            screen.inputValidationError.Should().Be("Operation 6 not found.");
            executedSteps.Should().BeEmpty();
        }

        [TestMethod]
        public async Task SkipAndRunSteps_ThrowsException()
        {
            // Arrange + Act
            await screen.PerformAction("-1,2");

            // Assert
            executedSteps.Should().BeEmpty();
            screen.inputValidationError.Should().Be("Either choose a set of options to skip or a set of options to run.");
        }

        [TestMethod]
        public async Task RunUnknownStep_ThrowsException()
        {
            // Arrange + Act
            await screen.PerformAction("19");

            // Assert
            executedSteps.Should().BeEmpty();
            screen.inputValidationError.Should().Be("Operation 19 not found.");
        }

        [TestMethod]
        public async Task RunKnownStep_ClearsError()
        {
            // Arrange + Act
            await screen.PerformAction("19");
            await screen.PerformAction("3");

            // Assert
            screen.inputValidationError.Should().BeEmpty();
            executedSteps.Should().BeEquivalentTo(new List<int> { 3 });
        }

        [TestMethod]
        public async Task EnterNonNumber_ThrowsException()
        {
            // Arrange + Act
            await screen.PerformAction("ABC");

            // Assert
            executedSteps.Should().BeEmpty();
            screen.inputValidationError.Should().Be("Invalid input. \"ABC\" is not a number or interval.");
        }
    }
}
