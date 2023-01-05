using Bogus;
using Microsoft.VisualBasic;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaCredit.AdaCredit.Clientes
{
    public sealed class Cliente
    {
        public string? Nome { get; private set; }
        public DateTime DataNascimento { get; private set; }
        public long CPF { get; private set; }
        public long Telefone { get; private set; }
        public string? Email { get; private set; }
        public long CEP { get; private set; }
        public decimal Saldo { get; private set; }
        public int NumeroConta { get; private set; }
        public bool Ativo { get; private set; }

        public Cliente(string nome, DateTime dataNascimento, long cpf, long telefone,
            string email, long cep, int numeroConta, decimal saldo, bool ativo)
        {
            Nome = nome;
            DataNascimento = dataNascimento;
            CPF = cpf;
            Telefone = telefone;
            Email = email;
            CEP = cep;
            NumeroConta = numeroConta;
            Saldo = saldo;
            Ativo = ativo;
        }

        public static Cliente CadastrarCliente(string nome, DateTime dataNascimento, long cpf, long telefone,
            string email, long cep, int numeroConta)
        {
            decimal saldo = 0;
            bool status = true;
            return new Cliente(nome, dataNascimento, cpf, telefone, email, cep, numeroConta, saldo, status);
        }

        public static Cliente CadastrarClienteFaker(string nome, DateTime dataNascimento, long cpf, long telefone,
    string email, long cep, int numeroConta, decimal saldo, bool status)
        {
            return new Cliente(nome, dataNascimento, cpf, telefone, email, cep, numeroConta, saldo, status);
        }

        public static void AtivarConta(Cliente cliente)
        {
            cliente.Ativo = true;
            RepositorioClientes.AtualizarArquivoClientes();
            return;
        }

        public static void AlterarCadastroCliente(Cliente cliente, string[] dadosParaAlterar, string nome, long telefone, string email, long cep, DateTime dataNascimento)
        {
            cliente.Nome = dadosParaAlterar.Any(x => x == "Nome") ? nome : cliente.Nome;
            cliente.Telefone = dadosParaAlterar.Any(x => x == "Telefone") ? telefone : cliente.Telefone;
            cliente.Email = dadosParaAlterar.Any(x => x == "E-mail") ? email : cliente.Email;
            cliente.CEP = dadosParaAlterar.Any(x => x == "CEP") ? cep : cliente.CEP;
            cliente.DataNascimento = dadosParaAlterar.Any(x => x == "Data de Nascimento") ? dataNascimento : cliente.DataNascimento;
            RepositorioClientes.AtualizarArquivoClientes();
            return;
        }

        public static void DesativarCadastroCliente(Cliente cliente)
        {
            cliente.Ativo = false;
            RepositorioClientes.AtualizarArquivoClientes();
            return;
        }

        public static void CreditarSaldoCliente(decimal valor, Cliente cliente)
        {
            cliente.Saldo += valor;
            RepositorioClientes.AtualizarArquivoClientes();
            return;
        }

        public static void DebitarSaldoCliente(decimal valor, Cliente cliente)
        {
            cliente.Saldo -= valor;
            RepositorioClientes.AtualizarArquivoClientes();
            return;
        }

        public static Table TabelaDadosCliente(Cliente cliente)
        {
            Table tabela = new Table()
                .LeftAligned()
                .Border(TableBorder.Rounded)
                .Title("[yellow]DADOS DO CLIENTE[/]")
                .AddColumn(new TableColumn("Coluna0").LeftAligned())
                .AddColumn(new TableColumn("Coluna1").LeftAligned())
                .AddRow("[green]Nome[/]", $"[cyan]{cliente.Nome}[/]")
                .AddRow("[green]Data de Nascimento[/]", $"[cyan]{cliente.DataNascimento.ToString("d")}[/]")
                .AddRow("[green]CPF[/]", $"[cyan]{cliente.CPF.ToString(@"000\.000\.000\-00")}[/]")
                .AddRow("[green]Telefone[/]", $"[cyan]{cliente.Telefone}[/]")
                .AddRow("[green]E-mail[/]", $"[cyan]{cliente.Email}[/]")
                .AddRow("[green]CEP[/]", $"[cyan]{cliente.CEP.ToString(@"00\.000\-000")}[/]")
                .AddRow("[green]Número da Conta[/]", $"[cyan]{cliente.NumeroConta.ToString(@"00000\-0")}[/]")
                .AddRow("[green]Status da Conta[/]", $"[cyan]{(cliente.Ativo ? "Ativa" : "Inativa")}[/]")
                .AddRow("[green]Saldo[/]", $"[cyan]{cliente.Saldo.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"))}[/]")
                .HideHeaders();
            return tabela;
        }
    }
}
