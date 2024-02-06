using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace _12_Stop_And_Smell_The_Flowers
{
    [Serializable]
    class World
    {
        private const double NectarHarvestPerNewFlower = 50.0;
        private const int FieldMinX = 15;
        private const int FieldMinY = 17 ;
        private const int FieldMaxX = 690;
        private const int FieldMaxY = 290;

        public Hive hive;
        public List<Bee> Bees;
        public List<Flower> Flowers;

        public event EventHandler sendFromWorld;

        public World()
        {
            this.Bees = new List<Bee>();
            this.Flowers = new List<Flower>();
            Random random = new Random();
            for(int i = 0; i < 10; i++)
            {
                this.AddFlower(random);
            }
            this.hive = new Hive(this);
            hive.sendFromHive += Hive_OnSendFromHive;
        }
        private void Hive_OnSendFromHive(Object sender, EventArgs e)
        {
            if(e is BeeEventArgs)
            {
                BeeEventArgs beeEventArgs = e as BeeEventArgs;
                OnSendFromWorld(beeEventArgs);
            }
        }

        protected void OnSendFromWorld(BeeEventArgs e)
        {
            if(sendFromWorld != null)
            {
                sendFromWorld(this, e);
            }
        }

        public void Go(Random random)
        {
            this.hive.Go(random);

            for(int i = Bees.Count - 1;i >= 0; i--)
            {
                Bee bee = Bees[i];
                bee.Go(random);
                if (bee.CurrentState == Bee.BeeState.Retired)
                    Bees.Remove(bee);
            }

            double totalNectarHarvested = 0;
            for(int i = Flowers.Count - 1; i >= 0; i--)
            {
                Flower flower = Flowers[i];
                flower.Go();
                totalNectarHarvested += flower.NectarHarvested;
                if (!flower.Alive)
                    Flowers.Remove(flower);
            }

            if(totalNectarHarvested > NectarHarvestPerNewFlower)
            {
                foreach (Flower flower in Flowers)
                    flower.NectarHarvested = 0;
                AddFlower(random);
            }
        }

        private void AddFlower(Random random)
        {
            Point location = new Point(random.Next(FieldMinX, FieldMaxX), random.Next(FieldMinY,FieldMaxY));
            Flower newFlower = new Flower(location, random);
            Flowers.Add(newFlower);
        }
    }
}
