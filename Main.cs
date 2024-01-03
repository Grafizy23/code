using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace INMGT_POS
{
    public partial class Main : Form
    {
        private int currentNo = 1;
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (dataGridView1.Columns.Count == 0)
            {
                dataGridView1.ColumnCount = 6;
                dataGridView1.Columns[0].Name = "No";
                dataGridView1.Columns[1].Name = "Item";
                dataGridView1.Columns[2].Name = "Category";
                dataGridView1.Columns[3].Name = "Size";
                dataGridView1.Columns[4].Name = "Quantity";
                dataGridView1.Columns[5].Name = "Price";
            }
        }
        private void AddRow(string[] rowData)
        {
            // Ensure columns are added only once
            if (dataGridView1.Columns.Count == 0)
            {
                dataGridView1.ColumnCount = 6;
                dataGridView1.Columns[0].Name = "No";
                dataGridView1.Columns[1].Name = "Item";
                dataGridView1.Columns[2].Name = "Category";
                dataGridView1.Columns[3].Name = "Size";
                dataGridView1.Columns[4].Name = "Quantity";
                dataGridView1.Columns[5].Name = "Price";
            }

            // Check if a similar row already exists
            bool rowExists = false;
            foreach (DataGridViewRow existingRow in dataGridView1.Rows)
            {
                if (AreRowsEqual(existingRow, rowData))
                {
                    rowExists = true;
                    break;
                }
            }

            // Add the row only if it doesn't exist
            if (!rowExists)
            {
                dataGridView1.Rows.Add(rowData);

                // Increment the No attribute
                currentNo++;

                // Increment the Quantity attribute
                int rowIndex = dataGridView1.Rows.Count - 1;
                IncrementQuantity(rowIndex);
            }
        }
        private bool AreRowsEqual(DataGridViewRow row1, string[] row2Data)
        {
            // Compare No, Item, and Produce columns
            for (int i = 0; i < 3; i++)
            {
                if (row1.Cells[i].Value?.ToString() != row2Data[i])
                {
                    return false;
                }
            }
            return true;
        }
        private void IncrementQuantity(int rowIndex)
        {
            // Get the current quantity from the specified row
            int currentQuantity;
            if (int.TryParse(dataGridView1.Rows[rowIndex].Cells[3].Value?.ToString(), out currentQuantity))
            {
                // Increment the quantity
                currentQuantity++;

                // Update the quantity in the DataGridView
                dataGridView1.Rows[rowIndex].Cells[3].Value = currentQuantity.ToString();
            }
        }

        private void cartbtn_Click(object sender, EventArgs e)
        {
            string size = sizecb.Text;
            string price = GetPriceBasedOnSize(size);

            if (price != null)
            {
                string[] row = new string[] { currentNo.ToString(), "Brown Sugar", "Cold", size, qtytb.Text, $"P{price}" };
                AddRow(row);
                UpdateTotal();
            }
            else
            {
                MessageBox.Show("Price not found for the selected size.");
            }
        }

        private string GetPriceBasedOnSize(string size)
        {
            // Add your if-else conditions to determine the price based on the selected size
            if (size == "Small")
            {
                return "30.00";
            }
            else if (size == "Medium")
            {
                return "40.00";
            }
            else if (size == "Large")
            {
                return "50.00";
            }
            else
            {
                // Return null or any default value if the size is not recognized
                return null;
            }
        }
        private void CalculateTotal()
        {
            decimal totalAmount = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    string priceString = row.Cells["Price"].Value.ToString().Replace("P00.00", ""); // Remove "P" from the price string
                    decimal price = Convert.ToDecimal(priceString);
                    totalAmount += price;
                }
            }

            totallbl.Text = $"Total: P{totalAmount.ToString("")}";
        }

        // Call this method whenever you want to update the total (e.g., after adding or editing rows)
        private void UpdateTotal()
        {
            CalculateTotal();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void paybtn_Click(object sender, EventArgs e)
        {
            try
            {
                string size = sizecb.Text;
                int quantity = int.Parse(qtytb.Text);

                string query = "SELECT Price FROM dataGridView1 WHERE Size = @Size";
                SqlCommand cmd = new SqlCommand(query);
                cmd.Parameters.AddWithValue("@Size", size);

                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    decimal price = Convert.ToDecimal(result);
                    decimal total = quantity * price;
                    totallbl.Text = total.ToString();
                }
                else
                {
                    MessageBox.Show("Product not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
