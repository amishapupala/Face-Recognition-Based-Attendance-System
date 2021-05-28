using System;
using System.Configuration;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Net.Mail;


namespace MultiFaceRec
{
    public partial class frmReport : Form
    {
        String strConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
        
        public frmReport()
        {
            InitializeComponent();
        }

        private void frmReport_Load(object sender, EventArgs e)
        {
            dtpDate.Value = DateTime.Today.Date.Date;
            GenerateReport();
        }
        private void GenerateReport()
        {
            string strCommandText;
            strCommandText = "Select [StudentName], 'Present' as [Status] From [Attendance] Where [AttendanceDate] = @dtpDate Union Select [StudentName], 'Absent' as [Status] From [Student] where Studentname not in(select [Studentname] from [Attendance] where AttendanceDate = @dtpDate Order by [Studentname]) ";
            System.Data.OleDb.OleDbDataAdapter objOleDbDataAdapter = new System.Data.OleDb.OleDbDataAdapter();
            System.Data.DataTable objDataTable = new System.Data.DataTable();
            try
            {
                objOleDbDataAdapter.SelectCommand = new System.Data.OleDb.OleDbCommand();
                objOleDbDataAdapter.SelectCommand.Connection = new System.Data.OleDb.OleDbConnection(strConnectionString);
                objOleDbDataAdapter.SelectCommand.CommandText = strCommandText;
                objOleDbDataAdapter.SelectCommand.Parameters.AddWithValue("@dtpDate",  dtpDate.Value);
                objOleDbDataAdapter.SelectCommand.CommandType = System.Data.CommandType.Text;
                objOleDbDataAdapter.Fill(objDataTable);
                if (objDataTable.Rows.Count == 0)
                {
                    MessageBox.Show("No Record Found");
                    return;
                }                
                dataGridView1.DataMember = objDataTable.TableName;
                //dataGridView1.AutoGenerateColumns = true;
                dataGridView1.DataSource = objDataTable;
                dataGridView1.Refresh();               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            GenerateReport();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int Rcount = dataGridView1.RowCount;
            int i = 0;
                      
            for (i = 0; i < Rcount; i++)
            {
                string attendance = dataGridView1.Rows[i].Cells["status"].Value.ToString();

                if (attendance == "Absent")
                {
                    i = i + 1;
                    string strCommandText;
                    strCommandText = "select * from student where id="+ i +"";
                    System.Data.OleDb.OleDbDataAdapter objOleDbDataAdapter = new System.Data.OleDb.OleDbDataAdapter();
                    System.Data.DataTable objDataTable = new System.Data.DataTable();
                    try
                    {
                        objOleDbDataAdapter.SelectCommand = new System.Data.OleDb.OleDbCommand();
                        objOleDbDataAdapter.SelectCommand.Connection = new System.Data.OleDb.OleDbConnection(strConnectionString);
                        objOleDbDataAdapter.SelectCommand.CommandText = strCommandText;
                        //objOleDbDataAdapter.SelectCommand.Parameters.AddWithValue("@dtpDate", dtpDate.Value);
                        objOleDbDataAdapter.SelectCommand.CommandType = System.Data.CommandType.Text;
                        objOleDbDataAdapter.Fill(objDataTable);
                        if (objDataTable.Rows.Count == 0)
                        {
                            MessageBox.Show("No Record Found");
                            return;
                        }
                        string email = objDataTable.Rows[0]["email_id"].ToString();

                        //---------mail-------------------------------------

                        //string result = "mail Sent Successfully..!!";
                        string senderID = "collegecommunityportal@gmail.com";// use sender's email id here..
                        string subject = "Attendance";
                        string body = "your child has absent that date= " + DateTime.Now;
                        const string senderPassword = "college*#"; // sender password here...
                        try
                        {
                            SmtpClient smtp = new SmtpClient
                            {
                                Host = "smtp.gmail.com", // smtp server address here...
                                Port = 587,
                                EnableSsl = true,
                                DeliveryMethod = SmtpDeliveryMethod.Network,
                                Credentials = new System.Net.NetworkCredential(senderID, senderPassword),
                                Timeout = 30000,
                            };
                            MailMessage message = new MailMessage(senderID, email, subject, body);
                            smtp.Send(message);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }//return result;

                        //-----------------------------------------------------------
                                                                   
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }
    }
}
