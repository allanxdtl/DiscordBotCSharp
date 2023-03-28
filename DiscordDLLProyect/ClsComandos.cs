using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordDLLProyect
{
    public class ClsComandos : BaseCommandModule
    {
        public delegate void Delegado(string print);
        public static Delegado Print;

        //Canciones que se lanzaran en forma aleatoria
        List<string> canciones = new List<string>()
        {
            "https://www.youtube.com/watch?v=qooWnw5rEcI",
            "https://www.youtube.com/watch?v=Up4WjdabA2c",
            "https://www.youtube.com/watch?v=oRdxUFDoQe0",
            "https://www.youtube.com/watch?v=HJqlA_HTEU8",
            "https://www.youtube.com/watch?v=3vjkh-acmTE",
            "https://www.youtube.com/watch?v=F9Ay74LfKd4",
            "https://www.youtube.com/watch?v=HJqlA_HTEU8",
            "https://www.youtube.com/watch?v=CEw-7cMnBDY"
        };

        List<string> wallpapers = new List<string>()
        {
            "https://images.wallpapersden.com/image/download/the-final-night-hd-illustrator_bWpnZWmUmZqaraWkpJRobWllrWdma2U.jpg",
            "https://images.wallpapersden.com/image/download/kurzgesagt-in-a-nutshell_bWxrbGmUmZqaraWkpJRmbmdlrWZlbWU.jpg",
            "https://images.wallpapersden.com/image/download/purple-sunset-reflected-in-the-ocean_a2ltZ2eUmZqaraWkpJRmbmtlrWZlbWU.jpg",
            "https://images.wallpapersden.com/image/download/sea-of-clouds-4k-photography_bWZoam2UmZqaraWkpJRobWllrWdma2U.jpg",
            "https://images.wallpapersden.com/image/download/sunset-point-illustration_bGdobmeUmZqaraWkpJRmbmdlrWZlbWU.jpg",
            "https://images7.alphacoders.com/102/1027412.jpg",
            "https://images6.alphacoders.com/909/909641.png",
            "https://images8.alphacoders.com/794/794362.jpg"
        };

        Random aleatorio;

        [Command("Wallpaper")]
        public async Task ImageCommand(CommandContext ctx)
        {
            aleatorio = new Random();
            var message = new DiscordEmbedBuilder()
            {
                Title = $"Cool {DiscordEmoji.FromName(ctx.Client, ":heart_eyes:")}",
                ImageUrl = wallpapers[aleatorio.Next(wallpapers.Count)],
                Color=DiscordColor.CornflowerBlue
            };
            await ctx.RespondAsync(embed: message);
            Print($"{DateTime.Now:hh:mm:ss}: Usuario {ctx.Member.Username} ejecuto comando wallpaper");
        }

        [Command("join")]
        public async Task JoinCommand(CommandContext ctx, DiscordChannel channel)
        {
            var lava = ctx.Client.GetLavalink();
            if (!lava.ConnectedNodes.Any())
            {
                await ctx.RespondAsync("El servidor Lavalink no esta activo :(");
                return;
            }

            var node = lava.ConnectedNodes.Values.First();

            if (channel.Type != ChannelType.Voice)
            {
                await ctx.RespondAsync("Lo que introduciste no es un canal de voz!");
                return;
            }

            await node.ConnectAsync(channel);
            await ctx.RespondAsync($"Joined {channel.Name}!");
            Print($"{DateTime.Now:hh:mm:ss}: Usuario {ctx.Member.Username} conecto el bot al canal {channel.Name}");
        }

        [Command("leave")]
        public async Task LeaveCommand(CommandContext ctx, DiscordChannel channel)
        {
            var lava = ctx.Client.GetLavalink();
            if (!lava.ConnectedNodes.Any())
            {
                await ctx.RespondAsync("El servidor Lavalink no esta activo :(");
                return;
            }

            var node = lava.ConnectedNodes.Values.First();

            if (channel.Type != ChannelType.Voice)
            {
                await ctx.RespondAsync("Lo que introduciste no es un canal de voz!");
                return;
            }

            var conn = node.GetGuildConnection(channel.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return;
            }

            await conn.DisconnectAsync();
            await ctx.RespondAsync($"Left {channel.Name}!");
            Print($"{DateTime.Now:hh:mm:ss}: Usuario {ctx.Member.Username} desconecto el bot de {channel.Name}");
        }

        [Command("play")]
        public async Task PlayCommand(CommandContext ctx, [RemainingText] string search)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("Asegurate de conectarte a un canal de voz");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Hubo un error, intenta unir al bot a un canal de voz con el comando !join [canal]");
                return;
            }

            var loadResult = await node.Rest.GetTracksAsync(search);

            if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed
                || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                await ctx.RespondAsync($"Se fallo al encontrar {search} :(. Vuelve a intentarlo");
                return;
            }

            var track = loadResult.Tracks.First();

            await conn.PlayAsync(track);

            await ctx.RespondAsync($"Reproduciendo {track.Title}!");
            Print($"{DateTime.Now:hh:mm:ss}: Usuario {ctx.Member.Username} pidio la cancion {search} y se esta reproduciendo {track.Title}");
        }

        [Command("pause")]
        public async Task Pause(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("No estas en un canal de voz, asegurate de unirte al que esta conectado el bot");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.RespondAsync("There are no tracks loaded.");
                return;
            }
            await conn.PauseAsync();
            Print($"{DateTime.Now:hh:mm:ss}: Usuario {ctx.Member.Username} pauso la reproducción de la música");
        }

        [Command("resume")]
        public async Task ResumeCommand(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected.");
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.RespondAsync("There are no tracks loaded.");
                return;
            }
            await conn.ResumeAsync();
            Print($"{DateTime.Now:hh:mm:ss}: Usuario {ctx.Member.Username} reaunudo la reproduccion de la música");
        }

        [Command("test")]
        public async Task TestCommand(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Este es un mensaje de prueba para el bot");
            Print($"{DateTime.Now:hh:mm:ss}: Usuario {ctx.Member.Username} ejecuto el comando test");
        }

        [Command("help")]
        public async Task HelpCommand(CommandContext ctx)
        {
            var message = new DiscordEmbedBuilder()
            {
                Title = $"Bienvenido {ctx.Member.Username}!!! {DiscordEmoji.FromName(ctx.Client, ":sunglasses:")}",
                Description = "Puedes utilizar los siguientes comandos:" +
                "\n*!test* : Este es un comando demo para probar el bot\n\n" +
                "*!join [canal de voz]*: Este es para hacer que el bot se una a un canal de audio y poder mandar canciones\n\n" +
                "*!play [nombre_cancion]*: Este es para reproducir la cancion que le mandes\n\n" +
                "*!pause*: Es para pausar la cancion en reproduccion\n\n" +
                "*!leave [canal de voz]*: Es para sacar del bot del canal que se le mande\n\n" +
                "*!operacion [num][signo][num]*: devuelve el resultado de una operacion matemática\n\n" +
                "*!Musicarandom*:Devuelve una cancion de youtube a sugerencia del bot\n\n" +
                "*!Wallpaper*: Devuelve un wallpaper a sugerencia del bot",
                Color = DiscordColor.Orange
            };

            await ctx.Channel.SendMessageAsync(embed: message);
            Print($"{DateTime.Now:hh:mm:ss}: Usuario {ctx.Member.Username} ejecuto el comando help");

        }

        [Command("operacion")]
        public async Task AddCommand(CommandContext ctx, [RemainingText] string texto)
        {
            double resultado = 0;
            if (texto.Contains('+'))
            {
                string[] numeros = texto.Split('+');
                resultado = double.Parse(numeros[0]) + double.Parse(numeros[1]);
            }
            if (texto.Contains('-'))
            {
                string[] numeros = texto.Split('-');
                resultado = double.Parse(numeros[0]) - double.Parse(numeros[1]);
            }
            if (texto.Contains('x'))
            {
                string[] numeros = texto.Split('x');
                resultado = double.Parse(numeros[0]) * double.Parse(numeros[1]);
            }
            if (texto.Contains('/'))
            {
                string[] numeros = texto.Split('/');
                resultado = double.Parse(numeros[0]) / double.Parse(numeros[1]);
            }

            await ctx.RespondAsync("El resultado es " + resultado);
            Print($"{DateTime.Now:hh:mm:ss}: Usuario {ctx.Member.Username} ejecuto el comando operacion y el resultado fue {resultado}");
        }

        [Command("MusicaRandom")]
        public async Task MusicRandCommand(CommandContext ctx)
        {
            aleatorio = new Random();
            await ctx.Channel.SendMessageAsync(canciones[aleatorio.Next(canciones.Count)]);
            Print($"{DateTime.Now:hh:mm:ss}: Usuario {ctx.Member.Username} ejecuto el comando MusicaRandom");
        }

    }
}
