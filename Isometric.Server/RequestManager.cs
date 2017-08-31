using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Isometric.Server
{
    public class RequestManager<T>
    {
        public delegate Dictionary<string, dynamic> ResponseGenerator(Dictionary<string, dynamic> args, Connection<T> connection);

        public delegate ResponseGenerator ExceptionResponseGenerator(Exception ex);



        private readonly Dictionary<string, ResponsePair<T>> _responses;

        private readonly ResponseGenerator _permissionErrorResponse;

        private readonly ExceptionResponseGenerator _requestErrorResponse;



        [Obsolete("Testing ctor")]
        public RequestManager() { }

        public RequestManager(
            Dictionary<string, ResponsePair<T>> responses, 
            ResponseGenerator permissionErrorResponse, 
            ExceptionResponseGenerator requestErrorResponse)
        {
            _responses = responses;
            _permissionErrorResponse = permissionErrorResponse;
            _requestErrorResponse = requestErrorResponse;
        }


        public virtual string GetResponse(string request, Connection<T> connection)
        {
            JToken jrequest;

            ResponseGenerator generator;
            var args = new Dictionary<string, dynamic>();

            try
            {
                jrequest = JToken.Parse(request);

                try
                {
                    var pair = _responses[jrequest["type"].ToString()];

                    generator = pair.Groups.Any(g => connection.Account?.Groups.Contains(g) ?? g == "")
                        ? _responses[jrequest["type"].ToString()].ResponseGenerator
                        : _permissionErrorResponse;

                    args = jrequest["args"]?.ToObject<Dictionary<string, dynamic>>(Serializer.Current)
                           ?? new Dictionary<string, dynamic>();
                }
                catch (KeyNotFoundException ex)
                {
                    generator = _requestErrorResponse(ex);
                }
            }
            catch (JsonReaderException ex)
            {
                generator = _requestErrorResponse(ex);
            }
            
            return JToken.FromObject(
                generator(args, connection) ?? new Dictionary<string, dynamic>(),
                Serializer.Current)
                .ToString();
        }



        [Serializable]
        public class InvalidRequestException : Exception
        {
            public string ReceivedData { get; set; }



            public InvalidRequestException(string receivedData)
            {
                ReceivedData = receivedData;
            }

            public InvalidRequestException(string receivedData, string message) : base(message)
            {
                ReceivedData = receivedData;
            }

            public InvalidRequestException(string receivedData, string message, Exception inner) : base(message, inner)
            {
                ReceivedData = receivedData;
            }

            protected InvalidRequestException(
                SerializationInfo info,
                StreamingContext context) : base(info, context)
            {
            }
        }
    }
}
