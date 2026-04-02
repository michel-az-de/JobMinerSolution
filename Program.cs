using System.Windows.Forms;

namespace JobMiner
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.Run(new Forms.FormMain());
      }
    }
}