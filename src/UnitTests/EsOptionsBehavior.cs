using System;
using MyLab.Search.EsAdapter;
using Xunit;

namespace UnitTests
{
    public class EsOptionsBehavior
    {
        [Theory]
        [InlineData("foo")]
        [InlineData("DocType")]
        [InlineData("UnitTests.EsOptionsBehavior+DocType")]
        public void ShouldProvideIndexForDocWithBindAttr(string doc)
        {
            //Arrange
            var options = new EsOptions
            {
                IndexBindings = new []
                {
                    new IndexBinding
                    {
                        Doc = doc,
                        Index = "bar"
                    }
                }
            };

            //Act
            var index = options.GetIndexForDocType<DocType>();

            //Assert
            Assert.Equal("bar", index);
        }

        [Theory]
        [InlineData("DocType2")]
        [InlineData("UnitTests.EsOptionsBehavior+DocType2")]
        public void ShouldProvideIndexForDocWithoutBindAttr(string doc)
        {
            //Arrange
            var options = new EsOptions
            {
                IndexBindings = new[]
                {
                    new IndexBinding
                    {
                        Doc = doc,
                        Index = "bar"
                    }
                }
            };

            //Act
            var index = options.GetIndexForDocType<DocType2>();

            //Assert
            Assert.Equal("bar", index);
        }

        [Fact]
        public void ShouldFailIfNotBound()
        {
            //Arrange
            var options = new EsOptions
            {
                IndexBindings = new[]
                {
                    new IndexBinding
                    {
                        Doc = "foo",
                        Index = "bar"
                    }
                }
            };

            //Act & Assert
            Assert.Throws<InvalidOperationException>(() => options.GetIndexForDocType<DocType2>());
        }

        [EsBindingKey("foo")]
        class DocType
        {

        }

        class DocType2
        {

        }
    }
}
