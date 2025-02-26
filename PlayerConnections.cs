using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text.Json.Serialization;

namespace PlayerConnections;

public class PlayerConnectionsConfig : IBasePluginConfig
{
    [JsonPropertyName("JoinMessage")]
    public string JoinMessage { get; set; } = "{BLUE}[Astro]{GREEN}{PLAYERNAME} {NORMAL}SUNUCUYA KATILDI!";

    [JsonPropertyName("LeaveMessage")]
    public string LeaveMessage { get; set; } = "{BLUE}[Astro]{RED}{PLAYERNAME} {NORMAL}SUNUCUDAN AYRILDI!";

    public int Version { get; set; } = 1;
}

public static class ChatColorHelper
{
    public static string ReplaceColorTags(string message)
    {
        return message
            .Replace("{DEFAULT}", " \x01")     // Varsayılan
            .Replace("{NORMAL}", " \x01")      // Varsayılan (alternatif)
            .Replace("{WHITE}", " \x01")       // Beyaz
            .Replace("{DARKRED}", " \x02")     // Koyu Kırmızı
            .Replace("{RED}", " \x02")         // Kırmızı
            .Replace("{PURPLE}", " \x03")      // Mor
            .Replace("{GREEN}", " \x04")       // Yeşil
            .Replace("{LIGHTGREEN}", " \x05")  // Açık Yeşil
            .Replace("{LIME}", " \x06")        // Limon Yeşili
            .Replace("{LIGHTRED}", " \x07")    // Açık Kırmızı
            .Replace("{GRAY}", " \x08")        // Gri
            .Replace("{YELLOW}", " \x09")      // Sarı
            .Replace("{GOLD}", " \x10")        // Altın
            .Replace("{BLUE}", " \x0B")        // Mavi
            .Replace("{DARKBLUE}", " \x0C")    // Koyu Mavi
            .Replace("{BLUEGRAY}", " \x0D")    // Mavi-Gri
            .Replace("{MAGENTA}", " \x0E")     // Magenta
            .Replace("{LIGHTRED2}", " \x0F")   // Açık Kırmızı 2
            .Replace("{ORANGE}", " \x10");     // Turuncu
    }
}

public class PlayerConnections : BasePlugin, IPluginConfig<PlayerConnectionsConfig>
{
    public override string ModuleName => "Player Connections";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Xhirbos";
    public override string ModuleDescription => "Oyuncular sunucuya bağlandığında ve ayrıldığında mesaj gönderir";

    private HashSet<string> _connectedPlayers = new HashSet<string>();
    public PlayerConnectionsConfig Config { get; set; } = new PlayerConnectionsConfig();

    public void OnConfigParsed(PlayerConnectionsConfig config)
    {
        Config = config;
    }

    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventPlayerConnect>(OnPlayerConnect);
        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect, HookMode.Pre);
    }

    private HookResult OnPlayerConnect(EventPlayerConnect @event, GameEventInfo info)
    {
        if (string.IsNullOrEmpty(@event.Name)) return HookResult.Continue;
        
        if (!_connectedPlayers.Contains(@event.Name))
        {
            _connectedPlayers.Add(@event.Name);
            string message = Config.JoinMessage
                .Replace("{PLAYERNAME}", @event.Name);
            
            message = ChatColorHelper.ReplaceColorTags(message);
            Server.PrintToChatAll($" \x01{message}");
        }
        
        return HookResult.Continue;
    }

    private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (player == null || !player.IsValid) return HookResult.Continue;

        string playerName = player.PlayerName;
        if (_connectedPlayers.Contains(playerName))
        {
            _connectedPlayers.Remove(playerName);
            string message = Config.LeaveMessage
                .Replace("{PLAYERNAME}", playerName);
            
            message = ChatColorHelper.ReplaceColorTags(message);
            Server.PrintToChatAll($" \x01{message}");
        }

        return HookResult.Continue;
    }
} 