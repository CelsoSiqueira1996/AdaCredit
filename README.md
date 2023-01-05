# AdaCredit
O primeiro login exige um usuário e senha iniciais, respectivamente, "user" e "pass".
Os arquivos/dados faker são gerados logo após o cadastramento do primeiro funcionário.
Os arquivos que contêm os clientes e os funcionários serão salvos na mesma pasta do arquivo .exe do sistema.
Os arquivos de transação serão salvos no desktop, na pasta Transactios (a mesma será criada no caso de sua ausência).
São gerados 30 clientes (saldos entre 0.00 e 5000.00), 15 funcionários e 200 transações pendentes (valores entre 200.00 e 2000.00).
Para alterar senhas de funcionários é necessário ter a senha antiga!
O nome de usuário está restrito a ter no mínimo 5 e no máximo 10 caractéres.
As senhas devem ter no mínimo 5 e no máximo 20 caractéres.
Não são permitidos usuários com mesmo login, mesmo se a conta estiver desativada.
A cada conta de cliente está associado um único CPF e vice versa. Esse link nunca se perde, mesmo ao desativar a conta. A mesma pode ser reativada cadastrando o CPF novamente no cadastramento de clientes.
Ao desativar uma conta de um cliente o saldo fica congelado. Caso a conta seja reativada, ela terá o saldo inicial igual ao saldo do momento da desativação.
Ao cadastrar um novo cliente, seu saldo inicia com valor zero. Entradas e saídas de valores das contas só ocorrem por meio das transações.

