using System;

namespace HodlWallet.Core.Models
{
    public class BackupWordModel: IEquatable<BackupWordModel>
    {
        public string WordIndex { get; set; }
        public string Word { get; set; }

        public bool Equals(BackupWordModel other)
        {
            if (other is null)
                return false;

            return this.WordIndex == other.WordIndex && this.Word == other.Word;
        }

        public override bool Equals(object obj) => Equals(obj as BackupWordModel);
        public override int GetHashCode() => (WordIndex, Word).GetHashCode();
    }
}
