using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SepetGiris
{
    public partial class SepetGiris : Form
    {
        //public string BASE_URL = "http://localhost:52210/api/";
        public string BASE_URL = "";
        public string subeNo = "";
        int bekleyen = 0;
        int hatali =0;
        int basarili = 0;
        string aktarilanSepetler = "";
        public SepetGiris()
        {
            InitializeComponent();
        }

        private void SepetGiris_Load(object sender, EventArgs e)
        {
            this.Text = "ServisUrl = " + BASE_URL;
            prgbar.Visible = true;
            var values = new System.Collections.Specialized.NameValueCollection();
            values["DuzMetin"] = "";
            JObject donen = JObject.Parse(Post(values, "fxpSubeNo"));
            subeNo = donen["subeNo"].ToString();
            lblSubeNo.Text = "Şube No : " + subeNo; 
            prgbar.Visible = false;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && textBox1.Text.Length>3)
            {
                bekleyen += 1;
                lblBekleyen.Text = bekleyen.ToString();
                if (textBox1.Text.Length==9 && textBox1.Text.ToUpper().Contains("S") && !textBox1.Text.Substring(3, 2).Equals(subeNo))
                {
                    textBox1.Clear();
                    textBox1.Focus();
                    bekleyen -= 1;
                    hatali += 1;
                    lblHatali.Text = hatali.ToString();
                    lblBekleyen.Text = bekleyen.ToString();
                    throw new Exception("Hatalı Şube.. sepet bu şubeye ait değil");
                    return;
                }
                prgbar.Visible = true;
                var values = new System.Collections.Specialized.NameValueCollection();
                values["DuzMetin"] = "{'sepetno':'" + textBox1.Text.Trim().Substring(textBox1.Text.Length - 4, 4) + "'}";

                Object[] _obj = new Object[2];
                _obj[0] = values;
                _obj[1] = "ReyonGiris";
                System.Threading.Thread t = new System.Threading.Thread(Postasenkron);
                t.IsBackground = true;
                t.Start(_obj);
                textBox1.Clear();
                textBox1.Focus();
            }
            textBox1.Focus();
        }

        private void Postasenkron(Object _params)
        {
            string donen = Post((System.Collections.Specialized.NameValueCollection)((object[])_params)[0], (string)((object[])_params)[1]);
            if (!donen.Equals(""))
            {
                if (donen.ToUpper().Contains("HATA"))
                {
                    hatali += 1;
                    bekleyen -= 1;
                    Invoke((MethodInvoker)delegate {
                        lblHatali.Text = hatali.ToString();
                        lblBekleyen.Text = bekleyen.ToString();
                    });
                    return;
                }
                bekleyen -= 1;
                basarili += 1;
                JObject obj = JObject.Parse(donen.TrimStart('\"').TrimEnd('\"'));
                Invoke((MethodInvoker)delegate {
                    richTextBox1.Text = richTextBox1.Text + " " + obj["sepetno"].ToString().Trim();
                    lblOkunan.Text = basarili.ToString();
                    lblBekleyen.Text = bekleyen.ToString();
                });
                
            }

        }

        private string Post(System.Collections.Specialized.NameValueCollection values, string url)
        {
            //POST
            using (var client = new System.Net.WebClient())
            {
                var response = client.UploadValues(BASE_URL+url, values);

                var responseString = Encoding.Default.GetString(response);
                return responseString;
            }

            return "";

            //GET
            //using (var client = new WebClient())
            //{
            //    var responseString = client.DownloadString("http://www.example.com/recepticle.aspx");
            //}
        }

        private string JsonPost(string json,string url)
        {
            var request = System.Net.WebRequest.Create(BASE_URL + url);
            request.Method = "POST";

            byte[] byteArray = Encoding.UTF8.GetBytes(json);

            using (var reqStream = request.GetRequestStream())
            {
                reqStream.Write(byteArray, 0, byteArray.Length);
            }

            using (var response = request.GetResponse())
            {
                string durum = ((System.Net.HttpWebResponse)response).StatusDescription;

                using (var respStream = response.GetResponseStream())
                {
                    using (var reader = new System.IO.StreamReader(respStream))
                    {
                        string data = reader.ReadToEnd();
                        throw new Exception(data);
                    }
                }
            }

            return "json";
        }

       
    }
}
