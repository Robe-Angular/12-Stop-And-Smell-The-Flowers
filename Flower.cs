using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace _12_Stop_And_Smell_The_Flowers
{
    [Serializable]
    class Flower
    {
        private Point location;
        private int age;
        private bool alive;
        private double nectar;
        private double nectarHarvested;
        private int lifespan;

        public Point Location { get { return this.location; } }
        public int Age { get { return this.age; } }
        public bool Alive { get { return this.alive; } }
        public double Nectar { get { return this.nectar; } }
        public double NectarHarvested { get { return this.nectarHarvested; } set { this.nectarHarvested = value; } }

        public const int LifeSpanMin = 15000;
        public const int LifeSpanMax = 30000;
        public const double InitialNectar = 1.5;
        public const double MaxNectar = 5.0;
        public const double NectarAddedPerTurn = 0.01;
        public const double NectarGatheredPerTurn = 0.3;

        public Flower(Point location, Random random )
        {
            this.location = location;
            this.age = 0;
            this.alive = true;
            this.nectar = InitialNectar;
            this.nectarHarvested = 0;
            this.lifespan = random.Next(LifeSpanMin, LifeSpanMax + 1);
        }

        public double HarvestNectar()
        {
            if (NectarGatheredPerTurn > this.nectar)
                return 0;

            this.nectarHarvested += NectarGatheredPerTurn;
            this.nectar -= NectarGatheredPerTurn;
            return NectarGatheredPerTurn;
        }

        public void Go()
        {
            this.age++;
            this.alive = this.age <= lifespan;
            if(MaxNectar > this.nectar)
            {
                this.nectar += NectarAddedPerTurn;
            }
        }
    }
}
