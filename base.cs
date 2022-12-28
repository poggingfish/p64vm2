using System.Collections;

namespace p64vm2
{
    class PvmErr
    {
#pragma warning disable CA1822 // Mark members as static
        public void NullPtr()
        {
            Console.WriteLine("Pointer is null.");
            Environment.Exit(1);
        }
        public void EmptyStack()
        {
            Console.WriteLine("Pointer stack is empty.");
            Environment.Exit(1);
        }
#pragma warning restore CA1822 // Mark members as static
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
        public static int ramsize = 0xFFFF;
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
    class PSM
    {
        public static int bits = 1024;
        public string humankey = "";
        private readonly bool[] x = new bool[bits];
        public PSM()
        {
            for (int i = 0; i < bits; i++)
            {
                var t = new Random().Next(2) == 1;
                if (t == false) { humankey += "0"; }
                else if (t == true) { humankey += "1"; }
                x[i] = t;
            }
        }
    }
    class CPU
    {
        private readonly RAM ram = new();
        public readonly PSM psm = new();
        private readonly Dictionary<string, AllocationObject?> vars = new();
        private readonly Stack<int> pointers = new();
        public CPU(){
            for (int i = 0; i < RAM.ramsize; i++)
            {
                pointers.Push(i);
            }
            Console.WriteLine("Allocated: " + RAM.ramsize*32 + " bits.");
        }
        public void NewVar(string name, int value)
        {
            if (vars.ContainsKey(name))
            {
                DeleteVar(name);
            }
            if (pointers.Count == 0)
            {
                new PvmErr().EmptyStack();
            }
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
}