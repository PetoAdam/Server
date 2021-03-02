namespace DummyServer
{
    public class Player
    {
        public string username;
        public int password;
        public int id;
        public int elo = 1000;
        public bool isLoggedIn = false;

        public Player()
        {
        }

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

        public Player(string _username, int _password, int _id, int _elo)
        {
            username = _username;
            password = _password;
            id = _id;
            elo = _elo;
        }

        public void RemoveId() { } // TODO
    }
}
