using System;
using System.Linq;
using System.Threading.Tasks;
using MyLab.Search.EsAdapter;
using MyLab.Search.EsAdapter.Indexing;
using MyLab.Search.EsAdapter.Inter;
using Nest;
using Xunit;

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
            Assert.Equal(409, ex.Response.ApiCall.HttpStatusCode);
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

        [Fact]
        public async Task ShouldPerformBulk()
        {
            //Arrange
            var docForReplace = TestDoc.Generate();
            var docForUpdate = TestDoc.Generate();
            var docForDelete = TestDoc.Generate();
            var docForCreate = TestDoc.Generate();
            var docReplacer = TestDoc.Generate(docForReplace.Id);
            var docUpdater = new TestDoc
            {
                Id = docForUpdate.Id,
                Content = Guid.NewGuid().ToString("N"),
                Content2 = null
            };

            var initialBulkReq = new EsBulkIndexingRequest<TestDoc>
            {
                CreateList = new[] { docForDelete, docForReplace, docForUpdate }
            };

            var controlBulkReq = new EsBulkIndexingRequest<TestDoc>
            {
                CreateList = new[] { docForCreate },
                IndexList = new[] { docReplacer },
                UpdateList = new[] { docUpdater },
                DeleteList = new[] { new Id(docForDelete.Id) }
            };

            ISearchRequest searchReq = new SearchRequest(_indexName)
            {
                Query = new QueryContainer(new IdsQuery
                {
                    Values = new Id[]
                    {
                        docForCreate.Id,
                        docForUpdate.Id,
                        docForDelete.Id,
                        docForReplace.Id
                    }
                })
            };

            //Act
            await _indexer.BulkAsync(initialBulkReq);
            await Task.Delay(1000);
            await _indexer.BulkAsync(controlBulkReq);
            await Task.Delay(1000);

            var searchResp = await _client.SearchAsync<TestDoc>(searchReq);

            TestTools.ResponseValidator.Validate(searchResp);

            var actualCreatedDoc = searchResp.Hits.FirstOrDefault(h => h.Id == docForCreate.Id)?.Source;
            var actualReplacedDoc = searchResp.Hits.FirstOrDefault(h => h.Id == docForReplace.Id)?.Source;
            var actualUpdatedDoc = searchResp.Hits.FirstOrDefault(h => h.Id == docForUpdate.Id)?.Source;

            //Assert
            Assert.DoesNotContain(searchResp.Hits, h => h.Id == docForDelete.Id);

            Assert.NotNull(actualCreatedDoc);
            Assert.Equal(docForCreate.Content, actualCreatedDoc.Content);
            Assert.Equal(docForCreate.Content2, actualCreatedDoc.Content2);

            Assert.NotNull(actualReplacedDoc);
            Assert.Equal(docReplacer.Content, actualReplacedDoc.Content);
            Assert.Equal(docReplacer.Content2, actualReplacedDoc.Content2);

            Assert.NotNull(actualUpdatedDoc);
            Assert.Equal(docUpdater.Content, actualUpdatedDoc.Content);
            Assert.Equal(docForUpdate.Content2, actualUpdatedDoc.Content2);
        }

        [Fact]
        public async Task ShouldPerformBulkWithLambda()
        {
            //Arrange
            var docForReplace = TestDoc.Generate();
            var docForUpdate = TestDoc.Generate();
            var docForDelete = TestDoc.Generate();
            var docForCreate = TestDoc.Generate();
            var docReplacer = TestDoc.Generate(docForReplace.Id);
            var docUpdater = new TestDoc
            {
                Id = docForUpdate.Id,
                Content = Guid.NewGuid().ToString("N"),
                Content2 = null
            };

            var initialBulkReq = new Func<BulkDescriptor, IBulkRequest>(
                d => d.CreateMany(new[] { docForDelete, docForReplace, docForUpdate })
            );

            var controlBulkReq = new Func<BulkDescriptor, IBulkRequest>(d => d
                .Create<TestDoc>(cd => cd.Document(docForCreate))
                .Index<TestDoc>(id => id.Document(docReplacer))
                .Update<TestDoc>(ud => ud.Id(docUpdater.Id).Doc(docUpdater).DocAsUpsert())
                .Delete<TestDoc>(dd => dd.Id(docForDelete.Id))
            );

            ISearchRequest searchReq = new SearchRequest(_indexName)
            {
                Query = new QueryContainer(new IdsQuery
                {
                    Values = new Id[]
                    {
                        docForCreate.Id,
                        docForUpdate.Id,
                        docForDelete.Id,
                        docForReplace.Id
                    }
                })
            };

            //Act
            await _indexer.BulkAsync(initialBulkReq);
            await Task.Delay(1000);
            await _indexer.BulkAsync(controlBulkReq);
            await Task.Delay(1000);

            var searchResp = await _client.SearchAsync<TestDoc>(searchReq);

            TestTools.ResponseValidator.Validate(searchResp);

            var actualCreatedDoc = searchResp.Hits.FirstOrDefault(h => h.Id == docForCreate.Id)?.Source;
            var actualReplacedDoc = searchResp.Hits.FirstOrDefault(h => h.Id == docForReplace.Id)?.Source;
            var actualUpdatedDoc = searchResp.Hits.FirstOrDefault(h => h.Id == docForUpdate.Id)?.Source;

            //Assert
            Assert.DoesNotContain(searchResp.Hits, h => h.Id == docForDelete.Id);

            Assert.NotNull(actualCreatedDoc);
            Assert.Equal(docForCreate.Content, actualCreatedDoc.Content);
            Assert.Equal(docForCreate.Content2, actualCreatedDoc.Content2);

            Assert.NotNull(actualReplacedDoc);
            Assert.Equal(docReplacer.Content, actualReplacedDoc.Content);
            Assert.Equal(docReplacer.Content2, actualReplacedDoc.Content2);

            Assert.NotNull(actualUpdatedDoc);
            Assert.Equal(docUpdater.Content, actualUpdatedDoc.Content);
            Assert.Equal(docForUpdate.Content2, actualUpdatedDoc.Content2);
        }
    }
}
