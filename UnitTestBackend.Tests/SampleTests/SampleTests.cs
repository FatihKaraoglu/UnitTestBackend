using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestBackend.Tests.SampleTests
{
    public class SampleTests
    {
        [Fact]
        public void PassingTest()
        {
            // Arrange
            int a = 5;
            int b = 5;

            // Act
            int sum = a + b;

            // Assert
            Assert.Equal(10, sum); //Success
        }

        [Fact]
        public void FailingTest()
        {
            // Arrange
            string expected = "Hello";
            string actual = "Hello";

            // Act & Assert
            Assert.Equal(expected, actual); //Fail
        }
    }
}
