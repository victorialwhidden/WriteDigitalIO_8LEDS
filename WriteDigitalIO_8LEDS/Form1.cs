using System;
using System.Drawing;
using NationalInstruments.DAQmx;
using System.Windows.Forms;

namespace WriteDigitalIO_8LEDS
{
    public partial class Form1 : Form
    {
        NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task(); //Creating a global NI task
        DigitalSingleChannelWriter writer; // create global writer class... designed for writing to single-channel digital outputs
        System.Windows.Forms.CheckBox[] chkArray; //global empty checkbox array


        public Form1()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Has occured in initialization", ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chkArray = new System.Windows.Forms.CheckBox[8] { checkBox1, checkBox2, checkBox3, checkBox4, checkBox5, checkBox6, checkBox7, checkBox8 };
            for (int i = 0; i < chkArray.Length; i++)
            {
                chkArray[i].Checked = false;
            }
                try
            {
                

                physicalChannelComboBox.DropDownStyle = ComboBoxStyle.DropDownList; //changes the combobox to be Non-Editable
                physicalChannelComboBox.Items.AddRange(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.DOPort, PhysicalChannelAccess.External));
                if (physicalChannelComboBox.Items.Count > 0)
                {
                    physicalChannelComboBox.SelectedIndex = 0;

                }
                else {
                    MessageBox.Show("DAQ not on"); 
                    this.Close();
                }

                //Using an EVENT HANDLER instead of the forms method "checkBox1_CheckedChanged"
                //Continuously loops through each checkbox and sees if there is a change
                foreach (System.Windows.Forms.CheckBox chk in chkArray)
                {
                    chk.CheckedChanged += new EventHandler(Uppdate); //Call Uppdate if there is a check changed
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occured in the form load", ex.Message);
            }
        }

        private void Uppdate(object sender, EventArgs e)
        {
            try
            {
                int binaryValue = 0;

                //For a given integer, until the end of the chkArray, increase i+1
                for (int i = 0; i < chkArray.Length; i++)
                {
                    if (chkArray[i].Checked)
                    {
                        binaryValue |= (1 << i); //Shift a 1 into the location of i if there is a checkCHanged
                        chkArray[i].BackColor = Color.Green; //Changes the color of checkbox to Green if 1
                    }
                    else
                    {
                        chkArray[i].BackColor = Color.Salmon; //changes color of CheckBx to SALMON if 0
                    }
                }

                //boolean values (bool[]), where each element represents a digital output (either ON or OFF).
                //Do exact same thing, but make a boolean array with logic

                bool[] booleanArray = new bool[8];
                for (int i = 0; i < 8; i++)
                {
                    booleanArray[i] = (binaryValue & (1 << i)) != 0; //Using != 0 allows handling of all possible non-zero results,
                }

                writer.WriteSingleSampleMultiLine(true, booleanArray); //Should send out the new array to every LED based off of if it is TRUE 
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error in the binary fetching", ex.Message);
            }
        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool[] booleanArray0 = { false, false, false, false, false, false, false, false };  // All values are false

            try
            {
                if (digitalWriteTask != null && physicalChannelComboBox.Items.Count > 0) //Only dispose of the task if it exists
                {
                    writer.WriteSingleSampleMultiLine(true, booleanArray0); //Should send out the new array to every LED based off of if it is TRUE 

                    digitalWriteTask.Dispose();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occured in the form close", ex.Message);
            }
        }

        private void btn_Quit_Click(object sender, EventArgs e)
        {

            Application.Exit();
        }

        private void physicalChannelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < chkArray.Length; i++)
            {
                chkArray[i].Checked = false;
            }

            try
            {
                if (digitalWriteTask != null) //Disposing a task if the combobox is changed
                {
                    digitalWriteTask.Dispose();
                    digitalWriteTask = new NationalInstruments.DAQmx.Task();
                    string selectedPort = physicalChannelComboBox.SelectedItem.ToString();
                    digitalWriteTask.DOChannels.CreateChannel(selectedPort, "", ChannelLineGrouping.OneChannelForAllLines);
                    writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occured when disposing a task for a combobox change", ex.Message);
            }
        }
    }
}