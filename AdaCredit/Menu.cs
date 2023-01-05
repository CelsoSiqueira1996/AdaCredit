using AdaCredit.AdaCredit.Clientes;
using AdaCredit.AdaCredit.Funcionarios;
using AdaCredit.AdaCredit.Transacoes;
using ConsoleTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaCredit
{
    public static class Menu
    {
        public static void Show()
        {
            var subMenuClientes = new ConsoleMenu()
                .Add("Cadastrar um Novo Cliente", () => InfoCliente.CadastrarCliente())
                .Add("Consultar Dados de Cliente", () => InfoCliente.ConsultarDadosCliente())
                .Add("Alterar Cadastro de Cliente", () => InfoCliente.AlterarCadastroCliente())
                .Add("Desativar Cadastro de Cliente", () => InfoCliente.DesativarCadastroCliente())
                .Add("Voltar", ConsoleMenu.Close)
                .Add("Sair", () => Environment.Exit(0))
                .Configure(config =>
                {
                    config.Title = "Clientes";
                    config.Selector = "--> ";
                    config.EnableBreadcrumb = true;
                    config.WriteBreadcrumbAction = titles => Console.WriteLine($">>> Usuário logado: {Login._usuarioAtual} <<<\n\n {string.Join(" / ", titles)}");
                    config.SelectedItemBackgroundColor = ConsoleColor.White;
                    config.SelectedItemForegroundColor = ConsoleColor.Black;
                });

            var subMenuFuncionarios = new ConsoleMenu()
                .Add("Cadastrar um Novo Funcionário", () => InfoFuncionario.CadastrarFuncionario())
                .Add("Alterar Senha de Funcionário", () => InfoFuncionario.AlterarSenhaFuncionario())
                .Add("Desativar Cadastro de Funcionário", () => InfoFuncionario.DesativarCadastroFuncionario())
                .Add("Voltar", ConsoleMenu.Close)
                .Add("Sair", () => Environment.Exit(0))
                .Configure(config =>
                {
                    config.Title = "Funcionarios";
                    config.Selector = "--> ";
                    config.EnableBreadcrumb = true;
                    config.WriteBreadcrumbAction = titles => Console.WriteLine($">>> Usuário logado: {Login._usuarioAtual} <<<\n\n {string.Join(" / ", titles)}");
                    config.SelectedItemBackgroundColor = ConsoleColor.White;
                    config.SelectedItemForegroundColor = ConsoleColor.Black;
                });

            var subMenuTransacoes = new ConsoleMenu()
                .Add("Processar Transações", () => InfoTransacoes.ProcessarTransacoes())
                .Add("Voltar", ConsoleMenu.Close)
                .Add("Sair", () => Environment.Exit(0))
                .Configure(config =>
                {
                    config.Title = "Transações";
                    config.Selector = "--> ";
                    config.EnableBreadcrumb = true;
                    config.WriteBreadcrumbAction = titles => Console.WriteLine($">>> Usuário logado: {Login._usuarioAtual} <<<\n\n {string.Join(" / ", titles)}");
                    config.SelectedItemBackgroundColor = ConsoleColor.White;
                    config.SelectedItemForegroundColor = ConsoleColor.Black;
                });

            var subMenuRelatorios = new ConsoleMenu()
                .Add("Exibir Todos os Clientes Ativos", () => InfoCliente.GerarRelatorioClientesAtivos())
                .Add("Exibir Todos os Clientes Inativos", () => InfoCliente.GerarRelatorioClientesInativos())
                .Add("Exibir Todos os Funcionários Ativos", () => InfoFuncionario.GerarRelatorioFuncionarios())
                .Add("Exibir Transações com Erro", () => InfoTransacoes.RelatorioTransacoes())
                .Add("Voltar", ConsoleMenu.Close)
                .Add("Sair", () => Environment.Exit(0))
                .Configure(config =>
                {
                    config.Title = "Relatórios";
                    config.Selector = "--> ";
                    config.EnableBreadcrumb = true;
                    config.WriteBreadcrumbAction = titles => Console.WriteLine($">>> Usuário logado: {Login._usuarioAtual} <<<\n\n {string.Join(" / ", titles)}");
                    config.SelectedItemBackgroundColor = ConsoleColor.White;
                    config.SelectedItemForegroundColor = ConsoleColor.Black;
                });

            var menu = new ConsoleMenu()
              .Add("Clientes", subMenuClientes.Show)
              .Add("Funcionários", subMenuFuncionarios.Show)
              .Add("Transações", subMenuTransacoes.Show)
              .Add("Relatórios", subMenuRelatorios.Show)
              .Add("Sair", () => Environment.Exit(0))
              .Configure(config =>
              {
                  config.Selector = "--> ";
                  config.Title = "Menu principal";
                  config.EnableWriteTitle = false;
                  config.EnableBreadcrumb = true;
                  config.WriteBreadcrumbAction = titles => Console.WriteLine($">>> Usuário logado: {Login._usuarioAtual} <<<\n\n {string.Join(" / ", titles)}");
                  config.SelectedItemBackgroundColor = ConsoleColor.White;
                  config.SelectedItemForegroundColor = ConsoleColor.Black;
              });
            menu.Show();
        }
    }
}
