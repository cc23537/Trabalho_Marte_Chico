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
        private PointF currentPosition;
        private Point ponto = new Point(-1, -1);



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
                lsbCidades.Items.Clear();  // limpa o listBox
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
                    pbMapa.Invalidate(); // Redesenha o ImageBox para exibir o ponto
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
                    Console.WriteLine(cid);
                    Console.WriteLine(umaCidade);
                    Console.WriteLine(umaCidade.X == cid.X);
                    Console.WriteLine(umaCidade.Y == cid.Y);
                    Console.WriteLine(umaCidade.NomeCidade == cid.NomeCidade);
                    Console.WriteLine(umaCidade.Equals(cid));
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
                //StreamWriter arquivo = new StreamWriter(dlgAbrir.FileName, true, Encoding.ASCII);
                //cidade.GravarDados(arquivo);

            }
        }


        private float DPI
        {
            get
            {
                using (var g = CreateGraphics())
                    return g.DpiX;
            }
        }

        private PointF PointToCartesian(Point point)
        {
            return new PointF(point.X, point.Y);
        }

        private float Pixel(float pixel)
        {
            return pixel * 25.4f / DPI;
        }



        private void SalvarPontoNaImagem(float x, float y, string a)
        {
            try
            {
                string caminhoOriginal = arquivo;
                string caminhoTemporario = "imagem_temporaria.jpg";

                // Verifica se o arquivo original existe
                if (!File.Exists(caminhoOriginal))
                {
                    MessageBox.Show("Arquivo original não encontrado.");
                    return;
                }

                // Carrega a imagem original
                using (Bitmap imagem = new Bitmap(caminhoOriginal))
                {
                    float cordX = imagem.Width * x;
                    float cordY = imagem.Height * y;
                    // Clona a imagem original para fazer edições
                    using (Bitmap imagemEditada = new Bitmap(imagem))
                    {
                        // Desenha o ponto na imagem editada
                        using (Graphics g = Graphics.FromImage(imagemEditada))
                        {
                            using (SolidBrush brush = new SolidBrush(Color.Black))
                            {
                                using (Font font = new Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0))))
                                {
                                    g.FillEllipse(brush, cordX, cordY, 40, 40);
                                    g.DrawString(a, font, brush, cordX + 10, cordY + 30);
                                    // O ponto será desenhado com raio 1
                                }
                            }
                        }

                        // Libera a referência à imagem atual do controle PictureBox
                        pbMapa.Image.Dispose();

                        // Salva a imagem editada em um arquivo temporário
                        imagemEditada.Save(caminhoTemporario, ImageFormat.Jpeg);
                    }
                }

                // Substitui o arquivo original pelo arquivo temporário
                File.Delete(caminhoOriginal);
                File.Move(caminhoTemporario, caminhoOriginal);

                // Carrega a imagem novamente no controle PictureBox
                pbMapa.Image = Image.FromFile(arquivo);

                MessageBox.Show("Ponto salvo na imagem com sucesso.");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar o ponto na imagem: " + ex.Message);
            }
        }
    }
}
