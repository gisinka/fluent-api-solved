﻿using FluentAssertions;
using NUnit.Framework;
using ObjectPrinting.Extensions;
using ObjectPrintingTests.TestHelpers;

namespace ObjectPrintingTests
{
    internal class PrinterExtensionsTests
    {
        [Test]
        public void CreatePrinterWithExtensionMethod_ShouldNotBeNull()
        {
            var person = new Person();
            var printer = person.CreatePrinter();

            printer.Should().NotBe(null);
        }
    }
}