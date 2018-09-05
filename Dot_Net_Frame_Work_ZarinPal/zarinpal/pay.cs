using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace zarinpal
{
    public class pay
    {
        private string _MerchantID, _Description, _CallbackURL, _Email, _Mobile, _urlpay = "https://www.zarinpal.com/pg/StartPay/";
        private int _Amount;
        public delegate void PArgs(object sender, PayArgs e);
        public event PArgs OnPaymentAction;
        /// <summary>
        /// Initilize a payment
        /// </summary>
        /// <param name="MerchantID">Reciever Unique ID</param>
        /// <param name="Amount">Amount to send</param>
        /// <param name="Description"></param>
        /// <param name="CallbackURL">Where to redirect after finish</param>
        /// <param name="Email">Sender mail address ( optionl )</param>
        /// <param name="Mobile">Sender mobile number ( optional )</param>
        /// <returns>Returns authority id</returns>
        public pay(string MerchantID, int Amount, string Description, string CallbackURL, string Email = "", string Mobile = "")
        {
            _MerchantID = MerchantID;
            _Amount = Amount;
            _Description = Description;
            _CallbackURL = CallbackURL;
            _Email = Email;
            _Mobile = Mobile;
        }
        /// <summary>
        /// Starts payment
        /// </summary>
        /// <returns>Returns payment autohority id</returns>
        public string StartPay()
        {
            try
            {
                ServiceReference.PaymentGatewayImplementationServicePortTypeClient request = new ServiceReference.PaymentGatewayImplementationServicePortTypeClient();
                string autohority = string.Empty;
                int value = request.PaymentRequest(_MerchantID, _Amount, _Description, _Email, _Mobile, _CallbackURL, out autohority);
                if (value > 0)
                {
                    new Thread(() =>
                    {
                        CheckPaymentStatus(autohority);
                    }).Start();
                    return autohority;
                }
                else
                {
                    return value.ToString();
                }
            }
            catch
            {
                return null;
            }
        }
        public string URL
        {
            get { return _urlpay; }
        }
        private void CheckPaymentStatus(string autohority)
        {
            ServiceReference.PaymentGatewayImplementationServicePortTypeClient request = new ServiceReference.PaymentGatewayImplementationServicePortTypeClient();
            long refID = -1;
            bool stopit = false;
            long curtick = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
            while (true)
            {
                if (stopit)
                    break;
                int verf = -21;
                try
                {
                    verf = request.PaymentVerification(_MerchantID, autohority, _Amount, out refID);
                }
                catch
                {

                }
                if (verf > 0)
                {
                    stopit = true;
                    if (OnPaymentAction != null)
                    {
                        OnPaymentAction(this, new PayArgs(verf, autohority, refID));
                    }
                }
                else
                {
                    if (!stopit && verf != -21)
                    {
                        stopit = true;
                        if (OnPaymentAction != null)
                        {
                            OnPaymentAction(this, new PayArgs(verf, autohority, refID));
                        }
                    }
                }
                long curtime = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
                if((curtime - curtick) > 1850) // 30 * 60 +- 50
                {
                    if(!stopit)
                    {
                        OnPaymentAction(this, new PayArgs(-22, autohority, refID));
                        stopit = true;
                    }
                }
            }
        }
        public string MerchantID
        {
            get { return _MerchantID; }
        }
        public string Description
        {
            get { return _Description; }
        }
        public string CallbackURL
        {
            get { return _CallbackURL; }
        }
        public string Email
        {
            get { return _Email; }
        }
        public string Mobile
        {
            get { return _Mobile; }
        }
        public int Amount
        {
            get { return _Amount; }
        }
        public class PayArgs
        {
            private int _Status;
            private string _Autohority;
            private long _RefID;
            public PayArgs(int Status, string Autohority, long RefID)
            {
                _Status = Status;
                _Autohority = Autohority;
                _RefID = RefID;
            }
            public int Status
            {
                get { return _Status; }
            }
            public string Autohority
            {
                get { return _Autohority; }
            }
            public long RefID
            {
                get { return _RefID; }
            }
            private string GetFromLastSlash(string text)
            {
                int where = text.LastIndexOf('/');
                return text.Substring(where);
            }
        }
    }
}
