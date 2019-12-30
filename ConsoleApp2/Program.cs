using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const int LARGEURZONE = 4000;
            const int HAUTEURZONE = 1800;
            const int MAXDISTANCEDRONE = 100;

            string[] inputs;
            inputs = Console.ReadLine().Split(' ');
            int P = int.Parse(inputs[0]); // number of players in the game (2 to 4 players)
            int ID = int.Parse(inputs[1]); // ID of your player (0, 1, 2, or 3)
            Console.Error.WriteLine("ID of your player" + ID);

            int D = int.Parse(inputs[2]); // number of drones in each team (3 to 11)
            Console.Error.WriteLine("Drones" + D);
            int Z = int.Parse(inputs[3]); // number of zones on the map (4 to
            List<Zone> zones = new List<Zone>();
            for (int i = 0; i < Z; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int X = int.Parse(inputs[0]); // corresponds to the position of the center of a zone. A zone is a circle with a radius of 100 units.
                int Y = int.Parse(inputs[1]);
                Console.Error.WriteLine("X" + X);
                Console.Error.WriteLine("Y" + Y);
                Zone zone = new Zone() { X = X, Y = Y, NumeroZone = i };
                zones.Add(zone);
            }

            // game loop
            while (true)
            {
                for (int i = 0; i < Z; i++)
                {
                    int TID = int.Parse(Console.ReadLine()); // ID of the team controlling the zone (0, 1, 2, or 3) or -1 if it is not controlled. The zones are given in the same order as in the initialization.
                    zones.Where(x => x.NumeroZone == i).FirstOrDefault().IdTeamControllingZone = TID;
                    Console.Error.WriteLine("TID" + TID);
                }
                List<Drone> drones = new List<Drone>();
                for (int i = 0; i < P; i++)
                {
                    for (int j = 0; j < D; j++)
                    {
                        inputs = Console.ReadLine().Split(' ');
                        int DX = int.Parse(inputs[0]); // The first D lines contain the coordinates of drones of a player with the ID 0, the following D lines those of the drones of player 1, and thus it continues until the last player.
                        int DY = int.Parse(inputs[1]);
                        Console.Error.WriteLine("DX" + DX);
                        Console.Error.WriteLine("DY" + DY);
                        Drone drone = new Drone() { DX = DX, DY = DY };
                        drones.Add(drone);
                    }
                }
                drones = drones.OrderBy(x => x.DX).ThenBy(x => x.DY).ToList();
                List<Zone> zoneTampon = new List<Zone>() { };
                zoneTampon.AddRange(zones);
                Console.Error.WriteLine(" zoneTampon.Count()" + zoneTampon.Count());

                List<Zone> zoneControllerParEnnemi = zoneTampon.Where(x => x.IdTeamControllingZone != ID && x.IdTeamControllingZone != -1).ToList();
                Console.Error.WriteLine(" zoneControllerParEnnemi.Count()" + zoneControllerParEnnemi.Count());

                List<Zone> zoneVide = zoneTampon.Where(x => x.IdTeamControllingZone == -1).ToList();
                Console.Error.WriteLine(" zoneVide.Count()" + zoneVide.Count());

                int iCourant = 0;

                int iCourantVide = 0;
                string str = "";
                List<DeplacementDrone> deplacementDrones = new List<DeplacementDrone>();
                //pour chaques drônes, on effectue une action
                //envoi sur une zone occupée par l'ennemi
                // ou envoi sur zone vide
                foreach (Drone drone in drones)
                {
                    bool alreadyPlacementDrone = false;
                    if (zoneVide.Any())
                    {
                        Console.Error.WriteLine("in1");

                        Zone uneZoneVide = zoneVide.Skip(iCourantVide).FirstOrDefault();
                        DeplacementDrone deplacementDrone = new DeplacementDrone() { X = uneZoneVide.X, Y = uneZoneVide.Y };
                        deplacementDrones.Add(deplacementDrone);
                        //zoneVide.Remove(uneZoneVide);
                        iCourantVide = iCourantVide + 1;
                        if (iCourantVide == zoneVide.Count())
                        {
                            iCourantVide = 0;
                        }
                        alreadyPlacementDrone = true;
                    }

                    if (!alreadyPlacementDrone && (zoneControllerParEnnemi.Any()))
                    {
                        Zone unezoneControllerParEnnemi = zoneControllerParEnnemi.Skip(iCourant).FirstOrDefault();
                        Console.Error.WriteLine("unezoneControllerParEnnemi : " + unezoneControllerParEnnemi.X + " " + unezoneControllerParEnnemi.Y);
                        Console.Error.WriteLine("iCourant : " + iCourant);
                        DeplacementDrone deplacementDrone = new DeplacementDrone() { X = unezoneControllerParEnnemi.X, Y = unezoneControllerParEnnemi.Y };
                        deplacementDrones.Add(deplacementDrone);
                        //zoneControllerParEnnemi.Remove(unezoneControllerParEnnemi);
                        iCourant = iCourant + 1;
                        if (iCourant == zoneControllerParEnnemi.Count())
                        {
                            iCourant = 0;
                        }
                    }

                    //if (!zoneVide.Any() && (!zoneControllerParEnnemi.Any()))
                    //{
                    //    Console.Error.WriteLine("in3");
                    //    DeplacementDrone deplacementDrone = new DeplacementDrone() { X = 0, Y = 0 };
                    //    deplacementDrones.Add(deplacementDrone);
                    //}
                }
                //
                foreach (DeplacementDrone deplacementDrone in deplacementDrones.OrderBy(x => x.X).ThenBy(x => x.Y))
                {
                    Console.Error.WriteLine("deplacementdrone");
                    Console.WriteLine(deplacementDrone.X + " " + deplacementDrone.Y);
                }

                int nbrDeplacementDrone = deplacementDrones.Count();
                while (nbrDeplacementDrone < D)
                {
                    Console.WriteLine("0 0");
                    nbrDeplacementDrone = nbrDeplacementDrone + 1;
                }
            }
        }
    }

    public class DeplacementDrone
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class Drone
    {
        public int DX { get; set; }
        public int DY { get; set; }
    }

    public class Zone
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int NumeroZone { get; set; }
        public int IdTeamControllingZone { get; set; }
    }
}