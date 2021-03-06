/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

namespace SkillPrestige.Bonuses.TypeRegistration
{
    public interface IBonusTypeRegistration
    {
        /// <summary>
        /// This call will 'register' available bonus types with the bonus type class.
        /// </summary>
        void RegisterBonusTypes();
    }
}