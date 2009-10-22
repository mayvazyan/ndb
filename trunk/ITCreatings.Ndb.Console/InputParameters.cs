using System;

namespace ITCreatings.Ndb.NdbConsole
{
    class InputParameters
    {
        private string[] args;
        public string Action { get; private set; }
        public string Provider { get; private set; }
        public string ConnectionString { get; private set; }

        public string[] Assemblies
        {
            get
            {
                var assemblies = new string[args.Length - 3];
                for (int i = 3; i < args.Length; i++)
                {
                    string path = args[i];
                    assemblies[i - 3] = path;
                }
                return assemblies;
            }
        }

        public bool Init(string[] Args)
        {
            args = Args;

            if (args.Length < 4)
            {
                return false;
            }

            Action = args[0];
            Provider = args[1];
            ConnectionString = args[2];

            return true;
        }

        public void Apply(Processor processor)
        {
            if (processor == null)
                throw new NullReferenceException("Can't apply parameters since Processor is null.");

            processor.SetProvider(Provider);
            processor.SetConnectionString(ConnectionString);
            processor.SetAction(Action);
        }
    }
}
