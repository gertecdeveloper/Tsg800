using Android.Nfc;
using Android.Nfc.Tech;
using Java.Lang;
using Java.Nio.Charset;
using Java.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GertecXamarinForms.Droid.Services
{
    class NfcLeituraGravacao
    {
        //private Android.Nfc.NfcAdapter nfcAdapter;
        private MifareClassic mifareClassic;

        // Mensagem padrão para ser usada quando o o cartão for formatado
        string MENSAGEM_PADRAO = "GERTEC";


        //Tag para ser usado no Log
        //private static readonly string Tag = NfcLerGravar.class.getSimpleName();

        private long tempInicial;
        private long tempFinal;
        private long direfenca;

        public static byte[] KEY_MIFARE_APPLICATION_DIRECTORY =
            {(byte)0xA0,(byte)0xA1,(byte)0xA2,(byte)0xA3,(byte)0xA4,(byte)0xA5};

        /**
         * Método construtor da classe.
         *
         * @param tag = contém as tag do cartão que foi lido.
         *
         * */
        public NfcLeituraGravacao(Tag tag)
        {
            this.mifareClassic = MifareClassic.Get(tag);
            this.GravaTempoInicia();
        }

        /**
         *
         * Método que grava os milesegundos na inicialização desta class
         *
         * */
        private void GravaTempoInicia()
        {
            this.tempInicial = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public List<object> LerTodosOsSetoresDoCartao()
        {
            byte[] byteRead;            // Irá armazenar os byte lidos do cartão
            bool auth = false;       // Valida se existe ou não permissão para ler o bloco
            int quantSetores = 0;       // Quantidade de setores existentes no cartão
            int blocoCount = 0;         // Quantidades de blocos existentes no cartão
            int blocoIndex = 0;         // Irá armazenar o indice que esta sendo lido do cartão
            List<object> ByteRetorno = new List<object>(); // Lista com a leitura de todos os blocos e setores

            try
            {

                // Faz a conexão com o cartão
                ValidConexaoCartao();

                // Irá armazenar a quantidade de setores existentes no cartão
                quantSetores = MifareClassic.BlockSize;

                // Percorre todos os setores existentes no cartão
                for (int i = 0; i < quantSetores; i++)
                {

                    // Faz a validação de permissão para a leitura do bloco A
                    auth = ValidPermissaoBlocoA(i);

                    if (!auth)
                    {
                        // Faz a validação de permissão para a leitura do bloco B
                        auth = ValidPermissaoBlocoB(i);
                    }

                    if (auth)
                    {

                        // Busca a quanidade de blocos em um setor
                        blocoCount = mifareClassic.GetBlockCountInSector(i);

                        // Percore todos os blocos do setor
                        for (int j = 0; j < blocoCount; j++)
                        {
                            // É necessário fazer novamente a validação de permissão Bloco a bloco

                            // Faz a validação de permissão para a leitura do bloco A
                            auth = ValidPermissaoBlocoA(j);

                            if (!auth)
                            {
                                // Faz a validação de permissão para a leitura do bloco B
                                auth = ValidPermissaoBlocoB(j);
                            }

                            if (auth)
                            {

                                // seta o indice do setor dentro do bloco
                                blocoIndex = mifareClassic.SectorToBlock(j);

                                // Faz a leitura de um BLOCO no setor
                                byteRead = mifareClassic.ReadBlock(blocoIndex);
                                ByteRetorno.Add(byteRead);
                            }
                        }
                    }

                }

            }
            catch (IOException e)
            {
                throw new IOException(e.Message);

            }
            finally
            {
                // Fecha a conexão com o cartão
                DesconectaCartao();
                this.GravaTempoFinal();
            }

            return ByteRetorno;
        }

        public byte[] LerUmSetoresDoCartao(int bloco, int setor)
        {

            byte[] byteRead = null;     // Irá armazenar os byte lidos do cartão
            bool auth = false;       // Valida se existe ou não permissão para ler o bloco
            int blocoIndex = 0;         // Irá armazenar o indice que esta sendo lido do cartão

            try
            {

                // Faz a conexão com o cartão
                mifareClassic.Connect();


                // Faz a validação de permissão para a leitura do bloco A
                auth = ValidPermissaoBlocoA(bloco);

                if (!auth)
                {
                    // Faz a validação de permissão para a leitura do bloco B
                    auth = ValidPermissaoBlocoB(bloco);

                    if (!auth)
                    {
                        throw new System.Exception("Falha na validação de senha.");
                    }

                }

                if (auth)
                {

                    // É necessário fazer novamente a validação de permissão Bloco a bloco

                    // Faz a validação de permissão para a leitura do bloco A
                    auth = ValidPermissaoBlocoA(setor);

                    if (!auth)
                    {
                        // Faz a validação de permissão para a leitura do bloco B
                        auth = ValidPermissaoBlocoB(setor);
                    }

                    if (auth)
                    {

                        // seta o indice do setor dentro do bloco
                        blocoIndex = mifareClassic.SectorToBlock(setor);

                        // Faz a leitura de um BLOCO no setor
                        byteRead = mifareClassic.ReadBlock(blocoIndex);

                    }
                }

            }
            catch (IOException e)
            {
                throw new System.Exception(e.Message);
            }
            finally
            {
                DesconectaCartao();
                this.GravaTempoFinal();
            }

            return byteRead;
        }

        /**
        *
        * Método faz a gravação de uma mensagem em um bloco específico
        * no cartão.
        *
        * A mensagem que será gravada não deve ser superior a 16 bits.
        *
        * A leitura sempre será retornada em Bytes.
        *
        * @param bloco = número que bloco que deve ser lido
        * @param mensagem = número do setor existente dentro do bloco que será lido
        *
        * @throws IOException
        *
        * @return true = caso haja um erro na gravação, será tratada no throw
        *
        * */
        public bool GravaSetorCartao(int bloco, byte[] mensagem)
        {

            try
            {
                // Valida a conexão com o cartão
                ValidConexaoCartao();

                // Valida a quantidade de bytes que serão gravados
                if (mensagem.Length != 16)
                {
                    throw new IllegalArgumentException("A mensagem não contem 16 bits");
                }
                // Grava no cartão
                mifareClassic.WriteBlock(bloco, mensagem);

            }
            catch (IOException e)
            {
                throw new System.Exception(e.Message);

            }
            finally
            {
                DesconectaCartao();
                this.GravaTempoFinal();
            }

            return true;
        }

        /**
         *
         * Método que faz o incremento de um valor em um bloco específico
         * no cartão.
         *
         * O valor a ser incrementado será sempre o INTEIRO
         *
         *
         * @param bloco = Número que bloco que deve ser incrementado
         * @param valor = Valor que será incrementado ao já existente no bloco
         *
         * @throws IOException
         *
         * @return true = caso haja um erro na gravação será tratado no throw
         *
         * */
        public bool IncrementaValorCartao(int bloco, int valor)
        {
            try
            {
                // Valida a conexão com o cartão
                ValidConexaoCartao();
                // Valida o valor a ser gravado
                ValidaValor(valor);
                // Incrementa o valor no cartão
                mifareClassic.Increment(bloco, valor);
            }
            catch (IOException e)
            {
                throw new System.Exception(e.Message);
            }
            finally
            {
                DesconectaCartao();
                this.GravaTempoFinal();
            }

            return true;
        }

        /**
        *
        * Método que faz o decremento de um valor em um bloco específico
        * no cartão.
        *
        * O valor a ser decrementado será sempre o INTEIRO
        *
        *
        * @param bloco = Número que bloco que deve ser incrementado
        * @param valor = Valor que será incrementado ao já existente no bloco
        *
        * @throws IOException
        *
        * @return true = caso haja um erro na gravação será tratado no throw
        *
        * */
        public bool DecrementaValorCartao(int bloco, int valor)
        {
            try
            {
                // Valida conexão
                ValidConexaoCartao();
                // Valida o valor
                ValidaValor(valor);
                // Decrementa o valor do cartão
                mifareClassic.Decrement(bloco, valor);
            }
            catch (IOException e)
            {
                throw new System.Exception(e.Message);
            }
            finally
            {
                DesconectaCartao();
                this.GravaTempoFinal();
            }

            return true;
        }

        /**
         *
         * Método faz a gravação de uma nova mensagem no cartão.
         *
         * Essa nova mensagem será códificada usando o padrão UTF-8.
         *
         * @param ndef = Contém as informações do cartão que esta sendo lido.
         * @param mensagem = Mensagem que será gravada no cartão
         *
         * @throws IOException
         * @throws FormatException
         *
         * @return boolean =>  True = Mensagem Gravada / False = Erro ao gravar mensagem
         *
         * */
        public bool GavarMensagemCartao(Ndef ndef, string mensagem)
        {
            bool retorno = false;
            try
            {
                if (ndef != null)
                {

                    ndef.Connect();
                    NdefRecord mimeRecord = null;

                    Java.Lang.String str = new Java.Lang.String(mensagem);
                    
                    mimeRecord = NdefRecord.CreateMime
                        ("UTF-8", str.GetBytes(Charset.ForName("UTF-8")));
                       
                    ndef.WriteNdefMessage(new NdefMessage(mimeRecord));
                    ndef.Close();
                    retorno = true;

                }
                else
                {
                    retorno = FormataCartao(ndef);
                }
            }
            catch (System.FormatException e)
            {
                throw new System.FormatException(e.Message);

            }
            catch (IOException e)
            {
                throw new IOException(e.Message);

            }
            finally
            {
                this.GravaTempoFinal();
            }

            return retorno;
        }

        /**
         *
         * Método faz a formatação do cartão.
         *
         * A formatação do cartão só é necessario na sua primeira gravação.
         *
         * Após já existir algum valor gravado no cartão, não será possível formata-lo
         * novamente.
         *
         * @param ndef = Contém as informações do cartão que esta sendo lido.
         *
         * @throws IOException
         * @throws FormatException
         *
         * @return boolean =>  True = Cartão Formatado / False = Cartão não formatado
         *
         * */
        public bool FormataCartao(Ndef ndef)
        {
            bool retorno = false;

            NdefFormatable ndefFormatable = NdefFormatable.Get(ndef.Tag);
            Java.Lang.String msg = new Java.Lang.String(MENSAGEM_PADRAO);
            try
            {

                if (ndefFormatable == null)
                {
                    return retorno;
                }

                if (!ndefFormatable.IsConnected)
                {
                    ndefFormatable.Connect();
                }
                ndefFormatable.Format(new NdefMessage(NdefRecord.CreateMime
                    ("UTF-8", msg.GetBytes(Charset.ForName("UTF-8")))));
                ndefFormatable.Close();
                retorno = true;

            }
            catch (IOException e)
            {
                throw new IOException(e.Message);
            }
            catch (System.FormatException e)
            {
                throw new System.FormatException(e.Message);
            }
            finally
            {
                this.GravaTempoFinal();
            }


            return retorno;
        }

        /**
        *
        * Método faz a gravação de uma nova mensagem no cartão.
        *
        * Essa nova mensagem será códificada usando o padrão UTF-8.
        *
        * @param ndef = Contém as informações do cartão que esta sendo lido.
        *
        * @throws IOException
        * @throws FormatException
        *
        * @return String = contém a mensagem que esta gravada no cartão
        *
        * */
        public string RetornaMensagemGravadaCartao(Ndef ndef)
        {

            string message;

            try
            {
                if (ndef == null)
                {
                    throw new System.Exception("Não foi possível ler o cartão.");
                }

                if (!ndef.IsConnected)
                {
                    ndef.Connect();
                }

                NdefMessage ndefMessage = ndef.NdefMessage;
                if (ndefMessage == null)
                {
                    throw new System.Exception("Não foi possível ler o cartão.");
                }
                else
                {
                    message = Encoding.UTF8.GetString(ndefMessage.GetRecords()[0].GetPayload());
                    Console.WriteLine(message);
                }

            }
            catch (IOException e)
            {
                throw new System.Exception(e.Message);
            }
            catch (System.FormatException e)
            {
                throw new System.Exception(e.Message);
            }
            catch (System.Exception e)
            {
                throw new System.Exception(e.Message);
            }
            finally
            {
                GravaTempoFinal();
            }
            return message;
        }

        /**
         *
         * Método que válida o valor a ser incrementado ou decrementado do cartão.
         *
         * @param valor = Valor a ser incrementado ou decrementado do cartão
         *
         * @throws IllegalArgumentException
         *
         * */
        private void ValidaValor(int valor)
        {
            if (valor < 0)
            {
                throw new IllegalArgumentException("O valor não poder ser negativo.");
            }
        }

        /**
         *
         * Método que faz a validação de senha
         * para leitura e escrita de um bloco no SETOR B
         *
         * @param bloco  = Número do bloco que será validado a permissão
         *
         * @throws IOException
         *
         * @return boolean = Caso falso é porque não existe a permissão para leitura do bloco
         *
         * */
        private bool ValidPermissaoBlocoB(int bloco)
        {

            bool retorno = false;


            try
            {

                if (mifareClassic.AuthenticateSectorWithKeyB(bloco, (byte[])MifareClassic.KeyMifareApplicationDirectory))
                {

                    retorno = true;

                }
                else if (mifareClassic.AuthenticateSectorWithKeyB(bloco, (byte[])MifareClassic.KeyDefault))
                {
                    retorno = true;

                }
                else if (mifareClassic.AuthenticateSectorWithKeyB(bloco, (byte[])MifareClassic.KeyNfcForum))
                {
                    retorno = true;

                }
                else
                {
                    retorno = false;
                }
            }
            catch (IOException e)
            {
                throw new System.Exception(e.Message);

            }

            return retorno;
        }

        private void GravaTempoFinal()
        {
            this.tempFinal = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        /**
         *
         * Método que faz a validação de senha
         * para leitura e escrita de um bloco no SETOR A
         *
         * @param bloco  = Número do bloco que será validado a permissão
         *
         * @throws IOException
         *
         * @return boolean = Caso falso é porque não existe a permissão para leitura do bloco
         *
         * */
        private bool ValidPermissaoBlocoA(int bloco)
        {

            bool retorno = false;

            try
            {

                if (mifareClassic.AuthenticateSectorWithKeyA(bloco, (byte[])MifareClassic.KeyMifareApplicationDirectory))
                {
                    retorno = true;

                }
                else if (mifareClassic.AuthenticateSectorWithKeyA(bloco, (byte[])MifareClassic.KeyDefault))
                {
                    retorno = true;

                }
                else if (mifareClassic.AuthenticateSectorWithKeyA(bloco, (byte[])MifareClassic.KeyNfcForum))
                {
                    retorno = true;

                }
                else
                {
                    retorno = false;
                }
            }
            catch (IOException e)
            {
                throw new System.Exception(e.Message);
            }

            return retorno;
        }

        /**
         *
         * Método que válida se existe ou não conexão com o atual cartão.
         *
         * Caso não haja conexão aberta com o cartão, esse método irá abrir
         * uma nova conexão.
         *
         * */
        private void ValidConexaoCartao()
        {
            if (!mifareClassic.IsConnected)
            {
                ConectarCartao();
            }
        }

        /**
         *
         * Método que faz a conexão com o cartão.
         *
         * @throws IOException
         *
         * */
        private void ConectarCartao()
        {
            try
            {
                mifareClassic.Connect();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        /**
         *
         * Método que desconecta o cartão.
         *
         * @throws IOException
         *
         * */
        private void DesconectaCartao()
        {
            try
            {
                mifareClassic.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        /**
         *
         * Método que retorna a quantidade de segundos que foram
         * necessário para faz uma execução.
         *
         * @return Long = Segundos que foram usado para concluir um processo.
         *
         *
         * */
        public long RetornaTempoDeExeculcaoSegundos()
        {
            this.direfenca = (this.tempFinal - this.tempInicial);
            direfenca /= 60;
            return direfenca;
        }

        /**
         *
         * Método que retorna o ID do cartão já convetido em Hexadecimal
         *
         * @return String = Id do cartão
         *
         * */
        public string IdCartaoHexadecimal()
        {

            byte[] idCartao = mifareClassic.Tag.GetId();
            long result = 0;

            if (idCartao == null) return "";

            for (int i = idCartao.Length - 1; i >= 0; --i)
            {
                result <<= 8;
                result |= idCartao[i] & 0x0FF;
            }
            return Long.ToString(result);
        }

        /**
         *
         * Método que retorna o ID do cartão em Bytes
         *
         * @return byte[] = Id do cartão
         *
         * */
        public byte[] CartaoId()
        {
            return mifareClassic.Tag.GetId();
        }

        /**
         *
         * Método que gera um String randomicamente para ser usada em testes.
         *
         * @return String = Texto gerado randomicamente
         *
         * */
        public string GeraString()
        {

            UUID uuid = UUID.RandomUUID();
            string myRandom = uuid.ToString();

            return myRandom.Substring(0, 30);

        }
    }
}
