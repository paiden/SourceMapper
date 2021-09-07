namespace SourceMapper.Tests
{
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

    public class VisiblityDto
    {
        public string PublicString { get; set; }
        internal string InternalString { get; set; }
        private string PrivateString { get; set; }
    }

    public class ReadonlyDto
    {
        public string PrivateSetter { get; private set; }
        public string GetterOnly { get; }
        public string Calculated => "Calc";
    }

    public class NoDefConstDto
    {
        public string X { get; set; }

        public NoDefConstDto(string x)
        {
            this.X = x;
        }
    }


    public class IgnorePropDto
    {
        public string CloneMe { get; set; } = "Ignored";

        public string IgnoreMe { get; set; } = "Ignored";

        public IgnorePropDto()
        {

        }

        public static readonly IgnorePropDto Source = new IgnorePropDto() { CloneMe = "Cloned", IgnoreMe = "Cloned" };
    }

    public class WithCloneablePropertyDto
    {
        public string Prop { get; set; } = "Original";

        public IgnorePropDto Inner { get; set; } = new();

        public IgnorePropDto InnerGetOnly { get; private set; }

        public WithCloneablePropertyDto(IgnorePropDto innerGetOnly)
        {
            this.InnerGetOnly = innerGetOnly;
        }

        public static readonly WithCloneablePropertyDto Source = new(IgnorePropDto.Source)
        {
            Prop = "Cloned",
            Inner = IgnorePropDto.Source,
            InnerGetOnly = IgnorePropDto.Source,
        };
    }
}
