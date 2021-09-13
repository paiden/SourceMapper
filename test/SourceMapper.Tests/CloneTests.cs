using FluentAssertions;
using Snapper;
using Xunit;

namespace SourceMapper.Tests
{
    public class CloneTests
    {
        [Fact]
        public void WhenPropertyIsCloneable_UsesCloneForAssignments()
        {
            // Arrange
            var source = WithCloneablePropertyDto.Source;

            // Act
            var clone = source.Clone();

            // Assert
            clone.Inner.Should().NotBeSameAs(source.Inner);
            clone.InnerGetOnly.Should().NotBeSameAs(source.InnerGetOnly);
            clone.ShouldMatchSnapshot();
        }

        [Fact]
        public void WhenPorpertyIsIgnored_CloneIgnoresTheProperty()
        {
            // Arrange
            var ign = new IgnorePropDto() { IgnoreMe = "Cloned", CloneMe = "Cloned" };

            // Act
            var cloned = ign.Clone();

            // Assert
            cloned.ShouldMatchSnapshot();
        }

        [Fact]
        public void WhenPostProcessingFuncSet_ExecutesItAfterStandardClone()
        {
            // Arrange
            var src = new ObjWithPostProcessFunc() { Prop = "Not Post Processed" };

            // Act
            var cloned = src.Clone();

            // Assert
            cloned.ShouldMatchSnapshot();
        }

        [Fact]
        public void WhenPostProcessingLambdaFuncSet_ExecutesItAfterStandardClone()
        {
            // Arrange
            var src = new LambdaPostProcObj() { Prop = "Not Post Processed" };

            // Act
            var cloned = src.Clone();

            // Assert
            cloned.ShouldMatchSnapshot();
        }

        [Fact]
        public void WhenMakeInstanceFuncSet_UsesMakeInstanceFunc()
        {
            // Arrange
            var src = new LambdaActivatorObj("Not created by custom Lambda!!!") { ShouldBeCloned = "This was cloned" };

            // Act
            var clone = src.Clone();

            // Assert
            clone.ShouldMatchSnapshot();
        }
    }
}
