using Snapper;
using SourceMapper.Tests.MappingTestObjects;
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

        [Fact]
        public void WhenTargetMissesProp_OtherPropIsStillMapped()
        {
            // Arrange
            var src = new MissingTgtPropSource();

            // Act
            var mapped = src.MapTo<MissingTgtPropTarget>();

            // Assert
            mapped.ShouldMatchSnapshot();
        }

        [Fact]
        public void WhenPropCombinedViaLambda_PropHasCorrectValue()
        {
            // Arrange
            var src = new CombinePropSource();

            // Act
            var mapped = src.MapTo<CombinePropTarget>();

            // Assert
            mapped.ShouldMatchSnapshot();
        }
    }
}
