using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Data.OleDb;

namespace watchWindows
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        static void Main(string[] args)
        {
            string createText = "Sto iniziando il mio lavoro" + DateTime.Now;
            inserisciDB("Sto iniziando il mio lavoro", DateTime.Now.ToString());
            File.AppendAllText("log.txt", createText);
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
            int i = 0;
            inizio:
            ServiceController sc = new ServiceController("wuauserv");
            //string createText = "inizio il mio lavoro "+ DateTime.Now;
            switch (sc.Status)
            {
                case ServiceControllerStatus.Running:
                    Console.WriteLine( "Running");
                    createText = "Running " + DateTime.Now;
                    inserisciDB("Running", DateTime.Now.ToString());
                    sc.Stop();
                    break;
                case ServiceControllerStatus.Stopped:
                    Console.WriteLine("Stopped");
                    //inserisciDB("Stopped", DateTime.Now.ToString());
                    break;
                case ServiceControllerStatus.Paused:
                    Console.WriteLine("Paused");
                    inserisciDB("Paused", DateTime.Now.ToString());
                    break;
                case ServiceControllerStatus.StopPending:
                    Console.WriteLine("Stopping");
                    inserisciDB("Stopping", DateTime.Now.ToString());
                    break;
                case ServiceControllerStatus.StartPending:
                    Console.WriteLine("Starting");
                    inserisciDB("Starting", DateTime.Now.ToString());
                    break;
                default:
                    Console.WriteLine("Status Changing");
                    inserisciDB("Status Changing", DateTime.Now.ToString());
                    break;
            }
            Thread.Sleep(1000);
            i++;
            if(i==10)
            {
                i = 0;
                Console.Clear();
            }
            goto inizio;
        }

        public static void inserisciDB(string data,string azione)
        {
            string currentPathLog = @"D:/WATch10/log.mdb";
            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OleDb.4.0;" + "Data Source="+ currentPathLog))
            {
                conn.Open();

                // DbCommand also implements IDisposable
                using (OleDbCommand cmd = conn.CreateCommand())
                {
                    // create command with placeholders
                    cmd.CommandText =
                       "INSERT INTO log " +
                       "([Data], [Azione]) " +
                       "VALUES(@data, @azione)";

                    // add named parameters
                    cmd.Parameters.AddRange(new OleDbParameter[]
                    {
               new OleDbParameter("@data", data),
               new OleDbParameter("@azione", azione),
           });

                    // execute
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
