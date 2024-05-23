using FluentAssertions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ObjectPropertyPruner.Tests
{
    // TODO : This is a failed attempt at using annotations to selectively prune null fields
    public class CustomNullHandlingTests
    {
        // todo : this test is expected to fail
        [Fact]
        public void When_UsingCustomNullHandler_ExpectItRemovesNullValuesWhenAnnotated()
        {
            // Arrange
            var input = new CustomAnnotatedTestObject
            {
                // todo : The attribute will only work if the property is set
                Bar = "Foo"
            };
            // Act
            var actual = JsonSerializer.Serialize(input);
            // Assert
            var expected = "{\"Foo\":\"bar\",\"FooBar\":0}";
            actual.Should().Be(expected);
        }
    }

    public class NullHandlingAttributeTests
    {
        [Fact]
        public void When_UsingCustomNullHandler_ExpectItRemovesNullValuesWhenAnnotated()
        {
            // Arrange
            var input = new AnnotatedTestObject
            {
                // todo : Figure out why the null value is still serialized. This technique works in newtonsoft?!
                Bar = "foo"
            };
            var options = new JsonSerializerOptions
            {
                Converters = { new CustomStringNullHandlingConverter() }
            };
            // Act
            var actual = JsonSerializer.Serialize(input, options);
            // Assert
            var expected = "{\"Bar\":\"foo\",\"FooBar\":0}";
            actual.Should().Be(expected);
        }
    }

    // I tried object, but got exceptions about not being compatible with System.String or System.Int
    public class CustomStringNullHandlingConverter : JsonConverter<string?>
    {
        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
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

    public class CustomAnnotatedTestObject
    {
        [JsonConverter(typeof(CustomStringNullHandlingConverter))]
        public string Foo { get; set; }

        public string Bar { get; set; }

        public int FooBar { get; set; }
    }

    public class AnnotatedTestObject
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Foo { get; set; }

        public string Bar { get; set; }

        public int FooBar { get; set; }
    }
}