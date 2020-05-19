using System;
using System.Threading.Tasks;
using Nest;
using Xunit;

namespace IntegrationTests.Nest
{
    public class IndexLifetimeBehavior : IClassFixture<ClientFixture>
    {
        private readonly ClientFixture _clFx;
        
        public IndexLifetimeBehavior(ClientFixture clFx)
        {
            _clFx = clFx;
        }

        [Fact]
        public async Task ShouldCreateIndex()
        {
            //Act && Assert
            await _clFx.UseTmpIndex(indNm => Task.CompletedTask);
        }

        [Fact]
        public async Task ShouldDeleteIndex()
        {
            //Arrange
            string indexName = null;

            //Act
            await _clFx.UseTmpIndex(indNm =>
            {
                indexName = indNm;
                return Task.CompletedTask;
            });

            var exists = await _clFx.EsClient.Indices.ExistsAsync(indexName);

            //Assert
            Assert.NotNull(exists);
            Assert.False(exists.Exists);
        }

        [Fact]
        public async Task ShouldDetectExistentIndex()
        {
            //Act
            ExistsResponse exists = null;

            await _clFx.UseTmpIndex(async indNm =>
            {
                exists = await _clFx.EsClient.Indices.ExistsAsync(indNm);
            });

            //Assert
            Assert.NotNull(exists);
            Assert.True(exists.Exists);
        }

        [Fact]
        public async Task ShouldNotDetectAbsentIndex()
        {
            //Arrange
            string absentIndexName = Guid.NewGuid().ToString("N");

            //Act
            var exists = await _clFx.EsClient.Indices.ExistsAsync(absentIndexName); ;
            
            //Assert
            Assert.NotNull(exists);
            Assert.False(exists.Exists);
        }
    }
}
