using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Windows.Forms.DataVisualization.Charting;

namespace ManagerSFCS
{
    public partial class View_Report : UserControl
    { SqlConnection cnn;
        public View_Report()
        {
            InitializeComponent();
            cnn = new SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Admin\Desktop\SFCS\SFCS\SFCS.mdf; Integrated Security = True");
            chart1.Hide();
            chart2.Hide();
        }

        private void bxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void BxField_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        public int slMonth { set; get; }
        public int countrow()
        {
            string stmt = "SELECT COUNT(*) FROM Salestbl";
            int count = 0;
            cnn.Open();
            SqlCommand cmdCount = new SqlCommand(stmt, cnn);
            count = (int)cmdCount.ExecuteScalar();
            cnn.Close();
           
            return count;
        }

        SaleList[] list;
        private void getData()
        {
            int n = countrow();
            list = new SaleList[n];

          
            string totalstring = "";
            string vname = "";
            string monthstr = "";
            string daystr = "";
            string sql = "select * from Salestbl";
            cnn.Open();
            SqlCommand cmd = new SqlCommand(sql, cnn);
            SqlDataReader dr = cmd.ExecuteReader();
            int i = 0;
            while (dr.Read())
            {
                totalstring = dr["Totalprice"].ToString();
                vname = dr["vendor"].ToString();
                
                monthstr = dr["Month"].ToString();
                
                daystr = dr["Day"].ToString();
                list[i] = new SaleList();
                list[i].Total = Convert.ToInt32(totalstring.Trim()) ;
                list[i].Vendor =vname ;
                list[i].Month = Convert.ToInt32(monthstr.Trim());
                list[i].Day = Convert.ToInt32(daystr.Trim());
               
                i++;
                
            }
            cnn.Close();

        }
        private void Processmonth(string vendor)
        {
            int l = 0;
            Int32[] ttMonth=new Int32[12];
            int n = countrow();
            for (int i=0;i<n;i++)
            {   if (list[i].Vendor.Trim() == vendor)
                {
                    l++; for(int k=1;k<13;k++)
                    if (list[i].Month == k) ttMonth[k-1] += list[i].Total;

                }
            }

           
            cnn.Open();
            for (int i = 0; i < 12; i++)
            {
                SqlCommand cmd2 = new SqlCommand("Update MonthRevenue set Revenue = @rv where Month = @month", cnn);
                cmd2.Parameters.AddWithValue("@rv", ttMonth[i]);
                cmd2.Parameters.AddWithValue("@month", i+1);
                cmd2.ExecuteNonQuery();
            }
            
            cnn.Close();

        }
       
        public void barchart()
        {
            string sql = "select * from MonthRevenue";


            cnn.Open();
            DataTable dt = new DataTable();
            SqlDataAdapter cmd = new SqlDataAdapter(sql, cnn);
            cmd.Fill(dt);
            chart1.DataSource = dt;
            chart1.ChartAreas["ChartArea1"].AxisX.Title = "Months";
            chart1.ChartAreas["ChartArea1"].AxisY.Title = "Revenue";
            chart1.Series["Month Revenue"].XValueMember = "Month";
            chart1.Series["Month Revenue"].YValueMembers = "Revenue";
            /*SqlDataReader dr = cmd.ExecuteReader();
            Series sr = new Series();
           while(dr.Read())
            {
                this.chart1.Series[0].Points.AddY(dr.GetInt32(0));
            }*/
            cnn.Close();
        }
        public void piechart()
        {
            string sql = "select * from MonthRevenue";


            cnn.Open();
            DataTable dt = new DataTable();
            SqlDataAdapter cmd = new SqlDataAdapter(sql, cnn);
            cmd.Fill(dt);
            chart2.DataSource = dt;
            chart2.ChartAreas["ChartArea1"].AxisX.Title = "Months";
            chart2.ChartAreas["ChartArea1"].AxisY.Title = "Revenue";
            chart2.Series["Month Revenue"].XValueMember = "Month";
            chart2.Series["Month Revenue"].YValueMembers = "Revenue";
            /*SqlDataReader dr = cmd.ExecuteReader();
            Series sr = new Series();
           while(dr.Read())
            {
                this.chart1.Series[0].Points.AddY(dr.GetInt32(0));
            }*/
            cnn.Close();
        }
        public void refreshdata()
        {
            getData();
            Processmonth("Com tam Ngo Quyen");
        }
        private void BtnView_Click(object sender, EventArgs e)
        {
            

            if (bxType.Text == "Pie Chart") {  piechart();chart1.Hide(); chart2.Show(); }

            if (bxType.Text == "Bar Chart") { barchart();chart2.Hide(); chart1.Show(); }
        }
    }
}
