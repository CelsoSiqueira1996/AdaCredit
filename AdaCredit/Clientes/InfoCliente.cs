using Bogus;
using Sharprompt;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;

namespace AdaCredit.AdaCredit.Clientes
{
    public static class InfoCliente
    {
        public static void CadastrarCliente()
        {
            Console.Clear();
            Console.WriteLine($">>> Usuário logado: {Login._usuarioAtual} <<<\n");
            Console.WriteLine("--- Cadastrar Novo Cliente ---\n");
            long cpf = 0;
            long cpfMatch = 1;
            do
            {
                Console.Write("CPF do cliente (somente dígitos): ");
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
            Console.WriteLine("--- Cadastrar Novo Cliente ---\n");
            Console.WriteLine($"CPF do cliente: {cpf.ToString(@"000\.000\.000\-00")}");
            Cliente? cliente = RepositorioClientes.ConsultarDadosCliente(cpf);
            if (cliente == null)
            {
                Console.Write("Nome do cliente: ");
                var nome = Console.ReadLine();
                Console.Write("Data de nascimento do cliente (dd/mm/aaaa): ");
                DateTime dataNascimento;
                while (!DateTime.TryParseExact(Console.ReadLine(), "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataNascimento))
                {
                    Console.Write("Data inválida. Preencha novamente (dd/mm/aaaa): ");
                }
                Console.Write("Telefone do cliente (somente dígitos): ");
                long telefone;
                while (!long.TryParse(Console.ReadLine(), out telefone))
                {
                    Console.Write("Telefone inválido. Preencha novamente (somente dígitos): ");
                }
                Console.Write("E-mail do cliente: ");
                var email = Console.ReadLine();
                Console.Write("CEP do cliente (somente dígitos): ");
                long cep;
                string? verificadorTamanhoCEP = Console.ReadLine();
                //É esperado que o CEP tenha 8 dígitos. A verificação aceita 0 à esquerda.
                while (verificadorTamanhoCEP.Length != 8 || !long.TryParse(verificadorTamanhoCEP, out cep))
                {
                    Console.Write("CEP inválido. Preencha novamente (somente dígitos): ");
                    verificadorTamanhoCEP = Console.ReadLine();
                }
                var numeroConta = RepositorioClientes.CadastrarCliente(nome, dataNascimento, cpf, telefone, email, cep);
                Console.WriteLine("\nCadastramento realizado com sucesso.");
                Console.WriteLine($"Número da conta: {numeroConta.ToString(@"00000\-0")}");
            }
            else
            {
                if (cliente.Ativo)
                {
                    Console.WriteLine($"Nome: {cliente.Nome}");
                    Console.WriteLine($"Número da conta: {cliente.NumeroConta.ToString(@"00000\-0")}");
                    Console.WriteLine($"\nCliente já possui conta ativa.");
                }
                else
                {
                    Cliente.AtivarConta(cliente);
                    Console.WriteLine($"Nome: {cliente.Nome}");
                    Console.WriteLine($"Número da conta: {cliente.NumeroConta.ToString(@"00000\-0")}");
                    Console.WriteLine($"\nCliente possui conta inativa. A conta foi reativada.");
                }
            }
            Console.WriteLine("\n... operação finalizada ...");
            Console.WriteLine("\nPressione qualquer tecla para voltar ao menu anterior.");
            Console.ReadKey();
            return;
        }

        public static void ConsultarDadosCliente()
        {
            Console.Clear();
            Console.WriteLine($">>> Usuário logado: {Login._usuarioAtual} <<<\n");
            Console.WriteLine("--- Consultar Dados de Cliente ---\n");
            Console.Write("Número do CPF do cliente (somente dígitos): ");
            long cpf;
            string? verificadorTamanhoCPF = Console.ReadLine();
            //É esperado que o CPF tenha 11 dígitos. A verificação aceita 0 à esquerda.
            while (verificadorTamanhoCPF.Length != 11 || !long.TryParse(verificadorTamanhoCPF, out cpf))
            {
                Console.Write("CPF inválido. Preencha novamente (somente dígitos): ");
                verificadorTamanhoCPF = Console.ReadLine();
            }
            Cliente? cliente = RepositorioClientes.ConsultarDadosCliente(cpf);
            if (cliente == null)
            {
                Console.WriteLine("\nCliente não encontrado!");
            }
            else
            {
                Console.WriteLine();
                AnsiConsole.Write(Cliente.TabelaDadosCliente(cliente));
            }
            Console.WriteLine("\n... operação finalizada ...");
            Console.WriteLine("\nPressione qualquer tecla para voltar ao menu anterior.");
            Console.ReadKey();
            return;
        }

        //CPF, saldo, status da conta e numero da conta não são alterados.

        public static void AlterarCadastroCliente()
        {
            Console.Clear();
            Console.WriteLine($">>> Usuário logado: {Login._usuarioAtual} <<<\n");
            Console.WriteLine("--- Alterar Cadastro de Cliente ---\n");
            Console.Write("Número do CPF do cliente (somente dígitos): ");
            long cpf;
            string? verificadorTamanhoCPF = Console.ReadLine();
            //É esperado que o CPF tenha 11 dígitos. A verificação aceita 0 à esquerda.
            while (verificadorTamanhoCPF.Length != 11 || !long.TryParse(verificadorTamanhoCPF, out cpf))
            {
                Console.Write("CPF inválido. Preencha novamente (somente dígitos): ");
                verificadorTamanhoCPF = Console.ReadLine();
            }
            Cliente? cliente = RepositorioClientes.ConsultarDadosCliente(cpf);
            if (cliente == null)
            {
                Console.WriteLine("\nCliente não encontrado!");
            }
            else
            {
                Console.WriteLine();
                AnsiConsole.Write(Cliente.TabelaDadosCliente(cliente));
                Prompt.ColorSchema.Answer = ConsoleColor.DarkGreen;
                Prompt.ColorSchema.Select = ConsoleColor.DarkCyan;
                var dadosParaAlterar = Prompt.MultiSelect("Selecione os dados a serem alterados: ", new[] { "Nome", "Data de Nascimento", "Telefone", "E-mail", "CEP" }, pageSize: 5);
                Console.WriteLine("\n--- Prencha com os novos dados ---\n");
                string? nome = "";
                long telefone = 0;
                string? email = "";
                long cep = 0;
                DateTime dataNascimento = default;
                foreach (var dado in dadosParaAlterar)
                {
                    switch (dado)
                    {
                        case "Nome":
                            Console.Write("Nome do cliente: ");
                            nome = Console.ReadLine();
                            break;
                        case "Data de Nascimento":
                            Console.Write("Data de nascimento do cliente (dd/mm/aaaa): ");
                            while (!DateTime.TryParseExact(Console.ReadLine(), "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataNascimento))
                            {
                                Console.Write("Data inválida. Preencha novamente (dd/mm/aaaa): ");
                            }
                            break;
                        case "Telefone":
                            Console.Write("Telefone do cliente (somente dígitos): ");
                            while (!long.TryParse(Console.ReadLine(), out telefone))
                            {
                                Console.Write("Telefone inválido. Preencha novamente (somente dígitos): ");
                            }
                            break;
                        case "E-mail":
                            Console.Write("E-mail do cliente: ");
                            email = Console.ReadLine();
                            break;
                        case "CEP":
                            Console.Write("CEP do cliente (somente dígitos): ");
                            string? verificadorTamanhoCEP = Console.ReadLine();
                            //É esperado que o CEP tenha 8 dígitos. A verificação aceita 0 à esquerda.
                            while (verificadorTamanhoCEP.Length != 8 || !long.TryParse(verificadorTamanhoCEP, out cep))
                            {
                                Console.Write("CEP inválido. Preencha novamente (somente dígitos): ");
                                verificadorTamanhoCEP = Console.ReadLine();
                            }
                            break;
                        default:
                            break;
                    }
                }
                Cliente.AlterarCadastroCliente(cliente, dadosParaAlterar.ToArray(), nome, telefone, email, cep, dataNascimento);
                Console.WriteLine("\nCadastro alterado com sucesso.");
            }
            Console.WriteLine("\n... operação finalizada ...");
            Console.WriteLine("\nPressione qualquer tecla para voltar ao menu anterior.");
            Console.ReadKey();
            return;
        }

        public static void DesativarCadastroCliente()
        {
            Console.Clear();
            Console.WriteLine($">>> Usuário logado: {Login._usuarioAtual} <<<\n");
            Console.WriteLine("--- Desativar Cadastro de Cliente ---\n");
            Console.WriteLine("***CASO O CLIENTE QUEIRA REATIVAR A CONTA, BASTA ENTRAR EM Cadastrar Novo Cliente E INFORMAR O CPF.***");
            Console.WriteLine("***A CONTA SERÁ REATIVADA NA HORA, COM O MESMO SALDO DO MOMENTO DA DESATIVAÇÃO.***\n");
            long cpf = 0;
            long cpfMatch = 1;
            do
            {
                Console.Write("CPF do cliente (somente dígitos): ");
                string? verificadorTamanhoCPF = Console.ReadLine();
                //É esperado que o CPF tenha 11 dígitos. A verificação aceita 0 à esquerda.
                while (verificadorTamanhoCPF.Length != 11 || !long.TryParse(verificadorTamanhoCPF, out cpf))
                {
                    Console.Write("CPF inválido. Preencha novamente (somente dígitos): ");
                    verificadorTamanhoCPF = Console.ReadLine();
                }
                Console.WriteLine("\n***ATENÇÃO: UMA VEZ CONFIRMADO O CPF, A CONTA ASSOCIADA SERÁ DESATIVADA E O SALDO FICARÁ CONGELADO!***");
                Console.Write("Digite o CPF novamente para confirmar: ");
                long.TryParse(Console.ReadLine(), out cpfMatch);
                if (cpf != cpfMatch)
                {
                    Console.WriteLine("\nCPFs diferem. Tente novamente!");
                }
            } while (cpf != cpfMatch);
            Console.Clear();
            Console.WriteLine($">>> Usuário logado: {Login._usuarioAtual} <<<\n");
            Console.WriteLine("--- Desativar Cadastro de Cliente ---\n");
            Console.WriteLine($"CPF do cliente: {cpf.ToString(@"000\.000\.000\-00")}");
            Cliente? cliente = RepositorioClientes.ConsultarDadosCliente(cpf);
            if (cliente == null)
            {
                Console.WriteLine("\nCliente não encontrado!");
            }
            else
            {
                Console.WriteLine($"Nome: {cliente.Nome}");
                Console.WriteLine($"Número da conta: {cliente.NumeroConta.ToString(@"00000\-0")}");
                if (!cliente.Ativo)
                {
                    Console.WriteLine("\nCadastro do cliente já está desativado.");
                }
                else
                {

                    Cliente.DesativarCadastroCliente(cliente);
                    Console.WriteLine("\nCadastro desativado com sucesso.");
                }
            }
            Console.WriteLine("\n... operação finalizada ...");
            Console.WriteLine("\nPressione qualquer tecla para voltar ao menu anterior.");
            Console.ReadKey();
            return;
        }

        public static void GerarRelatorioClientesAtivos()
        {
            Console.Clear();
            Console.WriteLine($">>> Usuário logado: {Login._usuarioAtual} <<<\n");
            AnsiConsole.Write(RepositorioClientes.GerarRelatorioClientesAtivos());
            Console.WriteLine("\n... operação finalizada ...");
            Console.WriteLine("\nPressione qualquer tecla para voltar ao menu anterior.");
            Console.ReadKey();
            return;

        }
        public static void GerarRelatorioClientesInativos()
        {
            Console.Clear();
            Console.WriteLine($">>> Usuário logado: {Login._usuarioAtual} <<<\n");
            AnsiConsole.Write(RepositorioClientes.GerarRelatorioClientesInativos());
            Console.WriteLine("\n... operação finalizada ...");
            Console.WriteLine("\nPressione qualquer tecla para voltar ao menu anterior.");
            Console.ReadKey();
            return;

        }
    }
}

