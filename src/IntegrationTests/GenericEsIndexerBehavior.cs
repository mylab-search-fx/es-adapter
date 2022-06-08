using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyLab.Search.EsAdapter;
using MyLab.Search.EsAdapter.Inter;
using Nest;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public partial class GenericEsIndexerBehavior : IClassFixture<TestClientFixture>, IAsyncLifetime
    {
        [Fact]
        public async Task ShouldCreate()
        {
            //Arrange
            var doc = TestDoc.Generate();

            //Act
            await _indexer.CreateAsync(doc);
            await Task.Delay(1000);

            var found = await SearchAsync(doc.Id);

            //Assert
            Assert.NotNull(found);
            Assert.Equal(doc.Id, found.Id);
            Assert.Equal(doc.Content, found.Content);
            Assert.Equal(doc.Content2, found.Content2);
        }

        [Fact]
        public async Task ShouldNotCreateIfAlreadyExists()
        {
            //Arrange
            var doc = TestDoc.Generate();

            await _indexer.CreateAsync(doc);
            await Task.Delay(1000);

            //Act 
            var ex = await Assert.ThrowsAsync<EsException>(() => _indexer.CreateAsync(doc));

            //Assert
            Assert.Equal(409, ex.Response.ServerError.Status);
        }

        [Fact]
        public async Task ShouldIndex()
        {
            //Arrange
            var doc = TestDoc.Generate();

            //Act
            await _indexer.IndexAsync(doc);
            await Task.Delay(1000);

            var found = await SearchAsync(doc.Id);

            //Assert
            Assert.NotNull(found);
            Assert.Equal(doc.Id, found.Id);
            Assert.Equal(doc.Content, found.Content);
            Assert.Equal(doc.Content2, found.Content2);
        }

        [Fact]
        public async Task ShouldIndexWithReplaceIfAlreadyExists()
        {
            //Arrange
            var docId = Guid.NewGuid().ToString("N");
            var doc1 = TestDoc.Generate(docId);
            var doc2 = TestDoc.Generate(docId);

            await _indexer.CreateAsync(doc1);
            await Task.Delay(1000);

            //Act
            await _indexer.IndexAsync(doc2);
            await Task.Delay(1000);

            var found = await SearchAsync(docId);

            //Assert
            Assert.NotNull(found);
            Assert.Equal(docId, found.Id);
            Assert.Equal(doc2.Content, found.Content);
            Assert.Equal(doc2.Content2, found.Content2);
        }

        [Fact]
        public async Task ShouldUpdateWithLambda()
        {
            //Arrange
            var docId = Guid.NewGuid().ToString("N");
            var doc = TestDoc.Generate(docId);

            await _indexer.CreateAsync(doc);
            await Task.Delay(1000);

            //Act
            await _indexer.UpdateAsync(docId, 
                () => new TestDoc
                {
                    Content = "foo"
                });
            await Task.Delay(1000);

            var found = await SearchAsync(docId);

            //Assert
            Assert.NotNull(found);
            Assert.Equal(docId, found.Id);
            Assert.Equal("foo", found.Content);
            Assert.Equal(doc.Content2, found.Content2);
        }

        [Fact]
        public async Task ShouldUpdateWithPartial()
        {
            //Arrange
            var docId = Guid.NewGuid().ToString("N");
            var doc1 = TestDoc.Generate(docId);
            var doc2 = new TestDoc
            {
                Id = docId,
                Content = "foo"
            };

            await _indexer.CreateAsync(doc1);
            await Task.Delay(1000);

            //Act
            await _indexer.UpdateAsync(doc2);
            await Task.Delay(1000);

            var found = await SearchAsync(docId);

            //Assert
            Assert.NotNull(found);
            Assert.Equal(docId, found.Id);
            Assert.Equal("foo", found.Content);
            Assert.Equal(doc1.Content2, found.Content2);
        }

        [Fact]
        public async Task ShouldDelete()
        {
            //Arrange
            var doc = TestDoc.Generate();

            await _indexer.CreateAsync(doc);
            await Task.Delay(1000);

            //Act
            await _indexer.DeleteAsync(doc.Id);
            await Task.Delay(1000);

            var found = await SearchAsync(doc.Id);

            //Assert
            Assert.Null(found);
        }
    }
}
