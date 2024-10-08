﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
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

        SqlConnection baglanti = new SqlConnection("Data Source=EKARAKURT1;Initial Catalog=Stok_Takip;Integrated Security=True;");
        DataSet daset = new DataSet();

        private void sepetlistele()
        {
            baglanti.Open();
            SqlDataAdapter adtr = new SqlDataAdapter("select * from sepet", baglanti);
            adtr.Fill(daset, "sepet");
            dataGridView1.DataSource = daset.Tables["sepet"];
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.Columns[2].Visible = false;
            baglanti.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            frmMüşteriEkle ekle = new frmMüşteriEkle();
            ekle.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            frmMüşteriListele listele = new frmMüşteriListele();
            listele.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            frmÜrünEkle ekle = new frmÜrünEkle();
            ekle.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmKategori kategori = new frmKategori();
            kategori.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmMarka marka = new frmMarka();
            marka.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            frmÜrünListele listele = new frmÜrünListele();
            listele.ShowDialog();
        }
        private void hesapla()
        {
            decimal genelToplam = 0;

            // Verilerin bulunduğu DataGridView kontrolü (örneğin, "dataGridView1")
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["toplamfiyat"] != null && row.Cells["toplamfiyat"].Value != null)
                {
                    // Toplam fiyat hücresindeki değeri al ve decimal türüne dönüştür
                    decimal toplamFiyat = Convert.ToDecimal(row.Cells["toplamfiyat"].Value);
                    genelToplam += toplamFiyat; // Genel toplamı güncelle
                }
            }

            // Genel toplamı label'a yazdır
            lblGenelToplam.Text = genelToplam.ToString("C2"); // "C2" formatı, para birimi formatında gösterir
        }

        private void frmSatis_Load(object sender, EventArgs e)
        {
            sepetlistele();
        }

        private void txtTc_TextChanged(object sender, EventArgs e)
        {
            if (txtTc.Text == "")
            {
                txtAdSoyad.Text = "";
                txtTelefon.Text = "";
            }
            else
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("select * from müşteri where tc like @tc", baglanti);
                komut.Parameters.AddWithValue("@tc", txtTc.Text);
                SqlDataReader read = komut.ExecuteReader();

                if (read.Read()) // Kayıt varsa oku
                {
                    txtAdSoyad.Text = read["adsoyad"].ToString();
                    txtTelefon.Text = read["telefon"].ToString();
                }
                baglanti.Close();
            }
        }

        private void txtBarkodNo_TextChanged(object sender, EventArgs e)
        {
            Temizle();
            try
            {
                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }

                SqlCommand komut = new SqlCommand("select * from urun where barkodno like @barkodno", baglanti);
                komut.Parameters.AddWithValue("@barkodno", txtBarkodNo.Text);
                SqlDataReader read = komut.ExecuteReader();

                if (read.Read())
                {
                    txtUrunAdi.Text = read["urunadi"].ToString();
                    txtSatisFiyati.Text = read["satisfiyati"].ToString();
                }
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close(); // Her zaman bağlantıyı kapat
                }
            }
        }

        private void Temizle()
        {
            if (txtBarkodNo.Text == "")
            {
                foreach (Control item in groupBox2.Controls)
                {
                    if (item is TextBox && item != txtMiktar)
                    {
                        item.Text = "";
                    }
                }
            }
        }

        bool durum;
        private void barkodkontrol()
        {
            durum = true;
            baglanti.Open();
            SqlCommand komut = new SqlCommand("select * from sepet", baglanti);
            SqlDataReader read = komut.ExecuteReader();
            while (read.Read())
            {
                if (txtBarkodNo.Text == read["barkodno"].ToString())
                {
                    durum = false;
                    break; // Barkod bulunduğunda döngüden çık
                }
            }
            baglanti.Close();
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            barkodkontrol();
            baglanti.Open();
            if (durum) // Yeni ürün ekleniyor
            {
                SqlCommand komut = new SqlCommand("insert into sepet(tc,adsoyad,telefon,barkodno,urunadi,miktari,satisfiyati,toplamfiyat,tarih) values(@tc,@adsoyad,@telefon,@barkodno,@urunadi,@miktari,@satisfiyati,@toplamfiyat,@tarih)", baglanti);
                komut.Parameters.AddWithValue("@tc", txtTc.Text);
                komut.Parameters.AddWithValue("@adsoyad", txtAdSoyad.Text);
                komut.Parameters.AddWithValue("@telefon", txtTelefon.Text);
                komut.Parameters.AddWithValue("@barkodno", txtBarkodNo.Text);
                komut.Parameters.AddWithValue("@urunadi", txtUrunAdi.Text);
                komut.Parameters.AddWithValue("@miktari", int.Parse(txtMiktar.Text));
                komut.Parameters.AddWithValue("@satisfiyati", double.Parse(txtSatisFiyati.Text));
                komut.Parameters.AddWithValue("@toplamfiyat", double.Parse(txtToplamFiyat.Text));
                komut.Parameters.AddWithValue("@tarih", DateTime.Now);
                komut.ExecuteNonQuery();

            }
            else // Var olan ürüne miktar ekleniyor
            {

                SqlCommand komut2 = new SqlCommand("update sepet set miktari = miktari + @miktari where barkodno = @barkodno", baglanti);
                komut2.Parameters.AddWithValue("@miktari", int.Parse(txtMiktar.Text));
                komut2.Parameters.AddWithValue("@barkodno", txtBarkodNo.Text);
                komut2.ExecuteNonQuery();

                SqlCommand komut3 = new SqlCommand("update sepet set toplamfiyat = miktari * satisfiyati where barkodno = @barkodno", baglanti);
                komut3.Parameters.AddWithValue("@barkodno", txtBarkodNo.Text);
                komut3.ExecuteNonQuery();
            }
            baglanti.Close();
            txtMiktar.Text = "1"; // Varsayılan miktar
            daset.Tables["sepet"].Clear();
            sepetlistele();
            hesapla();


            // Girdileri temizle
            foreach (Control item in groupBox2.Controls)
            {
                if (item is TextBox && item != txtMiktar)
                {
                    item.Text = "";
                }
            }
        }

        private void txtMiktar_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtMiktar.Text) && !string.IsNullOrEmpty(txtSatisFiyati.Text))
                {
                    txtToplamFiyat.Text = (double.Parse(txtMiktar.Text) * double.Parse(txtSatisFiyati.Text)).ToString();
                }
            }
            catch
            {
                txtToplamFiyat.Text = "0"; // Hata durumunda toplam fiyatı sıfırla
            }
        }

        private void txtSatisFiyati_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtMiktar.Text) && !string.IsNullOrEmpty(txtSatisFiyati.Text))
                {
                    txtToplamFiyat.Text = (double.Parse(txtMiktar.Text) * double.Parse(txtSatisFiyati.Text)).ToString();
                }
            }
            catch
            {
                txtToplamFiyat.Text = "0"; // Hata durumunda toplam fiyatı sıfırla
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("delete from sepet where barkodno='" + dataGridView1.CurrentRow.Cells["barkodno"].Value.ToString() + "'", baglanti);
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Ürün Sepetten Çıkarıldı");
            daset.Tables["sepet"].Clear();
            sepetlistele();
            hesapla();

        }

        private void btnSatisİptal_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("delete from sepet", baglanti);
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Ürünler Sepetten Çıkarıldı");
            daset.Tables["sepet"].Clear();
            sepetlistele();
            hesapla();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            frmSatışListele listele = new frmSatışListele();
            listele.ShowDialog();
        }

        private void btnSatisYap_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                baglanti.Open();
                if (durum) // Yeni ürün ekleniyor
                {
                    SqlCommand komut = new SqlCommand("insert into satis(tc,adsoyad,telefon,barkodno,urunadi,miktari,satisfiyati,toplamfiyat,tarih) values(@tc,@adsoyad,@telefon,@barkodno,@urunadi,@miktari,@satisfiyati,@toplamfiyat,@tarih)", baglanti);
                    komut.Parameters.AddWithValue("@tc", txtTc.Text);
                    komut.Parameters.AddWithValue("@adsoyad", txtAdSoyad.Text);
                    komut.Parameters.AddWithValue("@telefon", txtTelefon.Text);
                    komut.Parameters.AddWithValue("@barkodno", dataGridView1.Rows[i].Cells["barkodno"].Value.ToString());
                    komut.Parameters.AddWithValue("@urunadi", dataGridView1.Rows[i].Cells["urunadi"].Value.ToString());
                    komut.Parameters.AddWithValue("@miktari", int.Parse(dataGridView1.Rows[i].Cells["miktari"].Value.ToString()));
                    komut.Parameters.AddWithValue("@satisfiyati", double.Parse(dataGridView1.Rows[i].Cells["satisfiyati"].Value.ToString()));
                    komut.Parameters.AddWithValue("@toplamfiyat", double.Parse(dataGridView1.Rows[i].Cells["toplamfiyat"].Value.ToString()));
                    komut.Parameters.AddWithValue("@tarih", DateTime.Now);
                    komut.ExecuteNonQuery();
                    SqlCommand komut2 = new SqlCommand("update urun set miktari=miktari-'" + int.Parse(dataGridView1.Rows[i].Cells["miktari"].Value.ToString()) + "'where barkodno='" + dataGridView1.Rows[i].Cells["barkodno"].Value.ToString() + "'", baglanti);
                    komut2.ExecuteNonQuery();
                    baglanti.Close();

                }
                try
                {
                    if (baglanti.State != ConnectionState.Open)
                    {
                        baglanti.Open();
                    }

                    SqlCommand komut3 = new SqlCommand("DELETE FROM sepet", baglanti);
                    komut3.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    // Hata ile ilgili işlemler
                    Console.WriteLine("Bir hata oluştu: " + ex.Message);
                }
                finally
                {
                    if (baglanti.State == ConnectionState.Open)
                    {
                        baglanti.Close();
                    }
                }

                daset.Tables["sepet"].Clear();
                sepetlistele();
                hesapla();

            }
        }
    }
}
