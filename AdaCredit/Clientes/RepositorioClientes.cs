using Bogus;
using CsvHelper;
using CsvHelper.Configuration;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AdaCredit.AdaCredit.Clientes
{
    //Contas desativadas não podem ser utilizadas por outros clientes
    //Contas desativadas podem ser reativadas para o mesmo cliente
    //Qualquer atividade que envolva alterar dados de clientes ou adicionar novos atualiza o arquivo Cliente.txt,
    //a ideia é evitar do usuário fechar o console sem que as alterações tenham sido salvas.
    public static class RepositorioClientes
    {
        private static string _caminhoAssembly = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string _caminhoClientes = Path.Combine(_caminhoAssembly, "Clientes.csv");
        private static int _contadorArquivoGerado = 0;
        private static List<Cliente>? _clientes = new List<Cliente>();

        public static void CarregarArquivoClientes()
        {
            _contadorArquivoGerado = 1;
            var arquivoClientes = new FileInfo(_caminhoClientes);
            if (!arquivoClientes.Exists)
            {
                using (arquivoClientes.Create()) { }
                return;
            }
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {//Permite dar match com as propriedades que entram no constrututor
                PrepareHeaderForMatch = args => args.Header.ToLower(),
            };
            using (var reader = new StreamReader(_caminhoClientes))
            using (var csv = new CsvReader(reader, config))
            {
                _clientes = csv.GetRecords<Cliente>().ToList();
            }
            return;
        }

        public static void AtualizarArquivoClientes()
        {
            using (var writer = new StreamWriter(_caminhoClientes))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(_clientes);
            }
        }

        public static int CadastrarCliente(string nome, DateTime dataNascimento, long cpf, long telefone,
            string email, long cep)
        {
            if (_contadorArquivoGerado == 0)
            {
                CarregarArquivoClientes();
            }
            var numeroConta = Convert.ToInt32(new Faker().Random.Replace("######"));
            while (_clientes.Any(x => x.NumeroConta == numeroConta))
            {
                numeroConta = Convert.ToInt32(new Faker().Random.Replace("######"));
            }
            Cliente cliente = Cliente.CadastrarCliente(nome, dataNascimento, cpf, telefone, email, cep, numeroConta);
            _clientes.Add(cliente);
            AtualizarArquivoClientes();
            return cliente.NumeroConta;
        }

        public static void CadastrarClienteFaker(string nome, DateTime dataNascimento, long telefone,
    string email, long cep, decimal saldo, bool status)
        {
            if (_contadorArquivoGerado == 0)
            {
                CarregarArquivoClientes();
            }
            long cpf = Convert.ToInt64(new Faker().Random.Replace("###########"));
            while (_clientes.Any(x => x.CPF == cpf))
            {
                cpf = Convert.ToInt64(new Faker().Random.Replace("###########"));
            }
            var numeroConta = Convert.ToInt32(new Faker().Random.Replace("######"));
            while (_clientes.Any(x => x.NumeroConta == numeroConta))
            {
                numeroConta = Convert.ToInt32(new Faker().Random.Replace("######"));
            }
            _clientes.Add(Cliente.CadastrarClienteFaker(nome, dataNascimento, cpf, telefone, email, cep, numeroConta,
                saldo, status));
            return;
        }

        public static Cliente? ConsultarDadosCliente(long cpf)
        {
            if (_contadorArquivoGerado == 0)
            {
                CarregarArquivoClientes();
            }
            return _clientes.FirstOrDefault(x => x.CPF == cpf);
        }

        public static Cliente? ConsultarDadosCliente(string numeroConta)
        {
            if (_contadorArquivoGerado == 0)
            {
                CarregarArquivoClientes();
            }
            return _clientes.FirstOrDefault(x => x.NumeroConta == Convert.ToInt32(numeroConta));
        }

        public static Table GerarRelatorioClientesAtivos()
        {
            if (_contadorArquivoGerado == 0)
            {
                CarregarArquivoClientes();
            }
            var tabela = new Table()
            .Centered()
            .Border(TableBorder.Square)
            .Title("[yellow]RELATÓRIO CLIENTES ATIVOS[/]")
            .AddColumn(new TableColumn("[magenta]Nome[/]").Centered())
            .AddColumn(new TableColumn("[magenta]Data de Nascimento[/]").Centered())
            .AddColumn(new TableColumn("[magenta]CPF[/]").Centered())
            .AddColumn(new TableColumn("[magenta]Telefone[/]").Centered())
            .AddColumn(new TableColumn("[magenta]E-mail[/]").Centered())
            .AddColumn(new TableColumn("[magenta]CEP[/]").Centered())
            .AddColumn(new TableColumn("[magenta]Agência/Conta[/]").Centered())
            .AddColumn(new TableColumn("[magenta]Saldo[/]").Centered());
            tabela.Expand();
            foreach (Cliente cliente in _clientes.OrderBy(x => x.Nome))
            {
                if (!cliente.Ativo)
                {
                    continue;
                }
                tabela.AddRow(cliente.Nome == "" ? "------" : cliente.Nome,
                    cliente.DataNascimento.ToString("d"),
                    cliente.CPF.ToString(@"000\.000\.000\-00"),
                    cliente.Telefone.ToString(),
                    cliente.Email == "" ? "------" : cliente.Email,
                    cliente.CEP.ToString(@"00\.000\-000"),
                    $"0001/{cliente.NumeroConta.ToString(@"00000\-0")}",
                    cliente.Saldo.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR")));
            }
            return tabela;
        }

        public static Table GerarRelatorioClientesInativos()
        {
            if (_contadorArquivoGerado == 0)
            {
                CarregarArquivoClientes();
            }
            var tabela = new Table()
            .Centered()
            .Border(TableBorder.Square)
            .Title("[yellow]RELATÓRIO CLIENTES INATIVOS[/]")
            .AddColumn(new TableColumn("[magenta]Nome[/]").Centered())
            .AddColumn(new TableColumn("[magenta]Data de Nascimento[/]").Centered())
            .AddColumn(new TableColumn("[magenta]CPF[/]").Centered())
            .AddColumn(new TableColumn("[magenta]Telefone[/]").Centered())
            .AddColumn(new TableColumn("[magenta]E-mail[/]").Centered())
            .AddColumn(new TableColumn("[magenta]CEP[/]").Centered())
            .AddColumn(new TableColumn("[magenta]Agência/Conta[/]").Centered())
            .AddColumn(new TableColumn("[magenta]Saldo[/]").Centered());
            tabela.Expand();
            foreach (Cliente cliente in _clientes.OrderBy(x => x.Nome))
            {
                if (cliente.Ativo)
                {
                    continue;
                }
                tabela.AddRow(cliente.Nome == "" ? "------" : cliente.Nome,
                    cliente.DataNascimento.ToString("d"),
                    cliente.CPF.ToString(@"000\.000\.000\-00"),
                    cliente.Telefone.ToString(),
                    cliente.Email == "" ? "------" : cliente.Email,
                    cliente.CEP.ToString(@"00\.000\-000"),
                    $"0001/{cliente.NumeroConta.ToString(@"00000\-0")}",
                    cliente.Saldo.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR")));
            }
            return tabela;
        }

        public static List<string[]> GerarTransacoesFaker()
        {
            List<string[]> transacoesPendentes = new List<string[]>();
            Dictionary<string, string> bancos = new Dictionary<string, string>()
            {
                {"777", "AdaCredit"},
                {"888", "BancoA"},
                {"999", "BancoB"},
                {"111", "BancoC"},
                {"222", "BancoD"}
            };
            for (int i = 0; i < 200; i++)
            {
                var bancoOrigem = new Faker().PickRandom("777", "888", "999", "111", "222");
                var agenciaOrigem = new Faker().PickRandom("0001", "0002");
                string contaOrigem;
                string bancoDestino;
                var agenciaDestino = new Faker().PickRandom("0001", "0002");
                string contaDestino;
                var tipoTransacao = new Faker().PickRandom("TED", "TEF", "DOC");
                var valor = new Faker().Random.Decimal(200, 2000).ToString("F2", CultureInfo.CreateSpecificCulture("en-US"));
                string bancoParceiro;
                var dataTransacao = new Faker().PickRandom("20221129", "20221130", "20221201", "20221202");
                if (bancoOrigem != "777")
                {
                    bancoParceiro = bancos[bancoOrigem];
                    contaOrigem = new Faker().Random.Replace("######");
                    bancoDestino = "777";
                    contaDestino = new Faker().PickRandom(_clientes.Select(x => x.NumeroConta).ToArray()).ToString();
                }
                else
                {
                    contaOrigem = new Faker().PickRandom(_clientes.Select(x => x.NumeroConta).ToArray()).ToString();
                    bancoDestino = new Faker().PickRandom("777", "888", "999", "111", "222");
                    bancoParceiro = bancos[bancoDestino];
                    if (bancoDestino == "777")
                    {
                        contaDestino = new Faker().PickRandom(_clientes.Select(x => x.NumeroConta).ToArray()).ToString();
                    }
                    else
                    {
                        contaDestino = new Faker().Random.Replace("######");
                    }

                }
                var nomeArquivo = String.Concat(bancoParceiro, "-", dataTransacao, ".csv");
                string[] linha = new string[2]
                {
                    String.Concat(bancoOrigem, ",", agenciaOrigem, ",", contaOrigem, ",", bancoDestino,
                    ",", agenciaDestino, ",", contaDestino, ",", tipoTransacao, ",", valor),
                    nomeArquivo
                };
                transacoesPendentes.Add(linha);
            }
            return transacoesPendentes;
        }
    }
}
