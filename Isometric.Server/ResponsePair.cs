namespace Isometric.Server
{
    public class ResponsePair<T>
    {
        public string[] Groups { get; set; }
        
        public RequestManager<T>.ResponseGenerator ResponseGenerator { get; set; }



        public ResponsePair(RequestManager<T>.ResponseGenerator responseGenerator, params string[] groups)
        {
            Groups = groups;
            ResponseGenerator = responseGenerator;
        } 
    }
}