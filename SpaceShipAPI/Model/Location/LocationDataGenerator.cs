using System.Text;

namespace SpaceShipAPI.Model.Location;

public class LocationDataGenerator
{
   private static readonly string[] NAMES = new []{"Mebbomia", "Cruxeliv", "Zeron", "Etov", "Cueter", "Soiter", "Ogantu", "Lorix",
      "Vothagantu", "Tiliea", "Brosie", "Ema", "Vegenov", "Nomia", "Hietania", "Iclite", "Draloturn", "Thomone", "Nion",
      "Drypso", "Euruta", "Crosie", "Noveria", "Abatov", "Ferrix", "Rakis"};
   
   private static readonly string[] GREEK = new[]{"Alpha", "Beta", "Gamma", "Delta", "Epsilon", "Zeta", "Eta", "Theta", "Iota", "Kappa",
      "Lambda", "Mu", "Nu", "Xi", "Omicron", "Pi", "Rho", "Sigma", "Tau", "Upsilon", "Phi", "Chi", "Psi", "Omega"};

   private Random random;
   
   public LocationDataGenerator(Random random) {
      this.random = random;
   }

   public String DetermineName()
   {
      StringBuilder nameBuilder = new StringBuilder(NAMES[random.Next(NAMES.Length)]);
      nameBuilder.Append("-");
      nameBuilder.Append(random.Next(1, 10000));
      if (random.Next(2) == 1) {
         nameBuilder.Append((char) random.Next(65, 91));
      } else {
         nameBuilder.Append("-");
         nameBuilder.Append(GREEK[random.Next(GREEK.Length)]);
      }
      return nameBuilder.ToString();
   }
   
   public bool DeterminePlanetFound(int efficiency, double hours, int distance) {
      double _base = hours * distance * efficiency;
      Random random = new Random(); 
      double randomNum = random.NextDouble() * 10.0; 
      return _base * randomNum > 50;
   }
   
   public int DetermineResourceReserves(int efficiency, double hours, bool prioritize) {
      double weighedEfficiency = efficiency * (prioritize ? 15 : 0.5);
      double weighedHours = hours * (prioritize ? 40 : 5);
      double _base = weighedEfficiency + weighedHours;
      int bound = (int) (200 * Math.Ceiling(efficiency / 10.0));
      int randomNum = random.Next(bound);
      int generatedNumber = (int) Math.Round(_base + randomNum);
      return Math.Min(1000, generatedNumber);
   }

   public int DetermineDistance(int efficiency, int distance, bool prioritize) {
      double weighedEfficiency = efficiency * (prioritize ? 1 : 0.5);
      double weighedDistance = distance * (prioritize ? 0.25 : 0.75);
      int origin = Math.Max(1, (int) (distance - weighedEfficiency));
      int bound = (int) (distance + weighedDistance);
      return random.Next(origin, bound);
   }
}