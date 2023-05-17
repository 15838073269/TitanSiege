using System.Collections;
using System.Collections.Generic;
using Titansiege;
using UnityEngine;
namespace GF.Model {
    public class UserModel {
        public UserModel() {
            m_CharacterList = new List<CharactersData> ();
            //m_Users = new UsersData();
        }
        public List<CharactersData> m_CharacterList;
        public UsersData m_Users;
    }
}

