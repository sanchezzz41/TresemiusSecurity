using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Security2.Rsa;
using Web.Models;

namespace Tresemius.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HttpClient _httpClient;
        private RsaService _rsaService;
        public MainWindow()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(CommonEnvironment.HostServer)
            };

            _rsaService = new RsaService();
        }

        /// <summary>
        /// Авторизация 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AuthorizedClick(object sender, RoutedEventArgs e)
        {
            var loginModel = new UserModel();
            loginModel.Email = Email.Text;
            loginModel.Password = Password.Text;
            loginModel.Key = Key.Text;
            var pubKey = await GetPubKey();
            loginModel.Email = _rsaService.Encrypt(loginModel.Email, pubKey);
            loginModel.Password = _rsaService.Encrypt(loginModel.Password, pubKey);
            loginModel.Key = _rsaService.Encrypt(loginModel.Key, pubKey);

            var responseBool = await _httpClient.PostAsJsonAsync("user/login", loginModel);
            if (!responseBool.IsSuccessStatusCode)
            {
                var textError = await responseBool.Content.ReadAsStringAsync();
                MessageBox.Show($"Вы ввели неверные данные:\n{textError}");
                return;
            }

            var cookie = responseBool.Headers.GetValues(StorageSession.GetCookieName);
            StorageSession.Create();
            StorageSession.SetCookie(cookie);
            StorageSession.SetKey(Key.Text);
            var window = new MessageWindows();
            Close();
            window.ShowDialog();
        }

        /// <summary>
        /// Открытие окна для регисстрации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Registration(object sender, RoutedEventArgs e)
        {
            var model = new Registration();
            model.Show();
        }

        /// <summary>
        /// Получение публ ключа сервера
        /// </summary>
        /// <returns></returns>
        private async Task<RsaPublicKey> GetPubKey()
        {
            var responsePubKey = await _httpClient.GetAsync("rsa/RsaPublicKey");
            responsePubKey.EnsureSuccessStatusCode();

            return await responsePubKey.Content.ReadAsAsync<RsaPublicKey>();
        }
    }
}
