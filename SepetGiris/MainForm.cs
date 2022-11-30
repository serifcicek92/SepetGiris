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
    public partial class MainForm : Form
    {
        private string ipAdres = "";
        public MainForm()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ipAdres = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName())
                       .AddressList
                       .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                       .ToString();
            ipAdres = ipAdres.Substring(0,ipAdres.LastIndexOf('.')+1)+"8";
            txtIpAdres.Text = ipAdres;


        }

        private void btnGiris_Click(object sender, EventArgs e)
        {
            this.Hide();
            SepetGiris sepetGiris = new SepetGiris();
            sepetGiris.BASE_URL = "http://"+txtIpAdres.Text+ "/api/";
            sepetGiris.FormClosed += (s, args) => this.Close();
            sepetGiris.Show();
        }

        private static System.Reflection.Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            string dllName = string.Format("Newtonsoft.Json.dll", new System.Reflection.AssemblyName(args.Name).Name);
            var assem = System.Reflection.Assembly.GetExecutingAssembly();
            string resourceName = assem.GetManifestResourceNames().FirstOrDefault(rn => rn.EndsWith(dllName));
            if (resourceName == null)
            {
                return null;
            }
            using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    return null;
                }
                byte[] assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return System.Reflection.Assembly.Load(assemblyData);
            }
        }
    }
}
