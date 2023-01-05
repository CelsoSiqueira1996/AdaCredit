using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using static BCrypt.Net.BCrypt;

namespace AdaCredit.AdaCredit.Funcionarios
{
    public sealed class Funcionario
    {
        public string? Nome { get; private set; }
        public DateTime DataNascimento { get; private set; }
        public long CPF { get; private set; }
        public long Telefone { get; private set; }
        public string? Email { get; private set; }
        public long CEP { get; private set; }
        public string? NomeUsuario { get; private set; }
        public string? Salt { get; private set; }
        public string? SenhaHash { get; private set; }
        public bool Ativo { get; private set; }
        public string? DataUltimoLogin { get; private set; }

        public Funcionario(string nome, DateTime dataNascimento, long cpf, long telefone,
            string email, long cep, string nomeUsuario, string salt, string senhaHash, bool ativo, string dataUltimoLogin)
        {
            Nome = nome;
            DataNascimento = dataNascimento;
            CPF = cpf;
            Telefone = telefone;
            Email = email;
            CEP = cep;
            NomeUsuario = nomeUsuario;
            Salt = salt;
            SenhaHash = senhaHash;
            Ativo = ativo;
            DataUltimoLogin = dataUltimoLogin;
        }

        public static Funcionario CadastrarFuncionario(string nome, DateTime dataNascimento, long cpf, long telefone,
            string email, long cep, string nomeUsuario, string senha)
        {
            string salt = GenerateSalt();
            var senhaHash = HashPassword(senha, salt);
            bool status = true;
            string dataUltimoLogin = "";
            return new Funcionario(nome, dataNascimento, cpf, telefone, email, cep, nomeUsuario, salt, senhaHash, status, dataUltimoLogin);
        }

        public static Funcionario CadastrarFuncionarioFaker(string nome, DateTime dataNascimento, long cpf, long telefone,
    string email, long cep, string nomeUsuario, string senha, string dataUltimoLogin)
        {
            string salt = GenerateSalt();
            var senhaHash = HashPassword(senha, salt);
            bool status = true;
            return new Funcionario(nome, dataNascimento, cpf, telefone, email, cep, nomeUsuario, salt, senhaHash, status, dataUltimoLogin);
        }

        public static void AlterarSenhaFuncionario(Funcionario funcionario, string senha)
        {
            funcionario.Salt = GenerateSalt();
            funcionario.SenhaHash = HashPassword(senha, funcionario.Salt);
            RepositorioFuncionarios.AtualizarArquivoFuncionarios();
            return;
        }

        public static void DesativarCadastroFuncionario(Funcionario funcionario)
        {
            funcionario.Ativo = false;
            RepositorioFuncionarios.AtualizarArquivoFuncionarios();
            return;
        }

        public static void AtualizarUltimoLogin(Funcionario funcionario, string dataUltimoLogin)
        {
            funcionario.DataUltimoLogin = dataUltimoLogin;
        }
    }
}
