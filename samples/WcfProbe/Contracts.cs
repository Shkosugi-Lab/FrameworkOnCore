using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace WcfProbe
{
    [ServiceContract(Namespace = "http://probe.example/calculator")]
    public interface ICalculator
    {
        [OperationContract]
        int Add(int a, int b);

        [OperationContract]
        [FaultContract(typeof(CalculationFault))]
        int Divide(int a, int b);

        [OperationContract]
        Order Price(Order order);

        [OperationContract]
        int Calls();
    }

    [DataContract(Namespace = "http://probe.example/calculator")]
    public class CalculationFault
    {
        [DataMember] public string Problem { get; set; }
        [DataMember] public int Dividend { get; set; }
    }

    [DataContract(Namespace = "http://probe.example/calculator")]
    public class Order
    {
        [DataMember] public int Id { get; set; }
        [DataMember] public string Item { get; set; }
        [DataMember] public int Quantity { get; set; }
        [DataMember] public decimal UnitPrice { get; set; }
        [DataMember] public decimal Total { get; set; }
        [DataMember] public DateTime Placed { get; set; }
        [DataMember] public List<string> Notes { get; set; }
    }

    [ServiceContract]
    public interface ICatalog
    {
        [OperationContract]
        [WebGet(UriTemplate = "items/{id}", ResponseFormat = WebMessageFormat.Json)]
        Item GetItem(string id);

        [OperationContract]
        [WebGet(UriTemplate = "items?max={max}", ResponseFormat = WebMessageFormat.Json)]
        List<Item> GetItems(int max);

        [OperationContract]
        [WebGet(UriTemplate = "items/{id}/xml")]
        Item GetItemXml(string id);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "echo", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Item Echo(Item item);
    }

    [DataContract(Namespace = "http://probe.example/catalog")]
    public class Item
    {
        [DataMember] public string Id { get; set; }
        [DataMember] public string Name { get; set; }
        [DataMember] public decimal Price { get; set; }
        [DataMember] public bool InStock { get; set; }
    }

    [ServiceContract(Namespace = "http://probe.example/greeter")]
    public interface IGreeter
    {
        [OperationContract]
        string Greet(string name);
    }
}
