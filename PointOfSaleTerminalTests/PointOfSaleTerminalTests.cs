using FluentAssertions;
using Moq;
using NUnit.Framework;
using PointOfSale;
using System.Collections.Generic;

namespace PointOfSaleTerminalTests
{
    public class Tests
    {
        private Mock<IProductsProvider> productsProvider;

        [SetUp]
        public void Setup()
        {
            productsProvider = new Mock<IProductsProvider>();

            productsProvider.Setup(s => s.GetProducts()).Returns(
                    new Dictionary<char, Product>()
                    {
                        { 'A', new Product { Code='A', Price=1.25M, VolumeItemCount = 3, VolumePrice = 3M } },
                        { 'B', new Product { Code='B', Price=4.25M } },
                        { 'C', new Product { Code='C', Price=1M, VolumeItemCount = 6, VolumePrice = 5M } },
                        { 'D', new Product { Code='D', Price = 0.75M } },
                    }
                );
        }

        [Test]
        [TestCase("ABCDABA", 13.25)]
        [TestCase("CCCCCCC", 6)]
        [TestCase("ABCD", 7.25)]
        public void Test1(string items, decimal total)
        {
            // Arrange
            var posTerminal = GetClassUnderTest();

            // Act
            foreach(var itemCode in items)
            {
                posTerminal.ScanProduct(itemCode);
            }
            var result = posTerminal.CalculateTotal();

            // Arrange
            result.Should().Be(total);

        }

        private PointOfSaleTerminal GetClassUnderTest()
        {
            return new PointOfSaleTerminal(productsProvider.Object);
        }
    }
}