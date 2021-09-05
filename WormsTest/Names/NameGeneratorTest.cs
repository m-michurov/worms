using System.Collections.Generic;
using NUnit.Framework;
using Worms.Names;

namespace WormsTest.Names {
    public sealed class NameGeneratorTest {
        [TestCase]
        public void TestNameUniqueness() {
            var g = new NameGenerator();
            const int aLot = 1_000_000;
            var generated = new HashSet<string>();

            for (var i = 0; i < aLot; i += 1) {
                generated.Add(g.NextName);
            }
            
            Assert.AreEqual(aLot, generated.Count);
        }
    }
}