using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Nfc.Tech;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GertecXamarinForms.Droid.Services.Fragment
{
    [Obsolete]
    class NFCWriteFragment : DialogFragment
    {
        private NfcLeituraGravacao nfcLeituraGravacao;
        public static string TAG = "NFCWriteFragment";
        private TextView mTvMessage;
        private ProgressBar mProgress;
        private Nfc mListener;

        public static NFCWriteFragment newInstance()
        { 
            return new NFCWriteFragment();
        }

        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_gravacao, container, false);
            InitViews(view);
            return view;
        }

        
        private void InitViews(View view)
        {
            mTvMessage = view.FindViewById<TextView>(Resource.Id.tv_message);
            mProgress = view.FindViewById<ProgressBar>(Resource.Id.progress);
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

        public void OnNfcDetected(Ndef ndef, string messageToWrite)
        {
            nfcLeituraGravacao = new NfcLeituraGravacao(ndef.Tag);
            mProgress.Visibility = View.Visibility;
            writeToNfc(ndef, messageToWrite);
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
         * */
        private void writeToNfc(Ndef ndef, string message)
        {
            long tempoExecucao;

            mTvMessage.Text = "Gravando informação..";

            if(ndef != null)
            {
                try
                {
                    nfcLeituraGravacao.GavarMensagemCartao(ndef, message);
                    tempoExecucao = nfcLeituraGravacao.RetornaTempoDeExeculcaoSegundos();

                    mTvMessage.Text = ("Sucesso ao gravar informação!" +
                        "\n\nTempo de execução: "+ tempoExecucao + " segundos");

                } 
                catch (IOException e) 
                {
                    Toast.MakeText(Activity, e.Message, ToastLength.Long).Show();
                    Console.WriteLine(e.StackTrace);
                    mTvMessage.Text = "Erro ao gravar informação!";
                }
                catch(FormatException e)
                {
                    Toast.MakeText(Activity, e.Message, ToastLength.Long).Show();
                    Console.WriteLine(e.StackTrace);
                }
                finally
                {
                    mProgress.Visibility = ViewStates.Gone;
                }
            }
            else
            {
                Toast.MakeText(Activity, "Não foi possível ler este cartão", ToastLength.Long).Show();
                
            }
        }
    }
}
