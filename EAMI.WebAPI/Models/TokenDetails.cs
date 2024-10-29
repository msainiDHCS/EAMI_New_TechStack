namespace EAMI.WebApi.Models
{
    /// <summary>
    /// TokenDetails
    /// </summary>
    public class TokenDetails
    {
        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        public string Access_token { get; set; }
        /// <summary>
        /// Gets or sets the expires in.
        /// </summary>
        /// <value>
        /// The expires in.
        /// </value>
        public int? Expires_in { get; set; }
        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        /// <value>
        /// The refresh token.
        /// </value>
        public string Refresh_token { get; set; }
        /// <summary>
        /// Gets or sets the token unique identifier.
        /// </summary>
        /// <value>
        /// The token unique identifier.
        /// </value>
        public string Token_guid { get; set; }
    }
}
