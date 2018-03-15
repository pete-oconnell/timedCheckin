using ArchestrA.GRAccess;
using System;
using System.Diagnostics;

namespace timedCheckin
{
    class Program
    {
        static void Main(string[] args)
        {
            GRAccessApp grAccess = new GRAccessAppClass();
            string machineName = Environment.MachineName;

            Console.Write("Enter Galaxy name:");
            string galaxyName = Console.ReadLine();

            IGalaxies gals = grAccess.QueryGalaxies(machineName);
            IGalaxy galaxy = gals[galaxyName];

            if (galaxy == null)
            {
                Console.WriteLine("That galaxy does not exist");
                Console.WriteLine("Press return to exit.");
                Console.ReadLine();
                return;
            }

            Console.Write("Enter username, press return for blank:");
            string galaxyUser = Console.ReadLine();
            Console.Write("Enter password, press return for blank:");
            string galaxyPass = Console.ReadLine();

            galaxy.Login(galaxyUser, galaxyPass);

            if (!galaxy.CommandResult.Successful)
            {
                Console.WriteLine("Those login credentials are invalid");
                Console.WriteLine("Press return to exit.");
                Console.ReadLine();
                return;
            }

            string[] tagnames = new string[1];
            Console.Write("Please enter the template name you wish to modify, please ensure it starts with $:");
            tagnames[0] = Console.ReadLine();

            IgObjects queryResult = galaxy.QueryObjectsByName(EgObjectIsTemplateOrInstance.gObjectIsTemplate, ref tagnames);

            ITemplate myTemplate = (ITemplate)queryResult[1];
            myTemplate.CheckOut();
            string udaName = string.Format("uda_{0:yyyyMMddhhmmss}", DateTime.Now);
            myTemplate.AddUDA(udaName, MxDataType.MxBoolean, MxAttributeCategory.MxCategoryWriteable_USC_Lockable, MxSecurityClassification.MxSecurityOperate, false, null);

            myTemplate.Save();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            try
            {
                myTemplate.CheckIn();
            }
            catch
            {
                Console.WriteLine("Caught Error");
                if (stopWatch.IsRunning)
                {
                    stopWatch.Stop();
                    TimeSpan ts = stopWatch.Elapsed;
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
                    Console.WriteLine("RunTime " + elapsedTime);
                }

                Console.ReadLine();
            }

            stopWatch.Stop();
            TimeSpan ts2 = stopWatch.Elapsed;
            string elapsedTime2 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts2.Hours, ts2.Minutes, ts2.Seconds,
            ts2.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime2);

            galaxy.Logout();
            Console.WriteLine("Process has completed");
            Console.ReadLine();
        }
    }
}
