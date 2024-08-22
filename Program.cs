using PokeEQuem;
using Newtonsoft.Json;
using System.Net;
using System.Drawing;

static async Task<Pokemon> BuscarPokemonAleatorio()
{
    using var client = new HttpClient();
    var randomId = new Random().Next(1, 151);
    var url = $"https://pokeapi.co/api/v2/pokemon/{randomId}";
    var response = await client.GetAsync(url);
    var json = await response.Content.ReadAsStringAsync();
    return JsonConvert.DeserializeObject<Pokemon>(json);

}

static void DownloadImagemPokemon(string imagemUrl, string nomePokemon)
{
    string pastaImagens = Path.Combine(Directory.GetCurrentDirectory(), "imagens");
    string caminhoArquivo = Path.Combine(pastaImagens, $"{nomePokemon}.jpg");

    if (!Directory.Exists(pastaImagens))
    {
        Directory.CreateDirectory(pastaImagens);
    }

    using (WebClient client = new WebClient())
    {
        client.DownloadFile(imagemUrl,
            caminhoArquivo);
    }
}

static Bitmap ResizeImage(Bitmap image, int width, int height)
{
    Bitmap resizedImage = new Bitmap(width, height);
    using (Graphics g
= Graphics.FromImage(resizedImage))
    {
        g.DrawImage(image, 0, 0, width, height);
    }
    return resizedImage;

}

static char[,] ConvertToAscii(Bitmap image)
{
    char[] asciiChars = " .:-=+*#%@".ToCharArray(); // Tabela de mapeamento exemplo

    int width = image.Width;
    int height = image.Height;
    char[,] asciiImage = new char[height, width];

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            Color pixelColor = image.GetPixel(x, y);
            int grayScale = (int)(pixelColor.R * 0.299 + pixelColor.G * 0.587 + pixelColor.B * 0.114);
            int index = (int)(grayScale / 255.0 * (asciiChars.Length - 1));
            asciiImage[y, x] = asciiChars[index];
        }
    }

    return asciiImage;
}

static void ImagemParaAscii(string nomePokemon)
{
    string pastaImagens = Path.Combine(Directory.GetCurrentDirectory(), "imagens");

    string caminhoArquivo = Path.Combine(pastaImagens, $"{nomePokemon}.jpg");

    int width = 90;
    int height = 45;
    // Carrega a imagem
    Bitmap image = new Bitmap(caminhoArquivo);
    image = ResizeImage(image, width, height);

    char[,] asciiImage = ConvertToAscii(image);

    // Imprimir a imagem ASCII no console
    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            Console.Write(asciiImage[y, x]);
        }
        Console.WriteLine();
    }
}

var pokemon = await BuscarPokemonAleatorio();
DownloadImagemPokemon(pokemon.Sprites.Front_default, pokemon.Name);

Console.WriteLine(pokemon.Name);
Console.WriteLine("-------------- Quem é esse Pokémon? --------------");
Console.WriteLine("Acabei de pensar no nome de um Pokémon, será que você consegue adivinhar?");

var chute = 0;
while (true)
{
    Console.WriteLine(" ");
    Console.WriteLine("Qual Pokémon eu pensei? ");
    var resposta = Console.ReadLine().ToLower();
    chute++;

    if (resposta == pokemon.Name.ToLower())
    {
        Console.WriteLine($"Parabéns! Você acertou! É o {pokemon.Name.ToUpper()}");
        
        break;
    }
    else if (chute == 1)
    {
        Console.WriteLine("Errado! Lá vai uma dica: ");
        Console.WriteLine($"Esse Pokémon é do tipo {pokemon.Types[0].Type.Name}");
    }

    else if (chute == 2)
    {
        Console.WriteLine("Errado! Vou te dar uma dica infalível: ");
        Console.WriteLine($"O número da Pokedex deste Pokémon é {pokemon.Id}");
    }
    else
    {
        Console.WriteLine("Errado! Agora não tem como errar: ");
        ImagemParaAscii(pokemon.Name);
    }
    
}