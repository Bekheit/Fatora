using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fatora
{
    public partial class invoiceForm : Form
    {
        public invoiceForm()
        {
            InitializeComponent();
        }

        private void invoiceForm_Load(object sender, EventArgs e)
        {
            txtDate.Text = DateTime.Now.ToString("MM/dd/yyyy");

            Dictionary<String, int> itemsData = new Dictionary<string, int>();
            itemsData.Add("لاب توب", 1000);
            itemsData.Add("شاشة", 2000);
            itemsData.Add("ماوس", 3000);
            itemsData.Add("كيبورد", 4000);
            itemsData.Add("بادا", 5000);
            itemsData.Add("سماعات", 6000);

            cbxItems.DataSource = new BindingSource(itemsData, null);
            cbxItems.DisplayMember = "key";
            cbxItems.ValueMember = "value";
            txtPrice.Text = cbxItems.SelectedValue.ToString();

            txtName.Select();
            //txtName.Focus();
            //txtName.SelectAll();
        }

        private void txtDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        

        private void txtTotal_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cbxItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPrice.Text = cbxItems.SelectedValue.ToString();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cbxItems.SelectedIndex < 0) return;

            string item = cbxItems.Text;
            int qty = Convert.ToInt32(txtQty.Text);
            int price = Convert.ToInt32(txtPrice.Text);
            int subTotal = qty * price;

            for (int i = 0; i < dgvInvoice.Rows.Count; i++)
            {
                if (dgvInvoice.Rows[i].Cells[0].Value.ToString() == item) return;
            }

            object[] row = { item, price, qty, subTotal };

            dgvInvoice.Rows.Add(row);

            txtTotal.Text = (Convert.ToInt32(txtTotal.Text) + subTotal).ToString();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow item in dgvInvoice.SelectedRows)
            {
                int price = Convert.ToInt32(item.Cells[1].Value.ToString());
                int qty = Convert.ToInt32(item.Cells[2].Value.ToString());
                txtTotal.Text = (Convert.ToInt32(txtTotal.Text) - price * qty).ToString();
                dgvInvoice.Rows.RemoveAt(item.Index);
            }

            
        }

        private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) e.Handled = true;
        }

        string oldQty = "1";
        private void dgvInvoice_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dgvInvoice.CurrentRow != null)
                oldQty = dgvInvoice.CurrentRow.Cells["colQty"].Value.ToString();
        }

        private void dgvInvoice_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvInvoice.CurrentRow != null)
            {
                string newQty = dgvInvoice.CurrentRow.Cells["colQty"].Value.ToString();

                if (oldQty == newQty) return;

                if (!Regex.IsMatch(newQty, "^\\d+$"))
                {
                    dgvInvoice.CurrentRow.Cells["colQty"].Value = oldQty;
                }
                else
                {
                    int price = Convert.ToInt32(dgvInvoice.CurrentRow.Cells["colPrice"].Value);
                    int q = Convert.ToInt32(newQty);

                    int diff = (q - Convert.ToInt32(oldQty)) * price;

                    txtTotal.Text = (Convert.ToInt32(txtTotal.Text) + diff).ToString();

                    dgvInvoice.CurrentRow.Cells["colSubTotal"].Value = (price * q).ToString();
                }
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            ((Form)printPreviewDialog1).WindowState = FormWindowState.Maximized;
            if (printPreviewDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
            }
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            float margin = 20;
            Font fontBold = new Font("Arial", 14, FontStyle.Bold);
            Font font = new Font("Arial", 14);
            Font tahrerFont = new Font("Arial", 14, FontStyle.Bold | FontStyle.Underline);

            string companyName = "شركة ديوان لادارة الاستثمار العقاري";
            string companyLocation = "مصر - الجيزة - 24 شارع حسن القاضي الهرم";
            string companyPhone = "هاتف: 01023189955";
            string invoiceNumTxt = ": رقم الفاتورة";
            string clientName = txtName.Text;
            string clientLocation = txtAddress.Text;
            string clientPhone = txtPhone.Text;
            string tahrer = ": حررت الفاتورة ل";
            string nameTxt = ": اسم العميل";
            string phoneTxt = ": هاتف العميل";
            string locationTxt = ": عنوان العميل";
            string dateTxt = ": تاريخ الفاتورة";
            string dueDateTxt = ": تاريخ الاستحقاق";
            string priceTxt = ": المبلغ المستحق";
            string subTotalTxt = ": المجموع الجزئي";
            string invoiceDate = txtDate.Text;
            string dueDate = (Convert.ToDateTime(invoiceDate).AddMonths(1)).ToString("MM/dd/yyyy").Split(' ')[0] ;
            string invoicePrice = txtTotal.Text;
            string taxTxt = ": ضريبة القيمة المضافة";
            string totalPriceTxt = ": الاجمالي";

            SizeF invoiceNumTxtSize = e.Graphics.MeasureString(invoiceNumTxt, fontBold);
            SizeF companyNameSize = e.Graphics.MeasureString(companyName, fontBold);
            SizeF companyLocationSize = e.Graphics.MeasureString(companyLocation, font);
            SizeF companyPhoneSize = e.Graphics.MeasureString(companyPhone, font);
            SizeF clientNameSize = e.Graphics.MeasureString(clientName, font);
            SizeF clientLocationSize = e.Graphics.MeasureString(clientLocation, font);
            SizeF clientPhoneSize = e.Graphics.MeasureString(clientPhone, font);
            SizeF tahrerSize = e.Graphics.MeasureString(tahrer, tahrerFont);
            SizeF nameTxtSize = e.Graphics.MeasureString(nameTxt, fontBold);
            SizeF phoneTxtSize = e.Graphics.MeasureString(phoneTxt, fontBold);
            SizeF locationTxtSize = e.Graphics.MeasureString(locationTxt, fontBold);
            SizeF dateTxtSize = e.Graphics.MeasureString(dateTxt, fontBold);
            SizeF priceTxtSize = e.Graphics.MeasureString(priceTxt, fontBold);
            SizeF invoiceDateSize = e.Graphics.MeasureString(invoiceDate, font);
            SizeF invoicePriceSize = e.Graphics.MeasureString(invoicePrice, font);
            SizeF subTotalTxtSize = e.Graphics.MeasureString(subTotalTxt, fontBold);
            SizeF taxTxtSize = e.Graphics.MeasureString(taxTxt, fontBold);
            SizeF totalPriceTxtSize = e.Graphics.MeasureString(totalPriceTxt, fontBold);
            SizeF invoiceNumSize = e.Graphics.MeasureString(txtInvNum.Text, fontBold);
            SizeF dueDateTxtSize = e.Graphics.MeasureString(dueDateTxt, fontBold);
            SizeF dueDateSize = e.Graphics.MeasureString(dueDate.ToString(), fontBold);

            int width = e.PageBounds.Width;
            int height = e.PageBounds.Height;

            double tax = Convert.ToDouble(invoicePrice) * .14;
            tax = Math.Round(tax, 2);

            double totalPrice = Convert.ToDouble(invoicePrice) + tax;
            totalPrice = Math.Round(totalPrice, 2);

            e.Graphics.DrawImage(Properties.Resources.DewanLogo, 390 , margin, 450, 80);
            //e.Graphics.DrawString(invoiceNumTxt, fontBold,  Brushes.Red, 20, margin);
            e.Graphics.DrawString(companyName, fontBold, Brushes.Black, (width - companyNameSize.Width - margin + 10), margin + 100);
            e.Graphics.DrawString(companyLocation, font, Brushes.Black, (width - companyLocationSize.Width - margin + 5), margin + 100 + companyNameSize.Height);
            e.Graphics.DrawString(companyPhone, font, Brushes.Black, (width - companyPhoneSize.Width - margin + 5), margin + 100 + companyNameSize.Height + companyLocationSize.Height);

            float gapHeight = companyNameSize.Height + companyLocationSize.Height + companyPhoneSize.Height + margin + 150;
            e.Graphics.DrawRectangle(Pens.Black, margin, gapHeight, width / 3, 130);
            e.Graphics.DrawRectangle(Pens.Black, margin + width/3, gapHeight, (width * 2) / 3 - margin * 2, 130);
            
            e.Graphics.DrawString(tahrer, tahrerFont, Brushes.Black, width - tahrerSize.Width - margin - 5, gapHeight + 10);

            e.Graphics.DrawString(nameTxt, fontBold, Brushes.Black, width - nameTxtSize.Width - margin - 5, gapHeight + 10 + tahrerSize.Height);
            e.Graphics.DrawString(clientName, font, Brushes.Black, width - clientNameSize.Width - nameTxtSize.Width - margin - 5, gapHeight + 10 + tahrerSize.Height);

            e.Graphics.DrawString(locationTxt, fontBold, Brushes.Black, width - locationTxtSize.Width - margin - 5, gapHeight + 10 + tahrerSize.Height + nameTxtSize.Height);
            e.Graphics.DrawString(clientLocation, font, Brushes.Black, width - clientLocationSize.Width - locationTxtSize.Width - margin - 5, gapHeight + 10 + tahrerSize.Height + nameTxtSize.Height);

            e.Graphics.DrawString(phoneTxt, fontBold, Brushes.Black, width - phoneTxtSize.Width - margin - 5, gapHeight + 10 + tahrerSize.Height + nameTxtSize.Height + phoneTxtSize.Height);
            e.Graphics.DrawString(clientPhone, font, Brushes.Black, width - clientPhoneSize.Width - phoneTxtSize.Width - margin - 5, gapHeight + 10 + tahrerSize.Height + nameTxtSize.Height + locationTxtSize.Height);




            e.Graphics.DrawString(invoiceNumTxt, fontBold, Brushes.Black, width / 3 - invoiceNumTxtSize.Width + margin - 5, gapHeight + 10);
            e.Graphics.DrawString(txtInvNum.Text, font, Brushes.Black, width / 3 - invoiceNumSize.Width - invoiceNumTxtSize.Width + margin - 5, gapHeight + 10);

            e.Graphics.DrawString(dateTxt, fontBold, Brushes.Black, width / 3 - dateTxtSize.Width + margin - 5, gapHeight + invoiceNumTxtSize.Height + margin / 2);
            e.Graphics.DrawString(invoiceDate, font, Brushes.Black, width / 3 - dateTxtSize.Width - invoiceDateSize.Width + margin - 5, gapHeight + invoiceNumTxtSize.Height + margin / 2);

            e.Graphics.DrawString(dueDateTxt, fontBold, Brushes.Black, width / 3 - dueDateTxtSize.Width + margin - 5, gapHeight + invoiceNumTxtSize.Height + dateTxtSize.Height + margin / 2);
            e.Graphics.DrawString(dueDate.ToString(), font, Brushes.Black, width / 3 - dueDateSize.Width - dueDateTxtSize.Width + margin - 5, gapHeight + invoiceNumTxtSize.Height + dateTxtSize.Height + margin / 2);

            e.Graphics.DrawString(priceTxt, fontBold, Brushes.Black, width / 3 - priceTxtSize.Width + margin - 5, gapHeight + dateTxtSize.Height + 60);
            e.Graphics.DrawString(totalPrice.ToString(), font, Brushes.Black, width / 3 - priceTxtSize.Width - invoicePriceSize.Width + margin - 5, gapHeight + dateTxtSize.Height + 60);

            float gapHeight2 = gapHeight + tahrerSize.Height + nameTxtSize.Height + phoneTxtSize.Height + locationTxtSize.Height + 50;

            e.Graphics.DrawRectangle(Pens.Black, margin, gapHeight2, width - margin*2 , height - gapHeight2 - margin - 150);

            float colHeight = 40;

            float col1Width = 80;
            float col2Width = 350 + col1Width;
            float col3Width = 125 + col2Width;
            float col4Width = 125 + col3Width;
            float col5Width = 125 + col4Width;

            e.Graphics.DrawLine(Pens.Black, margin, gapHeight2 + colHeight, width - margin, gapHeight2 + colHeight);

            e.Graphics.DrawString("مسلسل", fontBold, Brushes.Black, width - col1Width - margin/2 + 10, gapHeight2 + margin/2);
            e.Graphics.DrawLine(Pens.Black, width - col1Width - margin + 10, gapHeight2, width - col1Width - margin + 10, height - margin - 150);

            e.Graphics.DrawString("الصنف", fontBold, Brushes.Black, width - col2Width - margin + (col2Width - col1Width)/2 , gapHeight2 + margin/2);
            e.Graphics.DrawLine(Pens.Black, width - col2Width - margin + 10, gapHeight2, width - col2Width - margin + 10, height - margin - 150);

            e.Graphics.DrawString("الكمية", fontBold, Brushes.Black, width - col3Width  - margin + (col3Width - col2Width)/2, gapHeight2 + margin/2);
            e.Graphics.DrawLine(Pens.Black, width - col3Width - margin + 10, gapHeight2, width - col3Width - margin + 10, height - margin - 150);

            e.Graphics.DrawString("السعر", fontBold, Brushes.Black, width - col4Width - margin + (col4Width - col3Width) / 2, gapHeight2 + margin/2);
            e.Graphics.DrawLine(Pens.Black, width - col4Width - margin + 10, gapHeight2, width - col4Width - margin + 10, height - margin - 150);
            e.Graphics.DrawString("المبلغ", fontBold, Brushes.Black, width - col5Width - margin*2 + (col5Width - col4Width) / 2, gapHeight2 + margin/2);


            float rowHeight = 50;
            for (int i=0; i<dgvInvoice.Rows.Count; i++)
            {
                e.Graphics.DrawString((i+1).ToString(), font, Brushes.Black, width - col1Width / 2 - margin, gapHeight2 + rowHeight);
                e.Graphics.DrawString(dgvInvoice.Rows[i].Cells[0].Value.ToString(), font, Brushes.Black, width - col2Width , gapHeight2 + rowHeight);
                e.Graphics.DrawString(dgvInvoice.Rows[i].Cells[1].Value.ToString(), font, Brushes.Black, width - col3Width + margin * 2, gapHeight2 + rowHeight);
                e.Graphics.DrawString(dgvInvoice.Rows[i].Cells[2].Value.ToString(), font, Brushes.Black, width - col4Width + margin * 2, gapHeight2 + rowHeight);
                e.Graphics.DrawString(dgvInvoice.Rows[i].Cells[3].Value.ToString(), font, Brushes.Black, width - col5Width + margin * 2, gapHeight2 + rowHeight);
                if (i != dgvInvoice.Rows.Count - 1)
                    e.Graphics.DrawLine(Pens.Black, margin, gapHeight2 + rowHeight + colHeight, width - margin, gapHeight2 + rowHeight + colHeight);
                rowHeight += 60;
            }

            float gapHeight3 = height - margin - 150 ;

            e.Graphics.DrawString(subTotalTxt, fontBold, Brushes.Black, margin * 4, gapHeight3 + margin /2 );
            e.Graphics.DrawString(invoicePrice, font, Brushes.Black, margin, gapHeight3 + margin / 2);
            e.Graphics.DrawLine(Pens.Black, margin, gapHeight3 + subTotalTxtSize.Height + margin /2 , 260, gapHeight3 + subTotalTxtSize.Height + margin / 2);

            

            e.Graphics.DrawString(taxTxt, fontBold, Brushes.Black, margin * 4, gapHeight3 + taxTxtSize.Height + margin);
            e.Graphics.DrawString(tax.ToString(), font, Brushes.Black, margin, gapHeight3 + taxTxtSize.Height + margin);
            e.Graphics.DrawLine(Pens.Black, margin, gapHeight3 + subTotalTxtSize.Height + taxTxtSize.Height + margin, 260, gapHeight3 + taxTxtSize.Height + subTotalTxtSize.Height + margin);

            

            e.Graphics.DrawString(totalPriceTxt, fontBold, Brushes.Black, margin * 4, gapHeight3 + taxTxtSize.Height + totalPriceTxtSize.Height + margin + 5);
            e.Graphics.DrawString(totalPrice.ToString(), font, Brushes.Black, margin, gapHeight3 + taxTxtSize.Height + totalPriceTxtSize.Height + margin + 5 );
            //e.Graphics.DrawLine(Pens.Black, margin, gapHeight3 + subTotalTxtSize.Height + taxTxtSize.Height + margin, 260, gapHeight3 + taxTxtSize.Height + subTotalTxtSize.Height + margin);
        }

        private void txtDate_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtQty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                for (int i = 0; i < dgvInvoice.Rows.Count; i++)
                {
                    if(dgvInvoice.Rows[i].Cells[0].Value.ToString() == cbxItems.Text)
                    {
                        int qty = Convert.ToInt32(txtQty.Text) - Convert.ToInt32(dgvInvoice.Rows[i].Cells[2].Value.ToString());
                        int price = Convert.ToInt32(dgvInvoice.Rows[i].Cells[1].Value.ToString());
                        dgvInvoice.Rows[i].Cells[2].Value = txtQty.Text;
                        dgvInvoice.Rows[i].Cells[3].Value = (Convert.ToInt32(txtQty.Text) * Convert.ToInt32(dgvInvoice.Rows[i].Cells[1].Value.ToString())).ToString();
                        txtTotal.Text = (Convert.ToInt32(txtTotal.Text) + qty * price).ToString();
                    }
                }
            }
        }
    }
}
