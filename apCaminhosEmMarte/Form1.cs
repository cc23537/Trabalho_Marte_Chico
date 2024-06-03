using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace apCaminhosEmMarte
{
    public partial class FrmCaminhos : Form
    {
        public FrmCaminhos()
        {
            InitializeComponent();
            pbMapa.Image = Image.FromFile(arquivo);

        }
        string pasta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
        static string p1 = Directory.GetCurrentDirectory();
        static string p2 = Directory.GetParent(p1).FullName;
        static string p3 = Directory.GetParent(p2).FullName;
        string arquivo = Path.Combine(p3, "Mapa Marte sem rotas.jpg");

        ITabelaDeHash<Cidade> tabela;

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void RemoverCidadeDoArquivo(string nomeCidade)
        {
            if (dlgAbrir.FileName != null && dlgAbrir.FileName != "")
            {
                string[] linhas = File.ReadAllLines(dlgAbrir.FileName);
                using (StreamWriter writer = new StreamWriter(dlgAbrir.FileName))
                {

                    foreach (string linha in linhas)
                    {

                        string[] campos = linha.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);


                        if (campos[0].Trim() != nomeCidade)
                        {
                            writer.WriteLine(linha);
                        }
                    }
                }
            }
        }

        private void btnLerArquivo_Click(object sender, EventArgs e)
        {
            if (dlgAbrir.ShowDialog() == DialogResult.OK)
            {
                if (rbBucketHash.Checked)
                    tabela = new BucketHash<Cidade>();
                else
                  if (rbHashLinear.Checked)
                    tabela = new HashLinear<Cidade>();
                else
                    if (rbHashQuadratico.Checked)
                    tabela = new HashQuadratico<Cidade>();
                else
                      if (rbHashDuplo.Checked)
                    tabela = new HashDuplo<Cidade>();

                var arquivo = new StreamReader(dlgAbrir.FileName);
                while (!arquivo.EndOfStream)
                {
                    Cidade umaCidade = new Cidade();
                    umaCidade.LerRegistro(arquivo);
                    tabela.Inserir(umaCidade);
                }
                lsbCidades.Items.Clear();
                var asCidades = tabela.Conteudo();
                foreach (Cidade cid in asCidades)
                    lsbCidades.Items.Add(cid);
                arquivo.Close();
            }
        }



        private void FrmCaminhos_FormClosing(object sender, FormClosingEventArgs e)
        {
            // abrir o arquivo para saida, se houver um arquivo selecionado
            // obter todo o conteúdo da tabela de hash
            // percorrer o conteúdo da tabela de hash, acessando
            // cada cidade individualmente e usar esse objeto Cidade
            // para gravar seus próprios dados no arquivo
            // fechar o arquivo ao final do percurso
        }

        private void btnInserir_Click(object sender, EventArgs e)
        {
            string nomeCidade = txtCidade.Text.Trim();
            string strX = udX.Text.Trim();
            string strY = udY.Text.Trim();

            if (!string.IsNullOrEmpty(nomeCidade) && !string.IsNullOrEmpty(strX) && !string.IsNullOrEmpty(strY))
            {
                double x, y;
                if (double.TryParse(strX, out x) && double.TryParse(strY, out y))
                {
                    Cidade novaCidade = new Cidade
                    {
                        NomeCidade = nomeCidade,
                        X = x,
                        Y = y
                    };

                    InserirCidadeNoArquivo(novaCidade);

                    float cordX = (float)x;
                    float cordY = (float)y;
                    lsbCidades.Items.Add(novaCidade);
                    pbMapa.Invalidate();
                    SalvarPontoNaImagem(cordX, cordY, nomeCidade);
                }
                else
                {
                    MessageBox.Show("Por favor, insira valores válidos para X e Y.");
                }
            }
            else
            {
                MessageBox.Show("Por favor, preencha todos os campos.");
            }
        }

        private void btnRemover_Click(object sender, EventArgs e)
        {
            string nomeCidade = txtCidade.Text.Trim();

            if (!string.IsNullOrEmpty(nomeCidade))
            {
                RemoverCidadeDoArquivo(nomeCidade);
                int indice = lsbCidades.FindString(nomeCidade);
                if (indice != ListBox.NoMatches)
                {
                    lsbCidades.Items.RemoveAt(indice);
                }
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            lsbCidades.Items.Clear();
            string nomeCidade = txtCidade.Text;
            string x = udX.Text;
            string y = udY.Text;

            Cidade cid = new Cidade();
            cid.NomeCidade = nomeCidade;
            cid.X = double.Parse(x);
            cid.Y = double.Parse(y);
            if (dlgAbrir.FileName != null && dlgAbrir.FileName != "")
            {

                StreamReader arquivo = new StreamReader(dlgAbrir.FileName);

                while (!arquivo.EndOfStream)
                {
                    Cidade umaCidade = new Cidade();
                    umaCidade.LerRegistro(arquivo);
                    if (cid.X == umaCidade.X && cid.Y == umaCidade.Y && cid.NomeCidade == umaCidade.NomeCidade)
                    {
                        lsbCidades.Items.Add(cid);
                        break;
                    }
                }
            }
        }
        private void btnListar_Click(object sender, EventArgs e)
        {
            lsbCidades.Items.Clear();
            var asCidades = tabela.Conteudo();
            foreach (Cidade cid in asCidades)
                lsbCidades.Items.Add(cid);
        }
        private void InserirCidadeNoArquivo(Cidade cidade)
        {
            if (dlgAbrir.FileName != null && dlgAbrir.FileName != "")
            {

                using (StreamWriter arquivo = new StreamWriter(dlgAbrir.FileName, true, Encoding.ASCII))
                {
                    cidade.GravarDados(arquivo);
                }
            }
        }
        private void SalvarPontoNaImagem(float x, float y, string a)
        {
            try
            {
                string caminhoOriginal = arquivo;
                string caminhoTemporario = "imagem_temporaria.jpg";
                if (!File.Exists(caminhoOriginal))
                {
                    MessageBox.Show("Arquivo não encontrado.");
                    return;
                }
                using (Bitmap imagem = new Bitmap(caminhoOriginal))
                {
                    float cordX = imagem.Width * x;
                    float cordY = imagem.Height * y;
                    using (Bitmap imagemEditada = new Bitmap(imagem))
                    {
                        using (Graphics g = Graphics.FromImage(imagemEditada))
                        {
                            using (SolidBrush brush = new SolidBrush(Color.Black))
                            {
                                using (Font font = new Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0))))
                                {
                                    g.FillEllipse(brush, cordX, cordY, 40, 40); // desenha o ponto
                                    g.DrawString(a, font, brush, cordX + 10, cordY + 30); // escreve a string
                                }
                            }
                        }
                        pbMapa.Image.Dispose();
                        imagemEditada.Save(caminhoTemporario, ImageFormat.Jpeg);
                    }
                }
                File.Delete(caminhoOriginal);
                File.Move(caminhoTemporario, caminhoOriginal);

                pbMapa.Image = Image.FromFile(arquivo); //atualiza a imagem antiga para a nova imagem
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar: " + ex.Message);
            }
        }

        /// <summary>
        /// /////////
        /// </summary>

        Cidade[] asCidades;
        int quantasCidades;
        private void tpCaminhos_Enter(object sender, EventArgs e)
        {
            asCidades = new Cidade[25];
            quantasCidades = 0;
            // abrir o arquivo de cidades
            // enquanto o arquivo de cidades não acabar
            //    instancie um objeto da classe cidade
            //    faça esse objeto ler um registro de cidade
            //    adicione esse registro de cidade após a última
            //    posição usada do vetor de cidades
            //    incremente quantasCidades

            // fechar o arquivo de cidades
            // ordenar o vetor de cidades pelo atributo nome

            OrdenarCidades();
            // copiar os nomes de cada cidade nos cbxOrigem e cbxDestino
        }

        private void OrdenarCidades()
        {
            //asCidades[0] = new Cidade("Campinas", 0, 0);
            //asCidades[1] = new Cidade("Americana", 0, 0); 
            //asCidades[2] = new Cidade("Sumaré", 0, 0);
            //asCidades[3] = new Cidade("Estiva Gerbi", 0, 0);
            //asCidades[4] = new Cidade("Rafard", 0, 0); 
            //asCidades[5] = new Cidade("Rifaina", 0, 0);
            //asCidades[6] = new Cidade("Hortolândia", 0, 0);
            //quantasCidades = 7;

            // Ordenação por seleção direta ou
            // Selection Sort
            for (int lento = 0; lento < quantasCidades; lento++)
            {
                int indiceMenorCidade = lento;
                for (int rapido = lento + 1; rapido < quantasCidades; rapido++)
                    if (asCidades[rapido].NomeCidade.CompareTo(
                          asCidades[indiceMenorCidade].NomeCidade) < 0)
                        indiceMenorCidade = rapido;

                if (indiceMenorCidade != lento)
                {
                    Cidade auxiliar = asCidades[indiceMenorCidade];
                    asCidades[indiceMenorCidade] = asCidades[lento];
                    asCidades[lento] = auxiliar;
                }
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
