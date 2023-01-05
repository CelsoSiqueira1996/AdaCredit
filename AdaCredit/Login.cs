using GetPass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using static BCrypt.Net.BCrypt;
using AdaCredit.AdaCredit.Clientes;
using AdaCredit.AdaCredit.Funcionarios;
using Spectre.Console;
using AdaCredit.AdaCredit.Transacoes;

namespace AdaCredit
{
    public static class Login
    {
        public static string? _usuarioAtual;
        public static string? _dataUltimoLogin;
        public static void FazerLogin()
        {
            Console.WriteLine("--- Bem-vinda(o) ao AdaCredit! ---\n");
            if (RepositorioFuncionarios.CarregarArquivoFuncionarios() == 0)
            {
                Console.Write("Nome de Usuário: ");
                var nomeUsuario = Console.ReadLine();
                while (nomeUsuario != "user")
                {
                    Console.WriteLine("Nome de usuário incorreto!");
                    Console.WriteLine("Este é o primeiro login, entre com usuário e senha iniciais!");
                    Console.WriteLine("Tente novamente!\n");
                    Console.Write("Nome de Usuário: ");
                    nomeUsuario = Console.ReadLine();
                }
                while (ConsolePasswordReader.Read("Senha: ") != "pass")
                {
                    Console.WriteLine("Senha incorreta!");
                    Console.WriteLine("Este é o primeiro login, entre com usuário e senha iniciais!\n");
                    Console.WriteLine("Tente novamente.\n");
                }
                Console.WriteLine("\n...login efetuado com sucesso...");
                Console.WriteLine("\nVocê está sendo redirecionado para o cadastro de funcionários, onde deverá criar seu cadastro.");
                Console.WriteLine("Pressione qualquer tecla para ser direcionada(o) ao cadastramento de funcionários.");
                Console.ReadKey();
                _usuarioAtual = InfoFuncionario.CadastrarFuncionario();
                Console.Clear();
                Console.WriteLine($">>> Usuário logado: {Login._usuarioAtual} <<<\n");
                Console.WriteLine("Aguarde enquanto o sistema gera os dados faker(este procedimento só ocorre no primeiro login):\n");
                AnsiConsole.Status()
                    .Start("Gerando dados clientes...", ctx =>
                    {
                        ClientesFaker.GerarClientesFaker();
                        AnsiConsole.MarkupLine("Dados clientes gerados");
                        ctx.Status("Gerando dados funcionários...");
                        FuncionariosFaker.GerarFuncionariosFaker();
                        AnsiConsole.MarkupLine("Dados funcionários gerados");
                        ctx.Status("Gerando transações pendentes...");
                        TransacoesFaker.GerarArquivosFaker();
                        ctx.Spinner(Spinner.Known.Star);
                        ctx.SpinnerStyle(Style.Parse("green"));
                        AnsiConsole.MarkupLine("Transações geradas");
                    });
            }
            else
            {
                Console.Write("Nome de Usuário: ");
                var nomeUsuario = Console.ReadLine();
                Funcionario? funcionario = RepositorioFuncionarios.MatchLogin(nomeUsuario);
                while (funcionario == null || !funcionario.Ativo)
                {
                    if (funcionario == null)
                    {
                        Console.WriteLine("Nome de usuário não existe!");
                    }
                    else
                    {
                        Console.WriteLine("Usuário desativado! Não tem mais acesso ao sistema!");
                    }
                    Console.WriteLine("Tente novamente com outro nome de usuário!\n");
                    Console.Write("Nome de Usuário: ");
                    nomeUsuario = Console.ReadLine();
                    funcionario = RepositorioFuncionarios.MatchLogin(nomeUsuario);
                }
                while (!Verify(ConsolePasswordReader.Read("Senha: "), funcionario.SenhaHash))
                {
                    Console.WriteLine("Senha incorreta!");
                    Console.WriteLine("Tente novamente!\n");
                }
                Console.WriteLine("\n...login efetuado com sucesso...");
                Console.WriteLine("\nPressione qualquer tecla para ir ao menu principal.");
                Console.ReadKey();
                _usuarioAtual = funcionario.NomeUsuario;
            }
            _dataUltimoLogin = DateTime.Now.ToString();
            RepositorioFuncionarios.AtualizarUltimoLogin(_usuarioAtual, _dataUltimoLogin);
            RepositorioFuncionarios.AtualizarArquivoFuncionarios();
            Menu.Show();
            return;
        }
    }
}
