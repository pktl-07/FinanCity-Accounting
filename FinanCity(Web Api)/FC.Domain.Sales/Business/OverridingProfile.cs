namespace FC.Domain.Sales.Business
{
    using System;
    using System.Collections.Generic;
    using FC.Core.Data;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;

    public class OverridingProfile : BusinessLogic<OverridingProfileEntity, OverridingProfileDAO>
    {
        #region Methods

        public IEnumerable<RankingTierDTO> GetTireRank(OverridingProfileEntity overrideProfile,
            OverridingSchemeEntity overrideScheme)
        {
            try
            {
                var tireRank = new List<RankingTierDTO>();

                Func<string, List<int>> InvokeLevel = (filter) =>
                {
                    var result = new List<int> { 0 };

                    if (!string.IsNullOrEmpty(filter))
                    {
                        var array = filter.Split(',');

                        foreach (var v in array)
                        {
                            result.Add(int.Parse(v));
                        }
                    }

                    return result;
                };

                var matchScheme = InvokeLevel(overrideProfile.UpTo);
                if (matchScheme.Contains(1))
                {
                    tireRank.Add(new RankingTierDTO
                    {
                        TireId = 1,
                        Rank = overrideProfile.L1Rank,
                        TireSK = overrideProfile.L1SK,
                        Percentage = overrideScheme.L1 ?? 0.0M,
                    });
                }

                if (matchScheme.Contains(2))
                {
                    tireRank.Add(new RankingTierDTO
                    {
                        TireId = 2,
                        Rank = overrideProfile.L2Rank,
                        TireSK = overrideProfile.L2SK,
                        Percentage = overrideScheme.L2 ?? 0.0M,
                    });
                }

                if (matchScheme.Contains(3))
                {
                    tireRank.Add(new RankingTierDTO
                    {
                        TireId = 3,
                        Rank = overrideProfile.L3Rank,
                        TireSK = overrideProfile.L3SK,
                        Percentage = overrideScheme.L3 ?? 0.0M,
                    });
                }

                if (matchScheme.Contains(4))
                {
                    tireRank.Add(new RankingTierDTO
                    {
                        TireId = 4,
                        Rank = overrideProfile.L4Rank,
                        TireSK = overrideProfile.L4SK,
                        Percentage = overrideScheme.L4 ?? 0.0M,
                    });
                }

                if (matchScheme.Contains(5))
                {
                    tireRank.Add(new RankingTierDTO
                    {
                        TireId = 5,
                        Rank = overrideProfile.L5Rank,
                        TireSK = overrideProfile.L5SK,
                        Percentage = overrideScheme.L5 ?? 0.0M,
                    });
                }

                return tireRank;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
