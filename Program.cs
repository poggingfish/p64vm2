namespace p64vm2
{
    class PvmErr
    {
#pragma warning disable CA1822 // Mark members as static
        public void NullPtr()
#pragma warning restore CA1822 // Mark members as static
        {
            Console.WriteLine("Pointer is null.");
            Environment.Exit(1);
        }
    }
    class AllocationObject
    {
        private readonly int allocated_pointer;

        public AllocationObject(int pointer) => allocated_pointer = pointer;
        public int Get()
        {
            return allocated_pointer;
        }
    }
    class RAM
    {
        public static int ramsize = 0xFFFFF;
        private readonly int[] _bus = new int[ramsize];
        private readonly bool[] _allocated = new bool[ramsize];
        public AllocationObject? Allocate(int ptr, int value)
        {
            _bus[ptr] = value;
            _allocated[ptr] = true;
            return new AllocationObject(ptr);
        }
        public void DeAllocate(ref AllocationObject? allocationObject)
        {
            if (allocationObject == null)
            {
                new PvmErr().NullPtr();
            }
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            _allocated[allocationObject.Get()] = false;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            allocationObject = null;
            GC.Collect();
        }
        public int Get(AllocationObject? allocationObject)
        {
            if (allocationObject == null)
            {
                new PvmErr().NullPtr();
            }
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            return _bus[allocationObject.Get()];
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
    }
    class CPU
    {
        private readonly RAM ram = new();
        private readonly Dictionary<string, AllocationObject?> vars = new();
        private readonly Stack<int> pointers = new();
        public CPU(){
            int i = 0;
            for (; i < RAM.ramsize; i++)
            {
                pointers.Push(i);
            }
            Console.WriteLine("Allocated: 33,554,400 Bits");
        }
        public void NewVar(string name, int value)
        {
            AllocationObject? allocationObject = ram.Allocate(pointers.Pop(), value);
            vars.Add(name, allocationObject);
        }
        public void DeleteVar(string name)
        {
            vars.TryGetValue(name, out var allocationObject);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            pointers.Push(allocationObject.Get());
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            ram.DeAllocate(ref allocationObject);
            vars.Remove(name);
        }
        public int GetVar(string name)
        {
            vars.TryGetValue(name, out var allocationObject);
            return ram.Get(allocationObject);
        }
    }
    class VM
    {
        static void Main()
        {
            Console.WriteLine(String.Concat(Enumerable.Repeat(" ", Console.WindowWidth/2))+"pogvm");
            CPU cpu = new();
            cpu.NewVar("test", 69);
            Console.WriteLine(cpu.GetVar("test"));
            cpu.DeleteVar("test");
        }
    }
}