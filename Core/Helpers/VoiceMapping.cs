namespace Core.Helpers;

public static class VoiceMapping
{
    public static readonly Dictionary<string, List<string>> CountryVoices = new()
    {
        ["Spain"] =
        [
            "es-ES-ElviraNeural", "es-ES-AlvaroNeural", "es-ES-AbrilNeural", "es-ES-ArnauNeural",
            "es-ES-DarioNeural", "es-ES-EliasNeural", "es-ES-EstrellaNeural", "es-ES-IreneNeural",
            "es-ES-LaiaNeural", "es-ES-LiaNeural", "es-ES-NilNeural", "es-ES-SaulNeural",
            "es-ES-TeoNeural", "es-ES-TrianaNeural", "es-ES-VeraNeural"
        ],
        ["Mexico"] =
        [
            "es-MX-DaliaNeural", "es-MX-JorgeNeural", "es-MX-BeatrizNeural", "es-MX-CandelaNeural",
            "es-MX-CarlotaNeural", "es-MX-CecilioNeural", "es-MX-GerardoNeural", "es-MX-LarissaNeural",
            "es-MX-LibertoNeural", "es-MX-LucianoNeural", "es-MX-MarinaNeural", "es-MX-NuriaNeural",
            "es-MX-PelayoNeural", "es-MX-RenataNeural", "es-MX-YagoNeural"
        ],
        ["Argentina"] = ["es-AR-ElenaNeural", "es-AR-TomasNeural"],
        ["Brazil"] =
        [
            "pt-BR-FranciscaNeural", "pt-BR-AntonioNeural", "pt-BR-BrendaNeural", "pt-BR-DonatoNeural",
            "pt-BR-ElzaNeural", "pt-BR-FabioNeural", "pt-BR-GiovannaNeural", "pt-BR-HumbertoNeural",
            "pt-BR-JulioNeural", "pt-BR-LeilaNeural", "pt-BR-LeticiaNeural", "pt-BR-ManuelaNeural",
            "pt-BR-NicolauNeural", "pt-BR-ValerioNeural", "pt-BR-YaraNeural"
        ],
        ["Portugal"] = ["pt-PT-RaquelNeural", "pt-PT-DuarteNeural", "pt-PT-FernandaNeural"],
        ["Japan"] =
        [
            "ja-JP-NanamiNeural", "ja-JP-KeitaNeural", "ja-JP-AoiNeural", "ja-JP-DaichiNeural",
            "ja-JP-MayuNeural", "ja-JP-NaokiNeural", "ja-JP-ShioriNeural"
        ],
        ["Germany"] =
        [
            "de-DE-KatjaNeural", "de-DE-ConradNeural", "de-DE-AmalaNeural", "de-DE-BerndNeural",
            "de-DE-ChristophNeural", "de-DE-ElkeNeural", "de-DE-GiselaNeural", "de-DE-KasperNeural",
            "de-DE-KillianNeural", "de-DE-KlarissaNeural", "de-DE-KlausNeural", "de-DE-LouisaNeural",
            "de-DE-MajaNeural", "de-DE-RalfNeural", "de-DE-TanjaNeural"
        ],
        ["Italy"] =
        [
            "it-IT-ElsaNeural", "it-IT-IsabellaNeural", "it-IT-DiegoNeural", "it-IT-BenignoNeural",
            "it-IT-CalimeroNeural", "it-IT-CataldoNeural", "it-IT-FabiolaNeural", "it-IT-FiammaNeural",
            "it-IT-GianniNeural", "it-IT-ImeldaNeural", "it-IT-IrmaNeural", "it-IT-LisandroNeural",
            "it-IT-PalmiraNeural", "it-IT-PierinaNeural", "it-IT-RinaldoNeural"
        ],
        ["France"] =
        [
            "fr-FR-DeniseNeural", "fr-FR-HenriNeural", "fr-FR-AlainNeural", "fr-FR-BrigitteNeural",
            "fr-FR-CelesteNeural", "fr-FR-ClaudeNeural", "fr-FR-CoralieNeural", "fr-FR-EloiseNeural",
            "fr-FR-JacquelineNeural", "fr-FR-JeromeNeural", "fr-FR-JosephineNeural", "fr-FR-MauriceNeural",
            "fr-FR-YvesNeural", "fr-FR-YvetteNeural"
        ],
        ["Canada"] = ["fr-CA-SylvieNeural", "fr-CA-JeanNeural", "fr-CA-AntoineNeural"]
    };

    public static string GetRandomVoice(string country)
    {
        if (!CountryVoices.TryGetValue(country, out var voices))
            
            throw new ArgumentException($"No voices found for country: {country}");
        var random = new Random();
        return voices[random.Next(voices.Count)];

    }
}