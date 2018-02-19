using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diadoc.Api;
using Korus.Eds.Api;
namespace EDOApi
{
    public enum EdoOperator
    {
    Diadoc, Courier
    }
    public enum DocType
    {
    Nonformal,
    Invoice,
    Act,
    Torg12,
    UTD,
    UKD
    }
    public enum UTDFunction
    { 
    INV,
    INVDOP,
    DOP
    }
    public class Party 
    {
    }
    public class Document {
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public DocType Type { get; set; }
        public Party Sender { get; set; }
        public Party Receiver { get; set; }
        public Decimal Amount { get; set; }
    }
    public interface IEDOConnector
    {
        EdoOperator Operator
        {
            get;
            set;
        }
         string Login { get; set; }
         string Password { get; set; }
         Boolean Connect();
       
        
    }
    public class EDOConnector : IEDOConnector
    {
        public EdoOperator Operator
        {
            get;
            set;
        }
        public string Login { get; set; }
        public string Password { get; set; }
        private string Secret { get; set; }
        public Boolean Connect()
        {
            return false;
        }
        public EDOConnector()
        { 
        }
        public static EDOConnector construct(EdoOperator EdoOperator)
        {
            switch (EdoOperator)
            {
                case EdoOperator.Courier:
                    {
                        return new CourierConnector ();
                    }
                case EdoOperator.Diadoc:
                    {
                        return new DiadocConnector();
                    }
                default:
                    {
                        return new EDOConnector();                        
                    }
            }
        }
    }
    public class CourierConnector : EDOConnector, IEDOConnector
    {
        public const EdoOperator Operator = EdoOperator.Courier;
        private Korus.Eds.Api.RestClient  client;
        private string token;
        public Boolean Connect()
        {
            RestSharp.IRestResponse<Korus.Eds.Api.LogonResponse> resp = client.Login(this.Login, this.Password);
            if (resp.StatusCode == System.Net .HttpStatusCode.OK)
            {
                this.token = resp.Content;
                return true;
            }


            return false;        
        }
        public CourierConnector()
        {
            client = new RestClient();
        
        }

    }
    public class DiadocConnector :EDOConnector , IEDOConnector
    {
        public const EdoOperator Operator =EdoOperator.Diadoc;
        public const string apiUrl = "a";
        private Diadoc.Api.Cryptography.WinApiCrypt crypt;
        public string ApiKey { get; set; }
        private string token;
        private Diadoc.Api.DiadocApi api;
        public Boolean Connect()
        {
            try
            {
                this.token = api.Authenticate(this.Login, this.Password);
            }
            catch
            {
                return false;
            }
            return true;
            
        }
        public DiadocConnector ()
        {
            try
            {
                api = new Diadoc.Api.DiadocApi(ApiKey, apiUrl, crypt);
            }
            catch
            { 
                    
            }
        }
    }
}
