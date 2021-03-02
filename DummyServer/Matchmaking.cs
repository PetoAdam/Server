using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DummyServer
{

    public enum TypeOfMatch
    {
        none = 0,
        threevthree = 1,
        oneoneonevoneoneone = 2,
        threevtwoone = 3,
        twoonevtwoone = 4,
        threevoneoneone = 5,
        twoonevoneoneone = 6
    }
    public static class Matchmaking
    {
        public static List<Lobby> lobbies;
        public static List<Match> matches;
        

        public static void Search()
        {
            List<Lobby> orderedLobbies = lobbies.OrderByDescending(o => o.GetAverageElo()).ToList();

            int typeOfMatch;
            while ((typeOfMatch = (int)MatchCanBeMade(orderedLobbies)) > 0)
            {
                switch (typeOfMatch)
                {
                    case 1:
                        CreateMatch1(orderedLobbies);
                        break;
                    case 2:
                        CreateMatch2(orderedLobbies);
                        break;
                    case 3:
                        CreateMatch3(orderedLobbies);
                        break;
                    case 4:
                        CreateMatch4(orderedLobbies);
                        break;
                    case 5:
                        CreateMatch5(orderedLobbies);
                        break;
                    case 6:
                        CreateMatch6(orderedLobbies);
                        break;
                    default:
                        break;
                }

                orderedLobbies = lobbies.OrderByDescending(o => o.GetAverageElo()).ToList();
            }
        }

        private static void CreateMatch1(List<Lobby> orderedLobbies)
        {
            Match match = new Match();
            int numOfThreePlayerLobbies = 0;
            List<Lobby> indexes = new List<Lobby>();
            int i = 0;
            while(numOfThreePlayerLobbies < 2)
            {
                if(orderedLobbies[i].GetPlayers().Count == 3)
                {
                    if(numOfThreePlayerLobbies == 0)
                    {
                        match.team1.Add(orderedLobbies[i]);
                    }
                    else
                    {
                        match.team2.Add(orderedLobbies[i]);
                        matches.Add(match);
                    }
                    indexes.Add(orderedLobbies[i]);
                    numOfThreePlayerLobbies++;
                }
                i++;
            }

            foreach(Lobby l in indexes)
            {
                lobbies.Remove(l);
            }
        }

        private static void CreateMatch2(List<Lobby> orderedLobbies)
        {
            Match match = new Match();
            int numOfOnePlayerLobbies = 0;
            List<Lobby> indexes = new List<Lobby>();
            int i = 0;
            while (numOfOnePlayerLobbies < 6)
            {
                if (orderedLobbies[i].GetPlayers().Count == 1)
                {
                    if (numOfOnePlayerLobbies <= 2)
                    {
                        match.team1.Add(orderedLobbies[i]);
                    }
                    else
                    {
                        match.team2.Add(orderedLobbies[i]);
                    }
                    indexes.Add(orderedLobbies[i]);
                    numOfOnePlayerLobbies++;
                    if(numOfOnePlayerLobbies == 6)
                    {
                        matches.Add(match);
                    }
                }
                i++;
            }
            foreach (Lobby l in indexes)
            {
                lobbies.Remove(l);
            }
        }

        private static void CreateMatch3(List<Lobby> orderedLobbies)
        {
            Match match = new Match();
            int numOfThreePlayerLobbies = 0;
            int numOfTwoPlayerLobbies = 0;
            int numOfOnePlayerLobbies = 0;
            List<Lobby> indexes = new List<Lobby>();
            int i = 0;
            while (numOfThreePlayerLobbies < 1  || numOfTwoPlayerLobbies < 1 ||numOfOnePlayerLobbies < 1)
            {
                if (orderedLobbies[i].GetPlayers().Count == 3)
                {
                    match.team1.Add(orderedLobbies[i]);
                    indexes.Add(orderedLobbies[i]);
                    numOfThreePlayerLobbies++;
                }
                else
                {
                    if(orderedLobbies[i].GetPlayers().Count == 2)
                    {
                        match.team2.Add(orderedLobbies[i]);
                        indexes.Add(orderedLobbies[i]);
                        numOfTwoPlayerLobbies++;
                    }
                    else
                    {
                        match.team2.Add(orderedLobbies[i]);
                        indexes.Add(orderedLobbies[i]);
                        numOfOnePlayerLobbies++;
                    }
                }
                i++;
            }
            matches.Add(match);
            foreach (Lobby l in indexes)
            {
                lobbies.Remove(l);
            }
        }

        private static void CreateMatch4(List<Lobby> orderedLobbies)
        {
            Match match = new Match();
            int numOfTwoPlayerLobbies = 0;
            int numOfOnePlayerLobbies = 0;
            List<Lobby> indexes = new List<Lobby>();
            int i = 0;
            while (numOfOnePlayerLobbies < 2 || numOfTwoPlayerLobbies < 2)
            {
                if (orderedLobbies[i].GetPlayers().Count == 2)
                {
                    if(numOfTwoPlayerLobbies == 0)
                    {
                        match.team1.Add(orderedLobbies[i]);
                    }
                    else
                    {
                        match.team2.Add(orderedLobbies[i]);
                    }
                    indexes.Add(orderedLobbies[i]);
                    numOfTwoPlayerLobbies++; 
                }
                else if (orderedLobbies[i].GetPlayers().Count == 1)
                {
                    if (numOfOnePlayerLobbies == 0)
                    {
                        match.team1.Add(orderedLobbies[i]);
                    }
                    else
                    {
                        match.team2.Add(orderedLobbies[i]);
                    }
                    indexes.Add(orderedLobbies[i]);
                    numOfOnePlayerLobbies++;
                }
                i++;
            }
            matches.Add(match);
            foreach (Lobby l in indexes)
            {
                lobbies.Remove(l);
            }
        }

        private static void CreateMatch5(List<Lobby> orderedLobbies)
        {
            Match match = new Match();
            int numOfThreePlayerLobbies = 0;
            int numOfOnePlayerLobbies = 0;
            List<Lobby> indexes = new List<Lobby>();
            int i = 0;
            while (numOfThreePlayerLobbies < 1 || numOfOnePlayerLobbies < 3)
            {
                if (orderedLobbies[i].GetPlayers().Count == 3)
                {
                    match.team1.Add(orderedLobbies[i]);
                    indexes.Add(orderedLobbies[i]);
                    numOfThreePlayerLobbies++;
                }
                else if(orderedLobbies[i].GetPlayers().Count == 1)
                {
                    match.team2.Add(orderedLobbies[i]);
                    indexes.Add(orderedLobbies[i]);
                    numOfOnePlayerLobbies++;
                }
                i++;
            }
            matches.Add(match);
            foreach (Lobby l in indexes)
            {
                lobbies.Remove(l);
            }
        }

        private static void CreateMatch6(List<Lobby> orderedLobbies)
        {
            Match match = new Match();
            int numOfTwoPlayerLobbies = 0;
            int numOfOnePlayerLobbies = 0;
            int i = 0;
            List<Lobby> indexes = new List<Lobby>();
            while (numOfTwoPlayerLobbies < 1 || numOfOnePlayerLobbies < 4)
            {
                if (orderedLobbies[i].GetPlayers().Count == 2)
                {
                    match.team1.Add(orderedLobbies[i]);
                    indexes.Add(orderedLobbies[i]);
                    numOfTwoPlayerLobbies++;
                }
                else if (orderedLobbies[i].GetPlayers().Count == 1)
                {
                    if(numOfOnePlayerLobbies == 0)
                    {
                        match.team1.Add(orderedLobbies[i]);
                    }
                    else
                    {
                        match.team2.Add(orderedLobbies[i]);
                    }
                    indexes.Add(orderedLobbies[i]);
                    numOfOnePlayerLobbies++;
                }
                i++;
            }
            matches.Add(match);
            foreach (Lobby l in indexes)
            {
                lobbies.Remove(l);
            }
        }

        /*public static bool MatchCanBeMade(List<Lobby> orderedLobbies)
        {
            int onePersonLobbyCount = 0;
            int twoPersonLobbyCount = 0;
            int threePersonLobbyCount = 0;
            foreach(Lobby l in orderedLobbies)
            {
                switch (l.GetPlayers().Count)
                {
                    case 1:
                        onePersonLobbyCount++;
                        break;
                    case 2:
                        twoPersonLobbyCount++;
                        break;
                    case 3:
                        threePersonLobbyCount++;
                        break;
                    default:
                        break;
                }
            }

            if(threePersonLobbyCount >= 2)
                return true;
            if (onePersonLobbyCount >= 6)
                return true;
            if (threePersonLobbyCount >= 1 && twoPersonLobbyCount >= 1 && onePersonLobbyCount >= 1)
                return true;
            if (twoPersonLobbyCount >= 2 && onePersonLobbyCount >= 2)
                return true;
            if (threePersonLobbyCount >= 1 && onePersonLobbyCount >= 3)
                return true;
            if (twoPersonLobbyCount >= 1 && onePersonLobbyCount >= 4)
                return true;
            return false;
        }*/

        public static TypeOfMatch MatchCanBeMade(List<Lobby> orderedLobbies)
        {
            int onePersonLobbyCount = 0;
            int twoPersonLobbyCount = 0;
            int threePersonLobbyCount = 0;
            foreach (Lobby l in orderedLobbies)
            {
                switch (l.GetPlayers().Count)
                {
                    case 1:
                        onePersonLobbyCount++;
                        break;
                    case 2:
                        twoPersonLobbyCount++;
                        break;
                    case 3:
                        threePersonLobbyCount++;
                        break;
                    default:
                        break;
                }

                if (threePersonLobbyCount >= 2)
                    return TypeOfMatch.threevthree;
                if (onePersonLobbyCount >= 6)
                    return TypeOfMatch.oneoneonevoneoneone;
                if (threePersonLobbyCount >= 1 && twoPersonLobbyCount >= 1 && onePersonLobbyCount >= 1)
                    return TypeOfMatch.threevtwoone;
                if (twoPersonLobbyCount >= 2 && onePersonLobbyCount >= 2)
                    return TypeOfMatch.twoonevtwoone;
                if (threePersonLobbyCount >= 1 && onePersonLobbyCount >= 3)
                    return TypeOfMatch.threevoneoneone;
                if (twoPersonLobbyCount >= 1 && onePersonLobbyCount >= 4)
                    return TypeOfMatch.twoonevoneoneone;
            }

            return TypeOfMatch.none;
        }
    }
}
