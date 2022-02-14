using System;
using CLI.Utilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ScaleUnitManagementTests
{
    [TestClass]
    public sealed class ArgumentParserTest
    {
        private ArgumentParser argumentParser;

        [TestInitialize]
        public void Setup()
        {
            argumentParser = new ArgumentParser();
        }

        [TestMethod]
        public void BeforeParsing_HasInitialValues()
        {
            // Arrange + Act + Assert
            argumentParser.Deploy.Should().BeFalse();
            argumentParser.CleanStorage.Should().BeFalse();
            argumentParser.DrainPipelines.Should().BeFalse();
            argumentParser.StartPipelines.Should().BeFalse();
        }

        [TestMethod]
        public void Parse_WithValidArguments_SetsValues()
        {
            // Arrange + Act
            string[] arguments = { "--single-box-deploy", "--clean-storage", "--drain-pipelines", "--start-pipelines" };
            argumentParser.Parse(arguments);

            // Assert
            argumentParser.Deploy.Should().BeTrue();
            argumentParser.CleanStorage.Should().BeTrue();
            argumentParser.DrainPipelines.Should().BeTrue();
            argumentParser.StartPipelines.Should().BeTrue();
        }

        [TestMethod]
        public void Parse_WithInvalidArgument_ThrowsError()
        {
            // Arrange
            string[] arguments = { "--unknown-argument" };

            // Act
            Action act = () => argumentParser.Parse(arguments);

            // Assert
            act.Should().Throw<Exception>();
        }
    }
}
