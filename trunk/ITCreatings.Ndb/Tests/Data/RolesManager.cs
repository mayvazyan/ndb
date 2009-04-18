#if DEBUG
using System.Collections.Generic;

namespace ITCreatings.Ndb.Tests.Data
{
    public class RolesManager
    {
        public enum Ids : ushort
        {
            Admin = 1,

            Manager = 2,
            TeamLeader = 3,
            Developer = 4,
            Designer = 5,
            Tester = 6,
            TechnicalWriter = 7,

            Customer = 100,
            Investor = 101
        }

        public static Role Admin { get { return Roles[(ushort)Ids.Admin]; } }
        public static Role Manager { get { return Roles[(ushort)Ids.Manager]; } }
        public static Role TeamLeader { get { return Roles[(ushort)Ids.TeamLeader]; } }
        public static Role Developer { get { return Roles[(ushort)Ids.Developer]; } }
        public static Role Designer { get { return Roles[(ushort)Ids.Designer]; } }
        public static Role Tester { get { return Roles[(ushort)Ids.Tester]; } }
        public static Role TechnicalWriter { get { return Roles[(ushort)Ids.TechnicalWriter]; } }

        public static Role Customer { get { return Roles[(ushort)Ids.Customer]; } }
        public static Role Investor { get { return Roles[(ushort)Ids.Investor]; } }


        public static IDictionary<ushort, Role> Roles { get; private set; }

        static RolesManager()
        {
            Roles = new Dictionary<ushort, Role>();

            InitRole(new Role((ushort)Ids.Admin, "Admin"));
            InitRole(new Role((ushort)Ids.Manager, "Manager"));
            InitRole(new Role((ushort)Ids.TeamLeader, "Team Leader"));
            InitRole(new Role((ushort)Ids.Developer, "Developer"));
            InitRole(new Role((ushort)Ids.Designer, "Designer"));
            InitRole(new Role((ushort)Ids.Tester, "Tester"));
            InitRole(new Role((ushort)Ids.TechnicalWriter, "Technical Writer"));

            InitRole(Ids.Customer);
            InitRole(Ids.Investor);
        }

        private static void InitRole(Ids id)
        {
            InitRole(new Role((ushort)id, id.ToString()));
        }

        private static void InitRole(Role role)
        {
            Roles.Add(role.Id, role);
        }

        public static Role Get(ushort Id)
        {
            return Roles[Id];
        }

        public static string GetName(ushort Id)
        {
            return Roles[Id].Name;
        }
    }
}
#endif