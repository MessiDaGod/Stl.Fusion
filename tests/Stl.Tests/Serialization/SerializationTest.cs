using System;
using System.Linq;
using FluentAssertions;
using Stl.Collections;
using Stl.Comparison;
using Stl.DependencyInjection.Internal;
using Stl.Fusion.Authentication;
using Stl.IO;
using Stl.Reflection;
using Stl.Serialization;
using Stl.Testing;
using Stl.Text;
using Stl.Time;
using Xunit;
using Xunit.Abstractions;

namespace Stl.Tests.Serialization
{
    public class SerializationTest : TestBase
    {
        public SerializationTest(ITestOutputHelper @out) : base(@out) { }

        [Fact]
        public void ExceptionInfoSerialization()
        {
            var n = default(ExceptionInfo);
            n.ToException().Should().BeNull();
            n = new ExceptionInfo(null!);
            n = n.AssertPassesThroughAllSerializers(Out);
            n.ToException().Should().BeNull();

            var e = new InvalidOperationException("Fail!");
            var p = e.ToExceptionInfo();
            p = p.AssertPassesThroughAllSerializers(Out);
            var e1 = p.ToException();
            e1.Should().BeOfType<InvalidOperationException>();
            e1!.Message.Should().Be(e.Message);
        }

        [Fact]
        public void TypeDecoratingSerializerTest()
        {
            var serializer = TypeDecoratingSerializer.Default;

            var value = new Tuple<DateTime>(DateTime.Now);
            var json = serializer.Writer.Write(value);
            Out.WriteLine(json);

            var deserialized = (Tuple<DateTime>) serializer.Reader.Read<object>(json);
            deserialized.Item1.Should().Equals(value.Item1);
        }

        [Fact]
        public void MomentSerialization()
        {
            default(Moment).AssertPassesThroughAllSerializers(Out);
            Moment.EpochStart.AssertPassesThroughAllSerializers(Out);
            SystemClock.Now.AssertPassesThroughAllSerializers(Out);
            new Moment(DateTime.MinValue.ToUniversalTime()).AssertPassesThroughAllSerializers(Out);
            new Moment(DateTime.MaxValue.ToUniversalTime()).AssertPassesThroughAllSerializers(Out);
        }

        [Fact]
        public void TypeRefSerialization()
        {
            default(TypeRef).AssertPassesThroughAllSerializers(Out);
            new TypeRef(typeof(bool)).AssertPassesThroughAllSerializers(Out);
        }

        [Fact]
        public void LTagSerialization()
        {
            default(LTag).AssertPassesThroughAllSerializers(Out);
            LTag.Default.AssertPassesThroughAllSerializers(Out);
            new LTag(3).AssertPassesThroughAllSerializers(Out);
            new LTag(-5).AssertPassesThroughAllSerializers(Out);
        }

        [Fact]
        public void SymbolSerialization()
        {
            default(Symbol).AssertPassesThroughAllSerializers(Out);
            Symbol.Empty.AssertPassesThroughAllSerializers(Out);
            new Symbol(null!).AssertPassesThroughAllSerializers(Out);
            new Symbol("").AssertPassesThroughAllSerializers(Out);
            new Symbol("1234").AssertPassesThroughAllSerializers(Out);
        }

        [Fact]
        public void PathStringSerialization()
        {
            default(PathString).AssertPassesThroughAllSerializers(Out);
            PathString.Empty.AssertPassesThroughAllSerializers(Out);
            PathString.New("C:\\").AssertPassesThroughAllSerializers(Out);
        }

        [Fact]
        public void ServiceTypeRefSerialization()
        {
            var s1 = new ServiceTypeRef(typeof(bool)).PassThroughAllSerializers(Out);
            s1.TypeRef.Resolve().Should().Be(typeof(bool));
        }

        [Fact]
        public void JsonStringSerialization()
        {
            default(JsonString).AssertPassesThroughAllSerializers(Out);
            JsonString.Null.AssertPassesThroughAllSerializers(Out);
            JsonString.Empty.AssertPassesThroughAllSerializers(Out);
            new JsonString("1").AssertPassesThroughAllSerializers(Out);
            new JsonString("12").AssertPassesThroughAllSerializers(Out);
        }

        [Fact]
        public void Base64DataSerialization()
        {
            void Test(Base64Encoded src)
            {
                var dst = src.PassThroughAllSerializers(Out);
                src.Data.SequenceEqual(dst.Data).Should().BeTrue();
            }

            Test(default);
            Test(new Base64Encoded(null!));
            Test(new Base64Encoded(new byte[] {}));
            Test(new Base64Encoded(new byte[] {1}));
            Test(new Base64Encoded(new byte[] {1, 2}));
        }

        [Fact]
        public void OptionSerialization()
        {
            default(Option<int>).AssertPassesThroughAllSerializers(Out);
            Option.None<int>().AssertPassesThroughAllSerializers(Out);
            Option.Some(0).AssertPassesThroughAllSerializers(Out);
            Option.Some(1).AssertPassesThroughAllSerializers(Out);
        }

        [Fact]
        public void OptionSetSerialization()
        {
            default(OptionSet).AssertPassesThroughAllSerializers(Out);
            var s = new OptionSet();
            s.Set(3);
            s.Set("X");
            s.Set((1, "X"));
            var s1 = s.PassThroughAllSerializers(Out);
            s1.Items.Should().BeEquivalentTo(s.Items);
        }

        [Fact]
        public void ImmutableOptionSetSerialization()
        {
            default(ImmutableOptionSet).AssertPassesThroughAllSerializers(Out);
            var s = new ImmutableOptionSet();
            s = s.Set(3);
            s = s.Set("X");
            s = s.Set((1, "X"));
            var s1 = s.PassThroughAllSerializers(Out);
            s1.Items.Should().BeEquivalentTo(s.Items);
        }
    }
}
