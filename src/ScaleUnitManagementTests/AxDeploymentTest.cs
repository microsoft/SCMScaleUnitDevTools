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
        public void IsApplicationVersionMoreRecentThan_WithLaterVersion_ReturnsFalse()
        {
            // Arrange
            AxDeploymentTestable.SetApplicationVersion("10.8.107.1233");

            // Act + Assert
            AxDeployment.IsApplicationVersionMoreRecentThan("10.8.107.1234").Should().BeFalse();
            AxDeployment.IsApplicationVersionMoreRecentThan("10.8.108.0").Should().BeFalse();
            AxDeployment.IsApplicationVersionMoreRecentThan("10.9.10.0").Should().BeFalse();
            AxDeployment.IsApplicationVersionMoreRecentThan("11.0.107.1234").Should().BeFalse();
        }

        [TestMethod]
        public void IsApplicationVersionMoreRecentThan_WithPreviousVersion_ReturnsTrue()
        {
            // Arrange
            AxDeploymentTestable.SetApplicationVersion("10.8.107.1233");

            // Act + Assert
            AxDeployment.IsApplicationVersionMoreRecentThan("10.8.107.1232").Should().BeTrue();
            AxDeployment.IsApplicationVersionMoreRecentThan("10.8.106.1333").Should().BeTrue();
            AxDeployment.IsApplicationVersionMoreRecentThan("10.7.120.1230").Should().BeTrue();
            AxDeployment.IsApplicationVersionMoreRecentThan("9.1000.1.1234").Should().BeTrue();
        }

        [TestMethod]
        public void IsApplicationVersionMoreRecentThan_WithEqualVersion_ReturnsFalse()
        {
            // Arrange
            AxDeploymentTestable.SetApplicationVersion("10.8.107.1233");

            // Act + Assert
            AxDeployment.IsApplicationVersionMoreRecentThan("10.8.107.1233").Should().BeFalse();
        }

        [TestMethod]
        public void IsApplicationVersionMoreRecentThan_SpecificVersion()
        {
            // Arrange
            AxDeploymentTestable.SetApplicationVersion("10.13.1837");

            // Act + Assert
            AxDeployment.IsApplicationVersionMoreRecentThan("10.13.0").Should().BeTrue();
            AxDeployment.IsApplicationVersionMoreRecentThan("10.13").Should().BeTrue();
        }

    }
}
