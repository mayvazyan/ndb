using System;
using System.Text;

namespace ITCreatings.Ndb.NdbConsole
{
    public class Program
    {
        //TODO: add params validation
        public static void Main(string[] args)
        {
            if (args.Length < 4)
            {
                printUsage();
                return;
            }

            string action = args[0];
            string provider = args[1];
            string connectionString = args[2];

            var processor = new Processor();

            try
            {
                processor.SetProvider(provider);
                processor.SetConnectionString(connectionString);
                processor.SetAction(action);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                printUsage();
                return;
            }

            Console.WriteLine("Action: {0}", action);

            try
            {
                switch (processor.action)
                {
                    case Action.Generate:
                        string Path = args[3];
                        
                        string Namespace = (args.Length > 4) ? args[4] : null;
                        processor.GenerateClasses(Path, Namespace);
                        break;

                    default:

                        var assemblies = new string[args.Length - 3];
                        for (int i = 3; i < args.Length; i++)
                        {
                            string path = args[i];
                            assemblies[i - 3] = path;
                        }

                        processor.Run(assemblies);
                        break;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Internal exception occured: " + ex.Message);
            }
        }

       
        private static void printUsage()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Usage: NdbConsole [action] [inputfiles]");
            sb.AppendLine();
            sb.AppendLine("Actions:");
            sb.AppendFormat("\\{0} \tCreates database structure based on specifyed  assemblies\r\n", Action.Create);
            sb.AppendFormat("\\{0} \t\tRemoves tables related to the objects in specifyed  assemblies\r\n", Action.Drop);
            sb.AppendFormat("\\{0} \t\tUpdates database structure to match specifyed assemblies\r\n", Action.Alter);

//            sb.AppendLine("Params:");
//            sb.AppendLine("- Alter");
//            sb.AppendFormat("\t\d\r\n");
            Console.WriteLine(sb.ToString());
        }
    }
}