using GemTracker.Shared.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GemTracker.Shared.Domain.Statics
{
    public static class DexTokenCompare
    {
        public static IEnumerable<Gem> DeletedTokens(IEnumerable<Token> oldList, IEnumerable<Token> newList)
        {
            var recentlyDeleted = new List<Gem>();

            var deletedFromDex = oldList
                    .Where(p => newList
                    .All(p2 => p2.Id != p.Id))
                    .ToList();

            if (deletedFromDex.Count() > 0)
            {
                foreach (var item in deletedFromDex)
                {
                    var gem = new Gem
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Symbol = item.Symbol,
                        Recently = TokenAction.DELETED,
                        Date = DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                        IsPublished = false
                    };
                    recentlyDeleted.Add(gem);
                }
            }
            return recentlyDeleted;
        }

        public static IEnumerable<Gem> AddedTokens(IEnumerable<Token> oldList, IEnumerable<Token> newList)
        {
            var recentlyAdded = new List<Gem>();

            var addedToDex = newList
                    .Where(p => oldList
                    .All(p2 => p2.Id != p.Id))
                    .ToList();

            if (addedToDex.Count() > 0)
            {
                foreach (var item in addedToDex)
                {
                    var gem = new Gem
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Symbol = item.Symbol,
                        Recently = TokenAction.ADDED,
                        Date = DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                        IsPublished = false
                    };
                    recentlyAdded.Add(gem);
                }
            }
            return recentlyAdded;
        }
    }
}