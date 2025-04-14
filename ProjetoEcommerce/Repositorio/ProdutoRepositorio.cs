using MySql.Data.MySqlClient;
using ProjetoEcommerce.Models;
using System.Data;


namespace ProjetoEcommerce.Repositorio
{
    // Define a classe responsável por interagir com os dados de produtos no banco de dados
    public class ProdutoRepositorio(IConfiguration configuration)
    {
        // Declara uma variável privada somente leitura para armazenar a string de conexão com o MySQL
        private readonly string _conexaoMySQL = configuration.GetConnectionString("ConexaoMySQL");


        // Método para cadastrar um novo Produto no banco de dados
        public void Cadastrar(Produto produto)
        {
            // Bloco using para garantir que a conexão seja fechada e os recursos liberados após o uso
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                // Abre a conexão com o banco de dados MySQL
                conexao.Open();
                // Cria um novo comando SQL para inserir dados na tabela 'Produto'
                MySqlCommand cmd = new MySqlCommand("insert into produto (NomeProd, DescricaoProd, QuantidadeProd, PrecoProd) values (@nome, @descricao, @quantidade, @preco)", conexao); // @: PARAMETRO
                                                                                                                                                                                          // Adiciona um parâmetro para o nome, definindo seu tipo e valor
                cmd.Parameters.Add("@nome", MySqlDbType.VarChar).Value = produto.NomeProd;
                cmd.Parameters.Add("@descricao", MySqlDbType.VarChar).Value = produto.DescricaoProd;
                cmd.Parameters.Add("@quantidade", MySqlDbType.Int32).Value = produto.QuantidadeProd;
                cmd.Parameters.Add("@preco", MySqlDbType.Float).Value = produto.PrecoProd;
                // Executa o comando SQL de inserção e retorna o número de linhas afetadas
                cmd.ExecuteNonQuery();
                // Fecha explicitamente a conexão com o banco de dados (embora o 'using' já faça isso)
                conexao.Close();
            }
        }

        // Método para Editar (atualizar) os dados de um produto existente no banco de dados
        public bool Atualizar(Produto produto)
        {
            try
            {
                // Bloco using para garantir que a conexão seja fechada e os recursos liberados após o uso
                using (var conexao = new MySqlConnection(_conexaoMySQL))
                {
                    // Abre a conexão com o banco de dados MySQL
                    conexao.Open();
                    // Cria um novo comando SQL para atualizar dados na tabela 'produto' com base no código
                    MySqlCommand cmd = new MySqlCommand("Update produto set NomeProd=@nome, DescricaoProd=@descricao, QuantidadeProd=@quantidade, PrecoProd=@preco " + " where CodProd=@codigo ", conexao);
                    cmd.Parameters.Add("@codigo", MySqlDbType.Int32).Value = produto.CodProd;
                    cmd.Parameters.Add("@nome", MySqlDbType.VarChar).Value = produto.NomeProd;
                    cmd.Parameters.Add("@descricao", MySqlDbType.VarChar).Value = produto.DescricaoProd;
                    cmd.Parameters.Add("@quantidade", MySqlDbType.VarChar).Value = produto.QuantidadeProd;
                    cmd.Parameters.Add("@preco", MySqlDbType.Float).Value = produto.PrecoProd;
                    // Executa o comando SQL de atualização e retorna o número de linhas afetadas
                    //executa e verifica se a alteração foi realizada
                    int linhasAfetadas = cmd.ExecuteNonQuery();
                    return linhasAfetadas > 0; // Retorna true se ao menos uma linha foi atualizada

                }
            }
            catch (MySqlException ex)
            {
                // Logar a exceção (usar um framework de logging como NLog ou Serilog)
                Console.WriteLine($"Erro ao atualizar produto: {ex.Message}");
                return false; // Retorna false em caso de erro

            }
        }

        // Método para listar todos os produtos do banco de dados
        public IEnumerable<Produto> TodosProdutos()
        {
            // Cria uma nova lista para armazenar os objetos Produto
            List<Produto> Productlist = new List<Produto>();

            // Bloco using para garantir que a conexão seja fechada e os recursos liberados após o uso
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                // Abre a conexão com o banco de dados MySQL
                conexao.Open();
                // Cria um novo comando SQL para selecionar todos os registros da tabela 'Produto'
                MySqlCommand cmd = new MySqlCommand("SELECT * from produto", conexao);

                // Cria um adaptador de dados para preencher um DataTable com os resultados da consulta
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                // Cria um novo DataTable
                DataTable dt = new DataTable();
                // metodo fill- Preenche o DataTable com os dados retornados pela consulta
                da.Fill(dt);
                // Fecha explicitamente a conexão com o banco de dados 
                conexao.Close();

                // interage sobre cada linha (DataRow) do DataTable
                foreach (DataRow dr in dt.Rows)
                {
                    // Cria um novo objeto Produto e preenche suas propriedades com os valores da linha atual
                    Productlist.Add(
                                new Produto
                                {
                                    CodProd = Convert.ToInt32(dr["CodProd"]),
                                    NomeProd = ((string)dr["NomeProd"]),
                                    DescricaoProd = ((string)dr["DescricaoProd"]),
                                    QuantidadeProd = Convert.ToInt32(dr["QuantidadeProd"]),
                                    PrecoProd = ((float)dr["PrecoProd"]),
                                });
                }
                // Retorna a lista de todos os Produtos
                return Productlist;
            }
        }

        // Método para buscar um Produto específico pelo seu código (Codigo)
        public Produto ObterProduto(int Codigo)
        {
            // Bloco using para garantir que a conexão seja fechada e os recursos liberados após o uso
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                // Abre a conexão com o banco de dados MySQL
                conexao.Open();
                // Cria um novo comando SQL para selecionar um registro da tabela 'Produto' com base no código
                MySqlCommand cmd = new MySqlCommand("SELECT * from produto where CodProd=@codigo ", conexao);

                // Adiciona um parâmetro para o código a ser buscado, definindo seu tipo e valor
                cmd.Parameters.AddWithValue("@codigo", Codigo);

                // Cria um adaptador de dados (não utilizado diretamente para ExecuteReader)
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                // Declara um leitor de dados do MySQL
                MySqlDataReader dr;
                // Cria um novo objeto Produto para armazenar os resultados
                Produto produto = new Produto();

                /* Executa o comando SQL e retorna um objeto MySqlDataReader para ler os resultados
                CommandBehavior.CloseConnection garante que a conexão seja fechada quando o DataReader for fechado*/

                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                // Lê os resultados linha por linha
                while (dr.Read())
                {
                    // Preenche as propriedades do objeto Produto com os valores da linha atual
                    produto.CodProd = Convert.ToInt32(dr["CodProd"]);
                    produto.NomeProd = (string)(dr["NomeProd"]);
                    produto.DescricaoProd = (string)(dr["DescricaoProd"]);
                    produto.QuantidadeProd = Convert.ToInt32(dr["QuantidadeProd"]);
                    produto.PrecoProd = (float)(dr["PrecoProd"]);
                }
                // Retorna o objeto Produto encontrado (ou um objeto com valores padrão se não encontrado)
                return produto;
            }
        }


        // Método para excluir um Produto do banco de dados pelo seu código (ID)
        public void Excluir(int Id)
        {
            // Bloco using para garantir que a conexão seja fechada e os recursos liberados após o uso
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                // Abre a conexão com o banco de dados MySQL
                conexao.Open();

                // Cria um novo comando SQL para deletar um registro da tabela 'produto' com base no código
                MySqlCommand cmd = new MySqlCommand("delete from produto where CodProd=@codigo", conexao);

                // Adiciona um parâmetro para o código a ser excluído, definindo seu tipo e valor
                cmd.Parameters.AddWithValue("@codigo", Id);

                // Executa o comando SQL de exclusão e retorna o número de linhas afetadas
                int i = cmd.ExecuteNonQuery();

                conexao.Close(); // Fecha  a conexão com o banco de dados
            }
        }
    }
}
