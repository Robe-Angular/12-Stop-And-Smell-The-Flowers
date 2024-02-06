using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace _12_Stop_And_Smell_The_Flowers
{
    [Serializable]
    class Hive
    {
        private double honey;
        public double Honey { get { return this.honey; } }
        private Dictionary<string, Point> locations;
        private int beeCount;
        private Random random;
        private World world;


        const int InitialNumberOfBees = 6;
        const double StartingAmountOfHoney = 3.2;
        const double MaxAmountOfHoney = 15.0;
        const double NectarRatio = 0.25;
        const int MaxNumberOfBees = 8;
        const double MinimumHoneyToBirth = 4.0;

        public Hive(World world)
        {
            this.world = world;
            this.honey = StartingAmountOfHoney;
            this.random = new Random();
            this.InitializeLocations();
            for(int i = 0; i < InitialNumberOfBees; i++)
            {
                this.AddBee(this.random);
            }
        }

        private void InitializeLocations()
        {
            locations = new Dictionary<string, Point>();
            locations.Add("Entrance", new Point(600, 100));
            locations.Add("Nursery", new Point(95, 174));
            locations.Add("HoneyFactory", new Point(157, 198));
            locations.Add("Exit", new Point(194, 213));
        }

        public bool AddHoney(double nectar)
        {
            double honeyToAdd = nectar * NectarRatio;
            if (honeyToAdd + this.Honey > MaxAmountOfHoney)
                return false;
            this.honey += honeyToAdd;

            return true;
        }

        public bool ConsumeHoney(double amount)
        {
            
            if (amount > this.honey)
                return false;
            else {
                this.honey -= amount;
                return true;
            }
            
        }

        private void AddBee(Random random)
        {
            if (this.world.Bees.Count == MaxNumberOfBees)
                return;
            beeCount++;
            int r1 = random.Next(100) - 50;
            int r2 = random.Next(100) - 50;
            Point startPoint = new Point(locations["Nursery"].X + r1, locations["Nursery"].Y + r2);
            Bee newBee = new Bee(beeCount, startPoint,this,this.world);
            newBee.sendMessage += Bee_OnSendMessage;
            
        }

        private void Bee_OnSendMessage(Object sender, EventArgs e)
        {
            if(e is BeeEventArgs)
            {
                BeeEventArgs beeEventArgs = e as BeeEventArgs;
                OnSendFromHive(beeEventArgs);
            }
        }

        public event EventHandler sendFromHive;

        protected void OnSendFromHive(BeeEventArgs e)
        {
            if(sendFromHive != null)
            {
                sendFromHive(this, e);
            }
        }

        public void Go(Random random)
        { 
            if (this.honey > MinimumHoneyToBirth && random.Next(10) == 1)
                AddBee(this.random);
                
        }

        public Point getLocation(string location)
        {
            Point pointToReturn;
            if (this.locations.TryGetValue(location, out pointToReturn))
                return pointToReturn;
            else
                throw (new ArgumentException("Unknown location" + location));
        }


    }
}
