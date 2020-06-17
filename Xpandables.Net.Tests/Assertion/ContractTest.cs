using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xpandables.Net.Tests.Assertion
{
    [TestClass]
    public class ContractTest
    {
        [DataTestMethod]
        [DataRow(null)]
        [ExpectedException(typeof(ArgumentNullException), AllowDerivedTypes = false)]
        public void ContractThrowsArgumentNullException(string name)
        {
            name.WhenNull(nameof(name)).ThrowArgumentNullException();
        }

        [DataTestMethod]
        [DataRow("value")]
        public void ContractNoException(string name)
        {
            name.WhenNull(nameof(name)).ThrowArgumentNullException();
        }
    }
}
