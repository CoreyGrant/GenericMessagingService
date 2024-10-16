using GenericMessagingService.Client.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GenericMessagingService.Client.Tests.Utils
{
    public class ClassToDictionaryConverterTests
    {
        private readonly ClassToDictionaryConverter sut;

        public ClassToDictionaryConverterTests()
        {
            sut = new ClassToDictionaryConverter();
        }

        [Fact]
        public void TestObjectToDictionary()
        {
            var testObj = new TestTypeOne 
            { 
                PropString = "PropValue",
                PropInt = 3,
                PropBool = true,
                PropLong = 5,
                PropByte = 6,
                PropDecimal = 7.77m,
                PropFloat = 8.88f,
                PropDouble = 9.99d
            };
            var result = sut.Convert(testObj);
            Assert.NotNull(result);
            Assert.Equal("PropValue", result["PropString"]);
            Assert.Null(result["PropStringNull"]);
            Assert.Equal("", result["PropIntNull"]);
            Assert.Equal("3", result["PropInt"]);
            Assert.Equal("", result["PropBoolNull"]);
            Assert.Equal("True", result["PropBool"]);
            Assert.Equal("", result["PropLongNull"]);
            Assert.Equal("5", result["PropLong"]);
            Assert.Equal("", result["PropByteNull"]);
            Assert.Equal("6", result["PropByte"]);
            Assert.Equal("", result["PropDecimalNull"]);
            Assert.Equal("7.77", result["PropDecimal"]);
            Assert.Equal("", result["PropFloatNull"]);
            Assert.Equal("8.88", result["PropFloat"]);
            Assert.Equal("", result["PropDoubleNull"]);
            Assert.Equal("9.99", result["PropDouble"]);
        }

        [Fact]
        public void TestExplicitMapping()
        {
            var testObj = new TestTypeOne
            {
                PropString = "PropValue",
                PropInt = 3,
                PropBool = true,
                PropLong = 5,
                PropByte = 6,
                PropDecimal = 7.77m,
                PropFloat = 8.88f,
                PropDouble = 9.99d
            };
            sut.AddMapping<TestTypeOne>((o, d) =>
            {
                var tto = (TestTypeOne)o;
                d["OverridePropValue"] = tto.PropString + tto.PropInt;
            });
            var dict = sut.Convert(testObj);
            Assert.Single(dict);
            Assert.Equal("PropValue3", dict["OverridePropValue"]);
        }

        private class TestTypeOne
        {
            public string PropString { get; set; }
            public string? PropStringNull { get; set; }
            public int? PropIntNull { get; set; }
            public int PropInt { get; set; }
            public bool? PropBoolNull { get; set; }
            public bool PropBool { get; set; }
            public long? PropLongNull { get; set; }
            public long PropLong { get; set; }
            public byte? PropByteNull { get; set; }
            public byte PropByte { get; set; }
            public decimal? PropDecimalNull { get; set; }
            public decimal PropDecimal { get; set; }
            public float? PropFloatNull { get; set; }
            public float PropFloat { get; set; }
            public double? PropDoubleNull { get; set; }
            public double PropDouble { get; set; }
        }
    }
}
