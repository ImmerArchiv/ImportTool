using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Archiv10.Domain.Shared.Locator;

namespace Archiv10.Domain.Tests
{
    [TestClass]
    public class UnitTestFileFilter
    {
        [TestMethod]
        public void TestMatchAll()
        {
            var fileFilter = DomainLocator.GetFileFilter(new[] { "*" });
            Assert.IsTrue(fileFilter.Match(@"C:\Dokument\hello"));
        }

        [TestMethod]
        public void TestMatchImages()
        {
            var fileFilter = DomainLocator.GetFileFilter(new[] { "*.jpg" , "*.jpeg" });
            Assert.IsTrue(fileFilter.Match(@"C:\Dokument\hello.jPg"));
            Assert.IsTrue(fileFilter.Match(@"C:\Dokument\hello.JPEG"));
            Assert.IsFalse(fileFilter.Match(@"C:\Dokument\hello"));
            Assert.IsFalse(fileFilter.Match(@"C:\Dokument\hello.txt"));
        }



        [TestMethod]
        public void TestMatchIndividual()
        {
            var fileFilter = DomainLocator.GetFileFilter(new[] { "wichtig_*.txt" });
            Assert.IsTrue(fileFilter.Match(@"C:\Dokument\wichtig_Text1.txt"));
            Assert.IsFalse(fileFilter.Match(@"C:\Dokument\wichtig_Text1.jpg"));
            Assert.IsFalse(fileFilter.Match(@"C:\Dokument\hello"));
            Assert.IsFalse(fileFilter.Match(@"C:\Dokument\hello.txt"));
        }
    }
}
