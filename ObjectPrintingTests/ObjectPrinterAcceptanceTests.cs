﻿using System;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using ObjectPrinting;
using ObjectPrinting.Extensions;
using ObjectPrintingTests.TestHelpers;

namespace ObjectPrintingTests
{
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {
        [Test]
        public void Demo()
        {
            var person = new Person { Name = "Alex", Age = 19, Height = 180.5 };

            var printer = ObjectPrinter.For<Person>();

            var actualString = printer.Excluding<SubPerson>()
                .Printing(p => p.Age)
                .Using(age => (age + 1000).ToString())
                .And().Printing<double>()
                .Using(CultureInfo.InvariantCulture)
                .And().Printing(p => p.Name)
                .Trim(1)
                .And().Excluding(p => p.PublicField)
                .WithRecursionHandler((_) => throw new ArgumentException())
                .PrintToString(person);

            actualString.Should().Be($"Person{Environment.NewLine}\tId = 00000000-0000-0000-0000-000000000000{Environment.NewLine}\tName = A{Environment.NewLine}\tHeight = 180.5{Environment.NewLine}\tAge = 1019{Environment.NewLine}");
        }
    }
}