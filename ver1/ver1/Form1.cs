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

namespace ver1
{
    public partial class Form1 : Form
    {
        private SqlDataAdapter adapter;
        private DataTable QLDT;
        Test2Entities5 db = new Test2Entities5();
        SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-0LSPCG8\TUTHANH;Initial Catalog=Test2;Integrated Security=True");
        public Form1()
        {
            InitializeComponent();
            load();
        }

        private void Reset()
        {
            foreach (ListViewItem item in lv_ds.Items)
            {
                item.Selected = false;
            }

            txtMaDon.Clear();
            txtTenPhim.Clear();
            txtQuocGia.Clear();
            rbtTinhCam.Checked = false;
            rbtHanhDong.Checked = false;
            dTNgayCongChieu.Value = DateTime.Now;
            txtDTQD.Clear();
            rB3D.Checked = false;
            rB2D.Checked = false;
            txtPTXCDB.Clear();
            txtPTGD.Clear();
        }

        private void rB2D_CheckedChanged(object sender, EventArgs e)
        {
            lblPTGD.Visible = true;
            txtPTGD.Visible = true;
            lblPTXCDB.Visible = false;
            txtPTXCDB.Visible = false;
        }

        private void rB3D_CheckedChanged(object sender, EventArgs e)
        {
            lblPTGD.Visible = false;
            txtPTGD.Visible = false;
            lblPTXCDB.Visible = true;
            txtPTXCDB.Visible = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblPTGD.Visible = true;
            txtPTGD.Visible = true;
            lblPTXCDB.Visible = false;
            txtPTXCDB.Visible = false;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lv_ds.SelectedItems.Count > 0)
            {
                //get id in listview
                string madon = lv_ds.SelectedItems[0].SubItems[0].Text;
                txtMaDon.Enabled = false;
                //find in _db if exists ?
                var thu = db.QLDTs.SingleOrDefault(z => z.MaDon == madon);
                if (thu != null)
                {
                    txtMaDon.Text = thu.MaDon.Trim();
                    txtTenPhim.Text = thu.TenPhim.Trim();
                    txtQuocGia.Text = thu.QuocGia.Trim();
                    if (lv_ds.SelectedItems[0].SubItems[2].Text == rbtTinhCam.Text)
                        rbtTinhCam.Checked = true;
                    else
                        rbtHanhDong.Checked = true;
                    dTNgayCongChieu.Value = thu.NgayCongChieu.Value;
                    txtDTQD.Text = thu.DoTuoi.HasValue ? thu.DoTuoi.Value.ToString() : string.Empty;
                    if (thu.DinhDang.ToString() == "2D")
                    {
                        rB2D.Checked = true;
                        txtPTGD.Text = thu.PTGD.ToString();
                        txtPTXCDB.Clear();
                    }
                    else
                    {
                        rB3D.Checked = true;
                        txtPTXCDB.Text = thu.PTSCDB.ToString().Trim();
                        txtPTGD.Clear();
                    }
                }
                else
                {
                    MessageBox.Show("ERROL", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            
        }
        public void load() // tai len danh sach trong sql
        {
            lv_ds.View = View.Details;
            lv_ds.GridLines = true;
            lv_ds.Columns.Add("MaDon");
            lv_ds.Columns.Add("TenPhim");
            lv_ds.Columns.Add("TheLoai");
            lv_ds.Columns.Add("NgayCongChieu");
            connection.Open();
            SqlCommand cmd = new SqlCommand("Select * From QLDT", connection);
            SqlDataReader da;
            da = cmd.ExecuteReader();
            while (da.Read())
            {
                var item1 = lv_ds.Items.Add(da[0].ToString());
                item1.SubItems.Add(da[1].ToString());
                item1.SubItems.Add(da[3].ToString());
                item1.SubItems.Add(da[4].ToString());

            }
            connection.Close();

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc muốn đóng chương trình?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            Reset();
            txtMaDon.Focus();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            using (Test2Entities5 db = new Test2Entities5())
            {
                QLDT phim = new QLDT();
                phim.MaDon = txtMaDon.Text;
                phim.TenPhim = txtTenPhim.Text;
                phim.TheLoai = rbtTinhCam.Text;
                phim.TheLoai = rbtHanhDong.Checked ? rbtTinhCam.Text : (rbtTinhCam.Checked ? rbtHanhDong.Text : "NONE");
                phim.NgayCongChieu = dTNgayCongChieu.Value;
                phim.QuocGia = txtQuocGia.Text;
                //        phim.DoTuoi = Convert.ToInt32(txtDTQD.Text);
                ListViewItem listViewItem = new ListViewItem(txtMaDon.Text);
                listViewItem.SubItems.Add(rB2D.Checked ? "2D" : "3D");

                if (rB2D.Checked)
                {
                    phim.PTGD = Convert.ToInt32(txtPTGD.Text);
                }
                if (rB3D.Checked)
                {
                    phim.PTSCDB = Convert.ToInt32(txtPTXCDB.Text);
                }
                db.QLDTs.Add(phim);
                db.SaveChanges();
                load();
            }

            Reset();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (lv_ds.SelectedItems.Count > 0)
                {
                    lv_ds.Items.Remove(lv_ds.SelectedItems[0]);
                    db.SaveChanges();
                }
            }
            else
            {
                txtMaDon.Text = "";
                txtTenPhim.Text = "";
                txtQuocGia.Text = "";
                dTNgayCongChieu.Value = DateTime.Now;
                txtDTQD.Text = "";
                txtPTXCDB.Text = "";
                txtPTGD.Text = "";
            }
            Reset();
        }
    }
}
