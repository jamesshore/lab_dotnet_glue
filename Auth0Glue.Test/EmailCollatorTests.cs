using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections;

namespace Auth0Glue.Test
{
    [TestClass]
    public class EmailCollatorTests
    {
        [TestMethod]
        public void CombinesTwoEmailListsIntoOne()
        {
            var list1 = new List<string>()
            {
                "email1",
                "email2"
            };
            var list2 = new List<string>()
            {
                "emailA",
                "emailB"
            };
            var expected = new HashSet<string>()
            {
                "email1",
                "email2",
                "emailA",
                "emailB"
            };

            AssertSetsEqual(expected, EmailCollator.collate(list1, list2));
        }

        [TestMethod]
        public void EliminatesDuplicates()
        {
            var list1 = new List<string>()
            {
                "email1",
                "email1"
            };
            var list2 = new List<string>()
            {
                "email1",
                "email1"
            };
            var expected = new HashSet<string>()
            {
                "email1"
            };

            AssertSetsEqual(expected, EmailCollator.collate(list1, list2));
        }

        [TestMethod]
        public void RemovesNulls()
        {
            var list1 = new List<string>()
            {
                null,
                "email1"
            };
            var list2 = new List<string>()
            {
                null
            };
            var expected = new HashSet<string>()
            {
                "email1"
            };

            AssertSetsEqual(expected, EmailCollator.collate(list1, list2));
        }

        private void AssertSetsEqual(ISet<string> expected, ISet<string> actual)
        {
            if (!expected.SetEquals(actual))
            {
                Assert.Fail($"Expected: \n{StringifySet(expected)}\nActual: \n{StringifySet(actual)}\n");
            }
        }

        private static string StringifySet(ISet<string> set)
        {
            // I would rather do this using an elegant functional map/reduce, but I don't know how to do that in C#.
            var result = "";
            foreach (var s in set)
            {
                var printable = s ?? "(null)";
                result += $"{printable}\n";
            }
            return result;
        }
    }
}
