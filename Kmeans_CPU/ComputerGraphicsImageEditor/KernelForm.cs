using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComputerGraphicsImageEditor
{
    public partial class KernelForm : Form
    {
        private ImageEditor mainForm = null;
        DataGridViewCell prevAnchorCell = null;
        public KernelForm(int width, int height, ImageEditor ie)
        {
            InitializeComponent();
            mainForm = ie;
            dataGridView1.RowCount = height + 1; //some black magic is happening with the datagrid that required me to add 1 to its size
            dataGridView1.ColumnCount = width;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            int kernelSum = 0;
            if(width == 3 && height == 3)
            {
                for (int i = 0; i < mainForm.kernel.Count; i++)
                {
                    for (int j = 0; j < mainForm.kernel[i].Count; j++)
                    {
                        dataGridView1.Rows[i].Cells[j].Value = mainForm.kernel[i][j];
                        kernelSum += (int)dataGridView1.Rows[i].Cells[j].Value;
                    }
                }
            }
            else
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        cell.Value = 1;
                        kernelSum++;
                    }
                }
            }
            
            dataGridView1.CellValueChanged -= dataGridView1_CellValueChanged;
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
            dataGridView1.CellMouseDown += dataGridView1_CellMouseDown;
            dataGridView1.AllowUserToAddRows = false;
            offsetTextBox.Text = "0";
            if(kernelSum == 0)
            {
                kernelSum = 1;
            }
            divisorTextBox.Text = kernelSum.ToString();

        }

        private void finishButton_Click(object sender, EventArgs e)
        {
            int i = 0;
            mainForm.kernel.Clear();
            for(int k = 0; k < dataGridView1.Rows.Count; k++)
            {
                mainForm.kernel.Add(new List<int>());
                for (int j = 0; j < dataGridView1.Rows[k].Cells.Count; j++)
                {
                    DataGridViewCell cell = dataGridView1.Rows[k].Cells[j];
                    if (cell.Value == null)
                    {
                       // mainForm.kernel[i].Add(0);
                    }
                    else
                    {
                        int value;
                        if(!(cell.Value is int))
                        {
                            int.TryParse(cell.Value.ToString(), out value);
                            mainForm.kernel[i].Add(value);
                        }
                        else
                        {
                            mainForm.kernel[i].Add((int)cell.Value);
                        }
                       
                    }                                     
                }
                i++;
            }
            int offset, div;
            if(!int.TryParse(divisorTextBox.Text, out div) || !int.TryParse(offsetTextBox.Text, out offset))
            {
                MessageBox.Show("Invalid integer values detected, using default values");

            }
            else
            {
                mainForm.divisor = div;
                mainForm.offset = offset;
            }
            this.Close();
        }
        //If value in cell is not a valid integer we set the value to 0
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
            //MessageBox.Show("cell val changed. cell val: " + cell.Value.ToString());
            int res = 0;
            if(cell != null)
            {
                
                if (!int.TryParse(cell.Value.ToString(), out res))
                {
                    cell.Value = 0;
                }
            }
            
        }
        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {                              
                DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                
                cell.Style.BackColor = Color.Green;
                mainForm.anchorXIndex = e.ColumnIndex;
                mainForm.anchorYIndex = e.RowIndex;
                if(prevAnchorCell == null)
                {                   
                    prevAnchorCell = cell;
                }
                else
                {
                    prevAnchorCell.Style.BackColor = Color.White;
                    prevAnchorCell = cell;
                }      
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                mainForm.divisor = 0;
                foreach (List<int> l in mainForm.kernel)
                {
                    foreach (int i in l)
                    {
                        mainForm.divisor += i;
                    }
                }
                if(mainForm.divisor == 0)
                {
                    mainForm.divisor = 1;
                }
                divisorTextBox.Text = mainForm.divisor.ToString();
                offsetTextBox.Text = "0";
                offsetTextBox.ReadOnly = true;
                divisorTextBox.ReadOnly = true;
            }
            else
            {
                offsetTextBox.ReadOnly = false;
                divisorTextBox.ReadOnly = false;
            }
        }
    }
}
