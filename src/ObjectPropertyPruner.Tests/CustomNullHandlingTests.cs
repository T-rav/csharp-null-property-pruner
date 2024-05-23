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
    public class CustomNullHandlingTests
    {
        [Fact]
        public void When_JsonIgnoreCondition_WhenWritingNull_RemovesNullValues()
        {
            // Arrange
            var input = new AnnotatedTestObject
            {
                Bar = "Foo"
            };
            // Act
            var actual = JsonSerializer.Serialize(input);
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
            JsonSerializer.Serialize(writer, value.GetType(), options);
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