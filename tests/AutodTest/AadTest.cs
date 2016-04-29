using System;
using NUnit.Framework;
using Autod.Core;
namespace AutodTest
{
    [TestFixture]
    public class AadTest
    {
        [Test]
        public void AadTest1()
        {
            var x0 = new Aad(4.0);
            var x1 = new Aad(2.0);
            var y = F(x0, x1);
            Assert.That(y.Value, Is.EqualTo(66));
            Assert.That(y.Derivative(x0), Is.EqualTo(24.5));
            Assert.That(y.Derivative(x1), Is.EqualTo(23));
        }

        [Test]
        public void AadTest2()
        {
            var x0 = new Aad(2.0);
            var x1 = new Aad(3.0);
            var x2 = new Aad(4.0);
            var y = Aad.Exp(x0) * x1;
            Assert.That(y.Value, Is.EqualTo(Math.Exp(2.0) * 3));
            Assert.That(y.Derivative(x0), Is.EqualTo(Math.Exp(2.0) * 3));
            Assert.That(y.Derivative(x1), Is.EqualTo(Math.Exp(2.0)));
            Assert.That(y.Derivative(x2), Is.EqualTo(0.0));
        }

        private static Aad F(Aad x0, Aad x1)
        {
            return 2.0 * x0 * x0 - 3 * x0 / x1 + 5 * x0 * x1;
        }
    }
}

