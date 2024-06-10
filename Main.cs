using System;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Diagnostics;

// IP Info by EnderDev
// github.com/enderfoxbg/ipinfo
// YouTube: @enderdevcom
// X (Twitter): @enderdevcom
namespace IPtracker
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            InitializeWebView2Async();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string ip = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(ip))
            {
                status.ForeColor = Color.Red;
                status.Text = "Please enter a valid IP address!";
                return;
            }

            try
            {
                Data ipinfo = await FetchIpInfo(ip);
                UpdateUI(ipinfo);
            }
            catch
            {
                status.ForeColor = Color.Red;
                status.Text = "Failed to fetch IP info. Check your internet connection.";
                return;
            }
        }

        private async Task<Data> FetchIpInfo(string ip)
        {
            string url = $"https://ipinfo.io/{ip}/json";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseData = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Data>(responseData);
            }
        }

        private void UpdateUI(Data ipinfo) // Display the IP info.
        {
            country.Text = $"Country: {ipinfo.country}";
            city.Text = $"City: {ipinfo.city}";
            coordinates.Text = $"Coordinates: {ipinfo.loc}";
            postal.Text = $"Postal: {ipinfo.postal}";
            region.Text = $"Region: {ipinfo.region}";
            asn.Text = $"ASN: {ipinfo.org}";

            status.ForeColor = Color.Green;
            status.Text = "Displaying IP Info!";
            label1.Visible = false;

            if (!string.IsNullOrEmpty(ipinfo.loc))
            {
                string[] coords = ipinfo.loc.Split(',');

                textBox2.Text = $"https://www.google.com/maps/?q={coords[0]},{coords[1]}";
                

                if (webView21.CoreWebView2 != null)
                {
                    webView21.CoreWebView2.Navigate(textBox2.Text);
                }
            }
        }

        private async void InitializeWebView2Async()
        {
            await webView21.EnsureCoreWebView2Async(null);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process.Start(textBox2.Text); // Open G Maps link in browser.
        }

        
    }

    public class Data // IP Data
    {
        public string city { get; set; }
        public string region { get; set; }
        public string country { get; set; }
        public string postal { get; set; }
        public string org { get; set; }
        public string loc { get; set; }
    }
}
