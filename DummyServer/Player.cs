namespace DummyServer
{
    class Player
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

        public void AssignId(int _id) { } // TODO

        public void RemoveId() { } // TODO
    }
}
