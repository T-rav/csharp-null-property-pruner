using FluentAssertions;
using System.Dynamic;
using System.Text.Json;

namespace ObjectPropertyPruner.Tests
{
    // TODO : Use ExpandoObject (dynamics) to prune selective null properties at runtime. NOT ADIVSED TO BE USED! 
    public class DynamicObjectPruning
    {
        [Fact]
        public void When_UsingCustomNullHandler_ExpectItRemovesNullValuesWhenAnnotated()
        {
            // Arrange
            var input = new TestObject
            {
                Bar = "Foo"
            };
            // Act
            var actual = CustomerJsonEmitter.PruneNullProperies(input, "Foo");
            // Assert
            var expected = "{\"Bar\":\"Foo\",\"FooBar\":0}";
            actual.Should().Be(expected);
        }
    }

    public static class CustomerJsonEmitter
    {
        public static string PruneNullProperies(object obj, params string[] propertyList)
        {
            var expando = new ExpandoObject();
            var expandoDict = (IDictionary<string, object>)expando;

            var properties = obj.GetType().GetProperties(System.Reflection.BindingFlags.Public 
                                                         | System.Reflection.BindingFlags.Instance);

            foreach(var property in properties)
            {
                var value = property.GetValue(obj);
                if(value != null)
                {
                    expandoDict.Add(property.Name, value);
                }
            }

            var json = JsonSerializer.Serialize(expando);

            return json;
        }
    }
}
