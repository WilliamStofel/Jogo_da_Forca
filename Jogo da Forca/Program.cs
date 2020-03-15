using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace Jogo_da_Forca
{
    class Program
    {


        [DllImport("user32.dll")]
        public static extern bool ShowWindow(System.IntPtr hWnd, int cmdShow);
        /// <summary>
        /// maxima o console em tela cheia.
        /// </summary>
        private static void Maximize()
        {
            Process p = Process.GetCurrentProcess();
            ShowWindow(p.MainWindowHandle, 3); //SW_MAXIMIZE = 3
        }


        struct Forca
        {
            public string Palavra;
            public int QtdDicas;
        }

        static int TempoSeg = 0, pontos = 0, y = 0, PosicaoPalavra = 0, MargemY, MargemX, chances = 9, plv = 0, words = 0, QtdPalavrasJogadas = 0, certo = 0;
        static int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        static string[] dicas = new string[10];
        static Forca[] p = new Forca[100];
        static Stopwatch sw = Stopwatch.StartNew();
        static int valido = 0;
        static char letra;

        /// <summary>
        /// Método para ler o arquivo texto. O for busca no struct se a palavra ja foi utilizada, caso ja foi o proximo if não é executado e é sorteado outra linha.
        /// O if salva a posição da palavra sorteada "PosicaoPalavra", define zero dicas palavra sorteada e salva a palavra sorteada.
        /// QtdPalavrasJogadas é incrementado para ser usado mais pra frente.
        /// </summary>
        static void LerArquivoTexto()
        {
            string[] linha = File.ReadAllLines("jogo.txt", Encoding.UTF8);
            Random x1 = new Random();
            int QtdPalavraVetor = 0, h;
            for (int i = 0; i < linha.Length - 1; i++)
            {
                if (linha[i].Substring(0, 1) == "P")
                    QtdPalavraVetor++;
            }
            do
            {
                bool sim = true;
                h = x1.Next(0, linha.Length - 1);
                for (int i = 0; i <= QtdPalavrasJogadas; i++)
                {
                    if (linha[h].Substring(2) == p[i].Palavra)
                    {
                        sim = false;
                    }
                }
                if (linha[h].Substring(0, 1) == "P" && sim == true)
                {
                    PosicaoPalavra = h;
                    p[plv].QtdDicas = 0;
                    p[plv].Palavra = linha[h].Substring(2);
                    QtdPalavrasJogadas++;

                }
                else if (QtdPalavraVetor == plv)
                {
                    Console.WriteLine("Total de Pontos: {0}", pontos);

                    Console.WriteLine("Deseja Jogar Novamente? Pressione 'S' para continuar, ou qualquer outra letra pra sair. \n");
                    letra = Console.ReadKey().KeyChar;
                    Console.WriteLine();
                    if (letra == 'S' || letra == 's')
                    {
                        restart();
                    }
                    else
                    {
                        Console.WriteLine("Obrigado por Jogar!!!");
                        Thread.Sleep(3000);
                        Environment.Exit(1);
                    }

                }

            }
            while (p[plv].Palavra == null);
            plv++;

        }

        static void LerArquivoTexto2()
        {


        }

        /// <summary>
        /// Método para mostrar pontuação final e reiniciar uma nova partida ou sair.
        /// </summary>
        static void EndGame()
        {
            Console.Clear();
            Console.WriteLine("Total de Pontos: {0}", pontos);

            Console.WriteLine("Deseja Jogar Novamente? Pressione 'S' para continuar, ou qualquer outra letra pra sair. \n");
            letra = Console.ReadKey().KeyChar;
            Console.WriteLine();
            if (letra == 'S' || letra == 's')
            {
                restart();
            }
            else
            {
                Console.WriteLine("Obrigado por Jogar!!!");
                Thread.Sleep(3000);
                Environment.Exit(1);
            }

        }


        /// <summary>
        /// Método para quando acaba o tempo.
        /// se usuario digitar 's' então reinicia o jogo, caso contrario encerra.
        /// </summary>
        static void TempoEsgotado()
        {
            Console.Clear();
            Console.WriteLine("Total de Pontos: {0}", pontos);
            Console.WriteLine("Acertou: {0} palavras", QtdPalavrasJogadas);
            Console.WriteLine("Deseja Jogar Novamente? Pressione 'S' para continuar, ou qualquer outra letra pra sair. \n");
            letra = Console.ReadKey().KeyChar;
            Console.WriteLine();
            if (letra == 'S' || letra == 's')
            {
                restart();
            }
            else
            {
                Console.WriteLine("Obrigado por Jogar!!!");
                Thread.Sleep(3000);
                Environment.Exit(1);
            }


        }

        /// <summary>
        /// começa do inicio o jogo, volta as variaveis ao estado incial.
        /// </summary>
        static void restart()
        {
            TempoSeg = 0;
            sw.Reset();
            pontos = 0; y = 0; PosicaoPalavra = 0; MargemY = 0; MargemX = 0; chances = 9; plv = 0; words = 0; QtdPalavrasJogadas = 0; certo = 0;
            dicas = new string[10];
            p = new Forca[100];
            valido = 0;
            letra = ' ';
            certo = 0;
            y = 0;
            sw.Start();
            numbers[1] = 2;
            numbers[2] = 3;
            numbers[3] = 4;
            numbers[4] = 5;
            numbers[5] = 6;
            numbers[6] = 7;
            numbers[7] = 8;
            numbers[8] = 9;
            numbers[9] = 10;
            Console.Clear();
            Console.WriteLine("Nome: William Stófel da Mota \nRA: 082170033");
            LerArquivoTexto();
            MostraTela();
            Dicas();
            Console.SetCursorPosition(0, 10 - p[words].QtdDicas);
            Console.WriteLine(dicas[p[words].QtdDicas - 1]);
            numbers[0] = 0;
            p[words].QtdDicas--;
            PressKey();

        }
        /// <summary>
        /// Desenha a tela, pega a altura e a largura da tela console e no eixo x subtrai pelos espaços que será usado pela palavra e divide por dois, para fazer as margens do lado.
        /// no eixo y divide por dois para fazer as margens.
        /// desenha a forca e os espaços para palavra.(é usado a variavel espaco para saber o tamanho da palavra, assim como foi usado para definir a margem no eixo x.)
        /// </summary>
        static void MostraTela()
        {

            int Espaços = p[words].Palavra.Length;
            int y = Console.LargestWindowHeight;
            int x = Console.LargestWindowWidth;
            MargemX = (x - (Espaços * 6)) / 2;
            MargemY = y / 2;

            Console.SetCursorPosition(MargemX - 8, MargemY - 1);
            Console.WriteLine("||||||||");
            Console.SetCursorPosition(MargemX - 8, MargemY);
            Console.WriteLine("||    ||");
            Console.SetCursorPosition(MargemX - 8, MargemY + 1);
            Console.WriteLine("||    ||");
            Console.SetCursorPosition(MargemX - 8, MargemY + 2);
            Console.WriteLine("||    ");
            Console.SetCursorPosition(MargemX - 8, MargemY + 3);
            Console.WriteLine("||    ");
            Console.SetCursorPosition(MargemX - 8, MargemY + 4);
            Console.WriteLine("||    ");
            Console.SetCursorPosition(MargemX - 8, MargemY + 5);
            Console.WriteLine("||    ");
            Console.SetCursorPosition(MargemX - 8, MargemY + 6);
            Console.WriteLine("||    ");
            Console.SetCursorPosition(MargemX - 8, MargemY + 7);
            Console.WriteLine("||    ");
            Console.SetCursorPosition(MargemX - 8, MargemY + 8);
            Console.WriteLine("||    ");
            Console.SetCursorPosition(MargemX - 8, MargemY + 9);
            Console.WriteLine("||    ");
            Console.SetCursorPosition(MargemX - 8, MargemY + 10);
            Console.WriteLine("||    ");
            Console.SetCursorPosition(MargemX - 8, MargemY + 11);
            Console.WriteLine("||    ");
            Console.SetCursorPosition(MargemX, MargemY + 11);
            for (int i = 0; i < Espaços; i++)
            {
                Console.Write(" ___ ");
            }
            Console.SetCursorPosition(MargemX - 8, MargemY + 13);
            Console.WriteLine("         ||||| ||||| ||||| ||||| ||||| ||||| ||||| ||||| ||||| ");
            Console.SetCursorPosition(MargemX - 8, MargemY + 14);
            Console.WriteLine("         |   | |   | |   | |   | |   | |   | |   | |   | |   | ");
            Console.SetCursorPosition(MargemX - 8, MargemY + 15);
            Console.WriteLine("Chances: |   | |   | |   | |   | |   | |   | |   | |   | |   | ");
            Console.SetCursorPosition(MargemX - 8, MargemY + 16);
            Console.WriteLine("         |   | |   | |   | |   | |   | |   | |   | |   | |   | ");
            Console.SetCursorPosition(MargemX - 8, MargemY + 17);
            Console.WriteLine("         ||||| ||||| ||||| ||||| ||||| ||||| ||||| ||||| ||||| ");


            Console.SetCursorPosition(MargemX, MargemY - 20);
            Console.WriteLine("Digite uma Letra Para Iniciar o Jogo! ");
            Console.SetCursorPosition(MargemX, MargemY - 19);
            Console.WriteLine("Caso Necessite aperte F2 para receber uma dica!");
        }

        /// <summary>
        /// metodo que salva todas as dicas da palavra sorteada.
        /// incrementa o NumeroDicas para pegar as proximas linhas que iniciam com "D", para salvar as dicas no vetor dicas.
        /// o if dentro do for é para quando chega na ultima dica, como o while sempre avalia a proxima então ele vai procurar a dica
        /// em uma linha maior do que o vetor  irá gerar uma exceção. Por isso o if para sair do looping, pois todas as dicas ja foram salvas.
        /// </summary>
        static void Dicas()
        {
            int NumeroDicas = 0;
            string[] linha = File.ReadAllLines("jogo.txt", Encoding.UTF8);
            int a = 0;

            do
            {
                p[words].QtdDicas++;
                NumeroDicas++;
                dicas[a] = linha[PosicaoPalavra + NumeroDicas].Substring(2);
                a++;
                if (PosicaoPalavra + NumeroDicas + 1 == 40)
                {
                    return;

                }
            }
            while (linha[PosicaoPalavra + NumeroDicas + 1].Substring(0, 1) != "P");


        }

        /// <summary>
        /// Metodo para mostrar as dicas e se o jogador errar tambem tira um ponto dele.
        /// primeiro if é para avaliar se a tecla F2 foi pressionada, dentro do escopo do primeiro if está explicando os demais.
        /// A ultima instrução, else no caso, é quando a letra está errada, ja que passou por todas as outras opções.
        /// </summary>
        static void MostrarDicas()
        {

            if (letra == '\0')
            {
                if (numbers[0] == 0 && p[words].QtdDicas != 0) // se a dica na posição 0 ja foi mostrada na tela e qtd de dicas não é zero.
                {
                    Console.SetCursorPosition(0, 10 - p[words].QtdDicas);

                    pontos -= 5;
                    chances--;
                    Console.WriteLine(dicas[p[words].QtdDicas - 1]);
                    numbers[p[words].QtdDicas] = 0;
                    p[words].QtdDicas--;
                    DesenhaErro();
                }
                else if (numbers[0] != 0 && p[words].QtdDicas != 0)// se a dica na posição 0 não foi mostrada na tela e qtd de dicas não é zero.
                {
                    Console.SetCursorPosition(0, 10 - p[words].QtdDicas);
                    Console.WriteLine(dicas[p[words].QtdDicas - 1]);
                    numbers[0] = 0;
                    p[words].QtdDicas--;
                }
                else // as dicas foram esgotadas.
                {
                    Console.SetCursorPosition(0, 10 - p[words].QtdDicas);
                    Console.WriteLine("Acabaram as dicas para está palavra!");
                }
            }
            else
            {
                chances--;
                pontos -= 5;
                DesenhaErro();
            }

        }

        /// <summary>
        /// Metodo que avalia se uma tecla foi pressionada.
        /// do while para continuar avaliando se uma tecla foi pressionada enquanto chances forem diferentes de zero.
        /// for com o tamanho da palavra sorteada para avaliar se a letra pressionada está correta.
        /// metodo escreve dentro do for para escrever as letras corretas.
        /// mais informações dentro do metodo.
        /// </summary>
        static void PressKey()
        {
            chances = 9;
            int PN;
            string forma = p[words].Palavra; // string com a palavra sorteada, a cada vez que uma letra está correta é removida da palavra. 
            do
            {
                Console.SetCursorPosition(MargemX, MargemY - 18);
                TempoSeg = Convert.ToInt32(sw.Elapsed.TotalSeconds);
                Console.WriteLine("Ainda restam: " + (60 - TempoSeg + "seg."));
                Thread.Sleep(100);
                Console.SetCursorPosition(0, 30);
                Console.WriteLine("Digite uma letra !");
                if (Console.KeyAvailable)
                {
                    letra = Console.ReadKey().KeyChar;

                    valido = 0;// aqui a variavel é zerada a a cada vez que uma letra é pressionada.
                    for (int i = 0; i < p[words].Palavra.Length; i++)
                    {
                        if (letra == p[words].Palavra[i] && forma.Contains(letra))// escreve a letra se estiver correta
                        {

                            forma = forma.Remove(forma.IndexOf(letra), 1);
                            PN = i;
                            valido++;
                            escreve(PN);
                        }
                    }
                    /*if (sw.Elapsed.Minutes >= 1)
                    {
                        sw.Stop();
                        TempoEsgotado();
                    }*/
                    if (valido == 0) //if para avaliar se nenhuma letra está correta.
                        MostrarDicas();// chamando metodo para avaliar se a letra esta errada ou se é uma dica
                }
            }
            while (chances > 0 && TempoSeg < 60);
            if (chances == 0)//se o jogador usou todas as suas chances é chamado o proximo metodo, que encerra o jogo.
            {
                Youlost();
            }
            else
                EndGame();

        }

        /// <summary>
        /// metodo para desenhar o erro, usar uma variavel aux "y" para localizar a posicão correta onde o x deve ser colocado.
        /// </summary>

        static void DesenhaErro()
        {

            y += 6;
            Console.SetCursorPosition(MargemX + y - 3, MargemY + 15);
            Console.Write("X");

        }

        /// <summary>
        /// metodo para escrever as letras corretas, recebe por parametro "i" que é a posição da letra.
        /// if que avalia quando a palavra foi completada corretamente. Quando for verdadeiro então ele reinicia todas variaveis globais que necessitam reiniciar
        /// executa os metodos que necessitam para a proxima palavra, e já da uma dica inicial.
        /// 
        /// </summary>
        static void escreve(int i)
        {
            certo++;
            int r = (5 * (i + 1)) - 3;
            Console.SetCursorPosition(MargemX + r, MargemY + 10);
            Console.WriteLine(p[words].Palavra[i]);
            if (certo == p[words].Palavra.Length)
            {
                sw.Start();
                pontos += 50;
                Console.SetCursorPosition(0, 3);
                Console.Write(pontos);
                certo = 0;
                y = 0;
                words++;
                numbers[1] = 2;
                numbers[2] = 3;
                numbers[3] = 4;
                numbers[4] = 5;
                numbers[5] = 6;
                numbers[6] = 7;
                numbers[7] = 8;
                numbers[8] = 9;
                numbers[9] = 10;
                Console.Clear();
                Console.WriteLine("Nome: William Stófel da Mota \nRA: 082170033");
                LerArquivoTexto();
                MostraTela();
                Dicas();
                Console.SetCursorPosition(0, 10 - p[words].QtdDicas);
                Console.WriteLine(dicas[p[words].QtdDicas - 1]);
                numbers[0] = 0;
                p[words].QtdDicas--;
                Console.SetCursorPosition(0, 3);
                Console.Write("PONTOS: {0}", pontos);
                PressKey();

            }

        }

        /// <summary>
        /// desenha uma forca com o boneco quando o jogador perde.
        /// </summary>
        static void Youlost()
        {

            Console.SetCursorPosition(MargemX + 5, MargemY - 1);
            Console.WriteLine("Você Perdeu !!! ");
            Console.SetCursorPosition(MargemX - 8, MargemY - 1);
            Console.WriteLine("||||||||");
            Console.SetCursorPosition(MargemX - 8, MargemY);
            Console.WriteLine("||    ||");
            Console.SetCursorPosition(MargemX - 8, MargemY + 1);
            Console.WriteLine("||    ||");
            Console.SetCursorPosition(MargemX - 8, MargemY + 2);
            Console.WriteLine("||   ****");
            Console.SetCursorPosition(MargemX - 8, MargemY + 3);
            Console.WriteLine("||  ******");
            Console.SetCursorPosition(MargemX - 8, MargemY + 4);
            Console.WriteLine("||   ****");
            Console.SetCursorPosition(MargemX - 8, MargemY + 5);
            Console.WriteLine("||    **");
            Console.SetCursorPosition(MargemX - 8, MargemY + 6);
            Console.WriteLine("||  ******");
            Console.SetCursorPosition(MargemX - 8, MargemY + 7);
            Console.WriteLine("|| *  **  *");
            Console.SetCursorPosition(MargemX - 8, MargemY + 8);
            Console.WriteLine("||    **");
            Console.SetCursorPosition(MargemX - 8, MargemY + 9);
            Console.WriteLine("||  *    *");
            Console.SetCursorPosition(MargemX - 8, MargemY + 9);
            Console.WriteLine("|| *      *");
            Console.SetCursorPosition(MargemX - 8, MargemY + 10);
            Thread.Sleep(2000);
        }

        static void Main(string[] args)
        {
            Maximize();// metodo para maximizar o console em tela cheia.
            
            Console.Write("JOGO               [ x ] >>>>> 6 pontos." + "\nDICAS               [ x ] >>>>> 2 pontos." + "\nCONTROLE DE TEMPO               [ x ] >>>>> 2 pontos.");
            Thread.Sleep(4000);
            Console.Clear();
            Console.WriteLine("Nome: William Stófel da Mota \nRA: 082170033");
            LerArquivoTexto();
            MostraTela();
            Dicas();
            MostrarDicas();

            PressKey();

            Console.ReadLine();

        }
    }
}
