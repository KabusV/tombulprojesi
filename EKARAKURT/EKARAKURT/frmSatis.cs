using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EKARAKURT
{
    public partial class frmSatis : Form
    {
        public frmSatis()
        {
            InitializeComponent();
        }


        private void button5_Click(object sender, EventArgs e)
        {
            frmMüşteriEkle ekle=new frmMüşteriEkle();
            ekle.ShowDialog();
                
        }

        private void button6_Click(object sender, EventArgs e)
        {
            frmMüşteriListele listele=new frmMüşteriListele();
            listele.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            frmÜrünEkle ekle = new frmÜrünEkle();
            ekle.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmKategori kategori=new frmKategori();
            kategori.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmMarka marka =new frmMarka();
            marka.ShowDialog();
        }
    }
}
