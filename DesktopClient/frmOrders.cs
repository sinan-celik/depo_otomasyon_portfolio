using BAT_Class_Library;
using DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DesktopClient
{
    public partial class frmOrders : Form
    {
        //Fields
        OrdersData ordersData = new OrdersData();
        OrderDetailsData orderDetailsData = new OrderDetailsData();
        Order order = new Order();
        PlacesData placesData = new PlacesData();
        List<Place> lstPlaces;

        public frmOrders()
        {
            InitializeComponent();
            dgvOrders.RowTemplate.Height = 50;
            lstPlaces = placesData.GetAllPlaces();
            cmbPlaces.ValueMember = "Id";
            cmbPlaces.DisplayMember = "Name";
            cmbPlaces.DataSource = lstPlaces;

            new TouchGrid(dgvOrders);

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            dgvOrders.DataSource = ordersData.GetAllOrders();

            dgvOrders.Columns["Id"].Width = 20;

            dgvOrders.Columns["LastUpdateDate"].Visible = false;
            dgvOrders.Columns["LastUptadeUser"].Visible = false;
            dgvOrders.Columns["IsDeleted"].Visible = false;
            dgvOrders.Columns["PlaceId"].Visible = false;


            dgvOrders.Columns["Id"].DisplayIndex = 0;
            dgvOrders.Columns["OrderNo"].DisplayIndex = 1;
            dgvOrders.Columns["PlaceId"].DisplayIndex = 2;
            dgvOrders.Columns["IsCompleted"].DisplayIndex = 3;



            DataGridViewComboBoxColumn place = new DataGridViewComboBoxColumn();
            place.DataSource = placesData.GetAllPlaces();
            place.HeaderText = "Place";
            place.DataPropertyName = "PlaceId";
            place.ValueMember = "Id";
            place.DisplayMember = "Name";

            dgvOrders.Columns.Add(place);



        }

        public void AssignOrderClass()
        {
            order.Id = Convert.ToInt32(txtId.Text);
            order.PlaceId = Convert.ToInt32(cmbPlaces.SelectedValue);
            order.OrderNo = txtOrderNo.Text;
            order.IsCompleted = chkIsCompleted.Checked;
            order.CreateDate = DateTime.Now;
            order.CreateUser = "";
            order.LastUpdateDate = DateTime.Now;
            order.LastUptadeUser = "";
            order.IsDeleted = false;

        }

        public void AssignOrderFormFromClass()
        {
            txtId.Text = order.Id.ToString();
            cmbPlaces.SelectedValue = order.PlaceId;
            txtOrderNo.Text = order.OrderNo;
            chkIsCompleted.Checked = order.IsCompleted;

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (CheckValues())
            {
                AssignOrderClass();
                ordersData.InsertOrder(order);
            }
        }

        private bool CheckValues()
        {
            return true;
        }

        private void dgvOrders_SelectionChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvOrders.SelectedRows)
            {
                order.Id = (int)row.Cells["Id"].Value;
                order.PlaceId = (int)row.Cells["PlaceId"].Value;
                order.OrderNo = row.Cells["OrderNo"].Value.ToString();
                order.IsCompleted = (bool)row.Cells["IsCompleted"].Value;
            }

            AssignOrderFormFromClass();

            FillOrderDetailGrid(order.Id);
        }

        private void FillOrderDetailGrid(int id)
        {
            List<OrderDetail> detailList = orderDetailsData.GetOrderDetailsByOrderId(id);

            dgvOrderDetails.DataSource = detailList;
        }
    }
}
