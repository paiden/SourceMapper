# Introduction 
Source Mapper is a [Source Generator](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview)
based version of functionality similar to [AutoMapper](https://automapper.org/)

At the moment is a experimental project used to learn how to create source generators and implement 
advanced code based configuration systems for these.

# Getting Started

1. Make sure you use VS2022 with Latest C# Language version
2. Install the [Source Mapper Nuget Package](https://www.nuget.org/packages/SourceMapper)
3. Configure a SourceMapper Context to auto generate Cloning/Mapping extension methods (see Configuration section below)

# Configuration

SourceMapper different to many other source generators does not use a Attribute based linking of user and generated code.
SourceMapper uses a fluent style code configuration system. 

The fluent code is not real code but instead is only lightweight compiler checked configuration code that will be parsed
by the source generator. You should not split it into helper methods etc. as this will definitely break parser logic.

The examples below should outline, what valid configurations look like.

Reasons to use this system: 

- Better extensibility than attributes
- No modification of mapped types needed
- All configuration in one place

## Make a object deep cloneable 

Cloning will create a new object instance of the same type. Properties that have a type that is 
also cloneable will also be cloned, otherwise reference assignment will be done.

```csharp
    // Input objects
    public class DataTransferObject
    {
        public string Property { get; set; }

        public InnerDataTranferObject Inner { get; set; } = new InnerDataTranferObject();
    }

    public class InnerDataTranferObject
    {
        public int Property { get; set; }
    }

    // Configuration
    internal class CloneSampleContext : SourceMapperContext
    {
        protected override void Configure(ContextConfig config)
        {
            config
                .Make<DataTransferObject>(it => it.Cloneable())
                .Make<InnerDataTranferObject>(it => it.Cloneable());
        }
    }

    // Usage
    public class CloneUsage
    {
        public static void Use()
        {
            var cloneable = new DataTransferObject();
            var cloned = cloneable.Clone();
        }
    }
```

The generated source code

```csharp
namespace Samples
{
    public static class InnerDataTranferObjectCloneSampleContextSourceMapperExtensions
    {
        public static InnerDataTranferObject Clone(this InnerDataTranferObject source)
        {
            var obj = new InnerDataTranferObject();

            obj.Property = source.Property;

            return obj;
        }
    }
}

namespace Samples
{
    public static class DataTransferObjectCloneSampleContextSourceMapperExtensions
    {
        public static DataTransferObject Clone(this DataTransferObject source)
        {
            var obj = new DataTransferObject();

            obj.Property = source.Property;
            obj.Inner = source.Inner.Clone();

            return obj;
        }
    }
}
```

## Make a object mapable 

Mapping will map a object of some type to another type. If public get/set property names match the property will be 
assigned from the source property to the target property. If the source property type is cloneable, a clone will be 
assigned otherwise direct reference assignment will be done.

```csharp
    // Input Objects
    public class A { public int Property { get; set; } }
    public class B {  public int Property { get; set; } }

    // Configuration
    internal class MappingSampleContext : SourceMapperContext
    {
        protected override void Configure(ContextConfig config)
        {
            config.Make<A>(it => it.MapTo<B>())
                .Make<B>(it => it.MapTo<A>());
        }
    }

    // Usage
    public static class MapToUsage
    {
        public static void Use()
        {
            var mapMe = new A();
            var b = mapMe.MapTo<B>();
        }
    }
```

Generated Code

```csharp
namespace SourceMapper.Samples
{
    public static class BMappingSampleContextSourceMapperExtensions
    {
        public static A MapTo<T>(this B source) where T : A
        {
            var obj = new A();

            obj.Property = source.Property;

            return obj;
        }
    }
}

namespace SourceMapper.Samples
{
    public static class AMappingSampleContextSourceMapperExtensions
    {
        public static B MapTo<T>(this A source) where T : B
        {
            var obj = new B();

            obj.Property = source.Property;

            return obj;
        }
    }
}

```

# Custom object instantiation

Sometimes objects needs custom initialization DI etc. You can set a custom init method for objects via the Activator config method.

```csharp
    // Input objects
    public record RecA(int X)
    {
        public string Prop { get; set; }
    }

    public record RecB(int Y)
    {
        public static RecB MapActivator(RecA src)
        {
            return new RecB(src.X + 1);
        }

        public string Prop { get; set; }
    }

    // Configuration
    internal class ActivatorSampleContext : SourceMapperContext
    {
        protected override void Configure(ContextConfig config)
        {
            config
                .Make<RecA>(it => it
                    .Cloneable(cloning => cloning
                        .Activator(src => new RecA(123)))
                    .MapTo<RecB>(mapping => mapping
                        .Activator(RecB.MapActivator)));
        }
    }
```

Generated code: 

```csharp 
    public static class RecAActivatorSampleContextSourceMapperExtensions
    {
        public static RecA Clone(this RecA source)
        {
            Func<RecA, RecA> instanceCreator = src => new RecA(123);
            var obj = instanceCreator(source);

            obj.Prop = source.Prop;

            return obj;
        }
        public static RecB MapTo<T>(this RecA source) where T : RecB
        {
            var obj = RecB.MapActivator(source);

            obj.Prop = source.Prop;

            return obj;
        }
    }
```

# Post Processing 

```csharp
    // Input Objects
    public class PostProcessSource
    {
        public string A { get; set; }
        public string B { get; set; }
    }

    public class PostProcessTarget { public string AB { get; set; } }

    // Configuration
    internal class PostProcessSampleContext : SourceMapperContext
    {
        protected override void Configure(ContextConfig config)
        {
            config
                .Make<PostProcessSource>(it => it
                    .MapTo<PostProcessTarget>(mapping => mapping
                        .Ignore(src => src.A)
                        .Ignore(src => src.B)
                        .PostProcess((ref PostProcessTarget tgt, PostProcessSource src) => tgt.AB = src.A + src.B)));
        }
    }
```

Generated code

```csharp
    public static class PostProcessSourcePostProcessSampleContextSourceMapperExtensions
    {
        public static PostProcessTarget MapTo<T>(this PostProcessSource source) where T : PostProcessTarget
        {
            var obj = new PostProcessTarget();

            var postProcess = (ref PostProcessTarget tgt, PostProcessSource src) => tgt.AB = src.A + src.B;
            postProcess(ref obj, source);

            return obj;
        }
    }
```

# Build and Test
Checkout repository, open with VS2022+ and compile the project.
