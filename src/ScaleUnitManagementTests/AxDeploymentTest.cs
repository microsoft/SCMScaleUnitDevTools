using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagementTests
{
    [TestClass]
    public class AxDeploymentTest
    {
        class AxDeploymentTestable : AxDeployment
        {
            public static void SetApplicationVersion(string version)
            {
                AxDeploymentTestable testable = new AxDeploymentTestable();
                testable.ApplicationVersion = version;
            }
        }

        [TestMethod]
        public void IsApplicationVersionLaterThan_WithLaterVersion_ReturnsTrue()
        {
            // Arrange
            AxDeploymentTestable.SetApplicationVersion("10.8.107.1233");

            // Act + Assert
            AxDeployment.IsApplicationVersionLaterThan("10.8.107.1234").Should().BeTrue();
            AxDeployment.IsApplicationVersionLaterThan("10.8.108.0").Should().BeTrue();
            AxDeployment.IsApplicationVersionLaterThan("10.9.10.0").Should().BeTrue();
            AxDeployment.IsApplicationVersionLaterThan("11.0.107.1234").Should().BeTrue();
        }

        [TestMethod]
        public void IsApplicationVersionLaterThan_WithPreviousVersion_ReturnsFalse()
        {
            // Arrange
            AxDeploymentTestable.SetApplicationVersion("10.8.107.1233");

            // Act + Assert
            AxDeployment.IsApplicationVersionLaterThan("10.8.107.1232").Should().BeFalse();
            AxDeployment.IsApplicationVersionLaterThan("10.8.106.1333").Should().BeFalse();
            AxDeployment.IsApplicationVersionLaterThan("10.7.120.1230").Should().BeFalse();
            AxDeployment.IsApplicationVersionLaterThan("9.1000.1.1234").Should().BeFalse();
        }

        [TestMethod]
        public void IsApplicationVersionLaterThan_WithEqualVersion_ReturnsFalse()
        {
            // Arrange
            AxDeploymentTestable.SetApplicationVersion("10.8.107.1233");

            // Act + Assert
            AxDeployment.IsApplicationVersionLaterThan("10.8.107.1233").Should().BeFalse();
        }
    }
}
