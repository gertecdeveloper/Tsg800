using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using BR.Com.Gertec.Gedi;
using BR.Com.Gertec.Gedi.Enums;
using BR.Com.Gertec.Gedi.Exceptions;
using BR.Com.Gertec.Gedi.Interfaces;
using BR.Com.Gertec.Gedi.Structs;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BR.Com.Gertec.Gedi.Impl;
using ZXing;
using ZXing.Common;
using System.Threading;
using Android.Graphics;
using Plugin.DeviceInfo;
using GertecXamarinForms.Droid.Impressao;
using ZXing.Mobile;

namespace GertecXamarinAndroid.Impressao
{
    public class GertecPrinter : IGertecPrinter
    {
        // Defines
        private const string IMPRESSORA_ERRO = "Impressora com erro.";

        private string modelo = CrossDeviceInfo.Current.Model;

        // Statics
        private static bool isPrintInit = false;

        private IGEDI iGedi;
        private IPRNTR iPrintr;
        private GEDI_PRNTR_st_StringConfig stringConfig;
        private GEDI_PRNTR_st_PictureConfig pictureConfig;
        private GEDI_PRNTR_e_Status status;

        private ConfigPrint configPrint;
        private Typeface typeface;

        // Thread starGedi;

        private Activity mainActivity;
        private Context mainContext;

        /**
         * Método construtor da classe TSG 800
         * @param a = Activity  atual que esta sendo inicializada a class
         */
        public GertecPrinter(Activity act)
        {
            this.mainActivity = act;
            startGediTSG800();
        }

        /**
         * Método que instância a classe GEDI da lib deve ser usado sempre o TSG 800
         *
         * @apiNote = Este mátodo faz a instância da classe GEDI através de uma Thread.
         *            Será sempre chamado na construção da classe.
         *            Não alterar...
         *
         */
        public void startGediTSG800()
        {
            new Thread(new ThreadStart(() =>
            {
                this.iGedi = new Gedi(this.mainActivity);
                this.iGedi = GEDI.GetInstance(this.mainActivity);
                this.iPrintr = iGedi.PRNTR;
                Thread.Sleep(250);
            })).Start();
        }

        /**
         * Método que faz a inicialização da impressao
         * @throws GediException = retorno o código do erro.
         * */
        private void ImpressoraInit()
        {
            try
            {
                if (this.iPrintr != null && !isPrintInit)
                {
                    this.iPrintr.Init();
                    isPrintInit = true;
                    getStatusImpressora();
                }
            }
            catch (GediException e)
            {
                throw new GediException(e.ErrorCode);
            }
        }

        /**
         * Método que faz a finalizacao do objeto iPrint
         * @throws GediException = retorno o código do erro.
         * */
        public void ImpressoraOutput()
        {
            try
            {
                if (this.iPrintr != null && isPrintInit)
                {
                    this.iPrintr.Output();
                    isPrintInit = false;
                }
            }
            catch (GediException e)
            {
                throw new GediException(e.ErrorCode);
            }
        }

        /**
         * Método que faz o avanço de linhas após uma impressão.
         *
         * @param linhas = Número de linhas que dever ser pulado após a impressão.
         *
         * @throws GediException = retorna o código do erro.
         *
         * @apiNote = Esse método não deve ser chamado dentro de um FOR ou WHILE,
         * o número de linhas deve ser sempre passado no atributo do método.
         * */
        public void AvancaLinha(int linhas)
        {
            try
            {
                this.iPrintr.DrawBlankLine(linhas);
            }
            catch (GediException e)
            {
                throw new GediException(e.ErrorCode);
            }
        }

        /**
         * Método que retorna o atual estado da impressora
         * @return String = traduzStatusImpressora()
         *
         * */
        public string getStatusImpressora()
        {
            try
            {
                this.status = this.iPrintr.Status();
            }
            catch (GediException e)
            {
                throw new GediException(e.ErrorCode);
            }
            return traduzStatusImpressora(this.status);
        }

        /**
         * Método que faz a impressão de código de barras
         *
         * @param texto = Texto que será usado para a impressão do código de barras
         * @param height  = Tamanho
         * @param width  = Tamanho
         * @param barcodeFormat  = Tipo do código que será impresso
         *
         * @throws GediException = retorna o código do erro.
         * */
        public bool ImprimeBarCode(string texto, int height, int width, string barcodeFormat)
        {
            try
            {

                BarcodeWriter writer = new BarcodeWriter();

                if (barcodeFormat.Equals("CODE_128"))
                {
                    writer.Format = BarcodeFormat.CODE_128;
                }
                else if (barcodeFormat.Equals("EAN_8"))
                {
                    writer.Format = BarcodeFormat.EAN_8;
                }
                else if (barcodeFormat.Equals("EAN_13"))
                {
                    writer.Format = BarcodeFormat.EAN_13;
                }
                else if (barcodeFormat.Equals("PDF_417"))
                {
                    writer.Format = BarcodeFormat.PDF_417;
                }
                else if (barcodeFormat.Equals("QR_CODE"))
                {
                    writer.Format = BarcodeFormat.QR_CODE;
                }

                writer.Options = new EncodingOptions()
                {
                    Width = width,
                    Height = height,
                    Margin = 2
                };

                var bitmap = writer.Write(texto);

                this.pictureConfig = new GEDI_PRNTR_st_PictureConfig();
                this.pictureConfig.Alignment = GEDI_PRNTR_e_Alignment.ValueOf(this.configPrint.Alinhamento);

                this.pictureConfig.Height = bitmap.Height;
                this.pictureConfig.Width = bitmap.Width;

                this.ImpressoraInit();
                this.iPrintr.DrawPictureExt(pictureConfig, bitmap);

                return true;
            }
            catch (GediException e)
            {
                throw new GediException(e.ErrorCode);
            }
        }

        /**
         * Método que faz a impressão de código de barras
         *
         * @param texto = Texto que será usado para a impressão do código de barras
         * @param height  = Tamanho
         * @param width  = Tamanho
         * @param barcodeFormat  = Tipo do código que será impresso
         *
         * @throws GediException = retorna o código do erro.
         * */
        public bool ImprimeBarCodeIMG(string texto, int height, int width, string barcodeFormat)
        {
            try
            {
                MultiFormatWriter multiFormatWriter = new MultiFormatWriter();
                BitMatrix bitMatrix = null;

                if (barcodeFormat.Equals("CODE_128"))
                {
                    bitMatrix = multiFormatWriter.encode(texto, BarcodeFormat.CODE_128, height, width);
                }
                else if (barcodeFormat.Equals("EAN_8"))
                {
                    bitMatrix = multiFormatWriter.encode(texto, BarcodeFormat.EAN_8, height, width);
                }
                else if (barcodeFormat.Equals("EAN_13"))
                {
                    bitMatrix = multiFormatWriter.encode(texto, BarcodeFormat.EAN_13, height, width);
                }
                else if (barcodeFormat.Equals("PDF_417"))
                {
                    bitMatrix = multiFormatWriter.encode(texto, BarcodeFormat.PDF_417, height, width);
                }
                else if (barcodeFormat.Equals("QR_CODE"))
                {
                    bitMatrix = multiFormatWriter.encode(texto, BarcodeFormat.QR_CODE, height, width);
                }

                var bitWidth = bitMatrix.Width;
                var bitHeight = bitMatrix.Height;

                int[] pixelsImage = new int[bitWidth * bitHeight];

                //Gerando o bitmap do barcodeformat com a largura, altura e formato de cor
                for (int i = 0; i < bitHeight; i++)
                {
                    for (int j = 0; j < bitWidth; j++)
                    {
                        if (bitMatrix[j, i])
                            pixelsImage[i * bitWidth + j] = (int)Convert.ToInt64(0xff000000);
                        else
                            pixelsImage[i * bitWidth + j] = (int)Convert.ToInt64(0xffffffff);
                    }
                }

                Bitmap bitmap = Bitmap.CreateBitmap(bitWidth, bitHeight, Bitmap.Config.Argb8888);
                bitmap.SetPixels(pixelsImage, 0, bitWidth, 0, 0, bitWidth, bitHeight);

                this.pictureConfig = new GEDI_PRNTR_st_PictureConfig();
                this.pictureConfig.Alignment = GEDI_PRNTR_e_Alignment.ValueOf(this.configPrint.Alinhamento);

                this.pictureConfig.Height = bitmap.Height;
                this.pictureConfig.Width = bitmap.Width;

                this.ImpressoraInit();
                this.iPrintr.DrawPictureExt(pictureConfig, bitmap);

                return true;
            }
            catch (GediException e)
            {
                throw new GediException(e.ErrorCode);
            }
        }

        /**
         * Método que faz a impressão de imagens
         * @param imagem = Nome da imagem que deve estar na pasta drawable
         * @throws GediException = retorna o código do erro.
         * */
        public bool ImprimeImagem(string imagem)
        {
            int id;
            Bitmap bitmap;
            try
            {
                this.pictureConfig = new GEDI_PRNTR_st_PictureConfig();
                this.pictureConfig.Alignment = GEDI_PRNTR_e_Alignment.ValueOf(this.configPrint.Alinhamento);

                id = this.mainActivity.Resources.GetIdentifier(imagem, "drawable", this.mainActivity.PackageName);
                bitmap = BitmapFactory.DecodeResource(this.mainActivity.Resources, id);
                if (modelo.Equals("Smart G800"))
                {
                    this.pictureConfig.Height = bitmap.Height;
                    this.pictureConfig.Width = bitmap.Width;
                }
                else
                {
                    this.pictureConfig.Height = configPrint.IHeight;
                    this.pictureConfig.Width = configPrint.IWidth;
                }
                ImpressoraInit();
                this.iPrintr.DrawPictureExt(pictureConfig, bitmap);

            }
            catch (GediException e)
            {
                throw new GediException(e.ErrorCode);
            }

            return true;
        }

        /**
         * Método que recebe o atual texto a ser impresso
         * @param texto  = Texto que será impresso.
         * @throws Exception = caso a impressora esteja com erro.
         * */
        public void ImprimeTexto(string texto)
        {
            try
            {
                ImpressoraInit();

                if (!this.IsImpressoraOK())
                {
                    throw new Exception(IMPRESSORA_ERRO);
                }
                sPrintLine(texto);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /**
         * Método que recebe o atual texto e o tamanho da fonte que deve ser usado na impressão.
         *
         * @param texto  = Texto que será impresso.
         * @param tamanho = Tamanho da fonte que será usada
         *
         * @throws Exception = caso a impressora esteja com erro.
         * */
        public void ImprimeTexto(string texto, int tamanho)
        {
            int tamanhoOld;
            try
            {
                this.getStatusImpressora();
                if (!this.IsImpressoraOK())
                {
                    throw new Exception(IMPRESSORA_ERRO);
                }
                tamanhoOld = this.configPrint.Tamanho;
                this.configPrint.Tamanho = tamanho;
                sPrintLine(texto);
                this.configPrint.Tamanho = tamanhoOld;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /**
         * Método que recebe o atual texto e ser o mesmo será impresso em negrito.
         *
         * @param texto  = Texto que será impresso.
         * @param negrito = Caso o texto deva ser impresso em negrito
         *
         * @throws Exception = caso a impressora esteja com erro.
         * */
        public void ImprimeTexto(string texto, bool negrito)
        {
            bool negritoOld;
            try
            {
                this.getStatusImpressora();
                if (!this.IsImpressoraOK())
                {
                    throw new Exception(IMPRESSORA_ERRO);
                }
                negritoOld = this.configPrint.Negrito;
                this.configPrint.Negrito = negrito;
                sPrintLine(texto);
                this.configPrint.Negrito = negritoOld;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /**
         * Método que recebe o atual texto e ser o mesmo será impresso em negrito e/ou itálico.
         *
         * @param texto  = Texto que será impresso.
         * @param negrito = Caso o texto deva ser impresso em negrito
         * @param italico  = Caso o texto deva ser impresso em itálico
         *
         * @throws Exception = caso a impressora esteja com erro.
         * */
        public void ImprimeTexto(string texto, bool negrito, bool italico)
        {
            bool negritoOld;
            bool italicoOld;
            try
            {
                this.getStatusImpressora();
                if (!this.IsImpressoraOK())
                {
                    throw new Exception(IMPRESSORA_ERRO);
                }
                negritoOld = this.configPrint.Negrito;
                italicoOld = this.configPrint.Italico;
                this.configPrint.Negrito = negrito;
                sPrintLine(texto);
                this.configPrint.Negrito = negritoOld;
                this.configPrint.Italico = italicoOld;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /**
         * Método que recebe o atual texto e ser o mesmo será impresso em negrito, itálico e/ou sublinhado.
         *
         * @param texto  = Texto que será impresso.
         * @param negrito = Caso o texto deva ser impresso em negrito
         * @param italico  = Caso o texto deva ser impresso em itálico
         * @param sublinhado   = Caso o texto deva ser impresso em itálico.
         *
         * @throws Exception = caso a impressora esteja com erro.
         * */
        public void ImprimeTexto(string texto, bool negrito, bool italico, bool sublinhado)
        {
            bool negritoOld;
            bool italicoOld;
            bool sublinhadoOld;
            try
            {
                this.getStatusImpressora();
                if (!this.IsImpressoraOK())
                {
                    throw new Exception(IMPRESSORA_ERRO);
                }
                negritoOld = this.configPrint.Negrito;
                italicoOld = this.configPrint.Italico;
                sublinhadoOld = this.configPrint.SubLinhado;
                this.configPrint.Negrito = negrito;
                sPrintLine(texto);
                this.configPrint.Negrito = negritoOld;
                this.configPrint.Italico = italicoOld;
                this.configPrint.SubLinhado = sublinhadoOld;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /**
        * Método que retorno se a impressora está apta a fazer impressões
        * @return true = quando estiver tudo ok.
        * */
        public bool IsImpressoraOK()
        {
            if (this.status.Value == 0)
            {
                return true;
            }
            return false;
        }

        /**
         * Método que recebe a configuração para ser usada na impressão
         * @param config  = Classe ConfigPrint que contém toda a configuração
         *                  para a impressão
         * */
        public void setConfigImpressao(ConfigPrint config)
        {
            this.configPrint = config;

            this.stringConfig = new GEDI_PRNTR_st_StringConfig(new Paint());
            this.stringConfig.Paint.TextSize = configPrint.Tamanho;
            this.stringConfig.Paint.TextAlign = Paint.Align.ValueOf(configPrint.Alinhamento);
            this.stringConfig.Offset = configPrint.OffSet;
            this.stringConfig.LineSpace = configPrint.LineSpace;

            switch (configPrint.Fonte)
            {
                case "NORMAL":
                    this.typeface = Typeface.Create(configPrint.Fonte, TypefaceStyle.Bold);
                    break;

                case "DEFAULT":
                    this.typeface = Typeface.Create(Typeface.Default, TypefaceStyle.Normal);
                    break;

                case "DEFAULT BOLD":
                    this.typeface = Typeface.Create(Typeface.DefaultBold, TypefaceStyle.Normal);
                    break;

                case "MONOSPACE":
                    this.typeface = Typeface.Create(Typeface.Monospace, TypefaceStyle.Normal);
                    break;

                case "SANS SERIF":
                    this.typeface = Typeface.Create(Typeface.SansSerif, TypefaceStyle.Normal);
                    break;

                case "SERIF":
                    this.typeface = Typeface.Create(Typeface.Serif, TypefaceStyle.Normal);
                    break;

                default:
                    this.typeface = Typeface.CreateFromAsset(this.mainActivity.Assets, $"fonts/{configPrint.Fonte}");
                    break;
            }

            if (this.configPrint.Negrito && this.configPrint.Italico)
            {
                this.typeface = Typeface.Create(typeface, TypefaceStyle.BoldItalic);
            }
            else if (this.configPrint.Negrito)
            {
                this.typeface = Typeface.Create(typeface, TypefaceStyle.Bold);
            }
            else if (this.configPrint.Italico)
            {
                this.typeface = Typeface.Create(typeface, TypefaceStyle.Italic);
            }

            if (this.configPrint.SubLinhado)
            {
                this.stringConfig.Paint.Flags = PaintFlags.UnderlineText;
            }

            this.stringConfig.Paint.SetTypeface(typeface);
        }

        /**
         * Método que faz a impressão do texto.
         * @param texto = Texto que será impresso
         * @throws GediException = retorna o código do erro
         * */
        public bool sPrintLine(string texto)
        {
            try
            {
                ImpressoraInit();
                this.iPrintr.DrawStringExt(this.stringConfig, texto);
                this.AvancaLinha(configPrint.AvancaLinha);
            }
            catch (GediException e)
            {
                throw new GediException(e.ErrorCode);
            }

            return true;
        }

        /**
         * Método que faz a tradução do status atual da impressora.
         * @param status = Recebe o GEDI_PRNTR_e_Status como atributo
         * @return String = Retorno o atual status da impressora
         * */
        private string traduzStatusImpressora(GEDI_PRNTR_e_Status status)
        {

            string retorno;

            if (status == GEDI_PRNTR_e_Status.Ok)
            {
                retorno = "IMPRESSORA OK";
            }
            else if (status == GEDI_PRNTR_e_Status.OutOfPaper)
            {
                retorno = "SEM PAPEL";
            }
            else if (status == GEDI_PRNTR_e_Status.Overheat)
            {
                retorno = "SUPER AQUECIMENTO";
            }
            else
            {
                retorno = "ERRO DESCONHECIDO";
            }

            return retorno;
        }
    }
}
