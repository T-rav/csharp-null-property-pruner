using FluentAssertions;
using System.Text.Json;

namespace ObjectPropertyPruner.Tests
{
    // TODO : This method just removes all null properties
    public class JsonSerializerOptionsTests
    {
        [Fact]
        public void When_JsonIgnoreCondition_WhenWritingNull_RemovesNullValues()
        {
            // Arrange
            var input = new AnnotatedTestObject
            {
                Foo = "bar",
                Bar = null
            };
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
            // Act
            var actual = JsonSerializer.Serialize(input, options);
            // Assert
            var expected = "{\"Foo\":\"bar\",\"FooBar\":0}";
            actual.Should().Be(expected);
        }

        [Fact]
        public void When_JsonIgnoreCondition_WhenWritingDefault_RemovesDefaultValues()
        {
            // Arrange
            var input = new AnnotatedTestObject
            {
                Foo = "bar"
            };
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
            };
            // Act
            var actual = JsonSerializer.Serialize(input, options);
            // Assert
            var expected = "{\"Foo\":\"bar\"}";
            actual.Should().Be(expected);
        }
    }

    public class TestObject
    {
        public string Foo { get; set; }

        public string Bar { get; set; }

        public int FooBar { get; set; }
    }
}