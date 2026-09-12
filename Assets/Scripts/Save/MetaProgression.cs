using System.Collections.Generic;

namespace Anubis.Save
{
    public sealed class MetaProgression
    {
        readonly SaveService _save;

        public MetaProgressionData Data => _save.Current.Meta;

        public MetaProgression(SaveService save)
        {
            _save = save;
        }

        public void RegisterRunStarted()
        {
            Data.RunsStarted++;
            _save.Write();
        }

        public void RegisterArenaCleared()
        {
            Data.ArenasCleared++;
            if (Data.ArenasCleared % 3 == 0)
            {
                Data.PermanentHealthBonus += 5;
            }

            _save.Write();
        }

        public void RegisterDeath()
        {
            Data.Deaths++;
            _save.Write();
        }

        public void RegisterBlessing(string id)
        {
            AddUnique(Data.BlessingsChosen, id);
            AddUnique(Data.BlessingsSeen, id);
            _save.Write();
        }

        public void RegisterBlessingsSeen(IEnumerable<string> ids)
        {
            foreach (var id in ids)
            {
                AddUnique(Data.BlessingsSeen, id);
            }

            _save.Write();
        }

        static void AddUnique(List<string> list, string value)
        {
            if (!string.IsNullOrEmpty(value) && !list.Contains(value))
            {
                list.Add(value);
            }
        }
    }
}
