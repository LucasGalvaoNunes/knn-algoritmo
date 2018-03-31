using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
/*
 Autor: Lucas Galvão Nunes

    Funcionamento:
        Para que funcione corretamente o algoritimo, precisa colocar os arquivos arff de teste e de treinamento na mesma
        pasta do executavel deste programa.
        
        Quando rodar o executavel ele ira pedir o nome do arquivo de treinamento e depois o de teste, colocar somente o nome
        não colocar a extensão.

     */
/// <summary>
/// Classe main do projeto
/// </summary>
class KNNAlgoritimo
{
    /// <summary>
    /// Pow = elevacao, Sqrt raiz
    /// </summary>
    /// <param name="pComparar"></param>
    /// <param name="pComparador"></param>
    /// <returns></returns>
    static Classificacao Calculo(Dado pDadoVerificar, Dado pDadoTreinamento)
    {
        double somatoria = 0;
        for (int i = 0; i < pDadoVerificar.Dados.Length; i++)
        {
            // Somatoria dos atributos elevados ao quadrado..
            somatoria += Math.Pow((pDadoVerificar.Dados[i] - pDadoTreinamento.Dados[i]), 2);
        }
        // Retorna a sua distancia.
        return new Classificacao(pDadoVerificar.Classe, pDadoTreinamento.Classe, Math.Sqrt(somatoria));
    }
    static void Main(string[] args)
    {
        int geralAcertos = 0;                                               // Quantia total de acertos
        int geraErros = 0;                                                  // Quantia total de erros
        List<string> classes = new List<string>();                          // Classes que serao avaliadas
        List<Dado> dadosTreinamento = new List<Dado>();                     // Lista dos dados de treinamento
        List<Dado> dadosVerificacao = new List<Dado>();                     // dados para verificacao
        List<Matriz> matriz = new List<Matriz>();                           // matriz para apresentar
        string line = "";                                                   // auxiliar para ler o arff em linhas
        int vizinhosK = 0;                                                  // Vizinhos
                                                                            // quantias de vizinhoa mais proximos
        string testes = "";
        string treinar = "";
        Console.WriteLine("Obs: Para que funcione corretamente, adcione os arquivos .arff no mesmo local que esta este executavel.\n\n\n");
        Console.Write("Digite o nome do arquivo de treinamento: ");
        treinar = Console.ReadLine();
        Console.Write("Digite o nome do arquivo de testes: ");
        testes = Console.ReadLine();
        Console.Write("Digite o valor para K: ");
        vizinhosK = Convert.ToInt32(Console.ReadLine());


        // Ler o arquivo Arrf de treinamento
        using (StreamReader sr = new StreamReader(treinar + ".arff"))
        {
            // linha por linha
            while ((line = sr.ReadLine()) != null)
            {
                // Ve se a linha contém algo
                if (!String.IsNullOrEmpty(line))
                {
                    // se for aributo
                    if (line.ToCharArray()[0] != '@' && line.ToCharArray()[0] != '%')
                    {
                        // separa os valores depois da virgula
                        string[] valores = line.Split(',');
                        string classe = valores[valores.Length - 1];
                        valores = valores.Where(val => val != classe).ToArray();
                        double[] dados = new double[valores.Length];
                        for (int i = 0; i < valores.Length; i++)
                        {
                            valores[i] = valores[i].Replace('.', ',');
                            dados[i] = Convert.ToDouble(valores[i]);
                        }
                        // Adciona a lista de dados de treinamento.
                        // Foi utilizado o replace de '.' para ',' por que ele estava arrendondando o valor de double.
                        dadosTreinamento.Add(new Dado(classe, dados));

                        //dadosTreinamento.Add(new Dado(Convert.ToDouble((valores[0] = valores[0].Replace('.', ','))), Convert.ToDouble((valores[1] = valores[1].Replace('.', ','))),
                        //    Convert.ToDouble((valores[2] = valores[2].Replace('.', ','))), Convert.ToDouble((valores[3] = valores[3].Replace('.', ','))), valores[4]));
                    }
                    if (line.Contains("@ATTRIBUTE class"))
                    {
                        line = line.Remove(0, 18);
                        line = line.Replace('{', ' ');
                        line = line.Replace('}', ' ');
                        line = line.Trim();
                        string[] classesAux = line.Split(',');
                        classes = classesAux.ToList();
                    }
                }
            }

        }
        // Ler o arquivo Arrf para verificaçã
        using (StreamReader sr = new StreamReader(testes + ".arff"))
        {
            // linha por linha
            while ((line = sr.ReadLine()) != null)
            {
                // Ve se a linha contém algo
                if (!String.IsNullOrEmpty(line))
                {
                    // se for aributo
                    if (line.ToCharArray()[0] != '@' && line.ToCharArray()[0] != '%')
                    {
                        // separa os valores depois da virgula
                        string[] valores = line.Split(',');
                        string classe = valores[valores.Length - 1];
                        valores = valores.Where(val => val != classe).ToArray();
                        double[] dados = new double[valores.Length];
                        for (int i = 0; i < valores.Length; i++)
                        {
                            valores[i] = valores[i].Replace('.', ',');
                            dados[i] = Convert.ToDouble(valores[i]);
                        }
                        // Adciona a lista de dados de treinamento.
                        // Foi utilizado o replace de '.' para ',' por que ele estava arrendondando o valor de double.
                        dadosVerificacao.Add(new Dado(classe, dados));

                        //dadosTreinamento.Add(new Dado(Convert.ToDouble((valores[0] = valores[0].Replace('.', ','))), Convert.ToDouble((valores[1] = valores[1].Replace('.', ','))),
                        //    Convert.ToDouble((valores[2] = valores[2].Replace('.', ','))), Convert.ToDouble((valores[3] = valores[3].Replace('.', ','))), valores[4]));
                    }
                }
            }

        }
        // Apenas instancia a classe de matriz conforme o numero de classes presentes no arquivo
        for (int cl = 0; cl < classes.Count; cl++)
        {
            List<QuatidadeDeRepeticao> aux = new List<QuatidadeDeRepeticao>();
            for (int j = 0; j < classes.Count; j++)
            {
                aux.Add(new QuatidadeDeRepeticao(classes[j], 0));
            }
            matriz.Add(new Matriz(classes[cl], aux));
        }
        // Passo um por um pelo dado a ser verificado
        for (int verificar = 0; verificar < dadosVerificacao.Count; verificar++)
        {
            // instancio uma classe responsavel para vez qual o vizinho mais proximos
            List<QuatidadeDeRepeticao> quantidadeRepeticao = new List<QuatidadeDeRepeticao>();
            for (int cl = 0; cl < classes.Count; cl++)
            {
                quantidadeRepeticao.Add(new QuatidadeDeRepeticao(classes[cl], 0));
            }
            // Total de distancias por dados.
            List<Classificacao> totasDistanciaPorVerificacao = new List<Classificacao>();
            // As menores distancias encontradas na lista de todas as distancias
            List<Classificacao> menoresdistanciaPorVerificacao = new List<Classificacao>();
            for (int treinado = 0; treinado < dadosTreinamento.Count; treinado++)
            {
                //Faz o calculo da distancia
                Classificacao distanciaEuclidiana = Calculo(dadosVerificacao[verificar], dadosTreinamento[treinado]);
                // Adciona o resultado na array de resulado com dado que esta sendo verificado
                totasDistanciaPorVerificacao.Add(distanciaEuclidiana);
            }
            // Ordeno em ordem crescente
            totasDistanciaPorVerificacao = totasDistanciaPorVerificacao.OrderBy(dis => dis.Distancia).ToList();
            // Neste laco pega-se os vizinhos mais proximos(Com a menor distancia)
            for (int vizinhos = 0; vizinhos < vizinhosK; vizinhos++)
            {
                menoresdistanciaPorVerificacao.Add(totasDistanciaPorVerificacao[vizinhos]);
            }
            // Ondeno em ordem crescente
            menoresdistanciaPorVerificacao = menoresdistanciaPorVerificacao.OrderBy(cre => cre.Distancia).ToList();
            // Neste laco verifico faco a contagem de quais vizinhos apareceram mais vezes
            for (int menor = 0; menor < menoresdistanciaPorVerificacao.Count; menor++)
            {
                for (int ver = 0; ver < quantidadeRepeticao.Count; ver++)
                {
                    if (menoresdistanciaPorVerificacao[menor].Comparado == quantidadeRepeticao[ver].Nome)
                    {
                        quantidadeRepeticao[ver].Qtd++;
                        break;
                    }
                }
            }
            // Ordeno em ordem crescente
            quantidadeRepeticao = quantidadeRepeticao.OrderBy(c => c.Qtd).ToList();
            if (menoresdistanciaPorVerificacao[0].Sou == quantidadeRepeticao[quantidadeRepeticao.Count - 1].Nome)
            {
                geralAcertos++;
            }
            else
            {
                geraErros++;
            }
            // Adciono no resultado geral de todos os dados
            foreach (var c in matriz)
            {
                // Onde procuro a classe para ver as classificacoes
                if (c.Classe == menoresdistanciaPorVerificacao[0].Sou)
                {
                    // dentro das classificacoes da classe
                    foreach (var b in c.Classificacao)
                    {
                        // verifico qual nome e igual ao vizinho que apareceu mais vezes  
                        if (b.Nome == quantidadeRepeticao[quantidadeRepeticao.Count - 1].Nome)
                        {
                            // incremento quantia de vezes que apareceu
                            b.Qtd++;
                            break;
                        }
                    }
                    break;
                }
            }
        }
        string[,] vetor = new string[matriz.Count, matriz.Count + 1];
        for (int i = 0; i < matriz.Count; i++)
        {
            for (int j = 0; j < matriz[i].Classificacao.Count; j++)
            {
                vetor[i, j] = matriz[i].Classificacao[j].Qtd.ToString();
                if (j == matriz[i].Classificacao.Count - 1)
                {
                    vetor[i, j + 1] = "Classe " + i.ToString() + "<- " + matriz[i].Classe;
                }
            }
        }
        
        Console.WriteLine("\n\nTOTAL DE DADOS VERIFICADOS: " + dadosVerificacao.Count);
        Console.WriteLine("TOTAL DE DADOS DE TREINAMENTO: " + dadosTreinamento.Count);
        Console.WriteLine("TOTAL DE ACERTOS: " + geralAcertos.ToString());
        Console.WriteLine("TOTAL DE ERROS: " + geraErros.ToString());
        double result = (double)((double)geralAcertos / (double)dadosVerificacao.Count);
        Console.WriteLine("PORCENTAGEM DE ACERTOS: " + Math.Round((double)result * 100, 2) + "%");
        Console.WriteLine("\n\nMATRIZ DE CONFUSÃO\n");
        for (int i = 0; i < matriz.Count; i++)
        {
            Console.Write("Classe " + i + "  ");
        }
        Console.Write("\n");
        for (int i = 0; i < matriz.Count; i++)
        {
            for (int j = 0; j < matriz.Count + 1; j++)
            {
                if (j == matriz.Count + 1 - 1)
                    Console.Write("     |");
                if (j != matriz.Count - 1)
                    Console.Write(vetor[i, j] + "           ");
                else
                    Console.Write(vetor[i, j]);

            }
            Console.Write("\n");
        }
        Console.Read();
    }
}
/// <summary>
/// Classe responsavel por armazenar os dados do arff
/// </summary>
class Dado
{
    public string Classe { get; set; }  // Classe do objeto vindo do arff
    public double[] Dados { get; set; }

    public Dado(string classe, double[] dados)
    {
        Classe = classe;
        Dados = dados;
    }
}
/// <summary>
/// Classe responsavel por armazenar o resultado do calculo da Distância euclidiana
/// </summary>
class Classificacao
{
    public string Sou { get; set; }         // Quem ele e
    public string Comparado { get; set; }   // Com quem foi comparado
    public double Distancia { get; set; }   // Qual foi sua distancia
    public Classificacao(string sou, string comparado, double distancia)
    {
        Sou = sou;
        Comparado = comparado;
        Distancia = distancia;
    }
}
/// <summary>
/// Classe responsavel por armazenar as informações da matriz de confusão
/// </summary>
class Matriz
{
    public string Classe { get; set; }
    public List<QuatidadeDeRepeticao> Classificacao = new List<QuatidadeDeRepeticao>();

    public Matriz(string ClasseMain, List<QuatidadeDeRepeticao> classifica)
    {
        Classe = ClasseMain;
        Classificacao = classifica;
    }
}
/// <summary>
/// Classe responsavel por verificar qual classe se repetiu mais nas distancias
/// </summary>
class QuatidadeDeRepeticao
{
    public string Nome { get; set; }
    public int Qtd { get; set; }

    public QuatidadeDeRepeticao(string nome, int qtd)
    {
        Nome = nome;
        Qtd = qtd;
    }
}