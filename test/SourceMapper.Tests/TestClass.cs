namespace SourceMapper.Tests
{
    [MapsInto(typeof(TestClassDto))]
    public class TestClass
    {
        public float F { get; set; } = 1.0f;

        public double D { get; set; } = 2.0;

        public string S { get; set; } = "s";
    }

    public class TestClassDto
    {
        public string F { get; set; }

        public double D { get; set; }

        public string S { get; set; }
    }
}
