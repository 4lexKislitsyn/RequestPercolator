using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;
using RequestPercolator.Logic.Contracts;
using RequestPercolator.Logic.Services;
using Shouldly;

namespace RequestPercolator.Logic.Tests.Services
{
    [TestFixture]
    internal class JsonPercolationHandlerTests : ServiceTests<IRequestPercolationHandler>
    {
        private IConfiguration configuration;

        [SetUp]
        public void SetUp()
        {
            configuration = Substitute.For<IConfiguration>();
        }

        [TestCase("")]
        [TestCase("$.[?(@.price >= 0)]")]
        [TestCase("[?(@.price >= 10)]")]
        public void Should_be_valid_format(string filter)
            => CreateInstance().CanHandleFilter(filter).ShouldBeTrue();
        
        [TestCase("$.")]
        [TestCase("$.[?(@.reason == 'brackets')")]
        [TestCase("$.[?(@.reason == 'brackets-2'")]
        [TestCase("$.[?(@.reason = 'invalid-sign'")]
        [TestCase("$.[?(@.reason == unknown-format")]
        public void Should_be_invalid_format(string filter)
            => CreateInstance().CanHandleFilter(filter).ShouldBeFalse();

        [TestCase("application/json")]
        [TestCase("application/problem+json")]
        public void Should_handle_request_with_mime_type(string contentMimeType)
        {
            // Given
            var request = Substitute.For<HttpRequest>();
            request.ContentType.Returns(contentMimeType);

            // When && Then
            CreateInstance().CanHandleRequest(request).ShouldBeTrue();
        }

        protected override IRequestPercolationHandler CreateInstance()
        {
            return new JsonRequestPercolationHandler(configuration);
        }
    }
}