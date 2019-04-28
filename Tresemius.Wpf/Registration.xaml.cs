using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Security2.Rsa;
using Web.Models;

namespace Tresemius.Wpf
{
    /// <summary>
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        private HttpClient _httpClient;
        private RsaService _rsaService;
        public Registration()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(CommonEnvironment.HostServer)
            };

            _rsaService = new RsaService();
        }

        private async void RegistrationPost(object sender, RoutedEventArgs e)
        {
            var serverPublicKey = await GetPubKey();
            var model = new UserModel();

            model.Email = _rsaService.Encrypt(Email.Text, serverPublicKey);
            model.Password = _rsaService.Encrypt(Password.Text, serverPublicKey);

            var responseGuid = await _httpClient.PostAsJsonAsync("user/Register", model);
            responseGuid.EnsureSuccessStatusCode();
            MessageBox.Show($"Регистрация прошла успешно.");
            this.Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async Task<RsaPublicKey> GetPubKey()
        {
            var responsePubKey = await _httpClient.GetAsync("rsa/RsaPublicKey");
            responsePubKey.EnsureSuccessStatusCode();

            return await responsePubKey.Content.ReadAsAsync<RsaPublicKey>();
        }
    }
}
