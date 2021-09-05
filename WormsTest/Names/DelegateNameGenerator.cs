using System;
using Worms.Names;

namespace WormsTest.Names {
    internal sealed class DelegateNameGenerator : INameGenerator {
        private readonly Func<string> f;
        public DelegateNameGenerator(Func<string> f_) => f = f_;

        public string NextName => f();
    }
}