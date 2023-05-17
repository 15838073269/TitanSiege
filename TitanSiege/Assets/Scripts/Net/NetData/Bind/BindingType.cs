using System;
using System.Collections.Generic;
using Net.Serialize;

namespace Binding
{
    public class BindingType : IBindingType
    {
        public int SortingOrder { get; } = 1;
        public Dictionary<Type, Type> BindTypes { get; } = new Dictionary<Type, Type>
        {
			{ typeof(Titansiege.UsersData), typeof(TitansiegeUsersDataBind) },
			{ typeof(Titansiege.UsersData[]), typeof(TitansiegeUsersDataArrayBind) },
			{ typeof(List<Titansiege.UsersData>), typeof(TitansiegeUsersDataGenericBind) },
			{ typeof(Titansiege.CharactersData), typeof(TitansiegeCharactersDataBind) },
			{ typeof(Titansiege.CharactersData[]), typeof(TitansiegeCharactersDataArrayBind) },
			{ typeof(List<Titansiege.CharactersData>), typeof(TitansiegeCharactersDataGenericBind) },
        };
    }
}
