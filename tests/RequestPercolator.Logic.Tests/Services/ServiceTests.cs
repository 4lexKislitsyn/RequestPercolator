using NUnit.Framework;
using Shouldly;

namespace RequestPercolator.Logic.Tests.Services
{
    internal abstract class ServiceTests<T>
    {
        [Test]
        public void Should_create_service()
        {
            Should.NotThrow(CreateInstance);
        }
        
        protected abstract T CreateInstance();
    }
}