using System;
using CLI;
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
        }

        [TestMethod]
        public void Parse_WithValidArguments_SetsValues()
        {
            // Arrange + Act
            string[] arguments = { "--single-box-deploy", "--clean-storage" };
            argumentParser.Parse(arguments);

            // Assert
            argumentParser.Deploy.Should().BeTrue();
            argumentParser.CleanStorage.Should().BeTrue();
        }

        [TestMethod]
        public void Parse_WithInvalidArgument_ThrowsError()
        {
            // Arrange
            string[] arguments = { "--single-box-deploy --unknown-argument" };

            // Act
            Action act = () => argumentParser.Parse(arguments);

            // Assert
            act.Should().Throw<Exception>();
        }
    }
}
