namespace p64vm2
{
    internal class Runner
    {
        static void Main() {
            p64vm2.Firmware.Init();
            Console.WriteLine(p64vm2.Firmware.cpu.psm.humankey);
        }
    }
}
