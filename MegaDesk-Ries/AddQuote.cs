﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MegaDesk_Ries;
using System.IO;

namespace MegaDesk_Ries
{
    public partial class AddQuote : Form
    {
        public AddQuote()
        {
            InitializeComponent();

            // Use List to populate the material box with available options from declared eNum
            List<DeskMaterial> DeskMaterialList = Enum.GetValues(typeof(DeskMaterial)).Cast<DeskMaterial>().ToList();
            deskMaterialComboBox.DataSource = DeskMaterialList;
        }

        private void cancelQuoteButton_Click(object sender, EventArgs e)
        {
            var ShowMainMenu = (MainMenu)Tag;
            ShowMainMenu.Show();
            this.Close();
        }

        private void AddQuote_Load(object sender, EventArgs e)
        {
            this.ActiveControl = custNameText;
        }



        /*********************************************************************************************
         * Submit Quote
         * This button will validate all data entered, and prompt user to enter valid data if there is
         * an error.
         * If the data is valid, the method will commit all variavles to the DeskQuote object to store
         * ******************************************************************************************/
        private void submitQuoteButton_Click(object sender, EventArgs e)
        {
            // Create a new desk and populate it with the selected values
            Desk desk = new Desk();
            desk.setWidth(Convert.ToInt32(Math.Round(deskWidthText.Value, 0)));
            desk.setDepth(Convert.ToInt32(Math.Round(deskDepthText.Value, 0)));
            desk.setNumdrawers(Convert.ToInt32(Math.Round(deskDrawersText.Value, 0)));
            desk.Material = (DeskMaterial)deskMaterialComboBox.SelectedValue;

            // Create the customer and rushDays preference
            string custName = custNameText.Text;
            int rushDays = 14;

            // Determine if default rush days is changed
            if (threeDayRadio.Checked)
                rushDays = 3;
            if (fiveDayRadio.Checked)
                rushDays = 5;
            if (sevenDayRadio.Checked)
                rushDays = 7;

            // Set the quote date to now
            DateTime quoteDate = DateTime.Now;

            // Create the quote object
            DeskQuote newQuote = new DeskQuote(custName, rushDays, quoteDate, desk);

            // Calculate quote total
            int totalQuote = newQuote.calcuateQuote(desk);
            newQuote.setTotalQuote(totalQuote);

            // Write the quote to file in CSV format
            var QuoteOutput = custName + "," + quoteDate + "," + desk.getWidth() + "," + desk.getDepth() + "," + desk.getDrawers() + ","
                + desk.Material + "," + rushDays + "," + totalQuote;
            string cFile = @"quotes.txt";

            // if file does not exist, create a file
            if (!File.Exists(cFile))
                using (StreamWriter sw = File.CreateText(cFile)) {}

            // file exists, write the data
            using (StreamWriter swa = File.AppendText(cFile))
            {
                swa.WriteLine(QuoteOutput);
                swa.Close();
            }

            

            // Quote file is created at this point. Display the quote in a new form
            DisplayQuote displayQuoteView = new DisplayQuote(newQuote);
            displayQuoteView.Tag = this;
            displayQuoteView.Show(this);
        }

        private void deskWidthText_Validating(object sender, CancelEventArgs e)
        {
            string errorMsg;
            if(!ValidWidth(int.Parse(deskWidthText.Text), out errorMsg))
            {
                e.Cancel = true;
                deskWidthText.Select();

                this.errorProvider1.SetError(deskWidthText, errorMsg);
            }
        }

        private bool ValidWidth(int width, out string errorMessage)
        {
            // validate width
            if (width < 24 || width > 96)
            {
                // ERROR
                errorMessage = "Width must be between 24 and 96";
                return false;
            }
            // Width is valid
            errorMessage = "";
            return true;
        }

        private void deskWidthText_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(deskWidthText, "");
        }

        private void deskDepthText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar))
            // Valid input
            {
                ;
            }
        }
    }
}
