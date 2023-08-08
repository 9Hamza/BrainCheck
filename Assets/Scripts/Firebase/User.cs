using System;


/*
 * Make sure that the variables of this class are exactly identical to the ones in firebase (sadly it is case-sensitive)
 *
 * I believe the order of how the variables are declared does not matter
 */
[Serializable]
public class User
    {
        public string Email;
        public string Name;
        public string Username;

        public User()
        {
        
        }

        public User(string username, string name, string email)
        {
            this.Email = email;
            this.Name = name;
            this.Username = username;
        }
    }
