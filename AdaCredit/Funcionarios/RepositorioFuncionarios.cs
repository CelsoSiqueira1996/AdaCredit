using Bogus;
using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Reflection;
using System.Drawing;
using Spectre.Console;

namespace AdaCredit.AdaCredit.Funcionarios
{
    public static class RepositorioFuncionarios
    {
        private static string _caminhoAssembly = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string _caminhoFuncionarios = Path.Combine(_caminhoAssembly, "Funcionarios.csv");
        private static List<Funcionario>? _funcionarios = new List<Funcionario>();

        public static int CarregarArquivoFuncionarios()
        {
            var arquivoFuncionarios = new FileInfo(_caminhoFuncionarios);
            if (!arquivoFuncionarios.Exists)
            {
                using (arquivoFuncionarios.Create()) { }
                return _funcionarios.Count;
            }
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {//Permite dar match com as propriedades que entram no constrututor
                PrepareHeaderForMatch = args => args.Header.ToLower(),
            };
            using (var reader = new StreamReader(_caminhoFuncionarios))
            using (var csv = new CsvReader(reader, config))
            {
                _funcionarios = csv.GetRecords<Funcionario>().ToList();
            }
            return _funcionarios.Count;
        }

        public static void AtualizarArquivoFuncionarios()
        {
            using (var writer = new StreamWriter(_caminhoFuncionarios))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(_funcionarios);
            }
        }

        public static void CadastrarFuncionario(string nome, DateTime dataNascimento, long cpf, long telefone,
            string email, long cep, string senha, string nomeUsuario)
        {
            _funcionarios.Add(Funcionario.CadastrarFuncionario(nome, dataNascimento, cpf, telefone, email, cep, nomeUsuario, senha));
            AtualizarArquivoFuncionarios();
            return;
        }

        public static void CadastrarFuncionarioFaker(string nome, DateTime dataNascimento, long telefone, 
            string email, long cep, string senha, string dataUltimoLogin)
        {
            long cpf = Convert.ToInt64(new Faker().Random.Replace("###########"));
            while(_funcionarios.Any(x => x.CPF == cpf))
            {
                cpf = Convert.ToInt64(new Faker().Random.Replace("###########"));
            }
            var nomeUsuario = new Faker().Random.String2(new Faker().Random.Int(5, 10), nome);
            while (_funcionarios.Any(x => x.NomeUsuario == nomeUsuario))
            {
                nomeUsuario = new Faker().Random.String2(new Faker().Random.Int(5, 10), nome);
            }
                _funcionarios.Add(Funcionario.CadastrarFuncionarioFaker(nome, dataNascimento, cpf, telefone, email, 
                    cep, nomeUsuario, senha, dataUltimoLogin));
            return;
        }

        public static Funcionario? ConsultarDadosFuncionario(long cpf)
        {
            return _funcionarios.FirstOrDefault(x => x.CPF == cpf && x.Ativo == true);
        }

        public static bool ConsultarDadosFuncionario(string nomeUsuario)
        {
            return _funcionarios.Any(x => x.NomeUsuario == nomeUsuario);
        }

        public static Funcionario? MatchLogin(string nomeUsuario)
        {
            return _funcionarios.FirstOrDefault(x => x.NomeUsuario == nomeUsuario);
        }

        public static int QuantidadeCadastrosAtivos()
        {
            return _funcionarios.Count(x => x.Ativo == true);
        }

        public static void AtualizarUltimoLogin(string usuarioAtual, string dataUltimoLogin)
        {
            Funcionario funcionario = _funcionarios.FirstOrDefault(x => x.NomeUsuario == usuarioAtual);
            Funcionario.AtualizarUltimoLogin(funcionario, dataUltimoLogin);
            return;
        }

        public static Table GerarRelatorioFuncionarios()
        {
            var tabela = new Table()
            .Centered()
            .Border(TableBorder.Square)
            .Title("[yellow]RELATÓRIO FUNCIONÁRIOS ATIVOS[/]")
            .AddColumn(new TableColumn("[magenta]Nome[/]").Centered())
            .AddColumn(new TableColumn("[magenta]Data de Nascimento[/]").Centered())
            .AddColumn(new TableColumn("[magenta]CPF[/]").Centered())
            .AddColumn(new TableColumn("[magenta]Telefone[/]").Centered())
            .AddColumn(new TableColumn("[magenta]E-mail[/]").Centered())
            .AddColumn(new TableColumn("[magenta]CEP[/]").Centered())
            .AddColumn(new TableColumn("[magenta]Nome de Usuário[/]").Centered())
            .AddColumn(new TableColumn("[magenta]Data do Último Login[/]").Centered());
            tabela.Expand();
            foreach (Funcionario funcionario in _funcionarios.OrderBy(x => x.Nome))
            {
                if (!funcionario.Ativo!)
                {
                    continue;
                }
                tabela.AddRow(funcionario.Nome == "" ? "------" : funcionario.Nome,
                    funcionario.DataNascimento.ToString("d"),
                    funcionario.CPF.ToString(@"000\.000\.000\-00"),
                    funcionario.Telefone.ToString(),
                    funcionario.Email == "" ? "------" : funcionario.Email,
                    funcionario.CEP.ToString(@"00\.000\-000"),
                    funcionario.NomeUsuario,
                    funcionario.DataUltimoLogin == "" ? "------" : funcionario.DataUltimoLogin);
            }
            return tabela;
        }
    }
}

