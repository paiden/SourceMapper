using Snapper;
using Xunit;

namespace SourceMapper.Tests
{
    public class MapToTests
    {
        [Fact]
        public void WhenCustomActivatorSet_UsesThatActivator()
        {
            // Arrange
            var src = new LambdaActivatorObj("Not created by custom Lambda!!!") { ShouldBeCloned = "This was cloned" };

            // Act
            var mapped = src.MapTo<FuncActivatorObj>();

            // Assert
            mapped.ShouldMatchSnapshot();
        }
    }
}
