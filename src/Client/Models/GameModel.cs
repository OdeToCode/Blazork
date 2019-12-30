using System.Net.Http;
using System.Threading.Tasks;

namespace Blazork.Client.Models
{
    public class GameModel
    {
        private readonly HttpClient httpClient;

        public GameModel(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task Load(string gameName)
        {

        }
    }
}
