using Bogus;
using Bogus.Extensions.Brazil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaCredit.AdaCredit.Funcionarios
{
    public sealed class FuncionariosFaker
    {
        public string? Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public long Telefone { get; set; }
        public string? Email { get; set; }
        public long CEP { get; set; }
        public string? Senha { get; set; }
        public string? DataUltimoLogin { get; set; }
        public static void GerarFuncionariosFaker()
        {
            string chars = "abcdefghijklmnopqrstuvxwyzABCDEFGHIJKLMNOPQRSTUVXWYZ0123456789!@#$%&*()_--+./|";
            for (int i = 0; i < 15; i++)
            {
                var faker = new Faker<FuncionariosFaker>("pt_BR").StrictMode(true)
                    .RuleFor(p => p.Nome, f => f.Person.FullName)
                    .RuleFor(p => p.DataNascimento, f => f.Person.DateOfBirth)
                    .RuleFor(p => p.Telefone, f => Convert.ToInt64(f.Random.Replace("###########")))
                    .RuleFor(p => p.Email, f => f.Person.Email)
                    .RuleFor(p => p.CEP, f => Convert.ToInt64(f.Random.Replace("########")))
                    .RuleFor(p => p.Senha, f => f.Random.String2(new Faker().Random.Int(5, 20), chars))
                    .RuleFor(p => p.DataUltimoLogin, DateTime.Now.ToString());

                var funcionario = faker.Generate();
                RepositorioFuncionarios.CadastrarFuncionarioFaker(funcionario.Nome, funcionario.DataNascimento,
                    funcionario.Telefone, funcionario.Email, funcionario.CEP, funcionario.Senha, 
                    funcionario.DataUltimoLogin);
            }
            RepositorioFuncionarios.AtualizarArquivoFuncionarios();
        }
    }
}
