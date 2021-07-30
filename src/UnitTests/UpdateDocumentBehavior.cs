using System;
using System.Collections.Generic;
using MyLab.Search.EsAdapter;
using Nest;
using Xunit;

namespace UnitTests
{
    public class UpdateDocumentBehavior
    {
        [Fact]
        public void ShouldCreateObjectWithSpecifiedParameters()
        {
            //Arrange
            var updateDoc = new UpdateDocument<TestModel>(() => new TestModel
            {
                Value = "foo"
            });

            //Act
            var updateModel = updateDoc.ToUpdateModel();

            //Assert
            Assert.NotNull(updateModel);
            Assert.Equal("foo", updateModel.val);
        }

        [Fact]
        public void ShouldSupportOnlyMemberInitExpression()
        {
            //Arrange
            var updateDoc = new UpdateDocument<TestModel>(() => new TestModel());

            //Act & Assert
            Assert.Throws<NotSupportedException>(() => updateDoc.ToUpdateModel());
        }

        [Fact]
        public void ShouldNotPassModelWithoutChangedProperties()
        {
            //Arrange
            var updateDoc = new UpdateDocument<TestModel>(() => new TestModel
            {

            });

            //Act & Assert
            Assert.Throws<InvalidOperationException>(() => updateDoc.ToUpdateModel());
        }

        [Fact]
        public void ShouldSupportOnlyPropertyAssignment()
        {
            //Arrange
            var updateDoc = new UpdateDocument<TestModel>(() => new TestModel
            {
                List = { "foo" }
            });

            //Act & Assert
            Assert.Throws<NotSupportedException>(() => updateDoc.ToUpdateModel());
        }

        class TestModel
        {
            public string Id { get; set; }

            [Text(Name = "val")]
            public string Value { get; set; }

            public List<string> List{ get; set; }
        }
    }
}
