using PC7866;

namespace PC7866
{
    internal static class Program
    {
        //namespace PC7866;

        internal static class Programm
        {
            [STAThread]
            static void Main()
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                ApplicationConfiguration.Initialize();
                Application.Run(new Form1());
            }
        }
    }
}
