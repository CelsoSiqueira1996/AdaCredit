using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using static BCrypt.Net.BCrypt;
using GetPass;
using Sharprompt;
using Spectre.Console;

namespace AdaCredit.AdaCredit.Funcionarios
{
    public static class InfoFuncionario
    {
        public static string CadastrarFuncionario()
        {
            Console.Clear();
            Console.WriteLine($">>> Usuário logado: {Login._usuarioAtual} <<<\n");
            Console.WriteLine("--- Cadastrar Novo Funcionário ---\n");
            long cpf = 0;
            long cpfMatch = 1;
            string nomeUsuario;
            do
            {
                Console.Write("CPF do funcionário (somente dígitos): ");
                string? verificadorTamanhoCPF = Console.ReadLine();
                //É esperado que o CPF tenha 11 dígitos. A verificação aceita 0 à esquerda.
                while (verificadorTamanhoCPF.Length != 11 || !long.TryParse(verificadorTamanhoCPF, out cpf))
                {
                    Console.Write("CPF inválido. Preencha novamente (somente dígitos): ");
                    verificadorTamanhoCPF = Console.ReadLine();
                }
                Console.WriteLine("\n***ATENÇÃO: UMA VEZ CONFIRMADO O CPF, O MESMO NÃO PODERÁ SER ALTERADO!***");
                Console.Write("Digite o CPF novamente para confirmar: ");
                long.TryParse(Console.ReadLine(), out cpfMatch);
                if (cpf != cpfMatch)
                {
                    Console.WriteLine("\nCPFs diferem. Tente novamente!");
                }
            } while (cpf != cpfMatch);
            Console.Clear();
            Console.WriteLine($">>> Usuário logado: {Login._usuarioAtual} <<<\n");
            Console.WriteLine("--- Cadastrar Novo Funcionário ---\n");
            Console.WriteLine($"CPF do funcionário: {cpf.ToString(@"000\.000\.000\-00")}");
            Funcionario? funcionario = RepositorioFuncionarios.ConsultarDadosFuncionario(cpf);
            if (funcionario == null)
            {
                nomeUsuario = Prompt.Input<string>("Nome de Usuário: ", validators: new[]
                { Validators.Required(),
                  Validators.MinLength(5),
                  Validators.MaxLength(10)
                });
                //Os logins não podem ser iguais, mesmo que seja de um cadastro desativado
                while (RepositorioFuncionarios.ConsultarDadosFuncionario(nomeUsuario))
                {
                    Console.WriteLine("\nNome de usuário já existe. Informe outro: ");
                    nomeUsuario = Prompt.Input<string>("Nome de Usuário: ", validators: new[]
                    { Validators.Required(),
                      Validators.MinLength(5),
                      Validators.MaxLength(10)
                    });
                }
                Console.Clear();
                Console.WriteLine($">>> Usuário logado: {Login._usuarioAtual} <<<\n");
                Console.WriteLine("--- Cadastrar Novo Funcionário ---\n");
                Console.WriteLine($"CPF do funcionário: {cpf.ToString(@"000\.000\.000\-00")}");
                Console.WriteLine($"Nome de Usuário: {nomeUsuario}");
                Console.Write("Nome do funcionário: ");
                var nome = Console.ReadLine();
                Console.Write("Data de nascimento do funcionário (dd/mm/aaaa): ");
                DateTime dataNascimento;
                while (!DateTime.TryParseExact(Console.ReadLine(), "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataNascimento))
                {
                    Console.Write("Data inválida. Preencha novamente (dd/mm/aaaa): ");
                }
                Console.Write("Telefone do funcionário (somente dígitos): ");
                long telefone;
                while (!long.TryParse(Console.ReadLine(), out telefone))
                {
                    Console.Write("Telefone inválido. Preencha novamente (somente dígitos): ");
                }
                Console.Write("E-mail do funcionário: ");
                var email = Console.ReadLine();
                Console.Write("CEP do funcionário (somente dígitos): ");
                long cep;
                string? verificadorTamanhoCEP = Console.ReadLine();
                //É esperado que o CEP tenha 8 dígitos. A verificação aceita 0 à esquerda.
                while (verificadorTamanhoCEP.Length != 8 || !long.TryParse(verificadorTamanhoCEP, out cep))
                {
                    Console.Write("CEP inválido. Preencha novamente (somente dígitos): ");
                    verificadorTamanhoCEP = Console.ReadLine();
                }
                var senha = Prompt.Password("Senha: ", validators: new[]
                    { Validators.Required(),
                      Validators.MinLength(5),
                      Validators.MaxLength(20)
                    });
                while (senha != ConsolePasswordReader.Read("Confirmar senha: "))
                {
                    Console.WriteLine("Senhas diferem! Tente novamente.");
                    senha = Prompt.Password("Senha: ", validators: new[]
                    { Validators.Required(),
                      Validators.MinLength(5),
                      Validators.MaxLength(10)
                    });
                }
                RepositorioFuncionarios.CadastrarFuncionario(nome, dataNascimento, cpf, telefone, email, cep, senha, nomeUsuario);
                Console.WriteLine("\nCadastramento realizado com sucesso.");
            }
            else
            {
                nomeUsuario = funcionario.NomeUsuario;
                Console.WriteLine($"Nome: {funcionario.Nome}");
                Console.WriteLine($"Nome de Usuário: {nomeUsuario}");
                Console.WriteLine($"\nFuncionário já possui cadastro ativo.");
            }
            Console.WriteLine("\n... operação finalizada ...");
            Console.WriteLine("\nPressione qualquer tecla para voltar ao menu anterior.");
            Console.ReadKey();
            return nomeUsuario;
        }

        public static void AlterarSenhaFuncionario()
        {
            Console.Clear();
            Console.WriteLine($">>> Usuário logado: {Login._usuarioAtual} <<<\n");
            Console.WriteLine("--- Alterar Senha de Funcionário ---\n");
            Console.Write("Número do CPF do funcionário (somente dígitos): ");
            long cpf;
            string? verificadorTamanhoCPF = Console.ReadLine();
            //É esperado que o CPF tenha 11 dígitos. A verificação aceita 0 à esquerda.
            while (verificadorTamanhoCPF.Length != 11 || !long.TryParse(verificadorTamanhoCPF, out cpf))
            {
                Console.Write("CPF inválido. Preencha novamente (somente dígitos): ");
                verificadorTamanhoCPF = Console.ReadLine();
            }
            Funcionario? funcionario = RepositorioFuncionarios.ConsultarDadosFuncionario(cpf);
            if (funcionario == null)
            {
                Console.WriteLine("\nFuncionário não encontrado!");
            }
            else
            {
                while (!Verify(ConsolePasswordReader.Read("Senha atual: "), funcionario.SenhaHash))
                {
                    Console.WriteLine("Senha incorreta! Tente novamente.");
                }
                Console.Clear();
                Console.WriteLine($">>> Usuário logado: {Login._usuarioAtual} <<<\n");
                Console.WriteLine("--- Alterar Senha de Funcionário ---\n");
                Console.WriteLine($"CPF do funcionário: {cpf.ToString(@"000\.000\.000\-00")}");
                Console.WriteLine($"Nome de Usuário: {funcionario.NomeUsuario}");
                var senha = Prompt.Password("Informe a nova senha: ", validators: new[]
                    { Validators.Required(),
                      Validators.MinLength(5),
                      Validators.MaxLength(10)
                    });
                while (ConsolePasswordReader.Read("Confirmar senha: ") != senha)
                {
                    Console.WriteLine("Senhas diferem! Tente novamente.");
                    senha = Prompt.Password("Informe a nova senha: ", validators: new[]
                    { Validators.Required(),
                      Validators.MinLength(5),
                      Validators.MaxLength(10)
                    });
                }
                Funcionario.AlterarSenhaFuncionario(funcionario, senha);
                Console.WriteLine("\nSenha alterada com sucesso.");
            }
            Console.WriteLine("\n... operação finalizada ...");
            Console.WriteLine("\nPressione qualquer tecla para voltar ao menu anterior.");
            Console.ReadKey();
            return;
        }

        public static void DesativarCadastroFuncionario()
        {
            Console.Clear();
            Console.WriteLine($">>> Usuário logado: {Login._usuarioAtual} <<<\n");
            Console.WriteLine("--- Desativar Cadastro de Funcionário ---\n");
            if (RepositorioFuncionarios.QuantidadeCadastrosAtivos() == 1)
            {
                Console.WriteLine("***OPERAÇÃO INDISPONÍVEL***");
                Console.WriteLine("#ERRO: APENAS UM FUNCIONÁRIO ATIVO!#");
                Console.WriteLine("Não é possível desativar cadastro!");
            }
            else
            {
                long cpf = 0;
                long cpfMatch = 1;
                do
                {
                    Console.Write("CPF do funcionário (somente dígitos): ");
                    string? verificadorTamanhoCPF = Console.ReadLine();
                    //É esperado que o CPF tenha 11 dígitos. A verificação aceita 0 à esquerda.
                    while (verificadorTamanhoCPF.Length != 11 || !long.TryParse(verificadorTamanhoCPF, out cpf))
                    {
                        Console.Write("CPF inválido. Preencha novamente (somente dígitos): ");
                        verificadorTamanhoCPF = Console.ReadLine();
                    }
                    Console.WriteLine("\n***ATENÇÃO: UMA VEZ CONFIRMADO O CPF, O CADASTRO DO FUNCIONÁRIO SERÁ DESATIVADO!***");
                    Console.Write("Digite o CPF novamente para confirmar: ");
                    long.TryParse(Console.ReadLine(), out cpfMatch);
                    if (cpf != cpfMatch)
                    {
                        Console.WriteLine("\nCPFs diferem. Tente novamente!");
                    }
                } while (cpf != cpfMatch);
                Console.Clear();
                Console.WriteLine($">>> Usuário logado: {Login._usuarioAtual} <<<\n");
                Console.WriteLine("--- Desativar Cadastro de Funcionário ---\n");
                Console.WriteLine($"CPF do funcionário: {cpf.ToString(@"000\.000\.000\-00")}");
                Funcionario? funcionario = RepositorioFuncionarios.ConsultarDadosFuncionario(cpf);
                if (funcionario == null)
                {
                    Console.WriteLine("\nFuncionário não encontrado (não existe ou o cadastro já está desativado!");
                }
                else
                {
                    Console.WriteLine($"Nome de usuário: {funcionario.NomeUsuario}");
                    Funcionario.DesativarCadastroFuncionario(funcionario);
                    Console.WriteLine("\nCadastro desativado com sucesso.");
                    if (funcionario.NomeUsuario == Login._usuarioAtual)
                    {
                        Console.WriteLine("\n... operação finalizada ...");
                        Console.WriteLine("\nPressione qualquer tecla para sair.");
                        Console.ReadKey();
                        Console.Clear();
                        Console.WriteLine("\n\n***Você perdeu seu acesso!***\n\n");
                        Environment.Exit(0);
                        return;
                    }
                }
            }
            Console.WriteLine("\n... operação finalizada ...");
            Console.WriteLine("\nPressione qualquer tecla para voltar ao menu anterior.");
            Console.ReadKey();
            return;
        }

        public static void GerarRelatorioFuncionarios()
        {
            Console.Clear();
            Console.WriteLine($">>> Usuário logado: {Login._usuarioAtual} <<<\n");
            AnsiConsole.Write(RepositorioFuncionarios.GerarRelatorioFuncionarios());
            Console.WriteLine("\n... operação finalizada ...");
            Console.WriteLine("\nPressione qualquer tecla para voltar ao menu anterior.");
            Console.ReadKey();
            return;

        }
    }
}
