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
    public enum DocStatus
    { 
    New,
    InProcess,
    Completed,
    Deleted
    }
    public class Party 
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Inn { get; set; }
    }
    public class Document {
        public string Id { get; set; } 
        public string Number { get; set; }
        public Boolean IsDraft { get; set; }
        public DateTime Date { get; set; }
        public DocType Type { get; set; }
        public DocStatus Status { get; set; }
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
         Boolean  Connect();
         Document getDocument(string Id);
         List<Document> getDocumentList(DateTime startDate, DateTime endDate);
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
        public virtual Boolean Connect()
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
        public virtual Document getDocument(string Id)
        {
        return new Document();
        }
        public virtual List<Document> getDocumentList()
        { 
        return new List<Document>();
        }
    }
    public class CourierConnector : EDOConnector, IEDOConnector
    {
        public const EdoOperator Operator = EdoOperator.Courier;
        private Korus.Eds.Api.RestClient  client;
        private string token;
        public override Boolean Connect()
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
        public const string apiUrl = "https://diadoc-api.kontur.ru";
        private Diadoc.Api.Cryptography.WinApiCrypt crypt;
        public string ApiKey { get; set; }
        private string token;
        private Diadoc.Api.DiadocApi api;
        public override Boolean Connect()
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
                this.ApiKey = "gradient-c5a5fb07-eafa-495c-9240-557096c92fff";
                this.crypt = new Diadoc.Api.Cryptography.WinApiCrypt();
                this.api = new Diadoc.Api.DiadocApi(this.ApiKey, apiUrl, this.crypt);
            }
            catch
            { 
                    
            }
        }
    }
}
