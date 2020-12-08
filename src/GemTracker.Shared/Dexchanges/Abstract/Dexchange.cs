using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Domain.Enums;
using GemTracker.Shared.Domain.Statics;
using System.Collections.Generic;

namespace GemTracker.Shared.Dexchanges.Abstract
{
    public abstract class Dexchange
    {
        public string StorageFilePath { get; private set; }
        public string StorageFilePathDeleted { get; private set; }
        public string StorageFilePathAdded { get; private set; }
        public void SetPaths(string storagePath, DexType dexType)
        {
            StorageFilePath = PathTo.All(dexType, storagePath);
            StorageFilePathDeleted = PathTo.Deleted(dexType, storagePath);
            StorageFilePathAdded = PathTo.Added(dexType, storagePath);
        }
        public IEnumerable<Gem> CheckDeleted(IEnumerable<Token> oldList, IEnumerable<Token> newList, TokenActionType tokenActionType)
            => DexTokenCompare.DeletedTokens(oldList, newList, tokenActionType);
        public IEnumerable<Gem> CheckAdded(IEnumerable<Token> oldList, IEnumerable<Token> newList, TokenActionType tokenActionType)
            => DexTokenCompare.AddedTokens(oldList, newList, tokenActionType);
    }
}