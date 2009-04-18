#if DEBUG
namespace ITCreatings.Ndb.Tests.Data
{
    public class Role
    {
        public ushort Id;
        public string Name;

        public Role(ushort Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }

        /*        public const ushort Admin = 1;
                public const ushort Manager = 2;
                public const ushort TeamLeader = 3;
                public const ushort Developer = 4;
                public const ushort Tester = 5;
                public const ushort TechnicalWriter = 6;


                public override string TableName
                {
                    get { return "Roles"; }
                }

                [DbField] public string Name;
        */
    }
}
#endif