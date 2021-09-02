using Snapper;
using Xunit;

namespace SourceMapper.Tests.CloneTests
{
    public class IgnoreTests
    {
        [Fact]
        public void Clone_DoesNotCopyIngoredProperty()
        {
            // Arrange
            var ign = new IgnorePropDto() { IgnoreMe = "Cloned", CloneMe = "Cloned" };

            // Act
            var cloned = ign.Clone();

            // Assert
            cloned.ShouldMatchSnapshot();
        }
    }
}
