namespace _1Laba.VM
{
    class MemoryPage
    {
        public int Number { get; set; }
        public bool[] UsingMap { get; set; }
        public int[] Values { get; set; }

        public MemoryPage(int number, bool[] usingMap, int[] values)
        {
            Number = number;
            UsingMap = usingMap.ToArray();
            Values = values.ToArray();
        }

        public MemoryPage(int number, int size)
        {
            Number = number;
            UsingMap = new bool[size];
            Values = new int[size];
        }
    }
}