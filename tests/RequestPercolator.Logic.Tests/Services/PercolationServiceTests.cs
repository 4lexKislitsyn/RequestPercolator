using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;
using RequestPercolator.Logic.Contracts;
using RequestPercolator.Logic.Services;
using RequestPercolator.Model;
using RequestPercolator.Model.Contracts;
using Shouldly;

namespace RequestPercolator.Logic.Tests.Services
{
    [TestFixture]
    internal sealed class PercolationServiceTests : ServiceTests<IPercolationService>
    {
        private List<IRequestPercolationHandler> requestHandlers;
        private IFilterRepository filterRepository;

        [SetUp]
        public void SetUp()
        {
            requestHandlers = new List<IRequestPercolationHandler>();
            filterRepository = Substitute.For<IFilterRepository>();
        }

        [TearDown]
        public void TearDown()
        {
            requestHandlers.Clear();
        }

        [Theory]
        public async Task Should_return_handler_result(bool byId, PercolationResult percolationResult)
        {
            // Given
            var id = Guid.NewGuid();
            var filter = TestContext.CurrentContext.Random.GetString(10);
            var request = Substitute.For<HttpRequest>();
            filterRepository.GetFilterAsync(default, default)
                .ReturnsForAnyArgs(filter); 

            var handler = Substitute.For<IRequestPercolationHandler>();
            handler.HandleAsync(default, default,default)
                .ReturnsForAnyArgs(percolationResult);
            handler.CanHandleRequest(default)
                .ReturnsForAnyArgs(true);
            handler.CanHandleFilter(default)
                .ReturnsForAnyArgs(true);
            requestHandlers.Add(handler);

            // When
            var result = byId
                ? await CreateInstance().PercolateRequestAsync(request, id, CancellationToken.None)
                : await CreateInstance().PercolateRequestAsync(request, filter, CancellationToken.None);

            // Then
            result.ShouldBe(percolationResult);
            if (byId)
            {
                await filterRepository.Received(1).GetFilterAsync(id, Arg.Any<CancellationToken>());
            }

            handler.Received(1).CanHandleRequest(request);
            handler.Received(1).CanHandleFilter(filter);
            await handler.Received(1).HandleAsync(request, filter, Arg.Any<CancellationToken>());
        }

        [Theory]
        public async Task Should_throw_not_found_exception(bool addHandler, bool byId)
        {
            // Given
            var id = Guid.NewGuid();
            var filter = TestContext.CurrentContext.Random.GetString(10);
            var request = Substitute.For<HttpRequest>();
            filterRepository.GetFilterAsync(default, default)
                .ReturnsForAnyArgs(filter); 

            var handler = Substitute.For<IRequestPercolationHandler>();
            if (addHandler)
            {
                requestHandlers.Add(handler);
            }

            // When
            Should.Throw<NotSupportedException>(() => ExecutePercolation(byId, request, id, filter));

            // Then
            if (byId)
            {
                await filterRepository.Received(1).GetFilterAsync(id, Arg.Any<CancellationToken>());
            }
            
            await handler.DidNotReceiveWithAnyArgs().HandleAsync(default, default, default);
            handler.DidNotReceiveWithAnyArgs().CanHandleFilter(default);
            if (addHandler)
            {
                handler.Received(1).CanHandleRequest(request);
            }
        }
        
        [Theory]
        public async Task Should_throw_format_exception(bool byId)
        {
            // Given
            var id = Guid.NewGuid();
            var filter = TestContext.CurrentContext.Random.GetString(10);
            var request = Substitute.For<HttpRequest>();
            filterRepository.GetFilterAsync(default, default)
                .ReturnsForAnyArgs(filter); 

            var handler = Substitute.For<IRequestPercolationHandler>();
            handler.CanHandleRequest(default)
                .ReturnsForAnyArgs(true);
            requestHandlers.Add(handler);

            // When
            Should.Throw<FormatException>(() => ExecutePercolation(byId, request, id, filter));

            // Then
            if (byId)
            {
                await filterRepository.Received(1).GetFilterAsync(id, Arg.Any<CancellationToken>());
            }
            await handler.DidNotReceiveWithAnyArgs().HandleAsync(default, default, default);
            handler.Received(1).CanHandleFilter(filter);
            handler.Received(1).CanHandleRequest(request);
        }

        protected override IPercolationService CreateInstance()
        {
            return new PercolationService(filterRepository, requestHandlers);
        }

        private Task<PercolationResult> ExecutePercolation(bool byId, HttpRequest request, Guid? id = null,
            string filter = null)
        {
            if (byId && !id.HasValue)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return byId
                ? CreateInstance().PercolateRequestAsync(request, id.Value, CancellationToken.None)
                : CreateInstance().PercolateRequestAsync(request, filter, CancellationToken.None);
        }
    }
}