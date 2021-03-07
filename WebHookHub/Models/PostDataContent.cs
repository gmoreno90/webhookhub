namespace WebHookHub.Models
{
    /// <summary>
    /// Post Data Content
    /// </summary>
    public class PostDataContent
    {
        /// <summary>
        /// EventCode
        /// </summary>
        public string EventCode { get; set; }
        /// <summary>
        /// ClientCode
        /// </summary>
        public string ClientCode { get; set; }
        /// <summary>
        /// PostData
        /// </summary>
        public string PostData { get; set; }
        /// <summary>
        /// ContentType
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// Delay Mode
        /// </summary>
        public DelayModeEnum DelayMode { get; set; }
        /// <summary>
        /// Delay Value
        /// </summary>
        public double? DelayValue { get; set; }
        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string strReturn = "";
            strReturn += "Event Code: " + EventCode + "\n";
            strReturn += "Client Code: " + ClientCode + "\n";
            strReturn += "Post Data: " + PostData + "\n";
            strReturn += "Content Type: " + ContentType + "\n";
            return strReturn;
        }
    }
}
