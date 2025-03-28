using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

namespace ToolredMZ
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnBuscarClicked(object sender, EventArgs e)
        {
            string input = txtDato.Text?.Trim();
            string tipo = pickerTipo.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(tipo))
            {
                DisplayAlert("Error", "Ingrese un dato y seleccione un tipo.", "OK");
                return;
            }

            string ip = "-", mac = "-", host = "-";

            if (tipo == "IP")
            {
                ip = input;
                host = GetHostnameFromIP(input);
                mac = GetMacFromIP(input);
            }
            else if (tipo == "MAC Address")
            {
                mac = input;
                ip = GetIpFromMac(input);
                if (!string.IsNullOrEmpty(ip))
                    host = GetHostnameFromIP(ip);
            }
            else if (tipo == "Hostname")
            {
                host = input;
                ip = GetIPFromHostname(input);
                if (!string.IsNullOrEmpty(ip))
                    mac = GetMacFromIP(ip);
            }

            // Mostrar los resultados en la Card
            lblIP.Text = $"IP: {ip}";
            lblMAC.Text = $"MAC: {mac}";
            lblHost.Text = $"Hostname: {host}";
        }

        private string GetHostnameFromIP(string ipAddress)
        {
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(ipAddress);
                return hostEntry.HostName;
            }
            catch { return "No encontrado"; }
        }

        private string GetIPFromHostname(string hostname)
        {
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(hostname);
                return hostEntry.AddressList.Length > 0 ? hostEntry.AddressList[0].ToString() : "No encontrado";
            }
            catch { return "No encontrado"; }
        }

        private string GetMacFromIP(string ipAddress)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = "arp";
                p.StartInfo.Arguments = "-a";
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();

                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();

                Match match = Regex.Match(output, $@"({ipAddress})\s+([\w-]+)");
                return match.Success ? match.Groups[2].Value : "No encontrado";
            }
            catch { return "No encontrado"; }
        }

        private string GetIpFromMac(string macAddress)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = "arp";
                p.StartInfo.Arguments = "-a";
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();

                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();

                Match match = Regex.Match(output, @"(\d+\.\d+\.\d+\.\d+)\s+" + macAddress.Replace("-", ":"));
                return match.Success ? match.Groups[1].Value : "No encontrado";
            }
            catch { return "No encontrado"; }
        }
    }
}