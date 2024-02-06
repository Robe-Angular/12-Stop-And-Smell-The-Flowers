using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace _12_Stop_And_Smell_The_Flowers
{
    public partial class Form1 : Form
    {
        private World world;
        Random random;
        private DateTime start = DateTime.Now;
        private DateTime end;
        private int framesRun = 0;
        private SimulationState simulationState = SimulationState.NoStarted;
        private enum SimulationState
        {
            NoStarted,
            NoPaused,
            Paused

        }
        public Form1()
        {
            InitializeComponent();
            this.world = new World();
            this.random = new Random();

            timer1.Interval = 50;
            timer1.Tick += new EventHandler(RunFrame);
            timer1.Enabled = false;
            UpdateStats(new TimeSpan());
            world.sendFromWorld += World_sendFromWorld;

        }

        private void World_sendFromWorld(object sender, EventArgs e)
        {
            if(e is BeeEventArgs)
            {
                BeeEventArgs beeEventArgs = e as BeeEventArgs;
                sendMessage(beeEventArgs.Id, beeEventArgs.Message);
            }
        }

        private void UpdateStats(TimeSpan frameDuration)
        {
            beesLabel.Text = world.Bees.Count.ToString();
            flowersLabel.Text = world.Flowers.Count.ToString();
            honeyInHiveLabel.Text = String.Format( "{0:f3}", world.hive.Honey);
            double nectar = 0;
            foreach(Flower flower in world.Flowers)
                nectar += flower.Nectar;
            nectarInFlowersLabel.Text = String.Format("{0:f3}", nectar);

            double nectarHarvested = 0;
            foreach (Flower flower in world.Flowers)
                nectarHarvested += flower.NectarHarvested;
            nectarHarvestedLabel.Text = String.Format("{0:f3}", nectarHarvested);

            framesRunLabel.Text = framesRun.ToString();
            double miliSeconds = frameDuration.TotalMilliseconds;
            if (miliSeconds != 0.0)
                frameRateLabel.Text = String.Format("{0:f0} ({1:f1}ms)", 1000 / miliSeconds, miliSeconds);
            else
                frameRateLabel.Text = "N/A";
        }

        public void RunFrame(object sender, EventArgs e)
        {
            framesRun++;
            this.world.Go(random);
            this.end = DateTime.Now;
            TimeSpan frameDuration = end - start;
            start = end;
            UpdateStats(frameDuration);

        }

        private void startButton_Click(object sender, EventArgs e)
        {
            switch (simulationState)
            {
                case SimulationState.NoStarted:
                    this.simulationState = SimulationState.NoPaused;
                    this.timer1.Start();
                    this.startButton.Text= "Pause Simulation";
                    break;
                case SimulationState.NoPaused:
                    this.simulationState = SimulationState.Paused;
                    this.timer1.Stop();
                    this.startButton.Text = "Resume Simulation";
                    break;
                case SimulationState.Paused:
                    this.simulationState = SimulationState.NoPaused;
                    this.timer1.Start();
                    this.startButton.Text = "Pause Simulation";
                    break;
            }
                

            

        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            this.simulationState = SimulationState.NoStarted;
            this.startButton.Text = "Start Simulation";
            this.timer1.Stop();
            framesRun = 0;
            this.world = new World();
            world.sendFromWorld += World_sendFromWorld;
        }

        private void sendMessage(int id, string message)
        {
            statusLabel.Text = "Bee #" + id + ": " + message;
            var beeGroups =
                from bee in world.Bees
                group bee by bee.CurrentState into beeGroup
                orderby beeGroup.Key
                select beeGroup;
            listBox1.Items.Clear();
            foreach(var group in beeGroups)
            {
                string s;
                if(group.Count() == 1)
                {
                    s = "";
                }
                else
                {
                    s = "s";
                }
                listBox1.Items.Add(group.Key.ToString() + ": " + group.Count() + "bee" + s);
                if(group.Key == Bee.BeeState.Idle && group.Count() == world.Bees.Count() && framesRun > 0)
                {
                    listBox1.Items.Add("Simulation ended: All bees are idle");
                    toolStrip1.Items[0].Text = "Simulation ended";
                    statusStrip1.Items[0].Text = "Simulation ended";
                    timer1.Stop();
                }
            }


        }

        private void abrirToolStripButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "(*.ws)|*ws";
            DialogResult result = openFileDialog1.ShowDialog();
            World currentWorld = this.world;
            int currentFrames = this.framesRun;
            if(result == DialogResult.OK)
            {
                try
                {
                    using (Stream input = File.OpenRead(openFileDialog1.FileName))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        this.world = (World)bf.Deserialize(input);
                        world.sendFromWorld += World_sendFromWorld;
                        this.framesRun = (int)bf.Deserialize(input);
                        this.UpdateStats(new TimeSpan());
                    }
                }
                catch
                {
                    MessageBox.Show("The file is invalid", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.world = currentWorld;
                    this.framesRun = currentFrames;
                }
            }
            
        }

        private void guardarToolStripButton_Click(object sender, EventArgs e)
        {
            this.timer1.Stop();
            
            saveFileDialog1.Filter = "(*.ws)|*ws";
            DialogResult result = saveFileDialog1.ShowDialog();
            saveFileDialog1.DefaultExt = "ws";
            saveFileDialog1.AddExtension = true;
            if(result == DialogResult.OK)
            {
                using(Stream output = File.Create(saveFileDialog1.FileName))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    world.sendFromWorld -= World_sendFromWorld;
                    bf.Serialize(output, this.world);
                    bf.Serialize(output, this.framesRun);
                    world.sendFromWorld += World_sendFromWorld;
                }
            }
        }

        private void imprimirToolStripButton_Click(object sender, EventArgs e)
        {

        }

        
    }
}
