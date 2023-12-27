using System.Collections.Generic;

namespace ObjectPrintingTests.TestHelpers
{
    public class CollectionsKeeper
    {
        public List<string> Strings { get; set; }

        public int[] Ints { get; set; }

        public Dictionary<int, string> Dictionary { get; set; }
    }
}