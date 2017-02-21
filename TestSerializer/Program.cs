using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Web;

namespace TestSerializer
{
    public class Program
    {
        [Serializable]
        public class SysAdminUnit
        {
            public List<Item> items { get; set; }
            public SysAdminUnit()
            {
                items = new List<Item>();
            }
        }
        [Serializable]
        public class Item
        {
            public string Name { get; set; }
        }
        [Serializable]
        [XmlRoot(ElementName = "SysAdminUnit")]
        public class SysAdminUnitResponed
        {
            [XmlElement(ElementName = "Item")]
            public List<ItemRespond> items { get; set; }
            public SysAdminUnitResponed()
            {
                items = new List<ItemRespond>();
            }
        }
        [Serializable]
        public class ItemRespond
        {
            public bool IsSuccess { get; set; }
            public int ErrorCode { get; set; }
            public string ErrorDescription { get; set; }
            public Guid UserOneCGuid { get; set; }
        }
        private static MemoryStream Serializ(Object obj)
        {
            MemoryStream stream = new MemoryStream();
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            serializer.Serialize(stream, obj);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }
        public static T Deserializ<T>(Stream stream) where T : class
        {
            T obj = Activator.CreateInstance<T>();
            try
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }
                obj = (T)serializer.Deserialize(stream);
            }
            catch (Exception ex)
            { }
            return obj;
        }
        public static void Main(string[] args)
        {
            /*
            var unitRespond = new SysAdminUnitResponed();
            unitRespond.items.Add(new ItemRespond() { ErrorCode = 1, ErrorDescription = "error1", IsSuccess = true, UserOneCGuid = Guid.NewGuid() });
            unitRespond.items.Add(new ItemRespond() { ErrorCode = 2, ErrorDescription = "error2", IsSuccess = true, UserOneCGuid = Guid.NewGuid() });
            unitRespond.items.Add(new ItemRespond() { ErrorCode = 3, ErrorDescription = "error3", IsSuccess = false, UserOneCGuid = Guid.NewGuid() });
            unitRespond.items.Add(new ItemRespond() { ErrorCode = 4, ErrorDescription = "error4", IsSuccess = true, UserOneCGuid = Guid.NewGuid() });

            var stream = Serializ(unitRespond);
            var streamReader = new StreamReader(stream);
            Console.WriteLine(streamReader.ReadToEnd());
                
            var deUnit = Deserializ<SysAdminUnitResponed>(stream);*/
            GetUserId();
            Console.ReadLine();
        }
        public static void GetUserId()
        {
            SysAdminUnitResponed deUnits = null;
            var unit = new SysAdminUnit();
            /*unit.items.Add(new Item() { Name = "Кирилл" });
            unit.items.Add(new Item() { Name = "Ирина" });
            unit.items.Add(new Item() { Name = "Евгения" });
            unit.items.Add(new Item() { Name = "Александр" });*/
            var stream = Serializ(unit);
            var streamReader = new StreamReader(stream);
            var rspStm = SendRequest(streamReader.ReadToEnd(), "http://localhost:58567/WebApi/GetUserId");

            deUnits = Deserializ<SysAdminUnitResponed>(rspStm);
            if (deUnits != null)
            {
                foreach (var deUnit in deUnits.items)
                {
                    //Сохранение в базу
                }
            }


        }
        public static Stream SendRequest(string Xml, string URL)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);

            request.ContentType = "text/xml; charset=UTF-8";
            request.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(Xml); //Encoding.UTF8.GetBytes(HttpUtility.UrlEncode(Xml));
            request.ContentLength = byteArray.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(byteArray, 0, byteArray.Length);
            }
            try
            {
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                return response.GetResponseStream();
            }
            catch (Exception ex)
            {
            }
            return new MemoryStream();

        }
    }
}
