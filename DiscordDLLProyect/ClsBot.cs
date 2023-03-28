using DSharpPlus;//Dlls start here
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Net;
using DSharpPlus.Lavalink;
using Newtonsoft.Json;// Dlls end here
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DiscordDLLProyect
{
    public class ClsBot
    {
        public static DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        public async Task RunAsync()
        {
            //Con estas instrucciones tomamos el json donde vienen las configuraciones y las metemos en la clase
            //ClsConfigJson
            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var configJson = JsonConvert.DeserializeObject<ClsConfigJson>(json);

            var config = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = configJson.Token,//Aqui usamos el json para recuperar el token
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            Client = new DiscordClient(config);

            Client.Ready += OnClientReady;

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix  },//Aqui usamos el json para recuperar los prefijos
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false
            };

            var endpoint = new ConnectionEndpoint
            {
                Hostname = configJson.Host, // Aqui usamos el json para recuperar el host
                Port = configJson.Port // y tambien el port
            };

            var lavalinkConfig = new LavalinkConfiguration
            {
                RestEndpoint= endpoint,
                SocketEndpoint= endpoint,
                Password= configJson.Password //usamos el json para recuperar la contraseña
            };

            //le asignamos a lavalink el cliente de discord
            var lavalink = Client.UseLavalink();

            //asignamos las configuraciones a la instancia de comandos y le registramos los comandos
            //de la clase ClsComandos
            Commands = Client.UseCommandsNext(commandsConfig);
            ClsComandos comandos = new ClsComandos();
            Commands.RegisterCommands<ClsComandos>();

            //Conectamos el cliente y el lavalink
            await Client.ConnectAsync();
            await lavalink.ConnectAsync(lavalinkConfig);
            await Task.Delay(-1);
        }

        private Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
