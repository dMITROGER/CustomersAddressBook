using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRUDSQLServer
{
    public partial class Form1 : Form
    {
        TestEntities testEntities;
        public Form1()
        {
            InitializeComponent();
        }

        // search box function (works after pressing Enter in keyboard)
        private void txbSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (string.IsNullOrEmpty(txbSearch.Text))
                {
                    dataGridView.DataSource = customerBindingSource;
                }
                else
                {
                        var query = from o in customerBindingSource.DataSource as List<Customer>
                                    where o.CustomerID.CompareTo(txbSearch.Text.Trim()) >= 0 || 
                                    o.FullName.Contains(txbSearch.Text.Trim()) ||
                                    o.Address.Contains(txbSearch.Text.Trim())
                                    select o;
                   
                        dataGridView.DataSource = query.ToList();
                    
                }
            }
        }

        // adding new customer funtion
        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                panel.Enabled = true;
                txbCustomerID.Focus();
                Customer c = new Customer();
                testEntities.Customers.Add(c);
                customerBindingSource.Add(c);
                customerBindingSource.MoveLast();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //edit selected customer function
        private void btnEdit_Click(object sender, EventArgs e)
        {
            panel.Enabled = true;
        }

        // cancel funtion
        private void btnCancel_Click(object sender, EventArgs e)
        {
            panel.Enabled = false;
            customerBindingSource.ResetBindings(false);
            foreach (DbEntityEntry entry in testEntities.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                }

            }
            dataGridView.DataSource = customerBindingSource;

        }

        // saving function
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                customerBindingSource.EndEdit();
                testEntities.SaveChangesAsync();
                panel.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                customerBindingSource.ResetBindings(false);
            }
        }

        //loading form and populating datagrid with data from database
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Size = new System.Drawing.Size(500, 460);
            panel.Enabled = false;
            testEntities = new TestEntities();
            customerBindingSource.DataSource = testEntities.Customers.ToList();
        }

        // populating datagrid with data from database
        private void dataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (MessageBox.Show("Are you sure that you want to delete this record?", "Message",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    testEntities.Customers.Remove(customerBindingSource.Current as Customer);
                    customerBindingSource.RemoveCurrent();
                }
            }
        }
    }
}
