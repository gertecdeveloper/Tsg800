using Android.App;
using Android.Content;
using Android.Nfc.Tech;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.IO;

namespace GertecXamarinForms.Droid.Services.Fragment
{
    [Obsolete]
    class NFCReadFragment : DialogFragment
    {
        private NfcLeituraGravacao nfcLeituraGravacao;

        public static string TAG = "NFCReadFragment";
        private TextView mTvMessage;
        private Nfc mListener;

        public static NFCReadFragment NewInstance()
        {
            return new NFCReadFragment();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_leitura, container, false);
            InitViews(view);
            return view;
        }

        private void InitViews(View view)
        {
            mTvMessage = view.FindViewById<TextView>(Resource.Id.tv_message);
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            mListener = (Nfc)context;
            mListener.OnDialogDisplayed();
        }

        public override void OnDetach()
        {
            base.OnDetach();
            mListener.OnDialogDismissed();
        }

        public void OnNfcDetected(Ndef ndef)
        {
            nfcLeituraGravacao = new NfcLeituraGravacao(ndef.Tag);
            ReadFromNFC(ndef);
        }

        /**
         * Este método irá apresentar na tela as atuais mensagens cadastadas no cartão
         *
         * @param ndef = contém as informações do cartão que acabou de ser lido.
         *
         * @exception IOException
         * @exception FormatException
         * @exception Exception
         *
         * */
        private void ReadFromNFC(Ndef ndef)
        {
            string mensagem;
            string idCartao;
            long tempoExecucao;

            try
            {
                // Recebe a leitura das atuais mensagens cadastradas no cartão
                mensagem =  nfcLeituraGravacao.RetornaMensagemGravadaCartao(ndef);
                idCartao = nfcLeituraGravacao.IdCartaoHexadecimal();

                // Recebe o tempo total de execução da operação de leitura
                tempoExecucao = nfcLeituraGravacao.RetornaTempoDeExeculcaoSegundos();

                if (mensagem.Equals("")){
                    mTvMessage.Text = "Não existe mensagem gravada no cartão";
                }else {
                    mTvMessage.Text = "ID Cartão: " + idCartao + "\n" + mensagem +
                        "\n\nTempo de execução: " + tempoExecucao + " segundos";
                }

            }
            catch (IOException e){
                Toast.MakeText(Activity, e.Message, ToastLength.Long).Show();
                //Toast.MakeText(this.ApplicationContext, "Tipo de cartão não suportado.", ToastLength.Short).Show();
            }catch (FormatException e){
                Toast.MakeText(Activity, e.Message, ToastLength.Long).Show();
            }
            catch (Exception e){
                Toast.MakeText(Activity, e.Message, ToastLength.Long).Show();
            }
        }
    }
}
