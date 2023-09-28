using System;
using System.Collections.Generic;
using Net.Serialize;

namespace Binding
{
    public class BindingType : IBindingType
    {
        public int SortingOrder { get; private set; }
        public Dictionary<Type, Type> BindTypes { get; private set; }
        public BindingType()
        {
            SortingOrder = 1;
            BindTypes = new Dictionary<Type, Type>
            {
				{ typeof(Titansiege.UsersData), typeof(TitansiegeUsersDataBind) },
				{ typeof(Titansiege.UsersData[]), typeof(TitansiegeUsersDataArrayBind) },
				{ typeof(List<Titansiege.UsersData>), typeof(SystemCollectionsGenericListTitansiegeUsersDataBind) },
				{ typeof(Titansiege.NpcsData), typeof(TitansiegeNpcsDataBind) },
				{ typeof(Titansiege.NpcsData[]), typeof(TitansiegeNpcsDataArrayBind) },
				{ typeof(List<Titansiege.NpcsData>), typeof(SystemCollectionsGenericListTitansiegeNpcsDataBind) },
				{ typeof(FightProp), typeof(FightPropBind) },
				{ typeof(FightProp[]), typeof(FightPropArrayBind) },
				{ typeof(List<FightProp>), typeof(SystemCollectionsGenericListFightPropBind) },
				{ typeof(Titansiege.CharactersData), typeof(TitansiegeCharactersDataBind) },
				{ typeof(Titansiege.CharactersData[]), typeof(TitansiegeCharactersDataArrayBind) },
				{ typeof(List<Titansiege.CharactersData>), typeof(SystemCollectionsGenericListTitansiegeCharactersDataBind) },
            };
        }
    }
}
