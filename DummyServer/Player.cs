namespace DummyServer
{
    public class Player
    {
        public string username;
        public int password;
        public int id;
        public bool isLoggedIn = false;

        public Player(string _username, int _password)
        {
            username = _username;
            password = _password;
        }

        public Player(string _username, int _password, int _id)
        {
            username = _username;
            password = _password;
            id = _id;
        }

        public void RemoveId() { } // TODO
    }
}
