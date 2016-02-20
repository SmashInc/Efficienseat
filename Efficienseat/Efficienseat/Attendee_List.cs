﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Efficienseat
{
    public partial class Attendee_List : Form
    {

        private List<Attendee> attendeeList = new List<Attendee>();

        public Attendee_List()
        {
            InitializeComponent();
        }

        private void Attendee_List_Load(object sender, EventArgs e)
        {
            // Set list of images for listview
            lvwAttendee.LargeImageList = imlTableNumbers;

            // Hide close button
            this.ControlBox = false;

            attendeeList = populateAttendees();
        } //END Attendee_List_Load EVENT

        // BUTTONS
        #region FormButtons

        private void btnAddAttendee_Click(object sender, EventArgs e)
        {
            addAttendee();   
        } //END btnAddAttendee_Click EVENT

        private void btnChangeView_Click(object sender, EventArgs e)
        {
            changeView();
        } //END btnChangeView_Click EVENT

        private void btnEditEntry_Click(object sender, EventArgs e)
        {

        } //END btnEditEntry_Click EVENT

        private void btnRemoveAttendee_Click(object sender, EventArgs e)
        {
            //To fix: Throws ArgumentOutOfRangeException if no guest selected
            removeAttendee(lvwAttendee.SelectedItems[0]);
        } //END btnRemoveAttendee_Click EVENT

        #endregion FormButtons

        // CONTEXT-SHORTCUT MENU
        #region ListViewContextMenu

        private void tmiRemoveAttendee_Click(object sender, EventArgs e)
        {
            // TODO: update to retrieve item under mouse
            removeAttendee(lvwAttendee.SelectedItems[0]);
        }

        private void tmiEdit_Click(object sender, EventArgs e)
        {
            editEntry();
        }

        private void tmiImport_Click(object sender, EventArgs e)
        {
            importAttendees();
        }

        #endregion ListViewContextMenu

        // METHODS
        #region Methods
        
        // Load attendees from DB
        private List<Attendee> populateAttendees()
        {
            List<Attendee> attendees = new List<Attendee>();

            return attendees;
        }
        
        // add item
        private void addAttendee()
        {
            //Create edit entry window for data population
            //  Pull data form form rather than passing through
            using (DataEntryForm data = new DataEntryForm())
            {                
                if (data.ShowDialog(this) == DialogResult.OK)
                {
                    string name = data.getFirstName + " " + data.getLastName;
                    string RSVP = data.getRSVP;
                    string address = data.getAddress1 + " " + data.getAddress2 + " " + data.getCity + ", " + data.getState + " " + data.getZIP;

                    ListViewItem newGuest = new ListViewItem(new string[] { name, RSVP, address, "" });
                    newGuest.ImageIndex = 0;
                    newGuest.Group = lvwAttendee.Groups[0];
                    lvwAttendee.Items.Add(newGuest);
                }
            }
        }

        // change listview visuals
        private void changeView()
        {
            if (lvwAttendee.View == View.Details)
            {
                lvwAttendee.View = View.Tile;
                lvwAttendee.ShowGroups = true;
            }
            else
            {
                lvwAttendee.View = View.Details;
                lvwAttendee.ShowGroups = false;
            }
        }

        // change data in selected item
        private void editEntry()
        {
            //should populate with selected user data
            //check for a user selection from listView prior to instantiating window
            if (lvwAttendee.SelectedItems.Count == 1)
            {
                //DataEntryForm will need a parameterized constructor to account for pushed data
                DataEntryForm def = new DataEntryForm();
                def.ShowDialog();
            }
        }

        // Remove selected item
        private void removeAttendee(ListViewItem lvi)
        {
            lvi.Remove();
            
            //use code to remove the entry from the DB
        }

        //import attendees from text file
        private void importAttendees()
        {
            string absolutePath = null;
            string line = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();

            //set options for open file menu
            openFileDialog.DefaultExt = "txt";
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.InitialDirectory = "C:\\";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = false;

            //if user selects file and hits confirms
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                absolutePath = openFileDialog.FileName;
            }
            openFileDialog.Dispose();

            //open file and parse by line
            try
            {
                StreamReader filestream = new StreamReader(absolutePath);

                //iterate through file line by line
                while((line = filestream.ReadLine()) != null)
                {
                    //control for item equality
                    bool equal = false;

                    //iterate through listviewitems to compare with current line from txt file
                    //  sloppy as it iterates through the entire listview for each line item
                    //  --Potentially faster to check txt file for duplicates first and then only
                    //      check existing listviewitems for equality?
                    foreach(ListViewItem lvi in lvwAttendee.Items)
                    {
                        //if names are equal, do not add and break from search
                        if (line.Equals(lvi.SubItems[0].Text))
                        {
                            equal = true;
                            break;
                        }
                    }
                    //if name does not already exist, add item to ListView
                    if (!equal)
                    {
                        ListViewItem newGuest = new ListViewItem(new string[] { line, "", "", "" });
                        newGuest.ImageIndex = 0;
                        newGuest.Group = lvwAttendee.Groups[0];
                        lvwAttendee.Items.Add(newGuest);
                    }
                }
                //close open stream
                filestream.Dispose();
            }
            catch(Exception e)
            {
                MessageBox.Show("Error: could not open file.\n Message: " + e.ToString());
            }

        }

        #endregion Methods
    }
}
