using System;

namespace HodlWallet2.Core.Interactions
{
    /// <summary>
    /// This asks the user a kind of question, we need to match
    /// the question `QuestionKey` to a key specified in a view.
    ///
    /// e.g.:
    /// var request = new YesNoQuestion
    /// {
    ///     QuestionKey = "wipe-wallet",
    ///     AnswerCallback = (yes) =>
    ///     {
    ///         if (yes) _WalletService.DestroyWallet(true);
    ///     }
    /// };
    /// </summary>
    public class YesNoQuestion
    {
        public string QuestionKey { get; set; }
        public Action<bool> AnswerCallback { get; set; }
    }
}
