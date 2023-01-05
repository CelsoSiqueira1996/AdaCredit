using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Bogus.Extensions.Brazil;

namespace AdaCredit.AdaCredit.Clientes
{
    public sealed class ClientesFaker
    {
        public string? Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public long Telefone { get; set; }
        public string? Email { get; set; }
        public long CEP { get; set; }
        public decimal Saldo { get; private set; }
        public bool Ativo { get; private set; }
        public static void GerarClientesFaker()
        {
            for (int i = 0; i < 30; i++)
            {
                var faker = new Faker<ClientesFaker>("pt_BR").StrictMode(true)
                    .RuleFor(p => p.Nome, f => f.Person.FullName)
                    .RuleFor(p => p.DataNascimento, f => f.Person.DateOfBirth)
                    .RuleFor(p => p.Telefone, f => Convert.ToInt64(f.Random.Replace("###########")))
                    .RuleFor(p => p.Email, f => f.Person.Email)
                    .RuleFor(p => p.CEP, f => Convert.ToInt64(f.Random.Replace("########")))
                    .RuleFor(p => p.Saldo, f => Math.Round(f.Random.Decimal(0, 5000), 2))
                    .RuleFor(p => p.Ativo, f => f.Random.Bool());
                var cliente = faker.Generate();
                RepositorioClientes.CadastrarClienteFaker(cliente.Nome, cliente.DataNascimento, cliente.Telefone,
                    cliente.Email, cliente.CEP, cliente.Saldo, cliente.Ativo);
            }
            RepositorioClientes.AtualizarArquivoClientes();
        }

    }
}
