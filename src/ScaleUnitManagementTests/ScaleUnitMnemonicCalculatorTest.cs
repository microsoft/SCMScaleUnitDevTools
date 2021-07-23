using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using ScaleUnitManagement.Utilities;
using System;

namespace ScaleUnitManagementTests
{
    [TestClass]
    public class ScaleUnitMnemonicCalculatorTest
    {
        
        [TestMethod]
        public void ToIntegralValue_WithValidMnemonic_ReturnsIntegral()
        {
            // Arrange + Act + Assert
            ScaleUnitMnemonicCalculator.ToIntegralValue("@@").Should().Be(0);
            ScaleUnitMnemonicCalculator.ToIntegralValue("AB").Should().Be(29);
            ScaleUnitMnemonicCalculator.ToIntegralValue("DAD").Should().Be(2947);
            ScaleUnitMnemonicCalculator.ToIntegralValue("@@@H").Should().Be(8);
        }

        [TestMethod]
        public void ToIntegralValue_WithInvalidMnemonic_ThrowsException()
        {
            // Arrange
            bool exceptionThrown = false;

            // Act
            try
            {
                ScaleUnitMnemonicCalculator.ToIntegralValue("1");
            }
            catch (Exception)
            {
                exceptionThrown = true;
            }

            exceptionThrown.Should().BeTrue();
        }

        [TestMethod]
        public void ToMnemonic_WithValidIntegral_ReturnsMnemonic()
        {
            // Arrange + Act + Assert
            ScaleUnitMnemonicCalculator.ToMnemonic(0).Should().Be("@@");
            ScaleUnitMnemonicCalculator.ToMnemonic(29).Should().Be("AB");
            ScaleUnitMnemonicCalculator.ToMnemonic(2947).Should().Be("DAD");
            ScaleUnitMnemonicCalculator.ToMnemonic(8).Should().Be("@H");
        }

        [TestMethod]
        public void ToMnemonic_WithInvalidInteger_ThrowsException()
        {
            // Arrange
            bool exceptionThrown = false;

            // Act
            try
            {
                ScaleUnitMnemonicCalculator.ToMnemonic(-1);
            }
            catch (Exception)
            {
                exceptionThrown = true;
            }

            exceptionThrown.Should().BeTrue();
        }
    }
}
