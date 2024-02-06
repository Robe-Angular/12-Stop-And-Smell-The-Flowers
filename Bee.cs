using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace _12_Stop_And_Smell_The_Flowers
{
    [Serializable]
    class Bee
    {
        public enum BeeState
        {
            Idle,
            FlyingToFlower,
            GatheringNectar,
            ReturningToHive,
            MakingHoney,
            Retired

        }
        private const double HoneyConsumed = 0.5;
        private const int MoveRate = 3;
        private const double MinimumFlowerNectar = 1.5;
        private const int CareerSpan = 1000;

        private BeeState currentState;

        public event EventHandler sendMessage;

        protected void OnSendMessage(BeeEventArgs e)
        {
            if(sendMessage != null)
            {
                sendMessage(this, e);
            }
        }
        

        public BeeState CurrentState { get { return this.currentState; } set {
                this.currentState = value;
                BeeEventArgs e = new BeeEventArgs(this.id, this.currentState.ToString());
                OnSendMessage(e);
            }
        }
        public int Age { get; private set; }
        public bool InsideHive { get; private set; }
        public double NectarCollected { get; private set; }
        private Point location;
        public Point Location { get { return location; } }

        private Hive hive;
        private World world;

        private int id;
        private Flower destinationFlower;
        public Bee(int id, Point initialLocation, Hive hive, World world)
        {
            this.id = id;
            this.Age = 0;
            this.hive = hive;
            this.world = world;
            this.location = initialLocation;
            this.InsideHive = true;
            this.destinationFlower = null;
            this.NectarCollected = 0;
            this.CurrentState = BeeState.Idle;
            
            this.world.Bees.Add(this);
        }

        public void Go(Random random)
        {
            this.Age++;

            switch (CurrentState) 
            {
                case BeeState.Idle:
                    if(this.Age > CareerSpan)
                    {
                        
                        this.CurrentState = BeeState.Retired;
                    }
                    else if (world.Flowers.Count > 0 && hive.ConsumeHoney(HoneyConsumed))
                    {
                        

                        Flower flower = world.Flowers[random.Next(world.Flowers.Count)];
                        if(flower.Nectar >= MinimumFlowerNectar && flower.Alive)
                        {
                            destinationFlower = flower;
                            CurrentState = BeeState.FlyingToFlower;
                        }
                    }
                    
                    

                    break;
                case BeeState.FlyingToFlower:
                    
                    if (!world.Flowers.Contains(destinationFlower))
                        CurrentState = BeeState.ReturningToHive;
                    else if (InsideHive)
                    {
                        if (MoveTowardsLocation(hive.getLocation("Exit")))
                        {
                            InsideHive = false;
                            location = hive.getLocation("Entrance");
                        }
                    }
                    else
                        if (MoveTowardsLocation(destinationFlower.Location))
                            CurrentState = BeeState.GatheringNectar;
                    break;
                case BeeState.GatheringNectar:
                    double nectar = destinationFlower.HarvestNectar();
                    if (nectar > 0)
                        this.NectarCollected += nectar;
                    else
                        CurrentState = BeeState.ReturningToHive;
                    break;
                case BeeState.ReturningToHive:
                    if (!this.InsideHive)
                    {
                        if (MoveTowardsLocation(hive.getLocation("Entrance")))
                        {
                            InsideHive = true;
                            location = hive.getLocation("Exit");
                        }
                        
                        
                        
                    }
                    else
                        if (MoveTowardsLocation(hive.getLocation("Exit")))
                            CurrentState = BeeState.MakingHoney;
                    break;
                case BeeState.MakingHoney:
                    
                    if (this.NectarCollected < 0.5)
                    {
                        this.NectarCollected = 0;
                        this.CurrentState = BeeState.Idle;
                    }
                    else
                        if (hive.AddHoney(0.5))
                            NectarCollected -= 0.5;
                        else
                            NectarCollected = 0;
                    break;
                case BeeState.Retired:
                    //Do nothing we are retired!
                    break;
            }
                


        }

        private bool MoveTowardsLocation(Point destination)
        {
            if(destination != null)
            {
                if (Math.Abs(destination.X - location.X) <= MoveRate &&
                    Math.Abs(destination.Y - location.Y) <= MoveRate)
                    return true;
                if (destination.X > location.X)
                    location.X += MoveRate;
                else if (destination.X < location.X)
                    location.X -= MoveRate;
                if (destination.Y > location.Y)
                    location.Y += MoveRate;
                else if (destination.Y < location.Y)
                    location.Y -= MoveRate;
            }
            return false;
        }
    }
}
