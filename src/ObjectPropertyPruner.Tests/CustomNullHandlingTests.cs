using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ObjectPropertyPruner.Tests
{
    // TODO : This is a failed attempt at using annotations to selectively prune null fields
    public class CustomNullHandlingTests
    {
        [Fact]
        public void When_UsingCustomNullHandler_ExpectItRemovesNullValuesWhenAnnotated()
        {
            // Arrange
            var input = new AnnotatedTestObject
            {
                // todo : Figure out why the null value is still serialized. This technique works in newtonsoft?!
                Bar = "Foo"
            };
            var options = new JsonSerializerOptions
            {
                Converters = { new CustomStringNullHandlingConverter() }
            };
            // Act
            var actual = JsonSerializer.Serialize(input, options);
            // Assert
            var expected = "{\"Foo\":\"bar\",\"FooBar\":0}";
            actual.Should().Be(expected);
        }
    }

    // I tried object, but got exceptions about not being compatible with System.String or System.Int
    public class CustomStringNullHandlingConverter : JsonConverter<string>
    {
        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                // Exclude null properties
                return;
            }

            // Default serialization behavior
            writer.WriteStringValue(value);
        }

        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Implement deserialization logic if needed
            throw new NotImplementedException();
        }
    }

    public class AnnotatedTestObject
    {
        [JsonConverter(typeof(CustomStringNullHandlingConverter))]
        public string Foo { get; set; }

        public string Bar { get; set; }

        public int FooBar { get; set; }
    }
}