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
            Console.Error.WriteLine(" number of players in the game (2 to 4 players) : " + P);

            int ID = int.Parse(inputs[1]); // ID of your player (0, 1, 2, or 3)
            Console.Error.WriteLine("ID of your player : " + ID);

            int D = int.Parse(inputs[2]); // number of drones in each team (3 to 11)
            Console.Error.WriteLine("Drones : " + D);

            int Z = int.Parse(inputs[3]); // number of zones on the map (4 to
            List<Zone> zones = new List<Zone>();
            for (int i = 0; i < Z; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int X = int.Parse(inputs[0]); // corresponds to the position of the center of a zone. A zone is a circle with a radius of 100 units.
                int Y = int.Parse(inputs[1]);

                Zone zone = new Zone() { X = X, Y = Y, NumeroZone = i, AlreadyAffectedZoneVides = false };
                Console.Error.WriteLine(zone.ToString());
                zones.Add(zone);
            }

            // game loop
            while (true)
            {
                foreach (Zone item in zones)
                {
                    item.AlreadyAffectedZoneVides = false;
                    item.AlreadyAffectedZoneDejaOccupee = false;
                }
                for (int i = 0; i < Z; i++)
                {
                    int TID = int.Parse(Console.ReadLine()); // ID of the team controlling the zone (0, 1, 2, or 3) or -1 if it is not controlled. The zones are given in the same order as in the initialization.
                    Zone zone = zones.Where(x => x.NumeroZone == i).FirstOrDefault();
                    if (zone != null)
                    {
                        zone.IdTeamControllingZone = TID;
                    }
                }
                //limiter la liste du joueur 1 en, premier , les autres drones sont ceux des autres joueurs
                List<Drone> drones = new List<Drone>();

                for (int i = 0; i < P; i++)
                {
                    for (int j = 0; j < D; j++)
                    {
                        inputs = Console.ReadLine().Split(' ');
                        int DX = int.Parse(inputs[0]); // The first D lines contain the coordinates of drones of a player with the ID 0, the following D lines those of the drones of player 1, and thus it continues until the last player.
                        int DY = int.Parse(inputs[1]);
                        //Console.Error.WriteLine("DX" + DX);
                        //Console.Error.WriteLine("DY" + DY);
                        Drone drone = new Drone() { DX = DX, DY = DY, AlreadyPlaced = false, IdJoueur = i, IdDrone = j };
                        drones.Add(drone);
                    }
                }

                //recherche du nombre de drone ennemi dans une zone
                foreach (var items in drones.GroupBy(y => y.IdJoueur))
                {
                    foreach (Zone zone in zones)
                    {
                        //nombre de drone  sur la zone correspondantes pour ce joueur
                        var dronesEnnemisSurZone = items.Where(x => x.DX >= (zone.X - 5) && x.DX <= (zone.X + 5) && x.DY >= (zone.Y - 5) && x.DY <= (zone.Y + 5) && x.IdJoueur != ID);
                        if ((zone.DronesEnnemiSurZone == null) || ((zone.DronesEnnemiSurZone != null && dronesEnnemisSurZone.Count() > zone.DronesEnnemiSurZone.Count())))
                        {
                            zone.DronesEnnemiSurZone = dronesEnnemisSurZone.ToList();
                        }
                        //revoir a + 50 -50
                        var dronesMoiSurZone = items.Where(x => x.DX >= (zone.X - 5) && x.DX <= (zone.X + 5) && x.DY >= (zone.Y - 5) && x.DY <= (zone.Y + 5) && x.IdJoueur == ID);
                        if ((zone.DronesMoiSurZoneSuperieurEnNBr == null) || ((zone.DronesMoiSurZoneSuperieurEnNBr != null && dronesMoiSurZone.Count() > zone.DronesMoiSurZoneSuperieurEnNBr.Count())))
                        {
                            zone.DronesMoiSurZoneSuperieurEnNBr = dronesMoiSurZone.ToList();
                        }
                    }
                }
                Console.Error.WriteLine("Affichage des zones ");
                foreach (Zone zone in zones)
                {
                    Console.Error.WriteLine(zone.ToString());
                }

                List<DeplacementDrone> deplacementDrones = new List<DeplacementDrone>();
                Console.Error.WriteLine("Mouvement des zones ");

                //Bouge pas si on est en superiorité sur ma zone
                foreach (Zone zone in zones)
                {
                    foreach (Drone drone in zone.DronesMoiSurZoneSuperieurEnNBr.Where(x => !x.AlreadyPlaced))
                    {
                        drone.AlreadyPlaced = true;
                        DeplacementDrone deplacementDrone = new DeplacementDrone() { X = zone.X, Y = zone.Y, IdDrone = drone.IdDrone };
                        deplacementDrones.Add(deplacementDrone);
                        Console.Error.WriteLine("Statique Mouv");
                        Console.Error.WriteLine(deplacementDrone.ToString());
                    }
                }

                List<Zone> zoneControllerParEnnemi = zones.Where(x => x.IdTeamControllingZone != ID && x.IdTeamControllingZone != -1).ToList();
                Console.Error.WriteLine(" zoneControllerParEnnemi.Count()" + zoneControllerParEnnemi.Count());

                List<Zone> zoneVide = zones.Where(x => x.IdTeamControllingZone == -1).ToList();
                Console.Error.WriteLine(" zoneVide.Count()" + zoneVide.Count());

                List<Zone> zoneAMoi = zones.Where(x => x.IdTeamControllingZone == ID).ToList();
                Console.Error.WriteLine(" zoneAMoi.Count()" + zoneAMoi.Count());

                Console.Error.WriteLine(" dronesMes.Count()" + drones.Where(x => x.IdJoueur == 0).Count());

                //tant qu'il reste des zones vides et qué l'on a des drones
                while ((zoneVide.Where(x => !x.AlreadyAffectedZoneVides).Count() > 0) && (drones.Where(x => x.IdJoueur == 0 && !x.AlreadyPlaced).Count() > 0))
                {
                    SetDroneZoneVides(drones, zoneVide, deplacementDrones, ID);
                }

                //pour chaque zone controllé par l'ennemi, on envoie au moins un drône
                //SetDroneInZoneControlleParLennemi(drones, zoneControllerParEnnemi, deplacementDrones);

                //parcours des zones ou il ya le moins de drône ennemi possibles
                foreach (Zone zone in zoneControllerParEnnemi.Where(x => x.DronesEnnemiSurZone.Count() > 0 && x.DronesMoiSurZoneSuperieurEnNBr.Count() <= x.DronesEnnemiSurZone.Count()).OrderBy(x => x.DronesEnnemiSurZone.Count()))
                {
                    int nbrDroneSurZone = zone.DronesEnnemiSurZone.Count();
                    List<Drone> mesDronesrestants = drones.Where(x => x.IdJoueur == ID && !x.AlreadyPlaced).ToList();
                    if (mesDronesrestants.Count() >= (nbrDroneSurZone + 1))
                    {
                        List<Drone> mesDronesrestantsAPositionner = mesDronesrestants.Take(nbrDroneSurZone + 1).ToList();
                        foreach (Drone drone in mesDronesrestantsAPositionner.Where(x => !x.AlreadyPlaced))
                        {
                            drone.AlreadyPlaced = true;
                            Console.Error.WriteLine(drone.ToString());
                            Console.Error.WriteLine(zone.ToString());
                            DeplacementDrone deplacementDrone = new DeplacementDrone() { X = zone.X, Y = zone.Y, IdDrone = drone.IdDrone };
                            deplacementDrones.Add(deplacementDrone);
                            Console.Error.WriteLine("zoneControllerParEnnemi Mouvement");
                            Console.Error.WriteLine(deplacementDrone.ToString());
                        }
                    }
                }

                foreach (DeplacementDrone deplacementDrone in deplacementDrones.OrderBy(x => x.IdDrone))
                {
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

        private static void SetDroneZoneVides(List<Drone> drones, List<Zone> zoneVide, List<DeplacementDrone> deplacementDrones, int id)
        {
            //Pour chaques zone vide
            foreach (Zone zone in zoneVide.Where(x => !x.AlreadyAffectedZoneVides))
            {
                SetDrone(drones, deplacementDrones, zone, id);
                zone.AlreadyAffectedZoneVides = true;
            }
        }

        private static void SetDrone(List<Drone> drones, List<DeplacementDrone> deplacementDrones, Zone zone, int id)
        {
            Drone drone = drones.Where(x => x.IdJoueur == id && !x.AlreadyPlaced).FirstOrDefault();
            if (drone != null)
            {
                drone.AlreadyPlaced = true;
                Console.Error.WriteLine(drone.ToString());

                Console.Error.WriteLine(zone.ToString());
                DeplacementDrone deplacementDrone = new DeplacementDrone() { X = zone.X, Y = zone.Y, IdDrone = drone.IdDrone };
                deplacementDrones.Add(deplacementDrone);
                Console.Error.WriteLine("SetDroneZoneVides");
                Console.Error.WriteLine(deplacementDrone.ToString());
            }
        }
    }

    public class Joueur
    {
        public int IDJoueur { get; set; }

        public Drone drone { get; set; }
    }

    public class DeplacementDrone
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int IdDrone { get; set; }

        public override string ToString()
        {
            return "DeplacementDrone : " + X + " " + Y + " " + IdDrone;
        }
    }

    public class Drone
    {
        public int DX { get; set; }
        public int DY { get; set; }
        public bool AlreadyPlaced { get; set; }
        public int IdJoueur { get; set; }
        public int IdDrone { get; set; }

        public override string ToString()
        {
            return "Drone : " + DX + " " + DY + " " + AlreadyPlaced + " " + IdJoueur;
        }
    }

    public class Zone
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int NumeroZone { get; set; }
        public int IdTeamControllingZone { get; set; }
        public bool AlreadyAffectedZoneVides { get; set; }
        public bool AlreadyAffectedZoneDejaOccupee { get; set; }
        public List<Drone> DronesEnnemiSurZone { get; set; }
        public List<Drone> DronesMoiSurZoneSuperieurEnNBr { get; set; }

        public override string ToString()
        {
            string str = "Zone : " + X + " " + Y + " " + NumeroZone + " " + IdTeamControllingZone + " " + AlreadyAffectedZoneVides + " " + AlreadyAffectedZoneDejaOccupee + " ";
            if (DronesEnnemiSurZone != null)
            {
                str += "- DronesEnnemiSurZone :" + DronesEnnemiSurZone.Count() + " ";
                foreach (Drone item in DronesEnnemiSurZone)
                {
                    str += item.ToString();
                }
                str += "- DronesMoiSurZone :" + DronesMoiSurZoneSuperieurEnNBr.Count() + " ";
                foreach (Drone item in DronesMoiSurZoneSuperieurEnNBr)
                {
                    str += item.ToString();
                }
            }

            return str;
        }
    }
}