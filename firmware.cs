namespace p64vm2
{
    internal class Firmware
    {
        readonly public static p64vm2.CPU cpu = new();
        public static void Init()
        {
            cpu.NewVar("firmware", 1);
        }
    }
}
