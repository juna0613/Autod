using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Autod.Core;
namespace AutodTest
{
    [TestFixture]
    public class FadTest
    {
        [Test]
        public void TestFad1()
        {
            var x0 = new Fad(4, 1);
            var x1 = new Fad(2.0);

            var f = F(x0, x1);
            Assert.That(f.Value, Is.EqualTo(66.0));
            Assert.That(f.Derivative, Is.EqualTo(24.5));
        }

        [Test]
        public void TestExp()
        {
            var x0 = new Fad(4.0, 1.0);
            var x1 = new Fad(3.0); 
            var f = Fad.Exp(x1) * x0;

            Assert.That(f.Value, Is.EqualTo(Math.Exp(3.0) * 4), "val");
            Assert.That(f.Derivative, Is.EqualTo(Math.Exp(3.0)), "deriv");

            var f2 = Fad.Exp(Fad.Log(x0));

            Assert.That(f2.Value, Is.EqualTo(4), "val");
            Assert.That(f2.Derivative, Is.EqualTo(1), "deriv");

            var f3 = Fad.Log(Fad.Exp(x1));

            Assert.That(f3.Value, Is.EqualTo(3), "val");
            Assert.That(f3.Derivative, Is.EqualTo(0), "deriv");

            var f4 = Fad.Sin(x0 * x0);
            Console.WriteLine(f4.Value);
            Console.WriteLine(f4.Derivative);
            Console.WriteLine((Math.Sin(4.0001 * 4.0001) - Math.Sin(3.9999 * 3.9999)) / 2 / 0.0001);

        }



        private static Fad F(Fad x0, Fad x1)
        {
            return 2.0 * x0 * x0 - 3 * x0 / x1 + 5 * x0 * x1;
        }
    }
}
