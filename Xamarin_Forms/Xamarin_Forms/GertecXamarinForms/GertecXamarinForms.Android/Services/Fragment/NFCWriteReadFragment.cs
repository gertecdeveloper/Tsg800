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
    public class NFCWriteReadFragment : DialogFragment
    {
        private NfcLeituraGravacao nfcLeituraGravacao;
        public static string TAG = "NFCWriteReadFragment";
        private TextView tvStatus;
        private EditText editProcesso;
        private ProgressBar mProgress;
        private Nfc mListener;

        public static NFCWriteReadFragment NewInstance()
        {
            return new NFCWriteReadFragment();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_forceteste, container, false);
            InitViews(view);
            return view;
        }

        private void InitViews(View view)
        {
            editProcesso = view.FindViewById<EditText>(Resource.Id.editProcesso);
            mProgress = view.FindViewById<ProgressBar>(Resource.Id.progress); 
            tvStatus = view.FindViewById<TextView>(Resource.Id.tv_status);
        }

        public override void OnAttach(Context context){
            base.OnAttach(context);
            mListener = (Nfc)context;
            mListener.OnDialogDisplayed();
        }

        public override void OnDetach(){
            base.OnDetach();
            mListener.OnDialogDismissed();
        }

        public void onNfcDetected(Ndef ndef, string message)
        {
            nfcLeituraGravacao = new NfcLeituraGravacao(ndef.Tag);
            if (WriteToNfc(ndef, message))
            {
                ReadFromNFC(ndef);
            }
        }


        /**
         * Este método irá grava uma nova mensagem no cartão.
         *
         * @param ndef = contém as informações do cartão que acabou de ser lido.
         * @param message  = mensagem que será gravada no cartão
         *
         * @exception IOException
         * @exception FormatException
         *
         * @return boolean = Sinalize que a mensagem foi gravada
         *
         * */
        private bool WriteToNfc(Ndef ndef, string message)
        {
            bool retorno = false;
            try
            {
                retorno = nfcLeituraGravacao.GavarMensagemCartao(ndef, message);

                if (retorno)
                {
                    editProcesso.Text = "Código ID:" + nfcLeituraGravacao.IdCartaoHexadecimal() +
                        "\nCódigo gravado: " + message + "\n";
                
                }
                else
                {
                    editProcesso.Text = "Falha ao gravar mensagem";
                }

            }
            catch (FormatException e)
            {
                Toast.MakeText(Activity, e.Message, ToastLength.Long).Show();
            }
            catch (IOException e)
            {
                Toast.MakeText(Activity, e.Message, ToastLength.Long).Show();
            }
            catch (Exception e)
            {
                Toast.MakeText(Activity, e.Message, ToastLength.Long).Show();
            }
            finally 
            {
                mProgress.Visibility = ViewStates.Gone;
            }
            return retorno;
        }

        /**
         * Este método irá grava uma nova mensagem no cartão.
         *
         * @param ndef = contém as informações do cartão que acabou de ser lido.
         *
         * @exception IOException
         * @exception FormatException
         *
         *
         * */
        private void ReadFromNFC(Ndef ndef)
        {
            string editTex;
            string mensagem;
            long tempoExecucao;

            try
            {
                mensagem = nfcLeituraGravacao.RetornaMensagemGravadaCartao(ndef);
                tempoExecucao = nfcLeituraGravacao.RetornaTempoDeExeculcaoSegundos();

                if (mensagem.Equals(""))
                {
                    tvStatus.Text = "Nenhuma mensagem cadastrada.";
                }
                else 
                {
                    tvStatus.Text = ("Aproxime o cartão");
                    editTex = editProcesso.Text.ToString();
                    editProcesso.Text = (editTex + "\nCódigo ID:" + nfcLeituraGravacao.IdCartaoHexadecimal() + "\nLeitura código: " +
                        mensagem + "\n\nTempo de execução: " + tempoExecucao + " segundos");
                }
            }
            catch (IOException e)
            {
                Toast.MakeText(Activity, e.Message, ToastLength.Long).Show();

            }
            catch (FormatException e)
            {
                Toast.MakeText(Activity, e.Message, ToastLength.Long).Show();

            }
            catch (Exception e)
            {
                Toast.MakeText(Activity, e.Message, ToastLength.Long).Show();
            }
        }

    }
}
