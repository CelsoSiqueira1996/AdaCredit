using AdaCredit.AdaCredit.Clientes;
using Bogus.DataSets;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaCredit.AdaCredit.Transacoes
{
    public static class RepositorioTransacoes
    {
        public static bool ProcessarTransacoesPendentes()
        {
            Transacoes.CriarPastasTransacoes();
            var pastaTransacoesPendentes = new DirectoryInfo(Transacoes._caminhoTransacoesPendentes);
            string[] infoTransacao;
            int contadorArquivo;
            if (!pastaTransacoesPendentes.EnumerateFiles().Any())
            {
                return false;
            }
            foreach (FileInfo arquivo in pastaTransacoesPendentes.EnumerateFiles().OrderBy(
                x => DateTime.ParseExact(Path.GetFileNameWithoutExtension(x.Name).Split("-")[Path.GetFileNameWithoutExtension(x.Name).Split("-").Length - 1],
                "yyyyMMdd", CultureInfo.InvariantCulture)))
            {
                Transacoes.CriarPastasTransacoesProcessadas(Path.GetFileNameWithoutExtension(arquivo.Name));
                FileInfo arquivoRealizadas = new FileInfo(Path.Combine(Transacoes._caminhoTransacoesRealizadas,
                    Path.GetFileNameWithoutExtension(arquivo.Name)+"-completed.csv"));
                FileInfo arquivoFalharam = new FileInfo(Path.Combine(Transacoes._caminhoTransacoesFalharam,
                    Path.GetFileNameWithoutExtension(arquivo.Name)+"-failed.csv"));
                using (StreamReader sr = arquivo.OpenText())
                {
                    var s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        contadorArquivo = 1;
                        infoTransacao = s.Split(",");
                        while (true)
                        {
                            var bancoOrigem = infoTransacao[0];
                            var agenciaOrigem = infoTransacao[1];
                            var contaOrigem = infoTransacao[2];
                            var bancoDestino = infoTransacao[3];
                            var agenciaDestino = infoTransacao[4];
                            var contaDestino = infoTransacao[5];
                            var tipoTransacao = infoTransacao[6];
                            var valor = Convert.ToDecimal(infoTransacao[7], CultureInfo.CreateSpecificCulture("en-US"));
                            Cliente? clienteOrigem = RepositorioClientes.ConsultarDadosCliente(contaOrigem);
                            Cliente? clienteDestino = RepositorioClientes.ConsultarDadosCliente(contaDestino);
                            if (bancoOrigem == "777" && bancoDestino == "777")
                            {
                                if (agenciaOrigem != "0001" || agenciaDestino != "0001")
                                {
                                    contadorArquivo = 0;
                                    break;
                                }
                                if (clienteOrigem == null || clienteDestino == null)
                                {
                                    contadorArquivo = 0;
                                    break;
                                }
                                if (!clienteOrigem.Ativo || !clienteDestino.Ativo)
                                {
                                    contadorArquivo = 0;
                                    break;
                                }
                                if (tipoTransacao != "TEF")
                                {
                                    contadorArquivo = 0;
                                    break;
                                }
                                if (clienteOrigem.Saldo >= valor)
                                {
                                    Cliente.DebitarSaldoCliente(valor, clienteOrigem);
                                    Cliente.CreditarSaldoCliente(valor, clienteDestino);
                                    break;
                                }
                                else
                                {
                                    contadorArquivo = 0;
                                    break;
                                }
                            }
                            if (bancoOrigem == "777")
                            {
                                if (agenciaOrigem != "0001")
                                {
                                    contadorArquivo = 0;
                                    break;
                                }
                                if (clienteOrigem == null)
                                {
                                    contadorArquivo = 0;
                                    break;
                                }
                                if (!clienteOrigem.Ativo)
                                {
                                    contadorArquivo = 0;
                                    break;
                                }
                                if (tipoTransacao == "TEF")
                                {
                                    contadorArquivo = 0;
                                    break;
                                }
                                var data = DateTime.ParseExact(Path.GetFileNameWithoutExtension(arquivo.Name).Split("-")[Path.GetFileNameWithoutExtension(arquivo.Name).Split("-").Length - 1],
                                    "yyyyMMdd", CultureInfo.InvariantCulture);
                                if (data >= new DateTime(2022, 12, 01))
                                {
                                    if (tipoTransacao == "TED")
                                    {
                                        if (clienteOrigem.Saldo >= valor + 5)
                                        {
                                            Cliente.DebitarSaldoCliente(valor + 5, clienteOrigem);
                                            break;
                                        }
                                        else
                                        {
                                            contadorArquivo = 0;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (clienteOrigem.Saldo >= Math.Min(1 + valor * 1.01M, 6 + valor))
                                        {
                                            Cliente.DebitarSaldoCliente(Math.Min(1 + valor * 1.01M, 6 + valor), clienteOrigem);
                                            break;
                                        }
                                        else
                                        {
                                            contadorArquivo = 0;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (clienteOrigem.Saldo >= valor)
                                    {
                                        Cliente.DebitarSaldoCliente(valor, clienteOrigem);
                                        break;
                                    }
                                    else
                                    {
                                        contadorArquivo = 0;
                                        break;
                                    }
                                }
                            }
                            if (bancoDestino == "777")
                            {
                                if (agenciaDestino != "0001")
                                {
                                    contadorArquivo = 0;
                                    break;
                                }
                                if (clienteDestino == null)
                                {
                                    contadorArquivo = 0;
                                    break;
                                }
                                if (!clienteDestino.Ativo)
                                {
                                    contadorArquivo = 0;
                                    break;
                                }
                                if (tipoTransacao == "TEF")
                                {
                                    contadorArquivo = 0;
                                    break;
                                }
                                Cliente.CreditarSaldoCliente(valor, clienteDestino);
                                break;
                            }
                        }
                        if (contadorArquivo == 0)
                        {
                            using (StreamWriter sw = arquivoFalharam.AppendText())
                            {
                                sw.WriteLine(s);
                            }
                        }
                        else
                        {
                            using (StreamWriter sw = arquivoRealizadas.AppendText())
                            {
                                sw.WriteLine(s);
                            }
                        }
                    }
                }
                arquivo.Delete();
            }
            return true;
        }

        public static Table GerarRelatorioTransacoesFalharam()
        {
            var pastaTransacoesFalharam = new DirectoryInfo(Transacoes._caminhoTransacoesFalharam);
            if (!pastaTransacoesFalharam.Exists)
            {
                Console.WriteLine("Pasta não existe. Realize o processamento das transações primeiro!");
                return null;
            }
            if (!pastaTransacoesFalharam.EnumerateFiles().Any())
            {
                Console.WriteLine("Não existem arquivos de transações com erros! Considere processar as transações primeiro!");
                return null;
            }
            string[] infoTransacao;
            List<string> erros = new List<string>()
            {
                "Agência Origem inválida",
                "Conta Origem inexistente",
                "Conta Origem inativa",
                "Agência Destino inválida",
                "Conta Destino inexistente",
                "Conta Destino inativa",
                "Tipo de transação incompátivel",
                "Saldo insuficiente",
            };
            int erro;
            var tabela = new Table()
            .Centered()
            .Border(TableBorder.Square)
            .Title("[yellow]RELATÓRIO TRANSAÇÕES COM ERRO[/]")
            .AddColumn(new TableColumn("[magenta]Banco Origem[/]").Centered())
            .AddColumn(new TableColumn("[magenta]Agência Origem[/]").Centered())
            .AddColumn(new TableColumn("[magenta]Conta Origem[/]").Centered())
            .AddColumn(new TableColumn("[magenta]Banco Destino[/]").Centered())
            .AddColumn(new TableColumn("[magenta]Agência Destino[/]").Centered())
            .AddColumn(new TableColumn("[magenta]Conta Destino[/]").Centered())
            .AddColumn(new TableColumn("[magenta]Transação[/]").Centered())
            .AddColumn(new TableColumn("[magenta]Valor[/]").Centered())
            .AddColumn(new TableColumn("[magenta]Data Transação[/]").Centered())
            .AddColumn(new TableColumn("[magenta]Erro[/]").Centered());
            tabela.Expand();
            foreach (FileInfo arquivo in pastaTransacoesFalharam.EnumerateFiles().OrderBy(
                x => DateTime.ParseExact(Path.GetFileNameWithoutExtension(x.Name).Split("-")[Path.GetFileNameWithoutExtension(x.Name).Split("-").Length - 2],
                "yyyyMMdd", CultureInfo.InvariantCulture)))
            {
                using (StreamReader sr = arquivo.OpenText())
                {
                    var dataTransacao = DateTime.ParseExact(Path.GetFileNameWithoutExtension(arquivo.Name).Split("-")[Path.GetFileNameWithoutExtension(arquivo.Name).Split("-").Length - 2],
                        "yyyyMMdd", CultureInfo.InvariantCulture).ToString("d");
                    var s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        infoTransacao = s.Split(",");
                        var bancoOrigem = infoTransacao[0];
                        var agenciaOrigem = infoTransacao[1];
                        var contaOrigem = infoTransacao[2];
                        var bancoDestino = infoTransacao[3];
                        var agenciaDestino = infoTransacao[4];
                        var contaDestino = infoTransacao[5];
                        var tipoTransacao = infoTransacao[6];
                        var valor = Convert.ToDecimal(infoTransacao[7],
                            CultureInfo.CreateSpecificCulture("en-US")).ToString("C2",
                            CultureInfo.CreateSpecificCulture("pt-BR"));
                        while (true)
                        {
                            Cliente? clienteOrigem = RepositorioClientes.ConsultarDadosCliente(contaOrigem);
                            Cliente? clienteDestino = RepositorioClientes.ConsultarDadosCliente(contaDestino);
                            if (bancoOrigem == "777")
                            {
                                if (agenciaOrigem != "0001")
                                {
                                    erro = 0;
                                    break;
                                }
                                if (clienteOrigem == null)
                                {
                                    erro = 1;
                                    break;
                                }
                                if (!clienteOrigem.Ativo)
                                {
                                    erro = 2;
                                    break;
                                }
                                if (tipoTransacao == "TEF" && bancoDestino != "777")
                                {
                                    erro = 6;
                                    break;
                                }
                            }
                            if (bancoDestino == "777")
                            {
                                if (agenciaDestino != "0001")
                                {
                                    erro = 3;
                                    break;
                                }
                                if (clienteDestino == null)
                                {
                                    erro = 4;
                                    break;
                                }
                                if (!clienteDestino.Ativo)
                                {
                                    erro = 5;
                                    break;
                                }
                                if (tipoTransacao == "TEF" && bancoOrigem != "777")
                                {
                                    erro = 6;
                                    break;
                                }
                                if (tipoTransacao != "TEF" && bancoOrigem == "777")
                                {
                                    erro = 6;
                                    break;
                                }
                            }
                            erro = 7;
                            break;
                        }
                        switch (erro)
                        {
                            case 0:
                                tabela.AddRow(bancoOrigem, $"[red]{agenciaOrigem}[/]", (Convert.ToInt32(contaOrigem)).ToString(@"00000\-0"), 
                                    bancoDestino, agenciaDestino, (Convert.ToInt32(contaDestino)).ToString(@"00000\-0"), 
                                    tipoTransacao, valor, dataTransacao, erros[erro]);
                                break;
                            case 1:
                            case 2:
                                tabela.AddRow(bancoOrigem, agenciaOrigem, $"[red]{(Convert.ToInt32(contaOrigem)).ToString(@"00000\-0")}[/]", 
                                    bancoDestino, agenciaDestino, (Convert.ToInt32(contaDestino)).ToString(@"00000\-0"), 
                                    tipoTransacao, valor, dataTransacao, erros[erro]);
                                break;
                            case 3:
                                tabela.AddRow(bancoOrigem, agenciaOrigem, (Convert.ToInt32(contaOrigem)).ToString(@"00000\-0"), 
                                    bancoDestino, $"[red]{agenciaDestino}[/]", (Convert.ToInt32(contaDestino)).ToString(@"00000\-0"), 
                                    tipoTransacao, valor, dataTransacao, erros[erro]);
                                break;
                            case 4:
                            case 5:
                                tabela.AddRow(bancoOrigem, agenciaOrigem, (Convert.ToInt32(contaOrigem)).ToString(@"00000\-0"),
                                    bancoDestino, agenciaDestino, $"[red]{(Convert.ToInt32(contaDestino)).ToString(@"00000\-0")}[/]",
                                    tipoTransacao, valor, dataTransacao, erros[erro]);
                                break;
                            case 6:
                                tabela.AddRow(bancoOrigem, agenciaOrigem, (Convert.ToInt32(contaOrigem)).ToString(@"00000\-0"),
                                    bancoDestino, agenciaDestino, (Convert.ToInt32(contaDestino)).ToString(@"00000\-0"), $"[red]{tipoTransacao}[/]", 
                                    valor, dataTransacao, erros[erro]);
                                break;
                            case 7:
                                tabela.AddRow(bancoOrigem, agenciaOrigem, (Convert.ToInt32(contaOrigem)).ToString(@"00000\-0"),
                                    bancoDestino, agenciaDestino, (Convert.ToInt32(contaDestino)).ToString(@"00000\-0"), 
                                    tipoTransacao, $"[red]{valor}[/]", dataTransacao, erros[erro]);
                                break;
                        }

                    }
                }
            }
            return tabela;
        }

    }
}
