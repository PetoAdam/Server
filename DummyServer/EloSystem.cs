using System;
using System.Collections.Generic;
using System.Text;

namespace DummyServer
{
    public class EloSystem
    {

        // d = Ra wins
        // Function to calculate the Probability 
        static float Probability(float rating1,
                                     float rating2)
        {
            return 1.0f * 1.0f / (1 + 1.0f *
                   (float)(Math.Pow(10, 1.0f *
                     (rating1 - rating2) / 400)));
        }

        // Function to calculate Elo rating 
        // K is a constant. 
        // d determines whether Player A wins or 
        // Player B.  
        static void CalculateEloRating(float Ra, float Rb,
                                    int K, bool d)
        {

            // To calculate the Winning 
            // Probability of Player B 
            float Pb = Probability(Ra, Rb);

            // To calculate the Winning 
            // Probability of Player A 
            float Pa = Probability(Rb, Ra);

            // Case -1 When Player A wins 
            // Updating the Elo Ratings 
            if (d == true)
            {
                Ra = Ra + K * (1 - Pa);
                Rb = Rb + K * (0 - Pb);
            }

            // Case -2 When Player B wins 
            // Updating the Elo Ratings 
            else
            {
                Ra = Ra + K * (0 - Pa);
                Rb = Rb + K * (1 - Pb);
            }

            Console.Write("Updated Ratings:-\n");

            Console.Write("Ra = " + (Math.Round(Ra
                         * 1000000.0) / 1000000.0)
                        + " Rb = " + Math.Round(Rb
                         * 1000000.0) / 1000000.0);
        }
    }
}