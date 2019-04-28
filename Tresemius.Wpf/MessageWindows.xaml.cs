using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Cipher;
using Newtonsoft.Json;
using Web.Models;

namespace Tresemius.Wpf
{
    /// <summary>
    /// Interaction logic for MessageWindows.xaml
    /// </summary>
    public partial class MessageWindows : Window
    {
        private HttpClient _httpClient;
        private readonly StorageSession _storage;


        public MessageWindows()
        {
            InitializeComponent();
            //Иницилизация сервиса для шифрования
           
            //Иницилизация Http клиента
            _httpClient = new HttpClient(new HttpClientHandler { UseCookies = false })
            {
                BaseAddress = new Uri(CommonEnvironment.HostServer)
            };
            _storage = StorageSession.Create();
            var cookieName = StorageSession.SetCookieName;
            var cookie = _storage.Cookie.First();
            _httpClient.DefaultRequestHeaders.Add(cookieName, cookie);
            UpdateNewsList();
        }

        /// <summary>
        /// Метод для обновления новостей
        /// </summary>
        public async void UpdateNewsList()
        {

            var res = await _httpClient.GetAsync($"message");
            if (!res.IsSuccessStatusCode)
            {
                MessageBox.Show("Произогла ошибка.");
                return;
            }

            var content = await res.Content.ReadAsAsync<List<MessageInfo>>();

            EncryptList.Text = JsonConvert.SerializeObject(content);
            foreach (var item in content)
            {
                item.Date = TresCipher.Decrypt(item.Date, _storage.Key);
                item.Text = TresCipher.Decrypt(item.Text, _storage.Key);
            }

            var result = content.OrderByDescending(x =>DateTime.Parse(x.Date))
                .Select(
                    x => $"Текст:\t{x.Text}\nДата:\t{x.Date}\n----------------");
            MessageList.Text = string.Join("\n", result);
        }

        /// <summary>
        /// Метод для получения новостей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UpdateList(object sender, RoutedEventArgs e)
        {
            UpdateNewsList();
        }
        /// <summary>
        /// метод для создания новости
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddMessage(object sender, RoutedEventArgs e)
        {
            var model = new MessageInfo(Text.Text);
            _httpClient.DefaultRequestHeaders.Add(StorageSession.SetCookieName, _storage.Cookie);
            //MessageBox.Show($"Данные до шифрования:{JsonConvert.SerializeObject(model)}");
            model.Text = TresCipher.Encrypt(model.Text, _storage.Key);
            model.Date = TresCipher.Encrypt(model.Date, _storage.Key);
            //MessageBox.Show($"Данные после шифрования:{encryptStr}");
            EncryptCreateData.Text = JsonConvert.SerializeObject(model);
            var res = await _httpClient.PostAsJsonAsync($"message", model);
            res.EnsureSuccessStatusCode();
            UpdateNewsList();
        }
    }
}
