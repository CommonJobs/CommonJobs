namespace Epnuke.OAuth
{
    public enum OAuthRequestError
    {
        InvalidVersion,
        NoTimestamp,
        InvalidTimestamp,
        InvalidNonce,
        InvalidSignatureMethod,
        InvalidSignature,
        DuplicatedConsumerKey,
        DuplicatedSignature,
        DuplicatedSignatureMethod,
        DuplicatedNonce,
        DuplicatedTimestamp,
        DuplicatedToken,
        DuplicatedVersion,
        DuplicateCallback
    }
}