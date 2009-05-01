﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Reflection;
using ITCreatings.Ndb.Exceptions;

namespace ITCreatings.Ndb.NdbConsole
{
    public class Processor
    {
        public Action action { get; private set; }
        DbProvider provider;
        DbAccessor accessor;
        DbStructureGateway gateway;

        private Type[] Process(Assembly assembly)
        {
            switch (action)
            {
                case Action.Create:
                    return gateway.CreateTables(assembly);

                case Action.Drop:
                    return gateway.DropTables(assembly);

                case Action.Alter:
                    return gateway.AlterTables(assembly);

                default:
                    throw new Exception("Unknown Action: " + action);
            }
        }

        private static string FixPath(string path)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), path);
        }

        public void Run(IEnumerable<string> assemblies)
        {
            foreach (string _assembly in assemblies)
            {
                string assembly = (Path.IsPathRooted(_assembly)) ? _assembly : FixPath(_assembly);

                Assembly file = Assembly.LoadFile(assembly);

                Console.WriteLine("Processing {0} assembly", file.FullName);

                Type[] process = Process(file);
                foreach (Type type in process)
                    Console.WriteLine("  - {0}", type);
            }
        }

        public void SetProvider(string _provider)
        {
            try
            {
                provider = (DbProvider)Enum.Parse(typeof(DbProvider), _provider, true);
            }
            catch
            {
                throw new ArgumentException(string.Format("Provider {0} doesn't supported", _provider));
            }
        }

        public void SetConnectionString(string connectionString)
        {
            accessor = DbAccessor.Create(provider, connectionString);
            gateway = new DbStructureGateway(accessor);

            if (!accessor.CanConnect)
                throw new NdbException(string.Format(
                                           "Can't connect to \"{0}\" using {1} provider", connectionString, provider));
        }

        public void SetAction(string _action)
        {
            try
            {
                action = (Action) Enum.Parse(typeof(Action), _action, true);
            }
            catch
            {
                throw new ArgumentException(string.Format("Action {0} doesn't supported", _action));
            }
        }

        public void GenerateClasses(string path, string @namespace)
        {
            DbCodeGenerator generator = new DbCodeGenerator(gateway);
            if (!string.IsNullOrEmpty(@namespace))
                generator.Namespace = @namespace;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string[] Tables = gateway.Accessor.LoadTables();
            foreach (string table in Tables)
            {
                string generatedClass = generator.GenerateClass(table);
                string filepath = Path.Combine(path, table + ".cs");

                if (File.Exists(filepath))
                {
                    Console.WriteLine("Error: File {0} already exists", filepath);
                    continue;
                }

                using (var sw = File.CreateText(filepath))
                {
                    sw.Write(generatedClass);
                    sw.Close();
                    
                }

                Console.WriteLine("Success: File {0} generated...", filepath);
            }
        }
    }
}